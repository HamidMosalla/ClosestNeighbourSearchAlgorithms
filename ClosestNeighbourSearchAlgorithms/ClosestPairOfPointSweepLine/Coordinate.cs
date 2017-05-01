﻿using System;
using System.Collections.Generic;

namespace ClosestNeighbourSearchAlgorithms.ClosestPairOfPointSweepLine
{
    public interface ICoordinate
    {
        long CoordinateId { get; set; }
        double Latitude { get; set; }
        double Longitude { get; set; }
        string ToString();
        bool Equals(object obj);
        int GetHashCode();
        double Distance(Coordinate otherCoordinate);
        int CompareTo(Coordinate other);
    }

    public class Coordinate : IComparable<Coordinate>, ICoordinate
    {
        public long CoordinateId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public override string ToString() => $"Id: {CoordinateId}; X: {Latitude}; Y: {Longitude}";

        public override bool Equals(object obj)
        {
            var other = obj as Coordinate;
            return CoordinateId == other?.CoordinateId;
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

        public int CompareTo(Coordinate other)
        {
            if (other == null) return -1;

            //compares latitudes
            if (this.Latitude < other.Latitude) return -1;
            if (this.Latitude > other.Latitude) return 1;

            return 0;
        }



        public class XComparer : IComparer<Coordinate>
        {
            public int Compare(Coordinate thisCoordinate, Coordinate otherCoordinate) => XCompare(thisCoordinate, otherCoordinate);

            public static int XCompare(Coordinate thisCoordinate, Coordinate otherCoordinate)
            {
                if (thisCoordinate.Longitude < otherCoordinate.Longitude) return -1;
                if (thisCoordinate.Longitude > otherCoordinate.Longitude) return 1;
                if (thisCoordinate.Latitude < otherCoordinate.Latitude) return -1;
                if (thisCoordinate.Latitude > otherCoordinate.Latitude) return 1;

                return 0;
            }
        }

        public class YComparer : IComparer<Coordinate>
        {
            public int Compare(Coordinate thisCoordinate, Coordinate otherCoordinate) => YCompare(thisCoordinate, otherCoordinate);

            public static int YCompare(Coordinate thisCoordinate, Coordinate otherCoordinate)
            {
                if (thisCoordinate.Longitude < otherCoordinate.Longitude) return -1;
                if (thisCoordinate.Longitude > otherCoordinate.Longitude) return 1;
                if (thisCoordinate.Latitude < otherCoordinate.Latitude) return -1;
                if (thisCoordinate.Latitude > otherCoordinate.Latitude) return 1;

                return 0;
            }
        }
    }
}