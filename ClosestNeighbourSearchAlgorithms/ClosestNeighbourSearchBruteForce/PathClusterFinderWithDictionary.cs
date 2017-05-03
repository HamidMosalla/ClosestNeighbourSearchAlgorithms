using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce
{
    public class PathClusterFinderWithDictionary
    {
        private readonly Dictionary<long, Coordinate> _coordinates;
        private readonly int _pointsPerCluster;

        public PathClusterFinderWithDictionary(Dictionary<long, Coordinate> coordinates, int pointsPerCluster)
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
                                                     .OrderBy(c => seed.Value.Distance(c.Value))
                                                     .Take(_pointsPerCluster - seedItself)
                                                     .ToDictionary(c => c.Key, c => c.Value);

                closestCoordinates.Add(seed.Key, seed.Value);

                closestCoordinates.ToList().ForEach(c => _coordinates.Remove(c.Key));

                yield return closestCoordinates;
            }
        }



    }
}
