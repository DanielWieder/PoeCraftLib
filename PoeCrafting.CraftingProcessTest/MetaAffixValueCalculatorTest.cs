using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCrafting.CraftingSim;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CraftingProcessTest
{
    [TestClass]
    public class MetaAffixValueCalculatorTest
    {
        AffixValueCalculator affixValueCalculator = new AffixValueCalculator();
        Random random = new Random();

        [TestMethod]
        public void MetamodOpenPrefixesTest()
        {
            String metaModName = AffixNames.OpenPrefix;

            Affix affix = CreateTestAffix("testMod", AffixType.Prefix);
            Stat stat = CreateTestStat(affix, 5, 10);
            Equipment testEquipment = new Equipment();

            for (int i = 0; i < 4; i++)
            {
                List<int> values = affixValueCalculator.GetAffixValues(metaModName, testEquipment, AffixType.Meta, StatValueType.Flat);
                testEquipment.Stats.Add(stat);

                Assert.AreEqual(3-i, values[0]);
                Assert.AreEqual(1, values.Count);
            }
        }

        [TestMethod]
        public void MetamodOpenSuffixesTest()
        {
            String metaModName = AffixNames.OpenSuffix;

            Affix affix = CreateTestAffix("testMod", AffixType.Suffix);
            Stat stat = CreateTestStat(affix, 5, 10);
            Equipment testEquipment = new Equipment();

            for (int i = 0; i < 4; i++)
            {
                List<int> values = affixValueCalculator.GetAffixValues(metaModName, testEquipment, AffixType.Meta, StatValueType.Flat);
                Assert.AreEqual(3 - i, values[0]);
                Assert.AreEqual(1, values.Count);

                testEquipment.Stats.Add(stat);
            }
        }

        [TestMethod]
        public void MetamodFlatTotalEnergyShieldTest()
        {
            String metaModName = AffixNames.TotalEnergyShield;

            string defenseProperty = ItemProperties.EnergyShield;
            string localDefense = AffixNames.LocalEnergyShield;
            List<string> percentDefense = AffixGroupings.EnergyShieldPercent;
            List<string> hybridDefense = AffixGroupings.HybridEnergyShieldPercent;

            EvaluateDefenseMetamod(metaModName, defenseProperty, localDefense, percentDefense, hybridDefense);
        }

        [TestMethod]
        public void MetamodFlatEvasionTest()
        {
            String metaModName = AffixNames.TotalEvasion;

            string defenseProperty = ItemProperties.Evasion;
            string localDefense = AffixNames.LocalEvasion;
            List<string> percentDefense = AffixGroupings.EvasionPercent;
            List<string> hybridDefense = AffixGroupings.HybridEvasionPercent;

            EvaluateDefenseMetamod(metaModName, defenseProperty, localDefense, percentDefense, hybridDefense);
        }

        [TestMethod]
        public void MetamodFlatArmourTest()
        {
            String metaModName = AffixNames.TotalArmor;

            string defenseProperty = ItemProperties.Armour;
            string localDefense = AffixNames.LocalArmour;
            List<string> percentDefense = AffixGroupings.ArmourPercent;
            List<string> hybridDefense = AffixGroupings.HybridArmourPercent;

            EvaluateDefenseMetamod(metaModName, defenseProperty, localDefense, percentDefense, hybridDefense);
        }

        [TestMethod]
        public void MetamodFlatEleResistanceTest()
        {
            string defenseProperty = ItemProperties.Armour;
            Equipment testEquipment = new Equipment();

            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { defenseProperty, 80 }
            };
            testEquipment.ItemBase = itemBase;

            Affix affix1 = CreateTestAffix(AffixNames.ColdResist, AffixType.Suffix);
            Affix affix2 = CreateTestAffix(AffixNames.FireResist, AffixType.Suffix);
            Affix affix3 = CreateTestAffix(AffixNames.LightningResist, AffixType.Suffix);
            Affix affix4 = CreateTestAffix(AffixNames.ChaosResist, AffixType.Suffix);
            Affix affix5 = CreateTestAffix(AffixNames.AllResist, AffixType.Suffix);

            Stat stat1 = CreateTestStat(affix1, 20);
            Stat stat2 = CreateTestStat(affix2, 25);
            Stat stat3 = CreateTestStat(affix3, 34);
            Stat stat4 = CreateTestStat(affix4, 40);
            Stat stat5 = CreateTestStat(affix5, 12);

            testEquipment.Stats.Add(stat1);
            testEquipment.Stats.Add(stat2);
            testEquipment.Stats.Add(stat3);
            testEquipment.Stats.Add(stat4);
            testEquipment.Stats.Add(stat5);

            List<int> values1 = affixValueCalculator.GetAffixValues(AffixNames.TotalResistances, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(110, values1[0]);
            Assert.AreEqual(1, values1.Count);

            List<int> values2 = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalResistances, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(95, values2[0]);
            Assert.AreEqual(1, values2.Count);
        }

        [TestMethod]
        public void MetamodFlatPhysicalDamageTest()
        {
            Equipment testEquipment = new Equipment();
            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { ItemProperties.MinDamage, 10 },
                { ItemProperties.MaxDamage, 30 },
                { ItemProperties.APS, 2 }
            };
            testEquipment.ItemBase = itemBase;

            List<int> noModsPhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> noModsTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> noModsElemental = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(48, noModsPhys[0]);
            Assert.AreEqual(48, noModsTotal[0]);
            Assert.AreEqual(0, noModsElemental[0]);

            Affix affix1 = CreateTestAffix(AffixNames.LocalAttackSpeed, AffixType.Suffix);
            Stat stat1 = CreateTestStat(affix1, 100);
            testEquipment.Stats.Add(stat1);

            List<int> attackSpeedPhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> attackSpeedTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(96, attackSpeedPhys[0]);
            Assert.AreEqual(96, attackSpeedTotal[0]);

            Affix affix2 = CreateTestAffix(random, AffixGroupings.FlatPhysicalDamage, AffixType.Prefix);
            Affix affix3 = CreateTestAffix(AffixNames.LocalPhysicalPercent, AffixType.Prefix);
            Affix affix4 = CreateTestAffix(AffixNames.LocalPhysicalHybrid, AffixType.Prefix);

            Stat stat2 = CreateTestStat(affix2, 10, 30);
            testEquipment.Stats.Add(stat2);
            Stat stat3 = CreateTestStat(affix3, 80);
            testEquipment.Stats.Add(stat3);
            Stat stat4 = CreateTestStat(affix4, 20);
            testEquipment.Stats.Add(stat4);

            List<int> fullPhysModsPhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> fullPhysModsTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(352, fullPhysModsPhys[0]);
            Assert.AreEqual(352, fullPhysModsTotal[0]);
        }

        [TestMethod]
        public void MetamodFlatElementalDamageTest()
        {
            Equipment testEquipment = new Equipment();
            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { ItemProperties.MinDamage, 10 },
                { ItemProperties.MaxDamage, 30 },
                { ItemProperties.APS, 2 }
            };
            testEquipment.ItemBase = itemBase;

            Affix affix1 = CreateTestAffix(random, AffixGroupings.FlatChaosDamage, AffixType.Prefix);
            Affix affix2 = CreateTestAffix(random, AffixGroupings.FlatColdDamage, AffixType.Prefix);
            Affix affix3 = CreateTestAffix(random, AffixGroupings.FlatFireDamage, AffixType.Prefix);
            Affix affix4 = CreateTestAffix(random, AffixGroupings.FlatLightningDamage, AffixType.Prefix);

            Stat stat1 = CreateTestStat(affix1, 1, 10);
            testEquipment.Stats.Add(stat1);
            Stat stat2 = CreateTestStat(affix2, 10, 30);
            testEquipment.Stats.Add(stat2);
            Stat stat3 = CreateTestStat(affix3, 10, 20);
            testEquipment.Stats.Add(stat3);
            Stat stat4 = CreateTestStat(affix4, 20, 30);
            testEquipment.Stats.Add(stat4);

            List<int> eleDamagePhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageEleDamage = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(48, eleDamagePhys[0]);
            Assert.AreEqual(120, eleDamageEleDamage[0]);
            Assert.AreEqual(168, eleDamageTotal[0]);

            Affix affix5 = CreateTestAffix(AffixNames.LocalAttackSpeed, AffixType.Suffix);
            Stat stat5 = CreateTestStat(affix5, 100);
            testEquipment.Stats.Add(stat5);

            List<int> eleDamageWithAttackSpeedPhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageWithAttackSpeedEleDamage = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageWithAttackSpeedTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(96, eleDamageWithAttackSpeedPhys[0]);
            Assert.AreEqual(240, eleDamageWithAttackSpeedEleDamage[0]);
            Assert.AreEqual(336, eleDamageWithAttackSpeedTotal[0]);
        }

        [TestMethod]
        public void MetamodFlatTotalDamageSevenPrefixesSuperiorPhysicalTest()
        {
            Equipment testEquipment = new Equipment();
            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { ItemProperties.MinDamage, 10 },
                { ItemProperties.MaxDamage, 30 },
                { ItemProperties.APS, 2 }
            };
            testEquipment.ItemBase = itemBase;

            Affix affix1 = CreateTestAffix(random, AffixGroupings.FlatChaosDamage, AffixType.Prefix);
            Affix affix2 = CreateTestAffix(random, AffixGroupings.FlatColdDamage, AffixType.Prefix);
            Affix affix3 = CreateTestAffix(random, AffixGroupings.FlatFireDamage, AffixType.Prefix);
            Affix affix4 = CreateTestAffix(random, AffixGroupings.FlatLightningDamage, AffixType.Prefix);
            Affix affix5 = CreateTestAffix(random, AffixGroupings.FlatPhysicalDamage, AffixType.Prefix);
            Affix affix6 = CreateTestAffix(AffixNames.LocalPhysicalPercent, AffixType.Prefix);
            Affix affix7 = CreateTestAffix(AffixNames.LocalPhysicalHybrid, AffixType.Prefix);
            Affix affix8 = CreateTestAffix(AffixNames.LocalAttackSpeed, AffixType.Suffix);

            Stat stat1 = CreateTestStat(affix1, 1, 10);
            testEquipment.Stats.Add(stat1);
            Stat stat2 = CreateTestStat(affix2, 10, 30);
            testEquipment.Stats.Add(stat2);
            Stat stat3 = CreateTestStat(affix3, 10, 20);
            testEquipment.Stats.Add(stat3);
            Stat stat4 = CreateTestStat(affix4, 20, 30);
            testEquipment.Stats.Add(stat4);
            Stat stat5 = CreateTestStat(affix5, 10, 30);
            testEquipment.Stats.Add(stat5);
            Stat stat6 = CreateTestStat(affix6, 80);
            testEquipment.Stats.Add(stat6);
            Stat stat7 = CreateTestStat(affix7, 20);
            testEquipment.Stats.Add(stat7);
            Stat stat8 = CreateTestStat(affix8, 100);
            testEquipment.Stats.Add(stat8);

            List<int> eleDamagePhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageEleDamage = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(352, eleDamagePhys[0]);
            Assert.AreEqual(240, eleDamageEleDamage[0]);
            Assert.AreEqual(352, eleDamageTotal[0]);
        }

        [TestMethod]
        public void MetamodFlatTotalDamageSevenPrefixesSuperiorElementalTest()
        {
            Equipment testEquipment = new Equipment();
            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { ItemProperties.MinDamage, 10 },
                { ItemProperties.MaxDamage, 30 },
                { ItemProperties.APS, 2 }
            };
            testEquipment.ItemBase = itemBase;

            Affix affix1 = CreateTestAffix(random, AffixGroupings.FlatChaosDamage, AffixType.Prefix);
            Affix affix2 = CreateTestAffix(random, AffixGroupings.FlatColdDamage, AffixType.Prefix);
            Affix affix3 = CreateTestAffix(random, AffixGroupings.FlatFireDamage, AffixType.Prefix);
            Affix affix4 = CreateTestAffix(random, AffixGroupings.FlatLightningDamage, AffixType.Prefix);
            Affix affix5 = CreateTestAffix(random, AffixGroupings.FlatPhysicalDamage, AffixType.Prefix);
            Affix affix6 = CreateTestAffix(AffixNames.LocalPhysicalPercent, AffixType.Prefix);
            Affix affix7 = CreateTestAffix(AffixNames.LocalPhysicalHybrid, AffixType.Prefix);
            Affix affix8 = CreateTestAffix(AffixNames.LocalAttackSpeed, AffixType.Suffix);

            Stat stat1 = CreateTestStat(affix1, 1, 10);
            testEquipment.Stats.Add(stat1);
            Stat stat2 = CreateTestStat(affix2, 10, 30);
            testEquipment.Stats.Add(stat2);
            Stat stat3 = CreateTestStat(affix3, 10, 20);
            testEquipment.Stats.Add(stat3);
            Stat stat4 = CreateTestStat(affix4, 60, 80);
            testEquipment.Stats.Add(stat4);
            Stat stat5 = CreateTestStat(affix5, 10, 30);
            testEquipment.Stats.Add(stat5);
            Stat stat6 = CreateTestStat(affix6, 80);
            testEquipment.Stats.Add(stat6);
            Stat stat7 = CreateTestStat(affix7, 20);
            testEquipment.Stats.Add(stat7);
            Stat stat8 = CreateTestStat(affix8, 100);
            testEquipment.Stats.Add(stat8);

            List<int> eleDamagePhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageEleDamage = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(352, eleDamagePhys[0]);
            Assert.AreEqual(420, eleDamageEleDamage[0]);
            Assert.AreEqual(420, eleDamageTotal[0]);
        }

        [TestMethod]
        public void MetamodFlatTotalDamageThreePrefixesCombinedPhysicalElementalTest()
        {
            Equipment testEquipment = new Equipment();
            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { ItemProperties.MinDamage, 10 },
                { ItemProperties.MaxDamage, 30 },
                { ItemProperties.APS, 2 }
            };
            testEquipment.ItemBase = itemBase;

            Affix affix1 = CreateTestAffix(random, AffixGroupings.FlatChaosDamage, AffixType.Prefix);
            Affix affix6 = CreateTestAffix(AffixNames.LocalPhysicalPercent, AffixType.Prefix);
            Affix affix7 = CreateTestAffix(AffixNames.LocalPhysicalHybrid, AffixType.Prefix);
            Affix affix8 = CreateTestAffix(AffixNames.LocalAttackSpeed, AffixType.Suffix);

            Stat stat1 = CreateTestStat(affix1, 30, 50);
            testEquipment.Stats.Add(stat1);
            Stat stat6 = CreateTestStat(affix6, 80);
            testEquipment.Stats.Add(stat6);
            Stat stat7 = CreateTestStat(affix7, 20);
            testEquipment.Stats.Add(stat7);
            Stat stat8 = CreateTestStat(affix8, 100);
            testEquipment.Stats.Add(stat8);

            List<int> eleDamagePhys = affixValueCalculator.GetAffixValues(AffixNames.TotalPhysicalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageEleDamage = affixValueCalculator.GetAffixValues(AffixNames.TotalElementalDps, testEquipment, AffixType.Meta, StatValueType.Flat);
            List<int> eleDamageTotal = affixValueCalculator.GetAffixValues(AffixNames.TotalDps, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(176, eleDamagePhys[0]);
            Assert.AreEqual(160, eleDamageEleDamage[0]);
            Assert.AreEqual(336, eleDamageTotal[0]);
        }

        private void EvaluateDefenseMetamod(string metaModName, string defenseProperty, string localDefense, List<string> percentDefense, List<string> hybridDefense)
        {
            Equipment testEquipment = new Equipment();

            ItemBase itemBase = new ItemBase();
            itemBase.Properties = new Dictionary<string, double>
            {
                { defenseProperty, 80 }
            };
            testEquipment.ItemBase = itemBase;

            Affix affix1 = CreateTestAffix(localDefense, AffixType.Prefix);
            Affix affix2 = CreateTestAffix(random, percentDefense, AffixType.Prefix);
            Affix affix3 = CreateTestAffix(random, hybridDefense, AffixType.Prefix);

            Stat stat1 = CreateTestStat(affix1, 20);
            Stat stat2 = CreateTestStat(affix2, 50);
            Stat stat3 = CreateTestStat(affix3, 30, 10);

            testEquipment.Stats.Add(stat1);
            testEquipment.Stats.Add(stat2);
            testEquipment.Stats.Add(stat3);

            List<int> values = affixValueCalculator.GetAffixValues(metaModName, testEquipment, AffixType.Meta, StatValueType.Flat);

            Assert.AreEqual(200, values[0]);
            Assert.AreEqual(1, values.Count);
        }

        private static Affix CreateTestAffix(Random random, List<string> grouping, AffixType affixType)
        {
            return CreateTestAffix(grouping[random.Next(0, grouping.Count - 1)], affixType);
        }

        private static Affix CreateTestAffix(string modName, AffixType affixType)
        {
            Affix affix = new Affix();
            affix.Group = modName;
            affix.GenerationType = affixType.ToString();
            return affix;
        }

        private static Stat CreateTestStat(Affix affix, int testValue1, int? testValue2 = null)
        {
            Stat stat = new Stat();
            stat.Affix = affix;
            stat.Value1 = testValue1;
            stat.Value2 = testValue2;
            return stat;
        }
    }
}
