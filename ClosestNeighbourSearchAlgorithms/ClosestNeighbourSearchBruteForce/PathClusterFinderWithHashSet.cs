using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce
{
    class PathClusterFinderWithHashSet
    {
        private readonly HashSet<Coordinate> _coordinates;
        private readonly int _pointsPerCluster;

        public PathClusterFinderWithHashSet(HashSet<Coordinate> coordinates, int pointsPerCluster)
        {
            _coordinates = coordinates;
            _pointsPerCluster = pointsPerCluster;
        }

        public IEnumerable<Dictionary<long, Coordinate>> GetPointClusters()
        {
            while (_coordinates.Any())
            {
                const int seedItself = 1;

                var seed = _coordinates.First();

                var closestCoordinates = _coordinates.AsParallel()
                                                     .Where(c => !Equals(c, seed))
                                                     .OrderBy(c => seed.Distance(c))
                                                     .Take(_pointsPerCluster - seedItself)
                                                     .ToDictionary(c => c.CoordinateId, c => c);

                closestCoordinates.Add(seed.CoordinateId, seed);

                closestCoordinates.ToList().ForEach(c => _coordinates.Remove(c.Value));

                yield return closestCoordinates;
            }
        }



    }
}
