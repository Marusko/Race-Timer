using System;
using System.Windows;
using RR_Timer.API;
using RR_Timer.UI;

namespace RR_Timer.Logic
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

        private Window? _clockWindow;
        private readonly MainWindow _mainWindow;

        public ClockLogic(MainWindow mw)
        {
            _mainWindow = mw;
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
        }
        /// <summary>
        /// Method called by timer, calls method in windows to update clocks
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
            }
        }

        /// <summary>
        /// When opening clock with API links, opens the clock and sets the clock window
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
                    var unusedLinkHandler = new LinkHandler(mainLink, this);
                }
                else
                {
                    var unusedLinkHandler = new LinkHandler(mainLink, listLink, this);
                }
            }
            else
            {
                var warning = new WarningWindow(WarningWindow.ApiLinkWarning);
                warning.ShowDialog();
                SetClockWindow(cw, "", "");
            }

            _clockWindow = cw;
        }
        /// <summary>
        /// When switching from minimized to fullscreen or backwards
        /// </summary>
        /// <param name="cw">To update _clockWindow</param>
        public void SetClockWindow(Window cw)
        {
            _clockWindow = cw;
            if (EventName == null) return;
            if (EventType != null)
                SetLabels(EventName, EventType);
        }
        /// <summary>
        /// When opening clock with manually set name and type, sets the labels
        /// </summary>
        /// <param name="cw">To open or update clock window</param>
        /// <param name="name">Name of the event</param>
        /// <param name="type">Type of event</param>
        public void SetClockWindow(Window cw, string name, string type)
        {
            _clockWindow = cw;
            SetLabels(name, type);
        }

        /// <summary>
        /// If current time is less than start time show big clock, else show big timer and small clock under
        /// in fullscreen clock
        /// </summary>
        /// <param name="timer">Timer label from fullscreen clock</param>
        /// <param name="clock">Clock label from fullscreen clock</param>
        public void ShowClockOrTimer(ref System.Windows.Controls.Label timer, ref System.Windows.Controls.Label clock)
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
        public void ShowMiniClockOrTimer(ref System.Windows.Controls.Label timer)
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
        /// Converts string to DateTime, when something goes wrong, show warning window and sets time to 00:00
        /// </summary>
        /// <param name="s">Time in string format</param>
        public void StringToDateTime(string s)
        {
            var split = s.Split(':');
            if (string.IsNullOrEmpty(split[0]))
            {
                var warning = new WarningWindow(WarningWindow.TimeWarning);
                warning.ShowDialog();
                StartTime = new DateTime(_nowDateTime.Year, _nowDateTime.Month, _nowDateTime.Day, 0, 0, 0);
                return;
            }

            if (split.Length > 1)
            {
                var hour = int.Parse(split[0]);
                var minute = int.Parse(split[1]);
                try
                {
                    StartTime = new DateTime(_nowDateTime.Year, _nowDateTime.Month, _nowDateTime.Day, hour, minute, 0);
                }
                catch (Exception)
                {
                    var warning = new WarningWindow(WarningWindow.TimeWarning);
                    warning.ShowDialog();
                    StartTime = new DateTime(_nowDateTime.Year, _nowDateTime.Month, _nowDateTime.Day, 0, 0, 0);
                }
            }
            else
            {
                var warning = new WarningWindow(WarningWindow.TimeWarning);
                warning.ShowDialog();
                StartTime = new DateTime(_nowDateTime.Year, _nowDateTime.Month, _nowDateTime.Day, 0, 0, 0);
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
            _mainWindow.MinimizeTimer();
        }
        public bool IsTimerMinimized()
        {
            return _mainWindow.MinimizedTimer;
        }
    }
}
