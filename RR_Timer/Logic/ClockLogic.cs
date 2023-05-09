﻿using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Race_timer.API;
using Race_timer.ClockUserControl;
using Race_timer.UI;

namespace Race_timer.Logic
{
    /// <summary>
    /// Handles all logic for clocks
    /// </summary>
    public class ClockLogic
    {
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();

        private DateTime _clockDateTime;
        private readonly DateTime _nowDateTime = DateTime.Now;
        private DateTime StartTime { get; set; }
        private string? EventName { get; set; }
        private string? EventType { get; set; }
        public BitmapImage? LogoImage { get; set; }
        public BitmapSource? CodeImage { get; set; }

        private Window? _clockWindow;
        public CodeWindowForMinimized? CodeWindowForMinimized { get; set; }
        public MainWindow MainWindow { get; }
        private readonly ScreenHandler _screenHandler;
        private LinkHandler? _linkHandler;
        private readonly ObservableCollection<string> _timerAlignmentNames = new();
        public UserControl? SelectedAlignment { get; set; }

        private DateTime _showCodeTime;
        private DateTime _hideCodeTime;
        private DateTime _showTime;

        /// <summary>
        /// Sets the main window, screen handler and alignment to top
        /// </summary>
        /// <param name="mw">Previously created main window</param>
        /// <param name="sh">Previously created screen handler</param>
        public ClockLogic(MainWindow mw, ScreenHandler sh)
        {
            MainWindow = mw;
            _screenHandler = sh;
            SetAlignmentList();
            if (_screenHandler.SelectedScreen != null)
                SelectedAlignment = new TimerTop(_screenHandler.SelectedScreen.WorkingArea.Width);
        }

        /// <summary>
        /// Creates list of different alignment names for timer
        /// </summary>
        private void SetAlignmentList()
        {
            _timerAlignmentNames.Add("Timer on top");
            _timerAlignmentNames.Add("Timer on left");
            _timerAlignmentNames.Add("Timer on right");
        }

        /// <summary>
        /// Sets the called method for timer, sets the interval and starts the timer
        /// </summary>
        public void StartTimer()
        {
            _timer.Tick += ClockTick;
            _timer.Interval = TimeSpan.FromMilliseconds(950);
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
            _linkHandler?.StopTimer();
        }
        /// <summary>
        /// Method called by timer, calls method in windows to update clocks
        /// If the timer is small and QR code is enabled, calls method MiniCodeShow()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClockTick(object? sender, EventArgs e)
        {
            if (_clockWindow == null) return;
            if (_clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).OnTimerClick();
            }
            else if (_clockWindow.GetType() == typeof(MiniClockWindow))
            {
                ((MiniClockWindow)_clockWindow).OnTimerClick();
                MiniCodeShow();
            }
        }

        /// <summary>
        /// Shows and hides QR code when timer is small, based on time specified in QR code menu
        /// </summary>
        private void MiniCodeShow()
        {
            if (!MainWindow.IsCodeOnMinimized() || CodeImage == null) return;
            if (CodeWindowForMinimized == null)
            {
                CodeWindowForMinimized = new CodeWindowForMinimized(_screenHandler);
                CodeWindowForMinimized.SetImage(CodeImage);
                _hideCodeTime = DateTime.Now;
                _showTime = DateTime.Now;
                CodeWindowForMinimized.Visibility = Visibility.Hidden;
            }
            else
            {
                if (_hideCodeTime >= _showTime.AddMinutes(MainWindow.GetCodeForMiniTimes().Item2))
                {
                    if (CodeWindowForMinimized.Visibility == Visibility.Hidden)
                    {
                        CodeWindowForMinimized.Show();
                        _showCodeTime = DateTime.Now;
                    }
                    _showCodeTime = _showCodeTime.Add(DateTime.Now - _showCodeTime);
                    if (_showCodeTime < _showTime.AddMinutes(MainWindow.GetCodeForMiniTimes().Item1 +
                                                             MainWindow.GetCodeForMiniTimes().Item2)) return;
                    CodeWindowForMinimized.Hide();
                    _hideCodeTime = DateTime.Now;
                    _showCodeTime = DateTime.Now;
                    _showTime = DateTime.Now;
                }
                else
                {
                    _hideCodeTime = _hideCodeTime.Add(DateTime.Now - _hideCodeTime);
                }
            }
        }

