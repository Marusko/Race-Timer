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

        private ClockLogic clockLogic = new();

        public ClockWindow(string name, string type, string startTime)
        {
            InitializeComponent();
            PreviewKeyDown += HandleEsc;
            EventNameLabel.Content = name;
            EventTypeLabel.Content = type;

            clockLogic.StringToDateTime(startTime);

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
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
