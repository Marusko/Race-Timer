using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RR_Timer
{
    public class ScreenHandler
    {
        private Screen[] _screens = Screen.AllScreens;
        public Screen SelectedScreen { get; set; }
        public ScreenHandler() { }

        public Screen[] GetScreens()
        {
            return _screens;
        }

        public void reloadScreens()
        {
            _screens = Screen.AllScreens;
        }
    }
}
