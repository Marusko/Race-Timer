using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonNamingPolicy = System.Text.Json.JsonNamingPolicy;
using JsonSerializerOptions = System.Text.Json.JsonSerializerOptions;

namespace RR_Timer
{
    class APIHandler
    {
        private string APIlink;
        private ClockLogic ClockLogic;

        public APIHandler(string APILink, ClockLogic cl)
        {
            APIlink = APILink;
            ClockLogic = cl;
            ReadAPI();
        }

        private async void ReadAPI()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(APIlink);
            
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                responseString = responseString.Replace("{", "").Replace("}", "").Replace("\"", "");
                var splitted = responseString.Split(',');
                string[,] doubleSplitted = new string[splitted.Length, 2];
                for (int i = 0; i < splitted.Length; i++)
                {
                    doubleSplitted[i,0] = splitted[i].Split(':')[0];
                    doubleSplitted[i,1] = splitted[i].Split(':')[1];
                }
                ClockLogic.SetLabels(doubleSplitted[(int)APIItemIndex.EventName, 1], ((EventType)(int.Parse(doubleSplitted[(int)APIItemIndex.EventType, 1]))).ToString());
            }
            
            
        }
    }
}
