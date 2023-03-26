using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow : Window
    {
        public static int API_LINK_WARNING = 0;
        public static int LIST_LINK_WARNING = 1;
        public static int TIME_WARNING = 2;
        
        public WarningWindow(int type)
        {
            InitializeComponent();
            SetLabel(type);
        }

        public WarningWindow(string message)
        {
            InitializeComponent();
            WarningLabel.Content = message;
        }

        private void SetLabel(int type)
        {
            switch (type)
            {
                case 0: WarningLabel.Content = "API link was not entered or incorrect!\nEvent name and type are not set";
                    break;
                case 1: WarningLabel.Content = "List link was not entered or incorrect!\nMaking the timer smaller is now manual";
                    break;
                case 2: WarningLabel.Content = "Time was not entered or incorrect!\nTime has been set to 00:00";
                    break;
            }
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
