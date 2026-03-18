using Microsoft.Maui.Controls.Handlers.Items;

namespace Trips_traps_trull;

public partial class StartPage : ContentPage
{
	Label lbl;
    Grid grid;
    Button btn;
    VerticalStackLayout vsl;
    bool isXTurn = true;

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
            HeightRequest = 300,
            WidthRequest = 300,
            HorizontalOptions = LayoutOptions.Center
        };

        for (int i = 0; i < 3; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
        }

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Frame frame = new Frame
                {
                    BorderColor = Colors.Black,
                    BackgroundColor = Colors.White
                };

                Label label = new Label
                {
                    Text = "",
                    FontSize = 32,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                frame.Content = label;
                grid.Add(frame, row, col);
                TapGestureRecognizer tap = new TapGestureRecognizer();
                tap.Tapped += Btn_Clicked;
                frame.GestureRecognizers.Add(tap);
            }

        }
        btn = new Button
        {
            Text = "Start Game",
            FontSize = 24,
            HorizontalOptions = LayoutOptions.Center
        };


        vsl = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(20),
            Children = { lbl, btn }
        };
        Content = vsl;
        btn.Clicked += (s, e) =>
        {
            vsl.Children.Clear();
            vsl.Children.Add(lbl);
            vsl.Children.Add(grid);
        };
    }


    private async void Btn_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Frame frame) return;

        if (frame.Content is not Label label) return;

        if (label.Text != "") return;

        string symbol;

        if (isXTurn)
        {
            symbol = "X";
        }
        else 
        {
            symbol = "O";
        }
        label.Text = symbol;
        bool IsDraw()
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
        if (CheckWin(symbol))
        {
            await DisplayAlertAsync("Game Over", symbol + " wins!", "OK");
            ResetGame();
        }
        else if (IsDraw())
        {
            await DisplayAlertAsync("Game Over", "Viik!", "OK");
            ResetGame();
        }

        isXTurn = !isXTurn;
    }


    bool CheckWin(string symbol)
    {
        Label[,] cells = new Label[3, 3];
        int index = 0;

        foreach (var child in grid.Children)
        {
            if (child is Frame frame && frame.Content is Label label)
            {
                int row = index / 3;
                int col = index % 3;
                cells[row, col] = label;
                index++;
            }
        }

        for (int i = 0; i < 3; i++)
        {
            if (cells[i, 0].Text == symbol && cells[i, 1].Text == symbol && cells[i, 2].Text == symbol) return true;
            if (cells[0, i].Text == symbol && cells[1, i].Text == symbol && cells[2, i].Text == symbol) return true;
        }

        if (cells[0, 0].Text == symbol && cells[1, 1].Text == symbol && cells[2, 2].Text == symbol) return true;
        if (cells[0, 2].Text == symbol && cells[1, 1].Text == symbol && cells[2, 0].Text == symbol) return true;

        return false;
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
    }

}