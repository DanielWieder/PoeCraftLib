using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CurrencyTest.Currency
{
    [TestClass]
    public class ScouringTest
    {
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private ScouringOrb _scouring;
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
            _scouring = new ScouringOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void ScouringRareSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);

            _alchemy.Execute(item, affixManager);
            var result = _scouring.Execute(item, affixManager);
            Assert.IsTrue(result);

            Assert.AreEqual(0, item.Suffixes.Count);
            Assert.AreEqual(0, item.Prefixes.Count);
            Assert.AreEqual(EquipmentRarity.Normal, item.Rarity);
        }

        [TestMethod]
        public void ScouringMagicSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            var result = _scouring.Execute(item, affixManager);
            Assert.IsTrue(result);

            Assert.AreEqual(0, item.Suffixes.Count);
            Assert.AreEqual(0, item.Prefixes.Count);
            Assert.AreEqual(EquipmentRarity.Normal, item.Rarity);
        }

        [TestMethod]
        public void ScouringNormalFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_scouring));
        }

        [TestMethod]
        public void ScouringCorruptionRareFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedRare(_scouring));
        }

        [TestMethod]
        public void ScouringCorruptionMagicFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedMagic(_scouring));
        }
    }
}