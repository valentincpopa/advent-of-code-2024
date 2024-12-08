using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var equations = File.ReadAllLines("./input.txt")
                .Where(row => !string.IsNullOrWhiteSpace(row))
                .Select(row =>
                {
                    var rawEquation = row.Split(':', StringSplitOptions.RemoveEmptyEntries);

                    var result = long.Parse(rawEquation[0]);
                    var operands = rawEquation[1]
                        .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => long.Parse(x))
                        .ToList();

                    return (result, operands);
                })
                .ToList();

            var totalCalibrationResult = 0L;
            
            foreach (var equation in equations)
            {
                if (IsValid(equation, equation.operands[0], 1))
                {
                    totalCalibrationResult += equation.result;
                }
            }

            Console.WriteLine(totalCalibrationResult);
        }

        private static bool IsValid((long result, List<long> operands) equation, long currentResult, int index)
        {
            if (index == equation.operands.Count)
            {
                return currentResult == equation.result;
            }

            foreach (var result in ComputeOperation(currentResult, equation.operands[index]))
            {
                if (IsValid(equation, result, ++index))
                {
                    return true;
                }

                index--;
            }

            return false;
        }

        private static IEnumerable<long> ComputeOperation(long firstOperand, long secondOperand)
        {
            // first part
            yield return firstOperand + secondOperand;
            yield return firstOperand * secondOperand;
            // second part
            yield return long.Parse(firstOperand.ToString() + secondOperand.ToString());
        }
    }
}
