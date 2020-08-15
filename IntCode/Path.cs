using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace IntCode
{
    public class Score : Dictionary<Point, int>
    {
        public Score() : base()
        { }

        public int this[Point p]
        {
            get
            {
                if (!ContainsKey(p)) return int.MaxValue;
                else return base[p];
            }
            set
            {
                base[p] = value;
            }
        }
    }

    class Path
    {
        readonly Func<Point, IEnumerable<Point>> Neighbours;
        readonly Func<Point, Point, int> D;
        //We need to allow input of the tables to look up points
        public Path(Func<Point, IEnumerable<Point>> neighbours, Func<Point, Point, int> d)
        {
            Neighbours = neighbours;
            D = d;
        }
        private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            var totalPath = new List<Point>
            {
                current
            };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath;
        }
        public List<Point> A_Star(Point start, Point goal, Func<Point, int> h)
        {
            HashSet<Point> openSet = new HashSet<Point> { start };

            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            Score gScore = new Score
            {
                [start] = 0
            };

            Score fScore = new Score
            {
                [start] = h(start)
            };

            while (openSet.Any())
            {
                var current = openSet.OrderBy((x) => fScore[x]).First();
                if (current.X == goal.X && current.Y == goal.Y)
                    return ReconstructPath(cameFrom, current);

                openSet.Remove(current);
                foreach (var neighbour in Neighbours(current))
                {
                    var tentative_gScore = gScore[current] + D(current, neighbour);
                    if (tentative_gScore < gScore[neighbour])
                    {
                        cameFrom[neighbour] = current;
                        gScore[neighbour] = tentative_gScore;
                        fScore[neighbour] = gScore[neighbour] + h(neighbour);
                        openSet.Add(neighbour);
                    }
                }
            }
            throw new Exception("No possible path");
        }
    }

    public class PathTests
    {
        [Test]
        public void SimplePathNoObstacles()
        {
            Point start = new Point(0, 0);
            Point end = new Point(0, 1);

            HashSet<Point> points = new HashSet<Point> { start, end };

            Func<Point, int> h = (x) => Distance(end, x);

            Path p = new Path(p1 => neighbours(p1, points), Distance);
            var moves = p.A_Star(start, end, h);
            foreach (var m in moves) Console.WriteLine(m.X.ToString() + ", " + m.Y.ToString());
            Assert.AreEqual(1, moves.Count);
        }

        private static int Distance(Point lhs, Point rhs) =>
                      Math.Abs(lhs.X - rhs.X) + Math.Abs(lhs.Y - rhs.Y);

        private static IEnumerable<Point> neighbours(Point p, HashSet<Point> points) =>
                    new List<Point> {
                new Point(p.X-1,p.Y),
                new Point(p.X,p.Y-1),
                new Point(p.X+1,p.Y),
                new Point(p.X,p.Y+1),
                    }.Where((x) => points.Contains(x));

        [Test]
        public void EmptyPointReturnsInfinity()
        {
            var p = new Score();
            Assert.AreEqual(int.MaxValue, p[new Point()]);
            p.Add(new Point(10, 20), 10);
            Assert.AreNotEqual(int.MaxValue, p[new Point(10, 20)]);
        }
    }
}
