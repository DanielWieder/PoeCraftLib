using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCraftLib.Crafting;
using PoeCraftLib.Crafting.CraftingSteps;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CraftingTest
{
    [TestClass]
    public class StatusManagerTest
    {
        readonly StatusManager _statusManager = new StatusManager();
        readonly Mock<IRandom> _random = new Mock<IRandom>();

        [TestMethod]
        public void NoCraftingStepsTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);
            
            Assert.IsFalse(metadata.CurrentStatus.Completed);
        }

        [TestMethod]
        public void EndCraftingStepsTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            EndCraftingStep endStep = new EndCraftingStep();
            craftingSteps.Add(endStep);

            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);

            Assert.IsTrue(metadata.CurrentStatus.Completed);
        }

        [TestMethod]
        public void CurrencyCraftingStepRarityChangeTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();

            AddOrb(craftingSteps, new AlchemyOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Rare);

            AddOrb(craftingSteps, new ScouringOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Normal);

            AddOrb(craftingSteps, new TransmutationOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Magic);

            AddOrb(craftingSteps, new RegalOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Rare);

            AddOrb(craftingSteps, new ScouringOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Normal);

            AddOrb(craftingSteps, new ChanceOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Magic | EquipmentRarity.Rare);

            AddOrb(craftingSteps, new RegalOrb(_random.Object));
            EvaluateRarity(craftingSteps, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void CurrencyCraftingStepImproperRarityNormalTest()
        {
            List<ICraftingStep> craftingSteps = GetNormalStatusNoMods();
            StatusMetadata controlMetadata = _statusManager.Evaluate(craftingSteps);

            AddOrb(craftingSteps, new AlterationOrb(_random.Object));
            AddOrb(craftingSteps, new AugmentationOrb(_random.Object));
            AddOrb(craftingSteps, new ChaosOrb(_random.Object));
            AddOrb(craftingSteps, new ExaltedOrb(_random.Object));
            AddOrb(craftingSteps, new RegalOrb(_random.Object));

            StatusMetadata actualMetadata = _statusManager.Evaluate(craftingSteps);

            Assert.IsTrue(controlMetadata.CurrentStatus.Rarity == actualMetadata.CurrentStatus.Rarity);

        }

        [TestMethod]
        public void CurrencyCraftingStepImproperRarityMagicTest()
        {
            List<ICraftingStep> craftingSteps = GetMagicStatusNoMods();
            StatusMetadata controlMetadata = _statusManager.Evaluate(craftingSteps);

            AddOrb(craftingSteps, new AlchemyOrb(_random.Object));
            AddOrb(craftingSteps, new ChanceOrb(_random.Object));
            AddOrb(craftingSteps, new ExaltedOrb(_random.Object));
            AddOrb(craftingSteps, new TransmutationOrb(_random.Object));

            StatusMetadata actualMetadata = _statusManager.Evaluate(craftingSteps);

            Assert.IsTrue(controlMetadata.CurrentStatus.Rarity == actualMetadata.CurrentStatus.Rarity);
        }

        [TestMethod]
        public void CurrencyCraftingStepImproperRarityRareTest()
        {
            List<ICraftingStep> craftingSteps = GetRareStatusNoMods();
            StatusMetadata controlMetadata = _statusManager.Evaluate(craftingSteps);

            AddOrb(craftingSteps, new AlchemyOrb(_random.Object));
            AddOrb(craftingSteps, new AlterationOrb(_random.Object));
            AddOrb(craftingSteps, new ChanceOrb(_random.Object));
            AddOrb(craftingSteps, new RegalOrb(_random.Object));
            AddOrb(craftingSteps, new TransmutationOrb(_random.Object));

            StatusMetadata actualMetadata = _statusManager.Evaluate(craftingSteps);

            Assert.IsTrue(controlMetadata.CurrentStatus.Rarity == actualMetadata.CurrentStatus.Rarity);
        }

        [TestMethod]
        public void IfCraftingStepCombinesStatusOnceTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            IfCraftingStep ifStep = new IfCraftingStep();
            craftingSteps.Add(ifStep);
            AddOrb(ifStep.Children, new RegalOrb(_random.Object));
            AddOrb(ifStep.Children, new TransmutationOrb(_random.Object));

            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);
            Assert.IsTrue(metadata.CurrentStatus.Rarity == (EquipmentRarity.Normal | EquipmentRarity.Magic));
        }

        [TestMethod]
        public void IfCraftingStepDoesNotCombineCompletedStatusTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            IfCraftingStep ifStep = new IfCraftingStep();
            craftingSteps.Add(ifStep);
            AddOrb(ifStep.Children, new AlchemyOrb(_random.Object));
            AddStep(ifStep.Children, new EndCraftingStep());

            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);
            Assert.IsTrue(metadata.CurrentStatus.Rarity == EquipmentRarity.Normal);
        }

        [TestMethod]
        public void WhileCraftingStepRepeatedlyCombinesStatusTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            WhileCraftingStep whileStep = new WhileCraftingStep();
            craftingSteps.Add(whileStep);
            AddOrb(whileStep.Children, new RegalOrb(_random.Object));
            AddOrb(whileStep.Children, new TransmutationOrb(_random.Object));

            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);
            Assert.IsTrue(metadata.CurrentStatus.Rarity == (EquipmentRarity.Normal | EquipmentRarity.Magic | EquipmentRarity.Rare));
        }

        private List<ICraftingStep> GetNormalStatusNoMods()
        {
            return new List<ICraftingStep>();
        }

        private List<ICraftingStep> GetMagicStatusNoMods()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            AddOrb(craftingSteps, new TransmutationOrb(_random.Object));
            AddOrb(craftingSteps, new AnullmentOrb(_random.Object));
            AddOrb(craftingSteps, new AnullmentOrb(_random.Object));
            return craftingSteps;
        }

        private List<ICraftingStep> GetRareStatusNoMods()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            AddOrb(craftingSteps, new TransmutationOrb(_random.Object));
            AddOrb(craftingSteps, new AnullmentOrb(_random.Object));
            AddOrb(craftingSteps, new AnullmentOrb(_random.Object));
            AddOrb(craftingSteps, new RegalOrb(_random.Object));
            AddOrb(craftingSteps, new AnullmentOrb(_random.Object));
            return craftingSteps;
        }

        private void EvaluateRarity(List<ICraftingStep> craftingSteps, EquipmentRarity rarity)
        {
            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);
            Assert.IsTrue(metadata.CurrentStatus.Rarity == rarity);
        }

        private void AddOrb(List<ICraftingStep> craftingSteps, ICurrency currency)
        {
            CurrencyCraftingStep alchemyStep = new CurrencyCraftingStep(currency);
            craftingSteps.Add(alchemyStep);
        }

        private void AddStep(List<ICraftingStep> craftingSteps, ICraftingStep craftingStep)
        {
            craftingSteps.Add(craftingStep);
        }

        [TestMethod]
        public void CurrencyCraftingStepOrbTest()
        {
            AlchemyOrb alchemyOrb = new AlchemyOrb(_random.Object);

            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            CurrencyCraftingStep alchemyStep = new CurrencyCraftingStep(alchemyOrb);
            craftingSteps.Add(alchemyStep);

            StatusMetadata metadata = _statusManager.Evaluate(craftingSteps);

            Assert.IsTrue(metadata.CurrentStatus.Rarity == EquipmentRarity.Rare);
        }
    }
}
