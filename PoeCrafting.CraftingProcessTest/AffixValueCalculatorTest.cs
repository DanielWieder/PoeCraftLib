using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using PoeCraftLib.CraftingSim;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CraftingProcessTest
{
    [TestClass]
    public class AffixValueCalculatorTest
    {
        readonly AffixValueCalculator _affixValueCalculator = new AffixValueCalculator();

        [TestMethod]
        public void GetFlatPrefixValueTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 15;
            AffixType affixType = AffixType.Prefix;

            Affix affix = CreateTestAffix(modName, affixType);
            Stat stat = CreateTestStat(testValue1, testValue2, testValue3, affix);
            Equipment testEquipment = CreateTestEquipment(stat);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Flat);

            Assert.AreEqual(stat.Value1, values[0]);
            Assert.AreEqual(stat.Value2, values[1]);
            Assert.AreEqual(stat.Value3, values[2]);
        }

        [TestMethod]
        public void GetFlatSuffixValueTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 15;
            AffixType affixType = AffixType.Suffix;

            Affix affix = CreateTestAffix(modName, affixType);
            Stat stat = CreateTestStat(testValue1, testValue2, testValue3, affix);
            Equipment testEquipment = CreateTestEquipment(stat);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Flat);

            Assert.AreEqual(stat.Value1, values[0]);
            Assert.AreEqual(stat.Value2, values[1]);
            Assert.AreEqual(stat.Value3, values[2]);
        }

        [TestMethod]
        [ExpectedException(typeof(NotImplementedException), "The affix type Implicit is not recognized.")]
        public void GetFlatImplicitValueNotYetImplementedTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 15;
            AffixType affixType = AffixType.Implicit;

            Affix affix = CreateTestAffix(modName, affixType);
            Stat stat = CreateTestStat(testValue1, testValue2, testValue3, affix);
            Equipment testEquipment = CreateTestEquipment(stat);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Flat);
        }

        [TestMethod]
        public void GetFlatPrefixMultipleAffixTest()
        {
            String correctModName = "correctMod";
            String incorrectModName = "incorrectMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 3;
            AffixType affixType = AffixType.Prefix;

            Affix correctAffix = CreateTestAffix(correctModName, affixType);
            Affix incorrectAffix = CreateTestAffix(incorrectModName, affixType);
            Stat stat1 = CreateTestStat(testValue1, null, null, correctAffix);
            Stat stat2 = CreateTestStat(testValue2, null, null, incorrectAffix);
            Stat stat3 = CreateTestStat(testValue3, null, null, incorrectAffix);

            Equipment testEquipment = new Equipment();
            testEquipment.Stats.Add(stat1);
            testEquipment.Stats.Add(stat2);
            testEquipment.Stats.Add(stat3);

            List<int> values = _affixValueCalculator.GetAffixValues(correctModName, testEquipment, affixType, StatValueType.Flat);

            Assert.AreEqual(stat1.Value1, values[0]);
        }

        [TestMethod]
        public void GetFlatPrefixHighestValueNoTieTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 7;
            AffixType affixType = AffixType.Prefix;

            Affix affix = CreateTestAffix(modName, affixType);
            Stat stat1 = CreateTestStat(testValue1, null, null, affix);
            Stat stat2 = CreateTestStat(testValue2, null, null, affix);
            Stat stat3 = CreateTestStat(testValue3, null, null, affix);

            Equipment testEquipment = new Equipment();
            testEquipment.Stats.Add(stat1);
            testEquipment.Stats.Add(stat2);
            testEquipment.Stats.Add(stat3);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Flat);

            Assert.AreEqual(stat2.Value1, values[0]);
        }

        [TestMethod]
        public void GetFlatPrefixHighestValueOneTieTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 7;
            AffixType affixType = AffixType.Prefix;

            Affix affix = CreateTestAffix(modName, affixType);
            Stat stat1 = CreateTestStat(0, testValue1, null, affix);
            Stat stat2 = CreateTestStat(0, testValue2, null, affix);
            Stat stat3 = CreateTestStat(0, testValue3, null, affix);

            Equipment testEquipment = new Equipment();
            testEquipment.Stats.Add(stat1);
            testEquipment.Stats.Add(stat2);
            testEquipment.Stats.Add(stat3);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Flat);

            Assert.AreEqual(stat2.Value1, values[0]);
        }

        [TestMethod]
        public void GetFlatPrefixHighestValueTwoTieTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            int testValue2 = 10;
            int testValue3 = 7;
            AffixType affixType = AffixType.Prefix;

            Affix affix = CreateTestAffix(modName, affixType);
            Stat stat1 = CreateTestStat(0, 0, testValue1, affix);
            Stat stat2 = CreateTestStat(0, 0, testValue2, affix);
            Stat stat3 = CreateTestStat(0, 0, testValue3, affix);

            Equipment testEquipment = new Equipment();
            testEquipment.Stats.Add(stat1);
            testEquipment.Stats.Add(stat2);
            testEquipment.Stats.Add(stat3);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Flat);

            Assert.AreEqual(stat2.Value1, values[0]);
        }

        [TestMethod]
        public void GetMaxPrefixValueTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            AffixType affixType = AffixType.Prefix;

            Affix affix = CreateTestAffix(modName, affixType);
            affix.StatMin1 = 0;
            affix.StatMax1 = 10;
            affix.StatName1 = "testStat";

            Stat stat = CreateTestStat(testValue1, null, null, affix);
            Equipment testEquipment = CreateTestEquipment(stat);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Max);

            Assert.AreEqual(affix.StatMax1, values[0]);
        }

        [TestMethod]
        public void GetTierPrefixValueTest()
        {
            String modName = "testMod";
            int testValue1 = 5;
            AffixType affixType = AffixType.Prefix;

            Affix affix = CreateTestAffix(modName, affixType);
            affix.StatMin1 = 0;
            affix.StatMax1 = 10;
            affix.StatName1 = "testStat";
            affix.Tier = 5;

            Stat stat = CreateTestStat(testValue1, null, null, affix);
            Equipment testEquipment = CreateTestEquipment(stat);

            List<int> values = _affixValueCalculator.GetAffixValues(modName, testEquipment, affixType, StatValueType.Tier);

            Assert.AreEqual(affix.Tier, values[0]);
        }

        private Equipment CreateTestEquipment(Stat stat)
        {
            Equipment testEquipment = new Equipment();
            testEquipment.Stats.Add(stat);
            return testEquipment;
        }

        private static Affix CreateTestAffix(string modName, AffixType affixType)
        {
            Affix affix = new Affix();
            affix.Group = modName;
            affix.GenerationType = affixType.ToString();
            return affix;
        }

        private static Stat CreateTestStat(int testValue1, int? testValue2, int? testValue3, Affix affix)
        {
            Stat stat = new Stat();
            stat.Affix = affix;
            stat.Value1 = testValue1;
            stat.Value2 = testValue2;
            stat.Value3 = testValue3;
            return stat;
        }
    }
}
