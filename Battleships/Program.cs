using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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

        public static HitInfo Parse(string s)
        {
            var v = s.Split().Select(int.Parse).ToArray();
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

        public bool IsValid(Board b)
        {
            if (Orientation == Orientation.None) return true;

            var before = new Point
            {
                X = Location.X - (Orientation == Orientation.Horizontal ? 0 : 1),
                Y = Location.Y - (Orientation == Orientation.Horizontal ? 1 : 0)
            };

            var after = new Point
            {
                X = Location.X + (Orientation == Orientation.Horizontal ? 0 : Length),
                Y = Location.Y + (Orientation == Orientation.Horizontal ? Length : 0)
            };

            return (before.IsValid(b) && !b.IsVisited(before)) ||
                   (after.IsValid(b) && !b.IsVisited(after));
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

        public char this[Point p] { get { return board[p.X, p.Y]; } }

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

        public IEnumerable<HitInfo> Hits()
        {  
            int step = 0;
            bool vertical = false;           

            while (step++ < 2)
            {
                vertical = !vertical;
                
                int size1 = vertical ? Width : Height;
                int size2 = vertical ? Height : Width;

                for (int i = 0; i < size1; i++)
                {
                    var p = new Point();
                    int l = 1;
                    bool found = false;

                    for (int j = 0; j < size2; j++)
                    {
                        var tmp = new Point { X = vertical ? j : i, Y = vertical ? i : j };

                        if (IsHit(tmp) && !found)
                        {
                            found = true;
                            p = tmp;
                        }
                        else if (IsHit(tmp) && found) l++;
                        else if (!IsHit(tmp) && found) break;
                    }

                    if (found) yield return new HitInfo
                    {
                        Location = p,
                        Orientation = l == 1 ? Orientation.None : vertical ? Orientation.Vertical : Orientation.Horizontal,
                        Length = l
                    };
                }
            }
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

    public class Battleships
    {
        public UnitList Ships { get; private set; }

        private const string file = "battleships.txt";

        public Point? LastMove { get; set; }

        public IList<HitInfo> LastHits { get; private set; }
        public IList<HitInfo> CurrentHits { get; private set; }

        public Board Board { get; private set; }

        private void UpdateShips()
        {
            if (!LastMove.HasValue) return;
            if (!Board.IsDestroyed(LastMove.Value)) return;

            var lastPoints = LastHits.SelectMany(h => h.GetPoints()).Distinct().Count();
            var currentPoints = CurrentHits.SelectMany(h => h.GetPoints()).Distinct().Count();

            Ships.Remove((Unit)(lastPoints - currentPoints + 1));
        }

        private void ReadSettings()
        {
            CurrentHits = Board.Hits()
                               .Where(h => h.IsValid(Board))
                               .OrderByDescending(h => h.Length)
                               .Distinct()
                               .ToList();
              
            if (String.IsNullOrEmpty(file) || !File.Exists(file))
            {
                Ships = new UnitList();
                return;
            }

            var f = new StreamReader(file);

            LastMove = Point.Parse(f.ReadLine());
            Ships = new UnitList(f.ReadLine().Split().Select(i => (Unit)int.Parse(i)));

            LastHits = Enumerable.Range(0, int.Parse(f.ReadLine()))
                                 .Select(i => HitInfo.Parse(f.ReadLine()))
                                 .ToList();

            f.Close();
        }

        private void WriteSettings()
        {
            var f = new StreamWriter(file);

            f.WriteLine(LastMove.Value);

            f.WriteLine(Ships.Select(u => ((int)u).ToString()).Aggregate((a, b) => a + " " + b));

            f.WriteLine(CurrentHits.Count());
            foreach (var h in CurrentHits)
                f.WriteLine(h);
           
            f.Close();
        }

        public Battleships(Board board)
        {
            Board = board;
            ReadSettings();
            UpdateShips();
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

                if (CurrentHits.Any())
                {
                    var moves = Board.PossibleMoves(ship, CurrentHits.First());
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
        public static Board ReadBoard(int height, int width)
        {
            var b = new char[height, width];
            for (int x = 0; x < height; x++)
            {
                string s = Console.ReadLine();
                for (int y = 0; y < width; y++)
                    b[x, y] = s[y];
            }

            return new Board(b);
        }

        static void Main()
        {
            int n = int.Parse(Console.ReadLine());
            var b = new Battleships(ReadBoard(n, n));
            b.NextMove();
            Console.WriteLine(b.LastMove);
        }

    }

}
