using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;

namespace PoeCraftLib.CurrencyTest.Currency
{
    [TestClass]
    public class ExaltedTest
    {
        private CurrencyTestHelper _currencyTestHelper;
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private ExaltedOrb _exalted;
        private RegalOrb _regal;
        private TransmutationOrb _transmutation;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _alchemy = new AlchemyOrb(_random.Object);
            _exalted = new ExaltedOrb(_random.Object);
            _regal = new RegalOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void ExaltedSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);

            _transmutation.Execute(item, affixManager);
            _regal.Execute(item, affixManager);

            var previousAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            var result = _exalted.Execute(item, affixManager);
            Assert.IsTrue(result);
            var currentAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(previousAffixCount + 1, currentAffixCount);
        }

        [TestMethod]
        public void ExaltedTooManyAffixesTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);

            _alchemy.Execute(item, affixManager);
            _exalted.Execute(item, affixManager);
            _exalted.Execute(item, affixManager);
            _exalted.Execute(item, affixManager);
            
            var result = _exalted.Execute(item, affixManager);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ExaltedCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedRare(_exalted));
        }

        [TestMethod]
        public void ExaltedMagicFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnMagic(_exalted));
        }

        [TestMethod]
        public void ExaltedNormalFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_exalted));
        }
    }
}
