using System.Collections.Generic;

namespace PoeCraftLib.Simulator.Model.Crafting.Steps
{
    public class EndCraftingStep : ICraftingStep
    {
        public string Name => "End";

        public List<ICraftingStep> Children { get; } = null;
        public CraftingCondition Condition { get; } = null;
    }
}
