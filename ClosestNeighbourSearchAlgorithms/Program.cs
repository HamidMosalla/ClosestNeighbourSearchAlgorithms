using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.Contracts;

namespace ClosestNeighbourSearchAlgorithms
{
    static class Program
    {
        public static void Main(string[] args)
        {
            var numberOfCoordinates = 10000;
            var coordinatesAsDoubleArray = Utilities.GenerateDoubles(numberOfCoordinates, range: 10000);
            var arrayOfCoordinates = Utilities.GenerateCoordinatesFromArray(coordinatesAsDoubleArray).ToArray();
            var listOfCoordinates = arrayOfCoordinates.ToList();
            var dictionaryOfCoordinates = arrayOfCoordinates.ToDictionary(c => c.CoordinateId, c => c);
            var hashSetOfCoordinates = arrayOfCoordinates.ToHashSet();

            Console.WriteLine($"For {numberOfCoordinates.ToString("n0")} Points:");


            //////==========================================================================================
            //var stopwatch1 = new Stopwatch();
            //stopwatch1.Start();

            //var coordinateClustersList = new PathClusterFinderWithList(listOfCoordinates, 500).GetPointClusters().ToList();

            //stopwatch1.Stop();

            //var elapsedTimeForcoordinateClustersList = stopwatch1.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinder BruteForce List Took: {elapsedTimeForcoordinateClustersList} Milliseconds");
            //////==========================================================================================


            //////==========================================================================================
            //var stopwatch2 = new Stopwatch();
            //stopwatch2.Start();

            //var coordinateClustersDictionary = new PathClusterFinderWithDictionary(dictionaryOfCoordinates, 500).GetPointClusters().ToList();

            //stopwatch2.Stop();

            //var elapsedTimeForcoordinateClustersDictionary = stopwatch2.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinder BruteForce Dictionary Took: {elapsedTimeForcoordinateClustersDictionary} Milliseconds");
            //////==========================================================================================


            //////==========================================================================================
            //var stopwatch3 = new Stopwatch();
            //stopwatch3.Start();

            //var coordinateClustersHashSet = new PathClusterFinderWithHashSet(hashSetOfCoordinates, 500).GetPointClusters().ToList();

            //stopwatch3.Stop();

            //var elapsedTimeForcoordinateClustersHashSet = stopwatch3.ElapsedMilliseconds;

            //Console.WriteLine($"PathClusterFinder BruteForce HashSet Took: {elapsedTimeForcoordinateClustersHashSet} Milliseconds");
            //////==========================================================================================


            //==========================================================================================
            var stopwatch4 = new Stopwatch();
            stopwatch4.Start();

            var nearestPointsKdTreeWithCoordinateLinear = new KDTreeCoordinate<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                                                             .NearestNeighborClusterLinear(500, arrayOfCoordinates).ToList();

            stopwatch4.Stop();

            var elapsedTimeKdTreeWithCoordinateLinear = stopwatch4.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder KdTree With Coordinate Linear Took: {elapsedTimeKdTreeWithCoordinateLinear} Milliseconds");
            //==========================================================================================


            //==========================================================================================
            var stopwatch5 = new Stopwatch();
            stopwatch5.Start();

            var nearestPointsKdTreeWithCoordinateRadial = new KDTreeCoordinate<Coordinate>(2, arrayOfCoordinates, Utilities.L2Norm_Squared_Coordinate)
                                                             .NearestNeighborClusterRadial(Radius.SuperSlowButAccurate, pointsPerCluster: 500, coordinates: arrayOfCoordinates).ToList();
            stopwatch5.Stop();

            var elapsedTimeKdTreeWithCoordinateRadial = stopwatch5.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder KdTree With Coordinate Radial Took: {elapsedTimeKdTreeWithCoordinateRadial} Milliseconds");
            //==========================================================================================


            //==========================================================================================
            var stopwatch6 = new Stopwatch();
            stopwatch6.Start();

            var nearestPontsKdTreeLinearPristine = new KDTree<double, Coordinate>(2, coordinatesAsDoubleArray, arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                                                            .NearestNeighborClusterLinear(500, arrayOfCoordinates).ToList();

            stopwatch6.Stop();

            var elapsedTimeKdTreeLinearPristine = stopwatch6.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder KdTree With Double Array Linear Took: {elapsedTimeKdTreeLinearPristine} Milliseconds");
            //==========================================================================================


            //==========================================================================================
            var stopwatch7 = new Stopwatch();
            stopwatch7.Start();

            var nearestPointsKdTreePristineRadial = new KDTree<double, Coordinate>(2, coordinatesAsDoubleArray, arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                                                             .NearestNeighborClusterRadial(Radius.SuperSlowButAccurate, pointsPerCluster: 500, coordinates: arrayOfCoordinates).ToList();

            stopwatch7.Stop();

            var elapsedTimeKdTreeRadialPristine = stopwatch7.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder KdTree With Double Array Radial Took: {elapsedTimeKdTreeRadialPristine} Milliseconds");
            //==========================================================================================
        }
    }
}