using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        private ClockLogic clockLogic;

        public ClockWindow(string name, string type, string startTime, ClockLogic cl)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;
            EventNameLabel.Content = name;
            EventTypeLabel.Content = type;
            clockLogic = cl;

            clockLogic.StringToDateTime(startTime);

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        public ClockWindow(string startTime, ClockLogic cl)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;
            clockLogic = cl;

            clockLogic.StringToDateTime(startTime);

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        public ClockWindow(ClockLogic cl)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;
            clockLogic = cl;

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        public void SetEventName(string name)
        {
            EventNameLabel.Content = name;
        }

        public void SetEventType(string type)
        {
            EventTypeLabel.Content = type;
        }

        private void ClockTick(object sender, EventArgs e)
        {
            clockLogic.ShowClockOrTimer(ref TimerClockLabel, ref MainClockLabel);
        }

        private void HandleEsc(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
                Close();
        }
    }
    
}
