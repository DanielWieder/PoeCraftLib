using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.CraftingSim;
using PoeCrafting.CraftingSim.CraftingSteps;
using PoeCrafting.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Crafting;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CraftingProcessTest
{
    [TestClass]
    public class CraftManagerTest
    {
        readonly CraftManager craftManager = new CraftManager();

        [TestInitialize]
        public void Initialize()
        {
        }

        [TestMethod]
        public void NoCraftingStepsTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            Equipment equipment = getTestEquipment(true);

            CancellationToken token = new CancellationToken(false);

            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);
            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            Assert.IsFalse(equipment.Completed);
        }

        [TestMethod]
        public void EndCraftingStepTest()
        {
            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            EndCraftingStep endCrafting = new EndCraftingStep();
            craftingSteps.Add(endCrafting);

            Equipment equipment = getTestEquipment(true);
            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);
            CancellationToken token = new CancellationToken(false);

            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            Assert.IsTrue(equipment.Completed);
        }

        [TestMethod]
        public void IfCraftingStepTrueConditionTest()
        {
            Mock<ICraftingStep> mockChildStep = new Mock<ICraftingStep>();

            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            IfCraftingStep ifStep = new IfCraftingStep();
            ifStep.Children.Add(mockChildStep.Object);
            ifStep.Condition = getTestCondition();
            craftingSteps.Add(ifStep);

            EndCraftingStep endCrafting = new EndCraftingStep();
            craftingSteps.Add(endCrafting);

            Equipment equipment = getTestEquipment(true);
            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);

            CancellationToken token = new CancellationToken(false);

            mockChildStep.Setup(x => x.ShouldVisitChildren(It.IsAny<Equipment>(), It.IsAny<int>())).Returns(false);

            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            mockChildStep.Verify(x => x.Craft(It.IsAny<Equipment>(), It.IsAny<AffixManager>()), Times.Once);

            Assert.IsTrue(equipment.Completed);
        }

        [TestMethod]
        public void IfCraftingStepFalseConditionTest()
        {
            Mock<ICraftingStep> mockChildStep = new Mock<ICraftingStep>();

            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            IfCraftingStep ifStep = new IfCraftingStep();
            ifStep.Children.Add(mockChildStep.Object);
            ifStep.Condition = getTestCondition();
            craftingSteps.Add(ifStep);

            EndCraftingStep endCrafting = new EndCraftingStep();
            craftingSteps.Add(endCrafting);

            Equipment equipment = getTestEquipment(false);
            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);

            CancellationToken token = new CancellationToken(false);

            mockChildStep.Setup(x => x.ShouldVisitChildren(It.IsAny<Equipment>(), It.IsAny<int>())).Returns(false);

            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            mockChildStep.Verify(x => x.Craft(It.IsAny<Equipment>(), It.IsAny<AffixManager>()), Times.Never);

            Assert.IsTrue(equipment.Completed);
        }

        [TestMethod]
        public void WhileCraftingStepFourTimesConditionTest()
        {
            Mock<ICraftingStep> mockChildStep = new Mock<ICraftingStep>();

            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            WhileCraftingStep whileStep = new WhileCraftingStep();
            whileStep.Children.Add(mockChildStep.Object);
            whileStep.Condition = getTestCondition();
            craftingSteps.Add(whileStep);

            EndCraftingStep endCrafting = new EndCraftingStep();
            craftingSteps.Add(endCrafting);

            Equipment equipment = getTestEquipment(true);
            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);

            CancellationToken token = new CancellationToken(false);

            int calls = 0;

            mockChildStep.Setup(x => x.Craft(It.IsAny<Equipment>(), It.IsAny<AffixManager>())).Callback(() =>
            {
                if (calls >= 3)
                {
                    updateEquipment(equipment, false);
                }

                calls++;
            });

            mockChildStep.Setup(x => x.ShouldVisitChildren(It.IsAny<Equipment>(), It.IsAny<int>())).Returns(false);

            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            mockChildStep.Verify(x => x.Craft(It.IsAny<Equipment>(), It.IsAny<AffixManager>()), Times.Exactly(4));

            Assert.IsTrue(equipment.Completed);
        }

        [TestMethod]
        public void WhileCraftingStepFalseConditionTest()
        {
            Mock<ICraftingStep> mockChildStep = new Mock<ICraftingStep>();

            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            WhileCraftingStep whileStep = new WhileCraftingStep();
            whileStep.Children.Add(mockChildStep.Object);
            whileStep.Condition = getTestCondition();
            craftingSteps.Add(whileStep);

            EndCraftingStep endCrafting = new EndCraftingStep();
            craftingSteps.Add(endCrafting);

            Equipment equipment = getTestEquipment(false);
            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);

            CancellationToken token = new CancellationToken(false);

            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            mockChildStep.Verify(x => x.Craft(It.IsAny<Equipment>(), It.IsAny<AffixManager>()), Times.Never);

            Assert.IsTrue(equipment.Completed);
        }

        [TestMethod]
        public void CurrencyCraftingTest()
        {
            Mock<ICraftingStep> mockChildStep = new Mock<ICraftingStep>();

            List<ICraftingStep> craftingSteps = new List<ICraftingStep>();
            WhileCraftingStep whileStep = new WhileCraftingStep();
            whileStep.Children.Add(mockChildStep.Object);
            whileStep.Condition = getTestCondition();
            craftingSteps.Add(whileStep);

            EndCraftingStep endCrafting = new EndCraftingStep();
            craftingSteps.Add(endCrafting);

            Equipment equipment = getTestEquipment(false);
            AffixManager affixManager = CreateAffixManager(equipment.ItemBase);

            CancellationToken token = new CancellationToken(false);

            craftManager.Craft(craftingSteps, equipment, affixManager, token, null);

            mockChildStep.Verify(x => x.Craft(It.IsAny<Equipment>(), It.IsAny<AffixManager>()), Times.Never);

            Assert.IsTrue(equipment.Completed);
        }

        public Equipment getTestEquipment(bool matchTestCondition)
        {
            Equipment item = new Equipment();

            Stat stat = new Stat();
            stat.Value1 = matchTestCondition ? 5 : -1;

            Affix affix = new Affix();
            affix.Group = "TestMod";
            affix.GenerationType = "Prefix";

            stat.Affix = affix;

            item.ItemBase = new ItemBase();
            item.ItemBase.Tags = new List<string>() {"default"};

            item.Stats.Add(stat);

            return item;
        }

        public AffixManager CreateAffixManager(ItemBase itemBase)
        {
            List<Affix> allAffixes = new List<Affix>();

            string defaultTag = itemBase.Tags.First();

            for (int i = 0; i < 10; i++)
            {
                var generationType = i < 5 ? "prefix" : "suffix";
                Affix affix = getTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }, generationType);
                allAffixes.Add(affix);
            }

            return new AffixManager(itemBase, allAffixes, new List<Affix>());

        }

        private Affix getTestAffix(string name, string group, Dictionary<string, int> spawnTagWeights, string generationType, params string[] tags)
        {
            return new Affix { FullName = name, Name = name, Group = group, Tags = new HashSet<string>(), AddsTags = tags.ToList(), SpawnWeights = spawnTagWeights, GenerationWeights = new Dictionary<string, int>(), GenerationType = generationType };
        }

        public void updateEquipment(Equipment equipment, bool matchTestCondition)
        {
            equipment.Stats.First().Value1 = matchTestCondition ? 5 : -1;
        }

        public CraftingCondition getTestCondition()
        {
            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                1,
                1,
                CreateTestConditionAffix(0, 10, "TestMod"));

            return CreateTestCondition(subcondition);

        }

        private static CraftingCondition CreateTestCondition(params CraftingSubcondition[] subconditions)
        {
            CraftingCondition condition = new CraftingCondition();
            condition.CraftingSubConditions.AddRange(subconditions.ToList());
            return condition;
        }

        private static CraftingSubcondition CreateCraftingSubcondition(SubconditionAggregateType aggregateType, StatValueType statType, int? min, int? max, params ConditionAffix[] affixes)
        {
            CraftingSubcondition subcondition = new CraftingSubcondition
            {
                AggregateType = aggregateType,
                AggregateMin = min,
                AggregateMax = max,
                ValueType = statType
            };
            subcondition.PrefixConditions.AddRange(affixes.ToList());
            return subcondition;
        }

        private static ConditionAffix CreateTestConditionAffix(int? min, int? max, string modName)
        {
            ConditionAffix conditionAffix = new ConditionAffix { Max1 = max, Min1 = min, ModType = modName };
            return conditionAffix;
        }
    }
}
