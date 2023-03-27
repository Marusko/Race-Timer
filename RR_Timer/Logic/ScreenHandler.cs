using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using RR_Timer.UI;

namespace RR_Timer.Logic
{
    public class ScreenHandler
    {
        private Screen[] _screens = Screen.AllScreens;
        private string[] _screenNames;
        public Screen? SelectedScreen { get; set; }
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly MainWindow _mainWindow;

        public ScreenHandler(MainWindow mw)
        {
            _mainWindow = mw;
            _screenNames = new string[_screens.Length];
            _timer.Tick += ReloadScreens;
            _timer.Interval = new TimeSpan(0, 0, 20);
            _timer.Start();
            SetScreenNames();
        }

        public Screen[] GetScreens()
        {
            return _screens;
        }

        public IEnumerable<string> GetScreenNames()
        {
            return _screenNames;
        }

        private void ReloadScreens(object? sender, EventArgs e)
        {
            if (_mainWindow.OpenedTimer) return;
            _screens = Screen.AllScreens;
            _screenNames = new string[_screens.Length];
            SetScreenNames();
            _mainWindow.ShowReloadedScreens(_screenNames);
        }

        private void SetScreenNames()
        {
            for (var i = 0; i < _screens.Length; i++)
            {
                _screenNames[i] = "Display " + Regex.Match(_screens[i].DeviceName, @"\d+").Value;
            }
        }
    }
}
