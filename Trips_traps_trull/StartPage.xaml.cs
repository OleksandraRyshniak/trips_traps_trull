using Microsoft.Maui.Controls;

namespace Trips_traps_trull;

public partial class StartPage : ContentPage
{
    Label lbl, lblTurn;
    Grid grid;
    Button btn_start, btn_end;
    VerticalStackLayout vsl;

    bool isXTurn = true;
    int currentSize = 3;

    Dictionary<string, int> size = new Dictionary<string, int>()
    {
        {"3x3", 3}, {"4x4", 4}, {"5x5",5 }
    };
    List<string> icons = new List<string>()
    {
        "X", "O", "🐧", "🕷️", "🌸"
    };

    string player1Symbol = "X";
    string player2Symbol = "O";

    // Новый флаг — играет ли второй игрок робот
    bool isRobotPlaying = false;

    public StartPage()
    {
        lbl = new Label
        {
            Text = "Trips, Traps, and Trull",
            FontSize = 32,
            HorizontalOptions = LayoutOptions.Center
        };

        grid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        btn_start = new Button
        {
            Text = "Alusta mäng",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center
        };

        lblTurn = new Label
        {
            FontSize = 22,
            HorizontalOptions = LayoutOptions.Center
        };

        btn_start.Clicked += Options;

        btn_end = new Button
        {
            Text = "New Game",
            FontSize = 24,
            BackgroundColor = Colors.LightPink,
            HorizontalOptions = LayoutOptions.Center
        };


        vsl = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(20),
            Children = { lbl, btn_start }
        };
        btn_end.Clicked += (s, e) =>
        {
            vsl.Children.Clear();
            vsl.Children.Add(lbl);
            vsl.Children.Add(btn_start);
            isRobotPlaying = false;
            isXTurn = true;
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            grid.Children.Clear();
        };


        Content = vsl;
    }

    private async void Options(object? sender, EventArgs e)
    {
        string sizeResult = await DisplayActionSheetAsync("Vali suurus", "Tühistamine", null, size.Keys.ToArray());

        currentSize = size.ContainsKey(sizeResult) ? size[sizeResult] : 3;

        string p1 = await DisplayActionSheetAsync("Mängija 1 märk", "Tühistamine", null, icons.ToArray());
        if (string.IsNullOrEmpty(p1) || p1 == "Tühistamine" || p1 == null)
            p1 = "X";

        var remainingIcons = icons.Where(i => i != p1).ToArray();
        if (remainingIcons.Length == 0) remainingIcons = new string[] { "O" };

        string p2 = await DisplayActionSheetAsync("Mängija 2 märk", "Tühistamine", null, remainingIcons);
        if (string.IsNullOrEmpty(p2) || p2 == "Tühistamine" || p2 == null)
            p2 = "O";

        player1Symbol = p1;
        player2Symbol = p2;

        // Выбор: играть против робота или с человеком
        string playWith = await DisplayActionSheetAsync("Kas mängid robotiga?", "Tühistamine", null, new string[] { "Jah", "Ei" });
        isRobotPlaying = playWith == "Jah";

        // Очистка и создание сетки
        grid.RowDefinitions.Clear();
        grid.ColumnDefinitions.Clear();
        grid.Children.Clear();

        for (int i = 0; i < currentSize; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        for (int row = 0; row < currentSize; row++)
        {
            for (int col = 0; col < currentSize; col++)
            {
                Frame frame = new Frame
                {
                    BorderColor = Colors.Black,
                    BackgroundColor = Colors.White,
                    Padding = 0,
                    MinimumHeightRequest = 70,
                    MinimumWidthRequest = 70
                };

                Label label = new Label
                {
                    Text = "",
                    FontSize = 40,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                frame.Content = label;
                grid.Add(frame, col, row);

                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Btn_Clicked;
                frame.GestureRecognizers.Add(tap);
            }
        }

        isXTurn = true;
        lblTurn.Text = $"Mängib: {player1Symbol}";

        vsl.Children.Clear();
        vsl.Children.Add(lbl);
        vsl.Children.Add(lblTurn);
        vsl.Children.Add(grid);
        vsl.Children.Add(btn_end);


        // Если начинает робот
        if (isRobotPlaying && !isXTurn)
        {
                await RobotMove();        // обычный для 4x4 и 5x5
        }
    }

    // Обработчик клика по ячейке
    private async void Btn_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Frame frame) return;
        if (frame.Content is not Label label) return;
        if (label.Text != "") return;

        string symbol = isXTurn ? player1Symbol : player2Symbol;
        label.Text = symbol;

        if (CheckWin(symbol))
        {
            await DisplayAlert("Game Over", symbol + " wins!", "OK");
            ResetGame();
            return;
        }

        if (IsDraw())
        {
            await DisplayAlert("Game Over", "Viik!", "OK");
            ResetGame();
            return;
        }

        isXTurn = !isXTurn;
        lblTurn.Text = $"Mängib: {(isXTurn ? player1Symbol : player2Symbol)}";
        // Если сейчас ходит робот, делаем ход
        if (isRobotPlaying && !isXTurn)
        {
            await RobotMove();
        }
    }
    private Label GetLabel(int row, int col)
    {
        int index = row * currentSize + col;

        if (grid.Children[index] is Frame frame && frame.Content is Label label)
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
    private async void MakeMove(int row, int col, string symbol)
    {
        var label = GetLabel(row, col);
        if (label == null) return;

        label.Text = symbol;

        if (CheckWin(symbol))
        {
            await DisplayAlert("Game Over", symbol + " wins!", "OK");
            ResetGame();
            return;
        }

        if (IsDraw())
        {
            await DisplayAlert("Game Over", "Viik!", "OK");
            ResetGame();
            return;
        }

        isXTurn = !isXTurn;
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

    private bool IsDraw()
    {
        foreach (var child in grid.Children)
        {
            if (child is Frame frame && frame.Content is Label label)
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
            if (child is Frame frame && frame.Content is Label label)
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
            if (child is Frame frame && frame.Content is Label label)
            {
                label.Text = "";
            }
        }

        isXTurn = true;
        lblTurn.Text = $"Mängib: {player1Symbol}";
    }
}