using System.Collections.Generic;

namespace PoeCraftLib.Simulator.Model.Crafting.Steps
{
    public class IfCraftingStep : ICraftingStep
    {
        public string Name => "If";
        public List<ICraftingStep> Children { get; set; } = new List<ICraftingStep>();
        public CraftingCondition Condition { set; get; } = new CraftingCondition();
    }
}
