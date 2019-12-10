using System;
using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class AugmentationOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => CurrencyNames.AugmentationOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };
        public AugmentationOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic || item.Prefixes.Count + item.Suffixes.Count != 1)
            {
                return false;
            }

            StatFactory.AddExplicit(Random, item, affixManager);

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity != EquipmentRarity.Magic ||  status.MaxPrefixes + status.MaxPrefixes == 2);
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Magic) != EquipmentRarity.Magic || status.IsCorrupted || status.MinPrefixes + status.MinSuffixes == 2;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }

            if (status.Rarity != EquipmentRarity.Magic && IsWarning(status))
            {
                status.MinPrefixes = Math.Min(1, status.MinPrefixes);
                status.MinSuffixes = Math.Min(1, status.MinSuffixes);
                status.MinAffixes = Math.Min(2, status.MinAffixes);

                status.MaxPrefixes = Math.Max(1, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(1, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(2, status.MaxAffixes);
            }
            else
            {
                status.MinPrefixes = 1;
                status.MinSuffixes = 1;
                status.MinAffixes = 2;

                status.MaxPrefixes = 1;
                status.MaxSuffixes = 1;
                status.MaxAffixes = 2;
            }

            return status;
        }
    }
}
