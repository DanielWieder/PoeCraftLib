using Newtonsoft.Json;

namespace DataJson.Entities
{
    public partial class ItemClassJson
    {
        public string FullName { get; set; }

        [JsonProperty("elder_tag")]
        public string ElderTag { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("shaper_tag")]
        public string ShaperTag { get; set; }
    }
}
