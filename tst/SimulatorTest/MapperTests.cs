using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Simulator;

namespace PoeCraftLib.SimulatorTest
{
    [TestClass]
    public class MapperTests
    {
        private readonly FossilFactory _fossilFactory;
        private readonly MasterModFactory _masterModFactory;
        private readonly EssenceFactory _essenceFactory;
        private readonly ItemFactory _itemFactory;
        public MapperTests()
        {
            AffixFactory affixFactory = new AffixFactory();
            _itemFactory = new ItemFactory(affixFactory);
            _fossilFactory = new FossilFactory(affixFactory);
            _masterModFactory = new MasterModFactory(affixFactory, _itemFactory);
            _essenceFactory = new EssenceFactory(_itemFactory, affixFactory);
        }

        [TestMethod]
        public void ClientToDomainMapperTest()
        {
            var currencyFactory = new CurrencyFactory(
                new PoeRandom(),
                _essenceFactory,
                _fossilFactory,
                _masterModFactory);

            var mapper = new ClientToDomainMapper(_itemFactory, currencyFactory);
            var config = mapper.GenerateMapper();
            config.ConfigurationProvider.AssertConfigurationIsValid();
        }

        [TestMethod]
        public void DomainToClientMapperTest()
        {
            var mapper = new DomainToClientMapper();
            var config = mapper.GenerateMapper();
            config.ConfigurationProvider.AssertConfigurationIsValid();
        }
    }
}
