using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ships;

namespace IntegrationTest2
{
    class Program
    {
        static IEnumerable<int> RunGames(int size)
        {
            int i = size;
            while (i-- > 0)
            {
                //Console.Clear();
                Console.WriteLine("Games left: {0}", i);

                if (File.Exists("battleships.txt")) File.Delete("battleships.txt");

                int moves = 0;

                var game = new Game(10, 10);
                

                Battleships battleships;
                do
                {
                    battleships = new Battleships(game.Board, game.Random);
                    battleships.NextMove();
                    moves++;
                } while (!game.Move(battleships.LastMove.Value));

                yield return moves;
                Console.WriteLine("Moves to finish game: {0}", moves);
            }
        }
        
        static void Main(string[] args)
        {
            Console.Write("Specify sample size: ");
            int n = int.Parse(Console.ReadLine());
            Console.WriteLine("Average moves: {0}", RunGames(n).Average());
            Console.ReadLine();
        }
    }
}
