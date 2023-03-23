using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RR_Timer
{
    internal class ClockLogic
    {
        private DateTime ClockDateTime;
        private DateTime NowDateTime = DateTime.Now;
        private DateTime StartTime;

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

        public void StringToDateTime(string s)
        {
            string[] splitted = s.Split(':');
            int hour = int.Parse(splitted[0]);
            int minute = int.Parse(splitted[1]);
            StartTime = new DateTime(NowDateTime.Year, NowDateTime.Month, NowDateTime.Day, hour, minute, 0);
        }

        public string FormatTime()
        {
            ClockDateTime = DateTime.Now;
            var clock = ClockDateTime.TimeOfDay.ToString();
            var clockLength = clock.Length - (clock.Length - clock.LastIndexOf("."));
            var clockString = clock.Substring(0, clockLength);
            return clockString;
        }
        public string FormatStartTime()
        {
            var clock = ClockDateTime.Subtract(StartTime).ToString();
            var tmp = clock.Length - (clock.Length - clock.LastIndexOf("."));
            var timerClock = clock.Substring(0, tmp);
            return timerClock;
        }

    }
}
