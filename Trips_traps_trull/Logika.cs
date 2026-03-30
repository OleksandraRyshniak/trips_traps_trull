namespace Trips_traps_trull
{
    public partial class StartPage
    {
        int currentSize = 3;


        Dictionary<string, int> size = new Dictionary<string, int>()
    {
        {"3x3", 3}, {"4x4", 4}, {"5x5", 5}
    };

        private async Task MakeMove(int row, int col)
        {
            var label = GetLabel(row, col);
            if (label == null || label.Text != "") return;

            string symbol = isXTurn ? player1Symbol : player2Symbol;
            string name = isXTurn ? player1Name : player2Name;

            label.Text = symbol;

            if (CheckWin(symbol))
            {
                await DisplayAlertAsync("Game Over", name + " wins!", "OK");

                if (isXTurn) point1p++;
                else point2p++;

                SaveHistory(name);

                UpdateScore();
                 await ResetGame();
                return;
            }

            if (IsDraw())
            {
                await DisplayAlertAsync("Game Over", "Viik!", "OK");
                SaveHistory("Draw");
                await ResetGame();
                return;
            }

            isXTurn = !isXTurn;

            lblTurn.Text = $"Mängib {(isXTurn ? player1Name : player2Name)}: {(isXTurn ? player1Symbol : player2Symbol)}";

            if (isRobotPlaying && !isXTurn)
                await RobotMove();
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
                if (Enumerable.Range(0, currentSize).All(j => cells[i, j].Text == symbol))
                    return true;

                if (Enumerable.Range(0, currentSize).All(j => cells[j, i].Text == symbol))
                    return true;
            }

            if (Enumerable.Range(0, currentSize).All(i => cells[i, i].Text == symbol))
                return true;

            if (Enumerable.Range(0, currentSize).All(i => cells[i, currentSize - 1 - i].Text == symbol))
                return true;

            return false;
        }
        private async Task ResetGame()
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

            lblTurn.Text = $"Mängib {currentName}: {currentSymbol}";

            if (isRobotPlaying && !isXTurn)
                await RobotMove();
        }

        List<string> gameHistory = new List<string>();

        private void LoadHistory()
        {
            string raw = Preferences.Get("game_history", "");
            if (!string.IsNullOrEmpty(raw))
                gameHistory = new List<string>(raw.Split('\n', StringSplitOptions.RemoveEmptyEntries));
            else
                gameHistory = new List<string>();
        }

        private void SaveHistory(string result)
        {
            string winner = result == "Draw" ? "Ничья" : result;
            string record = $"{DateTime.Now:dd.MM.yyyy HH:mm} | {player1Name} {point1p} : {point2p} {player2Name} | Победитель: {winner}";

            gameHistory.Add(record);

            string raw = string.Join('\n', gameHistory);
            Preferences.Set("game_history", raw);
        }

        private void ClearHistory()
        {
            gameHistory.Clear();
            Preferences.Remove("game_history");
        }
        private void UpdateScore()
        {
            lblPlayer1.Text = $"{player1Name} \n Points: {point1p}";
            lblPlayer2.Text = $"{player2Name} \n Points: {point2p}";
        }

    }
}
