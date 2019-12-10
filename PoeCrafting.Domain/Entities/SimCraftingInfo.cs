using System.Collections.Generic;
using PoeCrafting.CraftingSim.CraftingSteps;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Domain.Entities
{
    public class SimCraftingInfo
    {
        public List<CraftingTarget> CraftingTargets { get; set; } = new List<CraftingTarget>();

        public List<ICraftingStep> CraftingSteps { get; set; } = new List<ICraftingStep>();
    }
}
