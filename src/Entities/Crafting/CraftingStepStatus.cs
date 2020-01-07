using System;

namespace PoeCraftLib.Entities.Crafting
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
