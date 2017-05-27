using System.Collections.Generic;

namespace ClosestNeighbourSearchAlgorithms.Models
{
    public class PointCluster
    {
        public PointCluster()
        {
            this.PointClusterCoordinates = new List<Coordinate>();
        }

        public int? IdPlaceTree { get; set; }
        public List<Coordinate> PointClusterCoordinates { get; set; }
    }
}