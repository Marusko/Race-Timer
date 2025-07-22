using System;
using System.Windows;
using Race_timer.Logic;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for TimerTop.xaml
    /// </summary>
    public partial class TimerTop
    {
        private readonly int _screenWidth;

        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private int _stateOfScroll = ClockLogic.ScrollBegin;
        private int _currentTime;
        private int _currentDelay;

        private const int ScrollDelay = 500;
        private const int ScrollTimes = 1;
        private const int TimerMillis = 10;

        /// <summary>
        /// Initialize component, start timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        public TimerTop(int screenWidth)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _screenWidth = screenWidth;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, TimerMillis);
            _timer.Start();
        }

        /// <summary>
        /// Sets font size when window is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var controlSize = (double)_screenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Set the top margin according to length of event name
        /// </summary>
        /// <param name="nameLength"></param>
        public void SetTopMargin(int nameLength)
        {
            if (nameLength is <= 26 and > 0)
            {
                TimerGrid.Margin = new Thickness(0, 10, 0, 0);
            }

            if (nameLength == 0)
            {
                TimerGrid.Margin = new Thickness(0, -50, 0, 0);
            }
        }

        /// <summary>
        /// Called by timer, scrolls the contest timers horizontally, waits on beginning and end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object? sender, EventArgs e)
        {
            if (TimerStackPanel.ActualWidth > _screenWidth)
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
                        if (TimerScrollViewer.HorizontalOffset >= TimerScrollViewer.ScrollableWidth)
                        {
                            _currentTime = 0;
                            _stateOfScroll = ClockLogic.ScrollEnd;
                        }
                        else
                        {
                            _currentTime++;
                            TimerScrollViewer.ScrollToHorizontalOffset(_currentTime * ScrollTimes);
                            TimerScrollViewer.UpdateLayout();
                        }
                    }
                    else if (_stateOfScroll == ClockLogic.ScrollEnd)
                    {
                        TimerScrollViewer.ScrollToLeftEnd();
                        TimerScrollViewer.UpdateLayout();
                        _stateOfScroll = ClockLogic.ScrollBegin;
                    }
                    _currentDelay = 0;
                }
            }
        }
    }
}
