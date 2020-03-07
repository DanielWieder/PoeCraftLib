using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace PoeCraftLib.Data
{
    public class PoeNinjaHelper
    {
        public IDictionary<string, object> GetData(string league, string type)
        {
            string path = "itemoverview";

            if (type == "Currency")
            {
                path = "currencyoverview";
            }

            string url = $"https://poe.ninja/api/data/{path}?league={league}&type={type}";

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
