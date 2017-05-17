using System;
using System.Runtime.CompilerServices;

namespace ClosestNeighbourSearchAlgorithms
{
    /// <summary>
    /// Contains extension methods for <see cref="BoundedPriorityPristineList{TElement,TPriority}"/> class.
    /// </summary>
    public static class BoundedPriorityListPristineExtensions
    {
        /// <summary>
        /// Takes a <see cref="BoundedPriorityPristineList{TElement,TPriority}"/> storing the indexes of the points and nodes of a KDTree
        /// and returns the points and nodes.
        /// </summary>
        /// <param name="pristineList">The <see cref="BoundedPriorityPristineList{TElement,TPriority}"/>.</param>
        /// <param name="treePristine">The</param>
        /// <typeparam name="TPriority">THe type of the priority of the <see cref="BoundedPriorityPristineList{TElement,TPriority}"/></typeparam>
        /// <typeparam name="TDimension">The type of the dimensions of the <see cref="KDTreePristine{TDimension,TNode}"/></typeparam>
        /// <typeparam name="TNode">The type of the nodes of the <see cref="KDTreePristine{TDimension,TNode}"/></typeparam>
        /// <returns>The points and nodes in the <see cref="KDTreePristine{TDimension,TNode}"/> implicitly referenced by the <see cref="BoundedPriorityPristineList{TElement,TPriority}"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Tuple<TDimension[], TNode>[] ToResultSet<TPriority, TDimension, TNode>(
           this BoundedPriorityPristineList<int, TPriority> pristineList,
           KDTreePristine<TDimension, TNode> treePristine)
           where TDimension : IComparable<TDimension>
           where TPriority : IComparable<TPriority>
        {
            var array = new Tuple<TDimension[], TNode>[pristineList.Count];
            for (var i = 0; i < pristineList.Count; i++)
            {
                array[i] = new Tuple<TDimension[], TNode>(
                    treePristine.InternalPointArray[pristineList[i]],
                    treePristine.InternalNodeArray[pristineList[i]]);
            }

            return array;
        }
    }
}
