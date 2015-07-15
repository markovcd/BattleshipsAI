using System;
using System.Collections.Generic;
using System.Linq;
using Ships;

namespace IntegrationTest2
{
    public class Game
    {
        public Board Board { get; private set; }
        public IList<HitInfo> Ships { get; private set; }
        public Board GeneratedBoard { get; private set; }

        public Random Random { get; private set; }

        public IEnumerable<HitInfo> Generate()
        {
            var s = new UnitList();
            var b = Board.Duplicate();

            foreach (var ship in s.OrderBy(item => Random.Next()))
            {
                var move = b.PossibleMoves(ship)
                            .OrderBy(item => Random.Next())
                            .First();

                foreach (var p in move.GetPoints())
                {
                    if (b.IsVisited(p)) throw new Exception("overlaping");
                    b[p] = (char)(ship + '0');
                }

                yield return move;
            }

            GeneratedBoard = b;
        }

        public IEnumerable<HitInfo> Generate(Board board)
        {
            GeneratedBoard = board;
            board = board.Duplicate();

            var s = new UnitList();

            foreach (var ship in s)
            {
                var l = (int)ship;
                var p = new Point();
                bool b = true;

                for (int x = 0; x < board.Height && b; x++)
                    for (int y = 0; y < board.Width; y++)
                    {
                        p = new Point { X = x, Y = y };
                        if (board[p] == l + '0')
                        {
                            b = false;
                            break;
                        }
                    }

                int dx, dy;
                for (dx = 1; dx < l; dx++)
                {
                    var p2 = new Point { X = p.X + dx, Y = p.Y };
                    if (!p2.IsValid(board) || board[p2] != l + '0') break;
                }

                for (dy = 1; dy < l; dy++)
                {
                    var p2 = new Point { X = p.X, Y = p.Y + dy };
                    if (!p2.IsValid(board) || board[p2] != l + '0') break;
                }

                var o = l == 1 ? Orientation.None : l == dx ? Orientation.Vertical : Orientation.Horizontal;
                var hit = new HitInfo { Location = p, Orientation = o, Length = l };
                hit.GetPoints().ToList().ForEach(p3 => board[p3] = '-');

                yield return hit;
            }
        }

        public Game(int height, int width)
        {
            Random = new Random();
            Board = new Board(height, width);
            Ships = Generate().ToList();
        }

        public Game(Board board)
        {
            Random = new Random();
            Ships = Generate(board).ToList();
            Board = new Board(board.Height, board.Width);
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
