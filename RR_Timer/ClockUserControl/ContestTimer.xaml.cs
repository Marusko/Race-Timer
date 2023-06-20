using System.Windows;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for ContestTimer.xaml
    /// </summary>
    public partial class ContestTimer
    {
        private readonly int _screenWidth;
        public ContestTimer(int screenWidth)
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
        }
    }
}
