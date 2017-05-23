using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms;
using ClosestNeighbourSearchAlgorithms.Contracts;
using ClosestNeighbourSearchAlgorithms.Models;
using ClosestNeighbourSearchAlgorithms.Utilities;
using Xunit;
using FluentAssertions;

namespace ClosestNeighbourSearchTests
{
    public class KdTreeTests
    {
        private readonly int _numberOfCoordinates;
        private readonly int _pointPerCluster;
        private readonly Coordinate[] _arrayOfCoordinates;
        private double[][] _coordinatesAsDoubleArray;
        private readonly List<Coordinate> _listOfCoordinates;

        public KdTreeTests()
        {
            _numberOfCoordinates = 1000;
            _pointPerCluster = 500;
            _coordinatesAsDoubleArray = KdTreeHelper.GenerateDoubles(_numberOfCoordinates, range: 10000);
            _arrayOfCoordinates = KdTreeHelper.GenerateCoordinatesFromArray(_coordinatesAsDoubleArray).ToArray();
            _listOfCoordinates = _arrayOfCoordinates.ToList();
        }

        [Fact]
        public void KdTreeWithCoordinate_ReturnsTheSameResult_AsKdTreeWithArrayOfDouble()
        {
            var linearSearchWithOriginalKdTree = new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Double)
                                                             .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var linearSearchWithCoordinateKdTree = new KDTreeCoordinate<Coordinate>(2, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Coordinate)
                                                             .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var radialSearchWithOriginalKdTreeRadial = new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Double)
                                                             .NearestNeighborClusterRadial(radius: Radius.SuperSlowButAccurate, pointsPerCluster: 500, coordinates: _arrayOfCoordinates).ToList();

            var radialSearchWithCoordinateKdTreeRadial = new KDTreeCoordinate<Coordinate>(2, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Coordinate)
                                                             .NearestNeighborClusterRadial(radius: Radius.SuperSlowButAccurate, pointsPerCluster: 500, coordinates: _arrayOfCoordinates).ToList();

            linearSearchWithOriginalKdTree[0]
                .SequenceEqual(linearSearchWithCoordinateKdTree[0])
                .Should()
                .BeTrue();

            linearSearchWithOriginalKdTree[1]
                .SequenceEqual(linearSearchWithCoordinateKdTree[1])
                .Should()
                .BeTrue();


            radialSearchWithOriginalKdTreeRadial[0]
                .SequenceEqual(radialSearchWithCoordinateKdTreeRadial[0])
                .Should()
                .BeTrue();

