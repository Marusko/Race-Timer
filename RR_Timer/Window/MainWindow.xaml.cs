using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using RR_Timer.Data;
using RR_Timer.Logic;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string author = "Matúš Suský";
        private const string version = "0.7.0";
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;
        private Window _clockWindow;
        public bool OpenedTimer { get; set; }
        public bool MinimizedTimer { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _screenHandler = new ScreenHandler(this);
            _clockLogic = new ClockLogic(this);
            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));
            ScreenComboBox.ItemsSource = _screenHandler.GetScreenNames();
            ScreenComboBox.SelectedIndex = 0;
            _screenHandler.SelectedScreen = _screenHandler.GetScreens()[ScreenComboBox.SelectedIndex];
            ScreenComboBox.SelectionChanged += SelectScreen;
            MinimizedTimer = false;
            Closing += OnCloseCloseTimerWindow;
            SetInfoLabel();
        }

        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            if (!OpenedTimer)
            {
                _clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text, _clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((ClockWindow)_clockWindow, EventNameText.Text, EventTypeComboBox.Text);
                _clockWindow.Show();
                OpenedTimer = true;
                MinimizedTimer = false;
            }
        }

        private void OpenAPITimer(object sender, RoutedEventArgs e)
        {
            if (!OpenedTimer)
            {
                _clockWindow = new ClockWindow(APITimerStartTimeText.Text, _clockLogic, _screenHandler);
                _clockLogic.SetClockWindow(EventAPILinkText.Text, ListAPILinkText.Text, (ClockWindow)_clockWindow);
                _clockWindow.Show();
                OpenedTimer = true;
                MinimizedTimer = false;
            }
        }

        private void MinimizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MinimizeTimer();
        }

        private void MaximizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MaximizeTimer();
        }

        public void MaximizeTimer()
        {
            if (OpenedTimer)
            {
                _clockWindow.Close();
                _clockWindow = new ClockWindow(_clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((ClockWindow)_clockWindow);
                _clockWindow.Show();
                MinimizedTimer = false;
            }
        }

        public void MinimizeTimer()
        {
            if (OpenedTimer)
            {
                _clockWindow.Close();
                _clockWindow = new MiniClockWindow(_clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((MiniClockWindow)_clockWindow);
                _clockWindow.Show();
                MinimizedTimer = true;
            }
        }

        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            if (OpenedTimer)
            {
                _clockWindow.Close();
                OpenedTimer = false;
                MinimizedTimer = false;
            }
        }
        private void OnCloseCloseTimerWindow(object sender, EventArgs e)
        {
            if (OpenedTimer)
            {
                _clockWindow.Close();
                OpenedTimer = false;
                MinimizedTimer = false;
            }
        }

        private void SelectScreen(object sender, RoutedEventArgs e)
        {
            _screenHandler.SelectedScreen = _screenHandler.GetScreens()[ScreenComboBox.SelectedIndex];
        }

        public void ShowReloadedScreens(string[] s)
        {
            ScreenComboBox.ItemsSource = s;
        }

        private void SetInfoLabel()
        {
            InfoLabel.Content = "Made by: " + author + "\nVersion: " + version;
        }

        private void OpenGithub(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/Marusko/RR_Timer";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        private void OpenJsonPage(object sender, RoutedEventArgs e)
        {
            string url = "https://www.newtonsoft.com/json";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        private void OpenUIPage(object sender, RoutedEventArgs e)
        {
            string url = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        private void OpenIconPage(object sender, RoutedEventArgs e)
        {
            string url = "https://www.flaticon.com/free-icon/hourglass_9182366";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}
