﻿using System.Windows;
using System.Windows.Media.Imaging;
using Race_timer.Logic;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for CodeWindowForMinimized.xaml
    /// </summary>
    public partial class CodeWindowForMinimized
    {
        private readonly ScreenHandler _screenHandler;

        /// <summary>
        /// Initializes component, adds method when window is loaded
        /// </summary>
        /// <param name="sh">Already created ScreenHandler</param>
        public CodeWindowForMinimized(ScreenHandler sh)
        {
            InitializeComponent();
            _screenHandler = sh;

            if (_screenHandler.SelectedScreen == null) return;
            WindowState = WindowState.Minimized;
            var half = ((double)_screenHandler.SelectedScreen.WorkingArea.Width / 2) - (Width / 2);
            Left = _screenHandler.SelectedScreen.WorkingArea.Left + half;
            Top = _screenHandler.SelectedScreen.WorkingArea.Top;

            Loaded += WindowLoaded;
        }

        /// <summary>
        /// Method called after window is loaded, sets the position, state and width and height of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        /// <summary>
        /// Sets the QR code/image to be shown
        /// </summary>
        /// <param name="image">QR code/image to be shown</param>
        public void SetImage(BitmapSource image)
        {
            CodeForMinimized.Source = image;
        }
    }
}
