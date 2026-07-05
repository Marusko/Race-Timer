using System;
using System.Windows;
using System.Windows.Controls;
using Race_timer.Logic;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Shared logic for fullscreen clock timer alignments,
    /// sets font resources and scrolls the contest timers when they don't fit
    /// </summary>
    public abstract class TimerAlignmentBase : UserControl
    {
        protected readonly int ScreenWidth;

        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private int _stateOfScroll = ClockLogic.ScrollBegin;
        private int _currentTime;
        private int _currentDelay;

        private const int ScrollDelay = 500;
        private const int ScrollTimes = 1;
        private const int TimerMillis = 10;

        /// <summary>
        /// Panel with contest timers or clock
        /// </summary>
        public abstract StackPanel Timers { get; }

        /// <summary>
        /// Label with small clock shown while contest timers are running
        /// </summary>
        public abstract Label MainClock { get; }

        /// <summary>
        /// Image control for logo
        /// </summary>
        public abstract Image Logo { get; }

        /// <summary>
        /// Image control for QR code, only in alignments that show it
        /// </summary>
        public virtual Image? Code => null;

        /// <summary>
        /// Scroll viewer around contest timers
        /// </summary>
        protected abstract ScrollViewer Scroller { get; }

        /// <summary>
        /// Sets screen width, starts the scroll timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        protected TimerAlignmentBase(int screenWidth)
        {
            ScreenWidth = screenWidth;
            Loaded += WindowLoaded;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, TimerMillis);
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }

        /// <summary>
        /// If alignments are left and right, and Event name is longer than 26 characters when it starts wrapping,
        /// timers stack panel needs to be smaller, only vertical alignments need it
        /// </summary>
        public virtual void LimitTimersHeight()
        {
        }

        /// <summary>
        /// Sets font sizes when component is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)ScreenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
            SetAdditionalResources(controlSize);
        }

        /// <summary>
        /// For resources needed only by some alignments
        /// </summary>
        /// <param name="controlSize">Base control size computed from screen width</param>
        protected virtual void SetAdditionalResources(double controlSize)
        {
        }

        /// <summary>
        /// If contest timers overflow and need scrolling
        /// </summary>
        protected abstract bool NeedsScroll();

        /// <summary>
        /// Current scroll offset in the scrolled direction
        /// </summary>
        protected abstract double ScrollOffset();

        /// <summary>
        /// Maximum scroll offset in the scrolled direction
        /// </summary>
        protected abstract double ScrollableExtent();

        /// <summary>
        /// Scrolls to offset in the scrolled direction
        /// </summary>
        /// <param name="offset">Offset to scroll to</param>
        protected abstract void ScrollToOffset(double offset);

        /// <summary>
        /// Scrolls back to the beginning
        /// </summary>
        protected abstract void ScrollToStart();

        /// <summary>
        /// Called by timer, scrolls the contest timers, waits on beginning and end
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object? sender, EventArgs e)
        {
            if (!NeedsScroll()) return;
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
                if (ScrollOffset() >= ScrollableExtent())
                {
                    _currentTime = 0;
                    _stateOfScroll = ClockLogic.ScrollEnd;
                }
                else
                {
                    _currentTime++;
                    ScrollToOffset(_currentTime * ScrollTimes);
                    Scroller.UpdateLayout();
                }
            }
            else if (_stateOfScroll == ClockLogic.ScrollEnd)
            {
                ScrollToStart();
                Scroller.UpdateLayout();
                _stateOfScroll = ClockLogic.ScrollBegin;
            }
            _currentDelay = 0;
        }
    }

    /// <summary>
    /// Shared logic for alignments with vertically stacked contest timers (left and right)
    /// </summary>
    public abstract class VerticalTimerAlignmentBase : TimerAlignmentBase
    {
        /// <summary>
        /// Sets screen width, starts the scroll timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        protected VerticalTimerAlignmentBase(int screenWidth) : base(screenWidth)
        {
        }

        public override void LimitTimersHeight()
        {
            Scroller.MaxHeight = 400;
        }

        protected override void SetAdditionalResources(double controlSize)
        {
            Application.Current.Resources.Remove("ControlCodeSize");
            Application.Current.Resources.Add("ControlCodeSize", controlSize * 20);
        }

        protected override bool NeedsScroll() => Timers.ActualHeight > Scroller.MaxHeight;
        protected override double ScrollOffset() => Scroller.VerticalOffset;
        protected override double ScrollableExtent() => Scroller.ScrollableHeight;
        protected override void ScrollToOffset(double offset) => Scroller.ScrollToVerticalOffset(offset);
        protected override void ScrollToStart() => Scroller.ScrollToTop();
    }
}
