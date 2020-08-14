using System;

namespace IntCode
{
    public struct Point
    {
        readonly public int X;
        readonly public int Y;
        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }
        public static Point operator +(Point lhs, Point rhs)
            => new Point(lhs.X + rhs.X, lhs.Y + rhs.Y);
        public static Point operator +(Point lhs, OxygenSystem.Direction rhs)
            => rhs switch
            {
                OxygenSystem.Direction.north => lhs + new Point(0, -1),
                OxygenSystem.Direction.east => lhs + new Point(1, 0),
                OxygenSystem.Direction.south => lhs + new Point(0, 1),
                OxygenSystem.Direction.west => lhs + new Point(-1, 0),
                _ => throw new NotImplementedException(),
            };
        //public static bool operator ==(Point lhs, Point rhs)
        //    => lhs.X == rhs.X && lhs.Y == rhs.Y;

        //public static bool operator !=(Point lhs, Point rhs) => !(lhs == rhs);

    }
}
