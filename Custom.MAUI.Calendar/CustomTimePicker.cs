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
            _timeEntry.Focused += OnTimeEntryFocused;

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

            _dropdownButton.Clicked += OnDropdownButtonClicked;

            var inputLayout = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = GridLength.Auto }
            }
            };

            inputLayout.Children.Add(_timeEntry);
            Grid.SetColumn(_timeEntry, 0);

            inputLayout.Children.Add(_dropdownButton);
            Grid.SetColumn(_dropdownButton, 1);

            this.Children.Add(inputLayout);
        }

        private void OnDropdownButtonClicked(object sender, EventArgs e)
        {
            ShowTimePickerPopup();
        }

        private void OnTimeEntryFocused(object sender, FocusEventArgs e)
        {
            ShowTimePickerPopup();
        }

        private void ShowTimePickerPopup()
        {
            if (_popup != null)
            {
                return;
            }

            _popup = new TimePickerPopup(_selectedTime);
            _popup.TimeSelected += OnPopupTimeSelected;
            _popup.Closed += OnPopupClosed;

            // Получаем текущую страницу для отображения всплывающего окна
            var currentPage = GetCurrentPage();
            currentPage?.ShowPopup(_popup);
        }

        private void OnPopupTimeSelected(object sender, TimeSpan e)
        {
            _selectedTime = e;
            _timeEntry.Text = _selectedTime.ToString(@"hh\:mm\:ss");
            TimeSelected?.Invoke(this, _selectedTime);
        }

        private void OnPopupClosed(object sender, PopupClosedEventArgs e)
        {
            // Освобождаем ресурсы
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
                TimeSelected?.Invoke(this, newTime);
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
