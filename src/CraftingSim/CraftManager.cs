using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PoeCraftLib.Entities;
using PoeCraftLib.CraftingSim.CraftingSteps;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CraftingSim
{
    public class CraftManager
    {
        public CraftMetadata Craft(
            List<ICraftingStep> craftingSteps, 
            Equipment equipment,
            AffixManager affixManager,
            CancellationToken ct,
            double? currencyLimit)
        {
            CraftMetadata metadata = new CraftMetadata();
            metadata.Result = equipment;
            return this.Craft(craftingSteps, equipment, affixManager, ct, metadata, currencyLimit);
        }

        public CraftMetadata Craft(
            List<ICraftingStep> craftingSteps,
            Equipment equipment,
            AffixManager affixManager,
            CancellationToken ct,
            CraftMetadata metadata,
            double? currencyLimit)
        {
            foreach (var craftingStep in craftingSteps)
            {
                if (equipment.Completed)
                {
                    return metadata;
                }

                if (currencyLimit.HasValue && metadata.SpentCurrency > currencyLimit)
                {
                    return metadata;
                }

                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }

                bool isModified = craftingStep.Craft(equipment, affixManager);
                UpdateMetadataOnCraft(craftingStep, metadata, isModified);

                var times = 0;
                while (craftingStep.ShouldVisitChildren(equipment, times))
                {
                    metadata = Craft(craftingStep.Children, equipment, affixManager, ct, metadata, currencyLimit);
                    UpdateMetadataOnChildrenVisit(craftingStep, metadata);
                    times++;
                }
            }

            return metadata;
        }

        private CraftMetadata UpdateMetadataOnCraft(ICraftingStep craftingStep, CraftMetadata craftMetadata, bool isModified)
        {
            if (!craftMetadata.CraftingStepMetadata.ContainsKey(craftingStep))
            {
                CraftingStepMetadata stepMetadata = new CraftingStepMetadata {TimesVisited = 0, CurrencyAmounts = craftingStep.GetCurrency};
                craftMetadata.CraftingStepMetadata.Add(craftingStep, stepMetadata);
            }

            craftMetadata.CraftingStepMetadata[craftingStep].TimesVisited++;

            if (isModified)
            {
                craftMetadata.CraftingStepMetadata[craftingStep].TimesModified++;
            }


            return craftMetadata;
        }

        private void UpdateMetadataOnChildrenVisit(ICraftingStep craftingStep, CraftMetadata craftMetadata)
        {
            craftMetadata.CraftingStepMetadata[craftingStep].TimesChildrenVisited++;
        }
    }

    public class CraftMetadata
    {
        public double SpentCurrency = 0;

        public Equipment Result = new Equipment();

        public Dictionary<ICraftingStep, CraftingStepMetadata> CraftingStepMetadata = new Dictionary<ICraftingStep, CraftingStepMetadata>();
    }

    public class CraftingStepMetadata
    {
        public int TimesVisited = 0;

        public int TimesModified = 0;

        public int TimesChildrenVisited = 0;
        public Dictionary<string, int> CurrencyAmounts { get; set; }
    }
}