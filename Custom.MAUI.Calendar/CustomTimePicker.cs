using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Layouts;

namespace Custom.MAUI.Calendar
{
    public class CustomTimePicker : ContentView
    {
        private TimeSpan _selectedTime;
        private Entry _timeEntry;
        private Button _dropdownButton;
        private AbsoluteLayout _popupOverlay;
        private Grid _popupContent;

        public event EventHandler<TimeSpan> TimeSelected;

        public static readonly BindableProperty SelectedTimeProperty = BindableProperty.Create(
            nameof(SelectedTime),
            typeof(TimeSpan),
            typeof(CustomTimePicker),
            default(TimeSpan),
            propertyChanged: OnSelectedTimeChanged);

        public TimeSpan SelectedTime
        {
            get => (TimeSpan)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        private static void OnSelectedTimeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomTimePicker timePicker && newValue is TimeSpan newTime)
            {
                timePicker._selectedTime = newTime;
                timePicker._timeEntry.Text = newTime.ToString(@"hh\:mm\:ss");
            }
        }

        public CustomTimePicker()
        {
            WidthRequest = 150;
            HeightRequest = 40;

            _selectedTime = DateTime.Now.TimeOfDay;

            _timeEntry = new Entry
            {
                Text = _selectedTime.ToString(@"hh\:mm\:ss"),
                Keyboard = Keyboard.Text,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center
            };

            _timeEntry.TextChanged += OnTimeEntryTextChanged;
            _dropdownButton = new Button
            {
                Text = "▼",
                FontSize = 12,
                WidthRequest = 30,
                HeightRequest = 30,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Center,
                Padding = 0,
                Margin = new Thickness(0)
            };

            _dropdownButton.Clicked += (s, e) => ToggleTimePickerPopup();
            _timeEntry.Focused += (s, e) => ShowTimePickerPopup();

            var layout = new Grid
            {
                Children = { _timeEntry, _dropdownButton },
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Auto }
                }
            };

            Content = layout;
        }

        private void ToggleTimePickerPopup()
        {
            if (_popupOverlay != null)
            {
                CloseTimePickerPopup();
            }
            else
            {
                ShowTimePickerPopup();
            }
        }

        private void ShowTimePickerPopup()
        {
            if (_popupOverlay != null)
            {
                return; // Prevent multiple popups
            }

            _popupOverlay = new AbsoluteLayout
            {
                BackgroundColor = new Color(0, 0, 0, 0),
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            _popupContent = new Grid
            {
                BackgroundColor = Colors.White,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                WidthRequest = this.Width, // Match width of CustomTimePicker
                Padding = 10
            };

            var hoursView = CreateTimeScrollView(0, 23, _selectedTime.Hours, value => UpdateSelectedTime(value, _selectedTime.Minutes, _selectedTime.Seconds));
            var minutesView = CreateTimeScrollView(0, 59, _selectedTime.Minutes, value => UpdateSelectedTime(_selectedTime.Hours, value, _selectedTime.Seconds));
            var secondsView = CreateTimeScrollView(0, 59, _selectedTime.Seconds, value => UpdateSelectedTime(_selectedTime.Hours, _selectedTime.Minutes, value));

            _popupContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            _popupContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            _popupContent.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            _popupContent.Children.Add(hoursView);
            Grid.SetColumn(hoursView, 0);

            _popupContent.Children.Add(minutesView);
            Grid.SetColumn(minutesView, 1);

            _popupContent.Children.Add(secondsView);
            Grid.SetColumn(secondsView, 2);

            // Positioning popup content in the center of the overlay
            AbsoluteLayout.SetLayoutFlags(_popupContent, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(_popupContent, new Rect(0.5, 0.5, -1, -1)); // Centered

            _popupOverlay.Children.Add(_popupContent);

            // Adding overlay to the nearest layout that can host it
            if (Parent is Layout parentLayout)
            {
                parentLayout.Children.Add(_popupOverlay);
            }
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
                    BackgroundColor = i == selectedValue ? Colors.Purple : Colors.Transparent,
                    TextColor = Colors.Black
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
                            }
                        }
                        label.BackgroundColor = Colors.Purple;
                        onValueSelected(int.Parse(label.Text));
                    })
                });

                stackLayout.Children.Add(label);
            }

            return new ScrollView
            {
                Content = stackLayout,
                HeightRequest = 150,
                VerticalScrollBarVisibility = ScrollBarVisibility.Never // Hide scrollbars
            };
        }

        private void UpdateSelectedTime(int hours, int minutes, int seconds)
        {
            _selectedTime = new TimeSpan(hours, minutes, seconds);
            _timeEntry.Text = _selectedTime.ToString(@"hh\:mm\:ss");
            TimeSelected?.Invoke(this, _selectedTime);
        }

        private void CloseTimePickerPopup()
        {
            if (_popupOverlay != null && Parent is Layout parentLayout)
            {
                parentLayout.Children.Remove(_popupOverlay);
                _popupOverlay = null;
            }
        }

        private void OnTimeEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TimeSpan.TryParse(e.NewTextValue, out TimeSpan newTime))
            {
                _selectedTime = newTime;
                TimeSelected?.Invoke(this, newTime);
            }
        }
    }
}
