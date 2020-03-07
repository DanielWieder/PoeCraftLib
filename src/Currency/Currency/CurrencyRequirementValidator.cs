using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Crafting;

namespace PoeCraftLib.Currency.CurrencyV2
{

    public class CurrencyRequirementValidator
    {
        private static readonly string _multimodGroup = "ItemGenerationCanHaveMultipleCraftedMods";

        public Func<Equipment, bool> ValidateRarity(List<RarityOptions> rarityOptions)
        {
            return (item) => {
                foreach (var rarity in rarityOptions)
                {
                    if (item.Rarity == EquipmentRarity.Normal && rarity == RarityOptions.Normal)
                    {
                        return true;
                    }

                    if (item.Rarity == EquipmentRarity.Magic && rarity == RarityOptions.Magic)
                    {
                        return true;
                    }

                    if (item.Rarity == EquipmentRarity.Rare && rarity == RarityOptions.Rare)
                    {
                        return true;
                    }
                }

                return false;
            };
        }

        public Func<Equipment, bool> ValidateOpenExplicit(ExplicitOptions explicitOptions)
        {
            return (item) =>
            {
                var affixCount = item.Rarity == EquipmentRarity.Normal ? 0 :
                    item.Rarity == EquipmentRarity.Magic ? 1 :
                    item.Rarity == EquipmentRarity.Rare ? 3 : 0;

                if (explicitOptions == ExplicitOptions.Any)
                {
                    return item.Stats.Count < affixCount * 2;
                }

                if (explicitOptions == ExplicitOptions.None)
                {
                    return item.Stats.Count == affixCount * 2;
                }

                if (explicitOptions == ExplicitOptions.Prefix)
                {
                    return item.Prefixes.Count <= affixCount;
                }

                if (explicitOptions == ExplicitOptions.Suffix)
                {
                    return item.Suffixes.Count <= affixCount;
                }

                throw new InvalidOperationException("Invalid open explicit validation");
            };
        }

        public Func<Equipment, bool> ValidateHasTags(HashSet<String> tags)
        {
            return (item) => item.ItemBase.Tags.Any(x => tags.Contains(x));
        }

        public Func<Equipment, bool> ValidateImplicit(ImplicitOptions implicitOptions)
        {
            return (item) =>
            {
                if (implicitOptions == ImplicitOptions.Any)
                {
                    return item.Implicit != null;
                }

                if (implicitOptions == ImplicitOptions.None)
                {
                    return item.Implicit == null;
                }

                if (implicitOptions == ImplicitOptions.Variable)
                {
                    bool hasImplicit = item.Implicit != null && item.Implicit.Affix != null;
                    bool isStat1Variable = item.Implicit.Affix.StatName1 != null && item.Implicit.Affix.StatMin1 != item.Implicit.Affix.StatMax1;
                    bool isStat2Variable = item.Implicit.Affix.StatName2 != null && item.Implicit.Affix.StatMin2 != item.Implicit.Affix.StatMax2;
                    bool isStat3Variable = item.Implicit.Affix.StatName3 != null && item.Implicit.Affix.StatMin3 != item.Implicit.Affix.StatMax3;

                    return hasImplicit && (isStat1Variable || isStat2Variable || isStat3Variable);
                }
                throw new InvalidOperationException("Invalid implicit validation");
            };
        }

        public Func<Equipment, bool> ValidateHasExplicit(ExplicitOptions explicitOptions)
        {
            switch (explicitOptions)
            {
                case (ExplicitOptions.Any):
                    return item => item.Stats.Any();
                case (ExplicitOptions.Prefix):
                    return item => item.Prefixes.Any();
                case (ExplicitOptions.Suffix):
                    return item => item.Suffixes.Any();
                case (ExplicitOptions.None):
                    return item => !item.Stats.Any();
                case (ExplicitOptions.MasterMod):
                    return item => item.Stats.Any(x => x.Affix.GenerationType == "crafted");
            }
            throw new InvalidOperationException("Invalid explicit validation");
        }

        public Func<Equipment, bool> ValidateMatchingGroup(string group, GenericOptions genericOptions)
        {
            switch (genericOptions)
            {
                case (GenericOptions.Any):
                    return item => item.Stats.Any(x => x.Affix.Group == group);
                case (GenericOptions.None):
                    return item => item.Stats.All(x => x.Affix.Group != group);
            }

            throw new InvalidOperationException("Invalid matching group validation");
        }

        public Func<Equipment, bool> ValidateCanAddMasterMod()
        {
            return (item) =>
            {
                int masterModCount = item.Stats.Count(x => x.Affix.TierType == TierType.Craft);
                return masterModCount == 0 || (masterModCount <= 2 && HasMultiMod(item));
            };
        }

        private static bool HasMultiMod(Equipment item)
        {
            return item.Stats.Any(x => x.Affix.Group == _multimodGroup);
        }

        public Func<Equipment, bool> ValidateMatchingItemClasses(HashSet<string> itemClasses, GenericOptions genericOptions)
        {
            return (item) =>
            {
                switch (genericOptions)
                {
                    case (GenericOptions.Any):
                        return itemClasses.Contains(item.ItemBase.ItemClass);
                    case (GenericOptions.None):
                        return !itemClasses.Contains(item.ItemBase.ItemClass);
                }
                throw new InvalidOperationException("Invalid matching item class validation");
            };
        }
    }
}
