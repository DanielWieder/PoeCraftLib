using System.Collections.Generic;
using Newtonsoft.Json;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Entities
{
    public partial class ItemClassJson
    {
        public string FullName { get; set; }

        public Dictionary<Influence, string> InfluenceTags { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
