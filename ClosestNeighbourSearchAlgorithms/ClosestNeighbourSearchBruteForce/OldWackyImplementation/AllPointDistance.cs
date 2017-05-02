using System.Collections.Generic;
using ClosestNeighbourSearchAlgorithms.ClosestPairOfPointSweepLine;

namespace ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation
{
    public class AllPointDistance
    {
        public AllPointDistance()
        {
            this.AdjacentPointsDistance = new List<PointDistance>();
        }

        public CoordinateClass Point { get; set; }
        public List<PointDistance> AdjacentPointsDistance { get; set; }
    }
}