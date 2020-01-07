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

namespace PoeCraftLib.Data
{
    public class FetchCurrencyValues : IFetchCurrencyValues
    {
        readonly PoeNinjaHelper _helper = new PoeNinjaHelper();

        public string League { get; set; }

        public Dictionary<string, double> Execute()
        {
            var data = _helper.GetData(League, "Currency");
            Dictionary<string, double> values = new Dictionary<string, double>();

            var currencies = data["lines"] as IList<object>;

            foreach (dynamic currency in currencies)
            {
                string name = currency.currencyTypeName;
                double value = currency.chaosEquivalent;

                values.Add(name, value);
            }

            return values;
        }
    }

    public interface IFetchCurrencyValues : IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
