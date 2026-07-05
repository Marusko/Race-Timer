using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Race_timer.ClockUserControl;
using Race_timer.Logic;
using Race_timer.Logic.Interfaces;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow:IClockWindow
    {
        private bool _clockInPanel;
        public ContestTimer? Clock { get; set; }

        /// <summary>
        /// Initialize and open the window, used when switching between fullscreen and minimized clock window
        /// Initialize and open the window, converts time string to DateTime and adds method to call after window is loaded
        /// </summary>
        public ClockWindow()
        {
            InitializeComponent();

            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            WindowState = WindowState.Minimized;
            Left = ScreenHandler.GetInstance().SelectedScreen?.WorkingArea.Left ?? 0;
            Top = ScreenHandler.GetInstance().SelectedScreen?.WorkingArea.Top ?? 0;

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
            WindowState = WindowState.Maximized;

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)ScreenHandler.GetInstance().GetSelectedScreenArea().Width / 12 / 3 * 2 / 5 * 0.7;
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
            if (ClockLogic.GetInstance().EventName is { Length: <= 26 })
            {
                return;
            }
            switch (Alignment())
            {
                case TimerLeft left:
                    left.TimerScrollViewer.MaxHeight = 400;
                    break;
                case TimerRight right:
                    right.TimerScrollViewer.MaxHeight = 400;
                    break;
            }
        }

        /// <summary>
        /// Currently set alignment UserControl, or null when none is set yet
        /// </summary>
        private UIElement? Alignment()
        {
            return TimerPanel.Children.Count > 0 ? TimerPanel.Children[0] : null;
        }

        /// <summary>
        /// When closing, stop the scroll timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object? sender, EventArgs e)
        {
            switch (Alignment())
            {
                case TimerTop top:
                    top.StopTimer();
                    break;
                case TimerLeft left:
                    left.StopTimer();
                    break;
                case TimerRight right:
                    right.StopTimer();
                    break;
            }
        }

        /// <summary>
        /// Sets the event name to label
        /// </summary>
        /// <param name="name">Event name to show</param>
        private void SetEventName(string name)
        {
            EventNameLabel.Text = name;
        }

        /// <summary>
        /// Sets the event type to label
        /// </summary>
        /// <param name="type">Event type to show</param>
        private void SetEventType(string type)
        {
            EventTypeLabel.Content = type;
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        public void TimerTickLogic()
        {
            
        }

        /// <summary>
        /// Calls clock TimerClickLogic() method for updating time
        /// </summary>
        public void TimerClickLogic()
        {
            Clock?.TimerClickLogic();
        }

        /// <summary>
        /// Method to update timer and type label with correct time
        /// Method called by ClockLogic timer
        /// </summary>
        public void OnTimerClick()
        {
            switch (Alignment())
            {
                case TimerTop top:
                    ShowClockOrTimer(ref top.TimerStackPanel, ref top.MainClockLabel);
                    break;
                case TimerLeft left:
                    ShowClockOrTimer(ref left.TimerStackPanel, ref left.MainClockLabel);
                    break;
                case TimerRight right:
                    ShowClockOrTimer(ref right.TimerStackPanel, ref right.MainClockLabel);
                    break;
            }
        }

        /// <summary>
        /// Method for setting the event name and type labels
        /// </summary>
        /// <param name="name">Event name</param>
        /// <param name="type">Event type</param>
        public void SetLabels(string name, string type)
        {
            SetEventName(name);
            SetEventType(type);
        }

        /// <summary>
        /// If current time is less than start time show big clock, else show big timer and small clock under
        /// in fullscreen clock
        /// </summary>
        /// <param name="timers">Timer StackPanel from fullscreen clock</param>
        /// <param name="clock">Clock label from fullscreen clock</param>
        private void ShowClockOrTimer(ref StackPanel timers, ref Label clock)
        {
            if (ClockLogic.GetInstance().ActiveTimers.Values.Count == 0 && timers.Children.Count == 0)
            {
                AddClock(ref timers, ref clock);
            }
            else if (ClockLogic.GetInstance().ActiveTimers.Values.Count > 0 && ClockLogic.GetInstance().ActiveTimers.Values.Count == timers.Children.Count)
            {
                if (_clockInPanel || (timers.Children.Count == 1 && ((ContestTimer)timers.Children[0]).Clock))
                {
                    timers.Children.Clear();
                    _clockInPanel = false;
                    Clock = null;
                }
                clock.Content = FormatTime();
                foreach (var contestTimer in ClockLogic.GetInstance().ActiveTimers.Values)
                {
                    if (!timers.Children.Contains(contestTimer))
                    {
                        timers.Children.Add(contestTimer);
                    }
                }
            }
            else if (ClockLogic.GetInstance().ActiveTimers.Values.Count != timers.Children.Count)
            {
                //keep the clock only while no contest timer is running, otherwise it must be replaced
                if (timers.Children.Count == 1 && ((ContestTimer)timers.Children[0]).Clock
                    && ClockLogic.GetInstance().ActiveTimers.Values.Count == 0)
                {
                    return;
                }
                timers.Children.Clear();
                if (ClockLogic.GetInstance().ActiveTimers.Values.Count == 0)
                {
                    AddClock(ref timers, ref clock);
                }
                else
                {
                    clock.Content = FormatTime();
                    _clockInPanel = false;
                    Clock = null;
                    foreach (var contestTimer in ClockLogic.GetInstance().ActiveTimers.Values)
                    {
                        if (!timers.Children.Contains(contestTimer))
                        {
                            timers.Children.Add(contestTimer);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Add clock to timers stack panel
        /// </summary>
        /// <param name="timers">Timer StackPanel from fullscreen clock</param>
        /// <param name="clock">Clock label from fullscreen clock</param>
        private void AddClock(ref StackPanel timers, ref Label clock)
        {
            clock.Content = " ";
            timers.Children.Clear();
            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            var clockTimer = new ContestTimer(ScreenHandler.GetInstance().GetSelectedScreenArea().Width, true,
                ClockLogic.GetInstance().EventName?.Length ?? 0)
            {
                Name = " "
            };
            timers.Children.Add(clockTimer);
            _clockInPanel = true;
            Clock = clockTimer;
        }

        /// <summary>
        /// Formats clock time to 00:00:00
        /// </summary>
        /// <returns>Formatted time to show as clock</returns>
        private string FormatTime()
        {
            var now = DateTimeHandler.GetInstance().Now;
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
            switch (Alignment())
            {
                case TimerTop top:
                    top.TimerImage.Source = image;
                    break;
                case TimerLeft left:
                    left.TimerImage.Source = image;
                    break;
                case TimerRight right:
                    right.TimerImage.Source = image;
                    break;
            }
        }

        /// <summary>
        /// Method sets chosen/generated image/QR code to CodeImage
        /// </summary>
        /// <param name="image">Image/QR code to be shown</param>
        public void SetCodeImage(BitmapSource image)
        {
            switch (Alignment())
            {
                case TimerLeft left:
                    left.CodeImage.Source = image;
                    break;
                case TimerRight right:
                    right.CodeImage.Source = image;
                    break;
            }
        }

        /// <summary>
        /// Method for setting alignment of big clock window
        /// </summary>
        /// <param name="alignment">Selected alignment</param>
        public void SetChildren(UserControl alignment)
        {
            TimerPanel.Children.Clear();
            TimerPanel.Children.Add(alignment);
        }
    }
    
}
