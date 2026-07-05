using System;
using System.Media;
using System.Windows.Media;

namespace Race_timer.UI
{
    /// <summary>
    /// Kind of message shown by <see cref="WarningWindow"/>, drives the icon badge and sound
    /// </summary>
    public enum WarningKind
    {
        Info,
        Warning,
        Error,
        Success
    }

    /// <summary>
    /// Interaction logic for WarningWindow.xaml
    /// A modal dialog styled with the application theme, used in place of MessageBox.
    /// Shows a colored icon badge and mirrors the system sound MessageBox plays.
    /// </summary>
    public partial class WarningWindow
    {
        public const int ApiLinkWarning = 0;
        public const int CountLinkWarning = 1;
        public const int TimeWarning = 2;

        private SystemSound? _sound;

        /// <summary>
        /// When one of the constants is passed this constructor will be called
        /// </summary>
        /// <param name="type">Type of message</param>
        public WarningWindow(int type)
        {
            InitializeComponent();
            SetByType(type);
        }

        /// <summary>
        /// When a message is passed, this constructor will be called, shown as an error
        /// </summary>
        /// <param name="message">Message to show as warning</param>
        public WarningWindow(string message) : this("Error", message, WarningKind.Error)
        {
        }

        /// <summary>
        /// Full control constructor, sets the title, message, icon and sound
        /// </summary>
        /// <param name="title">Short title</param>
        /// <param name="message">Message body</param>
        /// <param name="kind">Kind of message</param>
        public WarningWindow(string title, string message, WarningKind kind)
        {
            InitializeComponent();
            SetContent(title, message, kind);
        }

        /// <summary>
        /// Maps the legacy int type to a title, message and kind
        /// </summary>
        /// <param name="type">Type of message</param>
        private void SetByType(int type)
        {
            switch (type)
            {
                case ApiLinkWarning:
                    SetContent("API link", "API link was not entered or incorrect!\nClosing the timer window!", WarningKind.Error);
                    break;
                case CountLinkWarning:
                    SetContent("Count link", "Count link was not entered or incorrect!\nMaking the timer smaller is now manual", WarningKind.Warning);
                    break;
                case TimeWarning:
                    SetContent("Start time", "Time was not entered or incorrect in some of the start times!\nClosing the timer window!", WarningKind.Error);
                    break;
                default:
                    SetContent("Warning", "", WarningKind.Warning);
                    break;
            }
        }

        /// <summary>
        /// Sets the labels, the icon badge and picks the sound to play when shown
        /// </summary>
        /// <param name="title">Short title</param>
        /// <param name="message">Message body</param>
        /// <param name="kind">Kind of message</param>
        private void SetContent(string title, string message, WarningKind kind)
        {
            TitleText.Text = title;
            WarningLabel.Text = message;

            var (background, foreground, glyph) = kind switch
            {
                WarningKind.Error => ("ErrorSoftBrush", "ErrorBrush", "✕"),
                WarningKind.Warning => ("WarningSoftBrush", "WarningBrush", "!"),
                WarningKind.Success => ("SuccessSoftBrush", "SuccessBrush", "✓"),
                _ => ("InfoSoftBrush", "InfoBrush", "i"),
            };
            IconBadge.Background = (Brush)FindResource(background);
            IconGlyph.Foreground = (Brush)FindResource(foreground);
            IconGlyph.Text = glyph;

            // Mirror the sounds MessageBox plays for each MessageBoxImage.
            _sound = kind switch
            {
                WarningKind.Error => SystemSounds.Hand,
                WarningKind.Warning => SystemSounds.Exclamation,
                WarningKind.Success => SystemSounds.Asterisk,
                _ => SystemSounds.Asterisk,
            };
            Loaded += (_, _) => _sound?.Play();
        }

        /// <summary>
        /// Close this warning window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
