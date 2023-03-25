using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using RR_Timer.API;

namespace RR_Timer.Logic
{
    public class ClockLogic
    {
        private DateTime ClockDateTime;
        private DateTime NowDateTime = DateTime.Now;
        public DateTime StartTime { get; set; }

        public string EventName { get; set; }
        public string EventType { get; set; }

        private APIHandler APIHandler;
        private Window clockWindow;
        private MainWindow mainWindow;

        public ClockLogic(MainWindow mw)
        {
            mainWindow = mw;
        }

        public void SetClockWindow(string APILink, string listLink, ClockWindow cw)
        {
            APIHandler = new APIHandler(APILink, listLink, this);
            clockWindow = cw;
        }
        public void SetClockWindow(Window cw)
        {
            clockWindow = cw;
            SetLabels(EventName, EventType);
        }
        public void SetClockWindow(Window cw, string name, string type)
        {
            clockWindow = cw;
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
                timer.Content = FormatStartTime();
                clock.Content = FormatTime();
            }

        }

        public void SetLabels(string name, string type)
        {
            EventName = name;
            EventType = type;

            if (clockWindow.GetType() == typeof(ClockWindow))
            {
                ((ClockWindow)clockWindow).SetEventName(name);
                ((ClockWindow)clockWindow).SetEventType(type);
            }
            else if (clockWindow.GetType() == typeof(MiniClockWindow))
            {
                ((MiniClockWindow)clockWindow).SetEventName(name);
            }
        }

        public void StringToDateTime(string s)
        {
            string[] splitted = s.Split(':');
            if (String.IsNullOrEmpty(splitted[0]))
            {
                StartTime = new DateTime(NowDateTime.Year, NowDateTime.Month, NowDateTime.Day, 0, 0, 0);
                return;
            }
            int hour = int.Parse(splitted[0]);
            int minute = int.Parse(splitted[1]);
            StartTime = new DateTime(NowDateTime.Year, NowDateTime.Month, NowDateTime.Day, hour, minute, 0);
            if (StartTime == null)
            {
                StartTime = new DateTime(NowDateTime.Year, NowDateTime.Month, NowDateTime.Day, 0, 0, 0);
            }
        }

        public string FormatTime()
        {
            ClockDateTime = DateTime.Now;
            var clock = ClockDateTime.TimeOfDay.ToString();
            var clockLength = clock.LastIndexOf(".");
            var clockString = clock.Substring(0, clockLength);
            return clockString;
        }
        public string FormatStartTime()
        {
            var clock = ClockDateTime.Subtract(StartTime).ToString();
            var tmp = clock.LastIndexOf(".");
            var timerClock = clock.Substring(0, tmp);
            return timerClock;
        }

        public void AutoMinimizeTimer()
        {
            mainWindow.MinimizeTimer();
        }

        public bool IsTimerMinimized()
        {
            return mainWindow.MinimizedTimer;
        }
    }
}