        /// <summary>
        /// When opening clock with API links, opens the clock and sets the clock window,
        /// sets alignment, and both images
        /// </summary>
        /// <param name="mainLink">Link for event name and type</param>
        /// <param name="listLink">Link for finished participants list</param>
        /// <param name="cw">To update or open clock window with API parameters</param>
        public void SetClockWindow(string mainLink, string listLink, ClockWindow cw)
        {
            if (!string.IsNullOrEmpty(mainLink))
            {
                if (string.IsNullOrEmpty(listLink))
                {
                    var warning = new WarningWindow(WarningWindow.ListLinkWarning);
                    warning.ShowDialog();
                    _linkHandler = new LinkHandler(mainLink, this);
                }
                else
                {
                    _linkHandler = new LinkHandler(mainLink, listLink, this);
                }
            }
            else
            {
                var warning = new WarningWindow(WarningWindow.ApiLinkWarning);
                warning.ShowDialog();
                MainWindow.CanOpenTimer = false;
                MainWindow.OnClose();
            }

            _clockWindow = cw;
            if (SelectedAlignment != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).TimerPanel.Children.Clear();
                ((ClockWindow)_clockWindow).TimerPanel.Children.Add(SelectedAlignment);
            }
            if (LogoImage != null)
            {
                if (_clockWindow.GetType() == typeof(ClockWindow))
                {
                    ((ClockWindow)_clockWindow).SetImage(LogoImage);
                }
                else if (_clockWindow.GetType() == typeof(MiniClockWindow))
                {
                    ((MiniClockWindow)_clockWindow).SetImage(LogoImage);
                }
            }
            if (CodeImage != null && SelectedAlignment != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).SetCodeImage(CodeImage);
            }
        }

        /// <summary>
        /// When switching from minimized to fullscreen or backwards,
        /// sets alignment, and both images
        /// </summary>
        /// <param name="cw">To update _clockWindow</param>
        public void SetClockWindow(Window cw)
        {
            _clockWindow = cw;
            if (SelectedAlignment != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).TimerPanel.Children.Clear();
                ((ClockWindow)_clockWindow).TimerPanel.Children.Add(SelectedAlignment);
            }
            if (LogoImage != null)
            {
                if (_clockWindow.GetType() == typeof(ClockWindow))
                {
                    ((ClockWindow)_clockWindow).SetImage(LogoImage);
                }
                else if (_clockWindow.GetType() == typeof(MiniClockWindow))
                {
                    ((MiniClockWindow)_clockWindow).SetImage(LogoImage);
                }
            }
            if (CodeImage != null && SelectedAlignment != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).SetCodeImage(CodeImage);
            }
            if (EventName == null) return;
            if (EventType != null)
                SetLabels(EventName, EventType);
        }
        /// <summary>
        /// When opening clock with manually set name and type, sets the labels,
        /// sets alignment, and both images
        /// </summary>
        /// <param name="cw">To open or update clock window</param>
        /// <param name="name">Name of the event</param>
        /// <param name="type">Type of event</param>
        public void SetClockWindow(Window cw, string name, string type)
        {
            _clockWindow = cw;
            if (SelectedAlignment != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).TimerPanel.Children.Clear();
                ((ClockWindow)_clockWindow).TimerPanel.Children.Add(SelectedAlignment);
            }
            if (LogoImage != null)
            {
                if (_clockWindow.GetType() == typeof(ClockWindow))
                {
                    ((ClockWindow)_clockWindow).SetImage(LogoImage);
                }
                else if (_clockWindow.GetType() == typeof(MiniClockWindow))
                {
                    ((MiniClockWindow)_clockWindow).SetImage(LogoImage);
                }
            }
            if (CodeImage != null && SelectedAlignment != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).SetCodeImage(CodeImage);
            }
            SetLabels(name, type);
        }

        /// <summary>
        /// If current time is less than start time show big clock, else show big timer and small clock under
        /// in fullscreen clock
        /// </summary>
        /// <param name="timer">Timer label from fullscreen clock</param>
        /// <param name="clock">Clock label from fullscreen clock</param>
        public void ShowClockOrTimer(ref Label timer, ref Label clock)
        {
            if (DateTime.Now.Subtract(StartTime).TotalSeconds < 0)
            {
                timer.Content = FormatTime();
                clock.Content = " ";
            }
            else
            {
                clock.Content = FormatTime();
                timer.Content = FormatStartTime();
            }
        }
        /// <summary>
        /// If current time is less than start time show clock, else show timer in minimized clock
        /// </summary>
        /// <param name="timer">Timer label from minimized clock</param>
        public void ShowMiniClockOrTimer(ref Label timer)
        {
            if (DateTime.Now.Subtract(StartTime).TotalSeconds < 0)
            {
                timer.Content = FormatTime();
            }
            else
            {
                _clockDateTime = DateTime.Now;
                timer.Content = FormatStartTime();
            }
        }

        /// <summary>
        /// Check what window is opening and sets the labels, and updates the properties
        /// </summary>
        /// <param name="name">Name of the event</param>
        /// <param name="type">Type of event</param>
        public void SetLabels(string name, string type)
        {
            EventName = name;
            EventType = type;

            if (_clockWindow != null && _clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)_clockWindow).SetEventName(name);
                ((ClockWindow)_clockWindow).SetEventType(type);
            }
            else if (_clockWindow != null && _clockWindow.GetType() == typeof(MiniClockWindow))
            {
                ((MiniClockWindow)_clockWindow).SetEventName(name);
            }
        }

        /// <summary>
        /// Converts string to DateTime, when something goes wrong, show warning window and closes the clock window
        /// </summary>
        /// <param name="s">Time in string format</param>
        public void StringToDateTime(string s)
        {
            var split = s.Split(':');
            if (string.IsNullOrEmpty(split[0]))
            {
                var warning = new WarningWindow(WarningWindow.TimeWarning);
                warning.ShowDialog();
                MainWindow.CanOpenTimer = false;
                return;
            }

            if (split.Length > 1)
            {
                var hour = int.Parse(split[0]);
                var minute = int.Parse(split[1]);
                var second = int.Parse(split[2]);
                try
                {
                    StartTime = new DateTime(_nowDateTime.Year, _nowDateTime.Month, _nowDateTime.Day, hour, minute, second);
                }
                catch (Exception)
                {
                    var warning = new WarningWindow(WarningWindow.TimeWarning);
                    warning.ShowDialog();
                    MainWindow.CanOpenTimer = false;
                }
            }
            else
            {
                var warning = new WarningWindow(WarningWindow.TimeWarning);
                warning.ShowDialog();
                MainWindow.CanOpenTimer = false;
            }
        }

        /// <summary>
        /// Formats clock time to 00:00:00
        /// </summary>
        /// <returns>Formatted time to show as clock</returns>
        private string FormatTime()
        {
            _clockDateTime = DateTime.Now;
            var clock = _clockDateTime.TimeOfDay.ToString();
            var clockLength = clock.LastIndexOf(".", StringComparison.Ordinal);
            var clockString = clock;
            if (clockLength > 0)
            {
                clockString = clock[..clockLength];
            }
            return clockString;
        }
        /// <summary>
        /// Formats timer time to 00:00:00
        /// </summary>
        /// <returns>Formatted time to show as timer</returns>
        private string FormatStartTime()
        {
            var clock = _clockDateTime.Subtract(StartTime).ToString();
            var tmp = clock.LastIndexOf(".", StringComparison.Ordinal);
            var timerClock = clock.Substring(0, tmp);
            return timerClock;
        }

        /// <summary>
        /// Calls minimize method from main widow
        /// </summary>
        public void AutoMinimizeTimer()
        {
            MainWindow.MinimizeTimer();
        }
        public bool IsTimerMinimized()
        {
            return MainWindow.MinimizedTimer;
        }

        public ObservableCollection<string> GetAlignments()
        {
            return _timerAlignmentNames;
        }
    }
}
