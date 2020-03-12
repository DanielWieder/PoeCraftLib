using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class Currency : ICurrency
    {
        public string Name { get; set; }
        public Dictionary<string, int> Execute(Equipment equipment, AffixManager affixManager)
        {
            if (equipment.Corrupted || Requirements.Any(requirement => !requirement(equipment)))
            {
                return new Dictionary<string, int>();
            }

            // Todo: Get modifiers for affixManager

            var currencySpent = CalculateCost(equipment, Name);

            foreach (var step in Steps)
            {
                step(equipment, affixManager, CurrencyModifiers);
            }

            return currencySpent;
        }

        public CurrencyModifiers CurrencyModifiers = new CurrencyModifiers(null, null, null, null, null);

        public List<Func<Equipment, bool>> Requirements { get; set; } = new List<Func<Equipment, bool>>();
        public List<Action<Equipment, AffixManager, CurrencyModifiers>> Steps { get; set; } = new List<Action<Equipment, AffixManager, CurrencyModifiers>>();

        public Func<Equipment, String, Dictionary<string, int>> CalculateCost { get; set; } = DefaultCalculateCost;

        private static Dictionary<string, int> DefaultCalculateCost(Equipment equipment, String name)
        {
            return new Dictionary<string, int>()
            {
                { name, 1 }
            };
        }
    }
}
