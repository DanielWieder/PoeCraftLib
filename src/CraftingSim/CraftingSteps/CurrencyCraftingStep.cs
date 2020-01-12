using System.Collections.Generic;
using Newtonsoft.Json;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting.CraftingSteps
{
    public class CurrencyCraftingStep : ICraftingStep
    {
        private readonly ICurrency _currency;

        public string Name => _currency.Name;

        [JsonIgnore]
        public List<ICraftingStep> Children => null;

        [JsonIgnore]
        public CraftingCondition Condition => null;

        public double Value { get; }

        public CurrencyCraftingStep(ICurrency currency)
        {
            this._currency = currency;
        }

        public Dictionary<string, int> Craft(Equipment equipment, AffixManager affixManager)
        {
            return _currency.Execute(equipment, affixManager);
        }

        public bool ShouldVisitChildren(Equipment equipment, int times) => false;
        public void UpdateStatus(Entities.ItemStatus metadataCurrentStatus)
        { 
            _currency.GetNextStatus(metadataCurrentStatus);
        }

        public bool ShouldVisitChildren(Entities.ItemStatus previousStatus, Entities.ItemStatus metadataCurrentStatus)
        {
            return false;
        }
    }
}
