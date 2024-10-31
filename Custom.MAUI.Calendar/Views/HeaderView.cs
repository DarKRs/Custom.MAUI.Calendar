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
            BindableProperty.Create(nameof(DisplayMode), typeof(CalendarDisplayMode), typeof(HeaderView), CalendarDisplayMode.Default, propertyChanged: OnDisplayModeChanged);

        public CalendarDisplayMode DisplayMode
        {
            get => (CalendarDisplayMode)GetValue(DisplayModeProperty);
            set => SetValue(DisplayModeProperty, value);
        }

        public static readonly BindableProperty CurrentDateProperty =
            BindableProperty.Create(nameof(CurrentDate), typeof(DateTime), typeof(HeaderView), DateTime.Today, propertyChanged: OnCurrentDateChanged);

        public DateTime CurrentDate
        {
            get => (DateTime)GetValue(CurrentDateProperty);
            set => SetValue(CurrentDateProperty, value);
        }

        // Свойство Culture для локализации
        public static readonly BindableProperty CultureProperty =
            BindableProperty.Create(nameof(Culture), typeof(CultureInfo), typeof(HeaderView), CultureInfo.CurrentCulture, propertyChanged: OnCultureChanged);

        public CultureInfo Culture
        {
            get => (CultureInfo)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
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

        private static void OnCultureChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HeaderView headerView)
            {
                headerView.UpdateLabels();
            }
        }

        private void InitializeHeader()
        {
            Content = null;

            // Инициализация компонентов
            switch (DisplayMode)
            {
                case CalendarDisplayMode.SeparateMonthYear:
                    _monthLabel = CreateLabel();
                    _yearLabel = CreateLabel();

                    _previousMonthButton = CreateNavigationButton("<", (s, e) => PreviousMonthClicked?.Invoke(this, EventArgs.Empty));
                    _nextMonthButton = CreateNavigationButton(">", (s, e) => NextMonthClicked?.Invoke(this, EventArgs.Empty));
                    _previousYearButton = CreateNavigationButton("<", (s, e) => PreviousYearClicked?.Invoke(this, EventArgs.Empty));
                    _nextYearButton = CreateNavigationButton(">", (s, e) => NextYearClicked?.Invoke(this, EventArgs.Empty));

                    var monthLayout = CreateNavigationLayout(_previousMonthButton, _monthLabel, _nextMonthButton);
                    var yearLayout = CreateNavigationLayout(_previousYearButton, _yearLabel, _nextYearButton);

                    Content = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children = { yearLayout, monthLayout },
                        HorizontalOptions = LayoutOptions.Center
                    };
                    break;

                case CalendarDisplayMode.SeparateMonthFixedYear:
                    _monthLabel = CreateLabel();
                    _yearLabel = CreateLabel();

                    _previousMonthButton = CreateNavigationButton("<", (s, e) => PreviousMonthClicked?.Invoke(this, EventArgs.Empty));
                    _nextMonthButton = CreateNavigationButton(">", (s, e) => NextMonthClicked?.Invoke(this, EventArgs.Empty));

                    var fixedYearLayout = new StackLayout
                    {
                        Orientation = StackOrientation.Vertical,
                        Children =
                    {
                        _yearLabel,
                        CreateNavigationLayout(_previousMonthButton, _monthLabel, _nextMonthButton)
                    },
                        HorizontalOptions = LayoutOptions.Center
                    };

                    Content = fixedYearLayout;
                    break;

                default:
                    _monthYearLabel = CreateLabel();

                    _previousMonthButton = CreateNavigationButton("<", (s, e) => PreviousMonthClicked?.Invoke(this, EventArgs.Empty));
                    _nextMonthButton = CreateNavigationButton(">", (s, e) => NextMonthClicked?.Invoke(this, EventArgs.Empty));

                    var defaultLayout = CreateNavigationLayout(_previousMonthButton, _monthYearLabel, _nextMonthButton);

                    Content = defaultLayout;
                    break;
            }

            UpdateLabels();
        }

        private Button CreateNavigationButton(string text, EventHandler clickedHandler)
        {
            var button = new Button
            {
                Text = text,
                WidthRequest = 40,
                HeightRequest = 40,
                Padding = new Thickness(5),
                BackgroundColor = Colors.LightGray,
                CornerRadius = 20
            };
            button.Clicked += clickedHandler;
            return button;
        }

        private Label CreateLabel()
        {
            return new Label
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                FontSize = 18
            };
        }

        private StackLayout CreateNavigationLayout(View previousButton, View label, View nextButton)
        {
            return new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                Children = { previousButton, label, nextButton }
            };
        }

        private void UpdateLabels()
        {
            if (Culture == null)
            {
                Culture = CultureInfo.CurrentCulture;
            }

            if (DisplayMode == CalendarDisplayMode.SeparateMonthYear || DisplayMode == CalendarDisplayMode.SeparateMonthFixedYear)
            {
                _monthLabel.Text = CurrentDate.ToString("MMMM", Culture);
                _yearLabel.Text = CurrentDate.ToString("yyyy", Culture);
            }
            else
            {
                _monthYearLabel.Text = CurrentDate.ToString("MMMM yyyy", Culture);
            }
        }
    }
}
