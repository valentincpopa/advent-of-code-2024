using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day5
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var allLines = File.ReadAllLines("./input.txt").ToList();

            var orderingRules = allLines
                .TakeWhile(x => !x.Equals(string.Empty))
                .ToArray();

            var updates = allLines
                .Skip(orderingRules.Length + 1)
                .Select(x => x.Split(',')
                    .Select(x => int.Parse(x))
                    .ToArray())
                .ToArray();

            var dependencyEntries = new Dictionary<int, List<int>>();

            foreach (var orderingRule in orderingRules)
            {
                var orderingRulePair = orderingRule
                    .Split('|')
                    .Select(int.Parse)
                    .ToArray();

                CreateOrUpdateDependencyEntry(dependencyEntries, orderingRulePair);
            }


            SolveFirstPart(dependencyEntries, updates);
            SolveSecondPart(dependencyEntries, updates);
        }

        private static void CreateOrUpdateDependencyEntry(Dictionary<int, List<int>> entries, int[] orderingRulePair)
        {
            if (entries.TryGetValue(orderingRulePair[0], out List<int> dependencies))
            {
                dependencies.Add(orderingRulePair[1]);
            }
            else
            {
                entries.Add(orderingRulePair[0], [orderingRulePair[1]]);
            }
        }

        private static void SolveFirstPart(Dictionary<int, List<int>> entries, int[][] updateRows)
        {
            var result = 0;

            foreach (var updateRow in updateRows)
            {
                if (IsValidUpdate(entries, updateRow))
                {
                    result += updateRow[updateRow.Length / 2];
                }
            }

            Console.WriteLine(result);
        }

        private static bool IsValidUpdate(Dictionary<int, List<int>> entries, int[] updateRow)
        {
            var validatedUpdateValues = new List<int>();

            foreach (var updateValue in updateRow)
            {
                var entryExists = entries.TryGetValue(updateValue, out List<int> dependencies);
                var outOfOrderDependencies = entryExists ? validatedUpdateValues.Intersect(dependencies).ToList() : [];

                if (outOfOrderDependencies.Count != 0)
                {
                    return false;
                }
                else
                {
                    validatedUpdateValues.Add(updateValue);
                }
            }

            return true;
        }

        private static void SolveSecondPart(Dictionary<int, List<int>> entries, int[][] updateRows)
        {
            var result = 0;

            foreach (var updateRow in updateRows)
            {
                if (ProcessedUpdateOrdering(entries, updateRow))
                {
                    result += updateRow[updateRow.Length / 2];
                }
            }

            Console.WriteLine(result);
        }

        private static bool ProcessedUpdateOrdering(Dictionary<int, List<int>> entries, int[] updateRow)
        {
            var correctedUpdate = false;
            var validatedUpdateValues = new List<int>();

            for (int i = 0; i < updateRow.Length; i++)
            {
                var entryExists = entries.TryGetValue(updateRow[i], out List<int> dependencies);
                var outOfOrderDependencies = entryExists ? validatedUpdateValues.Intersect(dependencies).ToList() : [];

                if (outOfOrderDependencies.Count != 0)
                {
                    correctedUpdate = true;
                    SwapValuesByIndexes(updateRow, i, validatedUpdateValues.FindIndex(x => outOfOrderDependencies.First() == x));
                    ResetValidationProcess(validatedUpdateValues, ref i);
                }
                else
                {
                    validatedUpdateValues.Add(updateRow[i]);
                }
            }

            return correctedUpdate;
        }

        private static void SwapValuesByIndexes(int[] updateRow, int i, int outOfOrderDependencyIndex)
        {
            (updateRow[i], updateRow[outOfOrderDependencyIndex]) = (updateRow[outOfOrderDependencyIndex], updateRow[i]);
        }

        private static void ResetValidationProcess(List<int> validatedUpdateValues, ref int i)
        {
            validatedUpdateValues.Clear();
            i = -1;
        }
    }
}
