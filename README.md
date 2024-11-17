# Custom MAUI Components

Этот репозиторий содержит настраиваемые компоненты для .NET MAUI. На данный момент календарь и TimePicker

## Возможности

### CustomCalendar
- **Выбор даты или диапазона дат**.
- **Поддержка локализации** через свойство `Culture`.
- **Настройка внешнего вида** через класс `CalendarStyle`.
- **События для обработки взаимодействия** (выбор даты, изменение месяца и др.).

### CustomTimePicker
- **Ввод времени вручную или через Popup**
- **Поддержка различных форматов времени (`HH:mm:ss`, `HH:mm` и т.д.).**
- **События для отслеживания изменения времени.**
- **Настройка внешнего вида** через класс `TimePickerStyle`.

## Установка

Установите пакет через NuGet Package Manager:

```bash
dotnet add package Custom.MAUI.Components
```
или
```bash
Install-Package Custom.MAUI.Components
```

**Важно:** Для использования CustomTimePicker так же нужно установить CommunityToolkit.Maui
И добавить строку UseMauiCommunityToolkit(); в ваш MauiProgram

```csharp
var builder = MauiApp.CreateBuilder();
builder
    .UseMauiApp<App>()
    .UseMauiCommunityToolkit()
    .ConfigureFonts(fonts =>
    {
        fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
        fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
    });
```

## Использование

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:custom="clr-namespace:Custom.MAUI.Components;assembly=Custom.MAUI.Components"
             x:Class="YourNamespace.MainPage">

           <custom:CustomCalendar x:Name="Calendar" DisplayMode="SeparateMonthYear" />

           <custom:CustomTimePicker TimeSelected="OnTimeSelected" CustomWidth="200" CustomHeight="50" TimeFormat="HH:mm" />

</ContentPage>
```

В этом примере будут использоваться стили по умолчанию.

### Режимы отображения календаря

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

### Настройка и использование стилей

```xaml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:custom="clr-namespace:Custom.MAUI.Components;assembly=Custom.MAUI.Components"
             xmlns:customstyles="clr-namespace:Custom.MAUI.Components.Styles;assembly=Custom.MAUI.Components"
             x:Class="YourNamespace.MainPage">

<custom:CustomCalendar x:Name="Calendar" DisplayMode="SeparateMonthYear" Style="{StaticResource MyCalendarStyle}"/>

<custom:CustomTimePicker TimeSelected="OnTimeSelected" CustomWidth="200" CustomHeight="50" TimeFormat="HH:mm" Style="{StaticResource MyTimePickerStyle}"/>

</ContentPage>
```


### Список событий

#### События календаря

- **DateSelected**  — срабатывает при выборе даты.
- **DateRangeSelected**  — срабатывает при выборе диапазона дат.
- **MonthChanged** — срабатывает при изменении месяца.
- **YearChanged**  — срабатывает при изменении года.
- **ViewModeChanged**  — срабатывает при изменении режима отображения.
- **DateDeselected**  — срабатывает при снятии выделения с даты.
- **DateRangeCleared**  — срабатывает при очистке выбранного диапазона дат.
- **CultureChanged**  — срабатывает при изменении культуры.
- **StyleChanged**  — срабатывает при изменении стиля.

#### События TimePicker

- **TimeSelected** — срабатывает при выборе времени.
- **TimeFormatChanged** — срабатывает при изменении формата времени.
- **TimeEntryCompleted** — срабатывает, когда пользователь завершает ввод времени.
- **PopupOpened** — срабатывает при открытии всплывающего окна выбора времени.
- **PopupClosed** — срабатывает при закрытии всплывающего окна выбора времени.
- **TimeEntryTextChanged** — срабатывает при изменении текста в поле ввода времени.
- **StyleChanged** — срабатывает при изменении стиля.
