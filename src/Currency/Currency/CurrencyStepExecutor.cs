using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeCraftLib.Currency.CurrencyV2
{
    public class CurrencyStepExecutor
    {
        private readonly IRandom _random;

        public CurrencyStepExecutor(IRandom random)
        {
            _random = random;
        }

        public Action<Equipment, AffixManager> SetRarity(RarityOptions rarityOption)
        {
            return (item, affixManager) => { 
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

        public Action<Equipment, AffixManager> AddExplicits(DistributionOptions distributionOptions)
        {
            return (item, affixManager) => {
                switch (distributionOptions)
                {
                    case DistributionOptions.MagicDistribution:
                        StatFactory.AddExplicits(_random, item, affixManager, StatFactory.MagicAffixCountOdds);
                        break;
                    case DistributionOptions.RareDistribution:
                        StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds);
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager> AddExplicit(ExplicitOptions explicitOptions)
        {
            // Todo: Add modifiers that include prefix/suffix blocking

            return (item, affixManager) => {
                switch (explicitOptions)
                {
                    case ExplicitOptions.Any:
                        StatFactory.AddExplicit(_random, item, affixManager);
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

        public Action<Equipment, AffixManager> AddExplicit(Affix affix)
        {
            return (item, affixManager) => { StatFactory.AddExplicit(_random, item, affix); };
        }

        public Action<Equipment, AffixManager> RemoveExplicits(ExplicitsOptions explicitOptions)
        {
            return (item, affixManager) => {
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

        public Action<Equipment, AffixManager> RemoveExplicit(ExplicitOptions explicitOptions)
        {
            // Todo: Add modifiers that include prefix/suffix blocking

            return (item, affixManager) => {
                switch (explicitOptions)
                {
                    case ExplicitOptions.Any:
                        StatFactory.RemoveExplicit(_random, item);
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

        public Action<Equipment, AffixManager> RerollExplicits()
        {
            return (item, affixManager) => {
                foreach (var stat in item.Stats)
                {
                    StatFactory.Reroll(_random, item, stat);
                }
            };
        }

        public Action<Equipment, AffixManager> RerollImplicits()
        {
            return (item, affixManager) => { StatFactory.Reroll(_random, item, item.Implicit); };
        }

        public Action<Equipment, AffixManager> RandomSteps(
            List<KeyValuePair<int, List<Action<Equipment, AffixManager>>>> stepsByChance)
        {
            int randomValue = _random.Next(100);

            foreach (var randomSteps in stepsByChance)
            {
                if (randomValue < randomSteps.Key)
                {
                    return (item, affixManager) => randomSteps.Value.ForEach(x => x.Invoke(item, affixManager));
                }

                randomValue -= randomSteps.Key;
            }
            return (item, affixManager) => { };
        }

        public Action<Equipment, AffixManager> Corrupt()
        {
            return (item, affixManager) => { item.Corrupted = true; };
        }

        public Action<Equipment, AffixManager> AddExplicitByItemClass(Dictionary<string, Affix> explicitsByItemClass)
        {
            return (item, affixManager) => {
                if (explicitsByItemClass.ContainsKey(item.ItemBase.ItemClass))
                {
                    StatFactory.AddExplicit(_random, item, explicitsByItemClass[item.ItemBase.ItemClass]);
                }
            };
        }

        public Action<Equipment, AffixManager> RemoveImplicits()
        {
            return (item, affixManager) => { item.Implicit = null; };
        }

        public Action<Equipment, AffixManager> AddImplicits(ImplicitTypes addImplicitArgs)
        {
            return (item, affixManager) => {
                // TODO add when implicit support is added
                };
        }
    }
}
