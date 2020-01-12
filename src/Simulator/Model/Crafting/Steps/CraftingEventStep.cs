using System;
using System.Collections.Generic;
using PoeCraftLib.Simulator.Model.Crafting.Currency;

namespace PoeCraftLib.Simulator.Model.Crafting.Steps
{
    public class CraftingEventStep : ICraftingStep
    {
        public CraftingEvent CraftingEvent { get; set; }

        public String Name => CraftingEvent.Name;

        public List<ICraftingStep> Children { get; } = null;
        public CraftingCondition Condition { get; } = null;
    }
}
