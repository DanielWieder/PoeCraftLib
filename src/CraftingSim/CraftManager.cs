using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PoeCraftLib.Crafting.CraftingSteps;
using PoeCraftLib.Currency;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Crafting
{
    public class CraftManager
    {
        public CraftMetadata Craft(
            List<ICraftingStep> craftingSteps, 
            Equipment equipment,
            AffixManager affixManager,
            CancellationToken ct,
            ProgressManager progressManager)
        {
            CraftMetadata metadata = new CraftMetadata();
            metadata.Result = equipment;
            return this.Craft(craftingSteps, equipment, affixManager, ct, metadata, progressManager);
        }

        public CraftMetadata Craft(
            List<ICraftingStep> craftingSteps,
            Equipment equipment,
            AffixManager affixManager,
            CancellationToken ct,
            CraftMetadata metadata,
            ProgressManager progressManager)
        {
            foreach (var craftingStep in craftingSteps)
            {
                if (equipment.Completed)
                {
                    return metadata;
                }

                if (progressManager.Progress >= 100)
                {
                    return metadata;
                }

                if (ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }

                var currency = craftingStep.Craft(equipment, affixManager);
                UpdateMetadataOnCraft(craftingStep, metadata, currency, progressManager);

                var times = 0;
                double previousProgress = 0;
                while (craftingStep.ShouldVisitChildren(equipment, times))
                {
                    metadata = Craft(craftingStep.Children, equipment, affixManager, ct, metadata, progressManager);
                    UpdateMetadataOnChildrenVisit(craftingStep, metadata);
                    times++;

                    if (times > 1)
                    {
                        // Check for no crafting steps
                        if (Math.Abs(previousProgress - progressManager.Progress) < double.Epsilon)
                        {
                            throw new ArgumentException("Crafting steps do not spend currency");
                        }

                        previousProgress = progressManager.Progress;
                    }
                }
            }

            return metadata;
        }

        private CraftMetadata UpdateMetadataOnCraft(
            ICraftingStep craftingStep, 
            CraftMetadata craftMetadata, 
            Dictionary<string, int> currencyAmounts,
            ProgressManager progressManager)
        {
            if (!craftMetadata.CraftingStepMetadata.ContainsKey(craftingStep))
            {
                CraftingStepMetadata stepMetadata = new CraftingStepMetadata {TimesVisited = 0, CurrencyAmounts = currencyAmounts };
                craftMetadata.CraftingStepMetadata.Add(craftingStep, stepMetadata);
            }

            craftMetadata.CraftingStepMetadata[craftingStep].TimesVisited++;

            if (currencyAmounts.Any())
            {
                craftMetadata.CraftingStepMetadata[craftingStep].TimesModified++;
            }

            foreach (var currency in currencyAmounts)
            {
                progressManager.SpendCurrency(currency.Key, currency.Value);
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