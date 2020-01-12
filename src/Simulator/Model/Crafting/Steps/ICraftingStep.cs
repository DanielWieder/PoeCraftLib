using System.Collections.Generic;

namespace PoeCraftLib.Simulator.Model.Crafting.Steps
{
    public interface ICraftingStep
    {
        string Name { get; }

        List<ICraftingStep> Children { get; }
        CraftingCondition Condition { get; }
    }
}
