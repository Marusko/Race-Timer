using Race_timer.Logic;
using System;
using System.Windows;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for InfoWindow.xaml
    /// </summary>
    public partial class InfoWindow : Window
    {
        private readonly ScreenHandler _screenHandler;

        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private int _stateOfScroll = ClockLogic.ScrollBegin;
        private int _currentTime;
        private int _currentDelay;

        private const int ScrollDelay = 500;
        private const int ScrollTimes = 20;
        private const int TimerMillis = 10;

        public int Laps { get; set; }

        /// <summary>
        /// Initializes component, adds method when window is loaded
        /// </summary>
        /// <param name="sh">Already created ScreenHandler</param>
        public InfoWindow(ScreenHandler sh)
        {
            InitializeComponent();
            _screenHandler = sh;
            Loaded += WindowLoaded;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, TimerMillis);
        }

        /// <summary>
        /// Method called after window is loaded, sets the position, state and width of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_screenHandler.SelectedScreen == null) return;
            WindowState = WindowState.Normal;
            Left = _screenHandler.SelectedScreen.WorkingArea.Left;
            Top = _screenHandler.SelectedScreen.WorkingArea.Bottom - 200;
            Width = _screenHandler.SelectedScreen.WorkingArea.Width;

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)_screenHandler.SelectedScreen.WorkingArea.Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("InfoFontSize");
            Application.Current.Resources.Add("InfoFontSize", controlSize * 3 - 5);
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }
        /// <summary>
        /// Starts the timer
        /// </summary>
        public void StartTimer()
        {
            _timer.Start();
        }

        /// <summary>
        /// Called by timer, scrolls the label content horizontally, waits on beginning and end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object? sender, EventArgs e)
        {
            if (InfoText.Text.Length > 110)
            {
                if (_currentDelay != ScrollDelay && (_stateOfScroll == ClockLogic.ScrollBegin || _stateOfScroll == ClockLogic.ScrollEnd))
                {
                    _currentDelay++;
                }
                else
                {
                    if (_stateOfScroll == ClockLogic.ScrollBegin)
                    {
                        _stateOfScroll = ClockLogic.Scrolling;
                    }
                    else if (_stateOfScroll == ClockLogic.Scrolling)
                    {
                        if (InfoScrollViewer.HorizontalOffset >= InfoScrollViewer.ScrollableWidth)
                        {
                            _currentTime = 0;
                            _stateOfScroll = ClockLogic.ScrollEnd;
                        }
                        else
                        {
                            _currentTime++;
                            InfoScrollViewer.ScrollToHorizontalOffset(_currentTime * (InfoScrollViewer.ScrollableWidth / (ScrollTimes * Math.Abs(InfoText.Text.Length - 110))));
                            InfoScrollViewer.UpdateLayout();
                        }
                    }
                    else if (_stateOfScroll == ClockLogic.ScrollEnd)
                    {
                        InfoScrollViewer.ScrollToLeftEnd();
                        InfoScrollViewer.UpdateLayout();
                        _stateOfScroll = ClockLogic.ScrollBegin;
                        Laps++;
                    }
                    _currentDelay = 0;
                }
            }
        }

        /// <summary>
        /// If label can be scrolled
        /// </summary>
        /// <returns>Bool if label can be scrolled</returns>
        public bool IsScrolling()
        {
            return InfoText.Text.Length > 110;
        }

        /// <summary>
        /// Sets new label
        /// </summary>
        /// <param name="text">New info to be set</param>
        public void SetLabel(string text)
        {
            var split = text.Split(";");
            InfoText.Text = string.Join("  |  ", split);
        }
    }
}
