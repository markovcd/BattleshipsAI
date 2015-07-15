using System;
using System.Linq;

namespace Ships
{
    static class Solution
    {
        static void Main()
        {
            int n = int.Parse(Console.ReadLine());
            var board = Enumerable.Range(0, n).Select(i => Console.ReadLine()).ToList();            
            var b = new Battleships(new Board(board), new Random());
            b.NextMove();
            Console.WriteLine(b.LastMove);
        }

    }

}
