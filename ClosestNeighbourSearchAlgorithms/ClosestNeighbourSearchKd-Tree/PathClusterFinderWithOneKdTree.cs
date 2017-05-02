using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using ClosestNeighbourSearchAlgorithms.KDTree;

namespace ClosestNeighbourSearchAlgorithms
{
    class PathClusterFinderWithOneKdTree
    {
        private readonly HashSet<Coordinate> _coordinates;
        private readonly int _pointsPerCluster;
        private readonly KDTree<Coordinate> _kdTree;

        public PathClusterFinderWithOneKdTree(KDTree<Coordinate> kdTree, HashSet<Coordinate> coordinates, int pointsPerCluster)
        {
            _coordinates = coordinates;
            _pointsPerCluster = pointsPerCluster;
            _kdTree = kdTree;
        }

        public IEnumerable<List<Coordinate>> GetPointClusters()
        {
            while (_coordinates.Any())
            {
                var seed = _coordinates.First();

                var closestCoordinates = _kdTree.NearestNeighbors(seed, _pointsPerCluster);

                closestCoordinates.ForEach(c => _coordinates.Remove(c));

                yield return closestCoordinates;
            }
        }
    }
}
