using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var firstList = new List<int>();
            var secondList = new List<int>();

            foreach (var item in input)
            {
                var values = item.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                firstList.Add(int.Parse(values[0]));
                secondList.Add(int.Parse(values[1]));
            }

            SolveFirstPart(firstList, secondList);
            SolveSecondPart(firstList, secondList);
        }

        private static void SolveFirstPart(List<int> firstList, List<int> secondList)
        {
            firstList.Sort();
            secondList.Sort();

            var totalDistance = 0;

            for (var i = 0; i < firstList.Count; i++)
            {
                totalDistance += Math.Abs(firstList[i] - secondList[i]);
            }

            Console.WriteLine(totalDistance);
        }

        private static void SolveSecondPart(List<int> firstList, List<int> secondList)
        {
            var frequencies = new Dictionary<int, int>();
            foreach (var item in secondList)
            {
                if (frequencies.TryGetValue(item, out int value))
                {
                    frequencies[item] = ++value;
                }
                else
                {
                    frequencies.Add(item, 1);
                }
            }

            var similarityScore = 0;

            foreach (var item in firstList)
            {
                frequencies.TryGetValue(item, out int value);
                similarityScore += item * value;
            }

            Console.WriteLine(similarityScore);
        }
    }
}
