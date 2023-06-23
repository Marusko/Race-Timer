using System.Windows;
using System;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for MiniContestTimer.xaml
    /// </summary>
    public partial class MiniContestTimer
    {
        private readonly int _screenWidth;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly bool _isClock;
        public DateTime StartTime { get; init; }

        public new string Name
        {
            get => _name;
            set
            {
                _name = value;
                ContestNameLabel.Content = value;
            }
        }

        private string _name = "";
        public MiniContestTimer(int screenWidth, bool isClock)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Tick += TimerClick;
            _timer.Start();
            _screenWidth = screenWidth;
            _isClock = isClock;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var controlSize = (double)_screenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 3);
        }

        /// <summary>
        /// Formats timer or clock time to 00:00:00 format
        /// </summary>
        /// <returns>Formatted time to show as timer or clock</returns>
        private string FormatStartTimeOrClock()
        {
            var now = DateTime.Now;
            TimeSpan time = TimeSpan.FromSeconds(!_isClock ? now.Subtract(StartTime).TotalSeconds : now.TimeOfDay.TotalSeconds);
            var timeString = time.ToString(@"hh\:mm\:ss");
            return timeString;
        }

        /// <summary>
        /// Updates label with correct formatted time
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerClick(object? sender, EventArgs e)
        {
            var c = FormatStartTimeOrClock();
            ContestTimeLabel.Content = c;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }
    }
}
