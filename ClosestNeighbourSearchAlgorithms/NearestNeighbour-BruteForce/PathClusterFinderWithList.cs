using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.Contracts;

namespace ClosestNeighbourSearchAlgorithms
{
    public class PathClusterFinderWithList
    {
        private readonly List<Coordinate> _coordinates;
        private readonly int _pointsPerCluster;

        public PathClusterFinderWithList(List<Coordinate> coordinates, int pointsPerCluster)
        {
            _coordinates = coordinates;
            _pointsPerCluster = pointsPerCluster;
        }

        public IEnumerable<List<Coordinate>> GetPointClusters()
        {
            while (_coordinates.Any())
            {
                const int seedItself = 1;

                var seed = _coordinates.First();

                var closestCoordinates = _coordinates.AsParallel()
                                                     .Where(c => !Equals(c, seed))
                                                     .OrderBy(c => seed.Distance(c))
                                                     .Take(_pointsPerCluster - seedItself)
                                                     .ToList();

                closestCoordinates.Add(seed);

                closestCoordinates.ForEach(c => _coordinates.Remove(c));

                yield return closestCoordinates;
            }
        }
    }
}