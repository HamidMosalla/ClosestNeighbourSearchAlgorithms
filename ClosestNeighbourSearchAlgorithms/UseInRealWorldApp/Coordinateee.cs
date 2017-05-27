using System;
using ClosestNeighbourSearchAlgorithms.Contracts;

namespace CapillarySale.BLL.Admin.ClosestNeighbourFinder.NewAlgorithm.Models
{
    public class Coordinatee : IComparable<Coordinate>, IEquatable<Coordinate>, ICoordinate
    {
        public Guid CoordinateId { get; set; }
        public decimal? PotansielAvalie { get; set; }
        public int TartibInRoad { get; set; }
        public int? IdPlaceTree { get; set; }
        public string CoordinateName { get; set; }
        public string CoordinateAddress { get; set; }
        public string CoordinateFaaliatName { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Used { get; set; }

        public override string ToString() => $"Id: {CoordinateId}; X: {Latitude}; Y: {Longitude}";

        public override bool Equals(object obj)
        {
            Coordinate c = obj as Coordinate;

            if (c?.CoordinateId == CoordinateId)
                return true;

            return false;
        }

        public bool Equals(Coordinate other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return CoordinateId == other.CoordinateId && Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = (int)2166136261;
                hash = (hash * 16777619) ^ CoordinateId.GetHashCode();
                hash = (hash * 16777619) ^ Latitude.GetHashCode();
                hash = (hash * 16777619) ^ Longitude.GetHashCode();
                return hash;
            }
        }

        public static bool operator ==(Coordinate lhs, Coordinate rhs)
        {
            if (ReferenceEquals(lhs, null) && ReferenceEquals(rhs, null)) return true;

            return Equals(lhs, rhs);

            //less readable
            //if (!ReferenceEquals(lhs, null)) return lhs.Equals(rhs);
            //return ReferenceEquals(rhs, null);
        }

        public static bool operator !=(Coordinate lhs, Coordinate rhs) => !(lhs == rhs);

        public double Distance(Coordinate otherCoordinate)
        {
            var dx = otherCoordinate.Latitude - this.Latitude;
            var dy = otherCoordinate.Longitude - this.Longitude;

            var dist = (dx * dx) + (dy * dy);
            dist = Math.Sqrt(dist);

            return dist;
        }

        public int CompareTo(Coordinate obj)
        {
            Coordinate other = obj;

            //compares latitudes
            if (this.Latitude < other?.Latitude) return -1;
            if (this.Latitude > other?.Latitude) return 1;

            return 0;
        }
    }
}