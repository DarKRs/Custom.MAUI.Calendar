using Custom.MAUI.Components.Styles;
using System.Globalization;

namespace Custom.MAUI.Components.Views
{
    internal class HeaderView : ContentView, IDisposable
    {
        private double _scaleFactor = 1.0;
        public double ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                if (_scaleFactor != value)
                {
                    _scaleFactor = value;
                    InitializeHeader();
                }
            }
        }

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

        private CalendarDisplayMode _displayMode = CalendarDisplayMode.Default;
        public CalendarDisplayMode DisplayMode
        {
            get => _displayMode;
            set
            {
                if (_displayMode != value)
                {
                    _displayMode = value;
                    InitializeHeader();
                    UpdateLabels();
                }
            }
        }

        private DateTime _currentDate = DateTime.Today;
        public DateTime CurrentDate
        {
            get => _currentDate;
            set
            {
                if (_currentDate != value)
                {
                    _currentDate = value;
                    UpdateLabels();
                }
            }
        }

        private CultureInfo _culture = CultureInfo.CurrentCulture;
        public CultureInfo Culture
        {
            get => _culture;
            set
            {
                if (_culture != value)
                {
                    _culture = value;
                    UpdateLabels();
                }
            }
        }

        public CalendarStyle Style { get; set; }

        public HeaderView()
        {
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

            switch (_displayMode)
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
                FontAttributes = FontAttributes.Bold,
                MinimumHeightRequest = Style.NavigationButtonSize * ScaleFactor,
                MinimumWidthRequest = Style.NavigationButtonSize * ScaleFactor,
                Padding = new Thickness(Style.NavigationButtonPadding.Left * ScaleFactor),
                BackgroundColor = Style.NavigationButtonBackgroundColor,
                TextColor = Style.NavigationButtonTextColor,
                CornerRadius = Style.NavigationButtonCornerRadius 
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
                FontSize = Style.LabelFontSize * ScaleFactor,
                TextColor = Style.LabelTextColor,
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
            if (_culture == null)
            {
                _culture = CultureInfo.CurrentCulture;
            }

            if (_displayMode == CalendarDisplayMode.SeparateMonthYear || _displayMode == CalendarDisplayMode.SeparateMonthFixedYear)
            {
                _monthLabel.Text = _currentDate.ToString("MMMM", _culture);
                _yearLabel.Text = _currentDate.ToString("yyyy", _culture);
            }
            else
            {
                _monthYearLabel.Text = _currentDate.ToString("MMMM yyyy", _culture);
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
