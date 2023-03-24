using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ClockLogic ClockLogic = new ClockLogic();
        private Window clockWindow;
        private bool openedTimer = false;
        public MainWindow()
        {
            InitializeComponent();
            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));
        }

        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            if (!openedTimer)
            {
                clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text, ClockLogic);
                ClockLogic.SetClockWindow((ClockWindow)clockWindow, EventNameText.Text, EventTypeComboBox.Text);
                clockWindow.Show();
                openedTimer = true;
            }
        }

        private void OpenAPITimer(object sender, RoutedEventArgs e)
        {
            if (!openedTimer)
            {
                clockWindow = new ClockWindow(APITimerStartTimeText.Text, ClockLogic);
                ClockLogic.SetClockWindow(EventAPILinkText.Text, (ClockWindow)clockWindow);
                clockWindow.Show();
                openedTimer = true;
            }
        }

        private void MinimizeTimer(object sender, RoutedEventArgs e)
        {
            if (openedTimer)
            {
                clockWindow.Close();
                clockWindow = new MiniClockWindow(ClockLogic);
                ClockLogic.SetClockWindow((MiniClockWindow)clockWindow);
                clockWindow.Show();
            }
        }

        private void MaximizeTimer(object sender, RoutedEventArgs e)
        {
            if (openedTimer)
            {
                clockWindow.Close();
                clockWindow = new ClockWindow(ClockLogic);
                ClockLogic.SetClockWindow((ClockWindow)clockWindow);
                clockWindow.Show();
            }
        }

        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            if (openedTimer)
            {
                clockWindow.Close();
                openedTimer = false;
            }
        }
    }
}
