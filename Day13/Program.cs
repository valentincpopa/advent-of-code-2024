using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day13
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputChunks = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Chunk(3)
                .ToList();

            var regex = new Regex(".*X[\\+=]+(?<X>\\d+), Y[\\+=]+(?<Y>\\d+)");

            var machines = new List<Machine>();

            foreach (var chunk in inputChunks)
            {
                machines.Add(BuildMachine(regex, chunk));
            }

            var firstPartResult = 0D;
            var secondPartResult = 0D;

            foreach (var machine in machines)
            {
                var hasNoSolution = (machine.ButtonA.X * machine.ButtonB.Y) - (machine.ButtonB.X * machine.ButtonA.Y) == 0;
                if (hasNoSolution)
                {
                    continue;
                }

                firstPartResult += ComputeNoOfTokens(machine);

                machine.Result.X = machine.Result.X + 10000000000000;
                machine.Result.Y = machine.Result.Y + 10000000000000;
                secondPartResult += ComputeNoOfTokens(machine);
            }

            Console.WriteLine(firstPartResult);
            Console.WriteLine(secondPartResult);
        }

        private static double ComputeNoOfTokens(Machine machine)
        {
            var aButtonPresses = GetPressesForA(machine);
            var bButtonPresses = GetPressesForB(machine);

            if (Math.Round(aButtonPresses) != aButtonPresses || Math.Round(bButtonPresses) != bButtonPresses)
            {
                return 0;
            }

            return Math.Round(aButtonPresses) * 3 + Math.Round(bButtonPresses);
        }

        private static double GetPressesForA(Machine machine)
        {
            return (double)((machine.Result.X * machine.ButtonB.Y) - (machine.ButtonB.X * machine.Result.Y))
                / ((machine.ButtonA.X * machine.ButtonB.Y) - (machine.ButtonB.X * machine.ButtonA.Y)); 
        }

        private static double GetPressesForB(Machine machine)
        {
            return (double)((machine.ButtonA.X * machine.Result.Y) - (machine.Result.X * machine.ButtonA.Y))
                / ((machine.ButtonA.X * machine.ButtonB.Y) - (machine.ButtonB.X * machine.ButtonA.Y)); 
        }

        private static Machine BuildMachine(Regex regex, string[] chunk)
        {
            return new Machine
            {
                ButtonA = GetPosition(regex.Match(chunk[0])),
                ButtonB = GetPosition(regex.Match(chunk[1])),
                Result = GetPosition(regex.Match(chunk[2]))
            };
        }

        private static Position GetPosition(Match buttonA)
        {
            return new Position
            {
                X = int.Parse(buttonA.Groups["X"].Value),
                Y = int.Parse(buttonA.Groups["Y"].Value)
            };
        }
    }

    internal class Machine
    {
        public Position ButtonA { get; set; }

        public Position ButtonB { get; set; }

        public Position Result { get; set; }
    }

    internal class Position
    {
        public long X { get; set; }
        public long Y { get; set; }
    }
}
