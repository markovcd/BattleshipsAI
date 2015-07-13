using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
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
}
