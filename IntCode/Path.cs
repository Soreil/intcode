using System;
using System.Collections.Generic;
using System.Linq;

namespace IntCode
{
    class Path
    {
        readonly Func<Point, List<Point>> Neighbours;
        readonly Func<Point, Point, int> D;
        //We need to allow input of the tables to look up points
        public Path(Func<Point, List<Point>> neighbours, Func<Point, Point, int> d)
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
            while (totalPath.Contains(current))
            {
                current = cameFrom[current];
                totalPath.Add(current);
            }
            totalPath.Reverse();
            return totalPath;
        }
        private List<Point> A_Star(Point start, Point goal, Func<Point, int> h)
        {
            HashSet<Point> openSet = new HashSet<Point> { start };

            Dictionary<Point, Point> cameFrom = new Dictionary<Point, Point>();

            Dictionary<Point, int> gScore = new Dictionary<Point, int>
            {
                [start] = 0
            };

            Dictionary<Point, int> fScore = new Dictionary<Point, int>
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
}
