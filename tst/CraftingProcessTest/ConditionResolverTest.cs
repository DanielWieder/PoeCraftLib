using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Crafting;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CraftingTest
{
    [TestClass]
    public class ConditionResolverTest
    {
        private readonly ConditionResolver _conditionResolver = new ConditionResolver();
        private readonly Random _random = new Random();

        [TestMethod]
        public void CountSubconditionIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                1,
                1,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition);

            Equipment item = new Equipment();

            Stat stat = new Stat();
            stat.Value1 = range;

            Affix affix = new Affix();
            affix.Group = modType;
            affix.GenerationType = "Prefix";

            stat.Affix = affix;

            item.Stats.Add(stat);

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void CountSubconditionModsAreOutOfRangeInvalidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                1,
                1,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition);

            var stat = CreateStat(conditionMax + range, modType);

            Equipment item = new Equipment();
            item.Stats.Add(stat);

            Assert.IsFalse(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void CountSubconditionWithMultipleSubconditionsIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                1,
                1,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat = CreateStat(range, modType);

            Equipment item = new Equipment();
            item.Stats.Add(stat);

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void CountSubconditionWithMultipleAffixRequirementsIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                2,
                2,
                CreateTestAffix(conditionMin, conditionMax, modType1), 
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(range, modType1);
            var stat2 = CreateStat(range, modType2);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);
            item.Stats.Add(stat2);

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void CountSubconditionWithZeroConditionalModsIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                0,
                2,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            Equipment item = new Equipment();

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void CountSubconditionWithZeroItemModsIsInvalidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.Count,
                StatValueType.Flat,
                1,
                3,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            Equipment item = new Equipment();

            Assert.IsFalse(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void AndSubconditionIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.And,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1),
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(range, modType1);
            var stat2 = CreateStat(range, modType2);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);
            item.Stats.Add(stat2);

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void AndSubconditionModIsOutOfRangeInvalidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.And,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1),
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(range, modType1);
            var stat2 = CreateStat(conditionMax + range, modType2);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);
            item.Stats.Add(stat2);

            Assert.IsFalse(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void AndSubconditionIsNotPresentInvalidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.And,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1),
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(range, modType1);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);

            Assert.IsFalse(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void IfSubconditionOneNotPresentIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.If,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1),
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(range, modType1);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void IfSubconditionZeroArePresentIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.If,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1),
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            Equipment item = new Equipment();

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void IfSubconditionOneDoesNotMatchIsInvalidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.If,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1),
                CreateTestAffix(conditionMin, conditionMax, modType2));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(conditionMax + range, modType1);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);

            Assert.IsFalse(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void NotSubconditionZeroModsIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType1";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.If,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            Equipment item = new Equipment();

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void NotSubconditionAbsentModIsValidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType1 = "TestModType1";
            string modType2 = "TestModType2";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.If,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType1));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(conditionMax + range, modType2);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);

            Assert.IsTrue(_conditionResolver.IsValid(condition, item));
        }

        [TestMethod]
        public void NotSubconditionPresentModIsInvalidTest()
        {
            int range = 10;
            int conditionMin = _random.Next(range);
            int conditionMax = range + _random.Next(range);

            string modType = "TestModType1";

            var subcondition = CreateCraftingSubcondition(
                SubconditionAggregateType.If,
                StatValueType.Flat,
                null,
                null,
                CreateTestAffix(conditionMin, conditionMax, modType));

            CraftingCondition condition = CreateTestCondition(subcondition, subcondition);

            var stat1 = CreateStat(conditionMax + range, modType);

            Equipment item = new Equipment();
            item.Stats.Add(stat1);

            Assert.IsFalse(_conditionResolver.IsValid(condition, item));
        }

        private static Stat CreateStat(int value, string modType)
        {
            Affix affix = new Affix {Group = modType, GenerationType = "Prefix"};
            Stat stat = new Stat { Value1 = value, Affix = affix };
            stat.Affix = affix;
            return stat;
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
                AggregateType = aggregateType, AggregateMin = min, AggregateMax = max, ValueType = statType
            };
            subcondition.PrefixConditions.AddRange(affixes.ToList());
            return subcondition;
        }

        private static ConditionAffix CreateTestAffix(int? min, int? max, string modName)
        {
            ConditionAffix conditionAffix = new ConditionAffix {Max1 = max, Min1 = min, ModType = modName};
            return conditionAffix;
        }
    }
}
