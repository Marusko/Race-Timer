using System;
using System.Net.Http;
using RR_Timer.Data;
using RR_Timer.Logic;
using RR_Timer.UI;

namespace RR_Timer.API
{
    internal class LinkHandler
    {
        private readonly string _mainLink;
        private readonly string? _listLink;
        private readonly ClockLogic _clockLogic;
        private readonly System.Windows.Threading.DispatcherTimer _timer = new();
        private readonly HttpClient _httpClient;

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

        public LinkHandler(string mainLink, ClockLogic cl)
        {
            _mainLink = mainLink;
            _listLink = null;
            _clockLogic = cl;
            _httpClient = new HttpClient();
            ReadMainLink();
        }

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

        private void RefreshListLink(object? sender, EventArgs e)
        {
            ReadListLink();
        }
    }
}
