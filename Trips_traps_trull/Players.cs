using System;
using System.Collections.Generic;
using System.Text;

namespace Trips_traps_trull
{
    public partial class StartPage
    {
        bool isRobotPlaying = false;
        string player1Name = "Mängija 1";
        string player2Name = "Mängija 2";
        
        List<string> icons = new List<string>()
    {
        "X", "O", "🐧", "🕷️", "🌸"
    };

        string player1Symbol = "X";
        string player2Symbol = "O";

        bool isXTurn = true;

        private Label GetLabel(int row, int col)
        {
            foreach (var child in grid.Children)
            {
                if (child is Border frame &&
                    Grid.GetRow(frame) == row &&
                    Grid.GetColumn(frame) == col &&
                    frame.Content is Label label)
                    return label;
            }
            return null;
        }

        private List<(int row, int col)> GetEmptyCells()
        {
            var list = new List<(int, int)>();

            for (int row = 0; row < currentSize; row++)
            {
                for (int col = 0; col < currentSize; col++)
                {
                    if (IsCellEmpty(row, col))
                        list.Add((row, col));
                }
            }

            return list;
        }

        private bool IsCellEmpty(int row, int col)
        {
            var label = GetLabel(row, col);
            return label != null && label.Text == "";
        }

        private void SetCell(int row, int col, string text)
        {
            var label = GetLabel(row, col);
            if (label != null)
                label.Text = text;
        }

        private (int row, int col)? FindBestMove(string symbol)
        {
            for (int row = 0; row < currentSize; row++)
            {
                for (int col = 0; col < currentSize; col++)
                {
                    if (IsCellEmpty(row, col))
                    {
                        SetCell(row, col, symbol);

                        if (CheckWin(symbol))
                        {
                            SetCell(row, col, ""); 
                            return (row, col);
                        }

                        SetCell(row, col, ""); 
                    }
                }
            }
            return null;
        }
        private async Task RobotMove()
        {
            await Task.Delay(500);
            var move = FindBestMove(player2Symbol);
            if (move != null)
            {
                await MakeMove(move.Value.row, move.Value.col);
                return;
            }
            move = FindBestMove(player1Symbol);
            if (move != null)
            {
                await MakeMove(move.Value.row, move.Value.col);
                return;
            }
            int center = currentSize / 2;
            if (IsCellEmpty(center, center))
            {
                await MakeMove(center, center); 
                return;
            }
            var emptyCells = GetEmptyCells();
            if (emptyCells.Count > 0)
            {
                var rnd = new Random();
                var randomMove = emptyCells[rnd.Next(emptyCells.Count)];
                await MakeMove(randomMove.row, randomMove.col); 
            }
        }


    }
}
