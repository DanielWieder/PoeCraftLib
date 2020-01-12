using System.Collections.Generic;
using System.Linq;

namespace PoeCraftLib.Simulator.Model.Items
{
    public class Equipment
    {
        public bool Completed { get; set; } = false;

        public ItemBase ItemBase { get; set; }

        public List<Stat> Stats { get; set; } = new List<Stat>();
        public List<Stat> Prefixes => Stats.Where(x => x.Affix.GenerationType == "prefix").ToList();
        public List<Stat> Suffixes => Stats.Where(x => x.Affix.GenerationType == "suffix").ToList();

        public Entities.Items.Stat Implicit { get; set; } = null;

        public int ItemLevel { get; set; } = 84;

        public EquipmentRarity Rarity { get; set; } = EquipmentRarity.Normal;
        public bool Corrupted { get; set; } = false;

        public List<Influence> Influence { get; set; }
    }
}
