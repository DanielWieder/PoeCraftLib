using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataJson.Entities
{
    public class EssenceJson
    {
        public string FullName { get; set; }

        [JsonProperty("item_level_restriction")]
        public long? ItemLevelRestriction { get; set; }

        [JsonProperty("level")]
        public long? Level { get; set; }

        [JsonProperty("mods")]
        public Dictionary<string, string> Mods { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("spawn_level_max")]
        public long? SpawnLevelMax { get; set; }

        [JsonProperty("spawn_level_min")]
        public long? SpawnLevelMin { get; set; }

        [JsonProperty("type")]
        public TypeClass Type { get; set; }
    }

    public partial class TypeClass
    {
        public bool? IsCorruptionOnly { get; set; }
        public long? Tier { get; set; }
    }
}
