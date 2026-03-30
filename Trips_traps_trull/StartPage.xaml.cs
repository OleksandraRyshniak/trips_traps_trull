using Microsoft.Maui.Controls;

namespace Trips_traps_trull;

public partial class StartPage : ContentPage
{
    Label lbl, lblTurn, lblPlayer1, lblPlayer2;
    Grid grid;
    Button btn_start, btn_end, btn_history;
    VerticalStackLayout vsl;
    HorizontalStackLayout hsl;
    int point1p = 0;
    int point2p = 0;

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
            Text = "Uus mäng",
            FontSize = 24,
            BackgroundColor = Colors.LightPink,
            HorizontalOptions = LayoutOptions.Center
        };

        lblPlayer1 = new Label
        {
            Text = "",
            FontSize = 22
        };
        lblPlayer2 = new Label
        {
            Text = "",
            FontSize = 22
        };
        btn_history = new Button
        {
            Text = "Ajalugu",
            FontSize = 20
        };
        btn_history.Clicked += async (s, e) =>
        {
            LoadHistory();
            await Navigation.PushAsync(new HistoryPage(gameHistory, ClearHistory));
            
        };

        vsl = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(20),
            Children = { lbl, btn_start, btn_history }
        };



        btn_end.Clicked += (s, e) =>
        {
            SaveHistory($"{player1Name}: {point1p}p. | {player2Name}: {point2p}p.");
            vsl.Children.Clear();
            vsl.Children.Add(lbl);
            vsl.Children.Add(btn_start);
            vsl.Children.Add(btn_history);
            isRobotPlaying = false;
            isXTurn = true;
            point1p = 0;
            point2p = 0;
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

        bool answer = await DisplayAlertAsync("Küsimus", "Kas mängid robotiga?", "Jah", "Ei");
        isRobotPlaying = answer;

        string? name1 = await DisplayPromptAsync("Mängija 1", "Mis on mängija 1 nimi?");
        if (string.IsNullOrWhiteSpace(name1)) name1 = "Mängija 1";
        player1Name = name1;
        Preferences.Set("PlayerName1", name1);

        if (!isRobotPlaying)
        {
            string? name2 = await DisplayPromptAsync("Mängija 2", "Mis on mängija 2 nimi?");
            if (string.IsNullOrWhiteSpace(name2)) name2 = "Mängija 2";
            player2Name = name2;
            Preferences.Set("PlayerName2", name2);
        }
        else
        {
            player2Name = "Robot";
        }

        LoadHistory();

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
                Border frame = new Border
                {
                    Stroke = Colors.Black,
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
        hsl = new HorizontalStackLayout
        {
            Spacing = 50,
            HorizontalOptions = LayoutOptions.Center

        };
        if (lblPlayer1.Parent is Layout oldParent1)
            oldParent1.Remove(lblPlayer1);

        if (lblPlayer2.Parent is Layout oldParent2)
            oldParent2.Remove(lblPlayer2);
        hsl.Add(lblPlayer1);
        hsl.Add(lblPlayer2);
        lblPlayer1.Text = $"{player1Name} \n Punktid: {point1p}";
        lblPlayer2.Text = $"{player2Name} \n Punktid: {point2p}";
        isXTurn = true;
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

        vsl.Children.Clear();
        vsl.Children.Add(lbl);
        vsl.Children.Add(lblTurn);
        vsl.Children.Add(grid);
        vsl.Children.Add(hsl);
        vsl.Children.Add(btn_end);
        vsl.Children.Add(btn_history);

        if (isRobotPlaying && !isXTurn)
        {
            await RobotMove();
        }
    }

    private async void Btn_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Border frame) return;

        int index = grid.Children.IndexOf(frame);
        int row = index / currentSize;
        int col = index % currentSize;

        await MakeMove(row, col);
    }
}