using System.Collections.Generic;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting.CraftingSteps
{
    public class EndCraftingStep : ICraftingStep
    {
        public string Name => "End";

        public List<ICraftingStep> Children => null;

        public bool ShouldVisitChildren(Equipment equipment, int times) => false;

        public CraftingCondition Condition => null;
        public Dictionary<string, int> GetCurrency => new Dictionary<string, int>();

        public Dictionary<string, int> Craft(Equipment equipment, AffixManager affixManager)
        {
            equipment.Completed = true;
            return new Dictionary<string, int>();
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
