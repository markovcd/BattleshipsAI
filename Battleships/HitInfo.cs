using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
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

        public Tuple<int, Orientation> SurroundingSpace(Board b)
        {
            bool vertical = Orientation == Orientation.Vertical;
            
            var begin = Location;
            var end = new Point
            {
                X = begin.X + (vertical ? Length - 1 : 0),
                Y = begin.Y + (vertical ? 0 : Length - 1)
            };

            int verticalSpace = 0;
            int horizontalSpace = 0;

            int step = 0;
            while (step++ < 2)
            {
                vertical = !vertical;

                if (vertical && Orientation == Orientation.Horizontal) continue;
                if (!vertical && Orientation == Orientation.Vertical) continue;

                int size = vertical ? b.Height : b.Width;
                int beginPos = vertical ? begin.X : begin.Y;
                int endPos = vertical ? end.X : end.Y;
      
                for (int i = beginPos - 1; i >= 0; i--)
                {
                    var p = new Point
                    {
                        X = vertical ? i : begin.X,
                        Y = vertical ? begin.Y : i
                    };

                    if (b.IsVisited(p)) break;

                    if (vertical) verticalSpace++;
                    else horizontalSpace++;
                }

                for (int i = endPos + 1; i < size; i++)
                {
                    var p = new Point
                    {
                        X = vertical ? i : end.X,
                        Y = vertical ? end.Y : i
                    };

                    if (b.IsVisited(p)) break;

                    if (vertical) verticalSpace++;
                    else horizontalSpace++;
                }
            }

            if (verticalSpace > horizontalSpace)
                return new Tuple<int, Orientation>(verticalSpace, Orientation.Vertical);
            if (verticalSpace < horizontalSpace)
                return new Tuple<int, Orientation>(horizontalSpace, Orientation.Horizontal);
            
            return new Tuple<int, Orientation>(horizontalSpace, Orientation.None);

        }
    }
}
