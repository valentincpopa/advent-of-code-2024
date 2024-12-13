using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day12
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

            _iLength = input.Count;
            _jLength = input[0].Length;

            var plants = input
                .Select((row, i) => row
                    .Select((character, j) =>
                    {
                        return BuildPlotPlant(input, character, i, j);
                    })
                    .ToList())
                .ToList();

            var firstPartCost = 0;
            var secondPartCost = 0;

            var visitedByAnySearch = new HashSet<(int i, int j)>();

            for (int i = 0; i < _iLength; i++)
            {
                for (int j = 0; j < _jLength; j++)
                {
                    if (!visitedByAnySearch.Add((i, j)))
                    {
                        continue;
                    }

                    var queue = new Queue<(int i, int j)>();
                    queue.Enqueue((i, j));

                    var visitedInPlot = new HashSet<Plant>
                    {
                        plants[i][j]
                    };

                    var corners = 0;

                    while(queue.Count > 0)
                    {
                        var currentPlantCoordinates = queue.Dequeue();
                        var currentPlant = plants[currentPlantCoordinates.i][currentPlantCoordinates.j];

                        for (int k = 0; k < _directions.Count; k++)
                        {
                            var direction = _directions[k];
                            var newI = currentPlantCoordinates.i + direction.i;
                            var newJ = currentPlantCoordinates.j + direction.j;

                            if (!ValuesInBounds(newI, newJ) || input[newI][newJ] != currentPlant.Type)
                            {
                                var newDirection = _directions[(k + 1) % 4];
                                var nextNewI = currentPlantCoordinates.i + newDirection.i;
                                var nextNewJ = currentPlantCoordinates.j + newDirection.j;

                                if (!ValuesInBounds(nextNewI, nextNewJ) || input[nextNewI][nextNewJ] != currentPlant.Type)
                                {
                                    corners++;
                                } 
                                else
                                {
                                    nextNewI = newI + newDirection.i;
                                    nextNewJ = newJ + newDirection.j;

                                    if (ValuesInBounds(nextNewI, nextNewJ) && input[nextNewI][nextNewJ] == currentPlant.Type)
                                    {
                                        corners++;
                                    }
                                }
                            }

                            if (!IsSamePlot(input, currentPlant.Type, currentPlantCoordinates.i, currentPlantCoordinates.j, direction) || !visitedInPlot.Add(plants[newI][newJ]))
                            {
                                continue;
                            }

                            visitedByAnySearch.Add((newI, newJ));

                            queue.Enqueue((newI, newJ));
                        }
                    }

                    firstPartCost += visitedInPlot.Count * visitedInPlot.Aggregate(0, (acc, curr) => acc + 4 - curr.NumberOfNeighbours);
                    secondPartCost += visitedInPlot.Count * corners;
                }
            }

            Console.WriteLine(firstPartCost);
            Console.WriteLine(secondPartCost);
        }

        private static Plant BuildPlotPlant(List<string> input, char character, int i, int j)
        {
            var samePlotNeighboursCount = _directions
                .Where(direction => IsSamePlot(input, character, i, j, direction))
                .Count();

            return new Plant(character, samePlotNeighboursCount);
        }

        private static bool IsSamePlot(List<string> input, char character, int i, int j, (int i, int j) direction)
        {
            var newI = i + direction.i;
            var newJ = j + direction.j;

            return ValuesInBounds(newI, newJ) && input[newI][newJ] == character;
        }

        private static bool ValuesInBounds(int newI, int newJ)
        {
            return newI >= 0 && newI < _iLength
                && newJ >= 0 && newJ < _jLength;
        }
    }

    internal class Plant
    {
        public Plant(char type, int numberOfNeighbours)
        {
            Type = type;
            NumberOfNeighbours = numberOfNeighbours;
        }

        public char Type { get; set; }

        public int NumberOfNeighbours { get; set; }
    }
}
