using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoeCraftLib.Data.Entities
{
    public partial class ModsJson
    {
        public string FullName { get; set; }

        [JsonProperty("adds_tags")]
        public List<string> AddsTags { get; set; }

        [JsonProperty("domain")]
        public string Domain { get; set; }

        [JsonProperty("generation_type")]
        public string GenerationType { get; set; }

        [JsonProperty("generation_weights")]
        public List<NWeight> GenerationWeights { get; set; }

        [JsonProperty("grants_buff")]
        public ModGrantsBuff GrantsBuff { get; set; }

        [JsonProperty("grants_effects")]
        public List<GrantsEffect> GrantsEffects { get; set; }

        [JsonProperty("group")]
        public string Group { get; set; }

        [JsonProperty("is_essence_only")]
        public bool IsEssenceOnly { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("required_level")]
        public long RequiredLevel { get; set; }

        [JsonProperty("spawn_weights")]
        public List<NWeight> SpawnWeights { get; set; }

        [JsonProperty("stats")]
        public List<Stat> Stats { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }
    }

    public partial class NWeight
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }
    }

    public partial class ModGrantsBuff
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("range", NullValueHandling = NullValueHandling.Ignore)]
        public long? Range { get; set; }
    }

    public partial class GrantsEffect
    {
        [JsonProperty("granted_effect_id")]
        public string GrantedEffectId { get; set; }

        [JsonProperty("level")]
        public long Level { get; set; }
    }

    public partial class Stat
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("max")]
        public long Max { get; set; }

        [JsonProperty("min")]
        public long Min { get; set; }
    }
}
