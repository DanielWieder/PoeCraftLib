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
    public class FossilTest
    {
        private readonly CurrencyTestHelper _currencyTestHelper;

        public FossilTest()
        {
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void FossilSuccessTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var spent = _currencyTestHelper.TestCurrency("Aberrant Fossil", equipment);

            Assert.AreEqual(1, spent["Aberrant Fossil"]);
            Assert.AreEqual(1, spent["Primitive Alchemical Resonator"]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);

            var currency = _currencyTestHelper.CurrencyFactory.GetCurrencyByName("Aberrant Fossil") as PoeCraftLib.Currency.Currency.Currency;

            Assert.IsFalse(currency.CurrencyModifiers.RollsLucky);
            Assert.AreEqual(0, currency.CurrencyModifiers.ExplicitWeightModifiers["lightning"]);
            Assert.AreEqual(10, currency.CurrencyModifiers.ExplicitWeightModifiers["chaos"]);
            Assert.AreEqual(5, currency.CurrencyModifiers.ExplicitWeightModifiers["poison"]);
            Assert.IsFalse(spent.ContainsKey(CurrencyNames.ScouringOrb));
            Assert.IsTrue(currency.CurrencyModifiers.AddedExplicits.Count > 0);
            Assert.AreEqual(100, currency.CurrencyModifiers.ItemLevelRestriction);
        }

        [TestMethod]
        public void FossilUsesScouringWhenRareTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            _currencyTestHelper.TestCurrency(CurrencyNames.AlchemyOrb, equipment);
            var spent = _currencyTestHelper.TestCurrency("Aberrant Fossil", equipment);

            Assert.AreEqual(1, spent["Aberrant Fossil"]);
            Assert.AreEqual(1, spent["Primitive Alchemical Resonator"]);
            Assert.AreEqual(1, spent[CurrencyNames.ScouringOrb]);
            Assert.AreEqual(EquipmentRarity.Rare, equipment.Rarity);
            Assert.IsTrue(equipment.Stats.Count >= 4);
        }

        [TestMethod]
        public void FossilCombinedModifiersDoubleFossilTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var currency = _currencyTestHelper.CurrencyFactory.GetFossilCraftByNames(new List<string>() { "Scorched Fossil", "Frigid Fossil" }) as PoeCraftLib.Currency.Currency.Currency;
            var affixManager = _currencyTestHelper.GetAffixManager(equipment);
            var spent = currency.Execute(equipment, affixManager);

            Assert.AreEqual(1, spent["Scorched Fossil"]);
            Assert.AreEqual(1, spent["Frigid Fossil"]);
            Assert.AreEqual(1, spent["Potent Alchemical Resonator"]);
            Assert.IsFalse(currency.CurrencyModifiers.RollsLucky);
            Assert.AreEqual(0, currency.CurrencyModifiers.ExplicitWeightModifiers["cold"]);
            Assert.AreEqual(0, currency.CurrencyModifiers.ExplicitWeightModifiers["fire"]);
            Assert.AreEqual(100, currency.CurrencyModifiers.ItemLevelRestriction);
            Assert.IsTrue(currency.CurrencyModifiers.AddedExplicits.Count > 0);
        }

        [TestMethod]
        public void FossilRollsLuckyModifierTest()
        {
            var currency = _currencyTestHelper.CurrencyFactory.GetCurrencyByName("Sanctified Fossil") as PoeCraftLib.Currency.Currency.Currency;
            Assert.IsTrue(currency.CurrencyModifiers.RollsLucky);
        }

        [TestMethod]
        public void FossilCombinedModifierRollsLuckyTest()
        {
            var currency = _currencyTestHelper.CurrencyFactory.GetFossilCraftByNames(new List<string>() { "Sanctified Fossil", "Frigid Fossil" }) as PoeCraftLib.Currency.Currency.Currency;
            Assert.IsTrue(currency.CurrencyModifiers.RollsLucky);
        }

        [TestMethod]
        public void FossilSingleCorruptedEssenceTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();
            var spent = _currencyTestHelper.TestCurrency("Glyphic Fossil", equipment);

            Assert.AreEqual(1, equipment.Stats.Count(x => x.Affix.Name == "of the Essence" || x.Affix.Name == "Essences"));
        }

        [TestMethod]
        public void FossilSingleCorruptedEssenceChanceTest()
        {
            var equipment = _currencyTestHelper.GetTestItem();

            bool hasEssence = false;
            bool missingEssence = false;

            var currency = _currencyTestHelper.CurrencyFactory.GetCurrencyByName("Tangled Fossil");
            var affixManager = _currencyTestHelper.GetAffixManager(equipment);

            // The chance for this to fail to add an affix 100 times in a row due to chance should be less than .01%
            // https://anydice.com/program/1a570
            for (int i = 0; i < 100; i++)
            {
                var spent = currency.Execute(equipment, affixManager);
                bool hasAffix = equipment.Stats.Count(x => x.Affix.Name == "of the Essence" || x.Affix.Name == "Essences") > 0;

                if (hasAffix)
                {
                    hasEssence = true;
                }
                else
                {
                    missingEssence = true;
                }

                if (hasEssence && missingEssence)
                {
                    break;
                }

            }

            Assert.IsTrue(hasEssence && missingEssence);
        }
    }
}
