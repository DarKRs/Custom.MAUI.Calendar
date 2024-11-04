# CustomCalendar for .NET MAUI

**CustomCalendar** — это настраиваемый календарный компонент для .NET MAUI.

## Возможности

- **Выбор даты или диапазона дат**.
- **Поддержка локализации** через свойство `Culture`.
- **Настройка внешнего вида** через класс `CalendarStyle`.
- **События для обработки взаимодействия** (выбор даты, изменение месяца и др.).

## Установка

Установите пакет через NuGet Package Manager:

```bash
dotnet add package Custom.MAUI.Calendar
```
## Использование

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:controls="clr-namespace:Custom.MAUI.Calendar;assembly=Custom.MAUI.Calendar"
             x:Class="YourNamespace.MainPage">

    <controls:CustomCalendar x:Name="MyCalendar" />
</ContentPage>
```

В этом примере будут использоваться стили по умолчанию.

### Режимы отображения

- "Default" Месяц и год отображаются в одном Label
- "SeparateMonthFixedYear" Месяц и год отображаются в разных Label. Есть кнопки смены месяца
- "SeparateMonthYear" Месяц и год отображаются в разных Label. Есть кнопки смены месяца и года

Прим.
```xaml
    <controls:CustomCalendar x:Name="Calendar"
                             DisplayMode="SeparateMonthYear"/>
```

### Настройка локализации

**CultureInfo** можно указать двумя способами:

1. Указать в App.xaml.cs
```csharp
public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        var culture = new CultureInfo("fr-FR");
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;

        MainPage = new AppShell();
    }
}
```

2. Привязать в YourPage.xaml.cs
```csharp
public MainPage()
{
    InitializeComponent();
    Calendar.Culture = CultureInfo.GetCultureInfo("fr-FR");
}
```

### Настройка стиля

```xaml
            <controls:CustomCalendar x:Name="Calendar">
                <controls:CustomCalendar.Style>
                    <controls:CalendarStyle
                        BackgroundColor="LightYellow"
                        DayTextColor="DarkBlue"
                        TodayBackgroundColor="LightGreen"
                        SelectedDateBackgroundColor="Orange"
                        NavigationButtonBackgroundColor="Gray"
                        NavigationButtonTextColor="White"
                        LabelTextColor="DarkRed"
                        LabelFontSize="20"
                        DayFontSize="16"
                        DayButtonPadding="10"
                        NavigationButtonSize="50"
                        NavigationButtonCornerRadius="25" />
                </controls:CustomCalendar.Style>
            </controls:CustomCalendar>
```


### Использование событий

```xaml
    <controls:CustomCalendar x:Name="Calendar"
                             DateSelected="OnDateSelected"
                             DateRangeSelected="OnDateRangeSelected" />
```

```csharp
public partial class MainPage : ContentPage
{
    int count = 0;

    public MainPage()
    {
        InitializeComponent();
        Calendar.DateSelected += OnDateSelected;
        Calendar.DateRangeSelected += OnDateRangeSelected;
    }

    private void OnDateSelected(object sender, DateTime selectedDate)
    {
        // Обновляем Label с выбранной датой
        SelectedDateLabel.Text = $"Вы выбрали дату: {selectedDate:d}";
    }

    private void OnDateRangeSelected(object sender, (DateTime, DateTime) dateRange)
    {
        // Обновляем Label с выбранным диапазоном дат
        DateRangeLabel.Text = $"Вы выбрали диапазон: {dateRange.Item1:d} - {dateRange.Item2:d}";
    }
}
```

#### Список событий

- **DateSelected**  — срабатывает при выборе даты.
- **DateRangeSelected**  — срабатывает при выборе диапазона дат.
- **MonthChanged** — срабатывает при изменении месяца.
- **YearChanged**  — срабатывает при изменении года.
- **ViewModeChanged**  — срабатывает при изменении режима отображения.
- **DayLongPressed**  — срабатывает при долгом нажатии на день.
- **DateDeselected**  — срабатывает при снятии выделения с даты.
- **DateRangeCleared**  — срабатывает при очистке выбранного диапазона дат.
- **CultureChanged**  — срабатывает при изменении культуры.
- **StyleChanged**  — срабатывает при изменении стиля.
- **HeaderTapped**  — срабатывает при нажатии на заголовок.
- **DayOfWeekLabelTapped**  — срабатывает при нажатии на название дня недели.

