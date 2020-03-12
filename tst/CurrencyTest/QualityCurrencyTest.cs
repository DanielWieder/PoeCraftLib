using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace CurrencyTest
{
    [TestClass]
    public class QualityCurrencyTest
    {
        private readonly CurrencyTestHelper _currencyTestHelper;

        public QualityCurrencyTest()
        {
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void BlacksmithWhetstoneWeaponSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.BlacksmithsWhetstone, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.BlacksmithsWhetstone]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Default, equipment.QualityType);
        }

        [TestMethod]
        public void QualityCurrencyMagicRarityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            equipment.Rarity = EquipmentRarity.Magic;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.BlacksmithsWhetstone, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.BlacksmithsWhetstone]);
            Assert.AreEqual(2, equipment.Quality);
            Assert.AreEqual(QualityType.Default, equipment.QualityType);
        }

        [TestMethod]
        public void QualityCurrencyRareRarityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            equipment.Rarity = EquipmentRarity.Rare;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.BlacksmithsWhetstone, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.BlacksmithsWhetstone]);
            Assert.AreEqual(1, equipment.Quality);
            Assert.AreEqual(QualityType.Default, equipment.QualityType);
        }

        [TestMethod]
        public void QualityCurrencyUniqueRarityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            equipment.Rarity = EquipmentRarity.Unique;
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.BlacksmithsWhetstone, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.BlacksmithsWhetstone]);
            Assert.AreEqual(1, equipment.Quality);
            Assert.AreEqual(QualityType.Default, equipment.QualityType);
        }

        [TestMethod]
        public void BlacksmithWhetstoneArmourFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.BlacksmithsWhetstone, equipment);
            Assert.IsFalse(spent.Any());
        }

        [TestMethod]
        public void ArmourerScrapArmourSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ArmourersScrap, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.ArmourersScrap]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Default, equipment.QualityType);
        }

        [TestMethod]
        public void ArmourerScrapWeaponFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ArmourersScrap, equipment);
            Assert.IsFalse(spent.Any());
        }

        // Todo: Add Glassblower Bauble test after flasks are supported
        /*[TestMethod]
        public void GlassblowerBaubleSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Flask");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.GlassblowersBauble, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.GlassblowersBauble]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Default, equipment.QualityType);
        }

        [TestMethod]
        public void GlassblowerBaubleWeaponFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.GlassblowersBauble, equipment);
            Assert.IsFalse(spent.Any());
        }*/

        [TestMethod]
        public void CatalystAmuletSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.TurbulentCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
        }

        [TestMethod]
        public void CatalystRingSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.TurbulentCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
        }

        [TestMethod]
        public void CatalystRemovesDifferentCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);

            Assert.AreEqual(10, equipment.Quality);
            Assert.AreEqual(QualityType.ElementalDamage, equipment.QualityType);

            _currencyTestHelper.TestCurrency(CurrencyNames.PrismaticCatalyst, equipment);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Resistance, equipment.QualityType);
        }

        [TestMethod]
        public void CatalystQualityCapsAtTwentyTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.QualityType = QualityType.ElementalDamage;
            equipment.Quality = 18;
            _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);

            Assert.AreEqual(20, equipment.Quality);
            Assert.AreEqual(QualityType.ElementalDamage, equipment.QualityType);
        }

        [TestMethod]
        public void CatalystWeaponFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            Assert.IsFalse(spent.Any());
        }

        [TestMethod]
        public void CatalystArmourFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Helmet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            Assert.IsFalse(spent.Any());
        }

        [TestMethod]
        public void TurbulentCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.TurbulentCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.ElementalDamage, equipment.QualityType);
        }

        [TestMethod]
        public void ImbuedCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.ImbuedCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.ImbuedCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Caster, equipment.QualityType);
        }

        [TestMethod]
        public void AbrasiveCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.AbrasiveCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.AbrasiveCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Attack, equipment.QualityType);
        }

        [TestMethod]
        public void TemperingCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.TemperingCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.TemperingCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Defense, equipment.QualityType);
        }

        [TestMethod]
        public void FertileCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.FertileCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.FertileCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.LifeAndMana, equipment.QualityType);
        }

        [TestMethod]
        public void PrismaticCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.PrismaticCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.PrismaticCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Resistance, equipment.QualityType);
        }
        [TestMethod]
        public void IntrinsicCatalystQualityTypeTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.IntrinsicCatalyst, equipment);
            Assert.AreEqual(1, spent[CurrencyNames.IntrinsicCatalyst]);
            Assert.AreEqual(5, equipment.Quality);
            Assert.AreEqual(QualityType.Attribute, equipment.QualityType);
        }

        [TestMethod]
        public void QualityCurrencyFailsWhenFullTest()
        {
            var amulet = _currencyTestHelper.GetTestItemByItemClass("Amulet");
            var weapon = _currencyTestHelper.GetTestItemByItemClass("Dagger");
            var armour = _currencyTestHelper.GetTestItemByItemClass("Body Armour");

            amulet.Quality = 20;
            weapon.Quality = 20;
            armour.Quality = 20;

            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.BlacksmithsWhetstone, weapon);
            Assert.IsFalse(spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.ArmourersScrap, armour);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.ElementalDamage;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.TurbulentCatalyst, amulet);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.Caster;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.ImbuedCatalyst, amulet);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.Attack;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.AbrasiveCatalyst, amulet);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.Defense;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.TemperingCatalyst, amulet);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.LifeAndMana;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.FertileCatalyst, amulet);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.Resistance;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.PrismaticCatalyst, amulet);
            Assert.IsFalse(spent.Any());

            amulet.QualityType = QualityType.Attribute;
            spent = _currencyTestHelper.TestCurrency(CurrencyNames.IntrinsicCatalyst, amulet);
            Assert.IsFalse(spent.Any());

        }

    }
}
