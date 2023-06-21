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
        private const int ScrollTimes = 1000;
        private const int TimerMillis = 10;

        public TimerTop(int screenWidth)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _screenWidth = screenWidth;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, TimerMillis);
            _timer.Start();
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var controlSize = (double)_screenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
        }

        public void StopTimer()
        {
            _timer.Stop();
        }

        private void TimerTick(object? sender, EventArgs e)
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
                    if (_currentTime == ScrollTimes)
                    {
                        _currentTime = 0;
                        _stateOfScroll = ClockLogic.ScrollEnd;
                    }
                    else
                    {
                        _currentTime++;
                        TimerScrollViewer.ScrollToHorizontalOffset(_currentTime * (TimerScrollViewer.ScrollableWidth/ScrollTimes));
                    }
                }
                else if (_stateOfScroll == ClockLogic.ScrollEnd)
                {
                    TimerScrollViewer.ScrollToLeftEnd();
                    _stateOfScroll = ClockLogic.ScrollBegin;
                }
                _currentDelay = 0;
            }
        }
    }
}
