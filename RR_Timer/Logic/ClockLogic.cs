using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Race_timer.API;
using Race_timer.ClockUserControl;
using Race_timer.Logic.Interfaces;
using Race_timer.UI;

namespace Race_timer.Logic
{
    /// <summary>
    /// Handles all logic for clocks
    /// </summary>
    public class ClockLogic
    {
        private static ClockLogic? _instance;

        public const int ScrollBegin = -1;
        public const int Scrolling = 0;
        public const int ScrollEnd = 1;

        private const int InfoPanelHide = 3;
        private const int InfoPanelMinShowSec = 30;

        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private DateTime? _lastNtp;

        public Dictionary<string, DateTime> StartTimes { get; }
        public Dictionary<string, ContestTimer> ActiveTimers { get; }
        public Dictionary<string, MiniContestTimer> MiniActiveTimers { get; }
        public string? EventName { get; private set; }
        private string? EventType { get; set; }
        public BitmapImage? LogoImage { get; set; }
        public BitmapSource? CodeImage { get; set; }
        public CodeWindowForMinimized? CodeWindowForMinimized { get; set; }
        public InfoWindow? InfoWindow { get; set; }
        public MainWindow MainWindow { get; }
        public UserControl? SelectedAlignment { get; set; }
        public bool Starts { get; set; }
        public bool ApiStarts { get; set; }

        private IClockWindow? _clockWindow;
        private readonly ObservableCollection<string> _timerAlignmentNames = new();
        public string InfoText { get; set; } = "";

        private DateTime _showCodeTime;
        private DateTime _hideCodeTime;
        private DateTime _showTime;

        private DateTime _showInfoTime;
        private DateTime _hideInfoTime;
        private DateTime _showTimeInfo;

        /// <summary>
        /// Sets the main window, screen handler and alignment to top
        /// </summary>
        /// <param name="mw">Previously created main window</param>
        private ClockLogic(MainWindow mw)
        {
            MainWindow = mw;
            StartTimes = new Dictionary<string, DateTime>();
            ActiveTimers = new Dictionary<string, ContestTimer>();
            MiniActiveTimers = new Dictionary<string, MiniContestTimer>();
            SetAlignmentList();
            if (ScreenHandler.GetInstance().SelectedScreen != null)
                SelectedAlignment = new TimerTop(ScreenHandler.GetInstance().GetSelectedScreenArea().Width);
        }

        /// <summary>
        /// Method for ClockLogic initialization
        /// </summary>
        /// <param name="mw">Already created MainWindow</param>
        /// <returns>ClockLogic instance</returns>
        public static ClockLogic Initialize(MainWindow mw)
        {
            if (_instance == null)
            {
                _instance = new ClockLogic(mw);
            }

            return _instance;
        }

