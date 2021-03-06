﻿using System;
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
        public IList<Unit> Ships { get; private set; }
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

        public static IEnumerable<HitInfo> Generate()
        {
            var s = new UnitList();
            var b = new Board(10, 10);
            var random = new Random();

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
        }

        private void UpdateShips()
        {
            if (!LastMove.HasValue || !Board.IsDestroyed(LastMove.Value)) return;

            var last = LastHits.SelectMany(h => h.GetPoints()).Distinct().Count();
            var current = CurrentHits.SelectMany(h => h.GetPoints()).Distinct().Count();

            Ships.Remove((Unit)(last - current + 1));
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

        public HitInfo ProbableHit()
        {
            var maxLength = CurrentHits.Max(h => h.Length);
            return CurrentHits.Where(h => h.Length == maxLength)
                       .OrderByDescending(h => h.SurroundingSpace(Board).Item1)
                       .First();
        }

        public void NextMove()
        {
            var points = CurrentHits.Any() ?
                Board.PossibleMoves(ProbableHit()) : 
                    FindPoints(Board.PossibleMoves(Ships.Max()));

            LastMove = points.OrderBy(p => random.Next()).First();

            WriteSettings();
        }
    }
}
