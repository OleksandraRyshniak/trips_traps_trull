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
                await DisplayAlertAsync("Mäng läbi", name + " võitis!", "OK");

                if (isXTurn) point1p++;
                else point2p++;
                UpdateScore();
                 await ResetGame();
                return;
            }

            if (IsDraw())
            {
                await DisplayAlertAsync("Mäng läbi", "Viik!", "OK");
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

            foreach (var child in grid.Children)
            {
                if (child is Border frame && frame.Content is Label label)
                {
                    int row = Grid.GetRow(frame);
                    int col = Grid.GetColumn(frame);
                    cells[row, col] = label;
                }
            }
            for (int i = 0; i < currentSize; i++)
            {
                if (Enumerable.Range(0, currentSize)
                    .All(j => cells[i, j]?.Text == symbol))
                {
                    for (int j = 0; j < currentSize; j++)
                        

                    return true;
                }
            }
            for (int i = 0; i < currentSize; i++)
            {
                if (Enumerable.Range(0, currentSize)
                    .All(j => cells[j, i]?.Text == symbol))
                {
                    for (int j = 0; j < currentSize; j++)
                       

                    return true;
                }
            }
            if (Enumerable.Range(0, currentSize)
                .All(i => cells[i, i]?.Text == symbol))
            {
                for (int i = 0; i < currentSize; i++)
                   

                return true;
            }
            if (Enumerable.Range(0, currentSize)
                .All(i => cells[i, currentSize - 1 - i]?.Text == symbol))
            {
                for (int i = 0; i < currentSize; i++)
                    

                return true;
            }

            return false;
        }
        private async Task ResetGame()
        {
            foreach (var child in grid.Children)
            {
                if (child is Border frame && frame.Content is Label label)
                {
                    label.Text = "";
                    frame.BackgroundColor = Colors.Transparent;
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
            string record = $"{result}";
            gameHistory.Add(record);
            string raw = string.Join("\n", gameHistory);
            Preferences.Set("game_history", raw);
        }

        private void ClearHistory()
        {
            gameHistory.Clear();
            Preferences.Remove("game_history");
        }
        private void UpdateScore()
        {
            lblPlayer1.Text = $"{player1Name} \n Punktid: {point1p}";
            lblPlayer2.Text = $"{player2Name} \n Punktid: {point2p}";
        }

    }
}
