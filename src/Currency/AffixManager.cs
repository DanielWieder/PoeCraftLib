using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency
{
    public class AffixManager
    {
        private static readonly string _noAttackMods = "ItemGenerationCannotRollAttackAffixes";
        private static readonly string _noCasterMods = "ItemGenerationCannotRollCasterAffixes";

        private static readonly string _casterTag = "caster";
        private static readonly string _attackTag = "attack";

        private readonly List<Affix> _itemAffixes;
        private readonly List<string> _baseTags;

        private readonly Dictionary<PoolKey, AffixPool> _poolDic = new Dictionary<PoolKey, AffixPool>();

        private readonly Dictionary<string, Affix> _allAffixes;
        private readonly Dictionary<Influence, List<Affix>> _influenceAffixes;
        private readonly Dictionary<Influence, string> _influenceTags;

        public AffixManager(ItemBase itemBase, 
            List<Affix> itemAffixes, 
            List<Affix> currencyAffixes, 
            Dictionary<Influence, List<Affix>> influenceAffixes,
            Dictionary<Influence, string> influenceTags)
        {
            _allAffixes = itemAffixes
                .Union(currencyAffixes)
                .Union(influenceAffixes.SelectMany(x => x.Value))
                .GroupBy(x => x.FullName)
                .Select(x => x.First())
                .ToDictionary(x => x.FullName, x => x);

            _itemAffixes = itemAffixes;
            _baseTags = itemBase.Tags;
            _influenceAffixes = influenceAffixes;
            _influenceTags = influenceTags;

        }

        public Affix GetAffix(EquipmentModifiers equipmentModifiers, CurrencyModifiers currencyModifiers, List<Affix> affixes, EquipmentRarity rarity, IRandom random)
        {
            PoolKey key = new PoolKey(equipmentModifiers, currencyModifiers);

            if (!_poolDic.ContainsKey(key))
            {
                _poolDic.Add(key, GenerateAffixPool(equipmentModifiers, currencyModifiers));
            }

            AffixPool pool = _poolDic[key];
            var existingGroups = new HashSet<string>(affixes.Select(x => x.Group));

            int affixesCount = rarity == EquipmentRarity.Normal ? 0 :
                rarity == EquipmentRarity.Magic ? 1 :
                rarity == EquipmentRarity.Rare ? 3 : 0;

            double prefixSkipAmount = CalculateSkipAmount("prefix", pool.PrefixWeight, pool.PrefixGroupWeights, affixes, affixesCount);
            double suffixSkipAmount = CalculateSkipAmount("suffix", pool.SuffixWeight, pool.SuffixGroupWeights, affixes, affixesCount);

            var targetWeight = random.NextDouble() * (pool.TotalWeight - prefixSkipAmount - suffixSkipAmount);

            return GetRandomAffixFromPool(pool, targetWeight, existingGroups, prefixSkipAmount, suffixSkipAmount);
        }

        public Affix GetInfluenceAffix(Influence influence, EquipmentModifiers equipmentModifiers, List<Affix> affixes, EquipmentRarity rarity, IRandom random)
        {
            var existingGroups = new HashSet<string>(affixes.Select(x => x.Group));

            int affixesCount = rarity == EquipmentRarity.Normal ? 0 :
                rarity == EquipmentRarity.Magic ? 1 :
                rarity == EquipmentRarity.Rare ? 3 : 0;

            HashSet<string> fullGenTypes = new HashSet<string>();
            if (affixes.Count(x => x.GenerationType == "suffix") >= affixesCount)
            {
                fullGenTypes.Add("suffix");
            }
            if (affixes.Count(x => x.GenerationType == "prefix") >= affixesCount)
            {
                fullGenTypes.Add("prefix");
            }

            var pool = _influenceAffixes[influence]
                .Where(x => x.RequiredLevel <= equipmentModifiers.ItemLevel)
                .Where(x => !existingGroups.Contains(x.Group))
                .Where(x => !fullGenTypes.Contains(x.GenerationType))
                .ToList();

            var tag = _influenceTags[influence];

            var currentWeight = pool.Sum(x => x.SpawnWeights[tag]);
            var randomValue = random.Next(currentWeight);

            foreach (var affix in pool)
            {
                currentWeight -= affix.Weight;
                if (randomValue < currentWeight)
                {
                    return affix;
                } 
            }
            throw new InvalidOperationException("An affix should have been selected");
        }

        private static double CalculateSkipAmount(string generationType, 
            double weightForGenType,
            Dictionary<string, double> groupWeights,
            List<Affix> affixes,
            int affixesCount)
        {
            if (affixes.Count(x => x.GenerationType == generationType) >= affixesCount)
            {
                return weightForGenType;
            }
            else
            {
                return affixes
                    .Select(x => x.Group)
                    .Where(groupWeights.ContainsKey)
                    .Sum(x => groupWeights[x]);
            }
        }

        private Affix GetRandomAffixFromPool(AffixPool pool,  double targetWeight, HashSet<string> existingGroups, double prefixSkipAmount, double suffixSkipAmount)
        {
            Affix affix = null;

            bool skipPrefixes = Math.Abs(prefixSkipAmount - pool.PrefixWeight) < .001;
            bool skipSuffixes = Math.Abs(suffixSkipAmount - pool.SuffixWeight) < .001;

            if (skipPrefixes && skipSuffixes) return null;

            if (targetWeight <= pool.PrefixWeight && !skipPrefixes)
            {
                affix = RandomAffixFromPool(targetWeight, existingGroups, pool.PrefixGroupWeights, 0, pool.PrefixGroupToAffixWeights);
            }

            if (affix != null) return affix;

            if (!skipSuffixes)
            {
                affix = RandomAffixFromPool(targetWeight, existingGroups, pool.SuffixGroupWeights, pool.PrefixWeight - prefixSkipAmount, pool.SuffixGroupToAffixWeights);
            }

            return affix;
        }

        private Affix RandomAffixFromPool(
            double targetWeight, 
            HashSet<string> existingGroups, 
            Dictionary<string, double> groupWeights,
            double currentWeight, 
            Dictionary<string, List<AffixWeight>> groupToAffixWeights)
        {
            foreach (var group in groupWeights)
            {
                if (existingGroups.Contains(group.Key)) continue;

                currentWeight += groupWeights[@group.Key];

                if (currentWeight >= targetWeight)
                {
                    var previousWeight = currentWeight - groupWeights[@group.Key];
                    var groupTargetWeight = targetWeight - previousWeight;

                    var affix = GetRandomAffixFromGroup(groupToAffixWeights[group.Key], groupTargetWeight);

                    if (affix != null)
                    {
                        return affix;
                    }
                }
            }

            return null;
        }

        private Affix GetRandomAffixFromGroup(List<AffixWeight> group, double groupTargetWeight)
        {
            double groupCurrentWeight = 0;
            foreach (var affix in group)
            {
                groupCurrentWeight += affix.Weight;

                if (groupCurrentWeight >= groupTargetWeight)
                {
                    return _allAffixes[affix.ModName];
                }
            }
            return null;
        }

        private AffixPool GenerateAffixPool(EquipmentModifiers equipmentModifiers, CurrencyModifiers currencyModifiers)
        {
            var tags = new HashSet<string>(_baseTags.Union(equipmentModifiers.AddedTags));
            var masterModSet = new HashSet<string>(equipmentModifiers.MasterMods);

            if (currencyModifiers == null)
            {
                currencyModifiers = new CurrencyModifiers(null, null, null, null);
            }

            var addedAffixes = currencyModifiers.AddedExplicits
                .Union(equipmentModifiers.Influence.SelectMany(x => _influenceAffixes[x]))
                .Where(x => x.RequiredLevel <= equipmentModifiers.ItemLevel)
                .ToList();

            var affixesToEvaluate = _itemAffixes.Union(addedAffixes).ToList();

            if (currencyModifiers.ItemLevelRestriction != null && currencyModifiers.ItemLevelRestriction < equipmentModifiers.ItemLevel)
            {
                affixesToEvaluate = affixesToEvaluate
                    .Where(x => x.RequiredLevel <= currencyModifiers.ItemLevelRestriction)
                    .ToList();
            }

            var weightModifiers = equipmentModifiers.GenerationWeights
                .Union(currencyModifiers.ExplicitWeightModifiers)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Aggregate(1d, (y, z) => y * z.Value / 100d));

            AffixPool pool = new AffixPool();

            pool.PrefixGroupToAffixWeights = GroupToAffixWeights(affixesToEvaluate.Where(x => x.GenerationType == "prefix").ToList(), tags, weightModifiers, masterModSet);
            pool.SuffixGroupToAffixWeights = GroupToAffixWeights(affixesToEvaluate.Where(x => x.GenerationType == "suffix").ToList(), tags, weightModifiers, masterModSet);

            pool.PrefixGroupWeights = pool.PrefixGroupToAffixWeights.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.Weight));
            pool.SuffixGroupWeights = pool.SuffixGroupToAffixWeights.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.Weight));

            pool.PrefixWeight = pool.PrefixGroupWeights.Sum(x => x.Value);
            pool.SuffixWeight = pool.SuffixGroupWeights.Sum(x => x.Value);

            pool.TotalWeight = pool.PrefixWeight + pool.SuffixWeight;

            pool.AffixCount = pool.PrefixGroupToAffixWeights.Sum(x => x.Value.Count) + pool.SuffixGroupToAffixWeights.Sum(x => x.Value.Count);

            return pool;
        }

        private Dictionary<string, List<AffixWeight>> GroupToAffixWeights(List<Affix> affixesToEvaluate, HashSet<string> tags, IReadOnlyDictionary<string, double> currencyWeightModifiers, HashSet<string> masterModSet)
        {
            return affixesToEvaluate
                .GroupBy(x => x.Group)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => new AffixWeight(y.FullName, GetAffixSpawnWeight(y, tags, currencyWeightModifiers, masterModSet), y.GenerationType))
                        .Where(y => Math.Abs(y.Weight) > .001)
                        .ToList())
                .Where(x => x.Value.Any())
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private double GetAffixSpawnWeight(Affix affix, HashSet<string> tags, IReadOnlyDictionary<string, double> currencyWeightModifiers, HashSet<string> masterMods)
        {
            if (masterMods.Contains(_noAttackMods) && affix.Tags.Contains(_attackTag)) return 0;
            if (masterMods.Contains(_noCasterMods) && affix.Tags.Contains(_casterTag)) return 0;

            foreach (var spawnWeight in affix.SpawnWeights)
            {
                if (tags.Contains(spawnWeight.Key))
                {
                    double weight = spawnWeight.Value;

                    weight *= affix.GenerationWeights.Where(x => tags.Contains(x.Key)).Aggregate(1d, (y, z) => y * z.Value / 100d);

                    if (currencyWeightModifiers.ContainsKey(spawnWeight.Key))
                    {
                        weight *= currencyWeightModifiers[spawnWeight.Key];
                    }

                    return weight;
                }
            }

            return 0;
        }
    }

    public class AffixWeight
    {
        public AffixWeight(string modName, double weight, string generationType)
        {
            this.ModName = modName;
            this.Weight = weight;
            this.GenerationType = generationType;
        }

        public string ModName { get; set; }
        public double Weight { get; set; }
        public string GenerationType { get; set; }
    }

    public class AffixPool
    {
        public Dictionary<string, List<AffixWeight>> PrefixGroupToAffixWeights { get; set; }
        public Dictionary<string, double> PrefixGroupWeights { get; set; }
        public Dictionary<string, List<AffixWeight>> SuffixGroupToAffixWeights { get; set; }
        public Dictionary<string, double> SuffixGroupWeights { get; set; }
        public double PrefixWeight { get; set; }
        public double SuffixWeight { get; set; }
        public double TotalWeight { get; set; }
        public int AffixCount { get; set; }
    }

    // The hashcode is deliberately order invariant for both tags and fossils.
    public class PoolKey
    {
        public EquipmentModifiers EquipmentModifiers { get; }

        public CurrencyModifiers CurrencyModifiers { get; }

        protected bool Equals(PoolKey other)
        {
            return EquipmentModifiers.Equals(other.EquipmentModifiers) && CurrencyModifiers.Equals(other.CurrencyModifiers);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((PoolKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((EquipmentModifiers != null ? EquipmentModifiers.GetHashCode() : 0) * 397) ^ 
                       (CurrencyModifiers != null ? CurrencyModifiers.GetHashCode() : 0);
            }
        }

        public static bool operator ==(PoolKey left, PoolKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(PoolKey left, PoolKey right)
        {
            return !Equals(left, right);
        }

        public PoolKey(EquipmentModifiers equipmentModifiers, CurrencyModifiers currencyModifiers)
        {
            this.EquipmentModifiers = equipmentModifiers;
            this.CurrencyModifiers = currencyModifiers;
        }
    }
}
