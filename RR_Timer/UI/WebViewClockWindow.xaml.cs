using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Race_timer.ClockUserControl;
using Race_timer.Logic;
using Race_timer.Logic.Interfaces;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for WebViewClockWindow.xaml
    /// </summary>
    public partial class WebViewClockWindow:IClockWindow
    {
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private int _showTimerIndex;

        private const int TimerShownForSeconds = 5;

        private bool _clockInMiniPanel;
        public MiniContestTimer? Clock { get; set; }

        /// <summary>
        /// Initializes the window, adds method to call after window is loaded
        /// If possible, start timer
        /// </summary>
        public WebViewClockWindow(string link)
        {
            InitializeComponent();

            WebView.Source = new Uri(link);

            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            WindowState = WindowState.Minimized;
            Left = ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Left;
            Top = ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Top;

            Loaded += WindowLoaded;
            Closed += StopTimer;
            _timer.Tick += TimerTick;
            _timer.Interval = new TimeSpan(0, 0, TimerShownForSeconds);
            TimerTickLogic();
            if (ClockLogic.GetInstance().MiniActiveTimers.Count > 0)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Sets the label to correct name
        /// </summary>
        /// <param name="name">Event name to show</param>
        private void SetEventName(string name)
        {
            EventNameMini.Text = name;
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StopTimer(object? sender, EventArgs e)
        {
            _timer.Stop();
        }

        /// <summary>
        /// Method called by timer, call TimerTickLogic()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerTick(object? sender, EventArgs e)
        {
            TimerTickLogic();
        }

        /// <summary>
        /// Switches between start times every 5 seconds
        /// </summary>
        public void TimerTickLogic()
        {
            if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count > 0)
            {
                if (_showTimerIndex >= ClockLogic.GetInstance().MiniActiveTimers.Values.Count)
                {
                    _showTimerIndex = 0;
                }
                TimerStackPanel.Children.Clear();
                TimerStackPanel.Children.Add(ClockLogic.GetInstance().MiniActiveTimers.Values.ElementAt(_showTimerIndex));
                _showTimerIndex++;
            }
        }

        public void TimerClickLogic()
        {
            Clock?.TimerClickLogic();
        }

        /// <summary>
        /// Method to update timer label with correct time
        /// Method called by ClockLogic timer
        /// </summary>
        public void OnTimerClick()
        {
            ShowMiniClockOrTimer(ref TimerStackPanel);
        }

        public void SetLabels(string name, string type)
        {
            SetEventName(name);
        }

        /// <summary>
        /// If current time is less than start time show clock, else show timer in minimized clock
        /// </summary>
        /// <param name="timers">Timer StackPanel from minimized clock</param>
        private void ShowMiniClockOrTimer(ref StackPanel timers)
        {
            if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count == 0 && timers.Children.Count == 0)
            {
                AddClock(ref timers);
            }
            else if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count == 0)
            {
                if (ScreenHandler.GetInstance().SelectedScreen == null) return;
                if (_clockInMiniPanel) return;
                TimerStackPanel.Children.Clear();
                _timer.Stop();
                AddClock(ref timers);
            }
            else if (ClockLogic.GetInstance().MiniActiveTimers.Values.Count > 0)
            {
                if (_clockInMiniPanel)
                {
                    TimerTickLogic();
                    _clockInMiniPanel = false;
                    Clock = null;
                    _timer.Start();
                }
            }
        }

        /// <summary>
        /// Add clock to timers stack panel
        /// </summary>
        /// <param name="timers">Timer StackPanel from minimized clock</param>
        private void AddClock(ref StackPanel timers)
        {
            timers.Children.Clear();
            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            var clock = new MiniContestTimer(ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Width, true)
            {
                Name = " "
            };
            timers.Children.Add(clock);
            _clockInMiniPanel = true;
            Clock = clock;
        }

        /// <summary>
        /// Method called after window is loaded, sets the position, state and width of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 3 - 5);

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlWidth = (double)ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Width / 3 - 50;
            Application.Current.Resources.Remove("ControlWidth");
            Application.Current.Resources.Add("ControlWidth", controlWidth);
        }
        /// <summary>
        /// Method sets chosen image to TimerImage, best used for rectangle logo
        /// </summary>
        /// <param name="image">Image to be shown</param>
        public void SetImage(BitmapImage image)
        {
            TimerImage.Source = image;
        }

        public void SetCodeImage(BitmapSource image)
        {
            
        }

        public void SetChildren(UserControl alignment)
        {
            
        }
    }
}
