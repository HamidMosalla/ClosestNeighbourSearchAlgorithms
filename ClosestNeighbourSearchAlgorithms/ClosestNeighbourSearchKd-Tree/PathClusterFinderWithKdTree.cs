using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using ClosestNeighbourSearchAlgorithms.KDTree;

namespace ClosestNeighbourSearchAlgorithms
{
    class PathClusterFinderWithKdTree
    {
        private readonly HashSet<Coordinate> _coordinates;
        private readonly int _pointsPerCluster;

        public PathClusterFinderWithKdTree(HashSet<Coordinate> coordinates, int pointsPerCluster)
        {
            _coordinates = coordinates;
            _pointsPerCluster = pointsPerCluster;
        }

        public IEnumerable<List<Coordinate>> GetPointClusters()
        {
            while (_coordinates.Any())
            {
                var seed = _coordinates.First();

                var closestCoordinates = new KDTree<Coordinate>(2, _coordinates.ToArray(), Utilities.L2Norm_Squared_Coordinate).NearestNeighbors(seed, _pointsPerCluster);

                closestCoordinates.ForEach(c => _coordinates.Remove(c));

                yield return closestCoordinates;
            }
        }
    }
}
