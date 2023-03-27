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

        /// <summary>
        /// Initializes the window, adds method to call after window is loaded
        /// </summary>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public MiniClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            _clockLogic = cl;
            _screenHandler = sh;

            Loaded += WindowLoaded;
        }

        /// <summary>
        /// Sets the label to correct name
        /// </summary>
        /// <param name="name">Event name to show</param>
        public void SetEventName(string name)
        {
            EventNameMini.Content = name;
        }

        /// <summary>
        /// Method to update timer label with correct time
        /// </summary>
        public void OnTimerClick()
        {
            _clockLogic.ShowMiniClockOrTimer(ref TimerMini);
        }

        /// <summary>
        /// Method called after window is loaded, sets the position, state and width of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
