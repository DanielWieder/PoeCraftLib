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
    public class FetchFossilValues : IFetchFossilValues
    {
        readonly PoeNinjaHelper _helper = new PoeNinjaHelper();

        public string League { get; set; }

        public Dictionary<string, double> Execute()
        {
            var data = _helper.GetData(League, "Fossil");
            Dictionary<string, double> values = new Dictionary<string, double>();

            var fossils = data["lines"] as IList<object>;

            foreach (dynamic currency in fossils)
            {
                string name = currency.name;
                double value = currency.chaosValue;

                values.Add(name, value);
            }

            return values;
        }
    }

    public interface IFetchFossilValues : IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
