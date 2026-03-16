using Microsoft.Maui.Controls.Handlers.Items;

namespace Trips_traps_trull;

public partial class StartPage : ContentPage
{
	Label lbl;
    Grid grid;
    bool isXTurn = true;
    Button btn;
    public StartPage()
    {
        lbl = new Label
        {
            Text = "Trips, Traps, and Trull",
            FontSize = 32,
            HorizontalOptions = LayoutOptions.Center

        };

        grid = new Grid {
            HeightRequest = 300,
            WidthRequest = 300,
            HorizontalOptions = LayoutOptions.Center
        };

        for (int i = 0; i < 3; i++)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
        }

        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                btn = new Button
                {
                    FontSize = 40,
                    BackgroundColor = Colors.White,
                    BorderColor = Colors.Black,
                    BorderWidth = 2
                };
                btn.Clicked += Btn_Clicked;
                grid.Add(btn, col, row);
            }
        }

        Content = new VerticalStackLayout
        {
            Spacing = 20,
            Padding = new Thickness(20),
            Children = {  lbl, grid }
        };
    }


       private async void Btn_Clicked(object? sender, EventArgs e)
    {
        if (sender is not Button btn)
            return;

        if (btn.Text != "")
            return;

        string symbol = isXTurn ? "X" : "O";
        btn.Text = symbol;

        if (CheckWin(symbol))
        {
            await DisplayAlertAsync("Game Over", symbol + " wins!", "OK");
            ResetGame();
        }

        isXTurn = !isXTurn;
    }
    

    bool CheckWin(string symbol)
    {
        Button[,] buttons = new Button[3, 3];

        int index = 0;

        foreach (Button b in grid.Children)
        {
            int row = index / 3;
            int col = index % 3;
            buttons[row, col] = b;
            index++;
        }

        for (int i = 0; i < 3; i++)
        {
            if (buttons[i, 0].Text == symbol &&
                buttons[i, 1].Text == symbol &&
                buttons[i, 2].Text == symbol)
                return true;

            if (buttons[0, i].Text == symbol &&
                buttons[1, i].Text == symbol &&
                buttons[2, i].Text == symbol)
                return true;
        }

        if (buttons[0, 0].Text == symbol &&
            buttons[1, 1].Text == symbol &&
            buttons[2, 2].Text == symbol)
            return true;

        if (buttons[0, 2].Text == symbol &&
            buttons[1, 1].Text == symbol &&
            buttons[2, 0].Text == symbol)
            return true;

        return false;
    }

    private void ResetGame()
    {
        foreach (Button b in grid.Children)
        {
            b.Text = "";
        }
        isXTurn = true;
    }

}