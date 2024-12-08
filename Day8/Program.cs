using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day8
{
    internal class Program
    {
        private static int _iLength;
        private static int _jLength;

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            _iLength = input.Count;
            _jLength = input[0].Length;

            var antennaGroups = GenerateAntennaGroups(input);
            var antennaPairs = GenerateAntennaPairs(antennaGroups);

            Solve(antennaPairs, false);
            Solve(antennaPairs, true);
        }

        private static void Solve(Dictionary<char, List<List<(int i, int j)>>> antennaPairs, bool canHaveMultipleAntinodes)
        {
            var antinodes = GenerateAntinodes(antennaPairs, canHaveMultipleAntinodes);
            var uniqueAntinodes = antinodes.Values.SelectMany(x => x).Distinct().Count();
            Console.WriteLine(uniqueAntinodes);
        }

        private static Dictionary<char, HashSet<(int i, int j)>> GenerateAntinodes(Dictionary<char, List<List<(int i, int j)>>> antennaPairs, bool canHaveMultipleAntinodes)
        {
            var antinodes = new Dictionary<char, HashSet<(int i, int j)>>();

            foreach (var antenna in antennaPairs)
            {
                var antennaAntinodes = new HashSet<(int i, int j)>();

                foreach (var pair in antenna.Value)
                {
                    GeneratePairAntinodes(antennaAntinodes, pair, 0, 1, canHaveMultipleAntinodes);
                    GeneratePairAntinodes(antennaAntinodes, pair, 1, 0, canHaveMultipleAntinodes);
                }

                antinodes.Add(antenna.Key, antennaAntinodes);
            }

            return antinodes;
        }

        private static void GeneratePairAntinodes(HashSet<(int i, int j)> groupPairs, List<(int i, int j)> pair, int firstPairIndex, int secondPairIndex, bool canHaveMultipleAntinodes)
        {
            var diffI = pair[firstPairIndex].i - pair[secondPairIndex].i;
            var diffJ = pair[firstPairIndex].j - pair[secondPairIndex].j;

            var antinodeI = pair[firstPairIndex].i + diffI;
            var antinodeJ = pair[firstPairIndex].j + diffJ;

            if (canHaveMultipleAntinodes)
            {
                groupPairs.Add((pair[firstPairIndex].i, pair[firstPairIndex].j));
            }

            while (ValuesInBounds(antinodeI, antinodeJ))
            {
                groupPairs.Add((antinodeI, antinodeJ));

                if (!canHaveMultipleAntinodes)
                {
                    return;
                }

                antinodeI += diffI;
                antinodeJ += diffJ;
            }
        }


        private static List<IGrouping<char, (int i, int j)>> GenerateAntennaGroups(List<string> input)
        {
            return input
                .SelectMany((row, i) => row
                    .Select((value, j) => (freq: value, i, j)))
                .Where(x => x.freq != '.')
                .GroupBy(x => x.freq, sel => (sel.i, sel.j))
                .ToList();
        }

        private static Dictionary<char, List<List<(int i, int j)>>> GenerateAntennaPairs(List<IGrouping<char, (int i, int j)>> antennaGroups)
        {
            var antennaPairs = new Dictionary<char, List<List<(int i, int j)>>>();

            foreach (var antennaGroup in antennaGroups)
            {
                var groupPairs = new List<List<(int i, int j)>>();
                var antennas = antennaGroup.ToList();

                for (var i = 0; i < antennas.Count; i++)
                {
                    for (var j = i + 1; j < antennas.Count; j++)
                    {
                        groupPairs.Add([antennas[i], antennas[j]]);
                    }
                }

                antennaPairs.Add(antennaGroup.Key, groupPairs);
            }

            return antennaPairs;
        }

        private static bool ValuesInBounds(int newI, int newJ)
        {
            return newI >= 0 && newI < _iLength
                && newJ >= 0 && newJ < _jLength;
        }
    }
}
