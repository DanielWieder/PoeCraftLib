using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest.Currency
{
    [TestClass]
    public class VaalTest
    {
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;
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
            _vaal = new VaalOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        // Currently 

        [TestMethod]
        public void VaalNormalTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            var result = _vaal.Execute(item, affixManager);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Corrupted);
        }

        [TestMethod]
        public void VaalMagicTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            var result = _vaal.Execute(item, affixManager);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Corrupted);
        }

        [TestMethod]
        public void VaalRareTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _alchemy.Execute(item, affixManager);
            var result = _vaal.Execute(item, affixManager);
            Assert.IsTrue(result);
            Assert.IsTrue(item.Corrupted);
        }

        [TestMethod]
        public void VaalCorruptionFailureTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedMagic(_vaal));
        }
        
    }
}