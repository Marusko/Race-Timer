using System.Windows;
using System;
using Race_timer.Logic;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for MiniContestTimer.xaml
    /// </summary>
    public partial class MiniContestTimer
    {
        //The name is only shown for five seconds at a time, so it waits half a second at both ends
        //and scrolls twice as fast as the event name, which is always on screen
        private const int ScrollDelay = 50;
        private const int ScrollTimes = 2;

        private readonly int _screenWidth;
        private readonly bool _isClock;
        private readonly NameScroller _nameScroller;
        public DateTime StartTime { get; set; }
        public bool Clock => _isClock;

        public new string Name
        {
            get => _name;
            set
            {
                _name = value;
                ContestNameLabel.Content = value;
                _nameScroller.Reset();
            }
        }

        private string _name = "";

        /// <summary>
        /// Initialize component, if isClock call TimerClickLogic() and start timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        /// <param name="isClock">If this timer is clock</param>
        public MiniContestTimer(int screenWidth, bool isClock)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            Unloaded += WindowUnloaded;
            _screenWidth = screenWidth;
            _isClock = isClock;
            _nameScroller = new NameScroller(ContestNameScrollViewer, ScrollDelay, ScrollTimes);
            if (isClock)
            {
                TimerClickLogic();
            }
        }

        /// <summary>
        /// Sets font size and how wide the name can get when component is loaded, and starts scrolling it
        /// Called again every time the minimized window switches back to this contest
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var controlSize = (double)_screenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 3);

            SetNameWidth();
            _nameScroller.Reset();
            _nameScroller.Start();
        }

        /// <summary>
        /// Stops scrolling when the minimized window switches to another contest, the control is
        /// taken out of the panel and there is nothing to scroll
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowUnloaded(object sender, RoutedEventArgs e)
        {
            _nameScroller.Stop();
        }

        /// <summary>
        /// Caps the name at the space actually left for it, so a long contest name scrolls
        /// instead of growing over the event name on the other side of the window
        /// The event name takes the left third of the window, this control gets the rest minus
        /// the 40 the timers panel keeps from the right edge, and minus the time next to the name
        /// </summary>
        private void SetNameWidth()
        {
            //The font size resource was just replaced, lay out again so the time width is the new one
            UpdateLayout();
            var margins = ContestNameScrollViewer.Margin.Left + ContestNameScrollViewer.Margin.Right
                          + ContestTimeLabel.Margin.Left + ContestTimeLabel.Margin.Right;
            var available = _screenWidth * 2.0 / 3 - 40 - ContestTimeLabel.ActualWidth - margins;
            ContestNameScrollViewer.MaxWidth = Math.Max(0, available);
        }

        /// <summary>
        /// Formats timer or clock time to 00:00:00 format
        /// </summary>
        /// <returns>Formatted time to show as timer or clock</returns>
        private string FormatStartTimeOrClock()
        {
            var now = DateTimeHandler.GetInstance().Now;
            TimeSpan time = TimeSpan.FromMilliseconds(!_isClock ? now.Subtract(StartTime).TotalMilliseconds : now.TimeOfDay.TotalMilliseconds);
            var timeString = time.ToString(@"hh\:mm\:ss");
            return timeString;
        }

        /// <summary>
        /// Updates label with correct formatted time
        /// </summary>
        public void TimerClickLogic()
        {
            var c = FormatStartTimeOrClock();
            ContestTimeLabel.Content = c;
        }
    }
}
