using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Entities;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Factory
{
    public class AffixFactory
    {
        private readonly IFetchAffixes _fetchAffixes = new FetchAffixes();
        private readonly IFetchItemClass _fetchItemClass = new FetchItemClass();
        private readonly IFetchModType _fetchModType = new FetchModType();

        private readonly Dictionary<string, ModsJson> _mods;
        private readonly Dictionary<string, ItemClassJson> _itemClass;
        private readonly Dictionary<string, HashSet<String>> _modTypeToTags; 


        public AffixFactory()
        {
            _mods = _fetchAffixes.Execute().ToDictionary(x => x.FullName, x => x);
            _itemClass = _fetchItemClass.Execute().ToDictionary( x => x.FullName, x => x);
            _modTypeToTags = _fetchModType.Execute().ToDictionary(x => x.FullName, x => new HashSet<string>(x.ModTags));
        }

        public List<Affix> GetAffixesForItem(List<string> itemTags, String itemClass, int itemLevel = 84)
        {
            HashSet<string> itemTagSet = new HashSet<string>(itemTags);

            var itemDomain = itemClass == "AbyssJewel" ? "abyss_jewel" : "item";

            var initialFilter = _mods
                .Where(x => x.Value.Domain == itemDomain)
                .Where(x => !x.Value.IsEssenceOnly)
                .Select(x => x.Value)
                .ToList();

            List<ModsJson> remainingAffixes = new List<ModsJson>();

            List<string> currentAddedTags = new List<string>();
            List<ModsJson> currentAffixes = new List<ModsJson>();

            List<ModsJson> addedAffixes = new List<ModsJson>();

            addedAffixes = initialFilter.Where(x => ItemCanHaveAffix(x, itemTagSet)).ToList();

            while (addedAffixes.Any())
            {
                currentAffixes.AddRange(addedAffixes);

                // Pull any new tags from the newly added affixes
                var newTags = addedAffixes
                    .SelectMany(x => x.AddsTags)
                    .Distinct()
                    .Except(currentAddedTags)
                    .ToList();

                currentAddedTags.AddRange(newTags);

                // Update remaining affixes
                remainingAffixes = remainingAffixes.Except(addedAffixes).ToList();

                // Mark affixes to be added if they have one of the new tags
                addedAffixes = remainingAffixes.Where(x => ItemCanHaveAffix(x, new HashSet<string>(newTags))).ToList();
            }

            var modsFilteredByBaseItem = currentAffixes;

            var modTiers = GetModTiers(modsFilteredByBaseItem.Select(x => x).ToList());

            var modsFilteredByLevel = modsFilteredByBaseItem
                .Where(x => x.RequiredLevel <= itemLevel)
                .ToList();

            var affixesForItem = modsFilteredByLevel
                .Select(x => ModJsonToAffix(x, itemTagSet, modTiers))
                .ToList();

            return affixesForItem;
        }

        public Dictionary<Influence, List<Affix>> GetAffixesByInfluence(List<Influence> influences, string itemClass,
            int itemLevel)
        {
            if (!_itemClass.ContainsKey(itemClass))
            {
                return influences.ToDictionary(x => x, x => new List<Affix>());
            }

            return influences.Select(x => new { Influence = x, Tag = _itemClass[itemClass].InfluenceTags[x]})
                .ToDictionary(x => x.Influence, x => GetAffixesForItem(new List<string>() {x.Tag}, itemClass, itemLevel));
        }

        public Dictionary<Influence, string> GetInfluenceSpawnTags(string itemClass)
        {
            return _itemClass[itemClass].InfluenceTags;
        }

        private string GetFactionTags(Influence influence, string itemClass)
        {
            return _itemClass[itemClass].InfluenceTags[influence];
        }

        private Dictionary<string, int> GetModTiers(List<ModsJson> itemMods)
        {
            var modsByType = itemMods.GroupBy(x => x.Type);

            Dictionary<string, int> modTiers = new Dictionary<string, int>();

            foreach (var affixType in modsByType)
            {
                var orderedTierList = affixType.ToList()
                    .OrderByDescending(x => x.Stats.Count > 0 ? x.Stats[0].Max : 0)
                    .ThenBy(x => x.Stats.Count > 0 ? x.Stats[0].Min : 0)
                    .ThenBy(x => x.Stats.Count > 1 ? x.Stats[1].Max : 0)
                    .ThenBy(x => x.Stats.Count > 1 ? x.Stats[1].Min : 0)
                    .ThenBy(x => x.Stats.Count > 2 ? x.Stats[2].Max : 0)
                    .ThenBy(x => x.Stats.Count > 2 ? x.Stats[2].Min : 0)
                    .ToList();

                for (int i = 0; i < orderedTierList.Count(); i++)
                {
                    modTiers.Add(orderedTierList[i].FullName, i + 1);
                }
            }
            return modTiers;
        }

        private Affix ModJsonToAffix(
            ModsJson modsJson,
            int modTier,
            TierType tierType)
        {
            Affix affix = new Affix();
            affix.GenerationType = modsJson.GenerationType;
            affix.Group = modsJson.Group;
            affix.Name = modsJson.Name;
            affix.FullName = modsJson.FullName;
            affix.RequiredLevel = (int)modsJson.RequiredLevel;
            affix.Type = modsJson.Type;
            affix.Tier = modTier;
            affix.TierType = tierType;
            affix.Tags = _modTypeToTags[modsJson.Type];
            affix.AddsTags = modsJson.AddsTags;
            affix.SpawnWeights = modsJson.SpawnWeights.ToDictionary(x => x.Tag, x => (int)x.Weight);
            affix.GenerationWeights = modsJson.GenerationWeights.ToDictionary(x => x.Tag, x => (int)x.Weight);

            if (modsJson.Stats.Count > 0)
            {
                int sMin = (int)Math.Abs(modsJson.Stats[0].Min);
                int sMax = (int)Math.Abs(modsJson.Stats[0].Max);

                affix.StatMin1 = sMin <= sMax ? sMin : sMax;
                affix.StatMax1 = sMax >= sMin ? sMax : sMin;
                affix.StatName1 = modsJson.Stats[0].Id;
            }
            if (modsJson.Stats.Count > 1)
            {
                int sMin = (int)Math.Abs(modsJson.Stats[1].Min);
                int sMax = (int)Math.Abs(modsJson.Stats[1].Max);

                affix.StatMin2 = sMin <= sMax ? sMin : sMax;
                affix.StatMax2 = sMax >= sMin ? sMax : sMin;
                affix.StatName2 = modsJson.Stats[1].Id;
            }
            if (modsJson.Stats.Count > 2)
            {
                int sMin = (int)Math.Abs(modsJson.Stats[2].Min);
                int sMax = (int)Math.Abs(modsJson.Stats[2].Max);

                affix.StatMin3 = sMin <= sMax ? sMin : sMax;
                affix.StatMax3 = sMax >= sMin ? sMax : sMin;
                affix.StatName3 = modsJson.Stats[2].Id;
            }
            return affix;
        }

        private Affix ModJsonToAffix(
            ModsJson modsJson,
            HashSet<string> itemTags,
            Dictionary<string, int> modTiers)
        {
            return ModJsonToAffix(modsJson,
                modTiers[modsJson.FullName],
                TierType.Default);
        }

        private bool ItemCanHaveAffix(ModsJson modsJsons, HashSet<string> tags)
        {
            return GetAffixSpawnWeight(modsJsons, tags) > 0;
        }

        private double GetAffixSpawnWeight(ModsJson modsJsons, HashSet<string> tags)
        {
            foreach (var spawnWeight in modsJsons.SpawnWeights)
            {
                if (tags.Contains(spawnWeight.Tag))
                {
                    return spawnWeight.Weight;
                }
            }

            return 0;
        }

        public List<Affix> GetFossilAffixes(List<String> addedMods)
        {
            var mods = addedMods.Where(x => _mods.ContainsKey(x)).Select(x => _mods[x]).ToList();

            var modTiers = GetModTiers(mods);

            return mods.Select(x => ModJsonToAffix(x, modTiers[x.FullName], TierType.Essence)).ToList();
        }

        public Dictionary<string, Affix> GetEssenceAffixes(Dictionary<string, string> itemClassToEssenceModName, int essenceLevel)
        {
            int tier = 9 - essenceLevel;

            return itemClassToEssenceModName.ToDictionary(x => x.Key, x => ModJsonToAffix(_mods[x.Value], tier, TierType.Essence));
        }

        public Affix GetMasterAffix(string modName, int tier)
        {
            return ModJsonToAffix(_mods[modName], tier, TierType.Craft);
        }
    }
}
