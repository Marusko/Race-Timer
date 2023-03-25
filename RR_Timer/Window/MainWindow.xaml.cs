﻿using System;
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
using RR_Timer.Data;
using RR_Timer.Logic;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;
        private Window _clockWindow;
        private bool _openedTimer = false;
        public bool MinimizedTimer { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            _screenHandler = new ScreenHandler(this);
            _clockLogic = new ClockLogic(this);
            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));
            ScreenComboBox.ItemsSource = _screenHandler.GetScreens();
            ScreenComboBox.SelectedIndex = 0;
            _screenHandler.SelectedScreen = (Screen)ScreenComboBox.SelectedItem;
            ScreenComboBox.SelectionChanged += SelectScreen;
            MinimizedTimer = false;
            Closing += CloseTimer;
        }

        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            if (!_openedTimer)
            {
                _clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text, _clockLogic, _screenHandler);
                _clockLogic.SetClockWindow((ClockWindow)_clockWindow, EventNameText.Text, EventTypeComboBox.Text);
                _clockWindow.Show();
                _openedTimer = true;
                MinimizedTimer = false;
            }
        }

        private void OpenAPITimer(object sender, RoutedEventArgs e)
        {
            if (!_openedTimer)
            {
                _clockWindow = new ClockWindow(APITimerStartTimeText.Text, _clockLogic, _screenHandler);
                _clockLogic.SetClockWindow(EventAPILinkText.Text, ListAPILinkText.Text, (ClockWindow)_clockWindow);
                _clockWindow.Show();
                _openedTimer = true;
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
            if (_openedTimer)
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
            if (_openedTimer)
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
            if (_openedTimer)
            {
                _clockWindow.Close();
                _openedTimer = false;
                MinimizedTimer = false;
            }
        }
        private void CloseTimer(object sender, EventArgs e)
        {
            if (_openedTimer)
            {
                _clockWindow.Close();
                _openedTimer = false;
                MinimizedTimer = false;
            }
        }

        private void SelectScreen(object sender, RoutedEventArgs e)
        {
            _screenHandler.SelectedScreen = (Screen)ScreenComboBox.SelectedItem;
        }

        public void ShowReloadedScreens(Screen[] s)
        {
            ScreenComboBox.ItemsSource = s;
        }
    }
}