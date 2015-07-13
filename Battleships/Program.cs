using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Ships
{
    public enum Orientation { None, Horizontal, Vertical }

    public enum Unit { None, Submarine, Destroyer, Cruiser, Battleship, Carrier }

    public class UnitList : List<Unit>
    {
        public static IEnumerable<Unit> GenerateShips()
        {
            for (int i = 1; i <= 5; i++)
                for (int j = 0; j < (i > 2 ? 1 : 2); j++)
                    yield return (Unit)i;
        }

        public IEnumerable<Unit> Descending()
        {
            return this.OrderByDescending(u => (int)u);
        }

        public UnitList()
            : base(GenerateShips())
        { }

        public UnitList(IEnumerable<Unit> units)
            : base(units)
        { }
    }

    public struct Point
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1}", X, Y);
        }

        public static Point? Parse(string s)
        {
            if (String.IsNullOrWhiteSpace(s)) return null;
            var v = s.Split().Select(int.Parse).ToArray();
            if (v.Length < 2) return null;
            return new Point { X = v[0], Y = v[1] };
        }

        public bool IsInHitInfo(HitInfo h)
        {
            var p = this;
            return h.GetPoints().Any(i => p.X == i.X && p.Y == i.Y);
        }

        public bool IsValid(Board b)
        {
            return X >= 0 && X < b.Height && Y >= 0 && Y < b.Width;
        }

        public IEnumerable<Point> NeighbourPoints(Board b)
        {
            var points = new[] 
            {
                new Point { X = X - 1, Y = Y },
                new Point { X = X, Y = Y + 1 },
                new Point { X = X + 1, Y = Y },
                new Point { X = X, Y = Y - 1 }
            };

            return points.Where(p => p.IsValid(b));
        }
    }

    public struct HitInfo
    {
        public Point Location { get; set; }
        public Orientation Orientation { get; set; }
        public int Length { get; set; }

        public override string ToString()
        {
            return String.Format("{0} {1} {2}", Location, (int)Orientation, Length);
        }

        public static HitInfo? Parse(string s)
        {
            if (String.IsNullOrWhiteSpace(s)) return null;
            var v = s.Split().Select(int.Parse).ToArray();
            if (v.Length < 4) return null;
            var p = new Point { X = v[0], Y = v[1] };
            return new HitInfo { Location = p, Orientation = (Orientation)v[2], Length = v[3] };
        }

        public IEnumerable<Point> GetPoints()
        {
            bool vertical = Orientation == Orientation.Vertical;

            int pos1 = vertical ? Location.X : Location.Y;
            int pos2 = vertical ? Location.Y : Location.X;

            for (int i = pos1; i < pos1 + Length; i++)
                yield return new Point { X = vertical ? i : pos2, Y = vertical ? pos2 : i };
        }
    }

    public class Board
    {
        private readonly char[,] board;

        public Board(char[,] b)
        {
            board = b;
        }

        public int Height { get { return board.GetLength(0); } }
        public int Width { get { return board.GetLength(1); } }

        public char this[Point p]
        {
            get { return board[p.X, p.Y]; }
            set { board[p.X, p.Y] = value; }
        }

        public Board(int height, int width)
        {
            var b = new char[height, width];

            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++)
                    b[x, y] = '-';

            board = b;
        }

        public bool IsVisited(Point p)
        {
            return this[p] != '-';
        }

        public bool IsPossibleShip(Point p)
        {
            return this[p] == '-' || this[p] == 'h';
        }

        public bool IsHit(Point p)
        {
            return this[p] == 'h';
        }

        public bool IsDestroyed(Point p)
        {
            return this[p] == 'd';
        }

        public void SetHit(Point p)
        {
            this[p] = 'h';
        }

        public void SetDestroyed(Point p)
        {
            this[p] = 'd';
        }

        public void SetMiss(Point p)
        {
            this[p] = 'm';
        }

        public Board Duplicate()
        {
            var b = new char[Height, Width];
            Array.Copy(board, b, Height * Width);
            return new Board(b);
        }

        public IEnumerable<HitInfo> PossibleMoves(Unit ship, HitInfo hit)
        {
            bool vertical = false;
            int step = 0;
            var u = (int)ship;

            while (step++ < 2)
            {
                vertical = !vertical;
                if (vertical && hit.Orientation == Orientation.Horizontal) continue;
                if (!vertical && hit.Orientation == Orientation.Vertical) continue;
                if (hit.Length >= u) continue;

                int size = vertical ? Height : Width;

                int pos1 = vertical ? hit.Location.X : hit.Location.Y;
                int pos2 = vertical ? hit.Location.Y : hit.Location.X;

                for (int i = Math.Max(pos1 - u + hit.Length, 0); i <= Math.Min(size - u, pos1); i++)
                {
                    var currentPoint = new Point { X = vertical ? i : pos2, Y = vertical ? pos2 : i };
                    var currentOrientation = vertical ? Orientation.Vertical : Orientation.Horizontal;
                    var currentHit = new HitInfo { Location = currentPoint, Length = u, Orientation = currentOrientation };

                    if (currentHit.GetPoints().All(IsPossibleShip))
                        yield return currentHit;
                }
            }
        }

        public IEnumerable<HitInfo> PossibleMoves(Unit ship)
        {
            bool vertical = false;
            int step = 0;
            var u = (int)ship;

            while (step++ < 2)
            {
                vertical = !vertical;
                if (vertical && u == 1) continue;

                int size1 = vertical ? Width : Height;
                int size2 = vertical ? Height : Width;

                for (int i = 0; i < size1; i++)
                    for (int j = 0; j < size2 - u + 1; j++)
                    {
                        var currentPoint = new Point { X = vertical ? j : i, Y = vertical ? i : j };
                        var currentOrientation = vertical ? Orientation.Vertical : Orientation.Horizontal;
                        var currentHit = new HitInfo { Location = currentPoint, Length = u, Orientation = currentOrientation };

                        if (currentHit.GetPoints().All(p => !IsVisited(p)))
                            yield return currentHit;
                    }
            }
        }
    }

    public class Game
    {
        public Board Board { get; private set; }
        public IList<HitInfo> Ships { get; private set; }
        public Board G { get; private set; }

        private readonly Random random;

        public IEnumerable<HitInfo> Generate()
        {
            var s = new UnitList();
            var b = Board.Duplicate();

            foreach (var ship in s.OrderBy(item => random.Next()))
            {
                var move = b.PossibleMoves(ship)
                            .OrderBy(item => random.Next())
                            .First();

                foreach (var p in move.GetPoints())
                {
                    if (b.IsVisited(p)) throw new Exception("overlaping");
                    b[p] = (char)(ship + '0');
                }

                yield return move;
            }

            G = b;
        }

        public Game(int height, int width)
        {
            random = new Random();
            Board = new Board(height, width);
            Ships = Generate().ToList();
        }

        public bool Move(Point p)
        {
            if (!p.IsValid(Board) || Board.IsVisited(p))
                throw new Exception("Invalid move");

            var s = Ships.Where(p.IsInHitInfo).ToList();

            if (!s.Any())
            {
                Board.SetMiss(p);
                return false;
            }

            Board.SetHit(p);

            var points = s.Single().GetPoints().ToList();

            if (points.All(Board.IsHit))
            {
                points.ForEach(Board.SetDestroyed);
                Ships.Remove(s.Single());
            }

            return !Ships.Any();
        }
    }

    public class Battleships
    {
        public UnitList Ships { get; private set; }

        private const string file = "battleships.txt";

        public Point? LastMove { get; set; }

        public HitInfo? LastHit { get; set; }
        public HitInfo? CurrentHit { get; set; }

        public Board Board { get; private set; }

        private void UpdateShips()
        {
            if (!LastMove.HasValue) return;
            if (!Board.IsDestroyed(LastMove.Value)) return;

            int u = 1;

            if (LastHit.HasValue)
                u += LastHit.Value.Length;

            if (CurrentHit.HasValue)
                u -= CurrentHit.Value.Length;

            Ships.Remove((Unit)u);
        }

        private void ReadSettings()
        {
            if (String.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Ships = new UnitList();
                return;
            }

            var f = new StreamReader(file);

            LastMove = Point.Parse(f.ReadLine());
            LastHit = HitInfo.Parse(f.ReadLine());

            Ships = new UnitList(f.ReadLine().Split().Select(i => (Unit)int.Parse(i)));

            f.Close();
        }

        private void WriteSettings()
        {
            var f = new StreamWriter(file);

            f.WriteLine(LastMove.Value);

            if (CurrentHit.HasValue)
                f.WriteLine(CurrentHit.Value);
            else
                f.WriteLine();

            f.WriteLine(Ships.Select(u => ((int)u).ToString()).Aggregate((a, b) => a + " " + b));

            f.Close();
        }

        public Battleships(Board board)
        {
            Board = board;

            ReadSettings();
            CurrentHit = FindHit();

            UpdateShips();
        }

        public HitInfo? FindHit()
        {
            var p1 = new Point();
            var p2 = new Point();

            bool b = true;

            for (int x = 0; x < Board.Height && b; x++)
                for (int y = 0; y < Board.Width && b; y++)
                {
                    p1 = new Point { X = x, Y = y };
                    if (Board.IsHit(p1)) b = false;
                }

            if (b) return null;

            int u = (int)Ships.Descending().First();

            b = true;
            for (int x = Math.Min(Board.Height - 1, p1.X + u - 1); x >= p1.X && b; x--)
                for (int y = Math.Min(Board.Width - 1, p1.Y + u - 1); y >= p1.Y && b; y--)
                {
                    p2 = new Point { X = x, Y = y };
                    if (Board.IsHit(p2)) b = false;
                }

            int x2 = p2.X - p1.X;
            int y2 = p2.Y - p1.Y;

            if (x2 != 0 && y2 != 0)
                throw new Exception("Diagonal ship!");
            if (x2 != 0)
                return new HitInfo { Location = p1, Orientation = Orientation.Vertical, Length = x2 + 1 };
            if (y2 != 0)
                return new HitInfo { Location = p1, Orientation = Orientation.Horizontal, Length = y2 + 1 };

            return new HitInfo { Location = p1, Orientation = Orientation.None, Length = 1 };
        }

        private IEnumerable<Point> FindPoints(IEnumerable<HitInfo> moves, Func<Point, bool> predicate = null)
        {
            var q = moves.SelectMany(m => m.GetPoints())
                         .Where(p => !Board.IsVisited(p))
                         .Where(predicate ?? (p => true))
                         .GroupBy(p => p)
                         .GroupBy(g => g.Count())
                         .OrderByDescending(g => g.Key)
                         .FirstOrDefault();

            return q == null ? Enumerable.Empty<Point>() : q.Select(g => g.Key);
        }

        public void NextMove()
        {
            foreach (var ship in Ships.Descending())
            {
                IEnumerable<Point> points;

                if (CurrentHit.HasValue)
                {
                    var moves = Board.PossibleMoves(ship, CurrentHit.Value);
                    points = FindPoints(moves, p => p.NeighbourPoints(Board).Any(Board.IsHit)).ToList();
                    if (!points.Any()) continue;
                }
                else
                    points = FindPoints(Board.PossibleMoves(ship)).ToList();

                var r = new Random();
                LastMove = points.OrderBy(p => r.Next()).First();

                break;
            }

            WriteSettings();
        }
    }

    static class Solution
    {
        static void PrintBoard(Board b)
        {
            for (int x = 0; x < b.Height; x++)
            {
                for (int y = 0; y < b.Width; y++)
                {
                    Console.Write(b[new Point { X = x, Y = y }]);
                }
                Console.WriteLine();
            }
        }

        public static char[,] ReadBoard(int height, int width)
        {
            var b = new char[height, width];
            for (int x = 0; x < height; x++)
            {
                string s = Console.ReadLine();
                for (int y = 0; y < width; y++)
                    b[x, y] = s[y];
            }

            return b;
        }

        static void Main()
        {
            var g = new Game(10, 10);

            Point p;
            if (File.Exists("battleships.txt")) File.Delete("battleships.txt");

            do
            {
                Thread.Sleep(1000);
                Console.Clear();
                PrintBoard(g.G);
                Console.WriteLine();
                PrintBoard(g.Board);
                var b = new Battleships(g.Board);
                b.NextMove();
                p = b.LastMove.Value;
            } while (!g.Move(p));


        }

    }

}
