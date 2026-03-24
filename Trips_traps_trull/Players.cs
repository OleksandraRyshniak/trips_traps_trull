using System;
using System.Collections.Generic;
using System.Text;

namespace Trips_traps_trull
{
    public partial class StartPage
    {
        // Новый флаг — играет ли второй игрок робот
        bool isRobotPlaying = false;
        string player1Name = "Mängija 1";
        string player2Name = "Mängija 2";
        bool name = false;
        List<string> icons = new List<string>()
    {
        "X", "O", "🐧", "🕷️", "🌸"
    };

        string player1Symbol = "X";
        string player2Symbol = "O";

        bool isXTurn = true;

        private Label GetLabel(int row, int col)
        {
            int index = row * currentSize + col;

            if (grid.Children[index] is Border frame && frame.Content is Label label)
                return label;

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
                            SetCell(row, col, ""); // откат
                            return (row, col);
                        }

                        SetCell(row, col, ""); // откат
                    }
                }
            }
            return null;
        }

        // Логика хода робота: ставит символ в первую свободную клетку
        private async Task RobotMove()
        {
            await Task.Delay(500);

            // 1. Если можем выиграть — делаем победный ход
            var move = FindBestMove(player2Symbol);
            if (move != null)
            {
                MakeMove(move.Value.row, move.Value.col, player2Symbol);
                return;
            }

            // 2. Если игрок может выиграть — блокируем
            move = FindBestMove(player1Symbol);
            if (move != null)
            {
                MakeMove(move.Value.row, move.Value.col, player2Symbol);
                return;
            }

            // 3. Берём центр (если есть)
            int center = currentSize / 2;
            if (IsCellEmpty(center, center))
            {
                MakeMove(center, center, player2Symbol);
                return;
            }

            // 4. Случайный ход
            var emptyCells = GetEmptyCells();
            if (emptyCells.Count > 0)
            {
                var rnd = new Random();
                var randomMove = emptyCells[rnd.Next(emptyCells.Count)];
                MakeMove(randomMove.row, randomMove.col, player2Symbol);
            }
        }


    }
}
