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

        private void OnClose(object? sender, EventArgs e)
        {
            
        }
    }
}
