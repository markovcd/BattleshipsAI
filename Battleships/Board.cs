using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
    public class Board
    {
        private readonly char[,] board;

        public int Height { get { return board.GetLength(0); } }
        public int Width { get { return board.GetLength(1); } }

        public char this[Point p]
        {
            get { return board[p.X, p.Y]; }
            set { board[p.X, p.Y] = value; }
        }

        public Board(char[,] b)
        {
            board = b;
        }

        public Board(IList<string> b)
        {
            board = new char[b.Count, b[0].Length];
            for (int x = 0; x < Height; x++)
                for (int y = 0; y < Width; y++)
                    board[x, y] = b[x][y];
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

                    var hit = new HitInfo
                    {
                        Location = p,
                        Orientation = l == 1 ? Orientation.None : vertical ? Orientation.Vertical : Orientation.Horizontal,
                        Length = l
                    };

                    //if (vertical && l == 1) continue;

                    if (found && hit.IsValid(this)) 
                        yield return hit;
                }
            }
        }

        public IEnumerable<Point> PossibleMoves(HitInfo hit)
        {
            if (hit.Length == 1)
            {
                foreach (var point in hit.Location.NeighbourPoints(this).Where(p => !IsVisited(p)))
                    yield return point;

                yield break;
            }

            bool vertical = false;
            int step = 0;

            var surroundingOrientation = hit.SurroundingSpace(this).Item2;

            while (step++ < 2)
            {
                vertical = !vertical;

                if ((surroundingOrientation != Orientation.None) &&
                    (vertical != (surroundingOrientation == Orientation.Vertical))) continue;

                if ((hit.Orientation == Orientation.Vertical) != vertical) continue;
                var before = new Point
                {
                    X = hit.Location.X - (vertical ? 1 : 0),
                    Y = hit.Location.Y - (vertical ? 0 : 1)
                };

                if (before.IsValid(this) && !IsVisited(before)) yield return before;

                var after = new Point
                {
                    X = hit.Location.X + (vertical ? hit.Length : 0),
                    Y = hit.Location.Y + (vertical ? 0 : hit.Length)
                };

                if (after.IsValid(this) && !IsVisited(after)) yield return after;
            }
        }

        public IEnumerable<HitInfo> PossibleMoves(Unit ship)
        {
            bool vertical = false;
            int step = 0;
            var l = (int)ship;

            while (step++ < 2)
            {
                vertical = !vertical;
                if (vertical && l == 1) continue;

                int size1 = vertical ? Width : Height;
                int size2 = vertical ? Height : Width;

                for (int i = 0; i < size1; i++)
                    for (int j = 0; j < size2 - l + 1; j++)
                    {
                        var currentPoint = new Point { X = vertical ? j : i, Y = vertical ? i : j };
                        var currentOrientation = vertical ? Orientation.Vertical : Orientation.Horizontal;
                        var currentHit = new HitInfo { Location = currentPoint, Length = l, Orientation = currentOrientation };

                        if (currentHit.GetPoints().All(p => !IsVisited(p)))
                            yield return currentHit;
                    }
            }
        }
    }
}
