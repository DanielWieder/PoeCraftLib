using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class DivineOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.DivineOrb;
        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.DivineOrb, 1 } };
        public DivineOrb(IRandom random)
        {
            Random = random;
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.Corrupted || item.Rarity == EquipmentRarity.Normal)
            {
                return new Dictionary<string, int>();
            }

            foreach (var stat in item.Stats)
            {
                StatFactory.Reroll(Random, item, stat);
            }

            return _currency;
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
