using System;
using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class ChaosOrb : ICurrency
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.ChaosOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public ChaosOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Rare)
            {
                return false;
            }

            int fourMod = 8;
            int fiveMod = 3;
            int sixMod = 1;

            var sum = fourMod + fiveMod + sixMod;

            var roll = Random.Next(sum);
            int modCount = roll < fourMod ? 4 : 
                           roll < fourMod + fiveMod ? 5 : 
                           6;

            item.Stats.Clear();
            for (int i = 0; i < modCount; i++)
            {
                StatFactory.AddExplicit(Random, item, affixManager);
            }

            return true;
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
