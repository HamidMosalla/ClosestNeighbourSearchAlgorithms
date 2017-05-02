using System;

namespace ClosestNeighbourSearchAlgorithms.ModelsAndContracts
{
    public struct Coordinate : IComparable<Coordinate>, ICoordinate
    {
        public long CoordinateId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool Used { get; set; }

        public override string ToString() => $"Id: {CoordinateId}; X: {Latitude}; Y: {Longitude}";

        public override bool Equals(object obj)
        {
            Coordinate? c = obj as Coordinate?;

            if (c?.CoordinateId == CoordinateId)
                return true;

            return false;
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
            Coordinate? other = obj as Coordinate?;

            //compares latitudes
            if (this.Latitude < other?.Latitude) return -1;
            if (this.Latitude > other?.Latitude) return 1;

            return 0;
        }
    }
}