using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ClosestNeighbourSearchAlgorithms.Contracts;

namespace ClosestNeighbourSearchAlgorithms
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
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTree{TDimension,TNode}"/></typeparam>
        /// <typeparam name="TNode">The type of the nodes of the <see cref="KDTree{TDimension,TNode}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTree{TDimension,TNode}"/> implicitly referenced by the <see cref="BoundedPriorityList{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TNode> ToResultSet<TPriority, TDimension, TNode>(this BoundedPriorityList<int, TPriority> list, KDTree<TDimension, TNode> tree)
           where TDimension : IComparable<TDimension>
           where TPriority : IComparable<TPriority>
           where TNode : ICoordinate
        {
            var clusterOfCoordinates = new List<TNode>();

            for (var i = 0; i < list.Count; i++)
            {
                tree.InternalNodeArray[list[i]].Used = true;
                clusterOfCoordinates.Add(tree.InternalNodeArray[list[i]]);
            }

            return clusterOfCoordinates;
        }


        /// <summary>
        /// Takes a <see cref="BoundedPriorityList{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes.
        /// </summary>
        /// <param name="list">The <see cref="BoundedPriorityList{TElement,TPriority}"/>.</param>
        /// <param name="tree">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityList{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTree{TDimension,TNode}"/></typeparam>
        /// <typeparam name="TNode">The type of the nodes of the <see cref="KDTree{TDimension,TNode}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTree{TDimension,TNode}"/> implicitly referenced by the <see cref="BoundedPriorityList{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TNode> ToResultSetRadial<TPriority, TDimension, TNode>(this BoundedPriorityList<int, TPriority> list,
                                                                                                    KDTree<TDimension, TNode> tree, int pointsPerCluster)
           where TDimension : IComparable<TDimension>
           where TPriority : IComparable<TPriority>
           where TNode : ICoordinate
        {
            if (list.Count < pointsPerCluster) return null;

            var clusterOfCoordinates = new List<TNode>();

            for (var i = 0; i < list.Count; i++)
            {
                tree.InternalNodeArray[list[i]].Used = true;
                clusterOfCoordinates.Add(tree.InternalNodeArray[list[i]]);
            }

            return clusterOfCoordinates;
        }
    }
}
