﻿using System;
using System.Collections.Generic;
using System.Linq;
using ClosestNeighbourSearchAlgorithms.ModelsAndContracts;

namespace ClosestNeighbourSearchAlgorithms.KDTree
{
    public static class Utilities
    {
        #region Metrics

        public static Func<float[], float[], double> L2Norm_Squared_Float = (x, y) =>
        {
            float dist = 0f;
            for (int i = 0; i < x.Length; i++)
            {
                dist += (x[i] - y[i]) * (x[i] - y[i]);
            }

            return dist;
        };

        public static Func<double[], double[], double> L2Norm_Squared_Double = (x, y) =>
        {
            double dist = 0f;
            for (int i = 0; i < x.Length; i++)
            {
                dist += (x[i] - y[i]) * (x[i] - y[i]);
            }

            return dist;
        };

        public static Func<Coordinate, Coordinate, double> L2Norm_Squared_Coordinate = (x, y) =>
        {
            var dx = x.Latitude - y.Latitude;
            var dy = x.Longitude - y.Longitude;

            var dist = (dx * dx) + (dy * dy);
            dist = Math.Sqrt(dist);

            return dist;
        };

        #endregion

        #region Data Generation

        public static double[][] GenerateDoubles(int points, double range, int dimensions)
        {
            var data = new List<double[]>();
            var random = new Random();

            for (var i = 0; i < points; i++)
            {
                var array = new double[dimensions];
                for (var j = 0; j < dimensions; j++)
                {
                    array[j] = random.NextDouble() * range;
                }
                data.Add(array);
            }

            return data.ToArray();
        }

        public static double[][] GenerateDoubles(int points, double range)
        {
            var data = new List<double[]>();
            var random = new Random();

            for (int i = 0; i < points; i++)
            {
                data.Add(new double[] { (random.NextDouble() * range), (random.NextDouble() * range) });
            }

            return data.ToArray();
        }

        public static float[][] GenerateFloats(int points, double range)
        {
            var data = new List<float[]>();
            var random = new Random();

            for (int i = 0; i < points; i++)
            {
                data.Add(new float[] { (float)(random.NextDouble() * range), (float)(random.NextDouble() * range) });
            }

            return data.ToArray();
        }

        public static float[][] GenerateFloats(int points, double range, int dimensions)
        {
            var data = new List<float[]>();
            var random = new Random();

            for (var i = 0; i < points; i++)
            {
                var array = new float[dimensions];
                for (var j = 0; j < dimensions; j++)
                {
                    array[j] = (float)(random.NextDouble() * range);
                }
                data.Add(array);
            }

            return data.ToArray();
        }

        #endregion


        #region Searches

        public static T[] LinearSearch<T>(T[][] data, T[] point, Func<T[], T[], float> metric)
        {
            var bestDist = Double.PositiveInfinity;
            T[] bestPoint = null;

            for (int i = 0; i < data.Length; i++)
            {
                var currentDist = metric(point, data[i]);
                if (bestDist > currentDist)
                {
                    bestDist = currentDist;
                    bestPoint = data[i];
                }
            }

            return bestPoint;
        }

        public static T[] LinearSearch<T>(T[][] data, T[] point, Func<T[], T[], double> metric)
        {
            var bestDist = Double.PositiveInfinity;
            T[] bestPoint = null;

            for (int i = 0; i < data.Length; i++)
            {
                var currentDist = metric(point, data[i]);
                if (bestDist > currentDist)
                {
                    bestDist = currentDist;
                    bestPoint = data[i];
                }
            }

            return bestPoint;
        }

        public static Tuple<TPoint[], TNode> LinearSearch<TPoint, TNode>(TPoint[][] points, TNode[] nodes, TPoint[] target, Func<TPoint[], TPoint[], double> metric)
        {
            var bestIndex = 0;
            var bestDist = Double.MaxValue;

            for (int i = 0; i < points.Length; i++)
            {
                var currentDist = metric(points[i], target);
                if (bestDist > currentDist)
                {
                    bestDist = currentDist;
                    bestIndex = i;
                }
            }

            return new Tuple<TPoint[], TNode>(points[bestIndex], nodes[bestIndex]);
        }

        public static T[][] LinearRadialSearch<T>(T[][] data, T[] point, Func<T[], T[], double> metric, double radius)
        {
            var pointsInRadius = new BoundedPriorityList<T[], double>(data.Length, true);

            for (int i = 0; i < data.Length; i++)
            {
                var currentDist = metric(point, data[i]);
                if (radius >= currentDist)
                {
                    pointsInRadius.Add(data[i], currentDist);
                }
            }

            return pointsInRadius.ToArray();
        }

        public static Tuple<TPoint[], TNode>[] LinearRadialSearch<TPoint, TNode>(TPoint[][] points, TNode[] nodes, TPoint[] target, Func<TPoint[], TPoint[], double> metric, double radius)
        {
            var pointsInRadius = new BoundedPriorityList<int, double>(points.Length, true);

            for (int i = 0; i < points.Length; i++)
            {
                var currentDist = metric(target, points[i]);
                if (radius >= currentDist)
                {
                    pointsInRadius.Add(i, currentDist);
                }
            }

            return pointsInRadius.Select(idx => new Tuple<TPoint[], TNode>(points[idx], nodes[idx])).ToArray();
        }

        #endregion
    }
}