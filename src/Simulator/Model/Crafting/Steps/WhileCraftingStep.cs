using System.Collections.Generic;

namespace PoeCraftLib.Simulator.Model.Crafting.Steps
{
    public class WhileCraftingStep : ICraftingStep
    {
        public string Name => "While";
        public List<ICraftingStep> Children { get; set; } = new List<ICraftingStep>();
        public CraftingCondition Condition { set; get; } = new CraftingCondition();
    }
}
