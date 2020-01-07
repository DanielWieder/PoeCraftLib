using System.Collections.Generic;

namespace PoeCraftLib.Data.Query
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

    public interface IFetchFossilValues : Data.IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
