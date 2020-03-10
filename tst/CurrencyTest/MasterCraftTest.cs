using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace CurrencyTest
{
    [TestClass]
    public class MasterCraftTest
    {
        private readonly CurrencyTestHelper _currencyTestHelper;

        public MasterCraftTest()
        {
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void MasterCraftSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");

            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.AnnulmentOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.AnnulmentOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency("MinionDamageOnWeapon1", equipment);

            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.AreEqual(1, equipment.Stats.Count);
            Assert.AreEqual("EinharMasterMinionDamageOnWeapon2h1_", equipment.Stats.First().Affix.FullName);
            Assert.AreEqual(4, spent[CurrencyNames.AugmentationOrb]);
        }

        [TestMethod]
        public void MasterCraftNoOpenExplicitFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");

            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.AugmentationOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency("MinionDamageOnWeapon1", equipment);

            Assert.AreEqual(EquipmentRarity.Magic, equipment.Rarity);
            Assert.AreEqual(2, equipment.Stats.Count);
            Assert.IsFalse(spent.Any());
        }

        [TestMethod]
        public void MasterCraftMatchingAffixGroupFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");
            equipment.Rarity = EquipmentRarity.Rare;
            var affixManager = _currencyTestHelper.GetAffixManager(equipment);

            var affixes = _currencyTestHelper.AffixFactory.GetAffixesForItem(equipment.ItemBase.Tags, equipment.ItemBase.ItemClass,
                84);

            var physAffix = affixes.First(x => x.Group == "PhysicalDamage");

            var stat = StatFactory.AffixToStat(_currencyTestHelper.Random, equipment, physAffix);
            equipment.Stats.Add(stat);

            var spent = _currencyTestHelper.TestCurrency("LocalPhysicalDamage1", equipment);

            Assert.AreEqual(1, equipment.Stats.Count);
            Assert.IsFalse(spent.Any());
        }

        [TestMethod]
        public void MasterCraftMatchingItemClassFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Helmet");
            equipment.Rarity = EquipmentRarity.Rare;
            var spent = _currencyTestHelper.TestCurrency("LocalPhysicalDamage1", equipment);
            Assert.IsFalse(spent.Any());
        }

        [TestMethod]
        public void MasterCraftTooManyMasterModsFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");
            equipment.Rarity = EquipmentRarity.Rare;
            var spent = _currencyTestHelper.TestCurrency("LocalPhysicalDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);

            spent = _currencyTestHelper.TestCurrency("LocalLightningDamage1", equipment);
            Assert.IsFalse(spent.Any());
            Assert.AreEqual(1, equipment.Stats.Count);
        }

        [TestMethod]
        public void MasterCraftMultimodThreeAffixesSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");
            equipment.Rarity = EquipmentRarity.Rare;

            var spent = _currencyTestHelper.TestCurrency("ItemGenerationCanHaveMultipleCraftedMods0", equipment);
            Assert.AreEqual(2, spent[CurrencyNames.ExaltedOrb]);

            spent = _currencyTestHelper.TestCurrency("LocalPhysicalDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);

            spent = _currencyTestHelper.TestCurrency("LocalLightningDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);
            Assert.AreEqual(3, equipment.Stats.Count);
        }

        [TestMethod]
        public void MasterCraftMultimodAddedSecondSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");
            equipment.Rarity = EquipmentRarity.Rare;

            var spent = _currencyTestHelper.TestCurrency("LocalPhysicalDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);

            spent = _currencyTestHelper.TestCurrency("ItemGenerationCanHaveMultipleCraftedMods0", equipment);
            Assert.AreEqual(2, spent[CurrencyNames.ExaltedOrb]);

            spent = _currencyTestHelper.TestCurrency("LocalLightningDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);
            Assert.AreEqual(3, equipment.Stats.Count);
        }

        [TestMethod]
        public void MasterCraftMultimodFourAffixesFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Two Hand Sword");
            equipment.Rarity = EquipmentRarity.Rare;

            var spent = _currencyTestHelper.TestCurrency("LocalPhysicalDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);

            spent = _currencyTestHelper.TestCurrency("ItemGenerationCanHaveMultipleCraftedMods0", equipment);
            Assert.AreEqual(2, spent[CurrencyNames.ExaltedOrb]);

            spent = _currencyTestHelper.TestCurrency("LocalLightningDamage1", equipment);
            Assert.AreEqual(4, spent[CurrencyNames.TransmuationOrb]);
            Assert.AreEqual(3, equipment.Stats.Count);

            spent = _currencyTestHelper.TestCurrency("LocalFireDamage1", equipment);
            Assert.IsFalse(spent.Any());
            Assert.AreEqual(3, equipment.Stats.Count);
        }
    }
}
