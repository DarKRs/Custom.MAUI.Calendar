using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Views
{
    public class MonthPickerView : ContentView
    {
        public event EventHandler<int> MonthSelected;

        public CultureInfo Culture { get; set; } = CultureInfo.CurrentCulture;

        public MonthPickerView()
        {
            BuildMonthGrid();
        }

        private void BuildMonthGrid()
        {
            var months = Culture.DateTimeFormat.MonthNames.Take(12).ToArray();

            var grid = new Grid
            {
                RowSpacing = 5,
                ColumnSpacing = 5
            };

            for (int i = 0; i < 3; i++)
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < 4; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            int index = 0;
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    if (index >= months.Length)
                        break;

                    var monthButton = new Button
                    {
                        Text = months[index],
                        CommandParameter = index + 1
                    };
                    monthButton.Clicked += OnMonthButtonClicked;

                    Grid.SetRow(monthButton, row);
                    Grid.SetColumn(monthButton, col);
                    grid.Children.Add(monthButton);

                    index++;
                }
            }

            Content = grid;
        }

        private void OnMonthButtonClicked(object sender, EventArgs e)
        {
            if (sender is Button button && button.CommandParameter is int month)
            {
                MonthSelected?.Invoke(this, month);
            }
        }
    }

}
