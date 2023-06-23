using System;
using System.Windows;
using System.Windows.Controls;
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
        private bool _clockInPanel;

        /// <summary>
        /// Initialize and open the window, used when switching between fullscreen and minimized clock window
        /// Initialize and open the window, converts time string to DateTime and adds method to call after window is loaded
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

            SetTimersMaxHeight();
        }

        /// <summary>
        /// If alignments are left and right, and Event name is longer than 26 characters when it starts wrapping,
        /// timers stack panel needs to be smaller
        /// </summary>
        private void SetTimersMaxHeight()
        {
            if (_clockLogic.EventName is { Length: <= 26 })
            {
                return;
            }
            if (TimerPanel.Children[0].GetType() == typeof(TimerLeft))
            {
                ((TimerLeft)TimerPanel.Children[0]).TimerScrollViewer.MaxHeight = 300;
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerRight))
            {
                ((TimerRight)TimerPanel.Children[0]).TimerScrollViewer.MaxHeight = 300;
            }
        }

        /// <summary>
        /// When closing, stop the scroll timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                ShowClockOrTimer(ref ((TimerTop)TimerPanel.Children[0]).TimerStackPanel, ref ((TimerTop)TimerPanel.Children[0]).MainClockLabel);
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerLeft))
            {
                ShowClockOrTimer(ref ((TimerLeft)TimerPanel.Children[0]).TimerStackPanel, ref ((TimerLeft)TimerPanel.Children[0]).MainClockLabel);
            }
            else if (TimerPanel.Children[0].GetType() == typeof(TimerRight))
            {
                ShowClockOrTimer(ref ((TimerRight)TimerPanel.Children[0]).TimerStackPanel, ref ((TimerRight)TimerPanel.Children[0]).MainClockLabel);
            }
        }

        /// <summary>
        /// If current time is less than start time show big clock, else show big timer and small clock under
        /// in fullscreen clock
        /// </summary>
        /// <param name="timers">Timer StackPanel from fullscreen clock</param>
        /// <param name="clock">Clock label from fullscreen clock</param>
        private void ShowClockOrTimer(ref StackPanel timers, ref Label clock)
        {
            if (_clockLogic.ActiveTimers.Values.Count == 0 && timers.Children.Count == 0)
            {
                clock.Content = " ";
                timers.Children.Clear();
                if (_screenHandler.SelectedScreen == null) return;
                timers.Children.Add(new ContestTimer(_screenHandler.SelectedScreen.WorkingArea.Width, true)
                {
                    Name = " "
                });
                _clockInPanel = true;
            }
            else if (_clockLogic.ActiveTimers.Values.Count > 0)
            {
                if (_clockInPanel)
                {
                    timers.Children.Clear();
                    _clockInPanel = false;
                }
                clock.Content = FormatTime();
                foreach (var contestTimer in _clockLogic.ActiveTimers.Values)
                {
                    if (!timers.Children.Contains(contestTimer))
                    {
                        timers.Children.Add(contestTimer);
                    }
                }
            }
        }

        /// <summary>
        /// Formats clock time to 00:00:00
        /// </summary>
        /// <returns>Formatted time to show as clock</returns>
        private string FormatTime()
        {
            var now = DateTime.Now;
            TimeSpan time = TimeSpan.FromSeconds(now.TimeOfDay.TotalSeconds);
            var timeString = time.ToString(@"hh\:mm\:ss");
            return timeString;
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
