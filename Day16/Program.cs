using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day16
{
    internal class Program
    {
        private static readonly List<(int i, int j)> _directions =
        [
            (-1, 0), // up
            (0, 1),  // right
            (1, 0),  // down
            (0, -1), // left
        ];

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("input.txt")
                            .Where(x => !string.IsNullOrWhiteSpace(x))
                            .Select(x => x.ToCharArray().ToList())
                            .ToList();

            var (startingPoint, endingPoint) = GetPathBoundaries(input);

            SolveFirstPart(input, startingPoint, endingPoint);
        }

        private static ((int i, int j) startingPoint, (int i, int j)) GetPathBoundaries(List<List<char>> input)
        {
            var rowIndexPairs = input
                .Select((row, index) => (row, index))
                .ToList();

            var startingPoint = rowIndexPairs
                .Where(x => x.row.Contains('S'))
                .Select(x => (i: x.index, j: x.row.IndexOf('S')))
                .First();
            var endingPoint = rowIndexPairs
                .Where(x => x.row.Contains('E'))
                .Select(x => (i: x.index, j: x.row.IndexOf('E')))
                .First();

            return (startingPoint, endingPoint);
        }

        private static void SolveFirstPart(List<List<char>> input, (int i, int j) startingPoint, (int i, int j) endingPoint)
        {
            var distances = input.SelectMany((row, i) => row.Select((column, j) => (i, j))
                .Select((coordinates, i) => (coordinates, details: (previous: ((int i, int j)?)null, distance: int.MaxValue))))
                .ToDictionary(x => x.coordinates, x => x.details);

            var priorityQueue = new PriorityQueue<((int i, int j)? previous, (int i, int j) current), int>();
            distances[startingPoint] = (null, 1);
            priorityQueue.Enqueue(((startingPoint.i, startingPoint.j - 1), startingPoint), 0);

            while (priorityQueue.Count > 0)
            {
                var entry = priorityQueue.Dequeue();

                var currentDistance = distances[entry.current];
                foreach (var direction in _directions)
                {
                    var directionPoint = (i: entry.current.i + direction.i, j: entry.current.j + direction.j);

                    if (input[directionPoint.i][directionPoint.j] == '#')
                    {
                        continue;
                    }

                    var directionDistance = distances[directionPoint];
                    var newDistance = currentDistance.distance + 1;

                    if (HasDifferentDirection(entry, directionPoint))
                    {
                        var previousDistance = distances[entry.current];
                        distances[entry.current] = (previousDistance.previous, previousDistance.distance + 1000);
                        newDistance += 1000;
                    }

                    if (newDistance < directionDistance.distance)
                    {
                        distances[directionPoint] = (entry.current, newDistance);
                        priorityQueue.Enqueue((entry.current, directionPoint), newDistance);
                    }
                }
            }

            Console.WriteLine(distances[distances[endingPoint].previous.Value].distance);
        }

        private static bool HasDifferentDirection(((int i, int j)? previous, (int i, int j) current) entry, (int i, int j) directionPoint)
        {
            return entry.previous != null && directionPoint.i != entry.previous.Value.i && directionPoint.j != entry.previous.Value.j;
        }

        private static void PrintCurrentPositions(List<List<char>> matrix)
        {
            for (int y = 0; y < matrix.Count; y++)
            {
                for (int x = 0; x < matrix[y].Count; x++)
                {
                    Console.Write(matrix[y][x]);
                }

                Console.Write(Environment.NewLine);
            }

            Console.Write(Environment.NewLine);
        }
    }
}