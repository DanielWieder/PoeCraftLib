using System.Collections.Generic;
using PoeCraftLib.Simulator.Model.Crafting;
using PoeCraftLib.Simulator.Model.Crafting.Steps;

namespace PoeCraftLib.Simulator.Model.Simulation
{
    public class SimCraftingInfo
    {
        public List<CraftingTarget> CraftingTargets { get; set; } = new List<CraftingTarget>();

        public List<ICraftingStep> CraftingSteps { get; set; } = new List<ICraftingStep>();
    }
}
