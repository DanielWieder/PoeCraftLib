using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Simulator;

namespace PoeCraftLib.SimulatorTest
{
    [TestClass]
    public class MapperTests
    {
        private readonly FossilFactory _fossilFactory = new FossilFactory();
        private readonly MasterModFactory _masterModFactory = new MasterModFactory();
        private readonly EssenceFactory _essenceFactory = new EssenceFactory();

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
