﻿using System.Collections.Generic;

namespace PoeCraftLib.Data.Query
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

    public interface IFetchCurrencyValues : Data.IQueryObject<Dictionary<string, double>>
    {
        string League { get; set; }
    }
}
