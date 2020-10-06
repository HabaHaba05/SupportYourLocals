

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SuppLocals { 
    public static class AutoComplete
    {

        //Method to get JSON from google API
        public static async Task<List<string>> GetData(string query)
        {
            List<string> data = new List<string>();

            try
            {
                string uri = Config.host + Config.path + "?input=" + query + "&types=geocode&language=lt&components=country:lt&key=" + Config.GOOGLE_API_KEY;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(uri);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                JObject o = JObject.Parse(responseBody);


                JObject jObj = (JObject)JsonConvert.DeserializeObject(responseBody);
                var ob = jObj["predictions"];
                int count = ob.Count();

                for (int i = 0; i < count; i++)
                {
                    data.Add((string)o.SelectToken("predictions[" + i + "].description"));
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
