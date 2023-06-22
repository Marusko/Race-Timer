using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using Race_timer.Logic;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for MiniClockWindow.xaml
    /// </summary>
    public partial class MiniClockWindow
    {
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private int _showTimerIndex;

        private const int TimerShownForSeconds = 5;

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
            Closed += StopTimer;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, TimerShownForSeconds);
            TimerTickLogic();
            _timer.Start();
        }

        /// <summary>
        /// Sets the label to correct name
        /// </summary>
        /// <param name="name">Event name to show</param>
        public void SetEventName(string name)
        {
            EventNameMini.Text = name;
        }

        private void StopTimer(object? sender, EventArgs e)
        {
            _timer.Stop();
        }

        private void TimerTick(object? sender, EventArgs e)
        {
            TimerTickLogic();
        }

        private void TimerTickLogic()
        {
            if (_clockLogic.MiniActiveTimers.Values.Count > 0)
            {
                if (_showTimerIndex >= _clockLogic.MiniActiveTimers.Values.Count)
                {
                    _showTimerIndex = 0;
                }
                TimerStackPanel.Children.Clear();
                TimerStackPanel.Children.Add(_clockLogic.MiniActiveTimers.Values.ElementAt(_showTimerIndex));
                _showTimerIndex++;
            }
        }

        /// <summary>
        /// Method to update timer label with correct time
        /// </summary>
        public void OnTimerClick()
        {
            _clockLogic.ShowMiniClockOrTimer(ref TimerStackPanel);
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

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)_screenHandler.SelectedScreen.WorkingArea.Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 3 - 5);

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
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
