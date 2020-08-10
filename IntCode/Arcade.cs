using System;
using System.Collections.Generic;
using System.Linq;

namespace IntCode
{
    class Arcade
    {
        readonly Program Program;
        public List<(long x, long y, Tile t)> Screen = new List<(long x, long y, Tile t)>();

        public int Score = 0;

        public Arcade(Program program) : this(program, false)
        {
        }

        public Arcade(Program program, bool play)
        {
            Program = program;
            if (play) AddCoin();
        }

        public enum Tile
        {
            empty = 0,
            wall = 1,
            block = 2,
            paddle = 3,
            ball = 4
        }

        private (long x, long y, Tile t) GetTriplet()
        {
            List<long> trip = new List<long>() { 0, 0, 0 };
            int index = 0;
            Program.output = (x) => trip[index++] = x;

            while (index < 3)
                if (Program.Step() && !Program.halted)
                    throw new Exception("Hit an input instruction instead of a triplet");
            return (trip[0], trip[1], (Tile)trip[2]);
        }
        public void GetUntilEndOrInputError()
        {
            List<(long x, long y, Tile t)> trips = new List<(long x, long y, Tile t)>();
            while (!Program.halted)
            {
                try { trips.Add(GetTriplet()); }
                catch { break; }
            }
            Screen = trips;
        }

        public int BlockCount() => Screen.Where((x) => x.t == Tile.block).Count();
        public void Play()
        {
            (long x, long y, Tile t) paddle = (0, 0, Tile.paddle);
            while (true)
            {
                GetUntilEndOrInputError();
                int score = (int)Screen.Where((x) => x.x == -1 && x.y == 0).FirstOrDefault().t;
                if (score != 0) Score = score;
                if (Program.halted)
                    return;
                var ball = Screen.Where((x) => x.t == Tile.ball).First();
                var possiblePaddle = Screen.Where((x) => x.t == Tile.paddle);
                if (possiblePaddle.Count() != 0) paddle = possiblePaddle.First();

                int move = 0;
                if (ball.x < paddle.x) move = -1;
                if (ball.x > paddle.x) move = 1;
                Program.AddInput(new List<long>() { move });
            }
        }
        private void AddCoin()
        {
            Program.n[0] = 2;
        }
    }
}
