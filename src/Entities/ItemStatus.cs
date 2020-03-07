

using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Entities
{
    public class ItemStatus : ICloneable
    {
        public int MinPrefixes = 0;
        public int MaxPrefixes = 0;
        public int MinSuffixes = 0;
        public int MaxSuffixes = 0;
        public int MinAffixes = 0;
        public int MaxAffixes = 0; 

        public bool IsCorrupted = false;

        public bool HasImplicit = false;

        public bool Completed { get; set; } = false;

        public EquipmentRarity Rarity = EquipmentRarity.Normal;

        public bool IsRarity(EquipmentRarity rarity)
        {
            return this.Rarity == rarity;
        }

        public bool MightBeRarity(EquipmentRarity rarity)
        {
            return (Rarity & rarity) != rarity;
        }

        public bool Validate()
        {
            int affixCap = GetAffixCap();

            return MinPrefixes <= MaxPrefixes &&
                   MinSuffixes <= MaxSuffixes &&
                   MinAffixes <= MaxAffixes &&
                   MinPrefixes <= MinAffixes &&
                   MinSuffixes <= MinAffixes &&
                   MaxPrefixes <= affixCap &&
                   MaxSuffixes <= affixCap &&
                   MaxAffixes <= affixCap * 2;
        }

        public int GetAffixCap()
        {
            return GetAffixCap(Rarity);
        }

        public int GetAffixCap(EquipmentRarity rarity)
        {
            return MightBeRarity(EquipmentRarity.Rare) ? 3 :
                MightBeRarity(EquipmentRarity.Magic) ? 1 : 0;
        }

        public object Clone()
        {
            return new ItemStatus
            {
                Rarity = Rarity,
                HasImplicit = HasImplicit,
                IsCorrupted = IsCorrupted,
                MaxAffixes = MaxAffixes,
                MaxPrefixes = MaxPrefixes,
                MaxSuffixes = MaxSuffixes,
                MinPrefixes = MinPrefixes,
                MinSuffixes = MinSuffixes,
                MinAffixes = MinAffixes,
                Completed = Completed
            };
        }

        public static ItemStatus Combine(List<ItemStatus> status)
        {
            if (status.All(x => x.Completed))
            {
                throw new InvalidOperationException("Unable to combine completed item statuses");
            }

            status = status.Where(x => !x.Completed).ToList();

            return new ItemStatus
            {
                Completed = false,
                Rarity = status.Select(x => x.Rarity).Aggregate((x, y) => x | y),
                HasImplicit = status.Any(x => x.HasImplicit),
                IsCorrupted = status.Any(x => x.IsCorrupted),
                MaxAffixes = status.Max(x => x.MaxAffixes),
                MaxPrefixes = status.Max(x => x.MaxPrefixes),
                MaxSuffixes = status.Max(x => x.MaxSuffixes),
                MinPrefixes = status.Min(x => x.MinPrefixes),
                MinSuffixes = status.Min(x => x.MinSuffixes),
                MinAffixes = status.Min(x => x.MinAffixes)
            };
        }

        public bool AreEqual(ItemStatus other)
        {
            return
                Completed == other.Completed &&
                Rarity == other.Rarity && 
                HasImplicit == other.HasImplicit &&
                IsCorrupted == other.IsCorrupted &&
                MaxAffixes == other.MaxAffixes &&
                MaxPrefixes == other.MaxPrefixes &&
                MaxSuffixes == other.MaxSuffixes &&
                MinPrefixes == other.MinPrefixes &&
                MinSuffixes == other.MinSuffixes &&
                MinAffixes == other.MinAffixes;
        }
    }
}
