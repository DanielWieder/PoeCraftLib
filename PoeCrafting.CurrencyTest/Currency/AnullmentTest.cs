using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;

namespace PoeCraftLib.CurrencyTest.Currency
{
    [TestClass]
    public class AnullmentTest
    {
        private AlchemyOrb _alchemy;
        private Mock<IRandom> _random;
        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;
        private AnullmentOrb _anullment;
        private VaalOrb _vaal;
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
            _anullment = new AnullmentOrb(_random.Object);
            _vaal = new VaalOrb(_random.Object);
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void AnullmentMagicTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            var oldAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            var result = _anullment.Execute(item, affixManager);
            Assert.IsTrue(result);
            var newAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(oldAffixCount, newAffixCount + 1);
        }

        [TestMethod]
        public void AnullmentRareTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _alchemy.Execute(item, affixManager);
            var oldAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            var result = _anullment.Execute(item, affixManager);
            Assert.IsTrue(result);
            var newAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(oldAffixCount, newAffixCount + 1);
        }

        [TestMethod]
        public void AnullmentInvalidNormalTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_anullment));
        }

        [TestMethod]
        public void AnullmentInvalidCorruptionMagicTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedRare(_anullment));
        }

        [TestMethod]
        public void AnullmentInvalidCorruptionRareTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedMagic(_anullment));
        }

        [TestMethod]
        public void AnullmentInvalidNoAffixesTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            _anullment.Execute(item, affixManager);
            _anullment.Execute(item, affixManager);
            var result = _anullment.Execute(item, affixManager);
            Assert.IsFalse(result);
        }
    }
}
