using System;
using System.Collections.Generic;
using System.Text;

namespace Trips_traps_trull
{
    public partial class StartPage
    {
        int currentSize = 3;


        Dictionary<string, int> size = new Dictionary<string, int>()
    {
        {"3x3", 3}, {"4x4", 4}, {"5x5", 5}
    };

        private async void MakeMove(int row, int col, string symbol)
        {
            var label = GetLabel(row, col);
            if (label == null) return;

            label.Text = symbol;

            if (CheckWin(symbol))
            {
                await DisplayAlertAsync("Game Over", symbol + " wins!", "OK");
                ResetGame();
                return;
            }

            if (IsDraw())
            {
                await DisplayAlertAsync("Game Over", "Viik!", "OK");
                ResetGame();
                return;
            }

            isXTurn = !isXTurn;
        }

        private bool IsDraw()
        {
            foreach (var child in grid.Children)
            {
                if (child is Border frame && frame.Content is Label label)
                {
                    if (label.Text == "")
                        return false;
                }
            }
            return true;
        }

        private bool CheckWin(string symbol)
        {
            Label[,] cells = new Label[currentSize, currentSize];
            int index = 0;

            foreach (var child in grid.Children)
            {
                if (child is Border frame && frame.Content is Label label)
                {
                    int row = index / currentSize;
                    int col = index % currentSize;
                    cells[row, col] = label;
                    index++;
                }
            }

            for (int i = 0; i < currentSize; i++)
            {
                bool rowWin = true;
                bool colWin = true;

                for (int j = 0; j < currentSize; j++)
                {
                    if (cells[i, j].Text != symbol) rowWin = false;
                    if (cells[j, i].Text != symbol) colWin = false;
                }

                if (rowWin || colWin) return true;
            }

            bool diag1 = true;
            bool diag2 = true;

            for (int i = 0; i < currentSize; i++)
            {
                if (cells[i, i].Text != symbol) diag1 = false;
                if (cells[i, currentSize - 1 - i].Text != symbol) diag2 = false;
            }

            return diag1 || diag2;
        }

        private void ResetGame()
        {
            foreach (var child in grid.Children)
            {
                if (child is Border frame && frame.Content is Label label)
                {
                    label.Text = "";
                }
            }

            isXTurn = new Random().Next(2) == 0;

            string currentName;
            string currentSymbol;

            if (isXTurn)
            {
                currentName = player1Name;
                currentSymbol = player1Symbol;
            }
            else
            {
                currentName = player2Name;
                currentSymbol = player2Symbol;
            }

            lblTurn.Text = $"Mängib: {currentSymbol}";
        }


    }
}
