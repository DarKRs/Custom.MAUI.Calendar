using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Views
{
    public class TimePickerPopup : Popup
    {
        private TimeSpan _selectedTime;
        public event EventHandler<TimeSpan> TimeSelected;

        public TimePickerPopup(TimeSpan initialTime)
        {
            _selectedTime = initialTime;
            CreatePopupContent();
        }

        private void CreatePopupContent()
        {
            var hoursView = CreateTimeScrollView(0, 23, _selectedTime.Hours, value => UpdateSelectedTime(value, _selectedTime.Minutes, _selectedTime.Seconds));
            var minutesView = CreateTimeScrollView(0, 59, _selectedTime.Minutes, value => UpdateSelectedTime(_selectedTime.Hours, value, _selectedTime.Seconds));
            var secondsView = CreateTimeScrollView(0, 59, _selectedTime.Seconds, value => UpdateSelectedTime(_selectedTime.Hours, _selectedTime.Minutes, value));

            var grid = new Grid
            {
                BackgroundColor = Colors.Transparent,
                ColumnSpacing = 15,
                RowSpacing = 10,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto },
                new ColumnDefinition { Width = GridLength.Auto }
            },
                RowDefinitions =
            {
                new RowDefinition { Height = GridLength.Auto },
                new RowDefinition { Height = GridLength.Auto }
            }
            };

            // Header labels
            var hoursLabel = new Label
            {
                Text = "Hour",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(0, 0, 0, 10)
            };
            var minutesLabel = new Label
            {
                Text = "Minute",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(0, 0, 0, 10)
            };
            var secondsLabel = new Label
            {
                Text = "Second",
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(0, 0, 0, 10)
            };

            grid.Children.Add(hoursLabel);
            Grid.SetRow(hoursLabel, 0);
            Grid.SetColumn(hoursLabel, 0);

            grid.Children.Add(minutesLabel);
            Grid.SetRow(minutesLabel, 0);
            Grid.SetColumn(minutesLabel, 1);

            grid.Children.Add(secondsLabel);
            Grid.SetRow(secondsLabel, 0);
            Grid.SetColumn(secondsLabel, 2);

            grid.Children.Add(hoursView);
            Grid.SetRow(hoursView, 1);
            Grid.SetColumn(hoursView, 0);

            grid.Children.Add(minutesView);
            Grid.SetRow(minutesView, 1);
            Grid.SetColumn(minutesView, 1);

            grid.Children.Add(secondsView);
            Grid.SetRow(secondsView, 1);
            Grid.SetColumn(secondsView, 2);

            var stackLayout = new StackLayout
            {
                Children = { grid },
                Padding = new Thickness(5), // Уменьшение Padding для минимизации пространства
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Content = new Frame
            {
                Content = stackLayout,
                CornerRadius = 10,
                BackgroundColor = Colors.White,
                Padding = new Thickness(5), // Уменьшение Padding
                Margin = new Thickness(5),  // Уменьшение Margin
                HasShadow = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private ScrollView CreateTimeScrollView(int minValue, int maxValue, int selectedValue, Action<int> onValueSelected)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 8,
                HorizontalOptions = LayoutOptions.Center
            };

            Label initialLabel = null;

            for (int i = minValue; i <= maxValue; i++)
            {
                var label = new Label
                {
                    Text = i.ToString("D2"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(10, 5),
                    FontSize = 12,
                    TextColor = i == selectedValue ? Colors.White : Colors.Black,
                    BackgroundColor = i == selectedValue ? Colors.Purple : Colors.Transparent,
                    Margin = new Thickness(3),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                if (i == selectedValue)
                {
                    initialLabel = label;
                }

                label.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = new Command(() =>
                    {
                        foreach (var child in stackLayout.Children)
                        {
                            if (child is Label lbl)
                            {
                                lbl.BackgroundColor = Colors.Transparent;
                                lbl.TextColor = Colors.Black;
                            }
                        }
                        label.BackgroundColor = Colors.Purple;
                        label.TextColor = Colors.White;
                        onValueSelected(int.Parse(label.Text));
                    })
                });

                stackLayout.Children.Add(label);
            }

            var scrollView = new ScrollView
            {
                Content = stackLayout,
                HeightRequest = 150, // Ограничение высоты для ScrollView
                VerticalScrollBarVisibility = ScrollBarVisibility.Never,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            scrollView.Loaded += (s, e) =>
            {
                if (initialLabel != null)
                {
                    scrollView.ScrollToAsync(initialLabel, ScrollToPosition.Center, animated: false);
                }
            };

            return scrollView;
        }

        private void UpdateSelectedTime(int hours, int minutes, int seconds)
        {
            _selectedTime = new TimeSpan(hours, minutes, seconds);
            TimeSelected?.Invoke(this, _selectedTime);
        }
    }
}
