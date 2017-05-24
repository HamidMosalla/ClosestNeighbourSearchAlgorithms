using System;
using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.Contracts;
using ClosestNeighbourSearchAlgorithms.Utilities;

namespace ClosestNeighbourSearchAlgorithms
{
    /// <summary>
    /// Represents a KD-Tree. KD-Trees are used for fast spatial searches. Searching in a
    /// balanced KD-Tree is O(log n) where linear search is O(n).
    /// </summary>
    [Serializable]
    public class KDTreeCoordinate<TDimension> where TDimension : IComparable<TDimension>, ICoordinate, new()
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
        public TDimension[] InternalTreeOfPoints { get; }

        /// <summary>
        /// The metric function used to calculate distance between points.
        /// </summary>
        public Func<TDimension, TDimension, double> Metric { get; set; }

        #region Not Used
        /// <summary>
        /// Gets a <see cref="BinaryTreeNavigatorCoordinate{TDimension}"/> that allows for manual tree navigation,
        /// </summary>
        public BinaryTreeNavigatorCoordinate<TDimension> NavigatorCoordinate => new BinaryTreeNavigatorCoordinate<TDimension>(this.InternalTreeOfPoints);
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
        /// Initializes a new instance of the <see cref="KDTreeCoordinate{TDimension}"/> class.
        /// </summary>
        /// <param name="dimensions">The number of dimensions in the data set.</param>
        /// <param name="points">The points to be constructed into a <see cref="KDTreeCoordinate{TDimension}"/></param>
        /// <param name="nodes">The nodes associated with each point.</param>
        /// <param name="metric">The metric function which implicitly defines the metric space in which the KDTree operates in. This should satisfy the triangle inequality.</param>
        /// <param name="searchWindowMinValue">The minimum value to be used in node searches. If null, we assume that <typeparamref name="TDimension"/> has a static field named "MinValue". All numeric structs have this field.</param>
        /// <param name="searchWindowMaxValue">The maximum value to be used in node searches. If null, we assume that <typeparamref name="TDimension"/> has a static field named "MaxValue". All numeric structs have this field.</param>
        public KDTreeCoordinate(
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
            this.InternalTreeOfPoints = Enumerable.Repeat(default(TDimension), elementCount).ToArray();
            this.Metric = metric;
            this.Count = points.Length;
            this.GenerateTree(0, 0, points);
        }

        /// <summary>
        /// Finds the nearest neighbors in the <see cref="KDTreeCoordinate{TDimension}"/> of the given <paramref name="point"/>.
        /// </summary>
        /// <param name="targetPoint">The point whose neighbors we search for.</param>
        /// <param name="neighbors">The number of neighbors to look for.</param>
        /// <returns>The</returns>
        public List<TDimension> NearestNeighborsLinear(TDimension targetPoint, int neighbors)
        {
            var nearestNeighbors = new BoundedPriorityListCoordinate<int, double>(neighbors, true);
            var rect = HyperRectCoordinate<TDimension>.Infinite(this.Dimensions, this.MaxValue, this.MinValue);
            this.SearchForNearestNeighbors(0, targetPoint, rect, 0, nearestNeighbors, double.MaxValue);

            return nearestNeighbors.ToResultSet(this);
        }

        /// <summary>
        /// Searches for the closest points in a hyper-sphere around the given center.
        /// </summary>
        /// <param name="center">The center of the hyper-sphere</param>
        /// <param name="radius">The radius of the hyper-sphere</param>
        /// <param name="neighboors">The number of neighbors to return.</param>
        /// <returns>The specified number of closest points in the hyper-sphere</returns>
        public List<TDimension> NearestNeighborsRadial(TDimension center, double radius, int neighboors = -1)
        {
            var nearestNeighbors = new BoundedPriorityListCoordinate<int, double>(neighboors == -1 ? this.Count : neighboors, false);
            var rect = HyperRectCoordinate<TDimension>.Infinite(this.Dimensions, this.MaxValue, this.MinValue);
            this.SearchForNearestNeighbors(0, center, rect, 0, nearestNeighbors, radius);

            return nearestNeighbors.ToResultSetRadial(this, neighboors);
        }


        /// <summary>
        /// Finds the nearest neighbors in the <see cref="KDTreeCoordinate{TDimension}"/> and returns them.
        /// </summary>
        /// <param name="pointsPerCluster">The number of points per cluster of neighbors.</param>
        /// <param name="coordinates">All the coordinates that will be turned to neighbors</param>
        /// <returns>IEnumerable<List<TDimension>></returns>
        public IEnumerable<List<TDimension>> NearestNeighborClusterLinear(int pointsPerCluster, TDimension[] coordinates)
        {
            var coordinateSet = coordinates.ToHashSet();

            while (coordinateSet.Any())
            {
                var seed = coordinateSet.First();

                var closestCoordinates = NearestNeighborsLinear(seed, pointsPerCluster);

                closestCoordinates.ForEach(c => coordinateSet.Remove(c));

                yield return closestCoordinates;
            }
        }

