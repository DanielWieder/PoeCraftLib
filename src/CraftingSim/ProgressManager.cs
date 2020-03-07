using System;
using System.Collections.Generic;

namespace PoeCraftLib.Crafting
{
    public class ProgressManager
    {
        private readonly Dictionary<string, double> _currencyValues;

        private readonly double _budget = 0;

        private double _spent = 0;
        private int _lastUpdate = 0;

        private readonly Action<int> _onUpdate;

        public double Progress => Math.Min(100, _spent / _budget * 100);

        public ProgressManager(Dictionary<string, double> currencyValues, double budget, Action<int> onUpdate)
        {
            _currencyValues = currencyValues;
            _budget = budget;
            _onUpdate = onUpdate;
        }

        public void AddCost(double value)
        {
            _spent += value;

            var intProgress = (int)Math.Min(100, _spent / _budget * 100);

            if (intProgress > _lastUpdate)
            {
                _lastUpdate = intProgress;
                _onUpdate(intProgress);
            }
        }

        public void SpendCurrency(string currency, int amount)
        {
            if (_currencyValues.ContainsKey(currency))
            {
                AddCost(_currencyValues[currency] * amount);
            }
        }
    }
}
