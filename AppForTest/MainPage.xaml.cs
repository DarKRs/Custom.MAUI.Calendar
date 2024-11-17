using Custom.MAUI.Components;
using System.Diagnostics;
using System.Globalization;
using Custom.MAUI.Components.Styles;

namespace AppForTest
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            //Calendar.Culture = CultureInfo.GetCultureInfo("fr-FR");

            Calendar.DateSelected += OnDateSelected;
            Calendar.DateDeselected += Calendar_DateDeselected;
            Calendar.DateRangeSelected += OnDateRangeSelected;
            Calendar.DateRangeCleared += Calendar_DateRangeCleared;
        }

        private void Calendar_DateRangeCleared(object? sender, EventArgs e)
        {
            Debug.WriteLine($"DateRangeCleard {e}");
        }

        private void Calendar_DateDeselected(object? sender, DateTime e)
        {
            Debug.WriteLine($"DateDeSelected {e}");
        }

        private void OnDateSelected(object sender, DayTappedEventArgs selectedDate)
        {
            Debug.WriteLine($"DateSelected {selectedDate}");
            // Обновляем Label с выбранной датой
            SelectedDateLabel.Text = $"Вы выбрали дату: {selectedDate:d}";
        }

        private void OnDateRangeSelected(object sender, DateRangeTappedEventArgs dateRange)
        {
            Debug.WriteLine($"DateRangeSelected {dateRange}");
            // Обновляем Label с выбранным диапазоном дат
            DateRangeLabel.Text = $"Вы выбрали диапазон: {dateRange.StartDate} - {dateRange.EndDate}";
        }

        private void OnTimeSelected(object sender, TimeSpan time)
        {
            Debug.WriteLine($"Выбрано время: {time}");
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}