using PoeCrafting.Entities;

namespace PoeCrafting.Domain.Entities
{
    public class SimBaseItemInfo
    {
        public string ItemName { get; set; }
        public int ItemLevel { get; set; } = 84;
        public Faction Faction { get; set; } = Faction.None;
        public double ItemCost { get; set; } = 0;
    }
}
