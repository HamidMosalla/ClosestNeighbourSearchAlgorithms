using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms.KDTree
{
    /// <summary>
    /// Contains extension methods for <see cref="BoundedPriorityList{TElement,TPriority}"/> class.
    /// </summary>
    public static class BoundedPriorityListExtensions
    {
        /// <summary>
        /// Takes a <see cref="BoundedPriorityList{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes.
        /// </summary>
        /// <param name="list">The <see cref="BoundedPriorityList{TElement,TPriority}"/>.</param>
        /// <param name="tree">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityList{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTree{TDimension}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTree{TDimension}"/> implicitly referenced by the <see cref="BoundedPriorityList{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TDimension> ToResultSet<TPriority, TDimension>(this BoundedPriorityList<int, TPriority> list, KDTree<TDimension> tree)
           where TDimension : IComparable<TDimension>, ICoordinate, new() where TPriority : IComparable<TPriority>
        {
            var array = new List<TDimension>();
            for (var i = 0; i < list.Count; i++)
            {
                array.Add(tree.InternalTreeOfPoints[list[i]]);
                tree.InternalTreeOfPoints[list[i]].Used = true;
            }

            return array;
        }

        /// <summary>
        /// Takes a <see cref="BoundedPriorityList{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes and set the used points property to true.
        /// </summary>
        /// <param name="list">The <see cref="BoundedPriorityList{TElement,TPriority}"/>.</param>
        /// <param name="tree">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityList{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTree{TDimension}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTree{TDimension}"/> implicitly referenced by the <see cref="BoundedPriorityList{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TDimension> ToResultSetRadial<TPriority, TDimension>(this BoundedPriorityList<int, TPriority> list, KDTree<TDimension> tree, int pointsPerCluster)
           where TDimension : IComparable<TDimension>, ICoordinate, new() where TPriority : IComparable<TPriority>
        {
            if (list.Count < pointsPerCluster) return null;

            var clusterOfCoordinates = new List<TDimension>();

            foreach (int t in list)
            {
                clusterOfCoordinates.Add(tree.InternalTreeOfPoints[t]);
                tree.InternalTreeOfPoints[t].Used = true;
            }

            return clusterOfCoordinates;
        }
    }
}