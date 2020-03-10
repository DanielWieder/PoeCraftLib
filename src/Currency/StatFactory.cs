using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency
{
    public static class StatFactory
    {
        private static readonly String _prefixesCannotBeChanged = "ItemGenerationCannotChangePrefixes";
        private static readonly String _suffixesCannotBeChanged = "ItemGenerationCannotChangeSuffixes";

        public static readonly Dictionary<int, int> RareAffixCountOdds = new Dictionary<int, int>()
        {
            {4, 8},
            {5, 3},
            {6, 1}
        };

        public static readonly Dictionary<int, int> MagicAffixCountOdds = new Dictionary<int, int>()
        {
            {1, 1},
            {2, 1}
        };

        public static void RemoveExplicit(IRandom random, Equipment item, CurrencyModifiers currencyModifiers)
        {
            bool cannotChangePrefixes = ItemHasGroup(item, _prefixesCannotBeChanged);
            bool cannotChangeSuffixes = ItemHasGroup(item, _suffixesCannotBeChanged);

            List<Stat> statPool = new List<Stat>();

            if (!cannotChangePrefixes)
            {
                statPool.AddRange(item.Prefixes);
            }

            if (!cannotChangeSuffixes)
            {
                statPool.AddRange(item.Suffixes);
            }

            var index = random.Next(statPool.Count);

            item.Stats.Remove(statPool[index]);
        }

        public static void RemoveAllExplicits(Equipment item, CurrencyModifiers currencyModifiers)
        {
            bool canChangePrefixes = !ItemHasGroup(item, _prefixesCannotBeChanged);
            bool canChangeSuffixes = !ItemHasGroup(item, _suffixesCannotBeChanged);

            if (canChangePrefixes)
            {
                item.Stats = item.Stats.Where(x => x.Affix.GenerationType != "prefix").ToList();
            }

            if (canChangeSuffixes)
            {
                item.Stats = item.Stats.Where(x => x.Affix.GenerationType != "suffix").ToList();
            }
        }

        private static bool ItemHasGroup(List<Affix> affixes, string affix)
        {
            return affixes.Any(x => x.Group == affix);
        }

        private static bool ItemHasGroup(Equipment item, string affix)
        {
            return ItemHasGroup(item.Stats.Select(x => x.Affix).ToList(), affix);
        }

        private static bool ItemHasGroup(List<Stat> stats, string affix)
        {
            return ItemHasGroup(stats.Select(x => x.Affix).ToList(), affix);
        }

        /*
         * Add explicits until the item has an amount equal to the selected count
         */
        public static void AddExplicits(IRandom random, Equipment item, AffixManager affixManager, Dictionary<int, int> affixCountOdds, CurrencyModifiers currencyModifiers)
        {
            var sum = affixCountOdds.Sum(x => x.Value);
            var roll = random.Next(sum);

            var affixCount = 0;
            int count = 0;
            foreach (var weight in affixCountOdds)
            {
                count += weight.Value;

                if (roll < count)
                {
                    affixCount = weight.Key;
                    break;
                }
            }

            while (item.Stats.Count < affixCount)
            {
                bool successful = StatFactory.AddExplicit(random, item, affixManager, currencyModifiers);

                if (!successful) break;
            }
        }

        public static bool AddInfluenceExplicit(Influence influence, IRandom random, Equipment item, AffixManager affixManager, CurrencyModifiers currencyModifiers)
        {
            var equipmentModifiers = new EquipmentModifiers(item);
            var affix = affixManager.GetInfluenceAffix(influence, equipmentModifiers, item.Stats.Select(x => x.Affix).ToList(), item.Rarity, random);

            if (affix == null) return false;

            var stat = AffixToStat(random, item, affix);

            item.Stats.Add(stat);

            return true;
        }

        public static bool AddExplicit(IRandom random, Equipment item, AffixManager affixManager, CurrencyModifiers currencyModifiers)
        {
            var equipmentModifiers = new EquipmentModifiers(item);
            var affix = affixManager.GetAffix(equipmentModifiers, currencyModifiers, item.Stats.Select(x => x.Affix).ToList(), item.Rarity, random);

            if (affix == null) return false;

            var stat = AffixToStat(random, item, affix);

            item.Stats.Add(stat);

            return true;
        }

        public static bool AddExplicit(IRandom random, Equipment item, Affix affix)
        {
            if (IsFull(item, affix)) return false;

            var stat = AffixToStat(random, item, affix);
            item.Stats.Add(stat);

            return true;
        }

        private static bool IsFull(Equipment item, Affix affix)
        {
            int affixesCount = item.Rarity == EquipmentRarity.Normal ? 0 :
                item.Rarity == EquipmentRarity.Magic ? 1 :
                item.Rarity == EquipmentRarity.Rare ? 3 : 0;

            var canAddPrefix = affix.GenerationType == "prefix" && item.Prefixes.Count < affixesCount;
            var canAddSuffix = affix.GenerationType == "suffix" && item.Suffixes.Count < affixesCount;
            return !canAddPrefix && !canAddSuffix;
        }

        public static void SetImplicit(IRandom random, Equipment item)
        {
            // TODO: Update implicit handling
        //    var pool = item.PossibleAffixes.Where(x => x.GenerationType == "corrupted").ToList();
        //    var affix = SelectAffixFromPool(random, new List<Stat>(), pool, item.TotalWeight);
       //     var stat = AffixToStat(random, item, affix);

       //    item.Implicit = stat;
        }

        public static void Reroll(IRandom random, Equipment item, Stat stat, CurrencyModifiers currencyModifiers)
        {
            if (stat?.Affix == null)
                return;

            if (stat.Affix.GenerationType == "prefix" && ItemHasGroup(item, _prefixesCannotBeChanged))
            {
                return;
            }

            if (stat.Affix.GenerationType == "suffix" && ItemHasGroup(item, _suffixesCannotBeChanged))
            {
                return;
            }

            var mod = stat.Affix;

            if (!string.IsNullOrEmpty(mod.StatName1))
                stat.Value1 = random.Next(mod.StatMin1, mod.StatMax1);
            if (!string.IsNullOrEmpty(mod.StatName2))
                stat.Value2 = random.Next(mod.StatMin2, mod.StatMax2);
            if (!string.IsNullOrEmpty(mod.StatName3))
                stat.Value3 = random.Next(mod.StatMin3, mod.StatMax3);
        }

        public static Stat AffixToStat(IRandom random, Equipment equipment, Affix affix)
        {
            Stat stat = new Stat();
            stat.Affix = affix;
            Reroll(random, equipment, stat, null);
            return stat;
        }
    }
}
