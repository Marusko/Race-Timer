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
using RR_Timer.Logic;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();

        private ClockLogic clockLogic;
        private ScreenHandler screenHandler;

        public ClockWindow(string name, string type, string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            
            EventNameLabel.Content = name;
            EventTypeLabel.Content = type;
            clockLogic = cl;
            screenHandler = sh;

            clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Left = screenHandler.SelectedScreen.WorkingArea.Left;
            this.Top = screenHandler.SelectedScreen.WorkingArea.Top;
            this.Width = screenHandler.SelectedScreen.WorkingArea.Width;
            this.Height = screenHandler.SelectedScreen.WorkingArea.Height;
            this.WindowState = WindowState.Maximized;
        }

        public ClockWindow(string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            clockLogic = cl;
            screenHandler = sh;

            clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;

            Timer.Tick += ClockTick;
            Timer.Interval = new TimeSpan(0, 0, 1);
            Timer.Start();
        }

        public ClockWindow(ClockLogic cl, ScreenHandler sh)
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
    }
    
}
