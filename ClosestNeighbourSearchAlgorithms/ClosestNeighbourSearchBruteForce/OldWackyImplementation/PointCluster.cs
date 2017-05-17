using System.Collections.Generic;

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