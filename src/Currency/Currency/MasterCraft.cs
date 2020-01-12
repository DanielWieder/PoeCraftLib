using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class MasterCraft : ICurrency
    {
        private static String Metamod = "ItemGenerationCanHaveMultipleCraftedMods";

        private readonly Dictionary<string, MasterMod> _masterMods;

        private Dictionary<string, int> _currency;
        private readonly string _generationType;

        private IRandom Random { get; set; }

        public MasterCraft(Dictionary<string, MasterMod> masterMods, IRandom random)
        {
            // All master mods in the map should have the same name and generation type since it's for the same affix on different item classes
            Name = masterMods.First().Value.Name;
            _generationType = masterMods.First().Value.Affix.GenerationType;

            _masterMods = masterMods;
            Random = random;
        }
        public string Name { get; }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (!_masterMods.ContainsKey(item.ItemBase.ItemClass))
            {
                return new Dictionary<string, int>();
            }

            if (!ItemHasGroup(item, Metamod) && item.Stats.Any(x => x.Affix.TierType == TierType.Craft))
            {
                return new Dictionary<string, int>();
            }

            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal)
            {
                return new Dictionary<string, int>();
            }

            var masterMod = _masterMods[item.ItemBase.ItemClass];

            _currency = new Dictionary<string, int>()
            {
                {
                    masterMod.CurrencyType, masterMod.CurrencyCost
                }
            };

            StatFactory.AddExplicit(Random, item, masterMod.Affix);

            return _currency;

        }

        public bool IsWarning(ItemStatus status)
        {
            if (IsError(status)) return false;

            if (_generationType == "prefix")
            {
                if (status.MightBeRarity(EquipmentRarity.Rare))
                {
                    return status.MaxPrefixes == 3;
                }

                if (status.MightBeRarity(EquipmentRarity.Magic))
                {
                    return status.MaxPrefixes == 1;
                }

                return false;
            }
            else if (_generationType == "suffix")
            {
                if (status.MightBeRarity(EquipmentRarity.Rare))
                {
                    return status.MaxSuffixes == 3;
                }

                if (status.MightBeRarity(EquipmentRarity.Magic))
                {
                    return status.MaxSuffixes == 1;
                }

                return false;
            }

            return true;
        }

        public bool IsError(ItemStatus status)
        {
            if (status.Rarity == EquipmentRarity.Normal || status.IsCorrupted)
            {
                return true;
            }

            if (_generationType == "prefix")
            {
                if (status.IsRarity(EquipmentRarity.Rare))
                {
                    return status.MaxPrefixes == 3;
                }

                if (status.IsRarity(EquipmentRarity.Magic))
                {
                    return status.MaxPrefixes == 1;
                }

                return false;
            }
            else if (_generationType == "suffix")
            {
                if (status.IsRarity(EquipmentRarity.Rare))
                {
                    return status.MaxSuffixes == 3;
                }

                if (status.IsRarity(EquipmentRarity.Magic))
                {
                    return status.MaxSuffixes == 1;
                }

                return false;
            }

            return false;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
                return status;

            if (_generationType == "prefix")
            {
                status.MinPrefixes = Math.Min(status.MinPrefixes + 1, 3);
                status.MaxPrefixes = Math.Max(status.MinPrefixes, status.MaxPrefixes);
            }
            if (_generationType == "suffix")
            {
                status.MinSuffixes = Math.Min(status.MinSuffixes + 1, 3);
                status.MaxSuffixes = Math.Max(status.MinSuffixes, status.MaxSuffixes);
            }

            status.MinAffixes = Math.Min(status.MinAffixes + 1, 6);
            status.MaxAffixes = Math.Max(status.MinAffixes, status.MaxAffixes);

            return status;
        }

        private static bool ItemHasGroup(Equipment item, string affix)
        {
            return item.Stats.Any(x => x.Affix.Group == affix);
        }
    }
}
