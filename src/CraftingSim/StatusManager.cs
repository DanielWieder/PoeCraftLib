using System.Collections.Generic;
using PoeCraftLib.Crafting.CraftingSteps;
using PoeCraftLib.Entities;

namespace PoeCraftLib.Crafting
{
    public class StatusManager
    {
        public StatusMetadata Evaluate(
            List<ICraftingStep> craftingSteps)
        {
            StatusMetadata metadata = new StatusMetadata();
            return this.Evaluate(craftingSteps, metadata);
        }

        private StatusMetadata Evaluate(
            List<ICraftingStep> craftingSteps,
            StatusMetadata metadata)
        {
            foreach (var craftingStep in craftingSteps)
            {
                if (metadata.CurrentStatus.Completed) return metadata;

                craftingStep.UpdateStatus(metadata.CurrentStatus);

                ItemStatus previousStatus = null;

                while (craftingStep.ShouldVisitChildren(previousStatus, metadata.CurrentStatus))
                {
                    previousStatus = (ItemStatus)metadata.CurrentStatus.Clone();
                    Evaluate(craftingStep.Children, metadata);
                    metadata.CurrentStatus = ItemStatus.Combine(new List<ItemStatus> { previousStatus, metadata.CurrentStatus });
                }

                metadata = UpdateMetadata(craftingStep, metadata);
            }

            return metadata;
        }

        private StatusMetadata UpdateMetadata(ICraftingStep craftingStep, StatusMetadata statusMetadata)
        {
            if (!statusMetadata.CraftingStepMetadata.ContainsKey(craftingStep))
            {
                statusMetadata.CraftingStepMetadata.Add(craftingStep, new CraftingStepStatusMetadata());
            }

            var craftingStepMetadata = statusMetadata.CraftingStepMetadata[craftingStep];

            craftingStepMetadata.Status = (ItemStatus)statusMetadata.CurrentStatus.Clone();
            craftingStepMetadata.TimesEvaluated++;

            return statusMetadata;
        }

    }

    public class StatusMetadata
    {
        public ItemStatus CurrentStatus = new ItemStatus();

        public Dictionary<ICraftingStep, CraftingStepStatusMetadata> CraftingStepMetadata = new Dictionary<ICraftingStep, CraftingStepStatusMetadata>();
    }

    public class CraftingStepStatusMetadata
    {
        public ItemStatus Status = null;

        public int TimesEvaluated = 0;
    }
}