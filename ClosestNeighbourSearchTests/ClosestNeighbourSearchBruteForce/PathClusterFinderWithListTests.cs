using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.KDTree;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using FluentAssertions;
using Xunit;

namespace ClosestNeighbourSearchTests.ClosestNeighbourSearchBruteForce
{
    public class PathClusterFinderWithListTests
    {

        private readonly int _numberOfCoordinates;
        private readonly int _pointPerCluster;
        private readonly Coordinate[] _arrayOfCoordinates;
        private readonly List<CoordinateClass> _listOfCoordinateClass;
        private readonly List<Coordinate> _listOfCoordinates;

        public PathClusterFinderWithListTests()
        {
            _numberOfCoordinates = 1000;
            _pointPerCluster = 500;
            _arrayOfCoordinates = Utilities.GenerateCoordinates(_numberOfCoordinates).ToArray();
            _listOfCoordinateClass = _arrayOfCoordinates.Select(a => new CoordinateClass { CoordinateId = a.CoordinateId, Latitude = a.Latitude, Longitude = a.Longitude }).ToList();
            _listOfCoordinates = _arrayOfCoordinates.ToList();
        }

        [Fact]
        public void GetPointClusters_ReturnTheCorrectNumberOfPoints_GivenANumber()
        {
            var pathClusterFinderWithListNeighboringPoints = new PathClusterFinderWithList(_listOfCoordinates, _pointPerCluster).GetPointClusters().ToList();

            pathClusterFinderWithListNeighboringPoints[0].Count().Should().Be(_pointPerCluster);
            pathClusterFinderWithListNeighboringPoints.SelectMany(p => p).Count().Should().Be(_numberOfCoordinates);
        }

        [Fact]
        public void GetPointClusters_ReturnTheCorrectType()
        {
            var pathClusterFinderWithListNeighboringPoints = new PathClusterFinderWithList(_listOfCoordinates, _pointPerCluster).GetPointClusters().ToList();

            pathClusterFinderWithListNeighboringPoints.Should().BeOfType<List<List<Coordinate>>>();
        }

        [Fact]
        public void GetPointClusters_ReturnTheSameToursAsOldOne_GivenASetOfPoints()
        {
            //Act
            var oldPathClusterFinderNeighboringPoints = new PathClusterFinder(_listOfCoordinateClass)
                .GetBatchOfPointCluster(_pointPerCluster)
                .Select(c => c.PointClusterCoordinates.Select(pc => new Coordinate { CoordinateId = pc.CoordinateId, Latitude = pc.Latitude, Longitude = pc.Longitude })
                .ToList())
                .ToList();

            var pathClusterFinderWithListNeighboringPoints = new PathClusterFinderWithList(_listOfCoordinates, _pointPerCluster).GetPointClusters().ToList();

            //Assert
            pathClusterFinderWithListNeighboringPoints[0].OrderBy(k => k.CoordinateId).SequenceEqual(oldPathClusterFinderNeighboringPoints[0].OrderBy(o => o.CoordinateId)).Should().BeTrue();
            pathClusterFinderWithListNeighboringPoints[1].OrderBy(k => k.CoordinateId).SequenceEqual(oldPathClusterFinderNeighboringPoints[1].OrderBy(o => o.CoordinateId)).Should().BeTrue();
        }
    }
}
