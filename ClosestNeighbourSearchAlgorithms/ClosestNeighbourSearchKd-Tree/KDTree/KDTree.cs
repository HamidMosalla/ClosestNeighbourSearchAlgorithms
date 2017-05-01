using System;
using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms.KDTree
{
    /// <summary>
    /// Represents a KD-Tree. KD-Trees are used for fast spatial searches. Searching in a
    /// balanced KD-Tree is O(log n) where linear search is O(n).
    /// </summary>
    [Serializable]
    public class KDTree<TDimension> where TDimension : IComparable<TDimension>, ICoordinate, new()
    {
        /// <summary>
        /// The number of points in the KDTree
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// The numbers of dimensions that the tree has.
        /// </summary>
        public int Dimensions { get; }

        /// <summary>
        /// The array in which the binary tree is stored. Enumerating this array is a level-order traversal of the tree.
        /// </summary>
        public TDimension[] InternalPointArray { get; }

        /// <summary>
        /// The metric function used to calculate distance between points.
        /// </summary>
        public Func<TDimension, TDimension, double> Metric { get; set; }

        #region Not Used
        /// <summary>
        /// Gets a <see cref="BinaryTreeNavigator{TPoint}"/> that allows for manual tree navigation,
        /// </summary>
        public BinaryTreeNavigator<TDimension> Navigator => new BinaryTreeNavigator<TDimension>(this.InternalPointArray);
        #endregion

        /// <summary>
        /// The maximum value along any dimension.
        /// </summary>
        private double MaxValue { get; }

        /// <summary>
        /// The minimum value along any dimension.
        /// </summary>
        private double MinValue { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="KDTree{TDimension}"/> class.
        /// </summary>
        /// <param name="dimensions">The number of dimensions in the data set.</param>
        /// <param name="points">The points to be constructed into a <see cref="KDTree{TDimension}"/></param>
        /// <param name="nodes">The nodes associated with each point.</param>
        /// <param name="metric">The metric function which implicitly defines the metric space in which the KDTree operates in. This should satisfy the triangle inequality.</param>
        /// <param name="searchWindowMinValue">The minimum value to be used in node searches. If null, we assume that <typeparamref name="TDimension"/> has a static field named "MinValue". All numeric structs have this field.</param>
        /// <param name="searchWindowMaxValue">The maximum value to be used in node searches. If null, we assume that <typeparamref name="TDimension"/> has a static field named "MaxValue". All numeric structs have this field.</param>
        public KDTree(
            int dimensions,
            TDimension[] points,
            Func<TDimension, TDimension, double> metric,
            double searchWindowMinValue = double.MinValue,
            double searchWindowMaxValue = double.MaxValue)
        {
            // Attempt find the Min/Max value if null.

            this.MinValue = searchWindowMinValue;
            this.MaxValue = searchWindowMaxValue;


            // Calculate the number of nodes needed to contain the binary tree.
            // This is equivalent to finding the power of 2 greater than the number of points
            var elementCount = (int)Math.Pow(2, (int)(Math.Log(points.Length) / Math.Log(2)) + 1);
            this.Dimensions = dimensions;
            this.InternalPointArray = Enumerable.Repeat(default(TDimension), elementCount).ToArray();
            this.Metric = metric;
            this.Count = points.Length;
            this.GenerateTree(0, 0, points);
        }

        /// <summary>
        /// Finds the nearest neighbors in the <see cref="KDTree{TDimension}"/> of the given <paramref name="point"/>.
        /// </summary>
        /// <param name="targetPoint">The point whose neighbors we search for.</param>
        /// <param name="neighbors">The number of neighbors to look for.</param>
        /// <returns>The</returns>
        public List<TDimension> NearestNeighbors(TDimension targetPoint, int neighbors)
        {
            var nearestNeighborList = new BoundedPriorityList<int, double>(neighbors, true);
            var rect = HyperRect<TDimension>.Infinite(this.Dimensions, this.MaxValue, this.MinValue);
            this.SearchForNearestNeighbors(0, targetPoint, rect, 0, nearestNeighborList, double.MaxValue);

            return nearestNeighborList.ToResultSet(this);
        }

        /// <summary>
        /// Finds the nearest neighbors in the <see cref="KDTree{TDimension}"/> of the given <paramref name="point"/>.
        /// </summary>
        /// <param name="targetPoint">The point whose neighbors we search for.</param>
        /// <param name="neighbors">The number of neighbors to look for.</param>
        /// <returns>The</returns>
        public IEnumerable<IEnumerable<TDimension>> NearestNeighborsCollection(int neighbors)
        {
            while (InternalPointArray.Any(a => !a.Equals(default(TDimension))))
            {
                var targetPoint = InternalPointArray.First(a => !a.Equals(default(TDimension)));
                var nodeIndex = InternalPointArray.ToList().FindIndex(i => i.CoordinateId == targetPoint.CoordinateId);

                var nearestNeighborList = new BoundedPriorityList<int, double>(neighbors, true);
                var rect = HyperRect<TDimension>.Infinite(this.Dimensions, this.MaxValue, this.MinValue);
                this.SearchForNearestNeighbors(nodeIndex, targetPoint, rect, 0, nearestNeighborList, double.MaxValue);

                yield return nearestNeighborList.ToResultSetAndRemove(this);
            }
        }

        /// <summary>
        /// Searches for the closest points in a hyper-sphere around the given center.
        /// </summary>
        /// <param name="center">The center of the hyper-sphere</param>
        /// <param name="radius">The radius of the hyper-sphere</param>
        /// <param name="neighboors">The number of neighbors to return.</param>
        /// <returns>The specified number of closest points in the hyper-sphere</returns>
        public List<TDimension> RadialSearch(TDimension center, double radius, int neighboors = -1)
        {
            var nearestNeighbors = new BoundedPriorityList<int, double>(neighboors == -1 ? this.Count : neighboors, false);
            var rect = HyperRect<TDimension>.Infinite(this.Dimensions, this.MaxValue, this.MinValue);
            this.SearchForNearestNeighbors(0, center, rect, 0, nearestNeighbors, radius);

            return nearestNeighbors.ToResultSet(this);
        }

        /// <summary>
        /// Grows a KD tree recursively via median splitting. We find the median by doing a full sort.
        /// </summary>
        /// <param name="index">The array index for the current node.</param>
        /// <param name="dim">The current splitting dimension.</param>
        /// <param name="points">The set of points remaining to be added to the kd-tree</param>
        /// <param name="nodes">The set of nodes RE</param>
        private void GenerateTree(
            int index,
            int dim,
            IReadOnlyCollection<TDimension> points)
        {
            // See wikipedia for a good explanation kd-tree construction.
            // https://en.wikipedia.org/wiki/K-d_tree

            // sort the points along the current dimension
            var sortedPoints = points.OrderBy(z => z.Latitude).ToArray();

            // get the point which has the median value of the current dimension.
            var medianPoint = sortedPoints[points.Count / 2];
            var medianPointIdx = sortedPoints.Length / 2;

            // The point with the median value all the current dimension now becomes the value of the current tree node
            // The previous node becomes the parents of the current node.
            this.InternalPointArray[index] = medianPoint;

            // We now split the sorted points into 2 groups
            // 1st group: points before the median
            var leftPoints = new TDimension[medianPointIdx];
            Array.Copy(sortedPoints.ToArray(), leftPoints, leftPoints.Length);

            // 2nd group: Points after the median
            var rightPoints = new TDimension[sortedPoints.Length - (medianPointIdx + 1)];
            Array.Copy(sortedPoints.ToArray(), medianPointIdx + 1, rightPoints, 0, rightPoints.Length);

            // We new recurse, passing the left and right arrays for arguments.
            // The current node's left and right values become the "roots" for
            // each recursion call. We also forward cycle to the next dimension.
            var nextDim = (dim + 1) % this.Dimensions; // select next dimension

            // We only need to recurse if the point array contains more than one point
            // If the array has no points then the node stay a null value
            if (leftPoints.Length <= 1)
            {
                if (leftPoints.Length == 1)
                {
                    this.InternalPointArray[BinaryTreeNavigation.LeftChildIndex(index)] = leftPoints[0];
                }
            }
            else
            {
                this.GenerateTree(BinaryTreeNavigation.LeftChildIndex(index), nextDim, leftPoints);
            }

            // Do the same for the right points
            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    this.InternalPointArray[BinaryTreeNavigation.RightChildIndex(index)] = rightPoints[0];
                }
            }
            else
            {
                this.GenerateTree(BinaryTreeNavigation.RightChildIndex(index), nextDim, rightPoints);
            }
        }

        /// <summary>
        /// A top-down recursive method to find the nearest neighbors of a given point.
        /// </summary>
        /// <param name="nodeIndex">The index of the node for the current recursion branch.</param>
        /// <param name="target">The point whose neighbors we are trying to find.</param>
        /// <param name="rect">The <see cref="HyperRect{T}"/> containing the possible nearest neighbors.</param>
        /// <param name="dimension">The current splitting dimension for this recursion branch.</param>
        /// <param name="nearestNeighbors">The <see cref="BoundedPriorityList{TElement,TPriority}"/> containing the nearest neighbors already discovered.</param>
        /// <param name="maxSearchRadiusSquared">The squared radius of the current largest distance to search from the <paramref name="target"/></param>
        private void SearchForNearestNeighbors(
            int nodeIndex,
            TDimension target,
            HyperRect<TDimension> rect,
            int dimension,
            BoundedPriorityList<int, double> nearestNeighbors,
            double maxSearchRadiusSquared)
        {
            //if (this.InternalPointArray.Length <= nodeIndex || nodeIndex < 0 || this.InternalPointArray[nodeIndex] == null) { return; }
            if (this.InternalPointArray.Length <= nodeIndex || nodeIndex < 0 || this.InternalPointArray[nodeIndex].CoordinateId == 0) { return; }

            // Work out the current dimension
            var dim = dimension % this.Dimensions;

            // Split our hyper-rectangle into 2 sub rectangles along the current
            // node's point on the current dimension
            var leftRect = rect.Clone();
            leftRect.MaxPoint = this.InternalPointArray[nodeIndex];

            var rightRect = rect.Clone();
            rightRect.MinPoint = this.InternalPointArray[nodeIndex];

            // Determine which side the target resides in
            var compare = target.CompareTo(this.InternalPointArray[nodeIndex]);

            var nearerRect = compare <= 0 ? leftRect : rightRect;
            var furtherRect = compare <= 0 ? rightRect : leftRect;

            var nearerNode = compare <= 0 ? BinaryTreeNavigation.LeftChildIndex(nodeIndex) : BinaryTreeNavigation.RightChildIndex(nodeIndex);
            var furtherNode = compare <= 0 ? BinaryTreeNavigation.RightChildIndex(nodeIndex) : BinaryTreeNavigation.LeftChildIndex(nodeIndex);

            // Move down into the nearer branch
            this.SearchForNearestNeighbors(
                nearerNode,
                target,
                nearerRect,
                dimension + 1,
                nearestNeighbors,
                maxSearchRadiusSquared);

            // Walk down into the further branch but only if our capacity hasn't been reached
            // OR if there's a region in the further rectangle that's closer to the target than our
            // current furtherest nearest neighbor
            var closestPointInFurtherRect = furtherRect.GetClosestPoint(target);
            var distanceSquaredToTarget = this.Metric(closestPointInFurtherRect, target);

            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                if (nearestNeighbors.IsFull)
                {
                    if (distanceSquaredToTarget.CompareTo(nearestNeighbors.MaxPriority) < 0)
                    {
                        this.SearchForNearestNeighbors(
                            furtherNode,
                            target,
                            furtherRect,
                            dimension + 1,
                            nearestNeighbors,
                            maxSearchRadiusSquared);
                    }
                }
                else
                {
                    this.SearchForNearestNeighbors(
                        furtherNode,
                        target,
                        furtherRect,
                        dimension + 1,
                        nearestNeighbors,
                        maxSearchRadiusSquared);
                }
            }

            // Try to add the current node to our nearest neighbors list
            distanceSquaredToTarget = this.Metric(this.InternalPointArray[nodeIndex], target);
            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0)
            {
                nearestNeighbors.Add(nodeIndex, distanceSquaredToTarget);
            }
        }
    }

}
