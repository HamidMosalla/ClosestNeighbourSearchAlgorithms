using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using Xunit;
using FluentAssertions;

namespace ClosestNeighbourSearchTests
{
    public class KdTreePristineTests
    {
        private readonly int _numberOfCoordinates;
        private readonly int _pointPerCluster;
        private readonly Coordinate[] _arrayOfCoordinates;
        private readonly List<CoordinateClass> _listOfCoordinateClass;
        private double[][] _coordinates;

        public KdTreePristineTests()
        {
            _numberOfCoordinates = 1000;
            _pointPerCluster = 500;
            _coordinates = Utilities.GenerateDoubles(_numberOfCoordinates, range: 10000);
            _arrayOfCoordinates = Utilities.GenerateCoordinatesFromArray(_coordinates).ToArray();
            _listOfCoordinateClass = _arrayOfCoordinates.Select(a => new CoordinateClass
            {
                CoordinateId = a.CoordinateId,
                Latitude = a.Latitude,
                Longitude = a.Longitude
            }).ToList();
        }

        [Fact]
        public void KdTreeSearchLinearAndRadialReturnTheSameSequenceOfElementsResult()
        {
            var nearestPontsKdTreePristineLinear = new KDTreePristine<double, Coordinate>(2, _coordinates, _arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var nearestPointsKdTreePristineRadial = new KDTreePristine<double, Coordinate>(2, _coordinates, _arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: _arrayOfCoordinates).ToList();

            nearestPontsKdTreePristineLinear[0].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(nearestPointsKdTreePristineRadial[0].OrderBy(o => o.CoordinateId))
                                      .Should()
                                      .BeTrue();

            nearestPontsKdTreePristineLinear[1].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(nearestPointsKdTreePristineRadial[1].OrderBy(o => o.CoordinateId))
                                      .Should()
                                      .BeTrue();
        }

        [Fact]
        public void KdTreeWithCoordinate_ReturnsTheSameResultAs_KdTreePristine()
        {
            var nearestPontsKdTreeRadial = new KDTree<Coordinate>(2, _arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                                                             .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: _arrayOfCoordinates).ToList();

            var nearestPointsKdTreePristineRadial = new KDTreePristine<double, Coordinate>(2, _coordinates, _arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                                                             .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: _arrayOfCoordinates).ToList();

            nearestPontsKdTreeRadial[0].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(nearestPointsKdTreePristineRadial[0])
                                      .Should()
                                      .BeTrue();

            nearestPontsKdTreeRadial[1].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(nearestPointsKdTreePristineRadial[1])
                                      .Should()
                                      .BeTrue();
        }

        [Fact]
        public void KdTreeSearchLinearAndRadia_ShouldNotBeTheSame()
        {
            var nearestPontsKdTreePristineLinear = new KDTreePristine<double, Coordinate>(2, _coordinates, _arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var nearestPointsKdTreePristineRadial = new KDTreePristine<double, Coordinate>(2, _coordinates, _arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: _arrayOfCoordinates).ToList();

            nearestPontsKdTreePristineLinear[0].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(nearestPointsKdTreePristineRadial[0])
                                      .Should()
                                      .BeFalse();

            nearestPontsKdTreePristineLinear[1].OrderBy(k => k.CoordinateId)
                                      .SequenceEqual(nearestPointsKdTreePristineRadial[1])
                                      .Should()
                                      .BeFalse();
        }
    }
}