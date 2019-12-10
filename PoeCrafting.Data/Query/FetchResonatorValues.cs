using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCrafting.Data
{
    public class FetchResonatorValues : IFetchResonatorValues
    {
        readonly PoeNinjaHelper _helper = new PoeNinjaHelper();

        public string League { get; set; }

        public Dictionary<string, double> Execute()
        {
            var data = _helper.GetData(League, "Resonator");
            Dictionary<string, double> values = new Dictionary<string, double>();

            var resonator = data["lines"] as IList<object>;

            foreach (dynamic currency in resonator)
            {
                string name = currency.name;
                double value = currency.chaosValue;

                values.Add(name, value);
            }

            return values;
        }
    }

    public interface IFetchResonatorValues : IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
