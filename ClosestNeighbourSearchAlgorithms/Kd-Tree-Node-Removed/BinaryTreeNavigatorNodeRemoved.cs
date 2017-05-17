﻿using System;

namespace ClosestNeighbourSearchAlgorithms
{
    /// <summary>
    /// Allows one to navigate a binary tree stored in an <see cref="Array"/> using familiar
    /// tree navigation concepts.
    /// </summary>
    /// <typeparam name="TPoint">The type of the individual points.</typeparam>
    /// <typeparam name="TNode">The type of the individual nodes.</typeparam>
    public class BinaryTreeNavigatorNodeRemoved<TPoint>
    {
        /// <summary>
        /// A reference to the pointArray in which the binary tree is stored in.
        /// </summary>
        private readonly TPoint[] pointArray;

        /// <summary>
        /// The index in the pointArray that the current node resides in.
        /// </summary>
        public int Index { get; }

        #region Not Used

        /// <summary>
        /// The left child of the current node.
        /// </summary>
        public BinaryTreeNavigatorNodeRemoved<TPoint> Left => BinaryTreeNavigationNodeRemoved.LeftChildIndex(this.Index) < this.pointArray.Length - 1 ?
           new BinaryTreeNavigatorNodeRemoved<TPoint>(this.pointArray, BinaryTreeNavigationNodeRemoved.LeftChildIndex(this.Index)) :
            null;

        /// <summary>
        /// The right child of the current node.
        /// </summary>
        public BinaryTreeNavigatorNodeRemoved<TPoint> Right => BinaryTreeNavigationNodeRemoved.RightChildIndex(this.Index) < this.pointArray.Length - 1 ?
           new BinaryTreeNavigatorNodeRemoved<TPoint>(this.pointArray, BinaryTreeNavigationNodeRemoved.RightChildIndex(this.Index)) :
            null;

        /// <summary>
        /// The parent of the current node.
        /// </summary>
        public BinaryTreeNavigatorNodeRemoved<TPoint> Parent => this.Index == 0 ? null : new BinaryTreeNavigatorNodeRemoved<TPoint>(this.pointArray, BinaryTreeNavigationNodeRemoved.ParentIndex(this.Index));

        #endregion

        /// <summary>
        /// The current <typeparamref name="TPoint"/>.
        /// </summary>
        public TPoint Point => this.pointArray[this.Index];

        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryTreeNavigatorNodeRemoved{TPoint}"/> class.
        /// </summary>
        /// <param name="pointArray">The point array backing the binary tree.</param>
        /// <param name="nodeArray">The node array corresponding to the point array.</param>
        /// <param name="index">The index of the node of interest in the pointArray. If not given, the node navigator start at the 0 index (the root of the tree).</param>
        public BinaryTreeNavigatorNodeRemoved(TPoint[] pointArray, int index = 0)
        {
            this.Index = index;
            this.pointArray = pointArray;
        }
    }
}
