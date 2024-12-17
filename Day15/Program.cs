using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    internal class Program
    {
        private static readonly Dictionary<char, (int row, int column)> _directions = new Dictionary<char, (int i, int j)>
        {
            { '^', (-1, 0) },  // up
            { '>', (0, 1) },   // right
            { 'v', (1, 0) },   // down
            { '<', (0, -1) }   // left
        };

        static void Main(string[] args)
        {
            var allLines = File.ReadAllLines("./input.txt").ToList();

            var map = allLines
                .TakeWhile(x => !x.Equals(string.Empty))
                .Select(x => x.ToCharArray().ToList())
                .ToList();

            var startingPoint = map
                .Select((line, rowIndex) => (row: rowIndex, column: line.IndexOf('@')))
                .FirstOrDefault(result => result.column != -1);

            var updatesString = string.Join(string.Empty, allLines.Skip(map.Count + 1));
            var updates = GetStackedUpdates(updatesString);

            foreach (var update in updates)
            {
                var offset = _directions[update.character];

                var newY = startingPoint.row + offset.row;
                var newX = startingPoint.column + offset.column;

                var stack = new Stack<char>();
                stack.Push(map[startingPoint.row][startingPoint.column]);

                var noOfMoves = ComputeNoOfMoves(map, update, offset, ref newY, ref newX, stack);

                var originalStartingPoint = startingPoint;
                startingPoint = (row: startingPoint.row + noOfMoves * offset.row, column: startingPoint.column + noOfMoves * offset.column);

                newY -= offset.row;
                newX -= offset.column;

                ProcessUpdates(map, offset, newY, newX, stack, noOfMoves, originalStartingPoint);
            }

            Console.WriteLine(ComputeGpsCoordinatesSum(map));
        }

        private static void ProcessUpdates(List<List<char>> map, (int row, int column) offset, int newY, int newX, Stack<char> stack, int noOfMoves, (int row, int column) originalStartingPoint)
        {
            while (newY != originalStartingPoint.row || newX != originalStartingPoint.column)
            {
                if (stack.Count > 0)
                {
                    map[newY][newX] = stack.Pop();
                }
                else
                {
                    map[newY][newX] = '.';
                }

                newY -= offset.row;
                newX -= offset.column;
            }

            if (noOfMoves != 0)
            {
                map[newY][newX] = '.';
            }
        }

        private static int ComputeNoOfMoves(List<List<char>> map, (char character, int count) update, (int row, int column) offset, ref int newY, ref int newX, Stack<char> stack)
        {
            var noOfMoves = 0;

            while (map[newY][newX] != '#' && noOfMoves != update.count)
            {
                if (map[newY][newX] == '.')
                {
                    noOfMoves++;
                }
                else if (map[newY][newX] == 'O')
                {
                    stack.Push('O');
                }

                newY += offset.row;
                newX += offset.column;
            }

            return noOfMoves;
        }

        private static int ComputeGpsCoordinatesSum(List<List<char>> map)
        {
            var coordinatesSum = 0;

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == 'O')
                    {
                        coordinatesSum += 100 * i + j;
                    }
                }
            }

            return coordinatesSum;
        }

        private static List<(char character, int count)> GetStackedUpdates(string updatesString)
        {
            var updates = new List<(char character, int count)>();
            var previous = (character: updatesString[0], count: 1);

            for (int i = 1; i < updatesString.Length; i++)
            {
                if (updatesString[i] == previous.character)
                {
                    previous.count++;
                }
                else
                {
                    updates.Add(previous);
                    previous = (character: updatesString[i], count: 1);
                }
            }

            updates.Add(previous);
            return updates;
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
