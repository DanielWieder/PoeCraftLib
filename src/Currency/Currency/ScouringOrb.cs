using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class ScouringOrb : ICurrency
    {
        public string Name => CurrencyNames.ScouringOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.ScouringOrb, 1 } };

        public ScouringOrb(IRandom random)
        { }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal || item.Rarity == EquipmentRarity.Unique)
            {
                return new Dictionary<string, int>();
            }

            StatFactory.RemoveAllExplicits(item);

            if (!item.Stats.Any()) item.Rarity = EquipmentRarity.Normal;

            return _currency;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity & EquipmentRarity.Normal) == EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity == EquipmentRarity.Normal || status.IsCorrupted;
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
                status.MinAffixes = Math.Min(0, status.MinAffixes);

                status.Rarity = status.Rarity & EquipmentRarity.Magic & EquipmentRarity.Rare | EquipmentRarity.Normal;
            }
            else
            {
                status.MinPrefixes = 0;
                status.MinSuffixes = 0;
                status.MinAffixes = 0;

                status.MaxPrefixes = 0;
                status.MaxSuffixes = 0;
                status.MaxAffixes = 0;

                status.Rarity = EquipmentRarity.Normal;
            }

            return status;
        }
    }
}
