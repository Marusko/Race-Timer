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
        /// <summary>
        /// Initializes component, adds method when window is loaded
        /// </summary>
        public CodeWindowForMinimized()
        {
            InitializeComponent();

            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            WindowState = WindowState.Minimized;
            var half = ((double)ScreenHandler.GetInstance().GetSelectedScreenArea().Width / 2) - (Width / 2);
            Left = ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Left + half;
            Top = ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Top;

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
