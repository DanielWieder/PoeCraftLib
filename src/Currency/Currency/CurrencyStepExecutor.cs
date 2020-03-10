using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class CurrencyStepExecutor
    {
        private readonly IRandom _random;

        public CurrencyStepExecutor(IRandom random)
        {
            _random = random;
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> SetRarity(RarityOptions rarityOption)
        {
            return (item, affixManager, currencyModifier) => { 
                switch (rarityOption)
                {
                    case RarityOptions.Normal: 
                        item.Rarity = EquipmentRarity.Normal;
                        break;
                    case RarityOptions.Magic:
                        item.Rarity = EquipmentRarity.Magic;
                        break;
                    case RarityOptions.Rare:
                        item.Rarity = EquipmentRarity.Rare;
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddExplicits(DistributionOptions distributionOptions)
        {
            return (item, affixManager, currencyModifier) => { 
                switch (distributionOptions)
                {
                    case DistributionOptions.MagicDistribution:
                        StatFactory.AddExplicits(_random, item, affixManager, StatFactory.MagicAffixCountOdds, currencyModifier);
                        break;
                    case DistributionOptions.RareDistribution:
                        StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds, currencyModifier);
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddExplicit(ExplicitOptions explicitOptions)
        {
            // Todo: Add modifiers that include prefix/suffix blocking

            return (item, affixManager, currencyModifier) => { 
                switch (explicitOptions)
                {
                    case ExplicitOptions.Any:
                        StatFactory.AddExplicit(_random, item, affixManager, currencyModifier);
                        break;
                    case ExplicitOptions.Prefix:
                        throw new NotImplementedException();
                        break;
                    case ExplicitOptions.Suffix:
                        throw new NotImplementedException();
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddExplicit(Affix affix)
        {
            return (item, affixManager, currencyModifier) => { StatFactory.AddExplicit(_random, item, affix); };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RemoveExplicits(ExplicitsOptions explicitOptions)
        {
            return (item, affixManager, currencyModifier) => { 
                switch (explicitOptions)
                {
                    case ExplicitsOptions.All:
                        item.Stats.Clear();
                        break;
                    case ExplicitsOptions.Prefixes:
                        item.Stats.RemoveAll(x => x.Affix.GenerationType == "prefix");
                        break;
                    case ExplicitsOptions.Suffixes:
                        item.Stats.RemoveAll(x => x.Affix.GenerationType == "suffix");
                        break;
                    case ExplicitsOptions.MasterMods:
                        var crafted = item.Stats.Where(x => x.Affix.GenerationType == "crafted").ToList();
                        item.Stats = item.Stats.Except(crafted).ToList();
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RemoveExplicit(ExplicitOptions explicitOptions)
        {
            // Todo: Add modifiers that include prefix/suffix blocking

            return (item, affixManager, currencyModifier) => { 
                switch (explicitOptions)
                {
                    case ExplicitOptions.Any:
                        StatFactory.RemoveExplicit(_random, item, currencyModifier);
                        break;
                    case ExplicitOptions.Prefix:
                        throw new NotImplementedException();
                        break;
                    case ExplicitOptions.Suffix:
                        throw new NotImplementedException();
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RerollExplicits()
        {
            return (item, affixManager, currencyModifier) => { 
                foreach (var stat in item.Stats)
                {
                    StatFactory.Reroll(_random, item, stat, currencyModifier);
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RerollImplicits()
        {
            return (item, affixManager, currencyModifier) => { StatFactory.Reroll(_random, item, item.Implicit, currencyModifier); };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RandomSteps(
            List<KeyValuePair<int, List<Action<Equipment, AffixManager, CurrencyModifiers>>>> stepsByChance)
        {
            return (item, affixManager, currencyModifier) =>
            {
                int randomValue = _random.Next(100);

                foreach (var randomSteps in stepsByChance)
                {
                    if (randomValue < randomSteps.Key)
                    {
                        randomSteps.Value.ForEach(x => x.Invoke(item, affixManager, currencyModifier));
                        break;
                    }
                    randomValue -= randomSteps.Key;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> Corrupt()
        {
            return (item, affixManager, currencyModifier) => { item.Corrupted = true; };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddExplicitByItemClass(Dictionary<string, Affix> explicitsByItemClass)
        {
            return (item, affixManager, currencyModifier) => { 
                if (explicitsByItemClass.ContainsKey(item.ItemBase.ItemClass))
                {
                    StatFactory.AddExplicit(_random, item, explicitsByItemClass[item.ItemBase.ItemClass]);
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RemoveImplicits()
        {
            return (item, affixManager, currencyModifier) => { item.Implicit = null; };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddImplicits(ImplicitTypes addImplicitArgs)
        {
            return (item, affixManager, currencyModifier) => { 
                // TODO add when implicit support is added
                };
        }
    }
}
