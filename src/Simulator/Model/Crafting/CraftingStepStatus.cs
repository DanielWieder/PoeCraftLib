using System;

namespace PoeCraftLib.Simulator.Model.Crafting
{
    [Flags]
    public enum CraftingStepStatus
    {
        Unreachable,
        Unusable,
        Inconsistent,
        Ok
    }
}
