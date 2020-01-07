using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoeCraftLib.Data.Entities
{
    public class CraftingBenchJson
    {
        public string FullName { get; set; }

        [JsonProperty("bench_group")]
        public string BenchGroup { get; set; }

        [JsonProperty("bench_tier")]
        public long BenchTier { get; set; }

        [JsonProperty("cost")]
        public Dictionary<string, long> Cost { get; set; }

        [JsonProperty("item_classes")]
        public List<string> ItemClasses { get; set; }

        [JsonProperty("master")]
        public string Master { get; set; }

        [JsonProperty("mod_id")]
        public string ModId { get; set; }
    }
}
