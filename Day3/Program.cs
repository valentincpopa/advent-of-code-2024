using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Day3
{
    internal class Program
    {
        static readonly string _firstParameterIdentifier = "firstOperand";
        static readonly string _secondParameterIdentifier = "secondOperand";

        static readonly string _mulRegex = $"mul\\((?<{_firstParameterIdentifier}>[0-9]+),(?<{_secondParameterIdentifier}>[0-9]+)\\)";
        static readonly string _doRegex = $"do\\(\\)";
        static readonly string _dontRegex = $"don't\\(\\)";

        static readonly string _doClause = $"do()";

        static void Main(string[] args)
        {
            var input = File.ReadAllLines("./input.txt")
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .ToList();

            var mulRegex = new Regex(_mulRegex);

            var memory = string.Concat(input);
            var mulMatches = mulRegex.Matches(memory);
            var clauses = BuildOrderedClauses(memory);
            var intervals = BuildIntervals(memory, clauses);

            Solve(mulMatches, intervals);
        }

        private static void Solve(MatchCollection mulMatches, List<(int start, int end)> intervals)
        {
            var firstPartResult = 0;
            var secondPartResult = 0;

            foreach (var match in mulMatches.Cast<Match>())
            {
                var multiplication = int.Parse(match.Groups[_firstParameterIdentifier].Value) * int.Parse(match.Groups[_secondParameterIdentifier].Value);

                firstPartResult += multiplication;

                if (intervals.Any(x => x.start < match.Index && x.end > match.Index))
                {
                    secondPartResult += multiplication;
                }
            }

            Console.WriteLine(firstPartResult);
            Console.WriteLine(secondPartResult);
        }

        private static List<Match> BuildOrderedClauses(string memory)
        {
            var clauses = new List<Match>();

            var doRegex = new Regex(_doRegex);
            var doMatches = doRegex.Matches(memory);
            clauses.AddRange(doMatches);

            var dontRegex = new Regex(_dontRegex);
            var dontMatches = dontRegex.Matches(memory);
            clauses.AddRange(dontMatches);

            clauses = [.. clauses.OrderBy(x => x.Index)];

            return clauses;
        }

        private static List<(int start, int end)> BuildIntervals(string memory, List<Match> clauses)
        {
            var intervals = new List<(int start, int end)>();
            var intervalStart = 0;
            var intervalEnd = 0;

            var previousClause = _doClause;

            foreach (var clause in clauses)
            {
                bool isDoClause = clause.Value == _doClause;
                bool wasDoClause = previousClause == _doClause;

                if (isDoClause && !wasDoClause)
                {
                    intervals.Add((intervalStart, intervalEnd));
                    intervalStart = clause.Index;
                }
                else if (!isDoClause && wasDoClause)
                {
                    intervalEnd = clause.Index;
                }

                previousClause = clause.Value;
            }

            if (intervals.Last().start != intervalStart)
            {
                intervals.Add((intervalStart, memory.Length));
            }

            return intervals;
        }
    }
}
