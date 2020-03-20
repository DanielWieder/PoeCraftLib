using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace PoeCraftLib.Entities.Items
{
    public class Equipment
    {
        [JsonIgnore]
        public bool Completed { get; set; } = false;

        [JsonIgnore]
        public ItemBase ItemBase { get; set; }

        [JsonIgnore]
        public List<Stat> Stats { get; set; } = new List<Stat>();
        public List<Stat> Prefixes => Stats.Where(x => x.Affix.GenerationType == "prefix").ToList();
        public List<Stat> Suffixes => Stats.Where(x => x.Affix.GenerationType == "suffix").ToList();

        public List<Stat> Implicits { get; set; } = null;

        public int ItemLevel { get; set; } = 84;

        public EquipmentRarity Rarity { get; set; } = EquipmentRarity.Normal;
        public bool Corrupted { get; set; } = false;

        public List<Influence> Influence { get; set; }
        public int Quality { get; set; }
        public QualityType QualityType { get; set; }
        public bool Destroyed { get; set; }
    }
}
