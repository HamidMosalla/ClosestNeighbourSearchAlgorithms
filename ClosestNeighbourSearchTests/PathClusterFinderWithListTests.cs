using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.KDTree;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;
using FluentAssertions;
using Xunit;

namespace ClosestNeighbourSearchTests
{
    public class PathClusterFinderWithListTests
    {
        [Fact]
        public void GetPointClusters_ReturnTheSameToursAsOldOne_GivenASetOfPoints()
        {
            //Arrange
            var numberOfCoordinates = 1000;
            var arrayOfCoordinates = Utilities.GenerateCoordinates(numberOfCoordinates).ToArray();
            var listOfCoordinateClass = arrayOfCoordinates.Select(a => new CoordinateClass { CoordinateId = a.CoordinateId, Latitude = a.Latitude, Longitude = a.Longitude }).ToList();
            var listOfCoordinates = arrayOfCoordinates.ToList();
            var dictionaryOfCoordinates = arrayOfCoordinates.ToDictionary(c => c.CoordinateId, c => c);
            var hashSetOfCoordinates = arrayOfCoordinates.ToHashSet();

            //Act
            var oldPathClusterFinderNeighboringPoints = new PathClusterFinder(listOfCoordinateClass)
                .GetBatchOfPointCluster(500)
                .Select(c => c.PointClusterCoordinates.Select(pc => new Coordinate { CoordinateId = pc.CoordinateId, Latitude = pc.Latitude, Longitude = pc.Longitude })
                .ToList())
                .ToList();

            var pathClusterFinderWithListNeighboringPoints = new PathClusterFinderWithList(listOfCoordinates, 500).GetPointClusters().ToList();

            //Assert
            pathClusterFinderWithListNeighboringPoints[0].OrderBy(k => k.CoordinateId).SequenceEqual(oldPathClusterFinderNeighboringPoints[0].OrderBy(o => o.CoordinateId)).Should().BeTrue();
            pathClusterFinderWithListNeighboringPoints[1].OrderBy(k => k.CoordinateId).SequenceEqual(oldPathClusterFinderNeighboringPoints[1].OrderBy(o => o.CoordinateId)).Should().BeTrue();
        }
    }
}
