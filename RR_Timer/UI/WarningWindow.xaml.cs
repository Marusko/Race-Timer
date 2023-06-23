using System;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow
    {
        public const int ApiLinkWarning = 0;
        public const int CountLinkWarning = 1;
        public const int TimeWarning = 2;

        /// <summary>
        /// When one of the constants is passed this constructor will be called
        /// </summary>
        /// <param name="type">Type of message</param>
        public WarningWindow(int type)
        {
            InitializeComponent();
            SetLabel(type);
        }

        /// <summary>
        /// When message is passed, this constructor will be called
        /// </summary>
        /// <param name="message">Message to show as warning</param>
        public WarningWindow(string message)
        {
            InitializeComponent();
            WarningLabel.Text = message;
        }

        /// <summary>
        /// If constant is passed to constructor, this method sets the message to show
        /// </summary>
        /// <param name="type">Type of message</param>
        private void SetLabel(int type)
        {
            WarningLabel.Text = type switch
            {
                0 => "API link was not entered or incorrect!\nClosing the timer window!",
                1 => "Count link was not entered or incorrect!\nMaking the timer smaller is now manual",
                2 => "Time was not entered or incorrect in some of the start times!\nClosing the timer window!",
                _ => WarningLabel.Text
            };
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
