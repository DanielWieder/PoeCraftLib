using PoeCrafting.Entities.Crafting;

namespace PoeCrafting.Entities.Items
{
    public class CraftingTarget
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public CraftingCondition Condition { get; set; }

    }
}
