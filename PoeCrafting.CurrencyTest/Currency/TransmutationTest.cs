using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CurrencyTest.Currency
{
    [TestClass]
    public class TransmutationTest
    {
        private Mock<IRandom> _random;

        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;
        private CurrencyTestHelper _currencyTestHelper;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _transmutation = new TransmutationOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void TransmutationSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);

            var result = _transmutation.Execute(item, affixManager);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Suffixes.Count + item.Prefixes.Count >= 1);
            Assert.AreEqual(EquipmentRarity.Magic, item.Rarity);
        }

        [TestMethod]
        public void TransmutationInvalidMagicTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnMagic(_transmutation));
        }

        [TestMethod]
        public void TransmutationInvalidRareTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnRare(_transmutation));
        }

        [TestMethod]
        public void TransmutationInvalidCorruptionTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedNormal(_transmutation));
        }
    }
}
