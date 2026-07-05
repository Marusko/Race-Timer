using System;
using System.Windows;
using System.Windows.Controls;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for WebViewClockWindow.xaml
    /// </summary>
    public partial class WebViewClockWindow
    {
        protected override TextBlock EventNameText => EventNameMini;
        protected override StackPanel TimersPanel => TimerStackPanel;
        protected override Image LogoImage => TimerImage;
        protected override ScrollViewer EventNameScroller => EventNameScrollViewer;
        protected override WindowState LoadedWindowState => WindowState.Maximized;

        /// <summary>
        /// Initializes the window and WebView page, base sets up timers and position
        /// </summary>
        /// <param name="link">Link to show in WebView</param>
        public WebViewClockWindow(string link)
        {
            InitializeComponent();

            WebView.Source = new Uri(link);
            InitializeMiniClock();
        }
    }
}
