using System;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Race_timer.ClockUserControl;
using Race_timer.Data;
using Race_timer.Logic;
using Race_timer.UI;

namespace Race_timer.API
{
    /// <summary>
    /// Handles retrieving data from links
    /// </summary>
    internal class LinkHandler
    {
        private static LinkHandler? _instance;

        private string _mainLink;
        private string? _countLink;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly HttpClient _httpClient;

        /// <summary>
        /// When both links are entered, this constructor will be called and main, count links will be read
        /// </summary>
        /// <param name="mainLink">For event name and type</param>
        /// <param name="countLink">For count of participants that already finished</param>
        private LinkHandler(string mainLink, string countLink)
        {
            _mainLink = mainLink;
            _countLink = countLink;
            _httpClient = new HttpClient();
            ReadMainLink();

            _timer.Tick += RefreshCountLink;
            _timer.Interval = new TimeSpan(0, 0, 15);
            _timer.Start();
        }
        /// <summary>
        /// When count link is not entered, this constructor will be called and main link will be read
        /// </summary>
        /// <param name="mainLink">For event name and type</param>
        private LinkHandler(string mainLink)
        {
            _mainLink = mainLink;
            _countLink = null;
            _httpClient = new HttpClient();
            ReadMainLink();
        }

        public static LinkHandler Initialize(string mainLink)
        {
            if (_instance == null)
            {
                _instance = new LinkHandler(mainLink);
            }
            else
            {
                _instance._mainLink = mainLink;
                _instance.ReadMainLink();
            }

            return _instance;
        }
        public static LinkHandler Initialize(string mainLink, string countLink)
        {
            if (_instance == null)
            {
                _instance = new LinkHandler(mainLink, countLink);
            }
            else
            {
                _instance._mainLink = mainLink;
                _instance._countLink = countLink;
                _instance.ReadMainLink();
                _instance._timer.Start();
            }

            return _instance;
        }

