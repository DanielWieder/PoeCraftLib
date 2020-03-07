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
    public class CurrencyTest
    {
        private readonly CurrencyFactory currencyFactory;
        private readonly ItemFactory itemFactory;
        private readonly AffixFactory affixFactory;

        private readonly int INVALID = -1;

        public CurrencyTest()
        {
            itemFactory = new ItemFactory();
            affixFactory = new AffixFactory();

            IRandom random = new PoeRandom();
            currencyFactory = new CurrencyFactory(
                random,
                new EssenceFactory(itemFactory, affixFactory), 
                new FossilFactory(affixFactory), 
                new MasterModFactory(affixFactory, itemFactory));
        }

        [TestMethod]
        public void AlchemyOrbRarityNormalSuccessTest()
        {
            var equipment = GetTestItem();
            var spent = TestCurrency(CurrencyNames.AlchemyOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AlchemyOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
        }

        [TestMethod]
        public void AlchemyOrbRarityMagicRareFailure()
        {
            TestRaritiesForFailure(CurrencyNames.AlchemyOrb, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }


        [TestMethod]
        public void AlterationOrbRarityMagicSuccessTest()
        {
            var equipment = GetTestItem();
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = TestCurrency(CurrencyNames.AlterationOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AlterationOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 1 && equipment.Stats.Count <= 2);
        }

        [TestMethod]
        public void AlterationOrbRarityNormalRareFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.AlterationOrb, EquipmentRarity.Normal, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void AugmentationOrbRarityMagicSuccessTest()
        {
            var equipment = GetTestItem();
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = TestCurrency(CurrencyNames.AugmentationOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AugmentationOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.AreEqual(1, equipment.Stats.Count);
        }

        [TestMethod]
        public void AugmentationOrbRarityNormalRareFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.AugmentationOrb, EquipmentRarity.Normal, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void AnnulmentOrbRarityRareSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            int initialStatCount = equipment.Stats.Count;

            var spent = TestCurrency(CurrencyNames.AnnulmentOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AnnulmentOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.AreEqual(initialStatCount - 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void AnnulmentOrbRarityMagicSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            TestCurrency(CurrencyNames.AugmentationOrb, equipment);
            int initialStatCount = equipment.Stats.Count;

            var spent = TestCurrency(CurrencyNames.AnnulmentOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.AnnulmentOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.AreEqual(initialStatCount - 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void AnnulmentOrbRarityNoAffixesFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.AnnulmentOrb, EquipmentRarity.Normal, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        //Todo: Add blessed orb tests here when fully implemented

        [TestMethod]
        public void ChanceOrbRarityNormalSuccessTest()
        {
            var equipment = GetTestItem();
            var spent = TestCurrency(CurrencyNames.ChanceOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ChanceOrb]);
            Assert.IsTrue(EquipmentRarity.Magic == equipment.Rarity || EquipmentRarity.Rare == equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 1);
        }

        [TestMethod]
        public void ChanceOrbRarityRarityMagicRareFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.ChanceOrb, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void ChaosOrbRarityRareSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent = TestCurrency(CurrencyNames.ChaosOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ChaosOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
        }

        [TestMethod]
        public void ChaosOrbRarityRarityNormalMagicFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.ChaosOrb, EquipmentRarity.Normal, EquipmentRarity.Magic);
        }

        [TestMethod]
        public void DivineOrbRarityMagicSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.TransmuationOrb, equipment);

            TestDivineOrb(equipment);

            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
        }

        [TestMethod]
        public void DivineOrbRarityRareSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.AlchemyOrb, equipment);

            TestDivineOrb(equipment);

            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
        }

        private void TestDivineOrb(Equipment equipment)
        {
            equipment.Stats[0].Value1 = INVALID;

            if (equipment.Stats[0].Value2 != null)
                equipment.Stats[0].Value2 = INVALID;

            if (equipment.Stats[0].Value3 != null)
                equipment.Stats[0].Value3 = INVALID;

            var spent = TestCurrency(CurrencyNames.DivineOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.DivineOrb]);

            Assert.AreNotEqual(INVALID, equipment.Stats[0].Value1);

            if (equipment.Stats[0].Value2 != null)
                Assert.AreNotEqual(INVALID, equipment.Stats[0].Value2);

            if (equipment.Stats[0].Value3 != null)
                Assert.AreNotEqual(INVALID, equipment.Stats[0].Value3);
        }

        [TestMethod]
        public void DivineOrbRarityRarityNoExplicitFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.DivineOrb, EquipmentRarity.Normal, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void RegalOrbRarityRareSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            int affixCount = equipment.Stats.Count;
            var spent = TestCurrency(CurrencyNames.RegalOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.RegalOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.AreEqual(affixCount + 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void RegalOrbRarityRarityNormalRareFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.RegalOrb, EquipmentRarity.Normal, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void ExaltedOrbRarityRareSuccessTest()
        {
            var equipment = GetTestItem();
            TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            TestCurrency(CurrencyNames.RegalOrb, equipment);
            int affixCount = equipment.Stats.Count;
            var spent = TestCurrency(CurrencyNames.ExaltedOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ExaltedOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.AreEqual(affixCount + 1, equipment.Stats.Count);
        }

        [TestMethod]
        public void ExaltedOrbRarityRarityNormalMagicFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.ExaltedOrb, EquipmentRarity.Normal, EquipmentRarity.Magic);
        }

        [TestMethod]
        public void ScouringOrbRarityMagicSuccessTest()
        {
            var equipment = GetTestItem();
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = TestCurrency(CurrencyNames.ScouringOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ScouringOrb]);
            Assert.AreEqual(EquipmentRarity.Normal, equipment.Rarity);
            Assert.AreEqual(0, equipment.Stats.Count);
        }

        [TestMethod]
        public void ScouringOrbRarityRareSuccessTest()
        {
            var equipment = GetTestItem();
            equipment.Rarity = EquipmentRarity.Rare;
            var spent = TestCurrency(CurrencyNames.ScouringOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.ScouringOrb]);
            Assert.AreEqual(EquipmentRarity.Normal, equipment.Rarity);
            Assert.AreEqual(0, equipment.Stats.Count);
        }

        [TestMethod]
        public void ScouringOrbRarityRarityNormalFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.ScouringOrb, EquipmentRarity.Normal);
        }

        [TestMethod]
        public void TransmutationOrbRarityNormalSuccessTest()
        {
            var equipment = GetTestItem();
            var spent = TestCurrency(CurrencyNames.TransmuationOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.TransmuationOrb]);
            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 1 && equipment.Stats.Count <= 2);
        }

        [TestMethod]
        public void TransmutationOrbRarityRarityMagicRareFailureTest()
        {
            TestRaritiesForFailure(CurrencyNames.TransmuationOrb, EquipmentRarity.Magic, EquipmentRarity.Rare);
        }

        [TestMethod]
        public void VaalOrbSuccessTest()
        {
            var equipment = GetTestItem();
            var spent = TestCurrency(CurrencyNames.VaalOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.VaalOrb]);
            Assert.IsTrue(equipment.Corrupted);
        }

        [TestMethod]
        public void CorruptionStopsCurrencyTest()
        {
            var equipment = GetTestItem();
            equipment.Corrupted = true;
            var spent1 = TestCurrency(CurrencyNames.VaalOrb, equipment);
            var spent2 = TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent3 = TestCurrency(CurrencyNames.TransmuationOrb, equipment);

            Assert.IsFalse(spent2.ContainsKey(CurrencyNames.AlchemyOrb));
            Assert.IsFalse(spent3.ContainsKey(CurrencyNames.TransmuationOrb));
            Assert.IsFalse(spent1.ContainsKey(CurrencyNames.VaalOrb));
        }

        // Todo: Update vaal orb test when implicits are fully implemented

        private Dictionary<string, int> TestCurrency(String currencyName, Equipment equipment)
        {
            var currency = currencyFactory.GetCurrencyByName(currencyName);
            var affixManager = GetAffixManager(equipment);
            var spent = currency.Execute(equipment, affixManager);
            return spent;
        }

        private AffixManager GetAffixManager(Equipment equipment)
        {
            var itemAffixes = affixFactory.GetAffixesForItem(equipment.ItemBase.Tags, equipment.ItemBase.ItemClass, 84);
            AffixManager affixManager = new AffixManager(equipment.ItemBase, itemAffixes, new List<Affix>());
            return affixManager;
        }

        private Equipment GetTestItem()
        {
            var itemBase = itemFactory.Armour.First();
            return itemFactory.ToEquipment(itemBase, 84, new List<Influence>());
        }

        private void TestRaritiesForFailure(String currencyName, params EquipmentRarity[] rarities)
        {
            var equipment = GetTestItem();
            TestRaritiesForFailure(currencyName, equipment, rarities);
        }

        private void TestRaritiesForFailure(String currencyName, Equipment equipment, params EquipmentRarity[] rarities)
        {
            foreach (var rarity in rarities)
            {
                equipment.Rarity = rarity;
                var spent = TestCurrency(currencyName, equipment);

                Assert.IsFalse(spent.ContainsKey(currencyName));
            }
        }
    }
}
