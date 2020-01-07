﻿using System.Collections.Generic;
using Newtonsoft.Json;
using PoeCrafting.Entities;

namespace DataJson.Entities
{
    public partial class ItemClassJson
    {
        public string FullName { get; set; }

        public Dictionary<Influence, string> InfluenceTags { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
