using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ClosestNeighbourSearchBruteForce;
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
            var dictionaryOfPoints = new Dictionary<int, Coordinate>();
            var hashSetOfPoints = new HashSet<Coordinate>();
            var arrayOfPoints = new Coordinate[numberOfCoordinates];

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

                dictionaryOfPoints.Add(i + 1, new Coordinate
                {
                    CoordinateId = i + 1,
                    Latitude = latitude,
                    Longitude = longitude,
                });

                hashSetOfPoints.Add(new Coordinate
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

            ////==========================================================================================
            //var stopwatch2 = new Stopwatch();
            //stopwatch2.Start();

            //var coordinateClustersList = new PathClusterFinderWithList(listOfPoints, 15).GetPointClusters().ToList();

            //stopwatch2.Stop();

            //var elapsedTimeForcoordinateClustersList = stopwatch2.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith List Took: {elapsedTimeForcoordinateClustersList} Milliseconds");
            ////==========================================================================================


            ////==========================================================================================
            //var stopwatch3 = new Stopwatch();
            //stopwatch3.Start();

            //var coordinateClustersDictionary = new PathClusterFinderWithDictionary(dictionaryOfPoints, 50).GetPointClusters().ToList();

            //stopwatch3.Stop();

            //var elapsedTimeForcoordinateClustersDictionary = stopwatch3.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith Dictionary Took: {elapsedTimeForcoordinateClustersDictionary} Milliseconds");
            ////==========================================================================================

            ////==========================================================================================
            //var stopwatch4 = new Stopwatch();
            //stopwatch4.Start();

            //var coordinateClustersHashSet = new PathClusterFinderWithHashSet(hashSetOfPoints, 50).GetPointClusters().ToList();

            //stopwatch4.Stop();

            //var elapsedTimeForcoordinateClustersHashSet = stopwatch4.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith HashSet Took: {elapsedTimeForcoordinateClustersHashSet} Milliseconds");
            ////==========================================================================================


            //==========================================================================================
            var stopwatch5 = new Stopwatch();
            stopwatch5.Start();

            var tree = new KDTree<Coordinate>(2, arrayOfPoints, Utilities.L2Norm_Squared_Coordinate);

            //for (int i = 0; i < arrayOfPoints.Length / 50; i++)
            //{
            //    var linearResult = tree.NearestNeighbors(arrayOfPoints[50 * i], 50);
            //}

            //var linearResult = tree.NearestNeighbors(listOfPoints[0], 15);
            //var line = tree.NearestNeighborsCollection(15).ToList();

            var nearDear = new PathClusterFinderWithKdTree(hashSetOfPoints, 500).GetPointClusters().ToList();

            stopwatch5.Stop();

            var elapsedTimeForcoordinateClustersKdTree = stopwatch5.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinderWith KdTree Linear Took: {elapsedTimeForcoordinateClustersKdTree} Milliseconds");
            //==========================================================================================


            //==========================================================================================
            //var stopwatch6 = new Stopwatch();
            //stopwatch6.Start();

            //var tree2 = new KDTree<Coordinate>(2, arrayOfPoints, Utilities.L2Norm_Squared_Coordinate);

            //for (int i = 0; i < arrayOfPoints.Length / 50; i++)
            //{
            //    var radialResult = tree2.RadialSearch(arrayOfPoints[50 * i], 10000, 50);
            //}

            ////var radialResult = tree2.RadialSearch(treePoints2[0], 10000, 50);

            //stopwatch6.Stop();

            //var elapsedTimeForcoordinateClustersKdTreeRadial = stopwatch6.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinderWith KdTree Radial Took: {elapsedTimeForcoordinateClustersKdTreeRadial} Milliseconds");
            //==========================================================================================
        }

    }
}