using PoeCraftLib.Entities.Crafting;

namespace PoeCraftLib.Entities.Items
{
    public class CraftingTarget
    {
        public int Value { get; set; }
        public string Name { get; set; }
        public CraftingCondition Condition { get; set; }

    }
}
