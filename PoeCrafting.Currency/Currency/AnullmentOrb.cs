using System;
using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class AnullmentOrb : ICurrency
    {
        public string Name => CurrencyNames.AnullmentOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        private readonly IRandom _random;

        public AnullmentOrb(IRandom random)
        {
            _random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal || item.Rarity == EquipmentRarity.Unique || item.Stats.Count == 0)
            {
                return false;
            }

            StatFactory.RemoveExplicit(_random, item);

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity & EquipmentRarity.Normal) == EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity == EquipmentRarity.Normal || status.IsCorrupted || status.MaxAffixes == 0;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.MinAffixes = Math.Max(0, status.MinAffixes - 1);
                status.MinPrefixes = Math.Max(0, status.MinPrefixes - 1);
                status.MinSuffixes = Math.Max(0, status.MinSuffixes - 1);
            }
            else
            {
                status.MaxAffixes = Math.Max(0, status.MaxAffixes - 1);
                status.MinAffixes = Math.Max(0, status.MinAffixes - 1);

                status.MinPrefixes = Math.Max(0, status.MinPrefixes - 1);
                status.MinSuffixes = Math.Max(0, status.MinSuffixes - 1);

                if (status.MaxPrefixes == 0)
                {
                    status.MaxSuffixes = Math.Max(0, status.MaxSuffixes - 1);
                }
                else if (status.MaxSuffixes == 0)
                {
                    status.MaxSuffixes = Math.Max(0, status.MaxPrefixes - 1);
                }
            }

            return status;
        }
    }
}