        /// <summary>
        /// Method for retrieving singleton instance of ClockLogic
        /// </summary>
        /// <returns>Singleton instance of ClockLogic</returns>
        /// <exception cref="InvalidOperationException">When ClockLogic is not initialized first</exception>
        public static ClockLogic GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("ClockLogic is not initialized. Call Initialize() first.");
            }
            return _instance;
        }

        /// <summary>
        /// Sets the active clock window observer
        /// </summary>
        /// <param name="w">Active clock window</param>
        private void SetActiveObserver(IClockWindow w)
        {
            _clockWindow = w;
        }

        /// <summary>
        /// Converts time string to DateTime for all contest
        /// </summary>
        public void ProcessStartTimes()
        {
            StartTimes.Clear();
            ActiveTimers.Clear();
            foreach (var time in MainWindow.StartTimes)
            {
                StartTimes.Add(time.Key, StringToDateTime(time.Value));
            }
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
            ClockTickLogic();
            _timer.Tick += ClockTick;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            _timer.Start();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
            try
            {
                LinkHandler.GetInstance().StopTimer();
            }
            catch (InvalidOperationException)
            {
            }
            InfoWindow?.StopTimer();
        }

        /// <summary>
        /// Method called by timer, calls ClockTickLogic()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClockTick(object? sender, EventArgs e)
        {
            ClockTickLogic();
        }

        /// <summary>
        /// Method called by timer, calls method in windows to update clocks
        /// If the timer is small and QR code is enabled, calls method MiniCodeShow()
        /// </summary>
        /// <param name="isSmall">If timer is small</param>
        public void ClockTickLogic(bool isSmall = false)
        {
            var now = DateTimeHandler.GetInstance().Now;
            if (_lastNtp != null)
            {
                if (_lastNtp.Value.Second == now.Second)
                {
                    return;
                }
            }
            _lastNtp = now;
            CheckTimers(isSmall);
            ClickTimers();
            AfterCheckTimers();
        }

        /// <summary>
        /// Calls methods to update timers, scrolls... in clock windows
        /// </summary>
        public void AfterCheckTimers()
        {
            if (_clockWindow == null) return;
            _clockWindow.OnTimerClick();
            if (MainWindow.MinimizedTimer)
            {
                MiniCodeShow();
                InfoPanelShow();
            }
        }

        /// <summary>
        /// Checks if som of the timers are started, if yes then it checks if clock window is small or fullscreen,
        /// and fills the corresponding Dictionary with started timers
        /// If timer has new start time greater than current time, remove it
        /// Is called every third of the second
        /// </summary>
        /// <param name="isSmall">Manually set filling to MiniActiveTimers</param>
        public void CheckTimers(bool isSmall = false)
        {
            for (int i = 0;i < StartTimes.Count; i++)
            {
                if (StartTimes.Values.ElementAt(i) < DateTimeHandler.GetInstance().Now)
                {
                    if (ScreenHandler.GetInstance().SelectedScreen == null) return;

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
                        ActiveTimers.Add(StartTimes.Keys.ElementAt(i), new ContestTimer(ScreenHandler.GetInstance().GetSelectedScreenArea().Width, false,
                            EventName?.Length ?? 0)
                        {
                            Name = StartTimes.Keys.ElementAt(i),
                            StartTime = StartTimes.Values.ElementAt(i)
                        });
                    }
                    else if (_clockWindow.GetType() == typeof(MiniClockWindow) || _clockWindow.GetType() == typeof(WebViewClockWindow) || isSmall)
                    {
                        if (ActiveTimers.Count > 0)
                        {
                            ActiveTimers.Clear();
                        }
                        if (MiniActiveTimers.ContainsKey(StartTimes.Keys.ElementAt(i)))
                        {
                            continue;
                        }
                        MiniActiveTimers.Add(StartTimes.Keys.ElementAt(i), new MiniContestTimer(ScreenHandler.GetInstance().GetSelectedScreenArea().Width, false)
                        {
                            Name = StartTimes.Keys.ElementAt(i),
                            StartTime = StartTimes.Values.ElementAt(i)
                        });
                    }
                }
                else
                {
                    if (ActiveTimers.ContainsKey(StartTimes.Keys.ElementAt(i)))
                    {
                        ActiveTimers.Remove(StartTimes.Keys.ElementAt(i));
                    }
                    if (MiniActiveTimers.ContainsKey(StartTimes.Keys.ElementAt(i)))
                    {
                        MiniActiveTimers.Remove(StartTimes.Keys.ElementAt(i));
                        _clockWindow?.TimerTickLogic();
                    }
                }
            }
        }

        /// <summary>
        /// Calls contest timer logic for updating label
        /// </summary>
        private void ClickTimers()
        {
            _clockWindow?.TimerClickLogic();
            if (MainWindow.MinimizedTimer)
            {
                foreach (var ct in MiniActiveTimers)
                {
                    ct.Value.TimerClickLogic();
                }
            }
            else
            {
                foreach (var ct in ActiveTimers)
                {
                    ct.Value.TimerClickLogic();
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
                CodeWindowForMinimized = new CodeWindowForMinimized();
                CodeWindowForMinimized.SetImage(CodeImage);
                _hideCodeTime = DateTimeHandler.GetInstance().Now;
                _showTime = DateTimeHandler.GetInstance().Now;
                CodeWindowForMinimized.Visibility = Visibility.Hidden;
            }
            else
            {
                if (_hideCodeTime >= _showTime.AddMinutes(MainWindow.GetCodeForMiniTimes().Item2))
                {
                    if (CodeWindowForMinimized.Visibility == Visibility.Hidden)
                    {
                        CodeWindowForMinimized.Show();
                        _showCodeTime = DateTimeHandler.GetInstance().Now;
                    }
                    _showCodeTime = _showCodeTime.Add(DateTimeHandler.GetInstance().Now - _showCodeTime);
                    if (_showCodeTime < _showTime.AddMinutes(MainWindow.GetCodeForMiniTimes().Item1 +
                                                             MainWindow.GetCodeForMiniTimes().Item2)) return;
                    CodeWindowForMinimized.Hide();
                    _hideCodeTime = DateTimeHandler.GetInstance().Now;
                    _showCodeTime = DateTimeHandler.GetInstance().Now;
                    _showTime = DateTimeHandler.GetInstance().Now;
                }
                else
                {
                    _hideCodeTime = _hideCodeTime.Add(DateTimeHandler.GetInstance().Now - _hideCodeTime);
                }
            }
        }

        /// <summary>
        /// Shows and hides info panel when timer is small, shown for minimum of 30 seconds, hidden for 3 minutes
        /// </summary>
        private void InfoPanelShow()
        {
            if (MainWindow.InfoEnableCheckBox.IsChecked != null && (!MainWindow.InfoEnableCheckBox.IsChecked.Value || string.IsNullOrEmpty(InfoText))) return;
            if (InfoWindow == null)
            {
                InfoWindow = new InfoWindow();
                InfoWindow.SetLabel(InfoText);
                _hideInfoTime = DateTimeHandler.GetInstance().Now;
                _showTimeInfo = DateTimeHandler.GetInstance().Now;
                InfoWindow.Visibility = Visibility.Hidden;
            }
            else
            {
                if (_hideInfoTime >= _showTimeInfo.AddMinutes(InfoPanelHide))
                {
                    if (InfoWindow.Visibility == Visibility.Hidden)
                    {
                        InfoWindow.SetLabel(InfoText);
                        InfoWindow.Show();
                        InfoWindow.StartTimer();
                        _showInfoTime = DateTimeHandler.GetInstance().Now;
                    }
                    _showInfoTime = _showInfoTime.Add(DateTimeHandler.GetInstance().Now - _showInfoTime);
                    if ((_showInfoTime < _showTimeInfo.AddSeconds(InfoPanelMinShowSec + InfoPanelHide * 60) 
                        && InfoWindow.Laps == 0 && !InfoWindow.IsScrolling()) 
                        || (InfoWindow.IsScrolling() && InfoWindow.Laps < 2)) return;
                    InfoWindow.Hide();
                    InfoWindow.Laps = 0;
                    InfoWindow.StopTimer();
                    _hideInfoTime = DateTimeHandler.GetInstance().Now;
                    _showInfoTime = DateTimeHandler.GetInstance().Now;
                    _showTimeInfo = DateTimeHandler.GetInstance().Now;
                }
                else
                {
                    _hideInfoTime = _hideInfoTime.Add(DateTimeHandler.GetInstance().Now - _hideInfoTime);
                }
            }
        }

        /// <summary>
        /// When opening clock with API links, opens the clock and sets the clock window,
        /// sets alignment, and both images
        /// </summary>
        /// <param name="mainLink">Link for event name and type</param>
        /// <param name="countLink">Link for finished participants list</param>
        /// <param name="cw">To update or open clock window with API parameters</param>
        public void SetClockWindow(string mainLink, string countLink, IClockWindow cw)
        {
            if (!string.IsNullOrEmpty(mainLink))
            {
                if (string.IsNullOrEmpty(countLink))
                {
                    var warning = new WarningWindow(WarningWindow.CountLinkWarning);
                    warning.ShowDialog();
                    LinkHandler.Initialize(mainLink);
                }
                else
                {
                    LinkHandler.Initialize(mainLink, countLink);
                }
            }
            else
            {
                var warning = new WarningWindow(WarningWindow.ApiLinkWarning);
                warning.ShowDialog();
                MainWindow.CanOpenTimer = false;
                MainWindow.OnClose();
            }

            SetActiveObserver(cw);
            if (SelectedAlignment != null && !MainWindow.MinimizedTimer)
            {
                _clockWindow?.SetChildren(SelectedAlignment);
                if (SelectedAlignment.GetType() == typeof(TimerTop))
                {
                    ((TimerTop)SelectedAlignment).SetTopMargin(EventName?.Length ?? 0);
                }
            }
            if (LogoImage != null)
            {
                _clockWindow?.SetImage(LogoImage);
            }
            if (CodeImage != null && SelectedAlignment != null && !MainWindow.MinimizedTimer)
            {
                _clockWindow?.SetCodeImage(CodeImage);
            }
        }

        /// <summary>
        /// When switching from minimized to fullscreen or backwards,
        /// sets alignment, and both images
        /// </summary>
        /// <param name="cw">To update _clockWindow</param>
        public void SetClockWindow(IClockWindow cw)
        {
            SetActiveObserver(cw);
            if (SelectedAlignment != null && !MainWindow.MinimizedTimer)
            {
                _clockWindow?.SetChildren(SelectedAlignment);
            }
            if (LogoImage != null)
            {
                _clockWindow?.SetImage(LogoImage);
            }
            if (CodeImage != null && SelectedAlignment != null && !MainWindow.MinimizedTimer)
            {
                _clockWindow?.SetCodeImage(CodeImage);
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
        public void SetClockWindow(IClockWindow cw, string name, string type)
        {
            SetActiveObserver(cw);
            if (SelectedAlignment != null && !MainWindow.MinimizedTimer)
            {
                _clockWindow?.SetChildren(SelectedAlignment);
            }
            if (LogoImage != null)
            {
                _clockWindow?.SetImage(LogoImage);
            }
            if (CodeImage != null && SelectedAlignment != null && !MainWindow.MinimizedTimer)
            {
                _clockWindow?.SetCodeImage(CodeImage);
            }
            SetLabels(name, type);
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

            _clockWindow?.SetLabels(name, type);
            if (_clockWindow != null && !MainWindow.MinimizedTimer)
            {
                if (SelectedAlignment?.GetType() == typeof(TimerTop))
                {
                    ((TimerTop)SelectedAlignment).SetTopMargin(EventName?.Length ?? 0);
                }
            }
        }

        /// <summary>
        /// Converts string to DateTime, when something goes wrong, show warning window and closes the clock window
        /// </summary>
        /// <param name="s">Time in string format</param>
        /// <returns>Date time with values from string</returns>
        public DateTime StringToDateTime(string s)
        {
            DateTime returnDateTime = DateTimeHandler.GetInstance().Now;
            var split = s.Split(':');

            if (split.Length > 2)
            {
                try
                {
                    var date = DateTimeHandler.GetInstance().Now;
                    var hour = int.Parse(split[0]);
                    var minute = int.Parse(split[1]);
                    var second = int.Parse(split[2]);
                    returnDateTime = new DateTime(date.Year, date.Month, date.Day, hour, minute, second);
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
        /// Calls minimize method from main widow
        /// </summary>
        public void AutoMinimizeTimer()
        {
            MainWindow.MinimizeTimer();
        }

        /// <summary>
        /// If currently opened window is small timer or not
        /// </summary>
        /// <returns>If timer window is small</returns>
        public bool IsTimerMinimized()
        {
            return MainWindow.MinimizedTimer;
        }

        /// <summary>
        /// All alignment names
        /// </summary>
        /// <returns>Collection of alignment names</returns>
        public ObservableCollection<string> GetAlignments()
        {
            return _timerAlignmentNames;
        }
    }
}
