using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCrafting.Currency;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Entities;

namespace PoeCrafting.CurrencyTest.Currency
{
    [TestClass]
    public class AugmentationTest
    {
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private AnullmentOrb _anullment;
        private AugmentationOrb _augmentation;
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
            _anullment = new AnullmentOrb(_random.Object);
            _augmentation = new AugmentationOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);
            _vaal = new VaalOrb(_random.Object);
            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void AugmentationSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            if (item.Prefixes.Count + item.Suffixes.Count == 2)
            {
                _anullment.Execute(item, affixManager);
            }
            Assert.AreEqual(1, item.Prefixes.Count + item.Suffixes.Count);

            var result = _augmentation.Execute(item, affixManager);
            Assert.IsTrue(result);
            Assert.AreEqual(1, item.Prefixes.Count);
            Assert.AreEqual(1, item.Suffixes.Count);
        }

        [TestMethod]
        public void AugmentationInvalidNormalTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_augmentation));
        }

        [TestMethod]
        public void AugmentationInvalidRareTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnRare(_augmentation));
        }

        [TestMethod]
        public void AugmentationInvalidCorruptionTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedNormal(_augmentation));
        }
    }
}
