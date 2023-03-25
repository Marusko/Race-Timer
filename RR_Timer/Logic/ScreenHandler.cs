using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace RR_Timer.Logic
{
    public class ScreenHandler
    {
        private Screen[] _screens = Screen.AllScreens;
        public Screen SelectedScreen { get; set; }
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        private MainWindow _mainWindow;

        public ScreenHandler(MainWindow mw)
        {
            _mainWindow = mw;
            Timer.Tick += ReloadScreens;
            Timer.Interval = new TimeSpan(0, 0, 30);
            Timer.Start();
        }

        public Screen[] GetScreens()
        {
            return _screens;
        }

        private void ReloadScreens(object sender, EventArgs e)
        {
            _screens = Screen.AllScreens;
            _mainWindow.ShowReloadedScreens(_screens);
        }
    }
}
