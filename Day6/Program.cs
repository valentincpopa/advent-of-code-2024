using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day6
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

        private static int _iLength;
        private static int _jLength;

        private static int _iStartingPosition;
        private static int _jStartingPosition;

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            _iLength = input.Count;
            _jLength = input[0].Length;

            var visitedPositions = new HashSet<(int i, int j)>();

            _iStartingPosition = input.FindIndex(x => x.Contains('^'));
            _jStartingPosition = input[_iStartingPosition].IndexOf('^');


            SolveFirstPart(input, visitedPositions);
            SolveSecondPart(input, visitedPositions);
        }

        private static void SolveFirstPart(List<string> input, HashSet<(int i, int j)> visitedPositions)
        {
            ProceedToNextToStartPosition(out var directionIndex, out var newI, out var newJ);

            while (ValuesInBounds(newI, newJ))
            {
                if (input[newI][newJ] == '#')
                {
                    ReturnToPreviousPosition(directionIndex, ref newI, ref newJ);
                    ProceedToNextDirection(ref directionIndex);
                }
                else
                {
                    visitedPositions.Add((newI, newJ));
                }

                ProceedToNextPosition(directionIndex, ref newI, ref newJ);
            }

            Console.WriteLine(visitedPositions.Count);
        }

        private static void SolveSecondPart(List<string> input, HashSet<(int i, int j)> visitedPositions)
        {
            var noOfLoops = 0;

            foreach (var visitedPosition in visitedPositions)
            {
                if (VisitedPositionAsObstacleTriggersLoop(input, visitedPosition))
                {
                    noOfLoops++;
                }
            }

            Console.WriteLine(noOfLoops);
        }

        private static bool VisitedPositionAsObstacleTriggersLoop(List<string> input, (int i, int j) visitedPosition)
        {
            var visitedObstaclesFromSameDirection = new HashSet<(int guardI, int guardJ, int directionI, int directionJ)>();
            ProceedToNextToStartPosition(out var directionIndex, out var newI, out var newJ);

            while (ValuesInBounds(newI, newJ))
            {
                if (input[newI][newJ] == '#' || (newI == visitedPosition.i && newJ == visitedPosition.j))
                {
                    if (!visitedObstaclesFromSameDirection.Add((newI, newJ, _directions[directionIndex].i, _directions[directionIndex].j)))
                    {
                        return true;
                    }

                    ReturnToPreviousPosition(directionIndex, ref newI, ref newJ);
                    ProceedToNextDirection(ref directionIndex);
                }

                ProceedToNextPosition(directionIndex, ref newI, ref newJ);
            }

            return false;
        }

        private static void ProceedToNextToStartPosition(out int directionIndex, out int newI, out int newJ)
        {
            directionIndex = 0;
            newI = _iStartingPosition + _directions[directionIndex].i;
            newJ = _jStartingPosition + _directions[directionIndex].j;
        }

        private static void ProceedToNextDirection(ref int directionIndex)
        {
            directionIndex = (directionIndex + 1) % _directions.Count;
        }

        private static void ProceedToNextPosition(int directionIndex, ref int newI, ref int newJ)
        {
            newI += _directions[directionIndex].i;
            newJ += _directions[directionIndex].j;
        }

        private static void ReturnToPreviousPosition(int directionIndex, ref int newI, ref int newJ)
        {
            newI -= _directions[directionIndex].i;
            newJ -= _directions[directionIndex].j;
        }

        private static bool ValuesInBounds(int newI, int newJ)
        {
            return newI >= 0 && newI < _iLength
                && newJ >= 0 && newJ < _jLength;
        }
    }
}
