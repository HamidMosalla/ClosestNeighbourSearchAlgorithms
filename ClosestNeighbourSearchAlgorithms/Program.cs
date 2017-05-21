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
            var numberOfCoordinates = 1000000;
            var coordinatesAsDoubleArray = Utilities.GenerateDoubles(numberOfCoordinates, range: 10000);
            var arrayOfCoordinates = Utilities.GenerateCoordinatesFromArray(coordinatesAsDoubleArray).ToArray();
            var listOfCoordinates = arrayOfCoordinates.ToList();
            var dictionaryOfCoordinates = arrayOfCoordinates.ToDictionary(c => c.CoordinateId, c => c);
            var hashSetOfCoordinates = arrayOfCoordinates.ToHashSet();

            Console.WriteLine($"For {numberOfCoordinates.ToString("n0")} Points:");


            ////==========================================================================================
            var stopwatch2 = new Stopwatch();
            stopwatch2.Start();

            var coordinateClustersList = new PathClusterFinderWithList(listOfCoordinates, 500).GetPointClusters().ToList();

            stopwatch2.Stop();

            var elapsedTimeForcoordinateClustersList = stopwatch2.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder BruteForce List Took: {elapsedTimeForcoordinateClustersList} Milliseconds");
            ////==========================================================================================


            ////==========================================================================================
            var stopwatch3 = new Stopwatch();
            stopwatch3.Start();

            var coordinateClustersDictionary = new PathClusterFinderWithDictionary(dictionaryOfCoordinates, 500).GetPointClusters().ToList();

            stopwatch3.Stop();

            var elapsedTimeForcoordinateClustersDictionary = stopwatch3.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder BruteForce Dictionary Took: {elapsedTimeForcoordinateClustersDictionary} Milliseconds");
            ////==========================================================================================


            ////==========================================================================================
            var stopwatch4 = new Stopwatch();
            stopwatch4.Start();

            var coordinateClustersHashSet = new PathClusterFinderWithHashSet(hashSetOfCoordinates, 500).GetPointClusters().ToList();

            stopwatch4.Stop();

            var elapsedTimeForcoordinateClustersHashSet = stopwatch4.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder BruteForce HashSet Took: {elapsedTimeForcoordinateClustersHashSet} Milliseconds");
            ////==========================================================================================


            //==========================================================================================
            var stopwatch8 = new Stopwatch();
            stopwatch8.Start();

            var nearestPontsKdTreeLinearPristine = new KDTree<double, Coordinate>(2, coordinatesAsDoubleArray, arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                                                            .NearestNeighborClusterLinear(500, arrayOfCoordinates).ToList();

            stopwatch8.Stop();

            var elapsedTimeKdTreeLinearPristine = stopwatch8.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder KdTree Linear Took: {elapsedTimeKdTreeLinearPristine} Milliseconds");
            //==========================================================================================


            //==========================================================================================
            var stopwatch9 = new Stopwatch();
            stopwatch9.Start();

            var nearestPointsKdTreePristineRadial = new KDTree<double, Coordinate>(2, coordinatesAsDoubleArray, arrayOfCoordinates, Utilities.L2Norm_Squared_Double)
                                                             .NearestNeighborClusterRadial(radius: 1000, pointsPerCluster: 500, coordinates: arrayOfCoordinates).ToList();
            stopwatch9.Stop();

            var elapsedTimeKdTreeRadialPristine = stopwatch9.ElapsedMilliseconds;

            Console.WriteLine($"PathClusterFinder KdTree Radial Took: {elapsedTimeKdTreeRadialPristine} Milliseconds");
            //==========================================================================================
        }

    }
}