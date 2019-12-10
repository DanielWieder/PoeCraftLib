using System;

namespace PoeCrafting.Entities.Crafting
{
    public class CraftingConfig
    {
        public int CraftingBudget { get; set; }
        public int ItemCost { get; set; }
        public string League { get; set; }

        public override bool Equals(Object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            var baseInfo = (CraftingConfig)obj;
            return CraftingBudget == baseInfo.CraftingBudget && ItemCost == baseInfo.ItemCost && League == baseInfo.League;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 1;
                hashCode = (hashCode * 397) ^ CraftingBudget;
                hashCode = (hashCode * 397) ^ ItemCost;
                hashCode = (hashCode * 397) ^ League.GetHashCode();
                return hashCode;
            }
        }
    }
}
