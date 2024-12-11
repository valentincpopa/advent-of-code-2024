using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var stoneMap = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .First()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse)
                .GroupBy(x => x)
                .ToDictionary(x => x.Key, x => new Pair(x.Count(), 0L));

            for (int i = 0; i < 75; i++)
            {
                foreach (var stonePair in stoneMap.ToList())
                {
                    ProcessStonePair(stoneMap, stonePair, i);
                    stoneMap = stoneMap.Where(x => x.Value.IsValid).ToDictionary(x => x.Key, x => x.Value);
                }
            }

            var result = stoneMap.Values.Aggregate(0L, (x, y) => x + y.Second);
            Console.WriteLine(result);
        }

        private static void ProcessStonePair(Dictionary<long, Pair> stoneMap, KeyValuePair<long, Pair> stonePair, int blinkIndex)
        {
            _ = stonePair.Key switch
            {
                var key when key == 0 => ProcessReplacement(stoneMap, stonePair.Key, 1, blinkIndex),
                var key when key > 9 && key.ToString().Length % 2 == 0 => ProcessSplit(stoneMap, stonePair.Key, blinkIndex),
                _ => ProcessReplacement(stoneMap, stonePair.Key, stonePair.Key * 2024, blinkIndex)
            };
        }

        private static object ProcessSplit(Dictionary<long, Pair> stoneMap, long oldStoneKey, int blinkIndex)
        {
            bool stonesToProcessInFirstItem = StonesToProcessInFirstItem(blinkIndex);
            var oldStoneKeyString = oldStoneKey.ToString();

            var splitIndex = oldStoneKeyString.Length / 2;
            var firstStoneKey = long.Parse(oldStoneKeyString[..splitIndex]);
            var secondStoneKey = long.Parse(oldStoneKeyString[splitIndex..]);

            var oldStonePair = stoneMap[oldStoneKey];
            var noOfStones = ExtractNoOfStones(oldStonePair, stonesToProcessInFirstItem);

            SetNewStonePair(stoneMap, firstStoneKey, !stonesToProcessInFirstItem, noOfStones);
            SetNewStonePair(stoneMap, secondStoneKey, !stonesToProcessInFirstItem, noOfStones);
            SetNoOfStones(oldStonePair, 0, stonesToProcessInFirstItem);

            return null;
        }

        private static object ProcessReplacement(Dictionary<long, Pair> stoneMap, long oldStoneKey, long newStoneKey, int blinkIndex)
        {
            var stonePair = stoneMap[oldStoneKey];
            bool stonesToProcessInFirstItem = StonesToProcessInFirstItem(blinkIndex);

            SetNewStonePair(stoneMap, newStoneKey, !stonesToProcessInFirstItem, ExtractNoOfStones(stonePair, stonesToProcessInFirstItem));
            SetNoOfStones(stonePair, 0, stonesToProcessInFirstItem);

            return null;
        }

        private static bool StonesToProcessInFirstItem(int blinkIndex)
        {
            return blinkIndex % 2 == 0;
        }

        private static void SetNewStonePair(Dictionary<long, Pair> stoneMap, long newStoneKey, bool stonesToProcessInFirstItem, long noOfStones)
        {
            var newStoneKeyExists = stoneMap.TryGetValue(newStoneKey, out var stonePair);

            if (newStoneKeyExists)
            {
                SetNoOfStones(stonePair, noOfStones, stonesToProcessInFirstItem);
            }
            else
            {
                stoneMap.Add(newStoneKey, stonesToProcessInFirstItem ? new Pair(noOfStones, 0) : new Pair(0, noOfStones));
            }
        }

        private static void SetNoOfStones(Pair stonePair, long noOfStones, bool stonesToProcessInFirstItem)
        {
            if (stonesToProcessInFirstItem)
            {
                stonePair.First = noOfStones;
                return;
            }

            stonePair.Second = noOfStones;
        }

        private static long ExtractNoOfStones(Pair stonePair, bool stonesToProcessInFirstItem)
        {
            return stonesToProcessInFirstItem
                ? stonePair.First
                : stonePair.Second;
        }
    }

    internal class Pair
    {
        private long _second;
        private long _first;

        public Pair(long first, long second)
        {
            First = first;
            Second = second;
        }

        public long First
        {
            get => _first;
            set
            {
                if (value == 0)
                {
                    _first = value;
                }
                else
                {
                    _first += value;
                }
            }
        }

        public long Second
        {
            get => _second;
            set
            {
                if (value == 0)
                {
                    _second = value;
                }
                else
                {
                    _second += value;
                }
            }
        }

        public bool IsValid => First != 0 || Second != 0;

        public override string ToString()
        {
            return $"first: {First} second: {Second}";
        }
    }
}
