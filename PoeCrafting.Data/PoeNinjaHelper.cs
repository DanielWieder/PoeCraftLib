using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace PoeCrafting.Data
{
    public class PoeNinjaHelper
    {
        public IDictionary<string, object> GetData(string league, string type)
        {
            string url = $"https://poe.ninja/api/data/itemoverview?league={league}&type={type}";

            Dictionary<string, double> values = new Dictionary<string, double>();

            using (WebClient wc = new WebClient())
            {
                string json = wc.DownloadString(url);
                using (var sr = new StringReader(json))
                {
                    using (var jr = new JsonTextReader(sr))
                    {
                        var js = new JsonSerializer();
                        IDictionary<string, object> topLevel = js.Deserialize<ExpandoObject>(jr);

                        return topLevel;
                    }
                }
            }
        }
    }
}
