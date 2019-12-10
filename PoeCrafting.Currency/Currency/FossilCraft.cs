using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class FossilCraft : ICurrency
    {
        private IRandom Random { get; }

        public string Name => "Fossil";

        public List<Fossil> Fossils { get; set; }

        public Dictionary<string, int> GetCurrency() => Fossils.ToDictionary(x => x.Name, x => 1);

        private List<Essence> _corruptionOnlyEssences { get; }

        public FossilCraft(IRandom random, List<Fossil> fossils, List<Essence> essences)
        {
            Random = random;
            this.Fossils = fossils;
            this._corruptionOnlyEssences = essences.Where(x => x.Tier == 6).ToList();
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity != EquipmentRarity.Rare)
            {
                return false;
            }

            var corruptedEssenceChance = Fossils.Max(x => x.CorruptedEssenceChance);

            bool addCorruptedEssence = false;

            if (corruptedEssenceChance == 100)
            {
                addCorruptedEssence = true;
            }
            else if (corruptedEssenceChance > 0)
            {
                var chance = Random.Next(100);
                if (chance >= corruptedEssenceChance)
                {
                    addCorruptedEssence = true;
                }
            }

            int addedAffixes = 0;

            if (addCorruptedEssence)
            {
                var essence = _corruptionOnlyEssences[Random.Next(_corruptionOnlyEssences.Count)];

                StatFactory.AddExplicit(Random, item, essence.ItemClassToMod[item.ItemBase.ItemClass]);

                addedAffixes = 1;
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
            for (int i = addedAffixes; i < modCount; i++)
            {
                StatFactory.AddExplicit(Random, item, affixManager, Fossils);
            }

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Rare && status.Rarity != EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Rare) != EquipmentRarity.Rare && 
                   (status.Rarity & EquipmentRarity.Normal) != EquipmentRarity.Normal || 
                   status.IsCorrupted;
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
