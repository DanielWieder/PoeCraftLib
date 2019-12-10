using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CurrencyTest.Currency
{
    [TestClass]
    public class AlchemyTest
    {
        private AlchemyOrb _alchemy;
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

            _alchemy = new AlchemyOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void AlchemySuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            var result = _alchemy.Execute(item, affixManager);
            Assert.IsTrue(result);

            Assert.IsTrue(item.Suffixes.Count >= 1);
            Assert.IsTrue(item.Prefixes.Count >= 1);
            Assert.AreEqual(EquipmentRarity.Rare, item.Rarity);
        }

        [TestMethod]
        public void AlchemyNormalSuccessTest()
        {
            Assert.IsTrue(_currencyTestHelper.CanUseOnNormal(_alchemy));
        }

        [TestMethod]
        public void AlchemyMagicFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnMagic(_alchemy));
        }

        [TestMethod]
        public void AlchemyRareFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnRare(_alchemy));
        }

        [TestMethod]
        public void AlchemyCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedNormal(_alchemy));
        }
    }


}
