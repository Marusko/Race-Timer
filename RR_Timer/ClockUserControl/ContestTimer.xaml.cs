using System;
using System.Windows;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for ContestTimer.xaml
    /// </summary>
    public partial class ContestTimer
    {
        private readonly int _screenWidth;
        private readonly bool _isClock;
        private readonly int _nameLength;
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
        /// <param name="nameLength">Length of event name</param>
        public ContestTimer(int screenWidth, bool isClock, int nameLength)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _screenWidth = screenWidth;
            _isClock = isClock;
            _nameLength = nameLength;
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
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
            if (_isClock && _nameLength <= 26)
            {
                ContestTimeLabel.FontSize = 300;
            }
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
        public void TimerClickLogic()
        {
            var c = FormatStartTimeOrClock();
            ContestTimeLabel.Content = c;
        }
    }
}
