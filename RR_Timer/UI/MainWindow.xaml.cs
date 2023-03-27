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
        private const string Version = "1.0.0";
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;
        private Window? _clockWindow;
        public bool OpenedTimer { get; private set; }
        public bool MinimizedTimer { get; private set; }

        /// <summary>
        /// Initializes and shows the main window, creates new ScreenHandler and ClockLogic,
        /// sets values for event type combobox and screen combobox from screen handler and selects first screen in list
        /// sets OnCloseCloseTimerWindow to be called when closing main window and calls SetInfoLabel() method
        /// </summary>
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

        /// <summary>
        /// Method called by Open timer button, only if clock window is not opened creates and shows new fullscreen clock window,
        /// sets it in clock logic, sets the properties and starts the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method called by Open timer button in API timer menu, only if clock window is not opened creates and shows
        /// new fullscreen clock window, sets it in clock logic, sets the properties and starts the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Method called by Minimize button, calls MinimizeTimer()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MinimizeTimer();
        }

        /// <summary>
        /// Method called by Maximize button, calls MaximizeTimer()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MaximizeTimer();
        }

        /// <summary>
        /// Closes the minimized clock window and open new fullscreen clock window, sets it in ClockLogic,
        /// sets the MinimizedTimer property to false
        /// </summary>
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

        /// <summary>
        /// Closes the maximized clock window and open new minimized clock window, sets it in ClockLogic,
        /// sets the MinimizedTimer property to true
        /// </summary>
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

        /// <summary>
        /// Method called by Close timer button, only if clock window is opened closes the clock window,
        /// sets both properties to false and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            OpenedTimer = false;
            MinimizedTimer = false;
            _clockLogic.StopTimer();
        }

        ///<summary>
        /// Method called when main window is closed, only if clock window is opened closes the clock window,
        /// sets both properties to false and stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnCloseCloseTimerWindow(object? sender, EventArgs e)
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            OpenedTimer = false;
            MinimizedTimer = false;
            _clockLogic.StopTimer();
        }

        /// <summary>
        /// After selecting string from list, set it also in ScreenHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectScreen(object sender, RoutedEventArgs e)
        {
            _screenHandler.SelectedScreen = _screenHandler.GetScreens()[ScreenComboBox.SelectedIndex];
        }

        /// <summary>
        /// After screen reloading, show new list of screens
        /// </summary>
        /// <param name="s">List of screen names</param>
        public void ShowReloadedScreens(IEnumerable<string> s)
        {
            ScreenComboBox.ItemsSource = s;
        }

        /// <summary>
        /// Sets the info label with correct author and version
        /// </summary>
        private void SetInfoLabel()
        {
            InfoLabel.Content = "Made by: " + Author + "\nVersion: " + Version;
        }

        /// <summary>
        /// Method called by Github hyperlink, opens project Github
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGithub(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/Marusko/RR_Timer";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Method called by JSON framework hyperlink, opens JSON framework page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenJsonPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://www.newtonsoft.com/json";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Method called by Material design hyperlink, opens Material design Github page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMaterialDesignPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Method called by App icon hyperlink, opens page with the icon
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenIconPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://www.flaticon.com/free-icon/hourglass_9182366";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}
