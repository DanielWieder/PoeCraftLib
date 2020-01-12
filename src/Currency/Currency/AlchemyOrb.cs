using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class AlchemyOrb : ICurrency
    {

        public string Name => CurrencyNames.AlchemyOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.AlchemyOrb, 1 } };
        private readonly IRandom _random;

        public AlchemyOrb(IRandom random)
        {
            _random = random;
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return new Dictionary<string, int>();
            }

            item.Rarity = EquipmentRarity.Rare;
            StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds);
            return _currency;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Normal) != EquipmentRarity.Normal || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.Rarity = status.Rarity & ~EquipmentRarity.Normal | EquipmentRarity.Rare;
            }
            else
            {
                status.Rarity = EquipmentRarity.Rare;
            }

            if (IsWarning(status))
            {
                status.MinPrefixes = Math.Min(0, status.MinPrefixes);
                status.MinSuffixes = Math.Min(0, status.MinSuffixes);
                status.MinAffixes = Math.Min(0, status.MinAffixes);

                status.MaxPrefixes = Math.Max(0, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(0, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(0, status.MaxAffixes);
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
