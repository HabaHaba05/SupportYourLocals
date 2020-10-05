

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

        #region Not using these methods right now, but i will be in near future
        /*
        //Method to add addresses suggestions
        private void AddItem(string text)
        {
            TextBlock block = new TextBlock
            {

                // Add the text   
                Text = text,

                // A little style...   
                Margin = new Thickness(2, 3, 2, 3),
                Cursor = Cursors.Hand
            };

            // Mouse events   
            block.MouseLeftButtonUp += (sender, e) =>
            {
                addressTextBox.Text = (sender as TextBlock).Text;
            };

            block.MouseEnter += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.PeachPuff;
            };

            block.MouseLeave += (sender, e) =>
            {
                TextBlock b = sender as TextBlock;
                b.Background = Brushes.Transparent;
            };

            // Add to the panel   
            resultStack.Children.Add(block);
        }

        private async void AddressTextBox_Changed(object sender, KeyEventArgs e)
        {
            string query = (sender as TextBox).Text;
            if (String.IsNullOrEmpty(query))
            {
                return;
            };

            bool found = false;
            var data = await GetData(query);

            if (query.Length == 0)
            {
                resultStack.Children.Clear();
            }

            // Clear the list   
            resultStack.Children.Clear();

            // Add the result   
            foreach (var obj in data)
            {
                if (obj.ToLower().StartsWith(query.ToLower()))
                {
                    AddItem(obj);
                    found = true;
                }
            }

            if (!found)
            {
                resultStack.Children.Add(new TextBlock() { Text = "No results found." });
            }
        }*/

        #endregion
    }
}
