using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var numberOfCoordinates = 10000;
            var coordinates = Utilities.GenerateDoubles(numberOfCoordinates, range: 10000);
            var arrayOfCoordinates = Utilities.GenerateCoordinatesFromArray(coordinates).ToArray();
            var listOfCoordinateClass = arrayOfCoordinates.Select(a => new CoordinateClass { CoordinateId = a.CoordinateId, Latitude = a.Latitude, Longitude = a.Longitude }).ToList();
            var listOfCoordinates = arrayOfCoordinates.ToList();
            var dictionaryOfCoordinates = arrayOfCoordinates.ToDictionary(c => c.CoordinateId, c => c);
            var hashSetOfCoordinates = arrayOfCoordinates.ToHashSet();

            Console.WriteLine($"For {numberOfCoordinates.ToString("n0")} Points:");

            //////==========================================================================================
            //var stopwatch1 = new Stopwatch();
            //stopwatch1.Start();

            //var oldPathClusterFinder = new PathClusterFinder(listOfCoordinateClass).GetBatchOfPointCluster(500).ToList();

            //stopwatch1.Stop();

            //var elapsedTimeOldPathClusterFinder = stopwatch1.ElapsedMilliseconds;

            //Console.WriteLine($"Old PathClusterFinder Took: {elapsedTimeOldPathClusterFinder} Milliseconds");
            //////==========================================================================================

            //////==========================================================================================
            //var stopwatch2 = new Stopwatch();
            //stopwatch2.Start();

            //var coordinateClustersList = new PathClusterFinderWithList(listOfCoordinates, 500).GetPointClusters().ToList();

            //stopwatch2.Stop();

            //var elapsedTimeForcoordinateClustersList = stopwatch2.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith List Took: {elapsedTimeForcoordinateClustersList} Milliseconds");
            //////==========================================================================================


            //////==========================================================================================
            //var stopwatch3 = new Stopwatch();
            //stopwatch3.Start();

            //var coordinateClustersDictionary = new PathClusterFinderWithDictionary(dictionaryOfCoordinates, 500).GetPointClusters().ToList();

            //stopwatch3.Stop();

            //var elapsedTimeForcoordinateClustersDictionary = stopwatch3.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith Dictionary Took: {elapsedTimeForcoordinateClustersDictionary} Milliseconds");
            //////==========================================================================================

            //////==========================================================================================
            //var stopwatch4 = new Stopwatch();
            //stopwatch4.Start();

            //var coordinateClustersHashSet = new PathClusterFinderWithHashSet(hashSetOfCoordinates, 500).GetPointClusters().ToList();

            //stopwatch4.Stop();

            //var elapsedTimeForcoordinateClustersHashSet = stopwatch4.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith HashSet Took: {elapsedTimeForcoordinateClustersHashSet} Milliseconds");
            //////==========================================================================================

            var nearestPointsKdTreeNodeRemoved = new KDTreeNodeRemoved<double>(2, coordinates, Utilities.L2Norm_Squared_Double).RadialSearch(coordinates.First(), 100000, 500);

            var nearestPointsKdTreePristine = new KDTreePristine<double, Coordinate>(2, coordinates, arrayOfCoordinates, Utilities.L2Norm_Squared_Double).RadialSearch(coordinates.First(), 100000, 500);

            var nearestPonitsKdTree = new KDTree<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate).NearestNeighborsRadial(arrayOfCoordinates.First(), 100000, 500);

            //==========================================================================================
            var stopwatch6 = new Stopwatch();
            stopwatch6.Start();

            var nearestPontsKdTreeLinear = new KDTree<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate).NearestNeighborClusterLinear(500, arrayOfCoordinates).ToList();

            stopwatch6.Stop();

            var elapsedTimeForcoordinateClustersKdTreeOneTreeForAll = stopwatch6.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith WithOneKdTree Linear Took: {elapsedTimeForcoordinateClustersKdTreeOneTreeForAll} Milliseconds");
            //==========================================================================================

            //==========================================================================================
            var stopwatch7 = new Stopwatch();
            stopwatch7.Start();

            var nearestPontsKdTreeRadial = new KDTree<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate).NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: arrayOfCoordinates).ToList();

            stopwatch7.Stop();

            var elapsedTimeForcoordinateClustersKdTreeOneTreeRadialForAll = stopwatch7.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith WithOneKdTree Radial Took: {elapsedTimeForcoordinateClustersKdTreeOneTreeRadialForAll} Milliseconds");
            //==========================================================================================
        }

    }
}