using System.Windows;
using System.Windows.Controls;
using Race_timer.Logic;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for MiniClockWindow.xaml
    /// </summary>
    public partial class MiniClockWindow
    {
        protected override TextBlock EventNameText => EventNameMini;
        protected override StackPanel TimersPanel => TimerStackPanel;
        protected override Image LogoImage => TimerImage;
        protected override ScrollViewer EventNameScroller => EventNameScrollViewer;
        protected override WindowState LoadedWindowState => WindowState.Normal;

        /// <summary>
        /// Initializes the window, sets width to screen width, base sets up timers and position
        /// </summary>
        public MiniClockWindow()
        {
            InitializeComponent();

            if (ScreenHandler.GetInstance().SelectedScreen != null)
            {
                Width = ScreenHandler.GetInstance().GetSelectedScreenArea().Width;
            }
            InitializeMiniClock();
        }
    }
}
