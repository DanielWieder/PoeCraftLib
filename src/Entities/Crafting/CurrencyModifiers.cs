using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PoeCraftLib.Entities;

namespace PoeCraftLib.Currency.Currency
{
    public class CurrencyModifiers
    {
        // Todo: Make all properties readonly when updated to C# 8.0
        public IReadOnlyList<Affix> AddedExplicits { get; }

        public IReadOnlyDictionary<string, double> ExplicitWeightModifiers { get; }

        public int ItemLevelRestriction { get; }

        public bool RollsLucky { get; }

        public bool QualityAffectsExplicitOdds { get; }

        private int HashCode { get; }

        public CurrencyModifiers() : this(null, null, null, null, null)
        {
        }

        public CurrencyModifiers(IReadOnlyList<Affix> addedExplicits, 
            IReadOnlyDictionary<string, double> explicitWeightModifiers, 
            int? itemLevelRestriction,
            bool? rollsLucky,
            bool? qualityAffectsExplicitOdds)
        {
            AddedExplicits = addedExplicits ?? new List<Affix>();
            ExplicitWeightModifiers = explicitWeightModifiers ?? new Dictionary<string, double>();
            ItemLevelRestriction = itemLevelRestriction ?? 100;
            RollsLucky = rollsLucky ?? false;
            QualityAffectsExplicitOdds = qualityAffectsExplicitOdds ?? false;

            HashCode = CalculateHashCode();
        }


        protected bool Equals(CurrencyModifiers other)
        {
            for (int i = 0; i < AddedExplicits.Count; i++)
            {
                var localExplicits = FormatExplicits(AddedExplicits);
                var otherExplicits = FormatExplicits(other.AddedExplicits);

                if (localExplicits[i] != otherExplicits[i]) return false;
            }

            foreach (var weightModifier in ExplicitWeightModifiers)
            {
                if (!other.ExplicitWeightModifiers.ContainsKey(weightModifier.Key)) return false;
                if (Math.Abs(other.ExplicitWeightModifiers[weightModifier.Key] - weightModifier.Value) > double.Epsilon) return false;
            }

            return ItemLevelRestriction == other.ItemLevelRestriction && 
                   RollsLucky == other.RollsLucky &&
                QualityAffectsExplicitOdds == other.QualityAffectsExplicitOdds;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((CurrencyModifiers) obj);
        }

        // Cache the hash code for performance reasons
        // All properties should be readonly
        public override int GetHashCode()
        {
            return HashCode;
        }

        public int CalculateHashCode()
        {
            var hashCode = MutateHashCode(ItemLevelRestriction);
            hashCode = MutateHashCode(hashCode, RollsLucky);
            hashCode = MutateHashCode(hashCode, QualityAffectsExplicitOdds);

            if (AddedExplicits != null)
            {
                foreach (var x in FormatExplicits(AddedExplicits))
                {
                    hashCode = MutateHashCode(hashCode, x);
                }
            }

            if (ExplicitWeightModifiers != null)
            {
                foreach (var x in FormatWeightModifiers(ExplicitWeightModifiers))
                {
                    hashCode = MutateHashCode(hashCode, x.Key);
                    hashCode = MutateHashCode(hashCode, x.Value);
                }
            }

            return hashCode;
        }

        private static IList<KeyValuePair<string, double>> FormatWeightModifiers(IReadOnlyDictionary<string, double> weightModifiers)
        {
            return weightModifiers.Select(x => x).OrderByDescending(x => x.Key).ToList();
        }

        private static IList<string> FormatExplicits(IReadOnlyList<Affix> explicits)
        {
            return explicits.Select(x => x.FullName).OrderByDescending(x => x).ToList();
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

        public static bool operator ==(CurrencyModifiers left, CurrencyModifiers right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(CurrencyModifiers left, CurrencyModifiers right)
        {
            return !Equals(left, right);
        }
    }
}