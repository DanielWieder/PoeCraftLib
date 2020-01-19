using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class ChaosOrb : ICurrency
    {
        private readonly IRandom _random;

        public string Name => CurrencyNames.ChaosOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.ChaosOrb, 1 } };

        public ChaosOrb(IRandom random)
        {
            _random = random;
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Rare)
            {
                return new Dictionary<string, int>();
            }

            item.Stats.Clear();
            StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds);
            return _currency;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Rare;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Rare) != EquipmentRarity.Rare || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.MinPrefixes = Math.Min(1, status.MinPrefixes);
                status.MinSuffixes = Math.Min(1, status.MinSuffixes);
                status.MinAffixes = Math.Min(4, status.MinAffixes);

                status.MaxPrefixes = Math.Max(3, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(3, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(6, status.MaxAffixes);
            }
            else
            {
                status.MinPrefixes = 1;
                status.MinSuffixes = 1;
                status.MinAffixes = 4;

                status.MaxPrefixes = 3;
                status.MaxSuffixes = 3;
                status.MaxAffixes = 6;
            }

            return status;
        }
    }
}
