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
                BackgroundColor = Colors.LightGray,
                WidthRequest = 180,
                Padding = new Thickness(10),
                ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Star }
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
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(0, 0, 0, 10)
            };
            var minutesLabel = new Label
            {
                Text = "Minute",
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(0, 0, 0, 10)
            };
            var secondsLabel = new Label
            {
                Text = "Second",
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

            var closeButton = new Button
            {
                Text = "Done",
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
                BackgroundColor = Colors.Purple,
                TextColor = Colors.White,
                CornerRadius = 10,
                Padding = new Thickness(10)
            };

            closeButton.Clicked += (s, e) => Close();

            var stackLayout = new StackLayout
            {
                Children = { grid, closeButton },
                Padding = new Thickness(10)
            };

            Content = new Frame
            {
                Content = stackLayout,
                CornerRadius = 12,
                BackgroundColor = Colors.White,
                Padding = new Thickness(5),
                Margin = new Thickness(20),
                BorderColor = Colors.Gray
            };
        }

        private ScrollView CreateTimeScrollView(int minValue, int maxValue, int selectedValue, Action<int> onValueSelected)
        {
            var stackLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Spacing = 5
            };

            for (int i = minValue; i <= maxValue; i++)
            {
                var label = new Label
                {
                    Text = i.ToString("D2"),
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Padding = new Thickness(5),
                    Margin = new Thickness(2),
                    BackgroundColor = i == selectedValue ? Colors.Purple : Colors.Transparent,
                    TextColor = Colors.Black,
                    FontSize = 16
                };

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

            return new ScrollView
            {
                Content = stackLayout,
                HeightRequest = 150,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never
            };
        }

        private void UpdateSelectedTime(int hours, int minutes, int seconds)
        {
            _selectedTime = new TimeSpan(hours, minutes, seconds);
            TimeSelected?.Invoke(this, _selectedTime);
        }
    }

}
