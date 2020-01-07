using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CraftingSim
{
    public class ConditionResolver
    {
        public bool IsValid(CraftingCondition condition, Equipment item)
        {
            int validCount = 0;
            for (int i = condition.CraftingSubConditions.Count - 1; i >= 0; i--)
            {
                if (IsSubconditionValid(condition.CraftingSubConditions[i], item))
                {
                    validCount++;
                }
            }
            return validCount == condition.CraftingSubConditions.Count;
        }

        private bool IsSubconditionValid(CraftingSubcondition subcondition, Equipment item)
        {
            var prefixResolutions =
                subcondition.PrefixConditions.Select(
                    x => ResolveCondition(x, item, AffixType.Prefix, subcondition.ValueType)).ToList();

            var suffixResolutions =
                subcondition.SuffixConditions.Select(
                    x => ResolveCondition(x, item, AffixType.Suffix, subcondition.ValueType)).ToList();

            var metaResolutions =
                subcondition.MetaConditions.Select(
                    x => ResolveCondition(x, item, AffixType.Meta, subcondition.ValueType)).ToList();

            var allResolutions = prefixResolutions
                .Concat(suffixResolutions)
                .Concat(metaResolutions)
                .ToList();

            var matched = allResolutions.Count(x => x.IsMatch);
            var sum = allResolutions.Where(x => x.IsPresent).Sum(x => x.Values.Sum());

            switch (subcondition.AggregateType)
            {
                case SubconditionAggregateType.Count:
                    return (!subcondition.AggregateMin.HasValue || matched >= subcondition.AggregateMin) && (!subcondition.AggregateMax.HasValue || matched <= subcondition.AggregateMax);
                case SubconditionAggregateType.And:
                    return allResolutions.All(x => x.IsPresent && x.IsMatch);
                case SubconditionAggregateType.If:
                    return allResolutions.Where(x => x.IsPresent).All(x => x.IsMatch);
                case SubconditionAggregateType.Not:
                    return !allResolutions.Any(x => x.IsMatch);
                case SubconditionAggregateType.Sum:
                    return (!subcondition.AggregateMin.HasValue || sum >= subcondition.AggregateMin) && (!subcondition.AggregateMax.HasValue || sum <= subcondition.AggregateMax);
                default:
                    throw new InvalidOperationException($"The subcondition aggregate type {Enum.GetName(typeof(SubconditionAggregateType), subcondition.AggregateType)} is not recognized");
            }
        }

        private ConditionResolution ResolveCondition(ConditionAffix affix, Equipment item, AffixType type, StatValueType valueType)
        {
            AffixValueCalculator calculator = new AffixValueCalculator();
            var value = calculator.GetAffixValues(affix.ModType, item, type, valueType);

            return new ConditionResolution()
            {
                IsPresent = value.Any(),
                IsMatch = value.Any() && IsValueWithinBounds(affix, value),
                Values = value
            };
        }

        private bool IsValueWithinBounds(ConditionAffix affix, List<int> value)
        {
            bool hasRequirement1 = affix.Min1.HasValue || affix.Max1.HasValue;
            bool hasRequirement2 = affix.Min2.HasValue || affix.Max2.HasValue;
            bool hasRequirement3 = affix.Min3.HasValue || affix.Max3.HasValue;

            bool meetsRequirement1 = !hasRequirement1 || (value.Count >= 1 && 
                        (!affix.Min1.HasValue || value[0] >= affix.Min1.Value) &&
                        (!affix.Max1.HasValue || value[0] <= affix.Max1.Value));

            bool meetsRequirement2 = !hasRequirement2 || (value.Count >= 2 &&
                         (!affix.Min2.HasValue || value[1] >= affix.Min2.Value) &&
                         (!affix.Max2.HasValue || value[1] <= affix.Max2.Value));

            bool meetsRequirement3 = !hasRequirement3 || (value.Count >= 3 &&
                         (!affix.Min3.HasValue || value[2] >= affix.Min3.Value) &&
                         (!affix.Max3.HasValue || value[2] <= affix.Max3.Value));

            return meetsRequirement1 && meetsRequirement2 && meetsRequirement3;
        }
    }
}
