using System.Windows.Controls;

namespace Race_timer.ClockUserControl
{
    /// <summary>
    /// Interaction logic for TimerLeft.xaml
    /// </summary>
    public partial class TimerLeft
    {
        public override StackPanel Timers => TimerStackPanel;
        public override Label MainClock => MainClockLabel;
        public override Image Logo => TimerImage;
        public override Image Code => CodeImage;
        protected override ScrollViewer Scroller => TimerScrollViewer;

        /// <summary>
        /// Initialize component, base starts the scroll timer
        /// </summary>
        /// <param name="screenWidth">Width of selected screen</param>
        public TimerLeft(int screenWidth) : base(screenWidth)
        {
            InitializeComponent();
        }
    }
}
