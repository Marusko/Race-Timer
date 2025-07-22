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
        private readonly int _screenWidth;
        private readonly bool _isClock;
        public DateTime StartTime { get; set; }
        public bool Clock => _isClock;

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

        /// <summary>
        /// Initialize component, if isClock call TimerClickLogic() and start timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        /// <param name="isClock">If this timer is clock</param>
        public MiniContestTimer(int screenWidth, bool isClock)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _screenWidth = screenWidth;
            _isClock = isClock;
            if (isClock)
            {
                TimerClickLogic();
            }
        }

        /// <summary>
        /// Sets font size when component is loaded
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
            var now = DateTimeHandler.GetInstance().Now;
            TimeSpan time = TimeSpan.FromSeconds(!_isClock ? now.Subtract(StartTime).TotalSeconds : now.TimeOfDay.TotalSeconds);
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
