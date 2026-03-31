namespace Trips_traps_trull;

public partial class HistoryPage : ContentPage
{
    public HistoryPage(List<string> history, Action onClear)
    {
        Title = "Mängu ajalugu";

        var items = history.AsEnumerable().Reverse().ToList();

        var listView = new CollectionView
        {
            ItemsSource = items,
            ItemTemplate = new DataTemplate(() =>
            {
                var label = new Label
                {
                    FontSize = 16,
                    Padding = new Thickness(10),
                    LineBreakMode = LineBreakMode.WordWrap
                };
                label.SetBinding(Label.TextProperty, ".");

                return new Border
                {
                    Stroke = Colors.LightGray,
                    BackgroundColor = Colors.White,
                    Margin = new Thickness(0, 6),
                    Padding = new Thickness(4),
                    Content = label
                };
            })
        };

        var btnClear = new Button
        {
            Text = "Kustuta ajalugu",
            BackgroundColor = Colors.LightCoral,
            TextColor = Colors.Black,
            FontSize = 16,
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
                    Text = $"Mängu ajalugu",
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