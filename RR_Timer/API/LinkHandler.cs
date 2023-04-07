using System;
using System.Net.Http;
using RR_Timer.Data;
using RR_Timer.Logic;
using RR_Timer.UI;

namespace RR_Timer.API
{
    /// <summary>
    /// Handles retrieving data from links
    /// </summary>
    internal class LinkHandler
    {
        private readonly string _mainLink;
        private readonly string? _listLink;
        private readonly ClockLogic _clockLogic;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly HttpClient _httpClient;

        /// <summary>
        /// When both links are entered, this constructor will be called and main, list links will be read
        /// </summary>
        /// <param name="mainLink">For event name and type</param>
        /// <param name="listLink">For list of participants that already finished</param>
        /// <param name="cl"></param>
        public LinkHandler(string mainLink, string listLink, ClockLogic cl)
        {
            _mainLink = mainLink;
            _listLink = listLink;
            _clockLogic = cl;
            _httpClient = new HttpClient();
            ReadMainLink();

            _timer.Tick += RefreshListLink;
            _timer.Interval = new TimeSpan(0, 0, 15);
            _timer.Start();
        }
        /// <summary>
        /// When list link is not entered, this constructor will be called and main link will be read
        /// </summary>
        /// <param name="mainLink">For event name and type</param>
        /// <param name="cl">For clock logic object</param>
        public LinkHandler(string mainLink, ClockLogic cl)
        {
            _mainLink = mainLink;
            _listLink = null;
            _clockLogic = cl;
            _httpClient = new HttpClient();
            ReadMainLink();
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
                var warning = new WarningWindow("Oops, something went wrong with main API!\nError code: \n[" + e.Message + "]");
                warning.ShowDialog();
                _clockLogic.MainWindow.CanOpenTimer = false;
                return;
            }

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                responseString = responseString.Replace("{", "").Replace("}", "").Replace("\"", "");
                var split = responseString.Split(',');
                var doubleSplit = new string[split.Length, 2];
                var name = 0;
                var type = 0;
                for (var i = 0; i < split.Length; i++)
                {
                    doubleSplit[i, 0] = split[i].Split(':')[0];
                    doubleSplit[i, 1] = split[i].Split(':')[1];
                    if (doubleSplit[i, 0] == "EventName")
                    {
                        name = i;
                    }
                    else if (doubleSplit[i, 0] == "EventType")
                    {
                        type = i;
                    }
                }
                _clockLogic.SetLabels(doubleSplit[name, 1], ((EventType)int.Parse(doubleSplit[type, 1])).ToString());
            }
            else
            {
                var warning = new WarningWindow("Oops, something went wrong with main API!\nError code: [" + response.StatusCode + "]");
                warning.ShowDialog();
                _clockLogic.MainWindow.OnClose();
            }
        }

        /// <summary>
        /// Reads list link and if one or mor participants finished, it minimizes timer, if something went wrong shows error
        /// and stops the timer for refreshing list link
        /// </summary>
        private async void ReadListLink()
        {
            if (!_clockLogic.IsTimerMinimized())
            {
                HttpResponseMessage response;
                try
                {
                    response = await _httpClient.GetAsync(_listLink);
                }
                catch (Exception e)
                {
                    _timer.Stop();
                    var warning = new WarningWindow("Oops, something went wrong with list API!\nError code: \n[" + e.Message + "]");
                    warning.ShowDialog();
                    return;
                }

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    responseString = responseString.Replace("[", "").Replace("]", "");
                    var asRacers = responseString.Split(",");
                    if (!string.IsNullOrEmpty(asRacers[0]) && !_clockLogic.IsTimerMinimized())
                    {
                        _clockLogic.AutoMinimizeTimer();
                    }
                }
                else
                {
                    _timer.Stop();
                    var warning = new WarningWindow("Oops, something went wrong with list API!\nError code: [" + response.StatusCode + "]");
                    warning.ShowDialog();
                }
            }
            else
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Timer calls this method, which calls ReadListLink()
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RefreshListLink(object? sender, EventArgs e)
        {
            ReadListLink();
        }

        /// <summary>
        /// Stops the timer
        /// </summary>
        public void StopTimer()
        {
            _timer.Stop();
        }
    }
}
