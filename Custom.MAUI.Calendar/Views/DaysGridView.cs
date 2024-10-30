using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Views
{
    public class DaysGridView : Grid
    {
        public event EventHandler<DateTime> DaySelected;

        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(DaysGridView), DateTime.Today, propertyChanged: OnCurrentDateChanged);

        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        public static readonly BindableProperty SelectedDatesProperty =
            BindableProperty.Create(nameof(SelectedDates), typeof(List<DateTime>), typeof(DaysGridView), new List<DateTime>(), propertyChanged: OnSelectedDatesChanged);

        public List<DateTime> SelectedDates
        {
            get => (List<DateTime>)GetValue(SelectedDatesProperty);
            set => SetValue(SelectedDatesProperty, value);
        }

        public DaysGridView()
        {
            // Инициализация сетки
            RowSpacing = 5;
            ColumnSpacing = 5;
            VerticalOptions = LayoutOptions.Fill;
            HorizontalOptions = LayoutOptions.Fill;

            for (int i = 0; i < 7; i++)
                ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            for (int i = 0; i < 7; i++)
                RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });

            BuildDaysGrid();
        }

        private static void OnCurrentDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DaysGridView daysGrid)
            {
                daysGrid.BuildDaysGrid();
            }
        }

        private static void OnSelectedDatesChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is DaysGridView daysGrid)
            {
                daysGrid.BuildDaysGrid();
            }
        }

        public void BuildDaysGrid()
        {
            Children.Clear();
            AddDaysOfWeekLabels();

            // Определяем первый день месяца и его позицию
            var firstDayOfMonth = new DateTime(CurrentDate.Year, CurrentDate.Month, 1);
            int startDay = ((int)firstDayOfMonth.DayOfWeek + 6) % 7;

            int daysInMonth = DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month);

            int row = 1;
            int column = startDay;

            for (int i = 0; i < startDay; i++)
            {
                var emptyLabel = new Label();
                Grid.SetRow(emptyLabel, row);
                Grid.SetColumn(emptyLabel, i);
                Children.Add(emptyLabel);
            }

            // Добавляем кнопки дней месяца
            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDay = new DateTime(CurrentDate.Year, CurrentDate.Month, day);

                var dayButton = new Button
                {
                    Text = day.ToString(),
                    BindingContext = currentDay,
                    BackgroundColor = Colors.Transparent
                };

                dayButton.Clicked += (s, e) => DaySelected?.Invoke(this, currentDay);

                // Выделение сегодняшнего дня
                if (currentDay.Date == DateTime.Today)
                {
                    dayButton.BackgroundColor = Colors.Blue;
                }

                // Выделение выбранных дат
                if (SelectedDates != null && SelectedDates.Count > 0)
                {
                    if (SelectedDates.Count == 1)
                    {
                        if (currentDay.Date == SelectedDates[0].Date)
                        {
                            dayButton.BackgroundColor = Colors.LightGreen;
                        }
                    }
                    else if (SelectedDates.Count == 2)
                    {
                        if (currentDay.Date >= SelectedDates[0].Date && currentDay.Date <= SelectedDates[1].Date)
                        {
                            dayButton.BackgroundColor = Colors.LightGreen;
                        }
                    }
                }

                Grid.SetRow(dayButton, row);
                Grid.SetColumn(dayButton, column);
                Children.Add(dayButton);

                column++;
                if (column > 6)
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
                Children.Add(dayLabel);
            }
        }
    }

}
