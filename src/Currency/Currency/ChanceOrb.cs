using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class ChanceOrb : ICurrency
    {
        private readonly IRandom _random;

        public string Name => CurrencyNames.ChanceOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.ChanceOrb, 1 } };

        public ChanceOrb(IRandom random)
        {
            _random = random;

        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Normal)
            {
                return new Dictionary<string, int>();
            }

            // Unique items are not currently handled
            var roll = _random.Next(5);

            if (roll == 0)
            {
                item.Rarity = EquipmentRarity.Rare;
                StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds);

            }
            else
            {
                item.Rarity = EquipmentRarity.Magic;
                StatFactory.AddExplicits(_random, item, affixManager, StatFactory.MagicAffixCountOdds);
            }

            return _currency;
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
