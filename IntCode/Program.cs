using System;
using System.Collections.Generic;

namespace IntCode
{
    internal class Program
    {
        public readonly List<long> n;
        private readonly Dictionary<long, Mode> modes;
        public Queue<long> Input;
        public Action<long> output;
        public long relativeBaseOffset;
        public bool halted;

        private long PC;
        public Program(string v) : this(v, new List<long>(), (x) => { })
        {

        }

        public Program(string v, List<long> inputs, Action<long> outputs)
        {
            n = new List<long>();
            modes = new Dictionary<long, Mode>();
            Input = new Queue<long>();
            AddInput(inputs);
            output = outputs;
            halted = false;
            relativeBaseOffset = 0;

            foreach (var t in v.Split(','))
                n.Add(long.Parse(t.Trim()));
            while (n.Count < 0x1000)
                n.Add(0);
        }

        enum OpCode
        {
            Add = 1,
            Multiply = 2,
            Input = 3,
            Output = 4,
            JumpNotZero = 5,
            JumpZero = 6,
            Less = 7,
            Equal = 8,
            AdjustRelativeBase = 9,
            Halt = 99
        }

        enum Mode
        {
            Position = 0,
            Immediate = 1,
            Relative = 2
        }

        internal void Run()
        {
            while (!Step()) ;
        }

        long Load(long arg)
        {
            modes.TryGetValue(arg, out Mode m);
            long at = n[(int)(PC + arg)];

            return m switch
            {
                Mode.Position => n[(int)at],
                Mode.Immediate => at,
                Mode.Relative => n[(int)(relativeBaseOffset + at)],
                _ => throw new NotImplementedException(),
            };
        }

        void Store(long arg, long at)
        {
            modes.TryGetValue(at, out Mode m);
            switch (m)
            {
                case Mode.Position:
                    {
                        var val = n[(int)(PC + at)];
                        n[(int)val] = arg;
                    }
                    break;
                case Mode.Relative:
                    {
                        var val = n[(int)(PC + at)];
                        n[(int)(relativeBaseOffset + val)] = arg;
                    }
                    break;
                case Mode.Immediate:
                    throw new Exception("Can't store in immediate mode");
            }
        }

        internal bool Step()
        {
            var OpWithModes = n[(int)PC++];
            OpCode op = (OpCode)(OpWithModes % 100);
            MakeModes(OpWithModes);
            switch (op)
            {
                case OpCode.Add:
                    Arithmetic((x, y) => x + y);
                    break;
                case OpCode.Multiply:
                    Arithmetic((x, y) => x * y);
                    break;
                case OpCode.Input:
                    if (Input.Count == 0)
                    {
                        PC--; //We will have to retry, dirty hack for now.
                        return true;
                    }
                    Store(Input.Dequeue(), 0);
                    PC += 1;
                    break;
                case OpCode.Output:
                    output(Load(0));
                    PC += 1;
                    break;
                case OpCode.JumpZero:
                    Jump((x) => x == 0);
                    break;
                case OpCode.JumpNotZero:
                    Jump((x) => x != 0);
                    break;
                case OpCode.Less:
                    Logical((x, y) => x < y);
                    break;
                case OpCode.Equal:
                    Logical((x, y) => x == y);
                    break;
                case OpCode.AdjustRelativeBase:
                    relativeBaseOffset += Load(0);
                    PC += 1;
                    break;
                case OpCode.Halt:
                    halted = true; //Done signal
                    return true;
                default:
                    throw new Exception("Unknown opcode:" + op.ToString());

            }
            return false;
        }

        private void MakeModes(long op)
        {
            modes.Clear();
            long ModeDigits = op / 100;
            for (long i = 0; ModeDigits != 0; i++)
            {
                modes[i] = (Mode)(ModeDigits % 10);
                ModeDigits /= 10;
            }
        }

        private void Arithmetic(Func<long, long, long> op)
        {
            var lhs = Load(0);
            var rhs = Load(1);
            Store(op(lhs, rhs), 2);
            PC += 3;
        }
        private void Logical(Func<long, long, bool> op)
        {
            var lhs = Load(0);
            var rhs = Load(1);
            Store(op(lhs, rhs) ? 1 : 0, 2);
            PC += 3;
        }

        private void Jump(Func<long, bool> op)
        {
            var arg = Load(0);
            var dest = Load(1);
            if (op(arg)) PC = dest;
            else PC += 2;
        }

        internal void AddInput(List<long> inputs)
        {
            foreach (var i in inputs)
                AddInput(i);
        }

        internal void AddInput(long input) => Input.Enqueue(input);
    }
}