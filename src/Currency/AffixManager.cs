using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly int _baseLevel;

        private readonly Dictionary<PoolKey, AffixPool> _poolDic = new Dictionary<PoolKey, AffixPool>();
        private readonly Dictionary<string, Affix> _allAffixes;

        public AffixManager(ItemBase itemBase, List<Affix> itemAffixes, List<Affix> fossilAffixes)
        {
            _allAffixes = itemAffixes.Union(fossilAffixes).GroupBy(x => x.FullName).Select(x => x.First()).ToDictionary(x => x.FullName, x => x);
            _itemAffixes = itemAffixes;
            _baseTags = itemBase.Tags;
            _baseLevel = itemBase.RequiredLevel;
        }

        public Affix GetAffix(List<Affix> affixes, EquipmentRarity rarity, IRandom random, List<Fossil> fossils = null)
        {
            if (fossils == null) fossils = new List<Fossil>();

            var addedTags = affixes.SelectMany(x => x.AddsTags).Distinct().ToList();
            var fossilNames = fossils.Select(x => x.FullName).Distinct().ToList();

            var masterMods = affixes.Where(x => x.Group.Contains(_noAttackMods) || x.Group.Contains(_noCasterMods)).Select(x => x.Group).ToList();

            PoolKey key = new PoolKey(addedTags, fossilNames, masterMods);

            if (!_poolDic.ContainsKey(key))
            {
                _poolDic.Add(key, GenerateAffixPool(addedTags, fossils, masterMods));
            }

            AffixPool pool = _poolDic[key];
            var existingGroups = new HashSet<string>(affixes.Select(x => x.Group));

            int affixesCount = rarity == EquipmentRarity.Normal ? 0 :
                rarity == EquipmentRarity.Magic ? 1 :
                rarity == EquipmentRarity.Rare ? 3 : 0;

            double prefixSkipAmount = 0;
            double suffixSkipAmount = 0;

            if (affixes.Count(x => x.GenerationType == "prefix") >= affixesCount)
            {
                prefixSkipAmount = pool.PrefixWeight;
            }
            else
            {
                prefixSkipAmount = affixes
                    .Select(x => x.Group)
                    .Where(x => pool.PrefixGroupWeights.ContainsKey(x))
                    .Sum(x => pool.PrefixGroupWeights[x]);
            }

            if (affixes.Count(x => x.GenerationType == "suffix") >= affixesCount)
            {
                suffixSkipAmount= pool.SuffixWeight;
            }
            else
            {
                suffixSkipAmount = affixes
                    .Select(x => x.Group)
                    .Where(x => pool.SuffixGroupWeights.ContainsKey(x))
                    .Sum(x => pool.SuffixGroupWeights[x]);
            }

            var totalWeight = pool.TotalWeight;

            totalWeight -= prefixSkipAmount;
            totalWeight -= suffixSkipAmount;

            var targetWeight = random.NextDouble() * totalWeight;

            return GetRandomAffixFromPool(pool, targetWeight, existingGroups, prefixSkipAmount, suffixSkipAmount);
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

        private Affix RandomAffixFromPool(double targetWeight, HashSet<string> existingGroups, Dictionary<string, double> groupWeights,
            double currentWeight, Dictionary<string, List<AffixWeight>> groupToAffixWeights)
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

        private AffixPool GenerateAffixPool(List<string> addedTags, List<Fossil> fossils, List<string> masterMods)
        {
            var tags = new HashSet<string>(_baseTags.Union(addedTags));
            var masterModSet = new HashSet<string>(masterMods);
            var fossilAffixes = fossils
                .SelectMany(x => x.AddedAffixes)
                .Distinct()
                .Where(x => x.RequiredLevel <= _baseLevel)
                .ToList();

            var affixesToEvaluate = _itemAffixes.Union(fossilAffixes).ToList();

            var fossilWeightModifiers = fossils
                .SelectMany(x => x.ModWeightModifiers)
                .GroupBy(x => x.Key)
                .ToDictionary(x => x.Key, x => x.Aggregate(1d, (y, z) => y * z.Value / 100d));

            AffixPool pool = new AffixPool();

            pool.PrefixGroupToAffixWeights = GroupToAffixWeights(affixesToEvaluate.Where(x => x.GenerationType == "prefix").ToList(), tags, fossilWeightModifiers, masterModSet);
            pool.SuffixGroupToAffixWeights = GroupToAffixWeights(affixesToEvaluate.Where(x => x.GenerationType == "suffix").ToList(), tags, fossilWeightModifiers, masterModSet);

            pool.PrefixGroupWeights = pool.PrefixGroupToAffixWeights.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.Weight));
            pool.SuffixGroupWeights = pool.SuffixGroupToAffixWeights.ToDictionary(x => x.Key, x => x.Value.Sum(y => y.Weight));

            pool.PrefixWeight = pool.PrefixGroupWeights.Sum(x => x.Value);
            pool.SuffixWeight = pool.SuffixGroupWeights.Sum(x => x.Value);

            pool.TotalWeight = pool.PrefixWeight + pool.SuffixWeight;

            pool.AffixCount = pool.PrefixGroupToAffixWeights.Sum(x => x.Value.Count) + pool.SuffixGroupToAffixWeights.Sum(x => x.Value.Count);

            return pool;
        }

        private Dictionary<string, List<AffixWeight>> GroupToAffixWeights(List<Affix> affixesToEvaluate, HashSet<string> tags, Dictionary<string, double> fossilWeightModifiers, HashSet<string> masterModSet)
        {
            return affixesToEvaluate
                .GroupBy(x => x.Group)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => new AffixWeight(y.FullName, GetAffixSpawnWeight(y, tags, fossilWeightModifiers, masterModSet), y.GenerationType))
                        .Where(y => Math.Abs(y.Weight) > .001)
                        .ToList())
                .Where(x => x.Value.Any())
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private double GetAffixSpawnWeight(Affix affix, HashSet<string> tags, Dictionary<string, double> fossilWeightModifiers, HashSet<string> masterMods)
        {
            if (masterMods.Contains(_noAttackMods) && affix.Tags.Contains(_attackTag)) return 0;
            if (masterMods.Contains(_noCasterMods) && affix.Tags.Contains(_casterTag)) return 0;

            foreach (var spawnWeight in affix.SpawnWeights)
            {
                if (tags.Contains(spawnWeight.Key))
                {
                    double weight = spawnWeight.Value;

                    weight *= affix.GenerationWeights.Where(x => tags.Contains(x.Key)).Aggregate(1d, (y, z) => y * z.Value / 100d);

                    if (fossilWeightModifiers.ContainsKey(spawnWeight.Key))
                    {
                        weight *= fossilWeightModifiers[spawnWeight.Key];
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
        public HashSet<string> Tags { get; }

        public HashSet<string> Fossils { get; }

        public HashSet<string> MasterMods { get; }

        protected bool Equals(PoolKey other)
        {
            return Tags.All(x => other.Tags.Contains(x)) && Fossils.All(x => other.Fossils.Contains(x)) && MasterMods.All(x => other.MasterMods.Contains(x));
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
            const int seed = 0x2D2816FE;
            const int prime = 397;

            unchecked
            {
                int tagsHash = Tags != null ? Tags.Aggregate(seed, (x, y) => x ^ y.GetHashCode()) : 0;
                int fossilsHash = Fossils != null ? Fossils.Aggregate(seed, (x, y) => x ^ y.GetHashCode()) : 0;
                int masterModHash = MasterMods != null ? MasterMods.Aggregate(seed, (x, y) => x ^ y.GetHashCode()) : 0;
                return tagsHash * prime + fossilsHash * prime + masterModHash;
            }
        }

        public PoolKey(List<string> tags, List<string> fossils, List<string> masterMods)
        {
            this.Tags = new HashSet<string>(tags);
            this.Fossils = new HashSet<string>(fossils);
            this.MasterMods = new HashSet<string>(masterMods);
        }
    }
}
