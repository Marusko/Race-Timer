using System.Windows;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for TimerLeft.xaml
    /// </summary>
    public partial class TimerLeft
    {
        private readonly int _screenWidth;
        public TimerLeft(int screenWidth)
        {
            InitializeComponent();
            Loaded += WindowLoaded;
            _screenWidth = screenWidth;
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            var controlSize = (double)_screenWidth / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
            Application.Current.Resources.Remove("ControlCodeSize");
            Application.Current.Resources.Add("ControlCodeSize", controlSize * 20);
        }
    }
}
