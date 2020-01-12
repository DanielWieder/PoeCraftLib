using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Simulator;
using PoeCraftLib.Simulator.Model.Crafting.Currency;
using PoeCraftLib.Simulator.Model.Crafting.Steps;
using PoeCraftLib.Simulator.Model.Simulation;

namespace PoeCraftLib.SimulatorTest
{
    [TestClass]
    public class CraftingSimulatorTests
    {
        private String alchemyName = "Orb of Alchemy";
        private string transmutationName = "Orb of Transmutation";
        private string annulmentName = "Orb of Annulment";
        private String essenceName = "Screaming Essence of Anger";
        private String fossilName = "Scorched Fossil";
        private string masterModName = "FireAndLightningResistance3";


        [TestMethod]
        public void AlchemyTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 10;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = alchemyName, Type = CraftingEventType.Currency }
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue( 10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(alchemyName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[alchemyName] );
        }

        [TestMethod]
        public void EssenceTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 100;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = essenceName, Type = CraftingEventType.Essence }
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(essenceName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[essenceName]);
        }

        [TestMethod]
        public void FossilTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 100;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = "Fossil", Children = new List<string> {fossilName}, Type = CraftingEventType.Fossil }
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(fossilName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[fossilName]);
        }

        [TestMethod]
        public void MasterCraftTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 100;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = transmutationName, Type = CraftingEventType.Currency }
                },
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = annulmentName, Type = CraftingEventType.Currency }
                },
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = annulmentName, Type = CraftingEventType.Currency }
                },
                new CraftingEventStep()
                {
                    CraftingEvent = new CraftingEvent() { Name = masterModName, Type = CraftingEventType.MasterCraft }
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(1 <= start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(annulmentName));
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(transmutationName));
            Assert.IsTrue(start.Result.CurrencyUsed.Any(x => x.Key != transmutationName && x.Key != annulmentName));
        }
    }
}
