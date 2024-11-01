using Custom.MAUI.Calendar.Views;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Custom.MAUI.Calendar
{
    public enum CalendarDisplayMode
    {
        Default,
        SeparateMonthYear,
        SeparateMonthFixedYear
    }

    public enum CalendarViewMode
    {
        Days,
        Months,
        Years
    }

    public class CustomCalendar : ContentView
    {
        private HeaderView _headerView;
        private DaysGridView _daysGridView;

        private Grid _mainGrid;
        private ContentView _overlayView;

        private CalendarViewMode _currentViewMode = CalendarViewMode.Days;

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
            BindableProperty.Create(nameof(DisplayMode), typeof(CalendarDisplayMode), typeof(CustomCalendar), CalendarDisplayMode.Default, propertyChanged: OnDisplayModeChanged);

        public CalendarDisplayMode DisplayMode
        {
            get => (CalendarDisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public static readonly BindableProperty CultureProperty =
            BindableProperty.Create(nameof(Culture), typeof(CultureInfo), typeof(CustomCalendar), CultureInfo.CurrentCulture, propertyChanged: OnCultureChanged);

        public CultureInfo Culture
        {
            get => (CultureInfo)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
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
                CurrentDate = _currentDate,
                Culture = Culture
            };
            _headerView.PreviousMonthClicked += OnPreviousMonthClicked;
            _headerView.NextMonthClicked += OnNextMonthClicked;
            _headerView.PreviousYearClicked += OnPreviousYearClicked;
            _headerView.NextYearClicked += OnNextYearClicked;
            _headerView.MonthLabelTapped += OnMonthLabelTapped;
            _headerView.YearLabelTapped += OnYearLabelTapped;
            _headerView.MonthYearLabelTapped += OnMonthYearLabelTapped;

            _daysGridView = new DaysGridView
            {
                CurrentDate = _currentDate,
                SelectedDates = SelectedDates,
                Culture = Culture
            };
            _daysGridView.DaySelected += OnDaySelected;
            _daysGridView.MonthSelected += OnMonthSelected;
            _daysGridView.YearSelected += OnYearSelected;

            var mainLayout = new StackLayout
            {
                BackgroundColor = CalendarBackgroundColor,
                Children = { _headerView, _daysGridView }
            };

            _mainGrid = new Grid();
            _mainGrid.Children.Add(mainLayout);

            Content = _mainGrid;
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

        private static void OnCultureChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                calendar._headerView.Culture = calendar.Culture;
                calendar._daysGridView.Culture = calendar.Culture;
                calendar.UpdateCalendar();
            }
        }

        private void OnPreviousMonthClicked(object sender, EventArgs e) => ChangeDate(months: -1);
        private void OnNextMonthClicked(object sender, EventArgs e) => ChangeDate(months: 1);
        private void OnPreviousYearClicked(object sender, EventArgs e) => ChangeDate(years: -1);
        private void OnNextYearClicked(object sender, EventArgs e) => ChangeDate(years: 1);

        private void ChangeDate(int months = 0, int years = 0)
        {
            var newDate = _currentDate.AddMonths(months).AddYears(years);
            if (newDate >= MinDate && newDate <= MaxDate)
            {
                _currentDate = newDate;
                UpdateCalendar();
            }
        }

        private void OnDaySelected(object sender, DateTime selectedDate)
        {
            UpdateSelectedDates(selectedDate);
            DateSelected?.Invoke(this, selectedDate);
        }

        private void OnMonthLabelTapped(object sender, EventArgs e)
        {
            _currentViewMode = CalendarViewMode.Months;
            UpdateCalendar();
        }

        private void OnYearLabelTapped(object sender, EventArgs e)
        {
            _currentViewMode = CalendarViewMode.Years;
            UpdateCalendar();
        }

        private void OnMonthYearLabelTapped(object sender, EventArgs e)
        {
            _currentViewMode = CalendarViewMode.Months; // или CalendarViewMode.Years
            UpdateCalendar();
        }

        private void ShowMonthPicker()
        {
            var monthPicker = new MonthPickerView { Culture = Culture };
            monthPicker.MonthSelected += OnMonthSelected;

            ShowOverlay(monthPicker);
        }

        private void ShowYearPicker()
        {
            var yearPicker = new YearPickerView
            {
                StartYear = MinDate.Year,
                EndYear = MaxDate.Year
            };
            yearPicker.YearSelected += OnYearSelected;

            ShowOverlay(yearPicker);
        }

        private void OnMonthSelected(object sender, int month)
        {
            _currentDate = new DateTime(_currentDate.Year, month, 1);
            _currentViewMode = CalendarViewMode.Days;
            UpdateCalendar();
        }

        private void OnYearSelected(object sender, int year)
        {
            _currentDate = new DateTime(year, _currentDate.Month, 1);
            _currentViewMode = CalendarViewMode.Days;
            UpdateCalendar();
        }

        private void UpdateSelectedDates(DateTime selectedDate)
        {
            if (SelectedDates.Contains(selectedDate))
            {
                SelectedDates.Clear();
                SelectedDate = selectedDate;
            }
            else
            {
                SelectedDates.Add(selectedDate);
                if (SelectedDates.Count > 1)
                {
                    SelectedDates = SelectedDates.OrderBy(d => d).ToList();
                    DateRangeSelected?.Invoke(this, (SelectedDates[0], SelectedDates[1]));
                }
            }

            SelectedDate = SelectedDates.Count == 1 ? SelectedDates[0] : (DateTime?)null;

            _daysGridView.SelectedDates = SelectedDates;
            _daysGridView.BuildGrid();
        }

        private async void UpdateCalendar()
        {
            await _daysGridView.FadeTo(0, 200);
            _headerView.CurrentDate = _currentDate;
            _daysGridView.CurrentDate = _currentDate;
            _daysGridView.SelectedDates = SelectedDates;
            _daysGridView.ViewMode = _currentViewMode;
            _daysGridView.MinDate = MinDate;
            _daysGridView.MaxDate = MaxDate;
            _daysGridView.BuildGrid();
            await _daysGridView.FadeTo(1, 200);
        }

        private void ShowOverlay(View overlayContent)
        {
            _overlayView = new ContentView
            {
                BackgroundColor = new Color(0, 0, 0, (float)0.5),
                Content = overlayContent
            };
            _overlayView.GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command(HideOverlay)
            });

            _mainGrid.Children.Add(_overlayView);
        }

        private void HideOverlay()
        {
            if (_overlayView != null && _mainGrid.Children.Contains(_overlayView))
            {
                _mainGrid.Children.Remove(_overlayView);
                _overlayView = null;
            }
        }
    }
}
