using System;
using System.Media;
using System.Windows;
using ClickWrap;
using Race_timer.Logic;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// A modal dialog shown at start when the ClickWrap server has a newer version,
    /// styled like <see cref="WarningWindow"/>
    /// </summary>
    public partial class UpdateWindow
    {
        /// <summary>
        /// Shows the found version, release notes when there are any, and offers to update
        /// </summary>
        /// <param name="update">Newer version found on the server</param>
        public UpdateWindow(UpdateInfo update)
        {
            InitializeComponent();

            VersionText.Text = $"Race Timer {update.LatestVersion} is available, this is {UpdateChecker.CurrentVersion}.";

            if (!string.IsNullOrWhiteSpace(update.ReleaseNotes))
            {
                NotesText.Text = update.ReleaseNotes.Trim();
                NotesPanel.Visibility = Visibility.Visible;
            }

            //Without the installer's update.exe there is nothing to hand off to, only tell the user
            if (!UpdateChecker.CanUpdate)
            {
                UpdateButton.Visibility = Visibility.Collapsed;
                NoUpdaterText.Visibility = Visibility.Visible;
                LaterButton.Content = "OK";
            }

            Loaded += (_, _) => SystemSounds.Asterisk.Play();
        }

        /// <summary>
        /// Starts the updater and closes the app, it must not keep running next to the copy
        /// setup.exe starts when it is done
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Update(object sender, RoutedEventArgs e)
        {
            if (UpdateChecker.StartUpdate()) return;

            //Nothing to start, the app is untouched and still running
            Close();
            var w = new WarningWindow("Update", "Could not find the updater!\nDownload and run the Race Timer installer again to update.", WarningKind.Warning);
            w.ShowDialog();
        }

        /// <summary>
        /// Close this update window, the app keeps running on the current version
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
    }
}
