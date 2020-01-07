using System.Collections.Generic;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting.CraftingSteps
{
    public class WhileCraftingStep : ICraftingStep
    {
        private readonly ConditionResolver _conditionResolution = new ConditionResolver();

        public string Name => "While";

        public Dictionary<string, int> GetCurrency => new Dictionary<string, int>();
        public List<ICraftingStep> Children { get; } = new List<ICraftingStep>();
        public CraftingCondition Condition { set; get; } = new CraftingCondition();
        public bool Craft(Equipment equipment, AffixManager affixManager) => false;

        public bool ShouldVisitChildren(Equipment equipment, int times)
        {
            return Children.Count > 0 && _conditionResolution.IsValid(Condition, equipment);
        }

        public void UpdateStatus(ItemStatus metadataCurrentStatus)
        {
        }

        public bool ShouldVisitChildren(ItemStatus previousStatus, ItemStatus metadataCurrentStatus)
        {
            return previousStatus == null || !previousStatus.AreEqual(metadataCurrentStatus);
        }
    }
}
