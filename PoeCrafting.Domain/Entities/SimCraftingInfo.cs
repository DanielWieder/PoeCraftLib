using System.Collections.Generic;
using PoeCraftLib.CraftingSim.CraftingSteps;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Domain.Entities
{
    public class SimCraftingInfo
    {
        public List<CraftingTarget> CraftingTargets { get; set; } = new List<CraftingTarget>();

        public List<ICraftingStep> CraftingSteps { get; set; } = new List<ICraftingStep>();
    }
}
