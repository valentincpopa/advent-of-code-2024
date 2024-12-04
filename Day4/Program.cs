using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day4
{
    internal class Program
    {
        private static readonly IEnumerable<(int i, int j)> _offsets = new List<(int i, int j)>
        {
            (-1, -1), // top left
            (0, -1),  // center left
            (1, -1),  // bottom left

            (-1, 0),  // top center
            (1, 0),   // bottom center

            (-1, 1),  // top right
            (0, 1),   // center right
            (1, 1),   // bottom right
        };

        private static readonly List<((int i, int j) pair1, (int i, int j) pair2)> _offsetPairs = new List<((int i, int j) pair1, (int i, int j) pair2)>
        {
            ((-1, -1), /* top left */ (1, 1)), /* bottom right */
            ((-1, 1), /* top right */ (1, -1))  /* bottom left */
        };

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var matrix = input
                .Select(line => line
                    .Select(character => character)
                    .ToList())
                .ToList();

            var iLength = matrix.Count;
            var jLength = matrix[0].Count;

            SolveFirstPart(matrix, iLength, jLength);
            SolveSecondPart(matrix, iLength, jLength);
        }

        private static void SolveFirstPart(List<List<char>> matrix, int iLength, int jLength)
        {
            var noOfXmasWords = 0;

            for (int i = 0; i < iLength; i++)
            {
                for (int j = 0; j < jLength; j++)
                {
                    var current = matrix[i][j];

                    if (current != 'X')
                    {
                        continue;
                    }

                    var expectedChildValue = GetExpectedChildValue(current);
                    noOfXmasWords += ExploreAdjacentCharacters(matrix, iLength, jLength, i, j, expectedChildValue);
                }
            }

            Console.WriteLine(noOfXmasWords);
        }

        private static void SolveSecondPart(List<List<char>> matrix, int iLength, int jLength)
        {
            var noOfXmasWords = 0;

            for (int i = 0; i < iLength; i++)
            {
                for (int j = 0; j < jLength; j++)
                {
                    var current = matrix[i][j];

                    if (current != 'A')
                    {
                        continue;
                    }

                    if (FormsMasWord(matrix, i, j, iLength, jLength, _offsetPairs[0]) && FormsMasWord(matrix, i, j, iLength, jLength, _offsetPairs[1]))
                    {
                        noOfXmasWords++;
                    }
                }
            }

            Console.WriteLine(noOfXmasWords);
        }

        private static bool FormsMasWord(List<List<char>> matrix, int i, int j, int iLength, int jLength, ((int i, int j) pair1, (int i, int j) pair2) pairs)
        {
            var pair1 = (i: i + pairs.pair1.i, j: j + pairs.pair1.j);
            var pair2 = (i: i + pairs.pair2.i, j: j + pairs.pair2.j);

            var pairsInBounds =
                ValuesInBounds(pair1.i, pair1.j, iLength, jLength)
             && ValuesInBounds(pair2.i, pair2.j, iLength, jLength);

            if (!pairsInBounds)
            {
                return false;
            }

            return FormsMasWord(matrix[pair1.i][pair1.j], matrix[pair2.i][pair2.j]);
        }

        private static bool FormsMasWord(char firstValue, char secondValue)
        {
            return firstValue == 'M' && secondValue == 'S'
                 || secondValue == 'M' && firstValue == 'S';
        }

        private static int ExploreAdjacentCharacters(List<List<char>> matrix, int iLength, int jLength, int i, int j, char expectedChildValue)
        {
            var noOfXmasWords = 0;

            foreach (var offset in _offsets)
            {
                var newI = i + offset.i;
                var newJ = j + offset.j;

                var localExpectedChildValue = expectedChildValue;

                while (ValuesInBounds(newI, newJ, iLength, jLength)
                    && matrix[newI][newJ] == localExpectedChildValue)
                {
                    localExpectedChildValue = GetExpectedChildValue(matrix[newI][newJ]);
                    newI += offset.i;
                    newJ += offset.j;
                }

                if (localExpectedChildValue == '0')
                {
                    noOfXmasWords++;
                }
            }

            return noOfXmasWords;
        }

        private static bool ValuesInBounds(int newI, int newJ, int iLength, int jLength)
        {
            return newI >= 0 && newI < iLength
                && newJ >= 0 && newJ < jLength;
        }

        static char GetExpectedChildValue(char character)
        {
            return character switch
            {
                'X' => 'M',
                'M' => 'A',
                'A' => 'S',
                _ => '0'
            };
        }
    }
}
