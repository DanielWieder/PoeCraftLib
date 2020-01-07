using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CurrencyTest.Currency
{
    [TestClass]
    public class ChanceTest
    {
        private CurrencyTestHelper _currencyTestHelper;
        private Mock<IRandom> _random;

        private TestEquipmentFactory _factory;
        private ChanceOrb _chance;


        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);
            _chance = new ChanceOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void ChanceSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            var result = _chance.Execute(item, affixManager);
            Assert.IsTrue(result);

            Assert.IsTrue(item.Suffixes.Count >= 1);
            Assert.IsTrue(item.Prefixes.Count >= 1);
            Assert.AreNotEqual(EquipmentRarity.Normal, item.Rarity);
        }

        [TestMethod]
        public void ChanceCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedNormal(_chance));
        }

        [TestMethod]
        public void ChanceMagicFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnMagic(_chance));
        }

        [TestMethod]
        public void ChanceRareFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnRare(_chance));
        }
    }


}
