using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Race_timer.API;
using Race_timer.Data;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for RaceResultSetupWindow.xaml
    ///
    /// The one place the app writes to RaceResult. It logs in, lists the events of the account,
    /// asks which Simple API entries to create on the chosen one, creates what is missing, corrects
    /// what points somewhere else and leaves everything else alone - then shows the links it ended
    /// up with, to be copied and pasted into the timer exactly as a link copied out of RaceResult
    /// by hand. Nothing is filled in anywhere automatically, the rest of the app works as it always
    /// did
    /// </summary>
    public partial class RaceResultSetupWindow
    {
        /// <summary>Above this many events the picker offers a search box</summary>
        private const int SearchFromEvents = 5;

        /// <summary>
        /// The RaceResult session, created on the first login and logged out again when this window
        /// closes. Nothing else in the app uses it
        /// </summary>
        private RaceResultSetupService? _setup;

        /// <summary>Every event the login returned, the picker shows those matching its search box</summary>
        private readonly List<RaceResultEventItem> _events = new();

        /// <summary>
        /// Initializes the window with the login step showing
        /// </summary>
        public RaceResultSetupWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Method called by the login method radio buttons, swaps the user and password inputs for
        /// the API key box and back
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginMethodChanged(object sender, RoutedEventArgs e)
        {
            //The radios raise Checked while the XAML is still being parsed, before the rest of the
            //tree exists
            if (!IsInitialized) return;

            var apiKey = ApiKeyLoginRadio.IsChecked == true;
            UserLoginPanel.Visibility = apiKey ? Visibility.Collapsed : Visibility.Visible;
            ApiKeyLoginPanel.Visibility = apiKey ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Method called by the Log in button, opens the RaceResult session and fills the event
        /// picker with the upcoming events of the account
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Login(object sender, RoutedEventArgs e)
        {
            var byApiKey = ApiKeyLoginRadio.IsChecked == true;
            if (byApiKey && string.IsNullOrWhiteSpace(ApiKeyText.Text))
            {
                var w = new WarningWindow("Login", "Enter the API key!", WarningKind.Warning);
                w.ShowDialog();
                return;
            }

            var user = UserText.Text.Trim();
            if (!byApiKey && (string.IsNullOrWhiteSpace(user) || PasswordText.Password.Length == 0))
            {
                var w = new WarningWindow("Login", "Enter the user and password!", WarningKind.Warning);
                w.ShowDialog();
                return;
            }

            var server = string.IsNullOrWhiteSpace(ServerText.Text)
                ? RaceResultApiCatalog.DefaultServer
                : ServerText.Text.Trim();

            var credentials = new RaceResultCredentials
            {
                Server = server,
                UseHttps = HttpsCheckBox.IsChecked == true,
                User = byApiKey ? null : user,
                Password = byApiKey ? null : PasswordText.Password,
                ApiKey = byApiKey ? ApiKeyText.Text.Trim() : null
            };

            SetBusy(true, "Logging in...");
            try
            {
                _setup ??= new RaceResultSetupService();
                var events = await _setup.LoginAndListEventsAsync(credentials);
                PasswordText.Clear();

                _events.Clear();
                _events.AddRange(events.Select(ev =>
                    new RaceResultEventItem(ev.Id, ev.EventName, ev.EventDate, ev.EventLocation)));

                EventFilterText.Clear();
                EventFilterPanel.Visibility = _events.Count > SearchFromEvents
                    ? Visibility.Visible
                    : Visibility.Collapsed;
                ShowEvents();

                AccountText.Text = byApiKey
                    ? $"Logged in to {server} with an API key"
                    : $"Logged in to {server} as {user}";
                ShowStep(EventPanel, EventActions);

                ShowStatus(_events.Count == 0
                    ? "The account has no upcoming events."
                    : $"Loaded events: {_events.Count}");
            }
            catch (Exception ex)
            {
                ShowStatus(null);
                var w = new WarningWindow("RaceResult", ex.Message, WarningKind.Error);
                w.ShowDialog();
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Method called by the Log out button, ends the session and goes back to the login step
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void Logout(object sender, RoutedEventArgs e)
        {
            SetBusy(true, "Logging out...");
            try
            {
                if (_setup != null)
                {
                    await _setup.LogoutAsync();
                }
            }
            finally
            {
                _events.Clear();
                EventFilterText.Clear();
                ShowEvents();
                ShowStep(LoginPanel, LoginButton);
                SetBusy(false);
                ShowStatus(null);
            }
        }

        /// <summary>
        /// Method called by the search box, shows the events matching what was typed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void EventFilterChanged(object sender, TextChangedEventArgs e)
        {
            if (!IsInitialized) return;

            ShowEvents();
        }

        /// <summary>
        /// Fills the picker with the events matching its search box, keeping the selected one
        /// selected if it survived the filter and selecting the first one otherwise, so Create API
        /// always has something to work on. With nothing to show, the list gives way to a line
        /// saying which of the two reasons it is
        /// </summary>
        private void ShowEvents()
        {
            var selected = EventsList.SelectedItem as RaceResultEventItem;
            var visible = _events.Where(ev => ev.Matches(EventFilterText.Text)).ToList();

            EventsList.ItemsSource = visible;
            EventsList.SelectedItem = selected != null && visible.Contains(selected)
                ? selected
                : visible.FirstOrDefault();

            var empty = visible.Count == 0;
            EventsList.Visibility = empty ? Visibility.Collapsed : Visibility.Visible;
            NoEventsText.Visibility = empty ? Visibility.Visible : Visibility.Collapsed;
            NoEventsText.Text = _events.Count == 0
                ? "The account has no upcoming events."
                : "No event matches the search.";
        }

        /// <summary>
        /// Method called by the Create API button, asks which APIs to create, creates what the event
        /// is missing, corrects what points somewhere else, leaves everything else alone - and shows
        /// the links to copy out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void CreateApi(object sender, RoutedEventArgs e)
        {
            if (EventsList.SelectedItem is not RaceResultEventItem selectedEvent)
            {
                var w = new WarningWindow("Create API", "Select an event first!", WarningKind.Warning);
                w.ShowDialog();
                return;
            }

            if (_setup is not { IsLoggedIn: true })
            {
                var w = new WarningWindow("Create API", "You are not logged in to RaceResult!", WarningKind.Warning);
                w.ShowDialog();
                return;
            }

            var selection = new RaceResultApiSelectWindow(selectedEvent.Name) { Owner = this };
            if (selection.ShowDialog() != true) return;

            SetBusy(true, "Creating the APIs...");
            try
            {
                var result = await _setup.ProvisionAsync(selectedEvent.Id, selection.Selected);

                ApiLinkText.Text = result.ApiLink;
                StartsLinkText.Text = result.StartsLink ?? "";
                StartsLinkPanel.Visibility = result.StartsLink == null
                    ? Visibility.Collapsed
                    : Visibility.Visible;
                ResultSummaryText.Text = result.Describe(selectedEvent.Name);
                ShowStep(ResultPanel, ResultActions);
                ShowStatus("Copy the links and continue in the app as with a link copied from RaceResult.");

                //The key travels in the path, so a link built from a plain-HTTP login is one the app
                //warns about, the same way it warns about a pasted http:// link
                if (!result.ApiLink.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
                {
                    var w = new WarningWindow("Insecure connection",
                        "The links use HTTP without encryption, the access key can be intercepted. Using HTTPS is recommended.",
                        WarningKind.Warning);
                    w.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                ShowStatus(null);
                var w = new WarningWindow("Create API", ex.Message, WarningKind.Error);
                w.ShowDialog();
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Method called by the Back button, returns to the event picker so another event can be
        /// set up without logging in again
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BackToEvents(object sender, RoutedEventArgs e)
        {
            ShowStep(EventPanel, EventActions);
            ShowStatus(null);
        }

        /// <summary>
        /// Method called by the Copy button of the API link, puts the link on the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyApiLink(object sender, RoutedEventArgs e)
        {
            Copy(ApiLinkText.Text, "API link");
        }

        /// <summary>
        /// Method called by the Copy button of the starts API link, puts the link on the clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyStartsLink(object sender, RoutedEventArgs e)
        {
            Copy(StartsLinkText.Text, "Starts API link");
        }

        /// <summary>
        /// Puts one link on the clipboard and says so. The clipboard is owned by whichever process
        /// grabbed it last, so the call can fail and that is not worth a dialog
        /// </summary>
        /// <param name="link">Link to copy</param>
        /// <param name="name">Name of the link, for the status line</param>
        private void Copy(string link, string name)
        {
            if (string.IsNullOrWhiteSpace(link)) return;

            try
            {
                Clipboard.SetText(link);
                ShowStatus($"{name} copied to the clipboard.");
            }
            catch (Exception)
            {
                ShowStatus($"Can't copy the {name.ToLower()}, select it and copy it by hand.");
            }
        }

        /// <summary>
        /// Method called by the Close button, closes the window, which logs the session out
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// Shows one of the three steps and the buttons belonging to it, hiding the other two.
        /// Every step lives in the same cell, so only one can be shown at a time
        /// </summary>
        /// <param name="panel">Panel of the step to show</param>
        /// <param name="actions">Buttons of the step to show</param>
        private void ShowStep(UIElement panel, UIElement actions)
        {
            LoginPanel.Visibility = panel == LoginPanel ? Visibility.Visible : Visibility.Collapsed;
            EventPanel.Visibility = panel == EventPanel ? Visibility.Visible : Visibility.Collapsed;
            ResultPanel.Visibility = panel == ResultPanel ? Visibility.Visible : Visibility.Collapsed;

            LoginButton.Visibility = actions == LoginButton ? Visibility.Visible : Visibility.Collapsed;
            EventActions.Visibility = actions == EventActions ? Visibility.Visible : Visibility.Collapsed;
            ResultActions.Visibility = actions == ResultActions ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Disables the window while a call is in flight, so a second click cannot start a second one
        /// </summary>
        /// <param name="busy">Whether a call is running</param>
        /// <param name="status">Status to show, or null to leave the line as it is</param>
        private void SetBusy(bool busy, string? status = null)
        {
            LoginPanel.IsEnabled = !busy;
            LoginButton.IsEnabled = !busy;
            EventPanel.IsEnabled = !busy;
            EventActions.IsEnabled = !busy;
            ResultActions.IsEnabled = !busy;
            Cursor = busy ? System.Windows.Input.Cursors.Wait : System.Windows.Input.Cursors.Arrow;

            if (status != null)
            {
                ShowStatus(status);
            }
        }

        /// <summary>
        /// The line under the buttons. It keeps its space when empty, so saying something and
        /// saying nothing are the same height
        /// </summary>
        /// <param name="message">Message to show, or null to clear the line</param>
        private void ShowStatus(string? message)
        {
            StatusText.Text = message ?? "";
        }

        /// <summary>
        /// Called when the window closes, ends the RaceResult session the login opened
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WindowClosed(object? sender, EventArgs e)
        {
            _setup?.Dispose();
            _setup = null;
        }
    }
}
