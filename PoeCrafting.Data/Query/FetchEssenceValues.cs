using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Data
{
    public class FetchEssenceValues : IFetchEssenceValues
    {
        readonly PoeNinjaHelper _helper = new PoeNinjaHelper();
        public string League { get; set; }

        public Dictionary<string, double> Execute()
        {
            var data = _helper.GetData(League, "Essence");
            Dictionary<string, double> values = new Dictionary<string, double>();

            var essences = data["lines"] as IList<object>;

            foreach (dynamic currency in essences)
            {
                string name = currency.name;
                double value = currency.chaosValue;

                values.Add(name, value);
            }

            return values;
        }
    }

    public interface IFetchEssenceValues : IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
