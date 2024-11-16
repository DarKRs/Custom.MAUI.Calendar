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
                VerticalOptions = LayoutOptions.Center,
                Placeholder = TimeFormat,
                Keyboard = Keyboard.Numeric
            };
            _timeEntry.TextChanged += OnTimeEntryTextChanged;
            _timeEntry.Unfocused += OnTimeEntryCompleted;
            _timeEntry.Completed += OnTimeEntryCompleted;

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
            if (string.IsNullOrWhiteSpace(e.NewTextValue))
                return;

            var entry = (Entry)sender;
            string input = e.NewTextValue;

            int originalCursorPosition = entry.CursorPosition;

            // Форматируем ввод
            input = FormatTimeInput(input, e.OldTextValue);

            // Устанавливаем текст и позицию курсора, только если текст изменился
            if (entry.Text != input)
            {
                entry.Text = input;

                if (originalCursorPosition < input.Length)
                    entry.CursorPosition = originalCursorPosition;
                else
                    entry.CursorPosition = input.Length; 
            }
        }

        private void OnTimeEntryCompleted(object sender, EventArgs e)
        {
            string input = _timeEntry.Text.Trim();

            // Пробуем обработать "неполный" ввод
            input = FormatPartialInput(input);

            //Для TimeSpan нужен формат вида hh\\:mm
            string TimeSpanFormat = TimeFormat.ToLower().Replace(":", "\\:");
            if (TimeSpan.TryParseExact(input, TimeSpanFormat, CultureInfo.InvariantCulture, out TimeSpan parsedTime))
            {

                _selectedTime = parsedTime;
                _timeEntry.Text = FormatTime(_selectedTime);
            }
            else
            {
                // Если формат некорректен, возвращаем последнее корректное значение
                _timeEntry.Text = FormatTime(_selectedTime);
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

        private string FormatTimeInput(string input, string previousInput)
        {
            // Ограничение длины текста в зависимости от формата (например, HH:mm:ss)
            int maxLength = TimeFormat.Replace(":", "").Length + (TimeFormat.Contains(":") ? TimeFormat.Count(c => c == ':') : 0);
            if (input.Length > maxLength)
                input = input.Substring(0, maxLength);

            // Проверка на допустимые символы (цифры и символ ":")
            if (!System.Text.RegularExpressions.Regex.IsMatch(input, @"^\d{0,2}:?\d{0,2}:?\d{0,2}$"))
                return previousInput;

            // Автоматическое добавление ведущего нуля, если пользователь завершил ввод часов
            if (TimeFormat.Contains("HH") && input.Length == 2 && !input.Contains(":"))
            {
                if (int.TryParse(input, out int hour) && hour >= 0 && hour <= 9)
                {
                    input = $"0{hour}:";
                }
            }

            // Автоматическое добавление двоеточия, если это необходимо по текущему формату
            if (TimeFormat.Contains("HH") && input.Length == 2 && !input.Contains(":"))
                input += ":";

            if (TimeFormat.Contains("mm") && input.Length == 5 && TimeFormat.Contains("ss"))
                input += ":";

            // Разделяем на компоненты времени и корректируем
            string[] parts = input.Split(':');
            if (TimeFormat.Contains("HH") && parts.Length >= 1)
                input = CorrectHours(parts);
            if (TimeFormat.Contains("mm") && parts.Length >= 2)
                input = CorrectMinutes(parts, input);
            if (TimeFormat.Contains("ss") && parts.Length == 3)
                input = CorrectSeconds(parts, input);

            return input;
        }

        private string FormatPartialInput(string input)
        {
            string[] parts = input.Split(':');
            if (parts.Length == 2)
            {
                if (parts[0].Length == 1)
                {
                    parts[0] = "0" + parts[0];
                }

                if (parts[1].Length == 1)
                {
                    parts[1] = "0" + parts[1];
                }

                input = string.Join(":", parts);
            }
            else if (parts.Length == 1 && parts[0].Length > 0)
            {
                // Если только часы введены (например, "4"), добавляем ":00" для минут
                input = parts[0].PadLeft(2, '0') + ":00";
            }

            return input;
        }

        private string CorrectHours(string[] parts)
        {
            if (int.TryParse(parts[0], out int hours) && hours > 23)
                return "23" + (parts.Length > 1 ? ":" + parts[1] : "");
            return string.Join(":", parts);
        }

        private string CorrectMinutes(string[] parts, string input)
        {
            if (parts.Length < 2)
                return input;

            if (parts[1].Length == 1 && int.TryParse(parts[1], out int firstMinuteDigit) && firstMinuteDigit > 5)
                return parts[0] + ":59";

            if (int.TryParse(parts[1], out int minutes) && minutes > 59)
                return parts[0] + ":59";

            return input;
        }

        private string CorrectSeconds(string[] parts, string input)
        {
            if (parts.Length < 3)
                return input;

            if (parts[2].Length == 1 && int.TryParse(parts[2], out int firstSecondDigit) && firstSecondDigit > 5)
                return parts[0] + ":" + parts[1] + ":59";

            if (int.TryParse(parts[2], out int seconds) && seconds > 59)
                return parts[0] + ":" + parts[1] + ":59";

            return input;
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
