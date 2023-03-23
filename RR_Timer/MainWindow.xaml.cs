using System;
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

namespace RR_Timer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));
        }

        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            ClockWindow clockWindow = new ClockWindow(EventNameText.Text, EventTypeComboBox.Text, StartTime.Text);
            clockWindow.Show();
        }
    }
}
