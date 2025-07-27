using Race_timer.Logic;
using System;
using System.Windows;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for StartsWindow.xaml
    /// </summary>
    public partial class StartsWindow : Window
    {
        /// <summary>
        /// Initializes the window, adds method to call after window is loaded
        /// </summary>
        public StartsWindow()
        {
            InitializeComponent();

            if (ScreenHandler.GetInstance().SelectedScreen == null) return;
            WindowState = WindowState.Minimized;
            Left = ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Left;
            Top = ScreenHandler.GetInstance().SelectedScreen.WorkingArea.Top;

            Loaded += WindowLoaded;
            Closed += OnClose;
        }

        /// <summary>
        /// Method called after window is loaded, sets the position, state and width and height of window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;

            //Accepted answer from https://learn.microsoft.com/en-us/answers/questions/384918/how-to-scale-font-size-in-wpf
            var controlSize = (double)ScreenHandler.GetInstance().GetSelectedScreenArea().Width / 12 / 3 * 2 / 5 * 0.7;
            Application.Current.Resources.Remove("ControlFontSize");
            Application.Current.Resources.Add("ControlFontSize", controlSize * 10);
            Application.Current.Resources.Remove("ControlClockFontSize");
            Application.Current.Resources.Add("ControlClockFontSize", controlSize * 20);
            Application.Current.Resources.Remove("ControlSmallFontSize");
            Application.Current.Resources.Add("ControlSmallFontSize", controlSize * 5);
        }

        /// <summary>
        /// Not implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnClose(object? sender, EventArgs e)
        {
            
        }
    }
}
