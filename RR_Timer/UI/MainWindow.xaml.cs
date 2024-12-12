using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using Race_timer.API;
using Race_timer.ClockUserControl;
using Race_timer.Data;
using Race_timer.Logic;
using Application = System.Windows.Application;
using Clipboard = System.Windows.Clipboard;

namespace Race_timer.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const string Author = "Matúš Suský";
        private const string Version = "2.6.5";
        private Window? _clockWindow;
        private bool _openedLinkTimer;
        public bool OpenedTimer { get; private set; }
        public bool MinimizedTimer { get; private set; }
        public bool CanOpenTimer { get; set; }
        public int StartTimeCount { get; set; }
        public List<int> UsedIndexes { get; }
        public Dictionary<string,string> StartTimes { get; }

        public string EventLink { get; set; } = "";
        public string CountLink { get; set; } = "";
        public string ContestLink { get; set; } = "";

        /// <summary>
        /// Initializes and shows the main window, creates new ScreenHandler and ClockLogic,
        /// sets values for event type combobox and screen combobox from screen handler and selects first screen in list
        /// sets OnCloseCloseTimerWindow to be called when closing main window and calls SetInfoLabel() method
        /// Sets ControlTab as disabled
        /// Sets alignment and Qr code time events
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            ScreenHandler.Initialize(this);
            ClockLogic.Initialize(this);

            EventTypeComboBox.ItemsSource = Enum.GetValues(typeof(EventType));

            ScreenComboBox.ItemsSource = ScreenHandler.GetInstance().GetScreenNames();
            ScreenComboBox.SelectionChanged += SelectScreen;
            ScreenComboBox.SelectedIndex = 0;

            AlignmentComboBox.ItemsSource = ClockLogic.GetInstance().GetAlignments();
            AlignmentComboBox.SelectedIndex = 0;
            AlignmentComboBox.SelectionChanged += SelectAlignmentHandler;

            StartTimesDataGrid.AutoGenerateColumns = false;

            MinimizedCodeTimes.EveryTimeText.TextChanged += CheckIfInt;
            MinimizedCodeTimes.HowLongTimeText.TextChanged += CheckIfInt;
            MinimizedTimer = false;
            CanOpenTimer = true;
            ControlTab.IsEnabled = false;
            MaximizeButton.IsEnabled = false;
            WebReloadButton.IsEnabled = false;
            StartTimeCount = 0;
            UsedIndexes = new List<int>();
            StartTimes = new Dictionary<string,string>();
            Closed += ShutDownApp;
            SetInfoLabel();
        }

        /// <summary>
        /// Method called by Open timer button, only if clock window is not opened creates and shows new fullscreen clock window,
        /// sets it in clock logic, sets the properties and starts the timer
        /// Disables all tabs, enables control tab and selects it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenTimer(object sender, RoutedEventArgs e)
        {
            if (!CanOpenTimer)
            {
                OpenedTimer = false;
                CanOpenTimer = true;
                return;
            }
            OpenTimerStart();
            if (!CanOpenTimer)
            {
                OpenedTimer = false;
                CanOpenTimer = true;
                return;
            }
            if (_clockWindow == null) return;
            ClockLogic.GetInstance().SetClockWindow((ClockWindow)_clockWindow, EventNameText.Text, EventTypeComboBox.Text);
            OpenTimerEnd();
            _clockWindow.Show();
            _openedLinkTimer = false;
        }

        /// <summary>
        /// Method called by Open timer button in API timer menu, only if clock window is not opened creates and shows
        /// new fullscreen clock window, sets it in clock logic, sets the properties and starts the timer
        /// Disables all tabs, enables control tab and selects it
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenLinkTimer(object sender, RoutedEventArgs e)
        {
            if (!CanOpenTimer)
            {
                OpenedTimer = false;
                CanOpenTimer = true;
                return;
            }
            OpenTimerStart();
            if (!CanOpenTimer)
            {
                OpenedTimer = false;
                CanOpenTimer = true;
                return;
            }
            if (_clockWindow == null) return;
            ClockLogic.GetInstance().SetClockWindow(EventLink, CountLink, (ClockWindow)_clockWindow);
            OpenTimerEnd();
            _clockWindow.Show();
            _openedLinkTimer = true;
        }

        /// <summary>
        /// Process the start times, set clock window and alignment, set contest combobox and datagrid content
        /// </summary>
        private void OpenTimerStart()
        {
            SaveStartTimesToDictionary();
            ContestComboBox.IsEnabled = true;
            NewStartTime.IsEnabled = true;
            NewStartButton.IsEnabled = true;
            if (StartTimeCount > 0)
            {
                StartTimesDataGrid.Items.Clear();
                ContestComboBox.ItemsSource = null;
                ContestComboBox.ItemsSource = StartTimes.Keys;
                foreach (var startTime in StartTimes)
                {
                    StartTimesDataGrid.Items.Add(startTime);
                }
            }
            else
            {
                ContestComboBox.IsEnabled = false;
                NewStartTime.IsEnabled = false;
                NewStartButton.IsEnabled = false;
            }
            ClockLogic.GetInstance().ProcessStartTimes();
            if (OpenedTimer) return;
            if (!CanOpenTimer) return;
            OpenedTimer = true;
            _clockWindow = new ClockWindow();
            ClockLogic.GetInstance().SelectedAlignment = null;
            SelectAlignment();
            ClockLogic.GetInstance().InfoText = InfoText.Text;
            if (InfoEnableCheckBox.IsChecked != null)
            {
                NewInfoButton.IsEnabled = InfoEnableCheckBox.IsChecked.Value;
                NewInfoText.IsEnabled = InfoEnableCheckBox.IsChecked.Value;
                if (InfoEnableCheckBox.IsChecked.Value)
                {
                    NewInfoText.Text = ClockLogic.GetInstance().InfoText;
                }
            }
        }

        /// <summary>
        /// Start clock window timer, disable unnecessary tabs
        /// </summary>
        private void OpenTimerEnd()
        {
            ClockLogic.GetInstance().StartTimer();
            LinkTimerTab.IsEnabled = false;
            TimerTab.IsEnabled = false;
            SettingsTab.IsEnabled = false;
            ControlTab.IsEnabled = true;
            CodeTab.IsEnabled = false;
            ContestsTab.IsEnabled = false;
            ResultTab.IsEnabled = false;
            TabControl.SelectedItem = ControlTab;
        }

        /// <summary>
        /// Method called by Minimize button, calls MinimizeTimer()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MinimizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MinimizeTimer();
        }

        /// <summary>
        /// Method called by Maximize button, calls MaximizeTimer()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MaximizeTimerHandler(object sender, RoutedEventArgs e)
        {
            MaximizeTimer();
        }

        /// <summary>
        /// Closes the minimized clock window and open new fullscreen clock window, sets it in ClockLogic,
        /// sets the MinimizedTimer property to false
        /// Disables maximize button, enables minimize button
        /// </summary>
        private void MaximizeTimer()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            if (!MinimizedTimer) return;
            MinimizedTimer = false;
            _clockWindow.Close();
            _clockWindow = new ClockWindow();
            ClockLogic.GetInstance().SelectedAlignment = null;
            SelectAlignment();
            ClockLogic.GetInstance().SetClockWindow((ClockWindow)_clockWindow);
            ClockLogic.GetInstance().CodeWindowForMinimized?.Close();
            ClockLogic.GetInstance().CodeWindowForMinimized = null;
            ClockLogic.GetInstance().InfoWindow?.StopTimer();
            ClockLogic.GetInstance().InfoWindow?.Close();
            ClockLogic.GetInstance().InfoWindow = null;
            MinimizeButton.IsEnabled = true;
            WebReloadButton.IsEnabled = false;
            MaximizeButton.IsEnabled = false;
            ClockLogic.GetInstance().ClockTickLogic();
            _clockWindow.Show();
        }

        /// <summary>
        /// Closes the maximized clock window and open new minimized classic or WebView clock window,
        /// sets it in ClockLogic, sets the MinimizedTimer property to true
        /// Disables minimize button, enables maximize button
        /// </summary>
        public void MinimizeTimer()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            if (MinimizedTimer) return;
            MinimizedTimer = true;
            _clockWindow.Close();
            ClockLogic.GetInstance().CheckTimers(true);
            if (WebViewEnableCheckBox.IsChecked != null && (bool)WebViewEnableCheckBox.IsChecked 
                                                        && !string.IsNullOrEmpty(ResultLinkText.Text))
            {
                _clockWindow = new WebViewClockWindow(ResultLinkText.Text);
                ClockLogic.GetInstance().SetClockWindow((WebViewClockWindow)_clockWindow);
            }
            else
            {
                _clockWindow = new MiniClockWindow();
                ClockLogic.GetInstance().SetClockWindow((MiniClockWindow)_clockWindow);
            }
            ClockLogic.GetInstance().AfterCheckTimers();
            _clockWindow.Show();
            MinimizeButton.IsEnabled = false;
            if (_clockWindow?.GetType() == typeof(WebViewClockWindow))
            {
                WebReloadButton.IsEnabled = true;
            }
            MaximizeButton.IsEnabled = true;
        }

        /// <summary>
        /// Method called by Close timer button, calls OnCLose()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CloseTimer(object sender, RoutedEventArgs e)
        {
            OnClose();
        }

        /// <summary>
        /// Only if clock window is opened closes the clock window,
        /// sets both properties to false and stops the timer
        /// Enables all tabs, disables control tab and
        /// selects timer tab from which the timer was started
        /// </summary>
        public void OnClose()
        {
            if (!OpenedTimer) return;
            if (_clockWindow == null) return;
            _clockWindow.Close();
            OpenedTimer = false;
            MinimizedTimer = false;
            ClockLogic.GetInstance().StopTimer();
            ClockLogic.GetInstance().SelectedAlignment = null;
            ClockLogic.GetInstance().CodeWindowForMinimized?.Close();
            ClockLogic.GetInstance().CodeWindowForMinimized = null;
            ClockLogic.GetInstance().InfoWindow?.StopTimer();
            ClockLogic.GetInstance().InfoWindow?.Close();
            ClockLogic.GetInstance().InfoWindow = null;
            LinkTimerTab.IsEnabled = true;
            TimerTab.IsEnabled = true;
            SettingsTab.IsEnabled = true;
            ControlTab.IsEnabled = false;
            CodeTab.IsEnabled = true;
            ContestsTab.IsEnabled = true;
            ResultTab.IsEnabled = true;
            TabControl.SelectedItem = _openedLinkTimer ? LinkTimerTab : TimerTab;
            MinimizeButton.IsEnabled = true;
            WebReloadButton.IsEnabled = false;
            MaximizeButton.IsEnabled = false;
            NewStartTime.Text = "00:00:00";
            CanOpenTimer = true;
        }

        private void SetInfo(object? sender, EventArgs e)
        {
            ClockLogic.GetInstance().InfoText = NewInfoText.Text;
        }

        /// <summary>
        /// Called when main window is closed, shuts down the application, closes clock window calls OnCLose(),
        /// stops the screen handler timer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShutDownApp(object? sender, EventArgs e)
        {
            OnClose();
            ScreenHandler.GetInstance().StopTimer();
            Application.Current.Shutdown();
        }

        /// <summary>
        /// After selecting string from list, set it also in ScreenHandler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectScreen(object sender, RoutedEventArgs e)
        {
            try
            {
                ScreenHandler.GetInstance().SelectedScreen = ScreenHandler.GetInstance().GetScreens()[ScreenComboBox.SelectedIndex];
            }
            catch (Exception)
            {
                ScreenHandler.GetInstance().SelectedScreen = ScreenHandler.GetInstance().GetScreens()[0];
                ScreenComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Calls SelectAlignment, used with alignment combo box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SelectAlignmentHandler(object sender, RoutedEventArgs e)
        {
            SelectAlignment();
        }

        /// <summary>
        /// Method selects alignment and sets it in ClockLogic
        /// </summary>
        private void SelectAlignment()
        {
            if (ScreenHandler.GetInstance().SelectedScreen != null)
            {
                var selectedScreen = ScreenHandler.GetInstance().SelectedScreen;
                if (selectedScreen != null)
                    ClockLogic.GetInstance().SelectedAlignment = AlignmentComboBox.SelectedIndex switch
                    {
                        0 => new TimerTop(selectedScreen.WorkingArea.Width),
                        1 => new TimerLeft(selectedScreen.WorkingArea.Width),
                        2 => new TimerRight(selectedScreen.WorkingArea.Width),
                        _ => ClockLogic.GetInstance().SelectedAlignment
                    };
            }
        }

        /// <summary>
        /// Opens file dialog where user chooses image to show, saves it in ClockLogic.LogoImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenImage(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            ClockLogic.GetInstance().LogoImage = new BitmapImage(new Uri(op.FileName));
            DeleteImageButton.IsEnabled = true;
            LinkDeleteImageButton.IsEnabled = true;
            var tmp = op.FileName;
            var index = tmp.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var name = tmp[index..];
            ImageName.Content = name;
            LinkImageName.Content = name;
        }

        /// <summary>
        /// Delete the image from ClockLogic.LogoImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteImage(object sender, RoutedEventArgs e)
        {
            ClockLogic.GetInstance().LogoImage = null;
            DeleteImageButton.IsEnabled = false;
            ImageName.Content = "";
            LinkDeleteImageButton.IsEnabled = false;
            LinkImageName.Content = "";
        }

        /// <summary>
        /// Opens file dialog where user chooses QR code/image to show, saves it in ClockLogic.CodeImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCode(object sender, RoutedEventArgs e)
        {
            var op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
                        "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
                        "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() != System.Windows.Forms.DialogResult.OK) return;
            ClockLogic.GetInstance().CodeImage = new BitmapImage(new Uri(op.FileName));
            var tmp = op.FileName;
            var index = tmp.LastIndexOf(Path.DirectorySeparatorChar) + 1;
            var name = tmp.Substring(index, tmp.Length - index);
            CodeName.Content = name;
            DeleteCodeButton.IsEnabled = true;
        }

        /// <summary>
        /// Delete the image from ClockLogic.CodeImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeleteCode(object sender, RoutedEventArgs e)
        {
            ClockLogic.GetInstance().CodeImage = null;
            CodeName.Content = "";
            DeleteCodeButton.IsEnabled = false;
            CodeCheckBoxForMinimized.IsChecked = false;
        }

        /// <summary>
        /// Generates code from link entered in CodeLinkText and saves it in ClockLogic.CodeImage
        /// updates necessary labels
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GenerateCode(object sender, RoutedEventArgs e)
        {
            DeleteCodeButton.IsEnabled = true;
            ClockLogic.GetInstance().CodeImage = CodeGenerator.GenerateCode(CodeLinkText.Text);
            CodeName.Content = "QR code generated!";
            if (ClockLogic.GetInstance().CodeImage != null) return;
            var ww = new WarningWindow("Something went wrong! QR code was not generated");
            ww.ShowDialog();
        }

        /// <summary>
        /// Sets the info label with correct author and version
        /// </summary>
        private void SetInfoLabel()
        {
            InfoLabel.Content = "Made by: " + Author + "\nVersion: " + Version;
        }

        /// <summary>
        /// Returns if the QR code on mini timer check box is checked or not
        /// </summary>
        /// <returns>Returns value of QR code on mini timer checkbox</returns>
        public bool IsCodeOnMinimized()
        {
            if(CodeCheckBoxForMinimized.IsChecked == null) return false;
            return (bool)CodeCheckBoxForMinimized.IsChecked;
        }

        /// <summary>
        /// Returns values from QR code menu bottom two text boxes when check box is checked,
        /// for showing and hiding QR code when timer is small
        /// </summary>
        /// <returns>Pair of int as minutes</returns>
        public (int, int) GetCodeForMiniTimes()
        {
            var show = int.Parse(MinimizedCodeTimes.HowLongTimeText.Text);
            var every = int.Parse(MinimizedCodeTimes.EveryTimeText.Text);
            return (show, every);
        }

        /// <summary>
        /// Checks if values from QR code menu bottom two text boxes can be converted into int,
        /// if it is not a number shows warning window and sets the text box value to "1",
        /// also checks if it is between 1 and 60, if not sets text box value to one which is closer [1/60]
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckIfInt(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.GetType().Name != "TextBox") return;
                if (((System.Windows.Controls.TextBox)sender).Text.Length <= 0) return;
                var i = int.Parse(((System.Windows.Controls.TextBox)sender).Text);
                ((System.Windows.Controls.TextBox)sender).Text = i switch
                {
                    > 60 => "60",
                    <= 0 => "1",
                    _ => ((System.Windows.Controls.TextBox)sender).Text
                };
            }
            catch (Exception exception)
            {
                if (sender.GetType().Name == "TextBox")
                {
                    var w = new WarningWindow($"Oops, cannot convert this [{((System.Windows.Controls.TextBox)sender).Text}] to number\n[{exception.Message}]");
                    w.ShowDialog();
                    ((System.Windows.Controls.TextBox)sender).Text = "1";
                }
                else
                {
                    var w = new WarningWindow($"Oops, cannot convert this to number\n[{exception.Message}]");
                    w.ShowDialog();
                }
            }
        }

        /// <summary>
        /// Returns first unused index of start time
        /// </summary>
        /// <returns>First free index</returns>
        public int GetFirstFreeIndex()
        {
            var indexer = 1;
            while (UsedIndexes.Contains(indexer))
            {
                indexer++;
            }
            return indexer;
        }

        /// <summary>
        /// Adds Start time user control, increments start time count and adds used index to list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddStartTime(object sender, RoutedEventArgs e)
        {
            var index = GetFirstFreeIndex();
            ContestsStackPanel.Children.Add(new MainWindowStartTimes
            {
                Index = index,
                MainWindow = this
            });
            UsedIndexes.Add(index);
            StartTimeCount++;
            TimesNumberLabel.Content = StartTimeCount.ToString();
        }

        /// <summary>
        /// Copies contests start time and name to Dictionary
        /// </summary>
        private void SaveStartTimesToDictionary()
        {
            StartTimes.Clear();
            foreach (UIElement i in ContestsStackPanel.Children)
            {
                var startTime = (MainWindowStartTimes)i;
                try
                {
                    StartTimes.Add(startTime.StartNameText.Text, startTime.StartTimeText.Text);
                }
                catch (Exception e)
                {
                    var v = new WarningWindow($"Oops, something went wrong when processing start times\n[{e.Message}]");
                    v.ShowDialog();
                    CanOpenTimer = false;
                    OnClose();
                }
                
            }
        }

        /// <summary>
        /// Update contest start time in MainWindow and ClockLogic start time collections
        /// Update contest start time directly in (mini)contest timer
        /// Updates data grid with current start times
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SetNewStartTime(object sender, RoutedEventArgs e)
        {
            if (ContestComboBox?.SelectedItem == null) return;
            var newStart = ClockLogic.GetInstance().StringToDateTime(NewStartTime.Text);
            var canSetTime = ClockLogic.GetInstance().StartTimes.TryGetValue(ContestComboBox.SelectedItem.ToString(), out var selectedStartTime);
            if (!canSetTime) return;
            ClockLogic.GetInstance().StartTimes[ContestComboBox.SelectedItem.ToString()] = newStart;
            StartTimes[ContestComboBox.SelectedItem.ToString()] = NewStartTime.Text;
            if (ClockLogic.GetInstance().ActiveTimers.Count > 0)
            {
                var can = ClockLogic.GetInstance().ActiveTimers.TryGetValue(ContestComboBox.SelectedItem.ToString(), out var selectedContest);
                if (can && selectedContest != null)
                {
                    selectedContest.StartTime = newStart;
                }
            }
            else if (ClockLogic.GetInstance().MiniActiveTimers.Count > 0)
            {
                var can = ClockLogic.GetInstance().MiniActiveTimers.TryGetValue(ContestComboBox.SelectedItem.ToString(), out var selectedContest);
                if (can && selectedContest != null)
                {
                    selectedContest.StartTime = newStart;
                }
            }
            ClockLogic.GetInstance().ClockTickLogic(MinimizedTimer);
            StartTimesDataGrid.Items.Clear();
            foreach (var startTime in StartTimes)
            {
                StartTimesDataGrid.Items.Add(startTime);
            }
        }

        /// <summary>
        /// Reload opened WebView page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reload(object sender, RoutedEventArgs e)
        {
            if (_clockWindow?.GetType() == typeof(WebViewClockWindow))
            {
                if (((WebViewClockWindow)_clockWindow).WebView is { CoreWebView2: not null })
                {
                    ((WebViewClockWindow)_clockWindow).WebView.Reload();
                }
            }
        }

        /// <summary>
        /// Copy event link settings to system clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyEventLink(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(EventSetText.Text);
        }

        /// <summary>
        /// Copy count link settings to system clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyCountLink(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(CountSetText.Text);
        }

        /// <summary>
        /// Copy contest link settings to system clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyContestLink(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ContestSetText.Text);
        }

        /// <summary>
        /// Copy All API link settings to system clipboard
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyApiLink(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(ApiSetText.Text);
        }

        /// <summary>
        /// Load all API from link
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoadApis(object sender, RoutedEventArgs e)
        {
            LinkHandler.LoadApi(LinkText.Text, this);
        }

        /// <summary>
        /// Method called by Github hyperlink, opens project Github
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenGithub(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/Marusko/RR_Timer";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Method called by JSON framework hyperlink, opens JSON framework page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenJsonPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://www.newtonsoft.com/json";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Method called by Material design hyperlink, opens Material design Github page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenMaterialDesignPage(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/MaterialDesignInXAML/MaterialDesignInXamlToolkit";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }

        /// <summary>
        /// Method called by QR code generator hyperlink, opens QR code generator page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenCodePage(object sender, RoutedEventArgs e)
        {
            const string url = "https://github.com/codebude/QRCoder";
            Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
        }
    }
}
