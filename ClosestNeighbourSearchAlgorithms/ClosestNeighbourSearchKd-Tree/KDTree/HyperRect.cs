﻿using System;
using System.Runtime.CompilerServices;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms.KDTree
{
    /// <summary>
    /// Represents a hyper-rectangle. An N-Dimensional rectangle.
    /// </summary>
    /// <typeparam name="TDimension">The type of "dimension" in the metric space in which the hyper-rectangle lives.</typeparam>
    public struct HyperRect<TDimension> where TDimension : IComparable<TDimension>, ICoordinate, new()
    {
        /// <summary>
        /// Backing field for the <see cref="MinPoint"/> property.
        /// </summary>
        private TDimension minPoint;

        /// <summary>
        /// Backing field for the <see cref="MaxPoint"/> property.
        /// </summary>
        private TDimension maxPoint;

        /// <summary>
        /// The minimum point of the hyper-rectangle. One can think of this point as the
        /// bottom-left point of a 2-Dimensional rectangle.
        /// </summary>
        public TDimension MinPoint
        {
            get
            {
                return this.minPoint;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.minPoint = new TDimension();
                value = this.minPoint;
            }
        }

        /// <summary>
        /// The maximum point of the hyper-rectangle. One can think of this point as the
        /// top-right point of a 2-Dimensional rectangle.
        /// </summary>
        public TDimension MaxPoint
        {
            get
            {
                return this.maxPoint;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                this.maxPoint = new TDimension();
                value = this.maxPoint;
            }
        }

        /// <summary>
        /// Get a hyper rectangle which spans the entire implicit metric space.
        /// </summary>
        /// <param name="dimensions">The number of dimensions in the hyper-rectangle's metric space.</param>
        /// <param name="positiveInfinity">The smallest possible values in any given dimension.</param>
        /// <param name="negativeInfinity">The largest possible values in any given dimension.</param>
        /// <returns>The hyper-rectangle which spans the entire metric space.</returns>
        public static HyperRect<TDimension> Infinite(int dimensions, double positiveInfinity, double negativeInfinity)
        {
            var rect = default(HyperRect<TDimension>);

            rect.MinPoint = new TDimension();
            rect.MaxPoint = new TDimension();

            rect.MinPoint.Latitude = negativeInfinity;
            rect.MaxPoint.Latitude = positiveInfinity;
            rect.MinPoint.Longitude = negativeInfinity;
            rect.MaxPoint.Longitude = negativeInfinity;

            return rect;
        }

        /// <summary>
        /// Gets the point on the rectangle that is closest to the given point.
        /// If the point is within the rectangle, then the input point is the same as the
        /// output point.f the point is outside the rectangle then the point on the rectangle
        /// that is closest to the given point is returned.
        /// </summary>
        /// <param name="toPoint">We try to find a point in or on the rectangle closest to this point.</param>
        /// <returns>The point on or in the rectangle that is closest to the given point.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TDimension GetClosestPoint(TDimension toPoint)
        {
            var closest = new TDimension();

            if (this.minPoint.Latitude.CompareTo(toPoint.Latitude) > 0)
            {
                closest = this.minPoint;
            }
            else if (this.maxPoint.Latitude.CompareTo(toPoint.Latitude) < 0)
            {
                closest = this.maxPoint;
            }
            else if (this.minPoint.Longitude.CompareTo(toPoint.Longitude) < 0)
            {
                closest = this.minPoint;
            }
            else if (this.maxPoint.Longitude.CompareTo(toPoint.Longitude) < 0)
            {
                closest = this.maxPoint;
            }
            else
            {
                // Point is within rectangle, at least on this dimension
                closest = toPoint;
            }

            return closest;
        }

        /// <summary>
        /// Clones the <see cref="HyperRect{T}"/>.
        /// </summary>
        /// <returns>A clone of the <see cref="HyperRect{T}"/></returns>
        public HyperRect<TDimension> Clone()
        {
            // For a discussion of why we don't implement ICloneable
            // see http://stackoverflow.com/questions/536349/why-no-icloneablet
            var rect = default(HyperRect<TDimension>);
            rect.MinPoint = this.MinPoint;
            rect.MaxPoint = this.MaxPoint;
            return rect;
        }
    }
}