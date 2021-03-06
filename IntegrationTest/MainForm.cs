﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ships;

namespace IntegrationTest
{

    public partial class MainForm : Form
    {
        public Game Game { get; set; }

        public void GenerateGame()
        {
            //Game = new Game(10, 10);
            //GameField.Text = PrintBoard(Game.GeneratedBoard);

            var s = new[]
            {
                "---4444---",
                "-3---221--",
                "-32-------",
                "-32-------",
                "----------",
                "--55555---",
                "----------",
                "----------",
                "----------",
                "--1-------"

            };

            /*
            var s = new[]
            {
                "----------",
                "------1---",
                "3----22---",
                "3----5----",
                "3----5--1-",
                "--22-5----",
                "-----5----",
                "-----5----",
                "-----4444-",
                "----------"
            };
            */


            Game = new Game(new Board(s));
            GameField.Text = PrintBoard(Game.GeneratedBoard);
        }

        public void RunGame()
        {
            if (File.Exists("battleships.txt")) File.Delete("battleships.txt");
            Battleships b;
            MovesList.Items.Clear();

            do
            {
                b = new Battleships(Game.Board, Game.Random);
                b.NextMove();

                MovesList.Items.Add(b.Board.Duplicate());

            } while (!Game.Move(b.LastMove.Value));
        }

        public void NextMove()
        {
            var b = new Battleships(Game.Board, Game.Random);
            b.NextMove();
            
            Game.Move(b.LastMove.Value);
            MovesList.Items.Add(b.Board.Duplicate());
            MovesList.SelectedIndex = MovesList.Items.Count - 1;
        }

        public string PrintBoard(Board board)
        {
            var result = new StringBuilder();

            for (int x = -1; x < board.Height; x++)
            {
                for (int y = -1; y < board.Width; y++)
                {
                    if (y == -1 && x == -1) result.Append("  ");
                    else if (x == -1) result.Append(y);
                    else if (y == -1) result.AppendFormat("{0} ", x);
                    else result.Append(board[new Point { X = x, Y = y }]);
                }

                result.AppendLine();
            }

            return result.ToString();
        }

        public MainForm()
        {
            InitializeComponent();
            GenerateGame();
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            GenerateGame();
        }

        private void RunButton_Click(object sender, EventArgs e)
        {
            RunGame();
        }

        private void MovesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var b = MovesList.SelectedItem as Board;
            BattleshipsField.Text = PrintBoard(b);
        }

        private void NextButton_Click(object sender, EventArgs e)
        {
            NextMove();
        }
    }
}
