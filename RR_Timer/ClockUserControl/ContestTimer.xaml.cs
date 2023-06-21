using System;
using System.DirectoryServices.ActiveDirectory;
using System.Windows;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for ContestTimer.xaml
    /// </summary>
    public partial class ContestTimer
    {
        private readonly int _screenWidth;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private bool _isClock;
        public DateTime StartTime { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                ContestNameLabel.Content = value;
            }
        }

        private string _name;
        public ContestTimer(int screenWidth, bool isClock)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _timer.Interval = new TimeSpan(0, 0, 0, 1);
            _timer.Tick += TimerClick;
            _timer.Start();
            _screenWidth = screenWidth;
            _isClock = isClock;
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var controlSize = (double)_screenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
        }

        /// <summary>
        /// Formats timer or clock time to 00:00:00 format
        /// </summary>
        /// <returns>Formatted time to show as timer or clock</returns>
        private string FormatStartTimeOrClock()
        {
            var now = DateTime.Now;
            string clock;
            clock = !_isClock ? now.Subtract(StartTime).ToString() : now.TimeOfDay.ToString();
            var tmp = clock.LastIndexOf(".", StringComparison.Ordinal);
            clock = clock.Substring(0, tmp);
            return clock;
        }

        private void TimerClick(object? sender, EventArgs e)
        {
            var c = FormatStartTimeOrClock();
            ContestTimeLabel.Content = c;
        }

        public void StopTimer()
        {
            _timer.Stop();
        }
    }
}
