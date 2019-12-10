using System;
using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class RegalOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.RegalOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public RegalOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Magic)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Rare;

            StatFactory.AddExplicit(Random, item, affixManager);

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
