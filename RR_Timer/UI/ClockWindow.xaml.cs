﻿using System.Windows;
using RR_Timer.Logic;

namespace RR_Timer.UI
{
    /// <summary>
    /// Interaction logic for ClockWindow.xaml
    /// </summary>
    public partial class ClockWindow
    {
        private readonly ClockLogic _clockLogic;
        private readonly ScreenHandler _screenHandler;

        /// <summary>
        /// Initialize and open the window, sets label contents, converts time string to DateTime and adds method
        /// to call after window is loaded
        /// </summary>
        /// <param name="name">Event name to show</param>
        /// <param name="type">Event type</param>
        /// <param name="startTime">Event start time in 00:00 format</param>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public ClockWindow(string name, string type, string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();
            
            EventNameLabel.Content = name;
            EventTypeLabel.Content = type;
            _clockLogic = cl;
            _screenHandler = sh;

            _clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;
        }
        /// <summary>
        /// Initialize and open the window, converts time string to DateTime and adds method to call after window is loaded
        /// </summary>
        /// <param name="startTime">Event start time in 00:00 format</param>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public ClockWindow(string startTime, ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            _clockLogic = cl;
            _screenHandler = sh;
            _clockLogic.StringToDateTime(startTime);

            Loaded += WindowLoaded;
        }
        /// <summary>
        /// Initialize and open the window, used when switching between fullscreen and minimized clock window
        /// </summary>
        /// <param name="cl">Already created ClockLogic object</param>
        /// <param name="sh">Already created ScreenHandler object</param>
        public ClockWindow(ClockLogic cl, ScreenHandler sh)
        {
            InitializeComponent();

            _clockLogic = cl;
            _screenHandler = sh;

            Loaded += WindowLoaded;
        }
        /// <summary>
        /// Method called after window is loaded, sets the position, state and width and height of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            if (_screenHandler.SelectedScreen == null) return;
            WindowState = WindowState.Normal;
            Left = _screenHandler.SelectedScreen.WorkingArea.Left;
            Top = _screenHandler.SelectedScreen.WorkingArea.Top;
            Width = _screenHandler.SelectedScreen.WorkingArea.Width;
            Height = _screenHandler.SelectedScreen.WorkingArea.Height;
            WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Sets the event name to label
        /// </summary>
        /// <param name="name">Event name to show</param>
        public void SetEventName(string name)
        {
            EventNameLabel.Content = name;
        }

        /// <summary>
        /// Sets the event type to label
        /// </summary>
        /// <param name="type">Event type to show</param>
        public void SetEventType(string type)
        {
            EventTypeLabel.Content = type;
        }

        /// <summary>
        /// Method to update timer and type label with correct time
        /// </summary>
        public void OnTimerClick()
        {
            _clockLogic.ShowClockOrTimer(ref TimerClockLabel, ref MainClockLabel);
        }
    }
    
}