using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class VaalOrb : ICurrency
    {
        private IRandom Random { get; }
        private ICurrency Chaos { get; }

        public string Name => CurrencyNames.VaalOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.VaalOrb, 1 } };

        public VaalOrb(IRandom random)
        {
            Random = random;
            Chaos = new ChaosOrb(random);
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted)
            {
                return new Dictionary<string, int>();
            }

            var roll = Random.Next(4);

            if (roll == 0)
            {
                StatFactory.SetImplicit(Random, item);
            }
            if (roll == 1)
            {
                item.Rarity = EquipmentRarity.Rare;
                return Chaos.Execute(item, affixManager);
            }

            item.Corrupted = true;

            return _currency;
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }

            status.MinPrefixes = Math.Min(1, status.MinPrefixes);
            status.MinSuffixes = Math.Min(1, status.MinSuffixes);
            status.MinAffixes = Math.Min(4, status.MinAffixes);
            status.MaxPrefixes = 3;
            status.MaxSuffixes = 3;
            status.MaxAffixes = 6;

            status.Rarity = status.Rarity |= EquipmentRarity.Rare;
            status.IsCorrupted = true;
            return status;
        }
    }
}
