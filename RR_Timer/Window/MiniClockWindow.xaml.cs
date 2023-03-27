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
using RR_Timer.Logic;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MiniClockWindow.xaml
    /// </summary>
    public partial class MiniClockWindow : Window
    {
        private ClockLogic clockLogic;
        private ScreenHandler screenHandler;

        public MiniClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            clockLogic = cl;
            screenHandler = sh;

            Loaded += WindowLoaded;
        }

        public void SetEventName(string name)
        {
            EventNameMini.Content = name;
        }

        public void OnTimerClick()
        {
            clockLogic.ShowMiniClockOrTimer(ref TimerMini);
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
