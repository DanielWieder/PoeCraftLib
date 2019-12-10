using System;

namespace PoeCrafting.Entities.Crafting
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
