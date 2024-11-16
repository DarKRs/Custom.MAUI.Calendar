using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Layouts;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Maui.Core;
using Custom.MAUI.Calendar.Views;

namespace Custom.MAUI.Calendar
{
    public class CustomTimePicker : Grid
    {
        private TimeSpan _selectedTime;
        private Entry _timeEntry;
        private Button _dropdownButton;
        private TimePickerPopup _popup;

        public event EventHandler<TimeSpan> TimeSelected;

        public static readonly BindableProperty SelectedTimeProperty = BindableProperty.Create(
            nameof(SelectedTime),typeof(TimeSpan),typeof(CustomTimePicker),default(TimeSpan),propertyChanged: OnSelectedTimeChanged);

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
            WidthRequest = 120;
            HeightRequest = 60;

            _selectedTime = DateTime.Now.TimeOfDay;
            _timeEntry = new Entry
            {
                Text = _selectedTime.ToString(@"hh\:mm\:ss"),
                Keyboard = Keyboard.Text,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                Margin = new Thickness(0),
                BackgroundColor = Colors.Transparent 
            };
            _timeEntry.TextChanged += OnTimeEntryTextChanged;

            _dropdownButton = new Button
            {
                Text = "▼",
                FontSize = 10,
                WidthRequest = 5,
                HeightRequest = 20,
                Padding = 0,
                BackgroundColor = Colors.Transparent, 
                TextColor = Colors.Black,
                BorderWidth = 0,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };
            _dropdownButton.Clicked += (s, e) => ToggleTimePickerPopup();

            // Создаем AbsoluteLayout для наложения кнопки поверх Entry
            var absoluteLayout = new AbsoluteLayout
            {
                WidthRequest = 120,
                HeightRequest = 60
            };


            AbsoluteLayout.SetLayoutFlags(_timeEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(_timeEntry, new Rect(0, 0, 1, 1));
            absoluteLayout.Children.Add(_timeEntry);

            AbsoluteLayout.SetLayoutFlags(_dropdownButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(_dropdownButton, new Rect(1, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize)); // Сдвигаем кнопку к правому краю и центрируем по вертикали

            absoluteLayout.Children.Add(_dropdownButton);

            this.Children.Add(absoluteLayout);
        }





        private void ToggleTimePickerPopup()
        {
            if (_popup == null)
            {
                _popup = new TimePickerPopup(_selectedTime);
                _popup.TimeSelected += OnPopupTimeSelected;
                _popup.Closed += OnPopupClosed;

                var currentPage = GetCurrentPage();
                currentPage?.ShowPopup(_popup);
            }
            else
            {
                _popup.Close();
                _popup = null;
            }
        }

        private void OnPopupTimeSelected(object sender, TimeSpan e)
        {
            _selectedTime = e;
            _timeEntry.Text = _selectedTime.ToString(@"hh\:mm\:ss");
            TimeSelected?.Invoke(this, _selectedTime);
        }

        private void OnPopupClosed(object sender, PopupClosedEventArgs e)
        {
            if (_popup != null)
            {
                _popup.TimeSelected -= OnPopupTimeSelected;
                _popup.Closed -= OnPopupClosed;
                _popup = null;
            }
        }

        private void OnTimeEntryTextChanged(object sender, TextChangedEventArgs e)
        {
            if (TimeSpan.TryParse(e.NewTextValue, out TimeSpan newTime))
            {
                _selectedTime = newTime;
            }
        }

        private Page GetCurrentPage()
        {
            var element = this as Element;
            while (element != null)
            {
                if (element is Page page)
                {
                    return page;
                }
                element = element.Parent;
            }
            return null;
        }
    }

}
