using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency
{
    public class EquipmentModifiers
    {
        private static readonly string NoAttackMods = "ItemGenerationCannotRollAttackAffixes";
        private static readonly string NoCasterMods = "ItemGenerationCannotRollCasterAffixes";

        public List<Influence> Influence { get; }
        public List<string> AddedTags { get; }
        public List<string> MasterMods { get; }
        public int ItemLevel { get; }
        public IDictionary<string, double> GenerationWeights { get; }

        public EquipmentModifiers(Equipment equipment)
        {
            AddedTags = equipment.Stats
                .SelectMany(x => x.Affix.AddsTags)
                .Distinct()
                .OrderByDescending(x => x)
                .ToList();

            MasterMods = equipment.Stats
                .Where(x => x.Affix.Group.Contains(NoAttackMods) || x.Affix.Group.Contains(NoCasterMods))
                .Select(x => x.Affix.Group)
                .OrderByDescending(x => x)
                .ToList();

            Influence = equipment.Influence
                .OrderByDescending(x => x)
                .ToList();

            ItemLevel = equipment.ItemLevel;

            GenerationWeights = equipment.Stats.SelectMany(x => x.Affix.GenerationWeights)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Aggregate(1d, (y, z) => y * z.Value / 100d));
        }

        public EquipmentModifiers(List<Influence> influence, List<string> addedTags, List<string> masterMods, int itemLevel, IDictionary<string, int> generationWeights)
        {
            Influence = influence;
            AddedTags = addedTags;
            MasterMods = masterMods;
            ItemLevel = itemLevel;
            GenerationWeights = generationWeights.ToDictionary(x => x.Key, x => (double)x.Value);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((EquipmentModifiers)obj);
        }

        protected bool Equals(EquipmentModifiers other)
        {
            for (int i = 0; i < Influence.Count; i++)
            {
                if (Influence[i] != other.Influence[i]) return false;
            }

            for (int i = 0; i < AddedTags.Count; i++)
            {
                if (AddedTags[i] != other.AddedTags[i]) return false;
            }

            for (int i = 0; i < MasterMods.Count; i++)
            {
                if (MasterMods[i] != other.MasterMods[i]) return false;
            }

            foreach (var weightModifier in GenerationWeights)
            {
                if (!other.GenerationWeights.ContainsKey(weightModifier.Key)) return false;
                if (Math.Abs(other.GenerationWeights[weightModifier.Key] - weightModifier.Value) > double.Epsilon) return false;
            }

            return ItemLevel == other.ItemLevel;
        }

        public int GetHashCode()
        {
            var hashCode = MutateHashCode(ItemLevel);

            foreach (var x in Influence)
            {
                hashCode = MutateHashCode(hashCode, x);
            }

            foreach (var x in AddedTags)
            {
                hashCode = MutateHashCode(hashCode, x);
            }

            foreach (var x in MasterMods)
            {
                hashCode = MutateHashCode(hashCode, x);
            }

            return hashCode;
        }

        private int MutateHashCode(object o)
        {
            return o?.GetHashCode() ?? 0;
        }

        private int MutateHashCode(int hashCode, object o)
        {
            unchecked
            {
                return (hashCode * 397) ^ (o?.GetHashCode() ?? 0);
            }
        }

        public static bool operator ==(EquipmentModifiers left, EquipmentModifiers right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EquipmentModifiers left, EquipmentModifiers right)
        {
            return !Equals(left, right);
        }
    }
}
