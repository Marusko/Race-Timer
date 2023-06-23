using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
        public const int ScrollBegin = -1;
        public const int Scrolling = 0;
        public const int ScrollEnd = 1;

        private readonly System.Windows.Threading.DispatcherTimer _timer = new();

        private DateTime _clockDateTime;
        private readonly DateTime _nowDateTime = DateTime.Now;
        private Dictionary<string, DateTime> StartTimes { get; }
        public Dictionary<string, ContestTimer> ActiveTimers { get; }
        public Dictionary<string, MiniContestTimer> MiniActiveTimers { get; }
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

        private bool _clockInPanel;
        private bool _clockInMiniPanel;

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
            StartTimes = new Dictionary<string, DateTime>();
            ActiveTimers = new Dictionary<string, ContestTimer>();
            MiniActiveTimers = new Dictionary<string, MiniContestTimer>();
            SetAlignmentList();
            if (_screenHandler.SelectedScreen != null)
                SelectedAlignment = new TimerTop(_screenHandler.SelectedScreen.WorkingArea.Width);
        }

        public void ProcessStartTimes()
        {
            StartTimes.Clear();
            ActiveTimers.Clear();
            _clockInPanel = true;
            foreach (var time in MainWindow.StartTimes)
            {
                StartTimes.Add(time.Key, StringToDateTime(time.Value));
            }
            MainWindow.StartTimes.Clear();
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
            _timer.Interval = new TimeSpan(0, 0, 1);
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
            CheckTimers();
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

        public void CheckTimers(bool isSmall = false)
        {
            for (int i = 0;i < StartTimes.Count; i++)
            {
                if (StartTimes.Values.ElementAt(i) < DateTime.Now)
                {
                    if (_screenHandler.SelectedScreen == null) return;

                    if (_clockWindow == null) return;
                    if (_clockWindow.GetType() == typeof(ClockWindow) && !isSmall)
                    {
                        if (MiniActiveTimers.Count > 0)
                        {
                            MiniActiveTimers.Clear();
                        }
                        if (ActiveTimers.ContainsKey(StartTimes.Keys.ElementAt(i)))
                        {
                            continue;
                        }
                        ActiveTimers.Add(StartTimes.Keys.ElementAt(i), new ContestTimer(_screenHandler.SelectedScreen.WorkingArea.Width, false)
                        {
                            Name = StartTimes.Keys.ElementAt(i),
                            StartTime = StartTimes.Values.ElementAt(i)
                        });
                    }
                    else if (_clockWindow.GetType() == typeof(MiniClockWindow) || isSmall)
                    {
                        if (ActiveTimers.Count > 0)
                        {
                            ActiveTimers.Clear();
                        }
                        if (MiniActiveTimers.ContainsKey(StartTimes.Keys.ElementAt(i)))
                        {
                            continue;
                        }
                        MiniActiveTimers.Add(StartTimes.Keys.ElementAt(i), new MiniContestTimer(_screenHandler.SelectedScreen.WorkingArea.Width, false)
                        {
                            Name = StartTimes.Keys.ElementAt(i),
                            StartTime = StartTimes.Values.ElementAt(i)
                        });
                    }
                }
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
                    var warning = new WarningWindow(WarningWindow.CountLinkWarning);
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
        /// <param name="timers">Timer StackPanel from fullscreen clock</param>
        /// <param name="clock">Clock label from fullscreen clock</param>
        public void ShowClockOrTimer(ref StackPanel timers, ref Label clock)
        {
            if (ActiveTimers.Values.Count == 0 && timers.Children.Count == 0)
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
            else if(ActiveTimers.Values.Count > 0) 
            {
                if (_clockInPanel)
                {
                    timers.Children.Clear();
                    _clockInPanel = false;
                }
                clock.Content = FormatTime();
                foreach (var contestTimer in ActiveTimers.Values)
                {
                    if (!timers.Children.Contains(contestTimer))
                    {
                        timers.Children.Add(contestTimer);
                    }
                }
            }
        }
        /// <summary>
        /// If current time is less than start time show clock, else show timer in minimized clock
        /// </summary>
        /// <param name="timers">Timer StackPanel from minimized clock</param>
        public void ShowMiniClockOrTimer(ref StackPanel timers)
        {
            if (MiniActiveTimers.Values.Count == 0 && timers.Children.Count == 0)
            {
                timers.Children.Clear();
                if (_screenHandler.SelectedScreen == null) return;
                timers.Children.Add(new MiniContestTimer(_screenHandler.SelectedScreen.WorkingArea.Width, true)
                {
                    Name = " "
                });
                _clockInMiniPanel = true;
            }
            else if (MiniActiveTimers.Values.Count > 0)
            {
                if (_clockInMiniPanel)
                {
                    timers.Children.Clear();
                    _clockInMiniPanel = false;
                }
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
        /// <returns>Date time with values from string</returns>
        private DateTime StringToDateTime(string s)
        {
            DateTime returnDateTime = DateTime.Now;
            var split = s.Split(':');
            if (string.IsNullOrEmpty(split[0]))
            {
                var warning = new WarningWindow(WarningWindow.TimeWarning);
                warning.ShowDialog();
                MainWindow.CanOpenTimer = false;
            }

            if (split.Length > 1)
            {
                try
                {
                    var hour = int.Parse(split[0]);
                    var minute = int.Parse(split[1]);
                    var second = int.Parse(split[2]);
                    returnDateTime = new DateTime(_nowDateTime.Year, _nowDateTime.Month, _nowDateTime.Day, hour, minute, second);
                }
                catch (Exception)
                {
                    var warning = new WarningWindow(WarningWindow.TimeWarning);
                    warning.ShowDialog();
                    MainWindow.CanOpenTimer = false;
                    MainWindow.OnClose();
                }
            }
            else
            {
                var warning = new WarningWindow(WarningWindow.TimeWarning);
                warning.ShowDialog();
                MainWindow.CanOpenTimer = false;
                MainWindow.OnClose();
            }
            return returnDateTime;
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
