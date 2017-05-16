using ClosestNeighbourSearchAlgorithms.KDTree;
using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using Xunit;
using FluentAssertions;

namespace ClosestNeighbourSearchTests
{
    public class KdTreeTests
    {
        private readonly int _numberOfCoordinates;
        private readonly int _pointPerCluster;
        private readonly Coordinate[] _arrayOfCoordinates;
        private readonly List<CoordinateClass> _listOfCoordinateClass;

        public KdTreeTests()
        {
            _numberOfCoordinates = 1000;
            _pointPerCluster = 500;
            _arrayOfCoordinates = Utilities.GenerateCoordinates(_numberOfCoordinates).ToArray();
            _listOfCoordinateClass = _arrayOfCoordinates.Select(a => new CoordinateClass
            {
                CoordinateId = a.CoordinateId,
                Latitude = a.Latitude,
                Longitude = a.Longitude
            }).ToList();
        }

        [Fact]
        public void GetNeighborClusters_ReturnTheCorrectNumberOfPoints_GivenANumber()
        {
            var kdTreeNeighboringPoints = new KDTree<Coordinate>(2, _arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                .NearestNeighborClusterLinear(500, _arrayOfCoordinates)
                .ToList();

            kdTreeNeighboringPoints[0].Count().Should().Be(_pointPerCluster);
            kdTreeNeighboringPoints.SelectMany(p => p).Count().Should().Be(_numberOfCoordinates);
        }

        [Fact]
        public void GetNeighborClusters_ReturnTheCorrectType()
        {
            var kdTreeNeighboringPoints = new KDTree<Coordinate>(2, _arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                .NearestNeighborClusterLinear(500, _arrayOfCoordinates)
                .ToList();

            kdTreeNeighboringPoints.Should().BeOfType<List<List<Coordinate>>>();
        }

        [Fact]
        public void GetNeighborClusters_CalculateDistanceCorrectly()
        {
            var kdTreeNeighboringPoints = new KDTree<Coordinate>(2, _arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                .NearestNeighborClusterLinear(500, _arrayOfCoordinates)
                .ToList();

            var targetPoint = kdTreeNeighboringPoints[0].First();
            var sortedPointsByDistance = _arrayOfCoordinates.OrderBy(a => a.Distance(targetPoint));

            kdTreeNeighboringPoints[0].ElementAt(1).ShouldBeEquivalentTo(sortedPointsByDistance.ElementAt(1));
        }

        [Fact]
        public void GetNeighborClusters_ReturnTheSameToursAsOldOne_GivenASetOfPoints()
        {
            //Act
            var oldPathClusterFinderNeighboringPoints = new PathClusterFinder(_listOfCoordinateClass)
                .GetBatchOfPointCluster(500)
                .Select(c => c.PointClusterCoordinates.Select(pc => new Coordinate { CoordinateId = pc.CoordinateId, Latitude = pc.Latitude, Longitude = pc.Longitude })
                .ToList())
                .ToList();

            var kdTreeNeighboringPoints = new KDTree<Coordinate>(2, _arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                .NearestNeighborClusterLinear(500, _arrayOfCoordinates)
                .ToList();

            //Assert
            kdTreeNeighboringPoints[0].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(oldPathClusterFinderNeighboringPoints[0].OrderBy(o => o.CoordinateId))
                                      .Should()
                                      .BeTrue();

            kdTreeNeighboringPoints[1].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(oldPathClusterFinderNeighboringPoints[1].OrderBy(o => o.CoordinateId))
                                      .Should()
                                      .BeTrue();
        }
    }
}