namespace Custom.MAUI.Components.Styles
{
    public class TimePickerStyle
    {
        // **Общие настройки**
        public Color BackgroundColor { get; set; } = Colors.White; // Фон компонента

        // **Настройки текстового ввода времени**
        public Color TimeEntryBackgroundColor { get; set; } = Colors.Transparent; // Фон текстового поля ввода
        public Color TimeEntryTextColor { get; set; } = Colors.Black; // Цвет текста ввода
        public double TimeEntryFontSize { get; set; } = 14; // Размер шрифта текстового ввода
        public Thickness TimeEntryPadding { get; set; } = new Thickness(5); // Отступы текстового поля ввода

        // **Настройки кнопки-стрелки**
        public Color DropdownButtonBackgroundColor { get; set; } = Colors.Transparent; // Фон кнопки-стрелки
        public Color DropdownButtonTextColor { get; set; } = Colors.Black; // Цвет текста (стрелки)
        public double DropdownButtonSize { get; set; } = 20; // Размер кнопки
        public double DropdownButtonFontSize { get; set; } = 10; // Размер текста (стрелки)
        public Thickness DropdownButtonPadding { get; set; } = new Thickness(0); // Отступы кнопки

        // **Настройки окна Popup**
        public Color PopupBackgroundColor { get; set; } = Colors.White; // Фон всплывающего окна
        public float PopupCornerRadius { get; set; } = 10; // Радиус скругления углов
        public Thickness PopupPadding { get; set; } = new Thickness(5); //Отступы окна
        public Thickness PopupMargin { get; set; } = new Thickness(5); //Отступы окна
        public Color PopupTextColor { get; set; } = Colors.Black; // Цвет текста во всплывающем окне
        public double PopupFontSize { get; set; } = 14; // Размер шрифта во всплывающем окне

        // **Настройки Popup (часы, минуты, секунды)**
        public Color TimeComponentBackgroundColor { get; set; } = Colors.Transparent; // Фон элементов
        public Color TimeComponentSelectedBackgroundColor { get; set; } = Colors.Purple; // Фон выделенного элемента
        public Color TimeComponentSelectedTextColor { get; set; } = Colors.White; // Цвет текста выделенного элемента
        public Thickness TimeComponentPadding { get; set; } = new Thickness(3); // Отступы для элементов
    }
}
