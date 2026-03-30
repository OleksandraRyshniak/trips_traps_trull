namespace Trips_traps_trull;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(List<string> history, Action onClear)
    {
        Title = "История игр";

        var items = history.AsEnumerable().Reverse().ToList();

        var listView = new CollectionView
        {
            ItemsSource = items,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label { FontSize = 14, Padding = new Thickness(8) };
                label.SetBinding(Label.TextProperty, ".");
                return new Border
                {
                    Stroke = Colors.LightGray,
                    Margin = new Thickness(0, 3),
                    Content = label
                };
            })
        };

        var btnClear = new Button
        {
            Text = "Очистить историю",
            BackgroundColor = Colors.LightCoral,
            HorizontalOptions = LayoutOptions.Center
        };

        btnClear.Clicked += (s, e) =>
        {
            onClear();
            items.Clear();
            listView.ItemsSource = null;
            listView.ItemsSource = items;
        };

        Content = new StackLayout
        {
            Padding = 20,
            Spacing = 10,
            Children =
            {
                new Label
                {
                    Text = $"История игр ({history.Count})",
                    FontSize = 24,
                    FontAttributes = FontAttributes.Bold,
                    HorizontalOptions = LayoutOptions.Center
                },
                btnClear,
                listView
            }
        };
    }
}