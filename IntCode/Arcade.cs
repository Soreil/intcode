using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public void GetScreen()
        {
            List<(long x, long y, Tile t)> trips = new List<(long x, long y, Tile t)>();
            while (!Program.halted) trips.Add(GetTriplet());
            Screen = trips;
        }

        public int BlockCount() => Screen.Where((x) => x.t == Tile.block).Count();

        public void AddCoin()
        {
            Program.n[0] = 2;
        }
    }
}
