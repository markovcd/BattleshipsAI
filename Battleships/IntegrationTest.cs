using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
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
}
