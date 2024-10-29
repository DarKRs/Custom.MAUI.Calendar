using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Custom.MAUI.Calendar
{
    public class CustomCalendar : ContentView
    {
        // Сетка для отображения календаря
        private Grid _calendarGrid;
        private Label _monthYearLabel;
        private DateTime _currentDate;
        // Верхняя панель, содержащая кнопки навигации и метку месяца/года
        private StackLayout _headerLayout;
        private Button _previousMonthButton;
        private Button _nextMonthButton;
        public DateTime MinDate { get; set; } = new DateTime(1900, 1, 1);
        public DateTime MaxDate { get; set; } = new DateTime(2100, 12, 31);
        private List<DateTime> _selectedDates = new();

        // Событие, вызываемое при выборе даты
        public event EventHandler<DateTime> DateSelected;
        // Событие, вызываемое при выборе диапазона дат
        public event EventHandler<(DateTime, DateTime)> DateRangeSelected;

        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate),typeof(DateTime?),typeof(CustomCalendar),null,BindingMode.TwoWay,propertyChanged: OnSelectedDateChanged);

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly BindableProperty CalendarBackgroundColorProperty =
            BindableProperty.Create(nameof(CalendarBackgroundColor),typeof(Color),typeof(CustomCalendar),Colors.White,BindingMode.TwoWay,propertyChanged: OnCalendarBackgroundColorChanged);

        // Свойство для доступа к цвету фона
        public Color CalendarBackgroundColor
        {
            get => (Color)GetValue(CalendarBackgroundColorProperty);
            set => SetValue(CalendarBackgroundColorProperty, value);
        }

        public CustomCalendar()
        {
            _currentDate = DateTime.Today;
            InitializeCalendar();
            CreateCalendarView();
        }

        private static void OnSelectedDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar && newValue is DateTime newDate)
            {
                calendar.UpdateSelectedDates(newDate);
                calendar.CreateCalendarView();
            }
        }

        private static void OnCalendarBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar && newValue is Color newColor)
            {
                if (calendar.Content is Layout contentLayout)
                {
                    contentLayout.BackgroundColor = newColor;
                }
            }
        }

        private void InitializeCalendar()
        {
            // Верняя панель месяц/год
            _headerLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Start, Margin = new Thickness(10) };

            _previousMonthButton = new Button { Text = "<", WidthRequest = 50 };
            _previousMonthButton.Clicked += OnPreviousMonthButtonClicked;

            _monthYearLabel = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FontSize = 20 };

            _nextMonthButton = new Button { Text = ">", WidthRequest = 50 };
            _nextMonthButton.Clicked += OnNextMonthButtonClicked;

            _headerLayout.Children.Add(_previousMonthButton);
            _headerLayout.Children.Add(_monthYearLabel);
            _headerLayout.Children.Add(_nextMonthButton);

            // Сетка для отображения дней календаря
            _calendarGrid = new Grid { RowSpacing = 5, ColumnSpacing = 5, VerticalOptions = LayoutOptions.Fill, HorizontalOptions = LayoutOptions.Fill };
            for (int i = 0; i < 7; i++)
                _calendarGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            for (int i = 0; i < 6; i++)
                _calendarGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            Content = new StackLayout
            {
                BackgroundColor = CalendarBackgroundColor,
                Children = { _headerLayout, _calendarGrid }
            };
        }

        //Создание представления календаря
        private void CreateCalendarView()
        {
            _calendarGrid.Children.Clear();
            _monthYearLabel.Text = _currentDate.ToString("MMMM yyyy", new CultureInfo("ru-RU"));
            AddDaysOfWeekLabels();

            // Определяем первый день месяца и позицию его в сетке
            var firstDayOfMonth = new DateTime(_currentDate.Year, _currentDate.Month, 1);
            var startDay = (int)firstDayOfMonth.DayOfWeek;
            if (startDay == 0) startDay = 7;
            startDay -= 1;

            // Добавляем кнопки для каждого дня месяца
            var daysInMonth = DateTime.DaysInMonth(_currentDate.Year, _currentDate.Month);
            int row = 1, column = startDay;
            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDay = new DateTime(_currentDate.Year, _currentDate.Month, day);
                var dayButton = new Button
                {
                    Text = day.ToString(),
                    BindingContext = currentDay,
                    BackgroundColor = Colors.Transparent
                };
                dayButton.Clicked += OnDayButtonClicked;

                // Выделяем сегодняшний день
                if (DateTime.Today.Day == day && DateTime.Today.Month == _currentDate.Month && DateTime.Today.Year == _currentDate.Year)
                {
                    dayButton.BackgroundColor = Colors.LightBlue;
                }

                if (_selectedDates.Contains(currentDay))
                {
                    dayButton.BackgroundColor = Colors.LightGreen;
                }

                if (_selectedDates.Count == 2 && currentDay >= _selectedDates[0] && currentDay <= _selectedDates[1])
                {
                    dayButton.BackgroundColor = Colors.LightYellow;
                }

                Grid.SetColumn(dayButton, column);
                Grid.SetRow(dayButton, row);
                _calendarGrid.Children.Add(dayButton);

                column++;
                if (column == 7)
                {
                    column = 0;
                    row++;
                }
            }
        }

        private void AddDaysOfWeekLabels()
        {
            string[] daysOfWeek = { "Пн", "Вт", "Ср", "Чт", "Пт", "Сб", "Вс" };
            for (int i = 0; i < 7; i++)
            {
                var dayLabel = new Label
                {
                    Text = daysOfWeek[i],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold
                };
                Grid.SetColumn(dayLabel, i);
                Grid.SetRow(dayLabel, 0);
                _calendarGrid.Children.Add(dayLabel);
            }
        }

        private void OnPreviousMonthButtonClicked(object sender, EventArgs e)
        {
            if (_currentDate.AddMonths(-1) >= MinDate)
            {
                _currentDate = _currentDate.AddMonths(-1);
                CreateCalendarView();
            }
        }

        private void OnNextMonthButtonClicked(object sender, EventArgs e)
        {
            if (_currentDate.AddMonths(1) <= MaxDate)
            {
                _currentDate = _currentDate.AddMonths(1);
                CreateCalendarView();
            }
        }

        private void OnDayButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is DateTime selectedDate)
            {
                UpdateSelectedDates(selectedDate);
                CreateCalendarView();

            }
        }

        private void UpdateSelectedDates(DateTime selectedDate)
        {
            if (_selectedDates.Count == 0)
            {
                _selectedDates.Add(selectedDate);
                SelectedDate = selectedDate;
                DateSelected?.Invoke(this, selectedDate);
            }
            else if (_selectedDates.Count == 1)
            {
                if (_selectedDates[0] != selectedDate)
                {
                    _selectedDates.Add(selectedDate);
                    _selectedDates = _selectedDates.OrderBy(d => d).ToList();
                    DateRangeSelected?.Invoke(this, (_selectedDates[0], _selectedDates[1]));
                }
            }
            else
            {
                _selectedDates.Clear();
                _selectedDates.Add(selectedDate);
                SelectedDate = selectedDate;
                DateSelected?.Invoke(this, selectedDate);
            }
        }
    }
}
