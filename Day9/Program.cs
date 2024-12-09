using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day9
{
    internal class Program
    {
        private static int _currentId = 0;

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .First();

            Solve(input, true);
            Solve(input, false);
        }

        private static void Solve(string input, bool isFirstPart)
        {
            _currentId = 0;

            var memory = input.SelectMany(ProcessDigit).ToList();

            if (isFirstPart)
            {
                MoveFileBlocks(memory);
            }
            else
            {
                MoveEntireFileBlocks(memory);
            }

            Console.WriteLine(ComputeChecksum(memory));
        }

        private static long ComputeChecksum(List<Block> memory)
        {
            return memory
                .Where(x => x != null)
                .Select((block, index) => (block, index))
                .Aggregate(0L, (acc, entry) =>
                {
                    return entry.block.Id != null
                    ? acc + (entry.index * entry.block.Id.Value)
                    : acc;
                });
        }

        private static void MoveFileBlocks(List<Block> memory)
        {
            var targetIndex = memory.Count - 1;

            for (int currentIndex = 0; currentIndex < memory.Count; currentIndex++)
            {
                // skip the blocks that do not represent free space
                if (memory[currentIndex].Id.HasValue)
                {
                    continue;
                }


                // find the next file block to move
                while (targetIndex > 0 && !memory[targetIndex].Id.HasValue)
                {
                    targetIndex--;
                }

                // stop if no more valid file blocks are available
                if (targetIndex <= currentIndex)
                {
                    break;
                }

                // swap blocks
                (memory[currentIndex], memory[targetIndex]) = (memory[targetIndex], memory[currentIndex]);
            }
        }

        private static void MoveEntireFileBlocks(List<Block> memory)
        {
            var targetIndex = memory.Count - 1;

            for (int currentIndex = 0; currentIndex < memory.Count; currentIndex++)
            {
                // skip the blocks that do not represent free space
                if (memory[currentIndex].Id != null)
                {
                    continue;
                }


                // find the next file block to move
                while (targetIndex > 0 && (!memory[targetIndex].Id.HasValue || memory[targetIndex].MovedOrProcessed))
                {
                    targetIndex--;
                }

                // stop if no more valid file blocks are available
                if (targetIndex <= 0)
                {
                    break;
                }

                // reset the loop and continue to process valid file blocks
                if (targetIndex <= currentIndex)
                {
                    memory[targetIndex].MovedOrProcessed = true;
                    currentIndex = 0;
                    continue;
                }

                // check if the free space block can accommodate the entire file block
                if (memory[currentIndex].Length >= memory[targetIndex].Length)
                {
                    var fileBlock = memory[targetIndex];
                    var freeSpaceBlock = memory[currentIndex];

                    // swap blocks and adjust the length of the free space block
                    while (targetIndex >= currentIndex && memory[targetIndex] == fileBlock)
                    {
                        (memory[currentIndex], memory[targetIndex]) = (memory[targetIndex], memory[currentIndex]);
                        currentIndex++;
                        targetIndex--;
                    }

                    freeSpaceBlock.Length -= fileBlock.Length;
                    fileBlock.MovedOrProcessed = true;

                    currentIndex = 0;
                }
            }
        }

        private static IEnumerable<Block> ProcessDigit(char digitChar, int index)
        {
            var digit = digitChar - '0';
            var isFile = index % 2 == 0;

            var blocks = new List<Block>();
            var block = new Block(isFile ? _currentId++ : null, digit);

            for (var i = 0; i < digit; i++)
            {
                blocks.Add(block);
            }

            return blocks;
        }
    }

    internal class Block
    {
        public int Length { get; set; }

        public bool MovedOrProcessed { get; set; }

        public int? Id { get; }

        public Block(int? id, int length)
        {
            Id = id;
            Length = length;
        }
    }
}
