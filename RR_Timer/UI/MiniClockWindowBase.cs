using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Race_timer.ClockUserControl;
using Race_timer.Logic;
using Race_timer.Logic.Interfaces;

namespace Race_timer.UI
{
    /// <summary>
    /// Shared logic for minimized clock windows, classic and WebView one
    /// </summary>
    public abstract class MiniClockWindowBase : Window, IClockWindow
    {
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private int _showTimerIndex;

        private readonly System.Windows.Threading.DispatcherTimer _nameScrollTimer = new();
        private int _stateOfScroll = ClockLogic.ScrollBegin;
        private int _currentTime;
        private int _currentDelay;

        private const int TimerShownForSeconds = 5;
        private const int ScrollDelay = 500;
        private const int ScrollTimes = 1;
        private const int ScrollTimerMillis = 10;

        private bool _clockInMiniPanel;
        public MiniContestTimer? Clock { get; set; }

        /// <summary>
        /// Text block with event name
        /// </summary>
        protected abstract TextBlock EventNameText { get; }

        /// <summary>
        /// Panel with currently shown contest timer or clock
        /// </summary>
        protected abstract StackPanel TimersPanel { get; }

        /// <summary>
        /// Image control for logo
        /// </summary>
        protected abstract Image LogoImage { get; }

        /// <summary>
        /// Scroll viewer around event name
        /// </summary>
        protected abstract ScrollViewer EventNameScroller { get; }

        /// <summary>
        /// Window state to set after window is loaded
        /// </summary>
        protected abstract WindowState LoadedWindowState { get; }

        /// <summary>
        /// Sets the position and timers, called from derived constructors after InitializeComponent()
        /// If possible, start timer
        /// </summary>
        protected void InitializeMiniClock()
        {
            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            WindowState = WindowState.Minimized;
            Left = ScreenHandler.GetInstance().SelectedScreen?.WorkingArea.Left ?? 0;
            Top = ScreenHandler.GetInstance().SelectedScreen?.WorkingArea.Top ?? 0;

            Loaded += WindowLoaded;
            Closed += StopTimer;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, TimerShownForSeconds);
            _nameScrollTimer.Tick += NameScrollTimerTick;
            _nameScrollTimer.Interval = new TimeSpan(0, 0, 0, 0, ScrollTimerMillis);
            _nameScrollTimer.Start();
            TimerTickLogic();
            if (ClockLogic.GetInstance().MiniActiveTimers.Count > 0)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Method called after window is loaded, sets the state and font resources
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = LoadedWindowState;

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)ScreenHandler.GetInstance().GetSelectedScreenArea().Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 3 - 5);

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlWidth = (double)ScreenHandler.GetInstance().GetSelectedScreenArea().Width / 3 - 50;
            Application.Current.Resources.Remove("ControlWidth");
            Application.Current.Resources.Add("ControlWidth", controlWidth);
        }

        /// <summary>
        /// Sets the label to correct name
        /// </summary>
        /// <param name="name">Event name to show</param>
        private void SetEventName(string name)
        {
            EventNameText.Text = name;
        }

        /// <summary>
        /// Stops the timers
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopTimer(object? sender, EventArgs e)
        {
            _timer.Stop();
            _nameScrollTimer.Stop();
        }

        /// <summary>
        /// Called by timer, scrolls the event name horizontally when it doesn't fit, waits on beginning and end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NameScrollTimerTick(object? sender, EventArgs e)
        {
            if (EventNameScroller.ScrollableWidth <= 0) return;
            if (_currentDelay != ScrollDelay && (_stateOfScroll == ClockLogic.ScrollBegin || _stateOfScroll == ClockLogic.ScrollEnd))
            {
                _currentDelay++;
                return;
            }
            if (_stateOfScroll == ClockLogic.ScrollBegin)
            {
                _stateOfScroll = ClockLogic.Scrolling;
            }
            else if (_stateOfScroll == ClockLogic.Scrolling)
            {
                if (EventNameScroller.HorizontalOffset >= EventNameScroller.ScrollableWidth)
                {
                    _currentTime = 0;
                    _stateOfScroll = ClockLogic.ScrollEnd;
                }
                else
                {
                    _currentTime++;
                    EventNameScroller.ScrollToHorizontalOffset(_currentTime * ScrollTimes);
                    EventNameScroller.UpdateLayout();
                }
            }
            else if (_stateOfScroll == ClockLogic.ScrollEnd)
            {
                EventNameScroller.ScrollToLeftEnd();
                EventNameScroller.UpdateLayout();
                _stateOfScroll = ClockLogic.ScrollBegin;
            }
            _currentDelay = 0;
        }

        /// <summary>
        /// Method called by timer, call TimerTickLogic()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object? sender, EventArgs e)
        {
            TimerTickLogic();
        }

        /// <summary>
        /// Switches between start times every 5 seconds
        /// </summary>
        public void TimerTickLogic()
        {
            if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count > 0)
            {
                if (_showTimerIndex >= ClockLogic.GetInstance().MiniActiveTimers.Values.Count)
                {
                    _showTimerIndex = 0;
                }
                TimersPanel.Children.Clear();
                TimersPanel.Children.Add(ClockLogic.GetInstance().MiniActiveTimers.Values.ElementAt(_showTimerIndex));
                _showTimerIndex++;
            }
        }

        /// <summary>
        /// Calls current contest timer TimerClickLogic() method for updating time
        /// </summary>
        public void TimerClickLogic()
        {
            Clock?.TimerClickLogic();
        }

        /// <summary>
        /// Method to update timer label with correct time
        /// Method called by ClockLogic timer
        /// </summary>
        public void OnTimerClick()
        {
            ShowMiniClockOrTimer();
        }

        /// <summary>
        /// Set only the event name
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="type">Not used</param>
        public void SetLabels(string name, string type)
        {
            SetEventName(name);
        }

        /// <summary>
        /// If current time is less than start time show clock, else show timer in minimized clock
        /// </summary>
        private void ShowMiniClockOrTimer()
        {
            if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count == 0 && TimersPanel.Children.Count == 0)
            {
                AddClock();
            }
            else if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count == 0)
            {
                if (ScreenHandler.GetInstance().SelectedScreen == null) return;
                if (_clockInMiniPanel) return;
                TimersPanel.Children.Clear();
                _timer.Stop();
                AddClock();
            }
            else if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count > 0)
            {
                if (_clockInMiniPanel)
                {
                    TimerTickLogic();
                    _clockInMiniPanel = false;
                    Clock = null;
                    _timer.Start();
                }
            }
        }

        /// <summary>
        /// Add clock to timers stack panel
        /// </summary>
        private void AddClock()
        {
            TimersPanel.Children.Clear();
            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            var clock = new MiniContestTimer(ScreenHandler.GetInstance().GetSelectedScreenArea().Width, true)
            {
                Name = " "
            };
            TimersPanel.Children.Add(clock);
            _clockInMiniPanel = true;
            Clock = clock;
        }

        /// <summary>
        /// Method sets chosen image to logo image control, best used for rectangle logo
        /// </summary>
        /// <param name="image">Image to be shown</param>
        public void SetImage(BitmapImage image)
        {
            LogoImage.Source = image;
        }

        /// <summary>
        /// Not implemented, minimized windows don't show QR code image
        /// </summary>
        /// <param name="image"></param>
        public void SetCodeImage(BitmapSource image)
        {
        }

        /// <summary>
        /// Not implemented, minimized windows don't use alignments
        /// </summary>
        /// <param name="alignment"></param>
        public void SetChildren(UserControl alignment)
        {
        }
    }
}
