﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using RR_Timer.API;
using RR_Timer.ClockUserControl;
using RR_Timer.Data;
using RR_Timer.Logic;
using Application = System.Windows.Application;

namespace RR_Timer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string Author = "Matúš Suský";
        private const string Version = "1.7.5";
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;
        private Window? _clockWindow;
        private bool _openedLinkTimer;
        public bool OpenedTimer { get; private set; }
        public bool MinimizedTimer { get; private set; }
        public bool CanOpenTimer { get; set; }

        /// <summary>
        /// Initializes and shows the main window, creates new ScreenHandler and ClockLogic,
        /// sets values for event type combobox and screen combobox from screen handler and selects first screen in list
        /// sets OnCloseCloseTimerWindow to be called when closing main window and calls SetInfoLabel() method
        /// Sets ControlTab as disabled
        /// Sets alignment and Qr code time events
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            _screenHandler = new ScreenHandler(this);
            _clockLogic = new ClockLogic(this, _screenHandler);

            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));
            ScreenComboBox.ItemsSource = _screenHandler.GetScreenNames();
            ScreenComboBox.SelectedIndex = 0;
            _screenHandler.SelectedScreen = _screenHandler.GetScreens()[ScreenComboBox.SelectedIndex];
            ScreenComboBox.SelectionChanged += SelectScreen;
            AlignmentComboBox.ItemsSource = _clockLogic.GetAlignments();
            AlignmentComboBox.SelectedIndex = 0;
            AlignmentComboBox.SelectionChanged += SelectAlignmentHandler;
            MinimizedCodeTimes.EveryTimeText.TextChanged += CheckIfInt;
            MinimizedCodeTimes.HowLongTimeText.TextChanged += CheckIfInt;
            MinimizedTimer = false;
            CanOpenTimer = true;
            ControlTab.IsEnabled = false;
            MaximizeButton.IsEnabled = false;
            Closed += ShutDownApp;
            SetInfoLabel();
        }

        /// <summary>
        /// Method called by Open timer button, only if clock window is not opened creates and shows new fullscreen clock window,
        /// sets it in clock logic, sets the properties and starts the timer
        /// Disables Timer, API timer, Display settings, QR tabs as disabled, enables control tab and selects it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            if (OpenedTimer) return;
            OpenedTimer = true;
            MinimizedTimer = false;
            _clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text, _clockLogic, _screenHandler);
            _clockLogic.SelectedAlignment = null;
            SelectAlignment();
            _clockLogic.SetClockWindow((ClockWindow)_clockWindow, EventNameText.Text, EventTypeComboBox.Text);
            if (!CanOpenTimer)
            {
                OpenedTimer = false;
                CanOpenTimer = true;
                return;
            }
            _clockWindow.Show();
            _clockLogic.StartTimer();
            LinkTimerTab.IsEnabled = false;
            TimerTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;
            ControlTab.IsEnabled = true;
            CodeTab.IsEnabled = false;
            TabControl.SelectedItem = ControlTab;
            _openedLinkTimer = false;
        }

        /// <summary>
        /// Method called by Open timer button in API timer menu, only if clock window is not opened creates and shows
        /// new fullscreen clock window, sets it in clock logic, sets the properties and starts the timer
        /// Disables Timer, API timer, Display settings, QR tabs, enables control tab and selects it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLinkTimer(object sender, RoutedEventArgs e)
        {
            if (OpenedTimer) return;
            OpenedTimer = true;
            MinimizedTimer = false;
            _clockWindow = new ClockWindow(LinkTimerStartTimeText.Text, _clockLogic, _screenHandler);
            _clockLogic.SelectedAlignment = null;
            SelectAlignment();
            _clockLogic.SetClockWindow(EventLinkText.Text, ListLinkText.Text, (ClockWindow)_clockWindow);
            if (!CanOpenTimer)
            {
                OpenedTimer = false;
                CanOpenTimer = true;
                return;
            }
            _clockWindow.Show();
            _clockLogic.StartTimer();
            LinkTimerTab.IsEnabled = false;
            TimerTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;
            ControlTab.IsEnabled = true;
            CodeTab.IsEnabled = false;
            TabControl.SelectedItem = ControlTab;
            _openedLinkTimer = true;
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
        /// Disables maximize button, enables minimize button
        /// </summary>
        private void MaximizeTimer()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            if (!MinimizedTimer) return;
            _clockWindow.Close();
            _clockWindow = new ClockWindow(_clockLogic, _screenHandler);
            _clockLogic.SelectedAlignment = null;
            SelectAlignment();
            _clockLogic.SetClockWindow((ClockWindow)_clockWindow);
            _clockLogic.CodeWindowForMinimized?.Close();
            _clockLogic.CodeWindowForMinimized = null;
            _clockWindow.Show();
            MinimizedTimer = false;
            MinimizeButton.IsEnabled = true;
            MaximizeButton.IsEnabled = false;
        }

        /// <summary>
        /// Closes the maximized clock window and open new minimized clock window, sets it in ClockLogic,
        /// sets the MinimizedTimer property to true
        /// Disables minimize button, enables maximize button
        /// </summary>
        public void MinimizeTimer()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            if (MinimizedTimer) return;
            _clockWindow.Close();
            _clockWindow = new MiniClockWindow(_clockLogic, _screenHandler);
            _clockLogic.SetClockWindow((MiniClockWindow)_clockWindow);
            _clockWindow.Show();
            MinimizedTimer = true;
            MinimizeButton.IsEnabled = false;
            MaximizeButton.IsEnabled = true;
        }

        /// <summary>
        /// Method called by Close timer button, calls OnCLose()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            OnClose();
        }

        /// <summary>
        /// Only if clock window is opened closes the clock window,
        /// sets both properties to false and stops the timer
        /// Enables Timer, API timer, Display settings, QR tabs, disables control tab and
        /// selects timer tab from which the timer was started
        /// </summary>
        public void OnClose()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            OpenedTimer = false;
            MinimizedTimer = false;
            _clockLogic.StopTimer();
            _clockLogic.SelectedAlignment = null;
            _clockLogic.CodeWindowForMinimized?.Close();
            _clockLogic.CodeWindowForMinimized = null;
            LinkTimerTab.IsEnabled = true;
            TimerTab.IsEnabled = true;
            SettingsTab.IsEnabled = true;
            ControlTab.IsEnabled = false;
            CodeTab.IsEnabled = true;
            TabControl.SelectedItem = _openedLinkTimer ? LinkTimerTab : TimerTab;
            MinimizeButton.IsEnabled = true;
            MaximizeButton.IsEnabled = false;
        }

        /// <summary>
        /// Called when main window is closed, shuts down the application, closes clock window calls OnCLose(),
        /// stops the screen handler timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShutDownApp(object? sender, EventArgs e)
        {
            OnClose();
            _screenHandler.StopTimer();
            Application.Current.Shutdown();
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
        /// Calls SelectAlignment, used with buttons
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAlignmentHandler(object sender, RoutedEventArgs e)
        {
            SelectAlignment();
        }
        /// <summary>
        /// Method selects alignment and sets it in ClockLogic
        /// </summary>
        private void SelectAlignment()
        {
            _clockLogic.SelectedAlignment = AlignmentComboBox.SelectedIndex switch
            {
                0 => new TimerTop(),
                1 => new TimerLeft(),
                2 => new TimerRight(),
                _ => _clockLogic.SelectedAlignment
            };
        }

        /// <summary>
        /// Opens file dialog where user chooses image to show, saves it in ClockLogic.LogoImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenImage(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            _clockLogic.LogoImage = new BitmapImage(new Uri(op.FileName));
            DeleteImageButton.IsEnabled = true;
            LinkDeleteImageButton.IsEnabled = true;
            var tmp = op.FileName;
            var index = tmp.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var name = tmp[index..];
            ImageName.Content = name;
            LinkImageName.Content = name;
        }

        /// <summary>
        /// Delete the image from ClockLogic.LogoImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteImage(object sender, RoutedEventArgs e)
        {
            _clockLogic.LogoImage = null;
            DeleteImageButton.IsEnabled = false;
            ImageName.Content = "";
            LinkDeleteImageButton.IsEnabled = false;
            LinkImageName.Content = "";
        }

        /// <summary>
        /// Opens file dialog where user chooses QR code/image to show, saves it in ClockLogic.CodeImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCode(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            _clockLogic.CodeImage = new BitmapImage(new Uri(op.FileName));
            var tmp = op.FileName;
            var index = tmp.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var name = tmp.Substring(index, tmp.Length - index);
            CodeName.Content = name;
            DeleteCodeButton.IsEnabled = true;
        }

        /// <summary>
        /// Delete the image from ClockLogic.CodeImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCode(object sender, RoutedEventArgs e)
        {
            _clockLogic.CodeImage = null;
            CodeName.Content = "";
            DeleteCodeButton.IsEnabled = false;
        }

        /// <summary>
        /// Generates code from link entered in CodeLinkText and saves it in ClockLogic.CodeImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateCode(object sender, RoutedEventArgs e)
        {
            DeleteCodeButton.IsEnabled = true;
            _clockLogic.CodeImage = CodeGenerator.GenerateCode(CodeLinkText.Text);
            CodeName.Content = "QR code generated!";
            if (_clockLogic.CodeImage != null) return;
            var ww = new WarningWindow("Something went wrong! QR code was not generated");
            ww.ShowDialog();
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
        /// Returns if the QR code on mini timer check box is checked or not
        /// </summary>
        /// <returns>Returns value of QR code on mini timer checkbox</returns>
        public bool IsCodeOnMinimized()
        {
            if(CodeCheckBoxForMinimized.IsChecked == null) return false;
            return (bool)CodeCheckBoxForMinimized.IsChecked;
        }

        /// <summary>
        /// Returns values from QR code menu bottom two text boxes when check box is checked,
        /// for showing and hiding QR code when timer is small
        /// </summary>
        /// <returns>Pair of int as minutes</returns>
        public (int, int) GetCodeForMiniTimes()
        {
            var show = int.Parse(MinimizedCodeTimes.HowLongTimeText.Text);
            var every = int.Parse(MinimizedCodeTimes.EveryTimeText.Text);
            return (show, every);
        }

        /// <summary>
        /// Checks if values from QR code menu bottom two text boxes can be converted into int,
        /// if it is not a number shows warning window and sets the text box value to "1",
        /// also checks if it is between 1 and 60, if not sets text box value to one which is closer [1/60]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckIfInt(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.GetType().Name != "TextBox") return;
                if (((System.Windows.Controls.TextBox)sender).Text.Length <= 0) return;
                var i = int.Parse(((System.Windows.Controls.TextBox)sender).Text);
                ((System.Windows.Controls.TextBox)sender).Text = i switch
                {
                    > 60 => "60",
                    <= 0 => "1",
                    _ => ((System.Windows.Controls.TextBox)sender).Text
                };
            }
            catch (Exception exception)
            {
                if (sender.GetType().Name == "TextBox")
                {
                    var w = new WarningWindow($"Oops, cannot convert this [{((System.Windows.Controls.TextBox)sender).Text}] to number\n[{exception.Message}]");
                    w.ShowDialog();
                    ((System.Windows.Controls.TextBox)sender).Text = "1";
                }
                else
                {
                    var w = new WarningWindow($"Oops, cannot convert this to number\n[{exception.Message}]");
                    w.ShowDialog();
                }
            }
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

        /// <summary>
        /// Method called by QR code generator hyperlink, opens QR code generator page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCodePage(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/codebude/QRCoder";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}
