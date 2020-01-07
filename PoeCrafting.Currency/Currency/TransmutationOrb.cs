using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class TransmutationOrb : ICurrency
    {
        private ICurrency Alteration { get; }

        public string Name => CurrencyNames.TransmuationOrb;
        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public TransmutationOrb(IRandom random)
        {
            Alteration = new AlterationOrb(random);
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {

            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            item.Rarity = EquipmentRarity.Magic;

            return Alteration.Execute(item, affixManager);
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
                status.MinPrefixes = Math.Min(0, status.MinPrefixes);
                status.MinSuffixes = Math.Min(0, status.MinSuffixes);
                status.MinAffixes = Math.Min(1, status.MinAffixes);

                status.MaxPrefixes = Math.Max(1, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(1, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(2, status.MaxAffixes);

                status.Rarity = status.Rarity & ~EquipmentRarity.Normal | EquipmentRarity.Magic;
            }
            else
            {
                status.MinPrefixes = 0;
                status.MinSuffixes = 0;
                status.MinAffixes = 1;

                status.MaxPrefixes = 1;
                status.MaxSuffixes = 1;
                status.MaxAffixes = 2;

                status.Rarity = EquipmentRarity.Magic;
            }

            return status;
        }
    }
}
