using System.Collections.Generic;
using Newtonsoft.Json;

namespace PoeCraftLib.Data.Entities
{
    public class ModTypeJson
    {
        public string FullName { get; set; }

        [JsonProperty("sell_price_types")]
        public List<string> SellPriceTypes { get; set; }

        [JsonProperty("tags")]
        public List<string> ModTags { get; set; }
    }
}