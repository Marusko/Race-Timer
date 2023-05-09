using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Race_timer.UI;

namespace Race_timer.Logic
{
    /// <summary>
    /// Handles loading and switching between PC screens
    /// </summary>
    public class ScreenHandler
    {
        private Screen[] _screens = Screen.AllScreens;
        private string[] _screenNames;
        public Screen? SelectedScreen { get; set; }
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Sets up timer, names
        /// </summary>
        /// <param name="mw">Already created MainWindow object</param>
        public ScreenHandler(MainWindow mw)
        {
            _mainWindow = mw;
            _screenNames = new string[_screens.Length];
            _timer.Tick += ReloadScreens;
            _timer.Interval = new TimeSpan(0, 0, 20);
            _timer.Start();
            SetScreenNames();
        }

        /// <summary>
        /// Returns screens that are connected to PC
        /// </summary>
        /// <returns>Array of screens</returns>
        public Screen[] GetScreens()
        {
            return _screens;
        }

        /// <summary>
        /// Returns edited names of screens, the screen numbers does not match with Windows numbers
        /// </summary>
        /// <returns>Array of screen names</returns>
        public IEnumerable<string> GetScreenNames()
        {
            return _screenNames;
        }

        /// <summary>
        /// Called by timer, to check if screen got connected or disconnected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReloadScreens(object? sender, EventArgs e)
        {
            if (_mainWindow.OpenedTimer) return;
            _screens = Screen.AllScreens;
            _screenNames = new string[_screens.Length];
            SetScreenNames();
            _mainWindow.ShowReloadedScreens(_screenNames);
        }

        /// <summary>
        /// Setting screen names for displaying
        /// </summary>
        private void SetScreenNames()
        {
            for (var i = 0; i < _screens.Length; i++)
            {
                _screenNames[i] = "Display " + Regex.Match(_screens[i].DeviceName, @"\d+").Value;
            }
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }
    }
}
