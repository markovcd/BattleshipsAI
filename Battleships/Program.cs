using System;

namespace Ships
{
    static class Solution
    {
        public static Board ReadBoard(int height, int width)
        {
            var b = new char[height, width];
            for (int x = 0; x < height; x++)
            {
                string s = Console.ReadLine();
                for (int y = 0; y < width; y++)
                    b[x, y] = s[y];
            }

            return new Board(b);
        }

        static void Main()
        {
            int n = int.Parse(Console.ReadLine());
            var b = new Battleships(ReadBoard(n, n));
            b.NextMove();
            Console.WriteLine(b.LastMove);
        }

    }

}
