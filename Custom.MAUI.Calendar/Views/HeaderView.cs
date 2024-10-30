using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Views
{
    public class HeaderView : ContentView
    {
        public event EventHandler PreviousMonthClicked;
        public event EventHandler NextMonthClicked;
        public event EventHandler PreviousYearClicked;
        public event EventHandler NextYearClicked;

        private Label _monthYearLabel;
        private Label _monthLabel;
        private Label _yearLabel;
        private Button _previousMonthButton;
        private Button _nextMonthButton;
        private Button _previousYearButton;
        private Button _nextYearButton;

        public static readonly BindableProperty DisplayModeProperty =
            BindableProperty.Create(nameof(DisplayMode), typeof(string), typeof(HeaderView), "Default", propertyChanged: OnDisplayModeChanged);

        public string DisplayMode
        {
            get => (string)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(HeaderView), DateTime.Today, propertyChanged: OnCurrentDateChanged);

        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        public HeaderView()
        {
            InitializeHeader();
        }

        private static void OnDisplayModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HeaderView headerView)
            {
                headerView.InitializeHeader();
            }
        }

        private static void OnCurrentDateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HeaderView headerView)
            {
                headerView.UpdateLabels();
            }
        }

        private void InitializeHeader()
        {
            Content = null;

            // Инициализация компонентов в зависимости от режима отображения
            if (DisplayMode == "SeparateMonthYear")
            {
                var monthLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                _previousMonthButton = CreateNavigationButton("<");
                _previousMonthButton.Clicked += (s, e) => PreviousMonthClicked?.Invoke(this, EventArgs.Empty);
                _monthLabel = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FontSize = 18 };
                _nextMonthButton = CreateNavigationButton(">");
                _nextMonthButton.Clicked += (s, e) => NextMonthClicked?.Invoke(this, EventArgs.Empty);
                monthLayout.Children.Add(_previousMonthButton);
                monthLayout.Children.Add(_monthLabel);
                monthLayout.Children.Add(_nextMonthButton);

                var yearLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                _previousYearButton = CreateNavigationButton("<");
                _previousYearButton.Clicked += (s, e) => PreviousYearClicked?.Invoke(this, EventArgs.Empty);
                _yearLabel = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FontSize = 18 };
                _nextYearButton = CreateNavigationButton(">");
                _nextYearButton.Clicked += (s, e) => NextYearClicked?.Invoke(this, EventArgs.Empty);
                yearLayout.Children.Add(_previousYearButton);
                yearLayout.Children.Add(_yearLabel);
                yearLayout.Children.Add(_nextYearButton);

                Content = new StackLayout
                {
                    Orientation = StackOrientation.Vertical,
                    Children = { yearLayout, monthLayout },
                    HorizontalOptions = LayoutOptions.Center
                };
            }
            else
            {
                var monthYearLayout = new StackLayout { Orientation = StackOrientation.Horizontal, HorizontalOptions = LayoutOptions.Center };
                _previousMonthButton = CreateNavigationButton("<");
                _previousMonthButton.Clicked += (s, e) => PreviousMonthClicked?.Invoke(this, EventArgs.Empty);
                _monthYearLabel = new Label { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center, FontSize = 18 };
                _nextMonthButton = CreateNavigationButton(">");
                _nextMonthButton.Clicked += (s, e) => NextMonthClicked?.Invoke(this, EventArgs.Empty);
                monthYearLayout.Children.Add(_previousMonthButton);
                monthYearLayout.Children.Add(_monthYearLabel);
                monthYearLayout.Children.Add(_nextMonthButton);

                Content = monthYearLayout;
            }

            UpdateLabels();
        }

        private Button CreateNavigationButton(string text)
        {
            return new Button
            {
                Text = text,
                WidthRequest = 40,
                HeightRequest = 40,
                Padding = new Thickness(5),
                BackgroundColor = Colors.LightGray,
                CornerRadius = 20
            };
        }

        private void UpdateLabels()
        {
            if (DisplayMode == "SeparateMonthYear")
            {
                _monthLabel.Text = CurrentDate.ToString("MMMM", new CultureInfo("ru-RU"));
                _yearLabel.Text = CurrentDate.Year.ToString();
            }
            else
            {
                _monthYearLabel.Text = CurrentDate.ToString("MMMM yyyy", new CultureInfo("ru-RU"));
            }
        }
    }
}
