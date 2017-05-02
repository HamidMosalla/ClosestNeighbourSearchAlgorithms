using System.Collections.Generic;
using ClosestNeighbourSearchAlgorithms.ClosestPairOfPointSweepLine;

namespace ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation
{
    public class PointCluster
    {
        public PointCluster()
        {
            this.PointClusterCoordinates = new List<CoordinateClass>();
        }

        public int? IdPlaceTree { get; set; }
        public List<CoordinateClass> PointClusterCoordinates { get; set; }
    }
}