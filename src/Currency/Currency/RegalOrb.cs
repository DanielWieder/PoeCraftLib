using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class RegalOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.RegalOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.RegalOrb, 1 } };

        public RegalOrb(IRandom random)
        {
            Random = random;
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic)
            {
                return new Dictionary<string, int>();
            }

            item.Rarity = EquipmentRarity.Rare;

            StatFactory.AddExplicit(Random, item, affixManager);

            return _currency;
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
                status.MaxPrefixes = Math.Min(3, status.MaxPrefixes++);
                status.MaxSuffixes = Math.Min(3, status.MaxSuffixes++);
                status.MaxAffixes = Math.Min(6, status.MaxAffixes++);

                status.Rarity = status.Rarity & ~EquipmentRarity.Magic | EquipmentRarity.Rare;
            }
            else
            {
                status.Rarity = EquipmentRarity.Rare;

                status.MinAffixes++;
                status.MaxPrefixes++;
                status.MaxSuffixes++;
                status.MaxAffixes++;
            }

            return status;
        }
    }
}
