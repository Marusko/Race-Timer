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
        private ScreenHandler screenHandler;

        public MiniClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            clockLogic = cl;
            screenHandler = sh;

            Loaded += WindowLoaded;

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
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Left = screenHandler.SelectedScreen.WorkingArea.Left;
            this.Top = screenHandler.SelectedScreen.WorkingArea.Top;
            this.Width = screenHandler.SelectedScreen.WorkingArea.Width;
        }
    }
}
