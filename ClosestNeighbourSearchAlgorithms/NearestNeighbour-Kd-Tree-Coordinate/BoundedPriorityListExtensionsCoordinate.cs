using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ClosestNeighbourSearchAlgorithms.Contracts;

namespace ClosestNeighbourSearchAlgorithms
{
    /// <summary>
    /// Contains extension methods for <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/> class.
    /// </summary>
    public static class BoundedPriorityListExtensionsCoordinate
    {
        /// <summary>
        /// Takes a <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes.
        /// </summary>
        /// <param name="listCoordinate">The <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/>.</param>
        /// <param name="treeCoordinate">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTreeCoordinate{TDimension}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTreeCoordinate{TDimension}"/> implicitly referenced by the <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TDimension> ToResultSet<TPriority, TDimension>(this BoundedPriorityListCoordinate<int, TPriority> listCoordinate, KDTreeCoordinate<TDimension> treeCoordinate)
           where TDimension : IComparable<TDimension>, ICoordinate, new() where TPriority : IComparable<TPriority>
        {
            var array = new List<TDimension>();
            for (var i = 0; i < listCoordinate.Count; i++)
            {
                array.Add(treeCoordinate.InternalTreeOfPoints[listCoordinate[i]]);
                treeCoordinate.InternalTreeOfPoints[listCoordinate[i]].Used = true;
            }

            return array;
        }

        /// <summary>
        /// Takes a <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes and set the used points property to true.
        /// </summary>
        /// <param name="listCoordinate">The <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/>.</param>
        /// <param name="treeCoordinate">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTreeCoordinate{TDimension}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTreeCoordinate{TDimension}"/> implicitly referenced by the <see cref="BoundedPriorityListCoordinate{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TDimension> ToResultSetRadial<TPriority, TDimension>(this BoundedPriorityListCoordinate<int, TPriority> listCoordinate, KDTreeCoordinate<TDimension> treeCoordinate, int pointsPerCluster)
           where TDimension : IComparable<TDimension>, ICoordinate, new() where TPriority : IComparable<TPriority>
        {
            if (listCoordinate.Count < pointsPerCluster) return null;

            var clusterOfCoordinates = new List<TDimension>();

            foreach (int t in listCoordinate)
            {
                clusterOfCoordinates.Add(treeCoordinate.InternalTreeOfPoints[t]);
                treeCoordinate.InternalTreeOfPoints[t].Used = true;
            }

            return clusterOfCoordinates;
        }
    }
}