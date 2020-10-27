using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SuppLocals
{
    public static class AutoComplete
    {
        //Method to get JSON from google API
        public static async Task<List<string>> GetDataAsync(string query)
        {
            var data = new List<string>();

            try
            {
                var uri = Config.Host + Config.Path + "?input=" + query +
                          "&types=geocode&language=lt&components=country:lt&key=" + Config.Google_Api_Key;

                var client = new HttpClient();
                var response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                var o = JObject.Parse(responseBody);


                var jObj = (JObject) JsonConvert.DeserializeObject(responseBody);
                var ob = jObj["predictions"];
                var count = ob.Count();

                for (var i = 0; i < count; i++)
                {
                    data.Add((string) o.SelectToken("predictions[" + i + "].description"));
                }

                return data;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }

            return data;
        }
    }
}