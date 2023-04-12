using System.Windows;
using System.Windows.Media.Imaging;
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
            EventNameMini.Text = name;
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

            var controlSize = (double)_screenHandler.SelectedScreen.WorkingArea.Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 3 - 5);

            var controlTimeSize = (double)_screenHandler.SelectedScreen.WorkingArea.Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlTimeFontSize");
            Application.Current.Resources.Add("ControlTimeFontSize", controlTimeSize * 3);

            var controlWidth = (double)_screenHandler.SelectedScreen.WorkingArea.Width / 3 - 50;
            Application.Current.Resources.Remove("ControlWidth");
            Application.Current.Resources.Add("ControlWidth", controlWidth );
        }
        /// <summary>
        /// Method sets chosen image to TimerImage, best used for rectangle logo
        /// </summary>
        /// <param name="image">Image to be shown</param>
        public void SetImage(BitmapImage image)
        {
            TimerImage.Source = image;
        }
    }
}
