//using System;
//using System.Collections.Generic;

//namespace ClosestNeighbourSearchAlgorithms.ClosestPairOfPointSweepLine
//{
//    public class ClosestPairOfPointFinder
//    {
//        readonly List<Coordinate> _points = new List<Coordinate>();

//        public int Distance { get; private set; }

//        const double MaxDistance = 10000;

//        public ClosestPairOfPointFinder(int n)
//        {
//            var rand = new Random();
//            for (var i = 0; i < n; i++)
//            {
//                _points.Add(new Coordinate
//                {
//                    Latitude = rand.Next(-10000, 10000),
//                    Longitude = rand.Next(-10000, 10000),
//                });
//            }

//            _points.Sort();
//        }

//        public Coordinate[] Run()
//        {
//            return ClosestPair(_points);
//        }

//        public Coordinate[] Run2()
//        {
//            return NaiveClosestPair(_points);
//        }

//        // adapted from: http://www.baptiste-wicht.com/2010/04/closest-pair-of-point-plane-sweep-algorithm/
//        static Coordinate[] ClosestPair(IEnumerable<Coordinate> points)
//        {
//            var closestPair = new Coordinate[2];

//            // When we start the min distance is the infinity
//            var crtMinDist = MaxDistance;

//            // Get the points and sort them
//            var sorted = new List<Coordinate>();
//            sorted.AddRange(points);
//            sorted.Sort(Coordinate.XComparer.XCompare);

//            // When we start the left most candidate is the first one
//            var leftMostCandidateIndex = 0;

//            // Vertically sorted set of candidates
//            var candidates = new TreeSet<Coordinate>(new Coordinate.YComparer()); // C5 data structure

//            // For each point from left to right
//            foreach (var current in sorted)
//            {
//                // Shrink the candidates
//                while (current.Latitude - sorted[leftMostCandidateIndex].Latitude > crtMinDist)
//                {
//                    candidates.Remove(sorted[leftMostCandidateIndex]);
//                    leftMostCandidateIndex++;
//                }

//                // Compute the y head and the y tail of the candidates set
//                var head = new Coordinate { Latitude = current.Latitude, Longitude = checked(current.Longitude - crtMinDist) };
//                var tail = new Coordinate { Latitude = current.Latitude, Longitude = checked(current.Longitude + crtMinDist) };

//                // We take only the interesting candidates in the y axis
//                var subset = candidates.RangeFromTo(head, tail);
//                foreach (var point in subset)
//                {
//                    var distance = current.Distance(point);
//                    if (distance < 0) throw new InvalidOperationException("number overflow");

//                    // Simple min computation
//                    if (distance < crtMinDist)
//                    {
//                        crtMinDist = distance;
//                        closestPair[0] = current;
//                        closestPair[1] = point;
//                    }
//                }

//                // The current point is now a candidate
//                candidates.Add(current);
//            }

//            return closestPair;
//        }

//        static Coordinate[] NaiveClosestPair(IEnumerable<Coordinate> points)
//        {
//            var min = MaxDistance;

//            var closestPair = new Coordinate[2];

//            foreach (var p1 in points)
//            {
//                foreach (var p2 in points)
//                {
//                    if (p1.Equals(p2)) continue;

//                    var dist = p1.Distance(p2);
//                    if (dist < min)
//                    {
//                        min = dist;
//                        closestPair[0] = p1;
//                        closestPair[1] = p2;
//                    }
//                }
//            }
//            return closestPair;
//        }
//    }
//}