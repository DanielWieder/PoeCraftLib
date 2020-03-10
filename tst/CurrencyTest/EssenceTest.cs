using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    public class EssenceTest
    {
        private string lowLevelEssenceName = "Muttering Essence of Anger";
        private string highLevelEssenceName = "Shrieking Essence of Anger";

        private string lowLevelEssenceProperty = "FireResist2";
        private string highLevelEssenceProperty = "FireResist7";

        private readonly CurrencyTestHelper _currencyTestHelper;

        public EssenceTest()
        {
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void EssenceSuccessTest()
        {
            int levelRestriction = 45;

            var equipment = _currencyTestHelper.GetTestItemByItemClass("Helmet");
            var spent = _currencyTestHelper.TestCurrency(lowLevelEssenceName, equipment);

            Assert.AreEqual(1, spent[lowLevelEssenceName]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
            Assert.IsTrue(equipment.Stats.Where(x => x.Affix.FullName != lowLevelEssenceProperty).All(x => x.Affix.RequiredLevel <= levelRestriction));
            Assert.IsTrue(equipment.Stats.Any(x => x.Affix.FullName == lowLevelEssenceProperty));
        }

        [TestMethod]
        public void LowLevelEssenceRarityRareFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Helmet");

            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(lowLevelEssenceName, equipment);

            Assert.IsTrue(!spent.Any());
        }

        [TestMethod]
        public void HighLevelEssenceRarityRareFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Helmet");

            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(highLevelEssenceName, equipment);

            Assert.AreEqual(1, spent[highLevelEssenceName]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
            Assert.IsTrue(equipment.Stats.Any(x => x.Affix.FullName == highLevelEssenceProperty));
        }

        [TestMethod]
        public void EssenceRarityNormalFailureTest()
        {
            var equipment = _currencyTestHelper.GetTestItemByItemClass("Helmet");

            _currencyTestHelper.TestCurrency(CurrencyNames.TransmuationOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency(lowLevelEssenceName, equipment);
            Assert.IsTrue(!spent.Any());
            spent = _currencyTestHelper.TestCurrency(highLevelEssenceName, equipment);
            Assert.IsTrue(!spent.Any());
        }
    }
}
