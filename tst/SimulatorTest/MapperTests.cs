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

        public MapperTests()
        {
            ItemFactory itemFactory = new ItemFactory();
            AffixFactory affixFactory = new AffixFactory();
            _fossilFactory = new FossilFactory(affixFactory);
            _masterModFactory = new MasterModFactory(affixFactory, itemFactory);
            _essenceFactory = new EssenceFactory(itemFactory, affixFactory);
        }

        [TestMethod]
        public void ClientToDomainMapperTest()
        {
            var currencyFactory = new CurrencyFactory(
                new PoeRandom(),
                _essenceFactory,
                _fossilFactory,
                _masterModFactory);

            var itemFactory = new ItemFactory();

            var mapper = new ClientToDomainMapper(itemFactory, currencyFactory);
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
