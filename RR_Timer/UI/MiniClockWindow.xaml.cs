using System.Windows;
using RR_Timer.Logic;

namespace RR_Timer.UI
{
    /// <summary>
    /// Interaction logic for MiniClockWindow.xaml
    /// </summary>
    public partial class MiniClockWindow
    {
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;

        public MiniClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            _clockLogic = cl;
            _screenHandler = sh;

            Loaded += WindowLoaded;
        }

        public void SetEventName(string name)
        {
            EventNameMini.Content = name;
        }

        public void OnTimerClick()
        {
            _clockLogic.ShowMiniClockOrTimer(ref TimerMini);
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_screenHandler.SelectedScreen == null) return;
            WindowState = WindowState.Normal;
            Left = _screenHandler.SelectedScreen.WorkingArea.Left;
            Top = _screenHandler.SelectedScreen.WorkingArea.Top;
            Width = _screenHandler.SelectedScreen.WorkingArea.Width;
        }
    }
}
