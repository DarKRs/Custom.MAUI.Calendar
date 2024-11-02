using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Custom.MAUI.Calendar.Views
{
    public class HeaderView : ContentView, IDisposable
    {
        public event EventHandler PreviousMonthClicked;
        public event EventHandler NextMonthClicked;
        public event EventHandler PreviousYearClicked;
        public event EventHandler NextYearClicked;
        public event EventHandler MonthLabelTapped;
        public event EventHandler YearLabelTapped;
        public event EventHandler MonthYearLabelTapped;

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

        public static readonly BindableProperty CultureProperty =
            BindableProperty.Create(nameof(Culture), typeof(CultureInfo), typeof(HeaderView), CultureInfo.CurrentCulture, propertyChanged: OnCultureChanged);

        public CultureInfo Culture
        {
            get => (CultureInfo)GetValue(CultureProperty);
            set => SetValue(CultureProperty, value);
        }

        public CalendarStyle Style { get; set; }

        public HeaderView()
        {
            InitializeHeader();
        }

        private static void OnDisplayModeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is HeaderView headerView)
            {
                headerView.InitializeHeader();
                headerView.UpdateLabels();
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

            _monthYearLabel = CreateLabel();
            _monthLabel = CreateLabel();
            _yearLabel = CreateLabel();

            _previousMonthButton = CreateNavigationButton("<", (s, e) => PreviousMonthClicked?.Invoke(this, EventArgs.Empty));
            _nextMonthButton = CreateNavigationButton(">", (s, e) => NextMonthClicked?.Invoke(this, EventArgs.Empty));
            _previousYearButton = CreateNavigationButton("<", (s, e) => PreviousYearClicked?.Invoke(this, EventArgs.Empty));
            _nextYearButton = CreateNavigationButton(">", (s, e) => NextYearClicked?.Invoke(this, EventArgs.Empty));

            switch (DisplayMode)
            {
                case CalendarDisplayMode.SeparateMonthYear:
                    ConfigureSeparateMonthYearLayout();
                    break;

                case CalendarDisplayMode.SeparateMonthFixedYear:
                    ConfigureSeparateMonthFixedYearLayout();
                    break;

                default:
                    ConfigureDefaultLayout();
                    break;
            }

            UpdateLabels();
        }

        private void ConfigureSeparateMonthYearLayout()
        {
            AddLabelTapGesture(_monthLabel, OnMonthLabelTapped);
            AddLabelTapGesture(_yearLabel, OnYearLabelTapped);

            var monthLayout = CreateNavigationLayout(_previousMonthButton, _monthLabel, _nextMonthButton);
            var yearLayout = CreateNavigationLayout(_previousYearButton, _yearLabel, _nextYearButton);

            Content = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                Children = { yearLayout, monthLayout },
                HorizontalOptions = LayoutOptions.Center
            };
        }

        private void ConfigureSeparateMonthFixedYearLayout()
        {
            AddLabelTapGesture(_monthLabel, OnMonthLabelTapped);

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
        }

        private void ConfigureDefaultLayout()
        {
            AddLabelTapGesture(_monthYearLabel, OnMonthYearLabelTapped);

            var defaultLayout = CreateNavigationLayout(_previousMonthButton, _monthYearLabel, _nextMonthButton);

            Content = defaultLayout;
        }

        private Button CreateNavigationButton(string text, EventHandler clickedHandler)
        {
            var button = new Button
            {
                Text = text,
                WidthRequest = 40,
                HeightRequest = 40,
                Padding = new Thickness(5),
                BackgroundColor = Style?.NavigationButtonBackgroundColor ?? Colors.LightGray,
                TextColor = Style?.NavigationButtonTextColor ?? Colors.Black,
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
                FontSize = Style?.LabelFontSize ?? 18,
                TextColor = Style?.LabelTextColor ?? Colors.Black,
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

        private void AddLabelTapGesture(Label label, EventHandler<TappedEventArgs> tappedHandler)
        {
            var tapGesture = new TapGestureRecognizer();
            tapGesture.Tapped += tappedHandler;
            label.GestureRecognizers.Add(tapGesture);
        }

        private void OnMonthLabelTapped(object sender, EventArgs e)
        {
            MonthLabelTapped?.Invoke(this, EventArgs.Empty);
        }

        private void OnYearLabelTapped(object sender, EventArgs e)
        {
            YearLabelTapped?.Invoke(this, EventArgs.Empty);
        }

        private void OnMonthYearLabelTapped(object sender, EventArgs e)
        {
            MonthYearLabelTapped?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose()
        {
            // Отсоединяем жесты от меток
            _monthLabel.GestureRecognizers.Clear();
            _yearLabel.GestureRecognizers.Clear();
            _monthYearLabel.GestureRecognizers.Clear();
        }
    }

}
