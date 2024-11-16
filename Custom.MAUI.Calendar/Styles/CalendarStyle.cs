using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Styles
{
    public class CalendarStyle
    {
        // **Общие настройки**
        public Color BackgroundColor { get; set; } = Colors.White; // Фон всего календаря

        // **Настройки дней**
        public Color DayTextColor { get; set; } = Colors.Black; // Цвет текста дней
        public double DayFontSize { get; set; } = 14; // Размер шрифта дней
        public Thickness DayButtonPadding { get; set; } = new Thickness(5); // Отступы кнопок дней
        public Color TodayBackgroundColor { get; set; } = Colors.LightBlue; // Фон сегодняшнего дня
        public Color SelectedDateBackgroundColor { get; set; } = Colors.LightGreen; // Фон выбранной даты
        public Color DateRangeBackgroundColor { get; set; } = Colors.LightGreen; // Фон диапазона дат

        // **Настройки навигационных кнопок**
        public Color NavigationButtonBackgroundColor { get; set; } = Colors.LightGray; // Фон кнопок навигации
        public Color NavigationButtonTextColor { get; set; } = Colors.Black; // Цвет текста кнопок навигации
        public double NavigationButtonSize { get; set; } = 40; // Размер кнопок навигации
        public double NavigationButtonCornerRadius { get; set; } = 20; // Радиус скругления кнопок навигации
        public Thickness NavigationButtonPadding { get; set; } = new Thickness(5); // Отступы кнопок навигации

        // **Настройки заголовков и меток**
        public Color LabelTextColor { get; set; } = Colors.Black; // Цвет текста меток (месяц, год, дни недели)
        public double LabelFontSize { get; set; } = 18; // Размер шрифта меток

        // **Настройки дней недели**
        public Color DayOfWeekLabelTextColor { get; set; } = Colors.Black; // Цвет текста названий дней недели
        public double DayOfWeekLabelFontSize { get; set; } = 14; // Размер шрифта названий дней недели

    }
}
