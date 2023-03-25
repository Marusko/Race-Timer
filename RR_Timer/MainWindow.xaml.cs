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
using System.Windows.Forms;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ClockLogic _clockLogic = new ClockLogic();
        private readonly ScreenHandler _screenHandler = new ScreenHandler();
        private Window _clockWindow;
        private bool _openedTimer = false;
        public MainWindow()
        {
            InitializeComponent();
            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));
            ScreenComboBox.ItemsSource = _screenHandler.GetScreens();
            ScreenComboBox.SelectionChanged += SelectScreen;
        }

        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            if (!_openedTimer)
            {
                _clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text, _clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((ClockWindow)_clockWindow, EventNameText.Text, EventTypeComboBox.Text);
                _clockWindow.Show();
                _openedTimer = true;
            }
        }

        private void OpenAPITimer(object sender, RoutedEventArgs e)
        {
            if (!_openedTimer)
            {
                _clockWindow = new ClockWindow(APITimerStartTimeText.Text, _clockLogic, _screenHandler);
                _clockLogic.SetClockWindow(EventAPILinkText.Text, (ClockWindow)_clockWindow);
                _clockWindow.Show();
                _openedTimer = true;
            }
        }

        private void MinimizeTimer(object sender, RoutedEventArgs e)
        {
            if (_openedTimer)
            {
                _clockWindow.Close();
                _clockWindow = new MiniClockWindow(_clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((MiniClockWindow)_clockWindow);
                _clockWindow.Show();
            }
        }

        private void MaximizeTimer(object sender, RoutedEventArgs e)
        {
            if (_openedTimer)
            {
                _clockWindow.Close();
                _clockWindow = new ClockWindow(_clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((ClockWindow)_clockWindow);
                _clockWindow.Show();
            }
        }

        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            if (_openedTimer)
            {
                _clockWindow.Close();
                _openedTimer = false;
            }
        }

        private void SelectScreen(object sender, RoutedEventArgs e)
        {
            _screenHandler.SelectedScreen = (Screen)ScreenComboBox.SelectedItem;
        }
     }
}
