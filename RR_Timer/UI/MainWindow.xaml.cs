using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using RR_Timer.Data;
using RR_Timer.Logic;

namespace RR_Timer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string Author = "Matúš Suský";
        private const string Version = "0.7.0";
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;
        private Window? _clockWindow;
        public bool OpenedTimer { get; private set; }
        public bool MinimizedTimer { get; private set; }

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
            if (OpenedTimer) return;
            _clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text, _clockLogic, _screenHandler);
            _clockLogic.SetClockWindow((ClockWindow)_clockWindow, EventNameText.Text, EventTypeComboBox.Text);
            _clockWindow.Show();
            OpenedTimer = true;
            MinimizedTimer = false;
            _clockLogic.StartTimer();
        }

        private void OpenLinkTimer(object sender, RoutedEventArgs e)
        {
            if (OpenedTimer) return;
            _clockWindow = new ClockWindow(LinkTimerStartTimeText.Text, _clockLogic, _screenHandler);
            _clockLogic.SetClockWindow(EventLinkText.Text, ListLinkText.Text, (ClockWindow)_clockWindow);
            _clockWindow.Show();
            OpenedTimer = true;
            MinimizedTimer = false;
            _clockLogic.StartTimer();
        }

        private void MinimizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MinimizeTimer();
        }

        private void MaximizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MaximizeTimer();
        }

        private void MaximizeTimer()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            _clockWindow = new ClockWindow(_clockLogic, _screenHandler);
            _clockLogic.SetClockWindow((ClockWindow)_clockWindow);
            _clockWindow.Show();
            MinimizedTimer = false;
        }

        public void MinimizeTimer()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            _clockWindow = new MiniClockWindow(_clockLogic, _screenHandler);
            _clockLogic.SetClockWindow((MiniClockWindow)_clockWindow);
            _clockWindow.Show();
            MinimizedTimer = true;
        }

        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            OpenedTimer = false;
            MinimizedTimer = false;
            _clockLogic.StopTimer();
        }
        private void OnCloseCloseTimerWindow(object? sender, EventArgs e)
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            OpenedTimer = false;
            MinimizedTimer = false;
            _clockLogic.StopTimer();
        }

        private void SelectScreen(object sender, RoutedEventArgs e)
        {
            _screenHandler.SelectedScreen = _screenHandler.GetScreens()[ScreenComboBox.SelectedIndex];
        }

        public void ShowReloadedScreens(IEnumerable<string> s)
        {
            ScreenComboBox.ItemsSource = s;
        }

        private void SetInfoLabel()
        {
            InfoLabel.Content = "Made by: " + Author + "\nVersion: " + Version;
        }

        private void OpenGithub(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/Marusko/RR_Timer";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        private void OpenJsonPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://www.newtonsoft.com/json";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        private void OpenMaterialDesignPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
        private void OpenIconPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://www.flaticon.com/free-icon/hourglass_9182366";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}