        /// <summary>
        /// Finds the nearest neighbors in the <see cref="KDTreeCoordinate{TDimension}"/> and returns them.
        /// </summary>
        /// <param name="pointsPerCluster">The number of points per cluster of neighbors.</param>
        /// <param name="coordinates">All the coordinates that will be turned to neighbors</param>
        /// <returns>IEnumerable<List<TDimension>></returns>
        public IEnumerable<List<TDimension>> NearestNeighborClusterRadial(double radius, int pointsPerCluster, TDimension[] coordinates)
        {
            var coordinateSet = coordinates.ToHashSet();
            double baseRadius = radius;
            double radiusGrowthRatio = 1;

            while (coordinateSet.Any())
            {
                var center = coordinateSet.First();

                var closestCoordinates = NearestNeighborsRadial(center, baseRadius, pointsPerCluster);

                if (coordinateSet.Count < pointsPerCluster) closestCoordinates = coordinateSet.ToList();

                if (closestCoordinates == null)
                {
                    radiusGrowthRatio = radiusGrowthRatio * 2;

                    var nextBaseRadius = baseRadius * radiusGrowthRatio;

                    baseRadius = double.IsInfinity(nextBaseRadius) ? double.MaxValue : nextBaseRadius;

                    continue;
                }

                baseRadius = radius;
                radiusGrowthRatio = 1;

                closestCoordinates.ForEach(c => coordinateSet.Remove(c));

                yield return closestCoordinates;
            }
        }

        /// <summary>
        /// Grows a KD tree recursively via median splitting. We find the median by doing a full sort based on latitude.
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
            var sortedPoints = dim == 0 ? points.OrderBy(p => p.Latitude).ToArray() : points.OrderBy(p => p.Longitude).ToArray();

            // get the point which has the median value of the current dimension.
            var medianPoint = sortedPoints[points.Count / 2];
            var medianPointIndex = sortedPoints.Length / 2;

            // The point with the median value all the current dimension now becomes the value of the current tree node
            // The previous node becomes the parents of the current node.
            this.InternalTreeOfPoints[index] = medianPoint;

            // We now split the sorted points into 2 groups
            // 1st group: points before the median
            var leftPoints = new TDimension[medianPointIndex];
            Array.Copy(sortedPoints.ToArray(), leftPoints, leftPoints.Length);

            // 2nd group: Points after the median
            var rightPoints = new TDimension[sortedPoints.Length - (medianPointIndex + 1)];
            Array.Copy(sortedPoints.ToArray(), medianPointIndex + 1, rightPoints, 0, rightPoints.Length);

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
                    this.InternalTreeOfPoints[BinaryTreeNavigationCoordinate.LeftChildIndex(index)] = leftPoints[0];
                }
            }
            else
            {
                this.GenerateTree(BinaryTreeNavigationCoordinate.LeftChildIndex(index), nextDim, leftPoints);
            }

            // Do the same for the right points
            if (rightPoints.Length <= 1)
            {
                if (rightPoints.Length == 1)
                {
                    this.InternalTreeOfPoints[BinaryTreeNavigationCoordinate.RightChildIndex(index)] = rightPoints[0];
                }
            }
            else
            {
                this.GenerateTree(BinaryTreeNavigationCoordinate.RightChildIndex(index), nextDim, rightPoints);
            }
        }

        /// <summary>
        /// A top-down recursive method to find the nearest neighbors of a given point.
        /// </summary>
        /// <param name="nodeIndex">The index of the node for the current recursion branch.</param>
        /// <param name="target">The point whose neighbors we are trying to find.</param>
        /// <param name="rectCoordinate">The <see cref="HyperRectCoordinate{TDimension}"/> containing the possible nearest neighbors.</param>
        /// <param name="dimension">The current splitting dimension for this recursion branch.</param>
        /// <param name="nearestNeighbors">The <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/> containing the nearest neighbors already discovered.</param>
        /// <param name="maxSearchRadiusSquared">The squared radius of the current largest distance to search from the <paramref name="target"/></param>
        private void SearchForNearestNeighbors(
            int nodeIndex,
            TDimension target,
            HyperRectCoordinate<TDimension> rectCoordinate,
            int dimension,
            BoundedPriorityListCoordinate<int, double> nearestNeighbors,
            double maxSearchRadiusSquared)
        {
            if (this.InternalTreeOfPoints.Length <= nodeIndex || nodeIndex < 0 || this.InternalTreeOfPoints[nodeIndex] == null) return;

            // Work out the current dimension
            var dim = dimension % this.Dimensions;

            var leftRect = rectCoordinate.Clone();

            var rightRect = rectCoordinate.Clone();

            if (dim == 0)
            {
                leftRect.MaxPoint.Latitude = this.InternalTreeOfPoints[nodeIndex].Latitude;
                rightRect.MinPoint.Latitude = this.InternalTreeOfPoints[nodeIndex].Latitude;
            }
            if (dim == 1)
            {
                leftRect.MaxPoint.Longitude = this.InternalTreeOfPoints[nodeIndex].Longitude;
                rightRect.MinPoint.Longitude = this.InternalTreeOfPoints[nodeIndex].Longitude;
            }

            // Determine which side the target resides in
            var compare = dim == 0 ? target.Latitude.CompareTo(this.InternalTreeOfPoints[nodeIndex].Latitude)
                                   : target.Longitude.CompareTo(this.InternalTreeOfPoints[nodeIndex].Longitude);

            var nearerRect = compare <= 0 ? leftRect : rightRect;
            var furtherRect = compare <= 0 ? rightRect : leftRect;

            var nearerNode = compare <= 0 ? BinaryTreeNavigationCoordinate.LeftChildIndex(nodeIndex) : BinaryTreeNavigationCoordinate.RightChildIndex(nodeIndex);
            var furtherNode = compare <= 0 ? BinaryTreeNavigationCoordinate.RightChildIndex(nodeIndex) : BinaryTreeNavigationCoordinate.LeftChildIndex(nodeIndex);

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
            distanceSquaredToTarget = this.Metric(this.InternalTreeOfPoints[nodeIndex], target);
            if (distanceSquaredToTarget.CompareTo(maxSearchRadiusSquared) <= 0 && this.InternalTreeOfPoints[nodeIndex].Used == false)
            {
                nearestNeighbors.Add(nodeIndex, distanceSquaredToTarget);
            }
        }
    }

}
