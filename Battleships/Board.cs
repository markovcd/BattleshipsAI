using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
    public class Board
    {
        private readonly char[,] board;

        public Board(char[,] b)
        {
            board = b;
        }

        public Board(int height, int width)
        {
            var b = new char[height, width];

            for (int x = 0; x < height; x++)
                for (int y = 0; y < width; y++)
                    b[x, y] = '-';

            board = b;
        }

        public int Height { get { return board.GetLength(0); } }
        public int Width { get { return board.GetLength(1); } }

        public char this[Point p]
        {
            get { return board[p.X, p.Y]; }
            set { board[p.X, p.Y] = value; }
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
}
