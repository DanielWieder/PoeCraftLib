using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CurrencyTest.Currency
{
    [TestClass]
    public class RegalTest
    {
        private CurrencyTestHelper _currencyTestHelper;
        private Mock<IRandom> _random;

        private TestEquipmentFactory _factory;
        private RegalOrb _regal;
        private TransmutationOrb _transmutation;

        [TestInitialize]
        public void Initialize()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _regal = new RegalOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);

            _currencyTestHelper = new CurrencyTestHelper();
        }

        [TestMethod]
        public void RegalSuccessTest()
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = _currencyTestHelper.CreateAffixManager(item.ItemBase);

            _transmutation.Execute(item, affixManager);
            var previousAffixCount = item.Prefixes.Count + item.Suffixes.Count;

            var result = _regal.Execute(item, affixManager);
            Assert.IsTrue(result);
            var currentAffixCount = item.Prefixes.Count + item.Suffixes.Count;
            Assert.AreEqual(previousAffixCount + 1, currentAffixCount);
            Assert.AreEqual(EquipmentRarity.Rare, item.Rarity);
        }

        [TestMethod]
        public void RegalInvalidNormalTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnNormal(_regal));
        }

        [TestMethod]
        public void RegalInvalidRareTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnRare(_regal));
        }

        [TestMethod]
        public void RegalInvalidCorruptionTest()
        {
            Assert.IsFalse(_currencyTestHelper.CanUseOnCorruptedMagic(_regal));
        }
    }
}
