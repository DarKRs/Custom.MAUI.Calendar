using Custom.MAUI.Components.Styles;
using Custom.MAUI.Components.Views;
using System.Globalization;

namespace Custom.MAUI.Components
{
    public class DayTappedEventArgs : EventArgs
    {
        public DateTime Date { get; set; }
        public VisualElement VisualElement { get; set; }
    }
    public class DateRangeTappedEventArgs : EventArgs
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<VisualElement> VisualElements { get; set; }
    }

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

    public class CustomCalendar : ContentView, IDisposable
    {
        private HeaderView _headerView;
        private DaysGridView _daysGridView;

        private CalendarViewMode _currentViewMode = CalendarViewMode.Days;
        private DateTime _currentDate;
        public DateTime MinDate { get; set; } = new DateTime(1900, 1, 1);
        public DateTime MaxDate { get; set; } = new DateTime(2100, 12, 31);

        private DateTime? _selectedDate;
        private List<DateTime> _selectedDates = new List<DateTime>();

        public event EventHandler<DayTappedEventArgs> DateSelected;
        public event EventHandler<DateRangeTappedEventArgs> DateRangeSelected;
        public event EventHandler<DateTime> MonthChanged;
        public event EventHandler<DateTime> YearChanged;
        public event EventHandler<CalendarDisplayMode> ViewModeChanged;
        public event EventHandler<DateTime> DateDeselected;
        public event EventHandler DateRangeCleared;
        public event EventHandler<CultureInfo> CultureChanged;
        public event EventHandler<CalendarStyle> StyleChanged;

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

        public static readonly BindableProperty StyleProperty =
            BindableProperty.Create(nameof(Style), typeof(CalendarStyle), typeof(CustomCalendar), new CalendarStyle(), propertyChanged: OnStyleChanged);

        public CalendarStyle Style
        {
            get => (CalendarStyle)GetValue(StyleProperty);
            set => SetValue(StyleProperty, value);
        }

        public CustomCalendar()
        {
            _currentDate = DateTime.Today;

            _headerView = new HeaderView
            {
                DisplayMode = DisplayMode,
                CurrentDate = _currentDate,
                Culture = Culture,
                Style = Style
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
                SelectedDates = _selectedDates,
                Culture = Culture,
                Style = Style,
                MinDate = MinDate,
                MaxDate = MaxDate
            };
            _daysGridView.DaySelected += OnDaySelected;
            _daysGridView.MonthSelected += OnMonthSelected;
            _daysGridView.YearSelected += OnYearSelected;

            var mainLayout = new StackLayout
            {
                BackgroundColor = Style?.BackgroundColor,
                Children = { _headerView, _daysGridView }
            };

            Content = mainLayout;
        }

        private static void OnDisplayModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                calendar._headerView.DisplayMode = calendar.DisplayMode;
                calendar.UpdateCalendar();
                calendar.ViewModeChanged?.Invoke(calendar, calendar._headerView.DisplayMode);
            }
        }

        private static void OnCultureChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                calendar._headerView.Culture = calendar.Culture;
                calendar._daysGridView.Culture = calendar.Culture;
                calendar.UpdateCalendar();
                calendar.CultureChanged?.Invoke(calendar, calendar.Culture);
            }
        }

        private static void OnStyleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is CustomCalendar calendar)
            {
                calendar.ApplyStyles();
                calendar.StyleChanged?.Invoke(calendar, calendar.Style);
            }
        }

        private void ApplyStyles()
        {
            _headerView.Style = Style;
            _daysGridView.Style = Style;
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
                if (months != 0)
                {
                    MonthChanged?.Invoke(this, _currentDate);
                }

                if (years != 0)
                {
                    YearChanged?.Invoke(this, _currentDate);
                }
            }
        }

        private void OnDaySelected(object sender, DayTappedEventArgs e)
        {
            UpdateSelectedDates(e.Date);
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
            _currentViewMode = CalendarViewMode.Months;
            UpdateCalendar();
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
            if (_selectedDates.Contains(selectedDate))
            {
                _selectedDates.Remove(selectedDate);

                DateDeselected?.Invoke(this, selectedDate);

                if (_selectedDates.Count == 0)
                {
                    DateRangeCleared?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                // Если уже выбран диапазон (две даты), сбрасываем выбор.
                if (_selectedDates.Count == 2)
                {
                    _selectedDates.Clear();
                    DateRangeCleared?.Invoke(this, EventArgs.Empty);
                }

                _selectedDates.Add(selectedDate);


                if (_selectedDates.Count == 2)
                {
                    _selectedDates = _selectedDates.OrderBy(d => d).ToList();
                    var datesInRange = GetDatesInRange(_selectedDates[0], _selectedDates[1]);


                    var visualElements = new List<VisualElement>();
                    foreach (var date in datesInRange)
                    {
                        if (_daysGridView.TryGetDayButton(date, out var button))
                        {
                            visualElements.Add(button);
                        }
                    }


                    var eventArgs = new DateRangeTappedEventArgs
                    {
                        StartDate = _selectedDates[0],
                        EndDate = _selectedDates[1],
                        VisualElements = visualElements
                    };
                    DateRangeSelected?.Invoke(this, eventArgs);
                }

                if (_selectedDates.Count == 1)
                {
                    DateSelected?.Invoke(this, new DayTappedEventArgs { Date = selectedDate });
                }

            }

            _selectedDate = _selectedDates.Count == 1 ? _selectedDates[0] : (DateTime?)null;

            _daysGridView.SelectedDates = _selectedDates;
            _daysGridView.BuildGrid();
        }

        private async void UpdateCalendar()
        {
            await _daysGridView.FadeTo(0, 200);
            _headerView.CurrentDate = _currentDate;
            _daysGridView.CurrentDate = _currentDate;
            _daysGridView.ViewMode = _currentViewMode;
            _daysGridView.BuildGrid();
            await _daysGridView.FadeTo(1, 200);
        }

        private IEnumerable<DateTime> GetDatesInRange(DateTime start, DateTime end)
        {
            for (var date = start.Date; date <= end.Date; date = date.AddDays(1))
            {
                yield return date;
            }
        }


        public void Dispose()
        {
            // Отсоединяем обработчики событий
            _headerView.PreviousMonthClicked -= OnPreviousMonthClicked;
            _headerView.NextMonthClicked -= OnNextMonthClicked;
            _headerView.PreviousYearClicked -= OnPreviousYearClicked;
            _headerView.NextYearClicked -= OnNextYearClicked;
            _headerView.MonthLabelTapped -= OnMonthLabelTapped;
            _headerView.YearLabelTapped -= OnYearLabelTapped;
            _headerView.MonthYearLabelTapped -= OnMonthYearLabelTapped;

            _daysGridView.DaySelected -= OnDaySelected;
            _daysGridView.MonthSelected -= OnMonthSelected;
            _daysGridView.YearSelected -= OnYearSelected;

            _headerView.Dispose();
            _daysGridView.Dispose();
        }
    }

}
