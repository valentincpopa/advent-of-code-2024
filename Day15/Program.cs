using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Day15
{
    internal class Program
    {
        private static readonly Dictionary<char, (int row, int column)> _directions = new Dictionary<char, (int i, int j)>
        {
            { '^', (-1, 0) },  // up
            { '>', (0, 1) },   // right
            { 'v', (1, 0) },   // down
            { '<', (0, -1) }   // left
        };

        static void Main(string[] args)
        {
            var allLines = File.ReadAllLines("./input.txt").ToList();

            var map = allLines
                .TakeWhile(x => !x.Equals(string.Empty))
                .Select(x => x.ToCharArray().ToList())
                .ToList();

            var expandedMap = CreateExpandedMap(map);

            var updatesString = string.Join(string.Empty, allLines.Skip(map.Count + 1));
            var updates = GetStackedUpdates(updatesString);

            var startingPoint = map
                .Select((line, rowIndex) => (row: rowIndex, column: line.IndexOf('@')))
                .FirstOrDefault(result => result.column != -1);

            SolveFirstPart(map, updates, startingPoint);
            SolveSecondPart(expandedMap, updatesString, startingPoint);
        }

        private static List<List<WarehouseItem>> CreateExpandedMap(List<List<char>> map)
        {
            var expandedMap = new List<List<WarehouseItem>>();

            for (int y = 0; y < map.Count; y++)
            {
                var expandedLine = new List<WarehouseItem>();
                expandedMap.Add(expandedLine);

                for (int x = 0; x < map[0].Count; x++)
                {
                    CreateWarehouseItem(map, y, expandedLine, x);
                }
            }

            UpdateCoordinates(expandedMap);

            return expandedMap;
        }

        private static void CreateWarehouseItem(List<List<char>> map, int y, List<WarehouseItem> expandedLine, int x)
        {
            if (map[y][x] == '.')
            {
                expandedLine.Add(null);
                expandedLine.Add(null);

                return;
            }

            if (map[y][x] == '@')
            {
                expandedLine.Add(new WarehouseItem { Value = '@' });
                expandedLine.Add(null);

                return;
            }

            if (map[y][x] == '#')
            {
                expandedLine.Add(new WarehouseItem { Value = '#' });
                expandedLine.Add(new WarehouseItem { Value = '#' });

                return;
            }

            if (map[y][x] == 'O')
            {
                var warehouseItem1 = new WarehouseItem { Value = '[' };
                var warehouseItem2 = new WarehouseItem { Value = ']', Pair = warehouseItem1 };
                warehouseItem1.Pair = warehouseItem2;

                expandedLine.Add(warehouseItem1);
                expandedLine.Add(warehouseItem2);

                return;
            }
        }

        private static void UpdateCoordinates(List<List<WarehouseItem>> expandedMap)
        {
            for (int y = 0; y < expandedMap.Count; y++)
            {
                for (int x = 0; x < expandedMap[0].Count; x++)
                {
                    var item = expandedMap[y][x];
                    if (item != null)
                    {
                        item.CurrentY = y;
                        item.CurrentX = x;
                    }
                }
            }
        }

        private static void SolveSecondPart(List<List<WarehouseItem>> map, string updatesString, (int row, int column) startingPoint)
        {
            var robot = map[startingPoint.row][startingPoint.column * 2];

            foreach (var updateCharacter in updatesString)
            {
                var direction = _directions[updateCharacter];

                var visited = new HashSet<WarehouseItem>();
                var stack = new Stack<WarehouseItem>();

                visited.Add(robot);
                stack.Push(robot);

                var blockAction = WarehouseItem.ActionType.Move;

                while (stack.Count > 0)
                {
                    var itemToMove = stack.Pop();
                    var action = itemToMove.GetPairAction(map, direction);
                    if (action == WarehouseItem.ActionType.Wait)
                    {
                        foreach (var neighbour in itemToMove.GetNeighbours(map).Where(visited.Add))
                        {
                            stack.Push(neighbour);
                        }
                    }
                    else if (action == WarehouseItem.ActionType.None)
                    {
                        blockAction = WarehouseItem.ActionType.None;
                    }
                }

                if (blockAction != WarehouseItem.ActionType.None)
                {
                    foreach (var item in GetOrderedSet(updateCharacter, GetDistinctPairSet(visited)))
                    {
                        item.ApplyMovement(map);
                    }
                }
            }

            Console.WriteLine(ComputeGpsCoordinatesSum(map));
        }

        private static IEnumerable<WarehouseItem> GetOrderedSet(char updateCharacter, HashSet<WarehouseItem> distinctPairSet)
        {
            if (updateCharacter == '>')
            {
                return distinctPairSet.OrderBy(x => x.CurrentY).ThenByDescending(x => x.CurrentX);
            }
            else if (updateCharacter == '<')
            {
                return distinctPairSet.OrderBy(x => x.CurrentY).ThenBy(x => x.CurrentX);
            }
            else if (updateCharacter == 'v')
            {
                return distinctPairSet.OrderByDescending(x => x.CurrentY).ThenBy(x => x.CurrentX);
            }
            else
            {
                return distinctPairSet.OrderBy(x => x.CurrentY).ThenBy(x => x.CurrentX);
            }
        }

        private static HashSet<WarehouseItem> GetDistinctPairSet(HashSet<WarehouseItem> visited)
        {
            var distinctPairSet = new HashSet<WarehouseItem>();

            foreach (var neighbour in visited)
            {
                if (!distinctPairSet.Contains(neighbour.Pair))
                {
                    distinctPairSet.Add(neighbour);
                }
            }

            return distinctPairSet;
        }

        private static void SolveFirstPart(List<List<char>> map, List<(char character, int count)> updates, (int row, int column) startingPoint)
        {
            foreach (var update in updates)
            {
                var offset = _directions[update.character];

                var newY = startingPoint.row + offset.row;
                var newX = startingPoint.column + offset.column;

                var stack = new Stack<char>();
                stack.Push(map[startingPoint.row][startingPoint.column]);

                var noOfMoves = ComputeNoOfMoves(map, update, offset, ref newY, ref newX, stack);

                var originalStartingPoint = startingPoint;
                startingPoint = (row: startingPoint.row + noOfMoves * offset.row, column: startingPoint.column + noOfMoves * offset.column);

                newY -= offset.row;
                newX -= offset.column;

                ProcessUpdates(map, offset, newY, newX, stack, noOfMoves, originalStartingPoint);
            }

            Console.WriteLine(ComputeGpsCoordinatesSum(map));
        }

        private static void ProcessUpdates(List<List<char>> map, (int row, int column) offset, int newY, int newX, Stack<char> stack, int noOfMoves, (int row, int column) originalStartingPoint)
        {
            while (newY != originalStartingPoint.row || newX != originalStartingPoint.column)
            {
                if (stack.Count > 0)
                {
                    map[newY][newX] = stack.Pop();
                }
                else
                {
                    map[newY][newX] = '.';
                }

                newY -= offset.row;
                newX -= offset.column;
            }

            if (noOfMoves != 0)
            {
                map[newY][newX] = '.';
            }
        }

        private static int ComputeNoOfMoves(List<List<char>> map, (char character, int count) update, (int row, int column) offset, ref int newY, ref int newX, Stack<char> stack)
        {
            var noOfMoves = 0;

            while (map[newY][newX] != '#' && noOfMoves != update.count)
            {
                if (map[newY][newX] == '.')
                {
                    noOfMoves++;
                }
                else if (map[newY][newX] == 'O')
                {
                    stack.Push('O');
                }

                newY += offset.row;
                newX += offset.column;
            }

            return noOfMoves;
        }

        private static int ComputeGpsCoordinatesSum(List<List<char>> map)
        {
            var coordinatesSum = 0;

            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[0].Count; j++)
                {
                    if (map[i][j] == 'O')
                    {
                        coordinatesSum += 100 * i + j;
                    }
                }
            }

            return coordinatesSum;
        }

        private static int ComputeGpsCoordinatesSum(List<List<WarehouseItem>> map)
        {
            var coordinatesSum = 0;

            for (int y = 0; y < map.Count; y++)
            {
                for (int x = 0; x < map[y].Count; x++)
                {
                    var character = map[y][x]?.Value ?? '.';

                    if (character == '[')
                    {
                        coordinatesSum += 100 * y + x;
                    }
                }
            }

            return coordinatesSum;
        }

        private static List<(char character, int count)> GetStackedUpdates(string updatesString)
        {
            var updates = new List<(char character, int count)>();
            var previous = (character: updatesString[0], count: 1);

            for (int i = 1; i < updatesString.Length; i++)
            {
                if (updatesString[i] == previous.character)
                {
                    previous.count++;
                }
                else
                {
                    updates.Add(previous);
                    previous = (character: updatesString[i], count: 1);
                }
            }

            updates.Add(previous);
            return updates;
        }
    }

    internal class WarehouseItem
    {
        public int CurrentX { get; set; }

        public int CurrentY { get; set; }

        public (int y, int x) Direction { get; set; }

        public char Value { get; set; }

        public WarehouseItem Pair { get; set; }

        public ActionType GetAction(List<List<WarehouseItem>> map, (int y, int x) direction)
        {
            Direction = direction;

            var neighbour = map[CurrentY + direction.y][CurrentX + direction.x];

            if (neighbour?.Pair == this)
            {
                neighbour = map[neighbour.CurrentY + direction.y][neighbour.CurrentX + direction.x];
            }

            if (neighbour == null)
            {
                return ActionType.Move;
            }

            if (neighbour != null && neighbour.Value == ']' || neighbour.Value == '[')
            {
                return ActionType.Wait;
            }

            Direction = (0, 0);
            return ActionType.None;
        }

        public ActionType GetPairAction(List<List<WarehouseItem>> map, (int y, int x) direction)
        {
            var currentAction = GetAction(map, direction);
            var pairAction = ActionType.Move;

            if (Pair != null)
            {
                pairAction = Pair.GetAction(map, direction);
            }

            if (currentAction == ActionType.None || pairAction == ActionType.None)
            {
                return ActionType.None;
            }

            if (currentAction == ActionType.Move && pairAction == ActionType.Move)
            {
                return ActionType.Move;
            }

            return ActionType.Wait;

        }

        public void ApplyMovement(List<List<WarehouseItem>> map)
        {
            if (Direction.x == 0 && Direction.y == 0)
            {
                return;
            }

            Pair?.Move(map);
            this.Move(map);
        }

        public List<WarehouseItem> GetNeighbours(List<List<WarehouseItem>> map)
        {
            var neighbours = new List<WarehouseItem>();

            var currentNeighbour = map[CurrentY + Direction.y][CurrentX + Direction.x];
            if (currentNeighbour != null && currentNeighbour.Pair != this)
            {
                neighbours.Add(map[CurrentY + Direction.y][CurrentX + Direction.x]);
            }

            if (Pair != null)
            {
                var pairNeighbour = map[Pair.CurrentY + Pair.Direction.y][Pair.CurrentX + Pair.Direction.x];
                if (pairNeighbour != null && !neighbours.Contains(pairNeighbour.Pair))
                {
                    neighbours.Add(pairNeighbour);
                }
            }

            return neighbours;
        }

        public void Move(List<List<WarehouseItem>> map)
        {
            var xToReset = CurrentX;
            var yToReset = CurrentY;

            CurrentX += Direction.x;
            CurrentY += Direction.y;

            map[CurrentY][CurrentX] = this;
            map[yToReset][xToReset] = null;
        }

        internal enum ActionType
        {
            None,
            Move,
            Wait
        }
    }
}