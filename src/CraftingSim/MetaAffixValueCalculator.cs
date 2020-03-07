using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting
{
    public class MetaAffixValueCalculator
    {
        public int GetMetaConditionValue(string modType, ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.GenerationType == AffixType.Prefix).ToList();
            var suffixes = a.Affixes.Where(x => x.GenerationType == AffixType.Suffix).ToList();

            if (modType.Contains(AffixTypes.OpenPrefix))
            {
                return prefixes.Count > 3 ? 3 : 3 - prefixes.Count;
            }
            if (modType.Contains(AffixTypes.OpenSuffix))
            {
                return suffixes.Count > 3 ? 3 : 3 - suffixes.Count;
            }
            if (modType == AffixTypes.TotalEnergyShield)
            {
                return GetDefenseConditionValue(a, ItemProperties.EnergyShield, 
                    AffixTypesByStat.EnergyShieldFlat, 
                    AffixTypesByStat.HybridEnergyShieldFlat, 
                    AffixTypesByStat.EnergyShieldPercent,
                    AffixTypesByStat.HybridEnergyShieldPercent,
                    AffixTypesByStat.EnergyShieldPercentSuffix);
            }
            if (modType == AffixTypes.TotalArmor)
            {
                return GetDefenseConditionValue(a, ItemProperties.Armour,
                    AffixTypesByStat.ArmourFlat,
                    AffixTypesByStat.HybridArmourFlat,
                    AffixTypesByStat.ArmourPercent, 
                    AffixTypesByStat.HybridArmourPercent,
                    AffixTypesByStat.ArmourPercentSuffix);
            }
            if (modType == AffixTypes.TotalEvasion)
            {
                return GetDefenseConditionValue(a, ItemProperties.Evasion,
                    AffixTypesByStat.EvasionFlat,
                    AffixTypesByStat.HybridEvasionFlat,
                    AffixTypesByStat.EvasionPercent,
                    AffixTypesByStat.HybridEvasionPercent,
                    AffixTypesByStat.EvasionPercentSuffix);
            }
            if (modType == AffixTypes.TotalResistances)
            {
                var resList = new List<int>
                {
                    GetMaxPropertyValue(suffixes, AffixTypes.ColdResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.FireResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.LightningResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.ChaosResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.AllResist) * 3
                };

                resList = resList.OrderByDescending(x => x).ToList();
                resList = resList.Take(3).ToList();
                return resList.Sum();
            }
            if (modType == AffixTypes.TotalElementalResistances)
            {
                var resList = new List<int>
                {
                    GetMaxPropertyValue(suffixes, AffixTypes.ColdResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.FireResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.LightningResist),
                    GetMaxPropertyValue(suffixes, AffixTypes.AllResist) * 3
                };

                resList = resList.OrderByDescending(x => x).ToList();
                resList = resList.Take(3).ToList();
                return resList.Sum();
            }
            if (modType == AffixTypes.TotalDps)
            {
                return GetDps(GetTotalDamage(a), a);
            }
            if (modType == AffixTypes.TotalElementalDps)
            {
                return GetDps(GetEleDamage(a), a);
            }
            if (modType == AffixTypes.TotalPhysicalDps)
            {
                return GetDps(GetPhysicalDamage(a), a);
            }

            throw new NotImplementedException($"The meta affix {modType} is not recognized");
        }

        private int GetDps(double flatDamage, ConditionContainer a)
        {
            return (int)(flatDamage * GetAttackSpeed(a));
        }

        private double GetAttackSpeed(ConditionContainer a)
        {
            var addedAttackSpeed = 1 + GetMaxPropertyValue(a.Affixes, AffixTypes.LocalAttackSpeed) / 100f;
            var baseAttackTime = a.ItemBase.Properties.ContainsKey(ItemProperties.AttackTime) ? a.ItemBase.Properties[ItemProperties.AttackTime] : 0;
            var baseAttackSpeed = 1000d / baseAttackTime; 
            return baseAttackSpeed * addedAttackSpeed;
        }

        private int GetTotalDamage(ConditionContainer a)
        {
            var physicalDamage = GetPhysicalDamage(a);
            var eleDamage = GetEleDamage(a);

            // Calculating the best 3 out of all the possible affixes could be tricky so I cheat a little
            // When analyzing all possible affixes, I expect that either full phys or full elemental will be better

            return IsGeneratedItem(a.Affixes.Count)
                ? (int) (physicalDamage + eleDamage)
                : (int) Math.Max(eleDamage, physicalDamage);
        }

        // This is close enough to bring correct
        // There might be some exceptions with very low level items and fossils blocking affixes
        // But that is so niche that I'm not going to take it into consideration
        private static bool IsGeneratedItem(int affixCount)
        {
            return affixCount <= 6;
        }

        private double GetPhysicalDamage(ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.GenerationType == AffixType.Prefix).ToList();

            var percentDamage = GetMaxPropertyValue(prefixes, AffixTypes.LocalPhysicalPercent) + GetMaxPropertyValue(prefixes, AffixTypes.LocalPhysicalHybrid);

            var physical = (GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatPhysicalDamage) +
                             GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatPhysicalDamage, 1)) / 2;

            double baseDamage = BaseDamage(a);

            return (baseDamage + physical) * (120 + percentDamage) / 100;
        }

        private double BaseDamage(ConditionContainer a)
        {
            var minPhysDamage = a.ItemBase.Properties.ContainsKey(ItemProperties.MinDamage) ? a.ItemBase.Properties[ItemProperties.MinDamage] : 0;
            var maxPhysDamage = a.ItemBase.Properties.ContainsKey(ItemProperties.MaxDamage) ? a.ItemBase.Properties[ItemProperties.MaxDamage] : 0;
            return (minPhysDamage + maxPhysDamage) / 2;
        }

        private double GetEleDamage(ConditionContainer a)
        {
            var prefixes = a.Affixes.Where(x => x.GenerationType == AffixType.Prefix).ToList();

            var chaos = (GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatChaosDamage) +
                         GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatChaosDamage, 1)) / 2;

            var cold = (GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatColdDamage) +
                         GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatColdDamage, 1)) / 2;

            var fire = (GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatFireDamage) +
                         GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatFireDamage, 1)) / 2;

            var lightning = (GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatLightningDamage) +
                         GetMaxPropertyValue(prefixes, AffixTypesByStat.FlatLightningDamage, 1)) / 2;

            var eleList = new List<int>
            {
                chaos,
                cold,
                fire,
                lightning
            };

            eleList = eleList.OrderByDescending(x => x).ToList();
            eleList = eleList.Take(3).ToList();
            return eleList.Sum();
        }

        private int GetDefenseConditionValue(
            ConditionContainer a, 
            string propertyName, 
            List<string> flatDefenseNames, 
            List<string> flatHybridNames, 
            List<string> percentDefensesNames, 
            List<string> hybridDefenseNames,
            List<string> percentDefenseNamesSuffix)
        {
            var baseDefense = a.ItemBase.Properties.ContainsKey(propertyName) ? a.ItemBase.Properties[propertyName] : 0;
            var flatDefense = GetMaxPropertyValue(a.Affixes, flatDefenseNames);
            var hybridFlatDefense = GetMaxPropertyValue(a.Affixes, flatHybridNames);
            var percentDefense = GetMaxPropertyValue(a.Affixes, percentDefensesNames);
            var hybridPercentDefense = GetMaxPropertyValue(a.Affixes, hybridDefenseNames);
            var percentDefenseSuffix = GetMaxPropertyValue(a.Affixes, percentDefenseNamesSuffix);

            // Take 3 main mods if >3 prefixes are provided
            if (flatDefense != 0 && hybridPercentDefense != 0 && percentDefense != 0 && hybridPercentDefense != 0)
            {
                return (int)((baseDefense + flatDefense) * (120 + percentDefense + hybridPercentDefense) / 100);
            }

            return (int)((baseDefense + flatDefense + hybridFlatDefense) * (120 + percentDefense + hybridPercentDefense + percentDefenseSuffix) / 100);
        }

        private int GetMaxPropertyValue(List<ItemProperty> items, List<string> propertyNames, int index = 0)
        {
            var properties = items.Where(x => propertyNames.Contains(x.Type)).ToList();
            return GetMaxPropertyValue(properties, index);
        }

        private int GetMaxPropertyValue(List<ItemProperty> items, string property, int index = 0)
        {
            var properties = items.Where(x => x.Type == property).ToList();
            return GetMaxPropertyValue(properties, index);
        }

        private int GetMaxPropertyValue(List<ItemProperty> items, int index = 0)
        {
            if (!items.Any())
            {
                return 0;
            }
            return items.Max(x => x.Values[index]);
        }
    }
}
