using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var reports = input
                .Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                              .Select(x => int.Parse(x))
                              .ToList())
                .ToList();

            SolveFirstPart(reports);
            SolveSecondPart(reports);
        }

        private static void SolveFirstPart(List<List<int>> reports)
        {
            var noOfSafeReports = 0;

            foreach (var levels in reports)
            {
                var (_, sign) = GetReportInfo(levels[0], levels[1]);

                if (IsReportSafe(levels, sign))
                {
                    noOfSafeReports++;
                }
            }

            Console.WriteLine(noOfSafeReports);
        }

        private static bool IsReportSafe(List<int> levels, int sign)
        {
            for (int i = 1; i < levels.Count; i++)
            {
                if (!ValidTransition(levels[i - 1], levels[i], sign))
                {
                    return false;
                }
            }

            return true;
        }

        private static void SolveSecondPart(List<List<int>> reports)
        {
            var noOfSafeReports = 0;

            foreach (var levels in reports)
            {
                var (_, sign) = GetReportInfo(levels[0], levels[1]);
                var isSafe = true;

                if (!IsReportSafe(levels, sign))
                {
                    isSafe = IsReportSafeWithTolerance(levels);
                }

                if (isSafe)
                {
                    noOfSafeReports++;
                }
            }

            Console.WriteLine(noOfSafeReports);
        }

        private static bool IsReportSafeWithTolerance(List<int> levels)
        {
            for (int i = 0; i < levels.Count; i++)
            {
                var removed = levels[i];
                levels.RemoveAt(i);

                var (_, sign) = GetReportInfo(levels[0], levels[1]);
                var isSafe = IsReportSafe(levels, sign);

                if (isSafe)
                {
                    return true;
                }

                levels.Insert(i, removed);
            }

            return false;
        }

        static bool ValidTransition(int firstLevel, int secondLevel, int reportSign)
        {
            var (difference, localSign) = GetReportInfo(firstLevel, secondLevel);
            var absoluteValue = Math.Abs(difference);

            if (reportSign != localSign || absoluteValue < 1 || absoluteValue > 3)
            {
                return false;
            }

            return true;
        }

        static (int difference, int sign) GetReportInfo(int firstLevel, int secondLevel)
        {
            var difference = firstLevel - secondLevel;
            var sign = Math.Sign(difference);

            return (difference, sign);
        }
    }
}
