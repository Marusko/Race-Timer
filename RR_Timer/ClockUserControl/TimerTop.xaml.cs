using RR_Timer.Logic;
using System.Windows;

namespace RR_Timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for TimerTop.xaml
    /// </summary>
    public partial class TimerTop
    {
        private readonly int _screenWidth;
        public TimerTop(int screenWidth)
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
