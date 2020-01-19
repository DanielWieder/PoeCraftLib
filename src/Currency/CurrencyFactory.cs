using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency
{
    public class CurrencyFactory
    {
        private readonly IRandom _random;
        private readonly Dictionary<string, Essence> _essences;
        private readonly Dictionary<string, Fossil> _fossils;
        private readonly Dictionary<string, ICurrency> _currency;

        public CurrencyFactory(
            IRandom random,
            EssenceFactory essenceFactory, 
            FossilFactory fossilFactory, 
            MasterModFactory masterModFactory)
        {
            _random = random;

            var currency = new List<ICurrency>()
            {
                { new TransmutationOrb(random) },
                { new AlterationOrb(random) },
                { new AugmentationOrb(random) },
                { new AlchemyOrb(random) },
                { new ChaosOrb(random) },
                { new RegalOrb(random) },
                { new BlessedOrb(random) },
                { new ChanceOrb(random) },
                { new DivineOrb(random) },
                { new ExaltedOrb(random) },
                { new ScouringOrb(random) },
                { new AnnulmentOrb(random) }
            };

            var fossilCurrency = fossilFactory.Fossils.Select(x => new FossilCraft(_random, new List<Fossil>() {x}, essenceFactory.Essence));
            var essenceCurrency = essenceFactory.Essence.Select(x => new EssenceCraft(_random, x));
            var masterCraftCurrency = GetMasterModsByName(masterModFactory).Select(x => new MasterCraft(x.Value, random));

            _currency = currency.Union(fossilCurrency)
                .Union(essenceCurrency)
                .Union(masterCraftCurrency)
                .ToDictionary(x => x.Name, x => x);

            _fossils = fossilFactory.Fossils.ToDictionary(x => x.Name, x => x);
            _essences = essenceFactory.Essence.ToDictionary(x => x.Name, x => x);
        }

        private static Dictionary<string, Dictionary<string, MasterMod>> GetMasterModsByName(MasterModFactory masterModFactory)
        {
            var masterMods = new Dictionary<string, Dictionary<string, MasterMod>>();
            foreach (var masterMod in masterModFactory.MasterMod)
            {
                foreach (var itemClass in masterMod.ItemClasses)
                {
                    if (!masterMods.ContainsKey(masterMod.Name))
                    {
                        masterMods.Add(masterMod.Name, new Dictionary<string, MasterMod>());
                    }

                    masterMods[masterMod.Name].Add(itemClass, masterMod);
                }
            }

            return masterMods;
        }

        public ICurrency GetCurrencyByName(string name)
        {
            if (!_currency.ContainsKey(name)) throw new ArgumentException("Unknown type of currency" + name);

            return _currency[name];
        }

        public ICurrency GetFossilCraftByNames(List<string> names)
        {
            if (names.Count < 1 || names.Count > 4) { throw new ArgumentException("Fossil crafts must have between 1 and 4 fossils");}

            return new FossilCraft(_random, names.Select(x => _fossils[x]).ToList(), _essences.Values.ToList());
        }
    }
}
