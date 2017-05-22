using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms;
using ClosestNeighbourSearchAlgorithms.Contracts;
using ClosestNeighbourSearchAlgorithms.Models;
using ClosestNeighbourSearchAlgorithms.Utilities;
using FluentAssertions;
using Xunit;

namespace ClosestNeighbourSearchTests
{
    public class PathClusterFinderWithListTests
    {

        private readonly int _numberOfCoordinates;
        private readonly int _pointPerCluster;
        private readonly Coordinate[] _arrayOfCoordinates;
        private readonly List<Coordinate> _listOfCoordinates;

        public PathClusterFinderWithListTests()
        {
            _numberOfCoordinates = 1000;
            _pointPerCluster = 500;
            _arrayOfCoordinates = KdTreeHelper.GenerateCoordinates(_numberOfCoordinates).ToArray();
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
    }
}