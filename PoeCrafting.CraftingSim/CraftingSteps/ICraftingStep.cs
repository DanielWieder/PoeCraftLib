using System.Collections.Generic;
using PoeCrafting.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Crafting;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CraftingSim.CraftingSteps
{
    public interface ICraftingStep
    {
        string Name { get; }

        List<ICraftingStep> Children { get; }
        CraftingCondition Condition { get; }
        Dictionary<string, int> GetCurrency { get; }

        bool Craft(Equipment equipment, AffixManager affixManager);

        bool ShouldVisitChildren(Equipment equipment, int times);
        void UpdateStatus(ItemStatus metadataCurrentStatus);
        bool ShouldVisitChildren(ItemStatus previousStatus, ItemStatus metadataCurrentStatus);
    }
}
