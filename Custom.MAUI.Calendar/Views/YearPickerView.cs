using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Views
{
    public class YearPickerView : ContentView
    {
        public event EventHandler<int> YearSelected;

        public int StartYear { get; set; } = DateTime.Today.Year - 10;
        public int EndYear { get; set; } = DateTime.Today.Year + 10;

        public YearPickerView()
        {
            BuildYearGrid();
        }

        private void BuildYearGrid()
        {
            var grid = new Grid
            {
                RowSpacing = 5,
                ColumnSpacing = 5
            };

            int totalYears = EndYear - StartYear + 1;
            int columns = 4;
            int rows = (int)Math.Ceiling((double)totalYears / columns);

            for (int i = 0; i < rows; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < columns; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            int year = StartYear;
            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < columns; col++)
                {
                    if (year > EndYear)
                        break;

                    var yearButton = new Button
                    {
                        Text = year.ToString(),
                        CommandParameter = year
                    };
                    yearButton.Clicked += OnYearButtonClicked;

                    Grid.SetRow(yearButton, row);
                    Grid.SetColumn(yearButton, col);
                    grid.Children.Add(yearButton);

                    year++;
                }
            }

            Content = new ScrollView { Content = grid };
        }

        private void OnYearButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int year)
            {
                YearSelected?.Invoke(this, year);
            }
        }
    }

}
