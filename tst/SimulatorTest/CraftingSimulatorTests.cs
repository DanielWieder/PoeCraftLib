using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Simulator;
using PoeCraftLib.Simulator.Model.Crafting;
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
                new CurrencyCraftingStep()
                {
                    Name = alchemyName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(alchemyName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[alchemyName]);
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
                new CurrencyCraftingStep()
                {
                    Name = essenceName
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
                new CurrencyCraftingStep()
                {
                    Name = "Fossil", SocketedCurrency = new List<string> {fossilName}
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
                new CurrencyCraftingStep()
                {
                    Name = transmutationName
                },
                new CurrencyCraftingStep()
                {
                    Name = annulmentName
                },
                new CurrencyCraftingStep()
                {
                    Name = annulmentName
                },
                new CurrencyCraftingStep()
                {
                    Name = masterModName
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

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void InfiniteRecursionTest() {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 100;

            var recursiveCraftingStep = new WhileCraftingStep();
            recursiveCraftingStep.Children.Add(recursiveCraftingStep);

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                recursiveCraftingStep,
                new CurrencyCraftingStep()
                {
                    Name = transmutationName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var task = craftingSimulator.Start();
            task.Wait();

            throw task.Exception;

        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void NoCurrencyTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 100;

            var flowControlCraftingStep = new WhileCraftingStep();

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                flowControlCraftingStep,
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var task = craftingSimulator.Start();
            task.Wait();

            throw task.Exception;
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void TimeoutTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 10000000;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CurrencyCraftingStep()
                {
                    Name = alchemyName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start(.01);

            start.Wait();
        }

        [TestMethod]
        public void ProgressTest()
        {
            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Fencer Helm";
            financeInfo.BudgetInChaos = 100;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CurrencyCraftingStep()
                {
                    Name = alchemyName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            double previousProgress = 0;

            List<double> progressList = new List<double>();

            craftingSimulator.OnProgressUpdate = (x) =>
            {
                Assert.IsTrue(x.Progress > previousProgress);
                progressList.Add(x.Progress);
                previousProgress = x.Progress;
            };

            var start = craftingSimulator.Start(10000);

            start.Wait();
            Assert.AreEqual(progressList.Count, 100);
        }

        [TestMethod]
        public void MatchingItemsTest()
        {
            String targetName = "Total ES Test";

            SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
            SimCraftingInfo craftingInfo = new SimCraftingInfo();
            SimFinanceInfo financeInfo = new SimFinanceInfo();

            baseItemInfo.ItemName = "Vaal Regalia";
            financeInfo.BudgetInChaos = 1000;

            craftingInfo.CraftingSteps = new List<ICraftingStep>()
            {
                new CurrencyCraftingStep()
                {
                    Name = alchemyName
                }
            };

            craftingInfo.CraftingTargets = new List<CraftingTarget>()
            {
                new CraftingTarget()
                {
                    Name = targetName,
                    Condition = new CraftingCondition()
                    {
                        CraftingSubConditions = new List<CraftingSubcondition>()
                        {
                            new CraftingSubcondition()
                            {
                                ValueType = StatValueType.Flat,
                                AggregateType = SubconditionAggregateType.And,
                                MetaConditions = new List<ConditionAffix>()
                                {
                                    new ConditionAffix()
                                    {
                                        ModType = "Total Energy Shield",
                                        Min1 = 200
                                    }
                                }
                            }
                        }
                    }
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.MatchingGeneratedItems[targetName].Any());
        }
    }
}
