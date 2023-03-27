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
        /// Reads main link and sets name and type of event in ClockLogic
        /// </summary>
        private async void ReadMainLink()
        {
            var response = await _httpClient.GetAsync(_mainLink);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                responseString = responseString.Replace("{", "").Replace("}", "").Replace("\"", "");
                var split = responseString.Split(',');
                var doubleSplit = new string[split.Length, 2];
                for (var i = 0; i < split.Length; i++)
                {
                    doubleSplit[i, 0] = split[i].Split(':')[0];
                    doubleSplit[i, 1] = split[i].Split(':')[1];
                }
                _clockLogic.SetLabels(doubleSplit[(int)MainLinkItemIndex.EventName, 1], ((EventType)int.Parse(doubleSplit[(int)MainLinkItemIndex.EventType, 1])).ToString());
            }
            else
            {
                var warning = new WarningWindow("Oops, something went wrong with main API!\nError code: [" + response.StatusCode + "]");
                warning.ShowDialog();
                _clockLogic.SetLabels("", "");
            }
        }

        /// <summary>
        /// Reads list link and if one or mor participants finished, it minimizes timer
        /// </summary>
        private async void ReadListLink()
        {
            if (!_clockLogic.IsTimerMinimized())
            {
                var response = await _httpClient.GetAsync(_listLink);

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
    }
}
