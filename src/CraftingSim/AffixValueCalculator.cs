using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting
{
    public class AffixValueCalculator
    {
        public List<int> GetAffixValues(string mod, Equipment item, AffixType type, StatValueType valueType)
        {
            var conditionItem = new ConditionContainer()
            {
                ItemBase = item.ItemBase,
                Affixes = item.Stats.Select(x => AffixToItemProperty(x.Affix, valueType, x.Values)).ToList()
            };
            return GetAffixValue(mod, type, conditionItem);
        }

        public List<int> GetModMax(string modType, ItemBase itemBase, List<Affix> affixes, AffixType type)
        {
            var max = StatValueType.Max;

            var conditionItem = new ConditionContainer
            {
                ItemBase = itemBase,
                Affixes = affixes.Select(x => AffixToItemProperty(x, max)).ToList()
            };
            return GetAffixValue(modType, type, conditionItem);
        }

        private ItemProperty AffixToItemProperty(Affix affix, StatValueType valueType, List<int> values = null)
        {
            return new ItemProperty()
            {
                Group = affix.Group,
                Values = GetTypedValue(affix, valueType, values),
                Type = (AffixType)Enum.Parse(typeof(AffixType), affix.GenerationType, true)
            };
        }

        private List<int> GetTypedValue(Affix affix, StatValueType valueType, List<int> values = null)
        {
            switch (valueType)
            {
                case StatValueType.Flat:
                    return values;
                case StatValueType.Max:
                    return affix.MaxStats;
                case StatValueType.Tier:
                    return new List<int> { affix.Tier };
                default:
                    throw new NotImplementedException($"Stat type {valueType} has not yet been implemented");
            }
        }

        private static List<int> GetAffixValue(string mod, AffixType type, ConditionContainer a)
        {
            if (type == AffixType.Prefix || type == AffixType.Suffix)
            {
                return a.Affixes
                           .Where(x => x.Type == type)
                           .Where(x => x.Group == mod)
                           .OrderByDescending(x => x.Values.Count < 1 ? 0 : x.Values[0])
                           .ThenByDescending(x => x.Values.Count < 2 ? 0 : x.Values[1])
                           .ThenByDescending(x => x.Values.Count < 3 ? 0 : x.Values[2])
                           .FirstOrDefault()?.Values ?? new List<int>();
            }
            if (type == AffixType.Meta)
            {
                MetaAffixValueCalculator calculator = new MetaAffixValueCalculator();
                return new List<int> { calculator.GetMetaConditionValue(mod, a)};
            }

            throw new NotImplementedException($"Affix type {Enum.GetName(typeof(AffixType), type)} has not yet been implemented");
        }
    }
}
