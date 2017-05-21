using System;
using System.Runtime.CompilerServices;
using ClosestNeighbourSearchAlgorithms.Contracts;

namespace ClosestNeighbourSearchAlgorithms
{
    /// <summary>
    /// Represents a hyper-rectangle. An N-Dimensional rectangle.
    /// </summary>
    /// <typeparam name="TDimension">The type of "dimension" in the metric space in which the hyper-rectangle lives.</typeparam>
    public struct HyperRectCoordinate<TDimension> where TDimension : IComparable<TDimension>, ICoordinate, new()
    {

        /// <summary>
        /// The minimum point of the hyper-rectangle. One can think of this point as the
        /// bottom-left point of a 2-Dimensional rectangle.
        /// </summary>
        public TDimension MinPoint { get; set; }

        /// <summary>
        /// The maximum point of the hyper-rectangle. One can think of this point as the
        /// top-right point of a 2-Dimensional rectangle.
        /// </summary>
        public TDimension MaxPoint { get; set; }

        /// <summary>
        /// Get a hyper rectangle which spans the entire implicit metric space.
        /// </summary>
        /// <param name="dimensions">The number of dimensions in the hyper-rectangle's metric space.</param>
        /// <param name="positiveInfinity">The smallest possible values in any given dimension.</param>
        /// <param name="negativeInfinity">The largest possible values in any given dimension.</param>
        /// <returns>The hyper-rectangle which spans the entire metric space.</returns>
        public static HyperRectCoordinate<TDimension> Infinite(int dimensions, double positiveInfinity, double negativeInfinity)
        {
            var rect = default(HyperRectCoordinate<TDimension>);

            var minPoint = new TDimension();
            var maxPoint = new TDimension();

            minPoint.Latitude = negativeInfinity;
            minPoint.Longitude = negativeInfinity;

            maxPoint.Latitude = positiveInfinity;
            maxPoint.Longitude = positiveInfinity;

            rect.MinPoint = minPoint;
            rect.MaxPoint = maxPoint;

            return rect;

            //rect.MinPoint = new TDimension();
            //rect.MaxPoint = new TDimension();

            //rect.MinPoint.Latitude = negativeInfinity;
            //rect.MinPoint.Longitude = negativeInfinity;

            //rect.MaxPoint.Latitude = positiveInfinity;
            //rect.MaxPoint.Longitude = positiveInfinity;

            //return rect;
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

            if (this.MinPoint.Latitude.CompareTo(toPoint.Latitude) > 0)
            {
                closest = this.MinPoint;
            }
            else if (this.MaxPoint.Latitude.CompareTo(toPoint.Latitude) < 0)
            {
                closest = this.MaxPoint;
            }
            else if (this.MinPoint.Longitude.CompareTo(toPoint.Longitude) > 0)
            {
                closest = this.MinPoint;
            }
            else if (this.MaxPoint.Longitude.CompareTo(toPoint.Longitude) < 0)
            {
                closest = this.MaxPoint;
            }
            else
            {
                // Point is within rectangle, at least on this dimension
                closest = toPoint;
            }

            return closest;
        }

        /// <summary>
        /// Clones the <see cref="HyperRectCoordinate{TDimension}"/>.
        /// </summary>
        /// <returns>A clone of the <see cref="HyperRectCoordinate{TDimension}"/></returns>
        public HyperRectCoordinate<TDimension> Clone()
        {
            // For a discussion of why we don't implement ICloneable
            // see http://stackoverflow.com/questions/536349/why-no-icloneablet
            var rect = default(HyperRectCoordinate<TDimension>);
            rect.MinPoint = this.MinPoint;
            rect.MaxPoint = this.MaxPoint;
            return rect;
        }
    }
}