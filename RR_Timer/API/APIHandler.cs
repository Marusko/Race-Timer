using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RR_Timer.Data;
using RR_Timer.Logic;
using JsonNamingPolicy = System.Text.Json.JsonNamingPolicy;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace RR_Timer.API
{
    class APIHandler
    {
        private string mainAPIlink;
        private string listAPIlink;
        private ClockLogic ClockLogic;
        private System.Windows.Threading.DispatcherTimer Timer = new System.Windows.Threading.DispatcherTimer();
        private HttpClient httpClient;

        public APIHandler(string APILink, string listLink, ClockLogic cl)
        {
            mainAPIlink = APILink;
            listAPIlink = listLink;
            ClockLogic = cl;
            httpClient = new HttpClient();
            ReadMainAPI();

            Timer.Tick += RefreshListAPI;
            Timer.Interval = new TimeSpan(0, 0, 30);
            Timer.Start();
        }

        private async void ReadMainAPI()
        {
            var response = await httpClient.GetAsync(mainAPIlink);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                responseString = responseString.Replace("{", "").Replace("}", "").Replace("\"", "");
                var splitted = responseString.Split(',');
                string[,] doubleSplitted = new string[splitted.Length, 2];
                for (int i = 0; i < splitted.Length; i++)
                {
                    doubleSplitted[i, 0] = splitted[i].Split(':')[0];
                    doubleSplitted[i, 1] = splitted[i].Split(':')[1];
                }
                ClockLogic.SetLabels(doubleSplitted[(int)APIItemIndex.EventName, 1], ((EventType)int.Parse(doubleSplitted[(int)APIItemIndex.EventType, 1])).ToString());
            } 
        }

        private async void ReadListAPI()
        {
            var response = await httpClient.GetAsync(listAPIlink);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                responseString = responseString.Replace("[", "").Replace("]", "");
                var splittedAsRacers = responseString.Split(",");
                if (!String.IsNullOrEmpty(splittedAsRacers[0]) && !ClockLogic.IsTimerMinimized())
                {
                    ClockLogic.AutoMinimizeTimer();
                }
            }
        }

        private void RefreshListAPI(object sender, EventArgs e)
        {
            ReadListAPI();
        }
    }
}
