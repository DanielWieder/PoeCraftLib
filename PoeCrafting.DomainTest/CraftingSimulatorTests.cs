using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataJson.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCrafting.CraftingSim;
using PoeCrafting.CraftingSim.CraftingSteps;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Data;
using PoeCrafting.Domain;
using PoeCrafting.Domain.Entities;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;

namespace PoeCrafting.DomainTest
{
    [TestClass]
    public class CraftingSimulatorTests
    {
        [TestMethod]
        public void TestBasicSimulation()
        {

            IRandom random = new PoeRandom();

            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 1000;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CurrencyCraftingStep(new AlchemyOrb(random))
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue( 1000 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(1000 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(CurrencyNames.AlchemyOrb));
            Assert.IsTrue(1000 < start.Result.CurrencyUsed[CurrencyNames.AlchemyOrb] );

        }
    }
}
