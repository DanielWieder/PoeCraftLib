using System.Collections.Generic;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting.CraftingSteps
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
