﻿using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class CurrencyStepExecutor
    {
        private Dictionary<EquipmentRarity, int> _rarityToQualityChange = new Dictionary<EquipmentRarity, int>()
        {
            {EquipmentRarity.Normal, 5 },
            {EquipmentRarity.Magic, 2 },
            {EquipmentRarity.Rare, 1 },
            {EquipmentRarity.Unique, 1 }
        };

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
                    case ExplicitOptions.Crusader:
                        StatFactory.AddInfluenceExplicit(Influence.Crusader, _random, item, affixManager, currencyModifier);
                        break;
                    case ExplicitOptions.Hunter:
                        StatFactory.AddInfluenceExplicit(Influence.Hunter, _random, item, affixManager, currencyModifier);
                        break;
                    case ExplicitOptions.Warlord:
                        StatFactory.AddInfluenceExplicit(Influence.Warlord, _random, item, affixManager, currencyModifier);
                        break;
                    case ExplicitOptions.Redeemer:
                        StatFactory.AddInfluenceExplicit(Influence.Redeemer, _random, item, affixManager, currencyModifier);
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
            return (item, affixManager, currencyModifier) => { item.Implicits.ForEach(x => StatFactory.Reroll(_random, item, x, currencyModifier)); };
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
            return (item, affixManager, currencyModifier) => { item.Implicits = null; };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddImplicits(ImplicitTypes addImplicitArgs)
        {
            return (item, affixManager, currencyModifier) => { 
                // TODO add when implicit support is added
                };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddInfluence(InfluenceOptions addInfluenceArgs)
        {
            return (item, affixManager, currencyModifier) => {
                switch (addInfluenceArgs)
                {
                    case InfluenceOptions.Hunter:
                        item.Influence.Add(Influence.Hunter);
                        break;
                    case InfluenceOptions.Crusader:
                        item.Influence.Add(Influence.Crusader);
                        break;
                    case InfluenceOptions.Redeemer:
                        item.Influence.Add(Influence.Redeemer);
                        break;
                    case InfluenceOptions.Warlord:
                        item.Influence.Add(Influence.Warlord);
                        break;
                    case InfluenceOptions.Random:
                        var influences = (Influence[]) Enum.GetValues(typeof(Influence));
                        influences = influences.Where(x => !item.Influence.Contains(x)).ToArray();
                        var index = _random.Next(influences.Length);
                        item.Influence.Add(influences[index]);
                        break;
                    case InfluenceOptions.One:
                        throw new NotImplementedException();
                        break;
                    case InfluenceOptions.None:
                        throw new NotImplementedException();
                        break;
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> SetQualityType(QualityType qualityType)
        {
            return (item, affixManager, currencyModifier) =>
            {
                if (item.QualityType == qualityType) return;

                item.QualityType = qualityType;
                item.Quality = 0;
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> AddQuality()
        {
            return (item, affixManager, currencyModifier) =>
            {
                item.Quality += _rarityToQualityChange[item.Rarity];
                item.Quality = Math.Min(20, item.Quality);
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> RemoveCatalystQuality(int change)
        {
            return (item, affixManager, currencyModifier) =>
            {
                if (item.QualityType != QualityType.Default && item.Quality > 0)
                {
                    item.Quality = Math.Max(item.Quality - change, 0);
                }
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> DestroyItem()
        {
            return (item, affixManager, currencyModifier) => { 
                item.Destroyed = true;
                item.Completed = true;
                item.Corrupted = true;
            };
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> ResetItem()
        {
            return (item, affixManager, currencyModifier) =>
            {
                item.Influence.Clear();
                item.Implicits.Clear();
                item.Rarity = EquipmentRarity.Normal;
                item.Quality = 0;
                item.QualityType = QualityType.Default;
                item.Stats.Clear();
                item.Destroyed = false;
                item.Completed = false;
                item.Corrupted = false;

                item.ItemBase.Implicits.ForEach(x => StatFactory.AddImplicit(_random, item, x));
            };
        }
    }
}
