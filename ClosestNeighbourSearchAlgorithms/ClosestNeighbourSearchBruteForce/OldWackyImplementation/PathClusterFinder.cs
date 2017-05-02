using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestPairOfPointSweepLine;

namespace ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation
{
    public class PathClusterFinder
    {
        private readonly IEnumerable<CoordinateClass> _coordinates;
        private readonly List<AllPointDistance> _allPointDistances;
        private readonly List<PointCluster> _pointClusters;

        public PathClusterFinder(IEnumerable<CoordinateClass> coordinates)
        {
            _coordinates = coordinates;
            _allPointDistances = new List<AllPointDistance>();
            _pointClusters = new List<PointCluster>();
            CalculateAllPointDistancesPerPoint();
        }

        public static double GetDistance(CoordinateClass origin, CoordinateClass destination)
        {
            var dx = destination.Latitude - origin.Latitude;
            var dy = destination.Longitude - origin.Longitude;

            var dist = (dx * dx) + (dy * dy);
            dist = Math.Sqrt(dist);

            return dist;
        }

        private void CalculateAllPointDistancesPerPoint()
        {
            foreach (var item in _coordinates)
            {
                var allPointDistance = new AllPointDistance { Point = item };

                foreach (var subItem in _coordinates.Where(subItem => subItem.CoordinateId != item.CoordinateId))
                {
                    allPointDistance.AdjacentPointsDistance.Add(new PointDistance { AdjasantPoint = subItem, Distance = GetDistance(origin: item, destination: subItem) });
                }

                _allPointDistances.Add(allPointDistance);
            }
        }

        private List<CoordinateClass> GetPointCluster(int? pointsPerCluster, CoordinateClass currentPoint)
        {
            var pointDistance = _allPointDistances.FirstOrDefault(a => a.Point.CoordinateId == currentPoint.CoordinateId);

            var pathBatch = _pointClusters.SelectMany(a => a.PointClusterCoordinates).ToList();

            RemoveAllExistingAdjacentPointsDistances(pointDistance, pathBatch);

            var sortedPointDistance = pointDistance.AdjacentPointsDistance.OrderBy(p => p.Distance).ToList();

            // -1 because we want to add the origin point itself to the list
            var closestNeighbourBatch = sortedPointDistance.Select(s => s.AdjasantPoint).Take(pointsPerCluster.Value - 1).ToList();
            closestNeighbourBatch.Insert(0, pointDistance.Point);

            return closestNeighbourBatch;
        }

        public List<PointCluster> GetBatchOfPointCluster(int? pointsPerCluster)
        {
            foreach (var item in _coordinates)
            {
                if (CoordinateAlreadyExistInCluster(item)) continue;

                var neighbouringPointsPath = GetPointCluster(pointsPerCluster, item);

                var pointCluster = new PointCluster { PointClusterCoordinates = neighbouringPointsPath, IdPlaceTree = null };

                _pointClusters.Add(pointCluster);
            }

            return _pointClusters;
        }

        private static void RemoveAllExistingAdjacentPointsDistances(AllPointDistance pointDistance, List<CoordinateClass> pathBatch)
        {
            pointDistance.AdjacentPointsDistance.RemoveAll(a => pathBatch.Any(p => p.CoordinateId == a.AdjasantPoint.CoordinateId));
        }

        private bool CoordinateAlreadyExistInCluster(CoordinateClass item) => _pointClusters.SelectMany(a => a.PointClusterCoordinates).Any(a => a.CoordinateId == item.CoordinateId);
    }
}