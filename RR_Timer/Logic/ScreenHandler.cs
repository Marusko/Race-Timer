using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RR_Timer.Logic
{
    public class ScreenHandler
    {
        private Screen[] _screens = Screen.AllScreens;
        private string[] _screenNames;
        public Screen SelectedScreen { get; set; }
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        private MainWindow _mainWindow;

        public ScreenHandler(MainWindow mw)
        {
            _mainWindow = mw;
            _screenNames = new string[_screens.Length];
            Timer.Tick += ReloadScreens;
            Timer.Interval = new TimeSpan(0, 0, 30);
            Timer.Start();
            SetScreenNames();
        }

        public Screen[] GetScreens()
        {
            return _screens;
        }

        public string[] GetScreenNames()
        {
            return _screenNames;
        }

        private void ReloadScreens(object sender, EventArgs e)
        {
            _screens = Screen.AllScreens;
            _screenNames = new string[_screens.Length];
            SetScreenNames();
            _mainWindow.ShowReloadedScreens(_screenNames);
        }

        private void SetScreenNames()
        {
            for (int i = 0; i < _screens.Length; i++)
            {
                _screenNames[i] = "Display " + Regex.Match(_screens[i].DeviceName, @"\d+").Value;
            }
        }
    }
}