            radialSearchWithOriginalKdTreeRadial[1]
                .SequenceEqual(radialSearchWithCoordinateKdTreeRadial[1])
                .Should()
                .BeTrue();
        }

        [Fact]
        public void BothVersionOfKdTreeGenerateTheSameTreeStructure()
        {
            var originalKdTree = new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Double);

            var coordinateKdTree = new KDTreeCoordinate<Coordinate>(2, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Coordinate);

            for (int i = 0; i < _numberOfCoordinates; i++)
            {
                if (coordinateKdTree.InternalTreeOfPoints[i].CoordinateId != 0)
                    originalKdTree.InternalPointArray[i][0].Should().Be(coordinateKdTree.InternalTreeOfPoints[i].Latitude);
            }
        }

        [Fact]
        public void BothVersionOfKdTree_ReturnsTheSameSetOfCoordinates_GivenASetOfPoints()
        {
            var pointsNeeded = 100;

            var originalKdTree = new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Double);

            var coordinateKdTree = new KDTreeCoordinate<Coordinate>(2, _arrayOfCoordinates, KdTreeHelper.L2Norm_Squared_Coordinate);

            var pointsFromOriginal = originalKdTree.NearestNeighborsLinear(_coordinatesAsDoubleArray.First(), pointsNeeded);

            var pointsFromModified = coordinateKdTree.NearestNeighborsLinear(_arrayOfCoordinates.First(), pointsNeeded);

            pointsFromOriginal.SequenceEqual(pointsFromModified).Should().BeTrue();
        }

        [Fact]
        public void KdTreeSearchLinearAndRadialReturnTheSameSequenceOfElementsResult()
        {
            var nearestPontsKdTreePristineLinear =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var nearestPointsKdTreePristineRadial =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: _arrayOfCoordinates)
                    .ToList();

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
        public void KdTreeLinearSearchDoubleArray_ReturnsTheSameResultAs_NearestNeighbourBruteForce()
        {
            var pathClusterFinderWithListNeighboringPoints = new PathClusterFinderWithList(_listOfCoordinates, _pointPerCluster)
                                                                                .GetPointClusters().ToList();

            var nearestPointsKdTreeLinear =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterLinear(_pointPerCluster, _arrayOfCoordinates)
                    .ToList();

            pathClusterFinderWithListNeighboringPoints[0].OrderBy(k => k.CoordinateId)
                .SequenceEqual(nearestPointsKdTreeLinear[0].OrderBy(k => k.CoordinateId))
                .Should()
                .BeTrue();

            pathClusterFinderWithListNeighboringPoints[1].OrderBy(k => k.CoordinateId)
                .SequenceEqual(nearestPointsKdTreeLinear[1].OrderBy(k => k.CoordinateId))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void KdTreeLinearSearchCoordinate_ReturnsTheSameResultAs_NearestNeighbourBruteForce()
        {
            var pathClusterFinderWithListNeighboringPoints = new PathClusterFinderWithList(_listOfCoordinates, _pointPerCluster)
                                                                                .GetPointClusters().ToList();

            var nearestPointsKdTreeLinear =
                new KDTreeCoordinate< Coordinate>(2, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Coordinate)
                    .NearestNeighborClusterLinear(_pointPerCluster, _arrayOfCoordinates)
                    .ToList();

            pathClusterFinderWithListNeighboringPoints[0].OrderBy(k => k.CoordinateId)
                .SequenceEqual(nearestPointsKdTreeLinear[0].OrderBy(k => k.CoordinateId))
                .Should()
                .BeTrue();

            pathClusterFinderWithListNeighboringPoints[1].OrderBy(k => k.CoordinateId)
                .SequenceEqual(nearestPointsKdTreeLinear[1].OrderBy(k => k.CoordinateId))
                .Should()
                .BeTrue();
        }

        [Fact]
        public void KdTreeSearchLinearAndRadial_ShouldNotBeTheSame()
        {
            var nearestPontsKdTreePristineLinear =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var nearestPointsKdTreePristineRadial =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: _arrayOfCoordinates)
                    .ToList();

            nearestPontsKdTreePristineLinear[0].SequenceEqual(nearestPointsKdTreePristineRadial[0]).Should().BeFalse();

            nearestPontsKdTreePristineLinear[1].SequenceEqual(nearestPointsKdTreePristineRadial[1]).Should().BeFalse();
        }

        [Fact]
        public void KdTreeSearchLinearAndRadia_ShouldBeTheSame_IfRadiusOfRadialIsDoubleMax()
        {
            var nearestPontsKdTreePristineLinear =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            var nearestPointsKdTreePristineRadial =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterRadial(radius: double.MaxValue, pointsPerCluster: 500, coordinates: _arrayOfCoordinates)
                    .ToList();

            nearestPontsKdTreePristineLinear[0].SequenceEqual(nearestPointsKdTreePristineRadial[0]).Should().BeTrue();

            nearestPontsKdTreePristineLinear[1].SequenceEqual(nearestPointsKdTreePristineRadial[1]).Should().BeTrue();
        }

        [Fact]
        public void GetNeighborClusters_ReturnTheCorrectNumberOfPoints_GivenANumber()
        {
            var nearestPontsKdTreePristineLinear =
                new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                        KdTreeHelper.L2Norm_Squared_Double)
                    .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            nearestPontsKdTreePristineLinear[0].Count().Should().Be(_pointPerCluster);
            nearestPontsKdTreePristineLinear.SelectMany(p => p).Count().Should().Be(_numberOfCoordinates);
        }

        [Fact]
        public void GetNeighborClusters_ReturnTheCorrectType()
        {
            var nearestPontsKdTreePristineLinear =
               new KDTree<double, Coordinate>(2, _coordinatesAsDoubleArray, _arrayOfCoordinates,
                       KdTreeHelper.L2Norm_Squared_Double)
                   .NearestNeighborClusterLinear(500, _arrayOfCoordinates).ToList();

            nearestPontsKdTreePristineLinear.Should().BeOfType<List<List<Coordinate>>>();
        }

        [Fact]
        public void GtClosestPointGenerateTheSameResultInBothVersion()
        {
            var minPoint = new Coordinate { Latitude = 6544, Longitude = 5577 };
            var maxPoint = new Coordinate { Latitude = 9687, Longitude = 1254 };

            var minPointA = new[] { minPoint.Latitude, minPoint.Longitude };
            var maxPointA = new[] { maxPoint.Latitude, maxPoint.Longitude };

            var targetPoint = new Coordinate { Latitude = 3322, Longitude = 4562 };
            var targetPointA = new[] { targetPoint.Latitude, targetPoint.Longitude };

            var hyperRect = new HyperRect<double>();
            hyperRect.MinPoint = minPointA;
            hyperRect.MaxPoint = maxPointA;


            var hyperRectCoordinate = new HyperRectCoordinate<Coordinate>();
            hyperRectCoordinate.MinPoint = minPoint;
            hyperRectCoordinate.MaxPoint = maxPoint;

            var rect = hyperRect.GetClosestPoint(targetPointA);
            var rectCoordinate = hyperRectCoordinate.GetClosestPoint(targetPoint);

            rect[0].Should().Be(rectCoordinate.Latitude);
            rect[1].Should().Be(rectCoordinate.Longitude);
        }

        [Fact]
        public void SquareOfDistanceWorksCorrectlyInBothVersionOfKdTree()
        {
            var minPoint = new Coordinate { Latitude = 6544, Longitude = 5577 };
            var maxPoint = new Coordinate { Latitude = 9687, Longitude = 1254 };

            var minPointA = new[] { minPoint.Latitude, minPoint.Longitude };
            var maxPointA = new[] { maxPoint.Latitude, maxPoint.Longitude };

            var coordinateDistance = KdTreeHelper.L2Norm_Squared_Coordinate(minPoint, maxPoint);
            var doubleArrayDistance = KdTreeHelper.L2Norm_Squared_Double(minPointA, maxPointA);

            coordinateDistance.Should().Be(doubleArrayDistance);
        }
    }
}