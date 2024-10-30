using Custom.MAUI.Calendar.Views;
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
        private HeaderView _headerView;
        private DaysGridView _daysGridView;

        public static readonly BindableProperty SelectedDateProperty =
            BindableProperty.Create(nameof(SelectedDate), typeof(DateTime?), typeof(CustomCalendar), null, BindingMode.TwoWay, propertyChanged: OnSelectedDateChanged);

        public DateTime? SelectedDate
        {
            get => (DateTime?)GetValue(SelectedDateProperty);
            set => SetValue(SelectedDateProperty, value);
        }

        public static readonly BindableProperty SelectedDatesProperty =
            BindableProperty.Create(nameof(SelectedDates), typeof(List<DateTime>), typeof(CustomCalendar), new List<DateTime>(), BindingMode.TwoWay, propertyChanged: OnSelectedDatesChanged);

        public List<DateTime> SelectedDates
        {
            get => (List<DateTime>)GetValue(SelectedDatesProperty);
            set => SetValue(SelectedDatesProperty, value);
        }

        public static readonly BindableProperty CalendarBackgroundColorProperty =
            BindableProperty.Create(nameof(CalendarBackgroundColor), typeof(Color), typeof(CustomCalendar), Colors.White, propertyChanged: OnCalendarBackgroundColorChanged);

        public Color CalendarBackgroundColor
        {
            get => (Color)GetValue(CalendarBackgroundColorProperty);
            set => SetValue(CalendarBackgroundColorProperty, value);
        }

        public static readonly BindableProperty DisplayModeProperty =
            BindableProperty.Create(nameof(DisplayMode), typeof(string), typeof(CustomCalendar), "Default", propertyChanged: OnDisplayModeChanged);

        public string DisplayMode
        {
            get => (string)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public DateTime MinDate { get; set; } = new DateTime(1900, 1, 1);
        public DateTime MaxDate { get; set; } = new DateTime(2100, 12, 31);

        private DateTime _currentDate;

        public event EventHandler<DateTime> DateSelected;
        public event EventHandler<(DateTime, DateTime)> DateRangeSelected;

        public CustomCalendar()
        {
            _currentDate = DateTime.Today;

            _headerView = new HeaderView
            {
                DisplayMode = DisplayMode,
                CurrentDate = _currentDate
            };
            _headerView.PreviousMonthClicked += OnPreviousMonthClicked;
            _headerView.NextMonthClicked += OnNextMonthClicked;
            _headerView.PreviousYearClicked += OnPreviousYearClicked;
            _headerView.NextYearClicked += OnNextYearClicked;

            _daysGridView = new DaysGridView
            {
                CurrentDate = _currentDate,
                SelectedDates = SelectedDates
            };
            _daysGridView.DaySelected += OnDaySelected;

            var mainLayout = new StackLayout
            {
                BackgroundColor = CalendarBackgroundColor,
                Children = { _headerView, _daysGridView }
            };

            Content = mainLayout;
        }

        private static void OnSelectedDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            
        }

        private static void OnSelectedDatesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                calendar._daysGridView.SelectedDates = calendar.SelectedDates;
            }
        }

        private static void OnCalendarBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                if (calendar.Content is Layout contentLayout)
                {
                    contentLayout.BackgroundColor = calendar.CalendarBackgroundColor;
                }
            }
        }

        private static void OnDisplayModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                calendar._headerView.DisplayMode = calendar.DisplayMode;
            }
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e)
        {
            if (_currentDate.AddMonths(-1) >= MinDate)
            {
                _currentDate = _currentDate.AddMonths(-1);
                UpdateCalendar();
            }
        }

        private void OnNextMonthClicked(object sender, EventArgs e)
        {
            if (_currentDate.AddMonths(1) <= MaxDate)
            {
                _currentDate = _currentDate.AddMonths(1);
                UpdateCalendar();
            }
        }

        private void OnPreviousYearClicked(object sender, EventArgs e)
        {
            if (_currentDate.AddYears(-1) >= MinDate)
            {
                _currentDate = _currentDate.AddYears(-1);
                UpdateCalendar();
            }
        }

        private void OnNextYearClicked(object sender, EventArgs e)
        {
            if (_currentDate.AddYears(1) <= MaxDate)
            {
                _currentDate = _currentDate.AddYears(1);
                UpdateCalendar();
            }
        }

        private void OnDaySelected(object sender, DateTime selectedDate)
        {
            UpdateSelectedDates(selectedDate);
            DateSelected?.Invoke(this, selectedDate);
        }

        private void UpdateSelectedDates(DateTime selectedDate)
        {
            if (SelectedDates.Count == 0)
            {
                SelectedDates.Add(selectedDate);
                SelectedDate = selectedDate;
            }
            else if (SelectedDates.Count == 1)
            {
                if (SelectedDates[0] != selectedDate)
                {
                    SelectedDates.Add(selectedDate);
                    SelectedDates = SelectedDates.OrderBy(d => d).ToList();
                    SelectedDate = null; 
                    DateRangeSelected?.Invoke(this, (SelectedDates[0], SelectedDates[1]));
                }
                else
                {
                    SelectedDates.Clear();
                    SelectedDates.Add(selectedDate);
                    SelectedDate = selectedDate;
                }
            }
            else
            {
                SelectedDates.Clear();
                SelectedDates.Add(selectedDate);
                SelectedDate = selectedDate;
            }

            // Обновляем DaysGridView
            _daysGridView.SelectedDates = SelectedDates;
            _daysGridView.BuildDaysGrid();
        }

        private void UpdateCalendar()
        {
            _headerView.CurrentDate = _currentDate;
            _daysGridView.CurrentDate = _currentDate;
            _daysGridView.SelectedDates = SelectedDates;
        }
    }
}
