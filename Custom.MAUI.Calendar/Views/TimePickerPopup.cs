﻿using CommunityToolkit.Maui.Core;
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
        private string TimeFormat;
        public event EventHandler<TimeSpan> TimeSelected;

        public TimePickerPopup(TimeSpan initialTime, string format)
        {
            _selectedTime = initialTime;
            TimeFormat = format.ToLower();
            CreatePopupContent();
        }

        private void CreatePopupContent()
        {
            var grid = new Grid
            {
                BackgroundColor = Colors.Transparent,
                ColumnSpacing = 15,
                RowSpacing = 5, 
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

            int currentColumn = 0;


            if (TimeFormat.Contains("hh"))
            {
                AddTimeComponent(grid, "Hour", 0, 23, _selectedTime.Hours, value => UpdateSelectedTime(value, _selectedTime.Minutes, _selectedTime.Seconds), currentColumn);
                currentColumn++;
            }

            if (TimeFormat.Contains("mm"))
            {
                AddTimeComponent(grid, "Minute", 0, 59, _selectedTime.Minutes, value => UpdateSelectedTime(_selectedTime.Hours, value, _selectedTime.Seconds), currentColumn);
                currentColumn++;
            }

            if (TimeFormat.Contains("ss"))
            {
                AddTimeComponent(grid, "Second", 0, 59, _selectedTime.Seconds, value => UpdateSelectedTime(_selectedTime.Hours, _selectedTime.Minutes, value), currentColumn);
                currentColumn++;
            }

            var stackLayout = new StackLayout
            {
                Children = { grid },
                Padding = new Thickness(5), 
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            Content = new Frame
            {
                Content = stackLayout,
                CornerRadius = 10,
                BackgroundColor = Colors.White,
                Padding = new Thickness(5), 
                Margin = new Thickness(5),  
                HasShadow = true,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
        }

        private void AddTimeComponent(Grid grid, string labelText, int minValue, int maxValue, int initialValue, Action<int> onValueSelected, int columnIndex)
        {
            var label = new Label
            {
                Text = labelText,
                TextColor = Colors.Black,
                HorizontalOptions = LayoutOptions.Center,
                FontSize = 14,
                FontAttributes = FontAttributes.Bold,
                Padding = new Thickness(0, 0, 0, 2)
            };
            var timeScrollView = CreateTimeScrollView(minValue, maxValue, initialValue, onValueSelected);

            grid.Children.Add(label);
            Grid.SetRow(label, 0);
            Grid.SetColumn(label, columnIndex);

            grid.Children.Add(timeScrollView);
            Grid.SetRow(timeScrollView, 1);
            Grid.SetColumn(timeScrollView, columnIndex);
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

                        // Программная прокрутка, чтобы выделенный элемент оставался по центру
                        ((ScrollView)stackLayout.Parent).ScrollToAsync(label, ScrollToPosition.Center, true);
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
