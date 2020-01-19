using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class EssenceCraft : ICurrency
    {
        private readonly IRandom _random;

        private const int LevelToReroll = 6;

        private Essence _essence;

        private readonly Dictionary<string, int> _currency;

        public EssenceCraft(IRandom random, Essence essence)
        {
            this._random = random;
            this._essence = essence;
            _currency = new Dictionary<string, int>() { { _essence.Name, 1 } };
        }

        public string Name => _essence.Name;
        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {

            if (item.Corrupted) return new Dictionary<string, int>();

            if (_essence.Level < LevelToReroll && item.Rarity == EquipmentRarity.Rare) return new Dictionary<string, int>();

            if (item.Rarity != EquipmentRarity.Normal) return new Dictionary<string, int>();

            item.Stats.Clear();

            item.Rarity = EquipmentRarity.Rare;
            var mod = _essence.ItemClassToMod[item.ItemBase.ItemClass];
            StatFactory.AddExplicit(_random, item, mod);
            StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds);

            return _currency;
        }

        public bool IsWarning(ItemStatus status)
        {
            if (IsError(status)) return false;

            if (_essence.Level >= LevelToReroll && status.Rarity == EquipmentRarity.Rare) return false;

            return (status.Rarity != EquipmentRarity.Normal);
        }

        public bool IsError(ItemStatus status)
        {
            if (status.IsCorrupted) return true;

            if (_essence.Level >= LevelToReroll &&
                (status.Rarity & EquipmentRarity.Rare) == EquipmentRarity.Rare) return false;

            return (status.Rarity & EquipmentRarity.Normal) != EquipmentRarity.Normal;
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
