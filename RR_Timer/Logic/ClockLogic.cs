using System;
using System.Windows;
using RR_Timer.API;
using RR_Timer.UI;

namespace RR_Timer.Logic
{
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

        public void StartTimer()
        {
            _timer.Tick += ClockTick;
            _timer.Interval = new TimeSpan(0, 0, 1);
            _timer.Start();
        }

        public void StopTimer()
        {
            _timer.Stop();
        }
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
        public void SetClockWindow(Window cw)
        {
            _clockWindow = cw;
            if (EventName == null) return;
            if (EventType != null)
                SetLabels(EventName, EventType);
        }
        public void SetClockWindow(Window cw, string name, string type)
        {
            _clockWindow = cw;
            SetLabels(name, type);
        }

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
        private string FormatStartTime()
        {
            var clock = _clockDateTime.Subtract(StartTime).ToString();
            var tmp = clock.LastIndexOf(".", StringComparison.Ordinal);
            var timerClock = clock.Substring(0, tmp);
            return timerClock;
        }

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
