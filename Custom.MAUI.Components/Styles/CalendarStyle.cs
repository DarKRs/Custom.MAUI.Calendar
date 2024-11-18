namespace Custom.MAUI.Components.Styles
{
    public class CalendarStyle
    {
        // **Общие настройки**
        public Color BackgroundColor { get; set; } = Colors.White; // Фон всего календаря
        public Thickness FramePadding { get; set; } = new Thickness(0);
        public Thickness FrameMargin { get; set; } = new Thickness(0);

        // **Настройки дней**
        public Color DayTextColor { get; set; } = Colors.Black; // Цвет текста дней
        public double DayFontSize { get; set; } = 12; // Размер шрифта дней
        public Thickness DayButtonPadding { get; set; } = new Thickness(2); // Отступы внутри кнопок
        public Thickness DayButtonMargin { get; set; } = new Thickness(5); // Отступы внешние
        public Color DayButtonBackgroundColor { get; set; } = Colors.Transparent;
        public Color TodayBackgroundColor { get; set; } = Colors.LightBlue; // Фон сегодняшнего дня
        public Color SelectedDateBackgroundColor { get; set; } = Colors.LightGreen; // Фон выбранной даты
        public Color DateRangeBackgroundColor { get; set; } = Colors.LightGreen; // Фон диапазона дат
        public double DaysButtonHeight { get; set; } = 20;
        public double DaysButtonWidth { get; set; } = 50;

        public double MonthFontSize { get; set; } = 14; //Размер шрифта месяцев в выборе месяцев
        public int MonthCornerRadius { get; set; } = 0; 
        public Color MonthButtonBackgroundColor { get; set; } = Color.FromArgb("#E3F2FD"); // Фон кнопок выбора месяца

        // **Настройки навигационных кнопок**
        public Color NavigationButtonBackgroundColor { get; set; } = Colors.Transparent; // Фон кнопок навигации
        public Color NavigationButtonTextColor { get; set; } = Colors.Black; // Цвет текста кнопок навигации
        public double NavigationButtonSize { get; set; } = 20; // Размер кнопок навигации
        public int NavigationButtonCornerRadius { get; set; } = 30; // Радиус скругления кнопок навигации
        public Thickness NavigationButtonPadding { get; set; } = new Thickness(2); // Отступы внутри кнопок навигации
        public Thickness NavigationButtonMargin { get; set; } = new Thickness(5); // Отступы внешние

        // **Настройки заголовков и меток**
        public Color LabelTextColor { get; set; } = Colors.Black; // Цвет текста меток (месяц, год, дни недели)
        public double LabelFontSize { get; set; } = 16; // Размер шрифта меток

        // **Настройки дней недели**
        public Color DayOfWeekLabelTextColor { get; set; } = Colors.Black; // Цвет текста названий дней недели
        public double DayOfWeekLabelFontSize { get; set; } = 12; // Размер шрифта названий дней недели


    }
}
