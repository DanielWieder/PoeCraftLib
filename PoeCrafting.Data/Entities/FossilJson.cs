using System.Collections.Generic;
using Newtonsoft.Json;

namespace DataJson.Entities
{
    public class FossilJson
    {
        public string FullName { get; set; }

        [JsonProperty("added_mods")]
        public List<string> AddedMods { get; set; }

        [JsonProperty("allowed_tags")]
        public List<string> AllowedTags { get; set; }

        [JsonProperty("blocked_descriptions")]
        public List<string> BlockedDescriptions { get; set; }

        [JsonProperty("changes_quality")]
        public bool ChangesQuality { get; set; }

        [JsonProperty("corrupted_essence_chance")]
        public long CorruptedEssenceChance { get; set; }

        [JsonProperty("descriptions")]
        public List<string> Descriptions { get; set; }

        [JsonProperty("enchants")]
        public bool Enchants { get; set; }

        [JsonProperty("forbidden_tags")]
        public List<string> ForbiddenTags { get; set; }

        [JsonProperty("forced_mods")]
        public List<string> ForcedMods { get; set; }

        [JsonProperty("mirrors")]
        public bool Mirrors { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("negative_mod_weights")]
        public List<ModWeight> NegativeModWeights { get; set; }

        [JsonProperty("positive_mod_weights")]
        public List<ModWeight> PositiveModWeights { get; set; }

        [JsonProperty("rolls_lucky")]
        public bool RollsLucky { get; set; }

        [JsonProperty("rolls_white_sockets")]
        public bool RollsWhiteSockets { get; set; }

        [JsonProperty("sell_price_mods")]
        public List<string> SellPriceMods { get; set; }
    }

    public partial class ModWeight
    {
        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("weight")]
        public long Weight { get; set; }
    }
}