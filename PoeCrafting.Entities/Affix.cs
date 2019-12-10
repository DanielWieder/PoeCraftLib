using System.Collections.Generic;
using Newtonsoft.Json;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.Entities
{
    public enum Faction
    {
        Elder,
        Shaper,
        None
    }

    public class Affix
    {
        /// <summary>
        /// The name of the affix  This is what get added to the name of magic items.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the mod. This identifier is present in all mods and unique to each mod.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Where the goes. (Prefix, Suffix, Corruption)
        /// </summary>
        public string GenerationType { get; set; }

        /// <summary>
        /// Mods of the same group do similar things and are exclusive among each other
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// Mods of the same type do the same things at different strengths and are exclusive among each other. The type is a subset of the group.
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// The strength of the mod compared to others of the same type
        /// </summary>
        public int Tier { get; set; }

        /// <summary>
        /// The type of affix tier
        /// </summary>
        public TierType TierType { get; set; }

        /// <summary>
        /// The minimum item level for the affix to spawn
        /// </summary>
        public int RequiredLevel { get; set; }

        /// <summary>
        /// The faction of the item (Elder/shaper)
        /// </summary>
        public Faction Faction { get; set; }

        /// <summary>
        /// The frequency that the affix occurs
        /// </summary>
        [JsonIgnore]
        public int Weight { get; set; }

        public Dictionary<string, int> SpawnWeights { get; set; }

        public Dictionary<string, int> GenerationWeights { get; set; }


        // Adds these tags to the item
        public List<string> AddsTags { get; set; }

        // The tags associated with the mod
        public HashSet<string> Tags { get; set; }

        public string StatName1 { get; set; }
        public int StatMin1 { get; set; }
        public int StatMax1 { get; set; }

        public string StatName2 { get; set; }
        public int StatMin2 { get; set; }
        public int StatMax2 { get; set; }

        public string StatName3 { get; set; }
        public int StatMin3 { get; set; }
        public int StatMax3 { get; set; }

        [JsonIgnore]
        public List<int> MaxStats
        {
            get
            {
                var list = new List<int>();
                if (!string.IsNullOrEmpty(StatName1))
                {
                    list.Add(StatMax1);
                }
                if (!string.IsNullOrEmpty(StatName2))
                {
                    list.Add(StatMax2);
                }
                if (!string.IsNullOrEmpty(StatName3))
                {
                    list.Add(StatMax3);
                }
                return list;
            }
        }
    }
}
