using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Ships
{
    public class Battleships
    {
        private const string file = "battleships.txt";
        private readonly Random random;

        public Point? LastMove { get; private set; }
        public UnitList Ships { get; private set; }
        public IList<HitInfo> LastHits { get; private set; }
        public IList<HitInfo> CurrentHits { get; private set; }
        public Board Board { get; private set; }

        public Battleships(Board board, Random random)
        {
            this.random = random;
            Board = board;
            ReadSettings();
            UpdateShips();
        }

        private void UpdateShips()
        {
            if (!LastMove.HasValue || !Board.IsDestroyed(LastMove.Value)) return;

            var lastPoints = LastHits.SelectMany(h => h.GetPoints()).Distinct().Count();
            var currentPoints = CurrentHits.SelectMany(h => h.GetPoints()).Distinct().Count();

            Ships.Remove((Unit)(lastPoints - currentPoints + 1));
        }

        private void ReadSettings()
        {
            CurrentHits = Board.Hits().Distinct().ToList();

            if (!File.Exists(file))
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
            foreach (var h in CurrentHits) f.WriteLine(h);

            f.Close();
        }
        
        private IEnumerable<Point> FindPoints(IEnumerable<HitInfo> moves)
        {
            var q = moves.SelectMany(m => m.GetPoints())
                         .Where(p => !Board.IsVisited(p))
                         .GroupBy(p => p)
                         .GroupBy(g => g.Count())
                         .OrderByDescending(g => g.Key)
                         .FirstOrDefault();

            return q == null ? Enumerable.Empty<Point>() : q.Select(g => g.Key);
        }

        private HitInfo ProbableHit(IList<HitInfo> hits)
        {
            var maxLength = hits.Max(h => h.Length);
            return hits.Where(h => h.Length == maxLength)
                       .OrderByDescending(h => h.SurroundingSpace(Board))
                       .First();
        }

        public void NextMove()
        {
            var points = CurrentHits.Any() ?
                Board.PossibleMoves(ProbableHit(CurrentHits)) : 
                FindPoints(Board.PossibleMoves(Ships.Max()));

            LastMove = points.OrderBy(p => random.Next()).First();

            WriteSettings();
        }
    }
}
