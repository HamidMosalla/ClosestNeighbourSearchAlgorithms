using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ClosestNeighbourSearchAlgorithms
{
    /// <summary>
    /// Contains extension methods for <see cref="BoundedPriorityListNodeRemoved{TElement,TPriority}"/> class.
    /// </summary>
    public static class BoundedPriorityListNodeRemovedExtensions
    {
        /// <summary>
        /// Takes a <see cref="BoundedPriorityListNodeRemoved{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes.
        /// </summary>
        /// <param name="listNodeRemoved">The <see cref="BoundedPriorityListNodeRemoved{TElement,TPriority}"/>.</param>
        /// <param name="treeNodeRemoved">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityListNodeRemoved{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTreeNodeRemoved{TDimension}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTreeNodeRemoved{TDimension}"/> implicitly referenced by the <see cref="BoundedPriorityListNodeRemoved{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<TDimension[]> ToResultSet<TPriority, TDimension>(this BoundedPriorityListNodeRemoved<int, TPriority> listNodeRemoved, KDTreeNodeRemoved<TDimension> treeNodeRemoved)
           where TDimension : IComparable<TDimension>
           where TPriority : IComparable<TPriority>
        {
            var array = new List<TDimension[]>();
            for (var i = 0; i < listNodeRemoved.Count; i++)
            {
                array.Add(treeNodeRemoved.InternalPointArray[listNodeRemoved[i]]);
            }

            return array;
        }
    }
}
