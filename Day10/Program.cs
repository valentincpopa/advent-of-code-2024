using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day10
{
    internal class Program
    {
        private static int _iLength;
        private static int _jLength;

        private static readonly List<(int i, int j)> _directions =
        [
            (-1, 0), // up
            (0, 1),  // right
            (1, 0),  // down
            (0, -1), // left
        ];

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var startingPoints = input
                .SelectMany((row, rowIndex) => row
                    .Select((character, columnIndex) => (value: character - '0', rowIndex, columnIndex)))
                .Where(x => x.value == 0)
                .ToList();

            _iLength = input.Count;
            _jLength = input[0].Length;

            Solve(input, startingPoints, true);
            Solve(input, startingPoints, false);
        }

        private static void Solve(List<string> input, List<(int value, int rowIndex, int columnIndex)> startingPoints, bool isFirstPart)
        {
            var ratings = 0;

            foreach (var startingPoint in startingPoints)
            {
                ratings += GetTrailRating(input, startingPoint, isFirstPart);
            }

            Console.WriteLine(ratings);
        }

        private static int GetTrailRating(List<string> input, (int value, int rowIndex, int columnIndex) startingPoint, bool isFirstPart)
        {
            var ratings = 0;
            var visited = new HashSet<(int value, int i, int j)>();
            var queue = new Queue<(int value, int i, int j)>();

            queue.Enqueue(startingPoint);

            while (queue.Count > 0)
            {
                var currentPosition = queue.Dequeue();
                visited.Add(currentPosition);

                foreach (var direction in _directions)
                {
                    var newI = currentPosition.i + direction.i;
                    var newJ = currentPosition.j + direction.j;

                    if (!ValuesInBounds(newI, newJ))
                    {
                        continue;
                    }

                    var newPosition = (value: input[newI][newJ] - '0', newI, newJ);

                    if ((isFirstPart && visited.Contains(newPosition)) || newPosition.value - currentPosition.value != 1)
                    {
                        continue;
                    }

                    visited.Add(newPosition);

                    if (newPosition.value == 9)
                    {
                        ratings++;
                    }
                    else
                    {
                        queue.Enqueue(newPosition);
                    }
                }
            }

            return ratings;
        }

        private static bool ValuesInBounds(int newI, int newJ)
        {
            return newI >= 0 && newI < _iLength
                && newJ >= 0 && newJ < _jLength;
        }
    }
}
