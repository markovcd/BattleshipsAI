using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Ships
{
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
}
