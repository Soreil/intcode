using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntCode
{
    class Painter
    {
        private readonly Program program;
        private Dictionary<(int x, int y), int> map;
        (int x, int y) position;
        int OutputsReceivedSinceLastInput;
        Direction direction;

        private HashSet<(int x, int y)> WasPainted;

        enum Direction
        {
            North,
            East,
            South,
            West
        }
        public Painter(Program p)
        {
            program = p;
            map = new Dictionary<(int x, int y), int>();
            WasPainted = new HashSet<(int x, int y)>();
            OutputsReceivedSinceLastInput = 0;
            position = (0, 0);
            direction = Direction.North;
            program.output = Output;
        }

        public void Run() => program.Run();
        public bool Done => program.halted;
        public int WhiteTileCount => WasPainted.Count;

        void Output(long l)
        {
            if (OutputsReceivedSinceLastInput == 0)
            {
                SetColour((int)l);
                OutputsReceivedSinceLastInput = 1;
            }
            else if (OutputsReceivedSinceLastInput == 1)
            {
                TurnRobot(l);
                MoveRobot();
                program.AddInput(new List<long>() { GetColour() });
                OutputsReceivedSinceLastInput = 0;
            }

        }

        private long GetColour()
        {
            map.TryGetValue(position, out int value);
            return value;
        }

        private void MoveRobot()
        {
            switch (direction)
            {
                case Direction.North:
                    position.y--;
                    break;
                case Direction.East:
                    position.x++;
                    break;
                case Direction.South:
                    position.y++;
                    break;
                case Direction.West:
                    position.x--;
                    break;
            }
        }

        private void SetColour(int l)
        {
            map[position] = l;
            WasPainted.Add(position);
        }

        private void TurnRobot(long l)
        {
            if (l == 0)
            {
                direction--;
                if (direction < Direction.North) direction = Direction.West;
            }
            else if (l == 1)
            {
                direction++;
                if (direction > Direction.West) direction = Direction.North;
            }
        }

        internal void Draw()
        {
            var yMin = 0;
            var yMax = 10;
            var xMin = 0;
            var xMax = 60;

            for (int y = yMin; y < yMax; y++)
            {
                for (int x = xMin; x < xMax; x++)
                {
                    map.TryGetValue((x, y), out int value);

                    var arg = (value == 1) ? "#" : ".";
                    Console.Write(arg);
                }
                Console.WriteLine();
            }
        }
    }
}
