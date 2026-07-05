using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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

        //one client reused for all requests, creating and disposing a client per request wastes connections
        private static readonly HttpClient SharedHttpClient = new();

        private string _mainLink;
        private string? _countLink;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();

        /// <summary>
        /// When both links are entered, this constructor will be called and main, count links will be read
        /// </summary>
        /// <param name="mainLink">For event name and type</param>
        /// <param name="countLink">For count of participants that already finished</param>
        private LinkHandler(string mainLink, string countLink)
        {
            _mainLink = mainLink;
            _countLink = countLink;
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
            ReadMainLink();
        }

        /// <summary>
        /// Method for initializing the LinkHandler with only main API link
        /// </summary>
        /// <param name="mainLink">Main API link</param>
        /// <returns>Instance of LinkHandler</returns>
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

        /// <summary>
        /// Method for initializing the LinkHandler with both main and count API links
        /// </summary>
        /// <param name="mainLink"></param>
        /// <param name="countLink"></param>
        /// <returns>Instance of LinkHandler</returns>
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

        /// <summary>
        /// Method for retrieving singleton instance of LinkHandler
        /// </summary>
        /// <returns>Singleton instance of LinkHandler</returns>
        /// <exception cref="InvalidOperationException">When LinkHandler is not initialized first</exception>
        public static LinkHandler GetInstance()
        {
            if (_instance == null)
            {
                throw new InvalidOperationException("LinkHandler is not initialized. Call Initialize() first.");
            }
            return _instance;
        }

        /// <summary>
        /// Shows unified warning window for failed API operations
        /// </summary>
        /// <param name="apiName">Name of the API that failed</param>
        /// <param name="detail">Error detail to show</param>
        private static void ShowApiError(string apiName, string detail)
        {
            var warning = new WarningWindow($"Oops, something went wrong with {apiName}!\nError code: \n[{detail}]");
            warning.ShowDialog();
        }

        /// <summary>
        /// Called when reading the event API fails, shows warning and sets the status label
        /// Does not close the clock window, ReadMainLink can fail synchronously during window opening
        /// (e.g. empty or invalid link) and closing it there would crash the pending Show() call
        /// </summary>
        /// <param name="detail">Error detail to show</param>
        private static void FailMainLink(string detail)
        {
            ShowApiError("Event API", detail);
            ClockLogic.GetInstance().MainWindow.EventStatusLabel.Content = "ERR";
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
                response = await SharedHttpClient.GetAsync(_mainLink);
            }
            catch (Exception e)
            {
                FailMainLink(e.Message);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                FailMainLink(response.StatusCode.ToString());
                return;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            Event? myEvent;
            try
            {
                myEvent = JsonConvert.DeserializeObject<Event>(responseString);
            }
            catch (Exception)
            {
                FailMainLink("Can't deserialize provided data");
                return;
            }
            if (myEvent?.EventName == null || myEvent.EventType == null)
            {
                FailMainLink("Can't read Event API link");
                return;
            }
            ClockLogic.GetInstance().SetLabels(myEvent.EventName, myEvent.GetFormatedType());
        }

        /// <summary>
        /// Called when reading the count API fails, stops the refresh timer,
        /// shows warning and sets the status label, auto minimize becomes manual
        /// </summary>
        /// <param name="detail">Error detail to show</param>
        private void FailCountLink(string detail)
        {
            _timer.Stop();
            ShowApiError("Count API", detail);
            ClockLogic.GetInstance().MainWindow.CountStatusLabel.Content = "ERR";
        }

        /// <summary>
        /// Reads count link and if one or more participants finished, it minimizes timer, if something went wrong shows error
        /// and stops the timer for refreshing count link
        /// </summary>
        private async void ReadCountLink()
        {
            if (ClockLogic.GetInstance().IsTimerMinimized())
            {
                _timer.Stop();
                return;
            }
            HttpResponseMessage response;
            try
            {
                response = await SharedHttpClient.GetAsync(_countLink);
            }
            catch (Exception e)
            {
                FailCountLink(e.Message);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                FailCountLink(response.StatusCode.ToString());
                return;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            if (!int.TryParse(responseString, out var asRacers))
            {
                FailCountLink($"Can't read finished count from '{responseString}'");
                return;
            }
            if (asRacers > 0 && !ClockLogic.GetInstance().IsTimerMinimized())
            {
                ClockLogic.GetInstance().AutoMinimizeTimer();
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
        /// Called when reading the contest API fails, shows warning and sets the status label
        /// </summary>
        /// <param name="mw">Already created main window</param>
        /// <param name="detail">Error detail to show</param>
        private static void FailContestLink(MainWindow mw, string detail)
        {
            ShowApiError("Contest API", detail);
            mw.ContestStatusLabel.Content = "ERR";
        }

        /// <summary>
        /// Reads the contest link and sets start times in main window with correct names and times,
        /// if something went wrong, shows warning window
        /// </summary>
        /// <param name="contestLink">For loading contests</param>
        /// <param name="mw">Already created main window</param>
        private static async void LoadContest(string contestLink, MainWindow mw)
        {
            HttpResponseMessage response;
            try
            {
                response = await SharedHttpClient.GetAsync(contestLink);
            }
            catch (Exception e)
            {
                FailContestLink(mw, e.Message);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                FailContestLink(mw, response.StatusCode.ToString());
                return;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            List<Contest>? contests;
            try
            {
                contests = JsonConvert.DeserializeObject<List<Contest>>(responseString);
            }
            catch (Exception)
            {
                FailContestLink(mw, "Can't deserialize provided data");
                return;
            }
            if (contests == null)
            {
                FailContestLink(mw, "Contests from link are null");
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

        /// <summary>
        /// Called when reading the all API link fails, shows warning and sets the status labels
        /// </summary>
        /// <param name="mw">Already created main window</param>
        /// <param name="detail">Error detail to show</param>
        private static void FailApiLink(MainWindow mw, string detail)
        {
            ShowApiError("All API link", detail);
            mw.EventStatusLabel.Content = "ERR";
            mw.CountStatusLabel.Content = "ERR";
            mw.ContestStatusLabel.Content = "ERR";
        }

        /// <summary>
        /// Reads all API link, sets Event, count and contest links and status and
        /// if it is possible calls LoadContest(), if something went wrong, shows warning window
        /// </summary>
        /// <param name="apiLink">For loading all APIs</param>
        /// <param name="mw">Already created main window</param>
        public static async void LoadApi(string apiLink, MainWindow mw)
        {
            HttpResponseMessage response;
            try
            {
                response = await SharedHttpClient.GetAsync(apiLink);
            }
            catch (Exception e)
            {
                FailApiLink(mw, e.Message);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                FailApiLink(mw, response.StatusCode.ToString());
                return;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            List<Api>? apis;
            try
            {
                apis = JsonConvert.DeserializeObject<List<Api>>(responseString);
            }
            catch (Exception)
            {
                FailApiLink(mw, "Can't deserialize provided data");
                return;
            }
            if (apis == null)
            {
                FailApiLink(mw, "APIs from link are null");
                return;
            }

            var index = apiLink.LastIndexOf("/", StringComparison.Ordinal);
            if (index == -1)
            {
                FailApiLink(mw, "Can't find main API link part");
                return;
            }
            var link = apiLink.Substring(0, index + 1);

            mw.EventStatusLabel.Content = "MIS";
            mw.CountStatusLabel.Content = "MIS";
            mw.ContestStatusLabel.Content = "MIS";

            var canLoadContest = false;
            foreach (var api in apis)
            {
                var label = api.Label?.ToLower();
                if (label == "main")
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
                else if (label == "count")
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
                else if (label == "contest")
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

            if (canLoadContest)
            {
                LoadContest(mw.ContestLink, mw);
            }
        }

        /// <summary>
        /// Called when reading the starts API fails, shows warning and sets the status label
        /// </summary>
        /// <param name="mw">Already created main window</param>
        /// <param name="detail">Error detail to show</param>
        private static void FailStartsLink(MainWindow mw, string detail)
        {
            ShowApiError("Starts API", detail);
            mw.StartsStatusLabel.Content = "ERR";
        }

        /// <summary>
        /// Reads starts API link, parse and set all loaded start times
        /// If something went wrong, shows warning window
        /// </summary>
        /// <param name="apiLink">For loading starts API</param>
        /// <param name="lastSeconds">Load all starts after certain time in seconds</param>
        /// <param name="mw">Already created main window</param>
        /// <returns></returns>
        public static async Task LoadStarts(string apiLink, int lastSeconds, MainWindow mw)
        {
            HttpResponseMessage response;
            try
            {
                response = await SharedHttpClient.GetAsync($"{apiLink}?&filter={Uri.EscapeDataString(mw.StartsFilterField)}%3E{lastSeconds}");
            }
            catch (Exception e)
            {
                FailStartsLink(mw, e.Message);
                return;
            }

            if (!response.IsSuccessStatusCode)
            {
                FailStartsLink(mw, response.StatusCode.ToString());
                return;
            }
            var responseString = await response.Content.ReadAsStringAsync();
            List<List<string>>? starts;
            try
            {
                starts = JsonConvert.DeserializeObject<List<List<string>>>(responseString);
            }
            catch (Exception)
            {
                FailStartsLink(mw, "Can't deserialize provided data");
                return;
            }
            if (starts == null)
            {
                FailStartsLink(mw, "Data from API are null");
                return;
            }

            var tmp = (from b in starts
                       where b.Count >= 3
                       select new StartTime { Bib = b[0], Name = b[1], Time = b[2] }).ToList();
            foreach (var st in tmp)
            {
                if (!string.IsNullOrEmpty(st.Time))
                {
                    StartsController.GetInstance().AddData(st, st.Time);
                }
            }
            mw.StartsStatusLabel.Content = "OK";
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
