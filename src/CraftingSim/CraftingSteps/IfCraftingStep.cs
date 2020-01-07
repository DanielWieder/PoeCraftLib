﻿using System.Collections.Generic;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting.CraftingSteps
{
    public class IfCraftingStep : ICraftingStep
    {
        private readonly ConditionResolver _conditionResolution = new ConditionResolver();

        public string Name => "If";

        public Dictionary<string, int> GetCurrency => new Dictionary<string, int>();
        public List<ICraftingStep> Children { get; } = new List<ICraftingStep>();
        public CraftingCondition Condition { get; set; } = new CraftingCondition();
        public bool Craft(Equipment equipment, AffixManager affixManager) => false;

        public bool ShouldVisitChildren(Equipment equipment, int times)
        {
            return times == 0 && Children.Count > 0 && _conditionResolution.IsValid(Condition, equipment);
        }

        public void UpdateStatus(ItemStatus metadataCurrentStatus)
        {
        }

        public bool ShouldVisitChildren(ItemStatus previousStatus, ItemStatus metadataCurrentStatus)
        {
            return previousStatus == null;
        }
    }
}
