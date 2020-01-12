using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency
{
    public class CurrencyFactory
    {
        private readonly IRandom _random;
        private readonly Dictionary<string, Essence> _essences;
        private readonly Dictionary<string, Fossil> _fossils;
        private readonly Dictionary<string, Dictionary<string, MasterMod>> _masterMods;
        private readonly Dictionary<string, ICurrency> _currency;

        public CurrencyFactory(
            IRandom random,
            EssenceFactory essenceFactory, 
            FossilFactory fossilFactory, 
            MasterModFactory masterModFactory)
        {
            _random = random;
            _currency = new Dictionary<string, ICurrency>()
            {
                { CurrencyNames.TransmuationOrb, new TransmutationOrb(random) },
                { CurrencyNames.AlterationOrb, new AlterationOrb(random) },
                { CurrencyNames.AugmentationOrb, new AugmentationOrb(random) },
                { CurrencyNames.AlchemyOrb, new AlchemyOrb(random) },
                { CurrencyNames.ChaosOrb, new ChaosOrb(random) },
                { CurrencyNames.RegalOrb, new RegalOrb(random) },
                { CurrencyNames.BlessedOrb, new BlessedOrb(random) },
                { CurrencyNames.ChanceOrb, new ChanceOrb(random) },
                { CurrencyNames.DivineOrb, new DivineOrb(random) },
                { CurrencyNames.ExaltedOrb, new ExaltedOrb(random) },
                { CurrencyNames.ScouringOrb, new ScouringOrb(random) },
                { CurrencyNames.AnnulmentOrb, new AnnulmentOrb(random) }
            };

            var bad1 = masterModFactory.MasterMod.GroupBy(x => x.Name).ToList();
                var bad2 = bad1.Where(x => x.Count() > 1).ToList();

            _fossils = fossilFactory.Fossils.ToDictionary(x => x.Name, x => x);
            _essences = essenceFactory.Essence.ToDictionary(x => x.Name, x => x);

            _masterMods = new Dictionary<string, Dictionary<string, MasterMod>>();
            foreach (var masterMod in masterModFactory.MasterMod)
            {
                foreach (var itemClass in masterMod.ItemClasses)
                {
                   if (!_masterMods.ContainsKey(masterMod.Name))
                   {
                        _masterMods.Add(masterMod.Name, new Dictionary<string, MasterMod>());
                   }

                   _masterMods[masterMod.Name].Add(itemClass, masterMod);
                }
            }
        }

        public ICurrency GetCurrencyByName(string name)
        {
            return _currency[name];
        }

        public ICurrency GetMasterCraftByName(string name)
        {
            return new MasterCraft(_masterMods[name], _random);
        }

        public ICurrency GetEssenceByName(string name)
        {
            return new EssenceCraft(_random, _essences[name]);
        }

        public ICurrency GetFossilCraftByNames(List<string> names)
        {
            if (names.Count < 1 || names.Count > 4) { throw new ArgumentException("Fossil crafts must have between 1 and 4 fossils");}

            return new FossilCraft(_random, names.Select(x => _fossils[x]).ToList(), _essences.Values.ToList());
        }
    }
}
