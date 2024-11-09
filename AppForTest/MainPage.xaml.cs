using Custom.MAUI.Calendar;
using System.Globalization;

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
            Calendar.DateRangeSelected += OnDateRangeSelected;
        }

        private void OnDateSelected(object sender, DayTappedEventArgs selectedDate)
        {
            // Обновляем Label с выбранной датой
            SelectedDateLabel.Text = $"Вы выбрали дату: {selectedDate:d}";
        }

        private void OnDateRangeSelected(object sender, DateRangeTappedEventArgs dateRange)
        {
            // Обновляем Label с выбранным диапазоном дат
            DateRangeLabel.Text = $"Вы выбрали диапазон: {dateRange.StartDate} - {dateRange.EndDate}";
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
