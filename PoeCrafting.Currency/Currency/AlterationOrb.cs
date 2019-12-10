using System;
using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class AlterationOrb : ICurrency
    {
        private IRandom Random { get; set; }

        public string Name => CurrencyNames.AlterationOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public AlterationOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic)
            {
                return false;
            }

            item.Stats.Clear();

            int affixCount = Random.Next(2) + 1;
            for (int i = 0; i < affixCount; i++)
            {
                StatFactory.AddExplicit(Random, item, affixManager);
            }

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Magic;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Magic) != EquipmentRarity.Magic || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.MinPrefixes = Math.Min(0, status.MinPrefixes);
                status.MinSuffixes = Math.Min(0, status.MinSuffixes);
                status.MinAffixes = Math.Min(2, status.MinAffixes);

                status.MaxPrefixes = Math.Max(1, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(1, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(2, status.MaxAffixes);
            }
            else
            {
                status.MinPrefixes = 0;
                status.MinSuffixes = 0;
                status.MinAffixes = 1;

                status.MaxPrefixes = 1;
                status.MaxSuffixes = 1;
                status.MaxAffixes = 2;
            }

            return status;
        }
    }
}
