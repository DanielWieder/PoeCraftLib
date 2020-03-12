

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace CurrencyTest
{
    [TestClass]
    public class DefaultCurrencyTest
    {
        private readonly int _invalid = -1;
        private readonly CurrencyTestHelper _currencyTestHelper;

        public DefaultCurrencyTest()
        {
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void AlchemyOrbRarityNormalSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AlchemyOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
        }

        [TestMethod]
        public void AlchemyOrbRarityMagicRareFailure()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.AlchemyOrb, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }


        [TestMethod]
        public void AlterationOrbRarityMagicSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.AlterationOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AlterationOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 1 && equipment.Stats.Count <= 2);
        }

        [TestMethod]
        public void AlterationOrbRarityNormalRareFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.AlterationOrb, EquipmentRarity.Normal, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void AugmentationOrbRarityMagicSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.AugmentationOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AugmentationOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.AreEqual(1, equipment.Stats.Count);
        }

        [TestMethod]
        public void AugmentationOrbRarityNormalRareFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.AugmentationOrb, EquipmentRarity.Normal, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void AugmentationOrbReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Defense;
            equipment.Rarity = EquipmentRarity.Magic;
            _currencyTestHelper.TestCurrency(CurrencyNames.AugmentationOrb, equipment);

            Assert.AreEqual(18, equipment.Quality);
        }

        [TestMethod]
        public void AnnulmentOrbRarityRareSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            int initialStatCount = equipment.Stats.Count;

            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.AnnulmentOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AnnulmentOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.AreEqual(initialStatCount - 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void AnnulmentOrbRarityMagicSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.AugmentationOrb, equipment);
            int initialStatCount = equipment.Stats.Count;

            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.AnnulmentOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AnnulmentOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.AreEqual(initialStatCount - 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void AnnulmentOrbRarityNoAffixesFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.AnnulmentOrb, EquipmentRarity.Normal, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void AnnulmentOrbReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            _currencyTestHelper.TestCurrency(CurrencyNames.AnnulmentOrb, equipment);

            Assert.AreEqual(0, equipment.Quality);
        }

        //Todo: Add blessed orb tests here when fully implemented

        [TestMethod]
        public void ChanceOrbRarityNormalSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ChanceOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ChanceOrb]);
            Assert.IsTrue(EquipmentRarity.Magic == equipment.Rarity || EquipmentRarity.Rare == equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 1);
        }

        [TestMethod]
        public void ChanceOrbRarityRarityMagicRareFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.ChanceOrb, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void ChaosOrbRarityRareSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ChaosOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ChaosOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
        }

        [TestMethod]
        public void ChaosOrbRarityRarityNormalMagicFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.ChaosOrb, EquipmentRarity.Normal, EquipmentRarity.Magic);
        }

        [TestMethod]
        public void DivineOrbRarityMagicSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);

            TestDivineOrb(equipment);

            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
        }

        [TestMethod]
        public void DivineOrbRarityRareSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);

            TestDivineOrb(equipment);

            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
        }

        private void TestDivineOrb(Equipment equipment)
        {
            equipment.Stats[0].Value1 = _invalid;

            if (equipment.Stats[0].Value2 != null)
                equipment.Stats[0].Value2 = _invalid;

            if (equipment.Stats[0].Value3 != null)
                equipment.Stats[0].Value3 = _invalid;

            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.DivineOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.DivineOrb]);

            Assert.AreNotEqual(_invalid, equipment.Stats[0].Value1);

            if (equipment.Stats[0].Value2 != null)
                Assert.AreNotEqual(_invalid, equipment.Stats[0].Value2);

            if (equipment.Stats[0].Value3 != null)
                Assert.AreNotEqual(_invalid, equipment.Stats[0].Value3);
        }

        [TestMethod]
        public void DivineOrbRarityRarityNoExplicitFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.DivineOrb, EquipmentRarity.Normal, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void RegalOrbRarityRareSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            int affixCount = equipment.Stats.Count;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.RegalOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.AreEqual(affixCount + 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void RegalOrbRarityRarityNormalRareFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.RegalOrb, EquipmentRarity.Normal, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void RegalOrbReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);

            Assert.AreEqual(15, equipment.Quality);
        }

        [TestMethod]
        public void ExaltedOrbRarityRareSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);
            int affixCount = equipment.Stats.Count;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ExaltedOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.AreEqual(affixCount + 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void ExaltedOrbTooManyAffixesFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);
            int affixCount = equipment.Stats.Count;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);

            Assert.IsTrue(!spent.Any());
        }

        [TestMethod]
        public void ExaltedOrbRarityRarityNormalMagicFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.ExaltedOrb, EquipmentRarity.Normal, EquipmentRarity.Magic);
        }

        [TestMethod]
        public void ExaltedOrbReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            equipment.Rarity = EquipmentRarity.Rare;

            _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);

            Assert.AreEqual(0, equipment.Quality);
        }

        [TestMethod]
        public void ScouringOrbRarityMagicSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ScouringOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ScouringOrb]);
            Assert.AreEqual(EquipmentRarity.Normal, equipment.Rarity);
            Assert.AreEqual(0, equipment.Stats.Count);
        }

        [TestMethod]
        public void ScouringOrbRarityRareSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            equipment.Rarity = EquipmentRarity.Rare;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ScouringOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ScouringOrb]);
            Assert.AreEqual(EquipmentRarity.Normal, equipment.Rarity);
            Assert.AreEqual(0, equipment.Stats.Count);
        }

        [TestMethod]
        public void ScouringOrbRarityRarityNormalFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.ScouringOrb, EquipmentRarity.Normal);
        }

        [TestMethod]
        public void TransmutationOrbRarityNormalSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.TransmuationOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 1 && equipment.Stats.Count <= 2);
        }

        [TestMethod]
        public void TransmutationOrbRarityRarityMagicRareFailureTest()
        {
            _currencyTestHelper.TestRaritiesForFailure(CurrencyNames.TransmuationOrb, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void VaalOrbSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.VaalOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.VaalOrb]);
            Assert.IsTrue(equipment.Corrupted);
        }

        [TestMethod]
        public void CorruptionStopsCurrencyTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            equipment.Corrupted = true;
            var spent1 = _currencyTestHelper.TestCurrency(CurrencyNames.VaalOrb, equipment);
            var spent2 = _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent3 = _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);

            Assert.IsFalse(spent2.ContainsKey(CurrencyNames.AlchemyOrb));
            Assert.IsFalse(spent3.ContainsKey(CurrencyNames.TransmuationOrb));
            Assert.IsFalse(spent1.ContainsKey(CurrencyNames.VaalOrb));
        }

        // Todo: Update vaal orb test when implicits are fully implemented
    }
}
