using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar
{
    public class CalendarStyle
    {
        public Color BackgroundColor { get; set; } = Colors.White;
        public Color TodayBackgroundColor { get; set; } = Colors.LightBlue;
        public Color SelectedDateBackgroundColor { get; set; } = Colors.LightGreen;
        public Color DateRangeBackgroundColor { get; set; } = Colors.LightGreen;
        public Color NavigationButtonBackgroundColor { get; set; } = Colors.LightGray;
        public Color NavigationButtonTextColor { get; set; } = Colors.Black;
        public Color LabelTextColor { get; set; } = Colors.Black;
        public double LabelFontSize { get; set; } = 18;
        public double DayButtonFontSize { get; set; } = 14;

    }
}
