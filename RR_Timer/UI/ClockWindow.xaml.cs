using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Race_timer.ClockUserControl;
using Race_timer.Logic;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow
    {
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;

        /// <summary>
        /// Initialize and open the window, sets label contents, converts time string to DateTime and adds method
        /// to call after window is loaded
        /// </summary>
        /// <param name="name">Event name to show</param>
        /// <param name="type">Event type</param>
        /// <param name="startTime">Event start time in 00:00:00 format</param>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public ClockWindow(string name, string type, string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            
            EventNameLabel.Text = name;
            EventTypeLabel.Content = type;
            _clockLogic = cl;
            _screenHandler = sh;

            _clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;
            Closed += OnClose;
        }
        /// <summary>
        /// Initialize and open the window, converts time string to DateTime and adds method to call after window is loaded
        /// </summary>
        /// <param name="startTime">Event start time in 00:00:00 format</param>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public ClockWindow(string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            _clockLogic = cl;
            _screenHandler = sh;
            _clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;
            Closed += OnClose;
        }
        /// <summary>
        /// Initialize and open the window, used when switching between fullscreen and minimized clock window
        /// </summary>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public ClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            _clockLogic = cl;
            _screenHandler = sh;

            Loaded += WindowLoaded;
            Closed += OnClose;
        }
        /// <summary>
        /// Method called after window is loaded, sets the position, state and width and height of window
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
            Height = _screenHandler.SelectedScreen.WorkingArea.Height;
            WindowState = WindowState.Maximized;

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)_screenHandler.SelectedScreen.WorkingArea.Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
        }

        private void OnClose(object? sender, EventArgs e)
        {
            if (TimerPanel.Children[0].GetType() == typeof(TimerTop))
            {
                ((TimerTop)TimerPanel.Children[0]).StopTimer();
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerLeft))
            {
                ((TimerLeft)TimerPanel.Children[0]).StopTimer();
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerRight))
            {
                ((TimerRight)TimerPanel.Children[0]).StopTimer();
            }
            
        }

        /// <summary>
        /// Sets the event name to label
        /// </summary>
        /// <param name="name">Event name to show</param>
        public void SetEventName(string name)
        {
            EventNameLabel.Text = name;
        }

        /// <summary>
        /// Sets the event type to label
        /// </summary>
        /// <param name="type">Event type to show</param>
        public void SetEventType(string type)
        {
            EventTypeLabel.Content = type;
        }

        /// <summary>
        /// Method to update timer and type label with correct time
        /// </summary>
        public void OnTimerClick()
        {
            if (TimerPanel.Children[0].GetType() == typeof(TimerTop))
            {
                _clockLogic.ShowClockOrTimer(ref ((TimerTop)TimerPanel.Children[0]).TimerStackPanel, ref ((TimerTop)TimerPanel.Children[0]).MainClockLabel);
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerLeft))
            {
                _clockLogic.ShowClockOrTimer(ref ((TimerLeft)TimerPanel.Children[0]).TimerStackPanel, ref ((TimerLeft)TimerPanel.Children[0]).MainClockLabel);
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerRight))
            {
                _clockLogic.ShowClockOrTimer(ref ((TimerRight)TimerPanel.Children[0]).TimerStackPanel, ref ((TimerRight)TimerPanel.Children[0]).MainClockLabel);
            }
        }

        /// <summary>
        /// Method sets chosen image to TimerImage, best used for rectangle logo
        /// </summary>
        /// <param name="image">Image to be shown</param>
        public void SetImage(BitmapImage image)
        {
            if (TimerPanel.Children[0].GetType() == typeof(TimerTop))
            {
                ((TimerTop)TimerPanel.Children[0]).TimerImage.Source = image;
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerLeft))
            {
                ((TimerLeft)TimerPanel.Children[0]).TimerImage.Source = image;
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerRight))
            {
                ((TimerRight)TimerPanel.Children[0]).TimerImage.Source = image;
            }
        }

        /// <summary>
        /// Method sets chosen/generated image/QR code to CodeImage
        /// </summary>
        /// <param name="image">Image/QR code to be shown</param>
        public void SetCodeImage(BitmapSource image)
        {
            if (TimerPanel.Children[0].GetType() == typeof(TimerLeft))
            {
                ((TimerLeft)TimerPanel.Children[0]).CodeImage.Source = image;
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerRight))
            {
                ((TimerRight)TimerPanel.Children[0]).CodeImage.Source = image;
            }
        }
    }
    
}
