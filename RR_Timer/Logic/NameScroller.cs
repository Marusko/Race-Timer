using System;
using System.Windows.Controls;

namespace Race_timer.Logic
{
    /// <summary>
    /// Scrolls the content of a ScrollViewer horizontally when it is too long to fit,
    /// waiting a moment at the beginning and at the end before starting over
    /// Used for the event name in minimized clock windows and for the contest name in mini timers
    /// </summary>
    public class NameScroller
    {
        private const int ScrollTimerMillis = 10;

        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly ScrollViewer _scrollViewer;
        private readonly int _delay;
        private readonly int _step;

        private int _stateOfScroll = ClockLogic.ScrollBegin;
        private int _currentTime;
        private int _currentDelay;

        /// <summary>
        /// Prepares the timer, does not start it
        /// </summary>
        /// <param name="scrollViewer">Scroll viewer around the text to scroll</param>
        /// <param name="delay">How many 10 ms ticks to wait at both ends before moving on</param>
        /// <param name="step">Pixels to scroll per tick, so step * 100 pixels per second</param>
        public NameScroller(ScrollViewer scrollViewer, int delay, int step)
        {
            _scrollViewer = scrollViewer;
            _delay = delay;
            _step = step;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, ScrollTimerMillis);
        }

        /// <summary>
        /// Starts scrolling
        /// </summary>
        public void Start()
        {
            _timer.Start();
        }

        /// <summary>
        /// Stops scrolling, the content stays where it is
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Puts the content back to the beginning without stopping the timer,
        /// for when the text changed or is about to be shown again
        /// </summary>
        public void Reset()
        {
            _stateOfScroll = ClockLogic.ScrollBegin;
            _currentTime = 0;
            _currentDelay = 0;
            _scrollViewer.ScrollToLeftEnd();
            _scrollViewer.UpdateLayout();
        }

        /// <summary>
        /// Called by timer, scrolls the content horizontally when it doesn't fit, waits on beginning and end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object? sender, EventArgs e)
        {
            if (_scrollViewer.ScrollableWidth <= 0) return;
            if (_currentDelay != _delay && (_stateOfScroll == ClockLogic.ScrollBegin || _stateOfScroll == ClockLogic.ScrollEnd))
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
                if (_scrollViewer.HorizontalOffset >= _scrollViewer.ScrollableWidth)
                {
                    _currentTime = 0;
                    _stateOfScroll = ClockLogic.ScrollEnd;
                }
                else
                {
                    _currentTime++;
                    _scrollViewer.ScrollToHorizontalOffset(_currentTime * _step);
                    _scrollViewer.UpdateLayout();
                }
            }
            else if (_stateOfScroll == ClockLogic.ScrollEnd)
            {
                _scrollViewer.ScrollToLeftEnd();
                _scrollViewer.UpdateLayout();
                _stateOfScroll = ClockLogic.ScrollBegin;
            }
            _currentDelay = 0;
        }
    }
}
