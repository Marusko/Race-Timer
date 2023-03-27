using System.Windows;
using RR_Timer.Logic;

namespace RR_Timer.UI
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow
    {
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;

        public ClockWindow(string name, string type, string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            
            EventNameLabel.Content = name;
            EventTypeLabel.Content = type;
            _clockLogic = cl;
            _screenHandler = sh;

            _clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;
        }
        public ClockWindow(string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            _clockLogic = cl;
            _screenHandler = sh;
            _clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;
        }
        public ClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            _clockLogic = cl;
            _screenHandler = sh;

            Loaded += WindowLoaded;
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_screenHandler.SelectedScreen == null) return;
            WindowState = WindowState.Normal;
            Left = _screenHandler.SelectedScreen.WorkingArea.Left;
            Top = _screenHandler.SelectedScreen.WorkingArea.Top;
            Width = _screenHandler.SelectedScreen.WorkingArea.Width;
            Height = _screenHandler.SelectedScreen.WorkingArea.Height;
            WindowState = WindowState.Maximized;
        }

        public void SetEventName(string name)
        {
            EventNameLabel.Content = name;
        }

        public void SetEventType(string type)
        {
            EventTypeLabel.Content = type;
        }

        public void OnTimerClick()
        {
            _clockLogic.ShowClockOrTimer(ref TimerClockLabel, ref MainClockLabel);
        }
    }
    
}
