﻿using System;
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
    public class CustomTimePicker : ContentView
    {
        private TimeSpan _selectedTime = DateTime.Now.TimeOfDay;
        private Entry _timeEntry;
        private Button _dropdownButton;
        private AbsoluteLayout _absoluteLayout;
        private TimePickerPopup _popup;

        public event EventHandler<TimeSpan> TimeSelected;

        public static readonly BindableProperty SelectedTimeProperty = BindableProperty.Create(
            nameof(SelectedTime),typeof(TimeSpan),typeof(CustomTimePicker), DateTime.Now.TimeOfDay, propertyChanged: OnSelectedTimeChanged);

        public TimeSpan SelectedTime
        {
            get => (TimeSpan)GetValue(SelectedTimeProperty);
            set => SetValue(SelectedTimeProperty, value);
        }

        public static readonly BindableProperty CustomWidthProperty = BindableProperty.Create(
            nameof(CustomWidth),typeof(double),typeof(CustomTimePicker),120.0, propertyChanged: OnSizePropertyChanged);

        public double CustomWidth
        {
            get => (double)GetValue(CustomWidthProperty);
            set => SetValue(CustomWidthProperty, value);
        }

        public static readonly BindableProperty CustomHeightProperty = BindableProperty.Create(
            nameof(CustomHeight),typeof(double),typeof(CustomTimePicker),45.0, propertyChanged: OnSizePropertyChanged);

        public double CustomHeight
        {
            get => (double)GetValue(CustomHeightProperty);
            set => SetValue(CustomHeightProperty, value);
        }

        public static readonly BindableProperty TimeFormatProperty = BindableProperty.Create(
            nameof(TimeFormat), typeof(string), typeof(CustomTimePicker), "HH:mm:ss", propertyChanged: OnTimeFormatChanged);

        public string TimeFormat
        {
            get => (string)GetValue(TimeFormatProperty);
            set => SetValue(TimeFormatProperty, value);
        }

        private static void OnSelectedTimeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomTimePicker timePicker && newValue is TimeSpan newTime)
            {
                timePicker._selectedTime = newTime;
                timePicker._timeEntry.Text = timePicker.FormatTime(newTime);
            }
        }

        private static void OnSizePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomTimePicker customTimePicker)
            {
                customTimePicker.UpdateLayout();
            }
        }

        private static void OnTimeFormatChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomTimePicker customTimePicker && newValue is string newFormat)
            {
                customTimePicker.UpdateTimeDisplayFormat(newFormat);
            }
        }

        public CustomTimePicker()
        {
            _timeEntry = new Entry
            {
                Text = FormatTime(_selectedTime),
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center
            };
            _timeEntry.TextChanged += OnTimeEntryTextChanged;

            _dropdownButton = new Button
            {
                Text = "▼",
                FontSize = 10,
                WidthRequest = 20,
                HeightRequest = 20,
                Padding = 0,
                BackgroundColor = Colors.Transparent,
                TextColor = Colors.Black,
                BorderWidth = 0
            };
            _dropdownButton.Clicked += (s, e) => ToggleTimePickerPopup();

            _absoluteLayout = new AbsoluteLayout();
            AbsoluteLayout.SetLayoutFlags(_timeEntry, AbsoluteLayoutFlags.All);
            AbsoluteLayout.SetLayoutBounds(_timeEntry, new Rect(0, 0, 1, 1));
            _absoluteLayout.Children.Add(_timeEntry);

            AbsoluteLayout.SetLayoutFlags(_dropdownButton, AbsoluteLayoutFlags.PositionProportional);
            AbsoluteLayout.SetLayoutBounds(_dropdownButton, new Rect(0.95, 0.5, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));
            _absoluteLayout.Children.Add(_dropdownButton);

            Content = _absoluteLayout;

            UpdateLayout();
        }

        private void UpdateLayout()
        {
            WidthRequest = CustomWidth;
            HeightRequest = CustomHeight;

            if (CustomWidth <= 0 || CustomHeight <= 0)
                return;

            double scaleFactor = Math.Min(CustomWidth / 150.0, CustomHeight / 40.0); // 150 и 40 - базовые значения по умолчанию

            // Масштабируем размеры элементов
            _timeEntry.WidthRequest = CustomWidth - (20 * scaleFactor);
            _timeEntry.HeightRequest = CustomHeight;
            _timeEntry.FontSize = 14 * scaleFactor;

            _dropdownButton.WidthRequest = 20 * scaleFactor;
            _dropdownButton.HeightRequest = 20 * scaleFactor;
            _dropdownButton.FontSize = 10 * scaleFactor;
        }

        private async void ToggleTimePickerPopup()
        {
            if (_popup == null)
            {
                _popup = new TimePickerPopup(_selectedTime, TimeFormat);
                _popup.TimeSelected += OnPopupTimeSelected;
                _popup.Closed += OnPopupClosed;

                var currentPage = GetCurrentPage();
                await currentPage?.ShowPopupAsync(_popup);
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
            _timeEntry.Text = FormatTime(_selectedTime);
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
            if (TimeSpan.TryParseExact(e.NewTextValue, TimeFormat, CultureInfo.InvariantCulture, out TimeSpan newTime))
            {
                _selectedTime = newTime;
            }
        }

        private void UpdateTimeDisplayFormat(string format)
        {
            _timeEntry.Text = FormatTime(_selectedTime);
            _popup?.Close();
            _popup = new TimePickerPopup(_selectedTime, TimeFormat);
        }

        private string FormatTime(TimeSpan time)
        {
            var dateTime = new DateTime(time.Ticks);
            return dateTime.ToString(TimeFormat);
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
