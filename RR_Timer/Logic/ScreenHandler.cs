using Race_timer.UI;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Forms;

namespace Race_timer.Logic
{
    /// <summary>
    /// Handles loading and switching between PC screens
    /// </summary>
    public class ScreenHandler
    {
        private Dictionary<int, (int Width, int Height)> _dpiAwareSizes;
        public (int Width, int Height) SelectedScreenSize { get; private set; } 

        private static ScreenHandler? _instance;

        private Screen[] _screens = Screen.AllScreens;
        private string[] _screenNames;
        public Screen? SelectedScreen { get; private set; }
        private int _selectedScreenIndex;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly MainWindow _mainWindow;

        /// <summary>
        /// Sets up timer, names
        /// </summary>
        /// <param name="mw">Already created MainWindow object</param>
        private ScreenHandler(MainWindow mw)
        {
            _mainWindow = mw;
            _screenNames = new string[_screens.Length];
            _timer.Tick += ReloadScreens;
            _timer.Interval = new TimeSpan(0, 0, 20);
            _timer.Start();
            _dpiAwareSizes = new Dictionary<int, (int Width, int Height)>();
            //CheckScreenAreas();
            SetScreenNames();
        }

        public static ScreenHandler Initialize(MainWindow mw)
        {
            if (_instance == null)
            {
                _instance = new ScreenHandler(mw);
            }
            
            return _instance;
        }

        public static ScreenHandler GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("ScreenHandler is not initialized. Call Initialize() first.");
            }
            return _instance;
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

        public void SetSelectedScreen(int index)
        {
            _selectedScreenIndex = index;
            SelectedScreen = _screens[index];
        }

        public (int Width, int Height) GetSelectedScreenArea()
        {
            return SelectedScreenSize;
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
            _mainWindow.ScreenComboBox.ItemsSource = _screenNames;
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

        public void CheckScreenArea()
        {
            SelectedScreenSize = GetMaximizedDpiAwareSize(_selectedScreenIndex);
        }

        private void CheckScreenAreas()
        {
            for (var i = 0; i < _screens.Length; i++)
            {
                _dpiAwareSizes[i] = GetMaximizedDpiAwareSize(i);
            }
        }

        private (int Width, int Height) GetMaximizedDpiAwareSize(int screenIndex = 0)
        {
            if (screenIndex < 0 || screenIndex >= _screens.Length)
                throw new ArgumentOutOfRangeException(nameof(screenIndex));

            var screen = _screens[screenIndex];

            int width = 0;
            int height = 0;

            var dummyWindow = new Window
            {
                WindowStyle = WindowStyle.None,
                ResizeMode = ResizeMode.NoResize,
                ShowInTaskbar = false,
                WindowStartupLocation = WindowStartupLocation.Manual,
                Left = screen.Bounds.Left,
                Top = screen.Bounds.Top,
                Width = 0,
                Height = 0
            };

            dummyWindow.Loaded += (s, e) =>
            {
                dummyWindow.WindowState = WindowState.Maximized;

                // Delay to ensure layout update
                dummyWindow.Dispatcher.InvokeAsync(() =>
                {
                    width = (int)dummyWindow.Width;
                    height = (int)dummyWindow.Height;

                    dummyWindow.Close();
                }, System.Windows.Threading.DispatcherPriority.ApplicationIdle);
            };

            dummyWindow.ShowDialog(); // blocking, ensures we get values

            return (width, height);
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
