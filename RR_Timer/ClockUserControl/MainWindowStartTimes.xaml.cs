using System.Windows;
using Race_timer.UI;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for MainWindowStartTimes.xaml
    /// </summary>
    public partial class MainWindowStartTimes
    {
        public MainWindow? MainWindow { get; init; }
        public int Index
        {
            get => _index;
            init
            {
                IndexLabel.Content = value + ":";
                _index = value;
            }
        }

        public new string? Name
        {
            get => _name;
            init
            {
                StartNameText.Text = value;
                _name = value;
            }
        }
        public string? StartTime
        {
            get => _startTime;
            init
            {
                StartTimeText.Text = value;
                _startTime = value;
            }
        }

        private readonly string? _name;
        private readonly string? _startTime;

        private readonly int _index;

        public MainWindowStartTimes()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Removes this UserControl from StackPanel in Main Window, releases used index
        /// and decreases the number of start times
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Close(object sender, RoutedEventArgs e)
        {
            if (MainWindow == null) return;
            var parent = MainWindow.StartTimesStackPanel;
            parent.Children.Remove(this);
            MainWindow.UsedIndexes.Remove(Index);
            MainWindow.StartTimeCount--;
            MainWindow.TimesNumberLabel.Content = MainWindow.StartTimeCount.ToString();
        }
    }
}
