using System.Windows;
using System.Windows.Controls;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for TimerTop.xaml
    /// </summary>
    public partial class TimerTop
    {
        public override StackPanel Timers => TimerStackPanel;
        public override Label MainClock => MainClockLabel;
        public override Image Logo => TimerImage;
        protected override ScrollViewer Scroller => TimerScrollViewer;

        /// <summary>
        /// Initialize component, base starts the scroll timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        public TimerTop(int screenWidth) : base(screenWidth)
        {
            InitializeComponent();
        }

        /// <summary>
        /// Set the top margin according to length of event name
        /// </summary>
        /// <param name="nameLength"></param>
        public void SetTopMargin(int nameLength)
        {
            if (nameLength is <= 26 and > 0)
            {
                TimerGrid.Margin = new Thickness(0, 10, 0, 0);
            }

            if (nameLength == 0)
            {
                TimerGrid.Margin = new Thickness(0, -50, 0, 0);
            }
        }

        protected override bool NeedsScroll() => TimerStackPanel.ActualWidth > ScreenWidth;
        protected override double ScrollOffset() => TimerScrollViewer.HorizontalOffset;
        protected override double ScrollableExtent() => TimerScrollViewer.ScrollableWidth;
        protected override void ScrollToOffset(double offset) => TimerScrollViewer.ScrollToHorizontalOffset(offset);
        protected override void ScrollToStart() => TimerScrollViewer.ScrollToLeftEnd();
    }
}
