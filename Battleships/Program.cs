using System;
using System.Collections.Generic;
using System.Linq;

namespace Ships
{
    static class Solution
    {
        static void Main()
        {
            string s = Console.ReadLine();

            if (s == "INIT")
            {
                foreach (var ship in Battleships.Generate().OrderBy(i => i.Length))
                {
                    var points = new HashSet<string>
                    {
                        ship.Location.ToString(),
                        new Point
                        {
                            X = ship.Location.X + (ship.Orientation == Orientation.Vertical ? ship.Length - 1 : 0),
                            Y = ship.Location.Y + (ship.Orientation == Orientation.Vertical ? 0 : ship.Length - 1),
                        }.ToString()
                    };

                    Console.WriteLine(points.Aggregate((a, b) => a + ":" + b));
                }
            }
            else
            {
                int n = int.Parse(s);

                var board = Enumerable.Range(0, n).Select(i => Console.ReadLine()).ToList();
                var b = new Battleships(new Board(board), new Random());
                b.NextMove();
                Console.WriteLine(b.LastMove);
            }
        }
    }

}
