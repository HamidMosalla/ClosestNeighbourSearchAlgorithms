using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce.OldWackyImplementation;
using ClosestNeighbourSearchAlgorithms.KDTree;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var numberOfCoordinates = 100;
            var arrayOfCoordinates = Utilities.GenerateCoordinates(numberOfCoordinates).ToArray();
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

            ////==========================================================================================
            //var stopwatch6 = new Stopwatch();
            //stopwatch6.Start();

            //var neighboringPoints = new KDTree<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate).GetNeighborClusters(50, arrayOfCoordinates).ToList();

            //stopwatch6.Stop();

            //var elapsedTimeForcoordinateClustersKdTreeOneTreeForAll = stopwatch6.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith WithOneKdTree Linear Took: {elapsedTimeForcoordinateClustersKdTreeOneTreeForAll} Milliseconds");
            ////==========================================================================================

            //==========================================================================================
            var stopwatch7 = new Stopwatch();
            stopwatch7.Start();

            var neighboringPointsRadial = new KDTree<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate).GetNeighborClustersRadial(1000, 50, arrayOfCoordinates).ToList();

            stopwatch7.Stop();

            var elapsedTimeForcoordinateClustersKdTreeOneTreeRadialForAll = stopwatch7.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith WithOneKdTree Linear Took: {elapsedTimeForcoordinateClustersKdTreeOneTreeRadialForAll} Milliseconds");
            //==========================================================================================
        }

    }
}