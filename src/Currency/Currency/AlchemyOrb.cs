using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class AlchemyOrb : ICurrency
    {
        private ICurrency Chaos { get; }

        public string Name => CurrencyNames.AlchemyOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public AlchemyOrb(IRandom random)
        {
            Chaos = new ChaosOrb(random);
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Rare;
            return Chaos.Execute(item, affixManager);
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

            return Chaos.GetNextStatus(status);
        }
    }
}
