using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{

    public class CurrencyRequirementValidator
    {
        private static readonly string _multimodGroup = "ItemGenerationCanHaveMultipleCraftedMods";

        public Func<Equipment, bool> ValidateAny(params Func<Equipment, bool>[] validations)
        {
            return (item) => validations.Any(x => x(item));
        }

        public Func<Equipment, bool> ValidateRarity(RarityOptions rarityOptions)
        {
            return (item) => {
                if (item.Rarity == EquipmentRarity.Normal && rarityOptions == RarityOptions.Normal)
                {
                    return true;
                }

                if (item.Rarity == EquipmentRarity.Magic && rarityOptions == RarityOptions.Magic)
                {
                    return true;
                }

                if (item.Rarity == EquipmentRarity.Rare && rarityOptions == RarityOptions.Rare)
                {
                    return true;
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
                    return item.Prefixes.Count < affixCount;
                }

                if (explicitOptions == ExplicitOptions.Suffix)
                {
                    return item.Suffixes.Count < affixCount;
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
                    return item.Implicits != null;
                }

                if (implicitOptions == ImplicitOptions.None)
                {
                    return item.Implicits == null;
                }

                if (implicitOptions == ImplicitOptions.Variable)
                {
                    foreach (var stat in item.Implicits)
                    {
                        bool hasImplicit = item.Implicits != null && stat != null && stat.Affix != null;
                        if (hasImplicit)
                        {
                            bool isStat1Variable = stat.Affix.StatName1 != null &&
                                                   stat.Affix.StatMin1 != stat.Affix.StatMax1;
                            bool isStat2Variable = stat.Affix.StatName2 != null &&
                                                   stat.Affix.StatMin2 != stat.Affix.StatMax2;
                            bool isStat3Variable = stat.Affix.StatName3 != null &&
                                                   stat.Affix.StatMin3 != stat.Affix.StatMax3;
                            if (isStat1Variable || isStat2Variable || isStat3Variable)
                            {
                                return true;
                            }
                        }
                    }

                    return false;
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

        public Func<Equipment, bool> ValidateCanAddMasterMod(bool addingMultiMod)
        {
            return (item) =>
            {
                int masterModCount = item.Stats.Count(x => x.Affix.TierType == TierType.Craft);
                return masterModCount == 0 || (masterModCount <= 2 && (addingMultiMod || HasMultiMod(item)));
            };
        }

        private static bool HasMultiMod(Equipment item)
        {
            return item.Stats.Any(x => x.Affix.Group == _multimodGroup);
        }

        public Func<Equipment, bool> ValidateHasInfluence(InfluenceOptions hasInfluenceArg)
        {
            return (item) =>
            {
                switch (hasInfluenceArg)
                {
                    case (InfluenceOptions.None):
                        return item.Influence.Count == 0;
                    case (InfluenceOptions.One):
                        return item.Influence.Count == 1;
                }
                throw new InvalidOperationException("Invalid matching item class validation");
            };
        }

        public Func<Equipment, bool> ValidateItemClass(HashSet<string> itemClasses, GenericOptions genericOptions)
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

        public Func<Equipment, bool> ValidateItemClass(HashSet<string> itemClasses)
        {
            return (item) => itemClasses.Contains(item.ItemBase.ItemClass);
        }

        public Func<Equipment, bool> ValidateItemClass(string itemClass)
        {
            return (item) => item.ItemBase.ItemClass == itemClass;
        }

        public Func<Equipment, bool> CanAddQuality(QualityType qualityType)
        {
            return (item) => item.QualityType != qualityType || item.Quality < 20;
        }
    }
}
