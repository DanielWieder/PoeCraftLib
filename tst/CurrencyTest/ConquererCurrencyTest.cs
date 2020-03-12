using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace CurrencyTest
{
    [TestClass]
    public class ConquererCurrencyTest
    {
        private readonly CurrencyTestHelper _currencyTestHelper;

        public ConquererCurrencyTest()
        {
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void HunterCurrencySuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.HuntersOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.HuntersOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 3 && equipment.Stats.Count <= 4);
            Assert.AreEqual(1, equipment.Stats.Count(x => x.Affix.SpawnWeights.ContainsKey("body_armour_basilisk")));
        }

        [TestMethod]
        public void CrusaderCurrencySuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.CrusadersOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.CrusadersOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 3 && equipment.Stats.Count <= 4);
            Assert.AreEqual(1, equipment.Stats.Count(x => x.Affix.SpawnWeights.ContainsKey("body_armour_crusader")));
        }

        [TestMethod]
        public void WarlordCurrencySuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.WarlordsOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.WarlordsOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 3 && equipment.Stats.Count <= 4);
            Assert.AreEqual(1, equipment.Stats.Count(x => x.Affix.SpawnWeights.ContainsKey("body_armour_adjudicator")));
        }

        [TestMethod]
        public void RedeemerCurrencySuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.RegalOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.RedeemersOrb, equipment);

            Assert.AreEqual(1, spent[CurrencyNames.RedeemersOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 3 && equipment.Stats.Count <= 4);
            Assert.AreEqual(1, equipment.Stats.Count(x => x.Affix.SpawnWeights.ContainsKey("body_armour_eyrie")));
        }

        [TestMethod]
        public void HunterReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            equipment.Rarity = EquipmentRarity.Rare;

            _currencyTestHelper.TestCurrency(CurrencyNames.HuntersOrb, equipment);

            Assert.AreEqual(0, equipment.Quality);
        }

        [TestMethod]
        public void WarlordReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            equipment.Rarity = EquipmentRarity.Rare;

            _currencyTestHelper.TestCurrency(CurrencyNames.WarlordsOrb, equipment);

            Assert.AreEqual(0, equipment.Quality);
        }

        [TestMethod]
        public void RedeemerReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            equipment.Rarity = EquipmentRarity.Rare;

            _currencyTestHelper.TestCurrency(CurrencyNames.RedeemersOrb, equipment);

            Assert.AreEqual(0, equipment.Quality);
        }

        [TestMethod]
        public void CrusaderReducesCatalystQualityTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Ring");
            equipment.Quality = 20;
            equipment.QualityType = QualityType.Resistance;
            equipment.Rarity = EquipmentRarity.Rare;

            _currencyTestHelper.TestCurrency(CurrencyNames.CrusadersOrb, equipment);

            Assert.AreEqual(0, equipment.Quality);
        }

        [TestMethod]
        public void ConquerersCurrencyUsesQualityTest()
        {
            Currency hunter = _currencyTestHelper.CurrencyFactory.GetCurrencyByName(CurrencyNames.HuntersOrb) as Currency;
            Assert.IsTrue(hunter.CurrencyModifiers.QualityAffectsExplicitOdds);

            Currency warlord = _currencyTestHelper.CurrencyFactory.GetCurrencyByName(CurrencyNames.WarlordsOrb) as Currency;
            Assert.IsTrue(warlord.CurrencyModifiers.QualityAffectsExplicitOdds);

            Currency crusader = _currencyTestHelper.CurrencyFactory.GetCurrencyByName(CurrencyNames.CrusadersOrb) as Currency;
            Assert.IsTrue(crusader.CurrencyModifiers.QualityAffectsExplicitOdds);

            Currency redeemer = _currencyTestHelper.CurrencyFactory.GetCurrencyByName(CurrencyNames.RedeemersOrb) as Currency;
            Assert.IsTrue(crusader.CurrencyModifiers.QualityAffectsExplicitOdds);
        }

        [TestMethod]
        public void ConquerersCurrencyFailsRarityNormalMagicTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.RedeemersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.CrusadersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.HuntersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.WarlordsOrb, equipment);
            Assert.IsTrue(!spent.Any());

            equipment.Rarity = EquipmentRarity.Magic;

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.RedeemersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.CrusadersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.HuntersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.WarlordsOrb, equipment);
            Assert.IsTrue(!spent.Any());
        }

        [TestMethod]
        public void ConquerersCurrencyFailsFullAffixesTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);
            _currencyTestHelper.TestCurrency(CurrencyNames.ExaltedOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.RedeemersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.CrusadersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.HuntersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.WarlordsOrb, equipment);
            Assert.IsTrue(!spent.Any());
        }

        [TestMethod]
        public void ConquerersCurrencyFailsWithAnyInfluenceTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Body Armour");
            equipment.Rarity = EquipmentRarity.Rare;
            equipment.Influence.Add(Influence.Elder);

            var spent = _currencyTestHelper.TestCurrency(CurrencyNames.RedeemersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.CrusadersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.HuntersOrb, equipment);
            Assert.IsTrue(!spent.Any());

            spent = _currencyTestHelper.TestCurrency(CurrencyNames.WarlordsOrb, equipment);
            Assert.IsTrue(!spent.Any());
        }
    }
}
