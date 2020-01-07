using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class ChanceOrb : ICurrency
    {
        private IRandom Random { get; }
        private ICurrency Alchemy { get; }
        private ICurrency Transmutation { get; }

        public string Name => CurrencyNames.ChanceOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public ChanceOrb(IRandom random)
        {
            Random = random;
            Alchemy = new AlchemyOrb(random);
            Transmutation = new TransmutationOrb(random);
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return false;
            }

            // Unique items are not currently handled
            var roll = Random.Next(5);

            if (roll == 0)
            {
                return Alchemy.Execute(item, affixManager);
            }
            else
            {
                return Transmutation.Execute(item, affixManager);
            }
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && (status.Rarity != EquipmentRarity.Normal);
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

                status.MaxPrefixes = Math.Max(3, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(3, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(6, status.MaxAffixes);

                status.Rarity = status.Rarity & ~EquipmentRarity.Normal | EquipmentRarity.Magic | EquipmentRarity.Rare;
            }
            else
            {
                status.MinPrefixes = 0;
                status.MinSuffixes = 0;
                status.MinAffixes = 1;

                status.MaxPrefixes = 3;
                status.MaxSuffixes = 3;
                status.MaxAffixes = 6;

                status.Rarity = EquipmentRarity.Magic | EquipmentRarity.Rare;
            }

            return status;
        }
    }
}
