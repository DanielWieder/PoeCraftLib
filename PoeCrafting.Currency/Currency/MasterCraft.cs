using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class MasterCraft : ICurrency
    {
        private static String Metamod = "ItemGenerationCanHaveMultipleCraftedMods";

        private readonly MasterMod _masterMod;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>()
        {
            {
                _masterMod.CurrencyType, _masterMod.CurrencyCost
            }
        };

        private IRandom Random { get; set; }

        public MasterCraft(MasterMod masterMod, IRandom random)
        {
            _masterMod = masterMod;
            Random = random;
        }
        public string Name => _masterMod.Name;
        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (!_masterMod.ItemClasses.Contains(item.ItemBase.ItemClass))
            {
                return false;
            }

            if (!ItemHasGroup(item, Metamod) && item.Stats.Any(x => x.Affix.TierType == TierType.Craft))
            {
                return false;
            }

            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal)
            {
                return false;
            }

            StatFactory.AddExplicit(Random, item, _masterMod.Affix);

            return true;

        }

        public bool IsWarning(ItemStatus status)
        {
            if (IsError(status)) return false;

            if (_masterMod.Affix.GenerationType == "prefix")
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
            else if (_masterMod.Affix.GenerationType == "suffix")
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

            if (_masterMod.Affix.GenerationType == "prefix")
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
            else if (_masterMod.Affix.GenerationType == "suffix")
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

            if (_masterMod.Affix.GenerationType == "prefix")
            {
                status.MinPrefixes = Math.Min(status.MinPrefixes + 1, 3);
                status.MaxPrefixes = Math.Max(status.MinPrefixes, status.MaxPrefixes);
            }
            if (_masterMod.Affix.GenerationType == "suffix")
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
