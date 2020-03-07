using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly String _alchemyName = "Orb of Alchemy";
        private readonly string _transmutationName = "Orb of Transmutation";
        private readonly string _annulmentName = "Orb of Annulment";
        private readonly String _essenceName = "Screaming Essence of Anger";
        private readonly String _fossilName = "Scorched Fossil";
        private readonly string _masterModName = "FireResistance2";


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
                    Name = _alchemyName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(_alchemyName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[_alchemyName]);
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
                    Name = _essenceName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(_essenceName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[_essenceName]);
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
                    Name = "Fossil", SocketedCurrency = new List<string> {_fossilName}
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(10 < start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(_fossilName));
            Assert.IsTrue(10 < start.Result.CurrencyUsed[_fossilName]);
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
                    Name = _transmutationName
                },
                new CurrencyCraftingStep()
                {
                    Name = _annulmentName
                },
                new CurrencyCraftingStep()
                {
                    Name = _annulmentName
                },
                new CurrencyCraftingStep()
                {
                    Name = _masterModName
                }
            };

            CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

            var start = craftingSimulator.Start();

            start.Wait();

            Assert.IsTrue(start.Result.AllGeneratedItems.All(x => x != null));
            Assert.IsTrue(1 <= start.Result.AllGeneratedItems.Count);
            Assert.IsTrue(10 <= start.Result.CostInChaos);
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(_annulmentName));
            Assert.IsTrue(start.Result.CurrencyUsed.ContainsKey(_transmutationName));
            Assert.IsTrue(start.Result.CurrencyUsed.Any(x => x.Key != _transmutationName && x.Key != _annulmentName));
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
                    Name = _transmutationName
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
                    Name = _alchemyName
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
                    Name = _alchemyName
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
                    Name = _alchemyName
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
