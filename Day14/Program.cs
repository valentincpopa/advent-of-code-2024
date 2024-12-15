using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day14
{
    internal class Program
    {
        static readonly int _yMax = 103;
        static readonly int _xMax = 101;

        static readonly int _yToSkip = _yMax / 2;
        static readonly int _xToSkip = _xMax / 2;

        static void Main(string[] args)
        {
            var regex = new Regex("p=(?<PX>\\d+),(?<PY>\\d+)\\sv=(?<VX>-?\\d+),(?<VY>-?\\d+)");

            var robots = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(row => BuildRobot(row, regex))
                .ToList();

            for (int i = 0; i < 100; i++)
            {
                foreach (var robot in robots)
                {
                    robot.ProcessSecond(_xMax, _yMax);
                }

                //var matrix = GetMatrix();
                //PlaceRobots(robots, matrix);
                //PrintCurrentPositions(matrix, false);
            }

            var firstQuadrant = robots.Where(IsInFirstQuadrant).Count();
            var secondQuadrant = robots.Where(IsInSecondQuadrant).Count();
            var thirdQuadrant = robots.Where(IsInThirdQuadrant).Count();
            var fourthQuadrant = robots.Where(IsInFourthQuadrant).Count();

            Console.WriteLine(firstQuadrant * secondQuadrant * thirdQuadrant * fourthQuadrant);
        }

        private static bool IsInFourthQuadrant(Robot x)
        {
            return x.Position.X > _xToSkip && x.Position.Y > _yToSkip;
        }

        private static bool IsInThirdQuadrant(Robot x)
        {
            return x.Position.X < _xToSkip && x.Position.Y > _yToSkip;
        }

        private static bool IsInSecondQuadrant(Robot x)
        {
            return x.Position.X > _xToSkip && x.Position.Y < _yToSkip;
        }

        private static bool IsInFirstQuadrant(Robot x)
        {
            return x.Position.X < _xToSkip && x.Position.Y < _yToSkip;
        }

        private static void PlaceRobots(List<Robot> robots, List<List<char>> matrix)
        {
            foreach (var robot in robots)
            {
                matrix[robot.Position.Y][robot.Position.X] = 'x';
            }
        }

        private static List<List<char>> GetMatrix()
        {
            return Enumerable
                .Range(0, _yMax)
                .Select(_ => Enumerable.Repeat('.', _xMax).ToList())
                .ToList();
        }

        private static void PrintCurrentPositions(List<List<char>> matrix, bool shouldSkip)
        {
            for (int y = 0; y < matrix.Count; y++)
            {
                if (shouldSkip && y == _yToSkip)
                {
                    Console.WriteLine(Environment.NewLine);
                    continue;
                }

                for (int x = 0; x < matrix[y].Count; x++)
                {
                    if (shouldSkip && x == _xToSkip)
                    {
                        Console.Write(" ");
                        continue;
                    }

                    Console.Write(matrix[y][x]);
                }

                Console.Write(Environment.NewLine);
            }

            Console.Write(Environment.NewLine);
        }

        private static Robot BuildRobot(string row, Regex regex)
        {
            var result = regex.Match(row);

            var robot = new Robot()
            {
                Position = new Coordinates
                {
                    X = int.Parse(result.Groups["PX"].Value),
                    Y = int.Parse(result.Groups["PY"].Value),
                },
                Velocity = new Coordinates
                {
                    X = int.Parse(result.Groups["VX"].Value),
                    Y = int.Parse(result.Groups["VY"].Value),
                }
            };

            return robot;
        }
    }

    internal class Robot
    {
        public Coordinates Position { get; set; }

        public Coordinates Velocity { get; set; }

        public void ProcessSecond(int xMax, int yMax)
        {
            ProcessSecondForX(xMax);
            ProcessSecondForY(yMax);
        }

        private void ProcessSecondForY(int yMax)
        {
            var newY = (Position.Y + Velocity.Y) % yMax;

            if (newY < 0)
            {
                newY += yMax;
            }

            Position.Y = newY;
        }

        private void ProcessSecondForX(int xMax)
        {
            var newX = (Position.X + Velocity.X) % xMax;

            if (newX < 0)
            {
                newX += xMax;
            }

            Position.X = newX;
        }
    }

    internal class Coordinates
    {
        public int X { get; set; }

        public int Y { get; set; }
    }
}
