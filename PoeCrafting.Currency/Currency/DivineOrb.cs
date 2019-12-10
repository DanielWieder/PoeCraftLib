using System;
using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class DivineOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.DivineOrb;
        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };
        public DivineOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal)
            {
                return false;
            }

            foreach (var stat in item.Stats)
            {
                StatFactory.Reroll(Random, item, stat);
            }

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity | EquipmentRarity.Normal) == EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return status.Rarity == EquipmentRarity.Normal || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            return status;
        }
    }
}
