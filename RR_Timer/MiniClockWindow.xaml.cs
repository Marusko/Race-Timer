using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MiniClockWindow.xaml
    /// </summary>
    public partial class MiniClockWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        private ClockLogic clockLogic;

        public MiniClockWindow(ClockLogic cl)
        {
            InitializeComponent();
            clockLogic = cl;

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        public void SetEventName(string name)
        {
            EventNameMini.Content = name;
        }

        private void ClockTick(object sender, EventArgs e)
        {
            TimerMini.Content = clockLogic.FormatStartTime();
        }
    }
}