        public static LinkHandler GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("LinkHandler is not initialized. Call Initialize() first.");
            }
            return _instance;
        }

        /// <summary>
        /// Reads main link and sets name and type of event in ClockLogic, if something went wrong shows error
        /// and closes clock window
        /// </summary>
        private async void ReadMainLink()
        {
            HttpResponseMessage response;
            try
            {
                response = await _httpClient.GetAsync(_mainLink);
            }
            catch (Exception e)
            {
                var warning = new WarningWindow($"Oops, something went wrong with Event API!\nError code: \n[{e.Message}]");
                warning.ShowDialog();
                ClockLogic.GetInstance().MainWindow.CanOpenTimer = false;
                ClockLogic.GetInstance().MainWindow.EventStatusLabel.Content = "ERR";
                return;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Event? myEvent;
                try
                {
                    myEvent = JsonConvert.DeserializeObject<Event>(responseString);
                }
                catch (Exception)
                {
                    var warning = new WarningWindow("Oops, something went wrong with Event API!\nError code: [Can't deserialize provided data]");
                    warning.ShowDialog();
                    ClockLogic.GetInstance().MainWindow.OnClose();
                    ClockLogic.GetInstance().MainWindow.EventStatusLabel.Content = "ERR";
                    return;
                }
                if (myEvent?.EventName == null || myEvent.EventType == null)
                {
                    var warning = new WarningWindow("Oops, something went wrong with Event API!\nError code: \n[Can't read Event API link]");
                    warning.ShowDialog();
                    ClockLogic.GetInstance().MainWindow.CanOpenTimer = false;
                    ClockLogic.GetInstance().MainWindow.EventStatusLabel.Content = "ERR";
                    return;
                }
                ClockLogic.GetInstance().SetLabels(myEvent.EventName, myEvent.GetFormatedType());
            }
            else
            {
                var warning = new WarningWindow($"Oops, something went wrong with Event API!\nError code: [{response.StatusCode}]");
                warning.ShowDialog();
                ClockLogic.GetInstance().MainWindow.OnClose();
                ClockLogic.GetInstance().MainWindow.EventStatusLabel.Content = "ERR";
            }
        }

        /// <summary>
        /// Reads count link and if one or mor participants finished, it minimizes timer, if something went wrong shows error
        /// and stops the timer for refreshing count link
        /// </summary>
        private async void ReadCountLink()
        {
            if (!ClockLogic.GetInstance().IsTimerMinimized())
            {
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.GetAsync(_countLink);
                }
                catch (Exception e)
                {
                    _timer.Stop();
                    var warning = new WarningWindow($"Oops, something went wrong with count API!\nError code: \n[{e.Message}]");
                    warning.ShowDialog();
                    ClockLogic.GetInstance().MainWindow.CountStatusLabel.Content = "ERR";
                    return;
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();

                    var asRacers = int.Parse(responseString);
                    if (asRacers > 0 && !ClockLogic.GetInstance().IsTimerMinimized())
                    {
                        ClockLogic.GetInstance().AutoMinimizeTimer();
                        _timer.Stop();
                    }
                }
                else
                {
                    _timer.Stop();
                    var warning = new WarningWindow($"Oops, something went wrong with count API!\nError code: [{response.StatusCode}]");
                    warning.ShowDialog();
                    ClockLogic.GetInstance().MainWindow.CountStatusLabel.Content = "ERR";
                }
            }
            else
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Timer calls this method, which calls ReadCountLink()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshCountLink(object? sender, EventArgs e)
        {
            ReadCountLink();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }

        /// <summary>
        /// Reads the contest link and sets start times in main window with correct names and times,
        /// if something went wrong, shows warning window
        /// </summary>
        /// <param name="contestLink">For loading contests</param>
        /// <param name="mw">Already created main window</param>
        private static async void LoadContest(string contestLink, MainWindow mw)
        {
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(contestLink);
            }
            catch (Exception e)
            {
                var warning = new WarningWindow($"Oops, something went wrong with contest API!\nError code: \n[{e.Message}]");
                warning.ShowDialog();
                mw.ContestStatusLabel.Content = "ERR";
                httpClient.Dispose();
                return;
            }
            httpClient.Dispose();
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                List<Contest>? contests;
                try
                {
                    contests = JsonConvert.DeserializeObject<List<Contest>>(responseString);
                }
                catch (Exception)
                {
                    var warning = new WarningWindow("Oops, something went wrong with contest API!\nError code: \n[Can't deserialize provided data]");
                    warning.ShowDialog();
                    mw.ContestStatusLabel.Content = "ERR";
                    return;
                }
                if (contests == null)
                {
                    var warning = new WarningWindow("Oops, something went wrong with contest API!\nError code: \n[Contests from link are null]");
                    warning.ShowDialog();
                    mw.ContestStatusLabel.Content = "ERR";
                    return;
                }
                mw.ContestsStackPanel.Children.Clear();
                mw.StartTimeCount = 0;
                mw.TimesNumberLabel.Content = mw.StartTimeCount.ToString();
                mw.UsedIndexes.Clear();
                foreach (var contest in contests)
                {
                    var index = mw.GetFirstFreeIndex();
                    mw.ContestsStackPanel.Children.Add(new MainWindowStartTimes
                    {
                        Index = index,
                        MainWindow = mw,
                        StartTime = SecondsToTimeString(contest.StartTime),
                        Name = contest.Name
                    });
                    mw.UsedIndexes.Add(index);
                    mw.StartTimeCount++;
                    mw.TimesNumberLabel.Content = mw.StartTimeCount.ToString();
                }
            }
            else
            {
                var warning = new WarningWindow($"Oops, something went wrong with contest API!\nError code: [{response.StatusCode}]");
                warning.ShowDialog();
                mw.ContestStatusLabel.Content = "ERR";
            }
        }

        /// <summary>
        /// Reads all API link, sets Event, count and contest links and status and
        /// if it is possible calls LoadContest(), if something went wrong, shows warning window
        /// </summary>
        /// <param name="apiLink">For loading all APIs</param>
        /// <param name="mw">Already created main window</param>
        public static async void LoadApi(string apiLink, MainWindow mw)
        {
            var canLoadContest = false;
            var httpClient = new HttpClient();
            HttpResponseMessage response;
            try
            {
                response = await httpClient.GetAsync(apiLink);
            }
            catch (Exception e)
            {
                var warning = new WarningWindow($"Oops, something went wrong with all API link!\nError code: \n[{e.Message}]");
                warning.ShowDialog();
                httpClient.Dispose();
                return;
            }
            httpClient.Dispose();
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                List<Api>? apis;
                try
                {
                    apis = JsonConvert.DeserializeObject<List<Api>>(responseString);
                }
                catch (Exception)
                {
                    var warning = new WarningWindow("Oops, something went wrong with all API link!\nError code: \n[Can't deserialize provided data]");
                    warning.ShowDialog();
                    return;
                }
                if (apis == null)
                {
                    var warning = new WarningWindow("Oops, something went wrong with all API link!\nError code: \n[APIs from link are null]");
                    warning.ShowDialog();
                    return;
                }

                var index = apiLink.LastIndexOf("/", StringComparison.Ordinal);
                if (index == -1)
                {
                    var warning = new WarningWindow("Oops, something went wrong with all API link!\nError code: \n[Can't find main API link part]");
                    warning.ShowDialog();
                    return;
                }
                var link = apiLink.Substring(0, index + 1);

                mw.EventStatusLabel.Content = "MIS";
                mw.CountStatusLabel.Content = "MIS";
                mw.ContestStatusLabel.Content = "MIS";

                foreach (var api in apis)
                {
                    if ((bool)api.Label?.ToLower().Equals("main"))
                    {
                        mw.EventLink = "";
                        if (api.Disabled != null && !(bool)api.Disabled)
                        {
                            mw.EventStatusLabel.Content = "OK";
                            mw.EventLink = link + api.Key;
                        }
                        else
                        {
                            mw.EventStatusLabel.Content = "OFF";
                        }
                    }
                    else if ((bool)api.Label?.ToLower().Equals("count"))
                    {
                        mw.CountLink = "";
                        if (api.Disabled != null && !(bool)api.Disabled)
                        {
                            mw.CountStatusLabel.Content = "OK";
                            mw.CountLink = link + api.Key;
                        }
                        else
                        {
                            mw.CountStatusLabel.Content = "OFF";
                        }
                    }
                    else if ((bool)api.Label?.ToLower().Equals("contest"))
                    {
                        mw.ContestLink = "";
                        if (api.Disabled != null && !(bool)api.Disabled)
                        {
                            mw.ContestStatusLabel.Content = "OK";
                            mw.ContestLink = link + api.Key;
                            canLoadContest = true;
                        }
                        else
                        {
                            mw.ContestStatusLabel.Content = "OFF";
                        }
                    }
                }
            }
            else
            {
                var warning = new WarningWindow($"Oops, something went wrong with all API!\nError code: [{response.StatusCode}]");
                warning.ShowDialog();
            }

            if (canLoadContest)
            {
                LoadContest(mw.ContestLink, mw);
            }
        }

        /// <summary>
        /// Converts number of seconds to string in 00:00:00 format, or returns "00:00:00" if something went wrong
        /// </summary>
        /// <param name="seconds">Seconds number to convert</param>
        /// <returns>Time string in 00:00:00 format</returns>
        private static string SecondsToTimeString(int? seconds)
        {
            var timeString = "00:00:00";

            if (seconds != null)
            {
                TimeSpan time = TimeSpan.FromSeconds((double)seconds);
                timeString = time.ToString(@"hh\:mm\:ss");
            }

            return timeString;
        }
    }
}
