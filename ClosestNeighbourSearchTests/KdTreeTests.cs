using ClosestNeighbourSearchAlgorithms.KDTree;
using System;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce;
using Xunit;
using FluentAssertions;

namespace ClosestNeighbourSearchTests
{
    public class KdTreeTests
    {
        [Fact]
        public void GetNeighborClusters_ReturnTheSameToursAsOldOne_GivenASetOfPoints()
        {
            //Arrange
            var numberOfCoordinates = 1000;
            var arrayOfCoordinates = Utilities.GenerateCoordinates(numberOfCoordinates).ToArray();
            var listOfCoordinateClass = arrayOfCoordinates
                .Select(a => new CoordinateClass
                {
                    CoordinateId = a.CoordinateId,
                    Latitude = a.Latitude,
                    Longitude = a.Longitude
                }).ToList();

            //Act
            var oldPathClusterFinderNeighboringPoints = new PathClusterFinder(listOfCoordinateClass)
                .GetBatchOfPointCluster(500)
                .Select(c => c.PointClusterCoordinates.Select(pc => new Coordinate { CoordinateId = pc.CoordinateId, Latitude = pc.Latitude, Longitude = pc.Longitude })
                .ToList())
                .ToList();

            var kdTreeNeighboringPoints = new KDTree<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                .GetNeighborClusters(500, arrayOfCoordinates)
                .ToList();

            //Assert
            kdTreeNeighboringPoints[0].OrderBy(k => k.CoordinateId).SequenceEqual(oldPathClusterFinderNeighboringPoints[0].OrderBy(o => o.CoordinateId)).Should().BeTrue();
            kdTreeNeighboringPoints[1].OrderBy(k => k.CoordinateId).SequenceEqual(oldPathClusterFinderNeighboringPoints[1].OrderBy(o => o.CoordinateId)).Should().BeTrue();
        }
    }
}