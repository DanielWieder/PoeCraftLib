using System;
using System.Collections.Generic;

namespace PoeCraftLib.Simulator.Model.Crafting.Steps
{
    public class CurrencyCraftingStep : ICraftingStep
    {
        public String Name { get; set; }

        public List<String> SocketedCurrency { get; set; }

        public List<ICraftingStep> Children { get; } = null;
        public CraftingCondition Condition { get; } = null;
    }
}
