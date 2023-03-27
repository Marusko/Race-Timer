using System;

namespace RR_Timer.UI
{
    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// </summary>
    public partial class WarningWindow
    {
        public const int ApiLinkWarning = 0;
        public const int ListLinkWarning = 1;
        public const int TimeWarning = 2;

        public WarningWindow(int type)
        {
            InitializeComponent();
            SetLabel(type);
        }

        public WarningWindow(string message)
        {
            InitializeComponent();
            WarningLabel.Content = message;
        }

        private void SetLabel(int type)
        {
            WarningLabel.Content = type switch
            {
                0 => "API link was not entered or incorrect!\nEvent name and type are not set",
                1 => "List link was not entered or incorrect!\nMaking the timer smaller is now manual",
                2 => "Time was not entered or incorrect!\nTime has been set to 00:00",
                _ => WarningLabel.Content
            };
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
