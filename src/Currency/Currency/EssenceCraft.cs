using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class EssenceCraft : ICurrency
    {
        private IRandom Random { get; }

        private const int LevelToReroll = 6;

        private Essence _essence;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public EssenceCraft(IRandom random, Essence essence)
        {
            this._essence = essence;
        }

        public string Name => _essence.Name;
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

            var mod = _essence.ItemClassToMod[item.ItemBase.ItemClass];
            StatFactory.AffixToStat(Random, item, mod);

            for (int i = 1; i < modCount; i++)
            {
                StatFactory.AddExplicit(Random, item, affixManager);
            }

            return true;
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
