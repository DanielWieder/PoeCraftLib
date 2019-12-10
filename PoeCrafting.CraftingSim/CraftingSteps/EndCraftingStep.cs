using System.Collections.Generic;
using PoeCrafting.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Crafting;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CraftingSim.CraftingSteps
{
    public class EndCraftingStep : ICraftingStep
    {
        public string Name => "End";

        public List<ICraftingStep> Children => null;

        public bool ShouldVisitChildren(Equipment equipment, int times) => false;

        public CraftingCondition Condition => null;
        public Dictionary<string, int> GetCurrency => new Dictionary<string, int>();

        public bool Craft(Equipment equipment, AffixManager affixManager)
        {
            equipment.Completed = true;
            return true;
        }
        public void UpdateStatus(ItemStatus metadataCurrentStatus)
        {
            metadataCurrentStatus.Completed = true;
        }

        public bool ShouldVisitChildren(ItemStatus previousStatus, ItemStatus metadataCurrentStatus)
        {
            return false;
        }
    }
}
