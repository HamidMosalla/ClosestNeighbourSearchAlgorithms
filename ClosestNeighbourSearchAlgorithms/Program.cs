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
            var numberOfCoordinates = 100000;
            var listOfPoints = new List<Coordinate>();
            var listOfPointClass = new List<CoordinateClass>();
            var dictionaryOfPoints = new Dictionary<int, Coordinate>();
            var hashSetOfPoints1 = new HashSet<Coordinate>();
            var hashSetOfPoints2 = new HashSet<Coordinate>();
            var hashSetOfPoints3 = new HashSet<Coordinate>();
            var arrayOfPoints = new Coordinate[numberOfCoordinates];

            Console.WriteLine($"For {numberOfCoordinates.ToString("n0")} Points:");

            var rand = new Random();
            for (var i = 0; i < numberOfCoordinates; i++)
            {
                var latitude = rand.Next(-10000, 10000);
                var longitude = rand.Next(-10000, 10000);

                listOfPoints.Add(new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                listOfPointClass.Add(new CoordinateClass
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                dictionaryOfPoints.Add(i + 1, new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                hashSetOfPoints1.Add(new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                hashSetOfPoints2.Add(new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                hashSetOfPoints3.Add(new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                arrayOfPoints[i] = new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                };
            }

            //////==========================================================================================
            //var stopwatch1 = new Stopwatch();
            //stopwatch1.Start();

            //var oldPathClusterFinder = new PathClusterFinder(listOfPointClass).GetBatchOfPointCluster(500).ToList();

            //stopwatch1.Stop();

            //var elapsedTimeOldPathClusterFinder = stopwatch1.ElapsedMilliseconds;

            //Console.WriteLine($"Old PathClusterFinder Took: {elapsedTimeOldPathClusterFinder} Milliseconds");
            //////==========================================================================================

            //////==========================================================================================
            //var stopwatch2 = new Stopwatch();
            //stopwatch2.Start();

            //var coordinateClustersList = new PathClusterFinderWithList(listOfPoints, 500).GetPointClusters().ToList();

            //stopwatch2.Stop();

            //var elapsedTimeForcoordinateClustersList = stopwatch2.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith List Took: {elapsedTimeForcoordinateClustersList} Milliseconds");
            //////==========================================================================================


            ////==========================================================================================
            var stopwatch3 = new Stopwatch();
            stopwatch3.Start();

            var coordinateClustersDictionary = new PathClusterFinderWithDictionary(dictionaryOfPoints, 500).GetPointClusters().ToList();

            stopwatch3.Stop();

            var elapsedTimeForcoordinateClustersDictionary = stopwatch3.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith Dictionary Took: {elapsedTimeForcoordinateClustersDictionary} Milliseconds");
            ////==========================================================================================

            ////==========================================================================================
            var stopwatch4 = new Stopwatch();
            stopwatch4.Start();

            var coordinateClustersHashSet = new PathClusterFinderWithHashSet(hashSetOfPoints1, 500).GetPointClusters().ToList();

            stopwatch4.Stop();

            var elapsedTimeForcoordinateClustersHashSet = stopwatch4.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith HashSet Took: {elapsedTimeForcoordinateClustersHashSet} Milliseconds");
            ////==========================================================================================

            //==========================================================================================
            var stopwatch6 = new Stopwatch();
            stopwatch6.Start();

            var tree = new KDTree<Coordinate>(2, arrayOfPoints, Utilities.L2Norm_Squared_Coordinate);

            var nearestOneTreeForAll = new PathClusterFinderKdTree(tree, hashSetOfPoints3, 500).GetPointClusters().ToList();

            stopwatch6.Stop();

            var elapsedTimeForcoordinateClustersKdTreeOneTreeForAll = stopwatch6.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith WithOneKdTree Linear Took: {elapsedTimeForcoordinateClustersKdTreeOneTreeForAll} Milliseconds");
            //==========================================================================================
        }

    }
}