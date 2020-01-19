using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using PoeCraftLib.Crafting;
using PoeCraftLib.Crafting.CraftingSteps;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;
using PoeCraftLib.Simulator.Model.Simulation;

namespace PoeCraftLib.Simulator
{
    public class CraftingSimulator
    {
        // Singleton
        private readonly ItemFactory _itemFactory = new ItemFactory();
        private readonly AffixFactory _affixFactory = new AffixFactory();
        private readonly CraftManager _craftingManager = new CraftManager();
        private readonly CurrencyValueFactory _currencyValueFactory = new CurrencyValueFactory();
        private readonly ConditionResolver _conditionResolution = new ConditionResolver();

        private readonly FossilFactory _fossilFactory = new FossilFactory();
        private readonly MasterModFactory _masterModFactory = new MasterModFactory();
        private readonly EssenceFactory _essenceFactory = new EssenceFactory();

        // Arguments
        private readonly SimFinanceInfo _financeInfo;
        private readonly SimCraftingInfo _craftingInfo;
        private readonly SimBaseItemInfo _baseItemInfo;

        // Stored
        private readonly AffixManager _affixManager;
        private readonly ItemBase _baseItem;
        private readonly Dictionary<string, double> _currencyValues;

        // Simulation Control
        private Task<SimulationArtifacts> _task;
        private readonly TimeSpan _timeout = TimeSpan.FromSeconds(60);
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        //Mappings
        private readonly IMapper _clientToDomain;
        private readonly IMapper _domainToClient;

        // Public
        public double Progress { get; set; } = 0;
        public SimulationStatus Status { get; set; } = SimulationStatus.Stopped;
        public delegate void ProgressUpdateEventHandler(ProgressUpdateEventArgs e);
        public ProgressUpdateEventHandler OnProgressUpdate;
        public delegate void SimulationCompleteEventHandler(SimulationCompleteEventArgs a);
        public SimulationCompleteEventHandler OnSimulationComplete;

        // Artifacts
        private SimulationArtifacts _simulationArtifacts { get; } = new SimulationArtifacts();

        public CraftingSimulator(
            SimBaseItemInfo baseItemInfo,
            SimFinanceInfo financeInfo,
            SimCraftingInfo craftingInfo)
        {
            var currencyFactory = new CurrencyFactory(
                new PoeRandom(), 
                _essenceFactory,
                _fossilFactory,
                _masterModFactory);

            var clientToDomainMapper = new ClientToDomainMapper(_itemFactory, currencyFactory);
            var domainToClientMapper = new DomainToClientMapper();

            _clientToDomain = clientToDomainMapper.GenerateMapper();
            _domainToClient = domainToClientMapper.GenerateMapper();

            _financeInfo = financeInfo;
            _craftingInfo = craftingInfo;
            _baseItemInfo = baseItemInfo;

            _baseItem = _itemFactory.Items.First(x => x.Name == _baseItemInfo.ItemName);

            var itemAffixes = _affixFactory.GetAffixesForItem(
                _baseItem.Tags,
                _baseItem.ItemClass,
                _baseItemInfo.ItemLevel,
                InfluenceToDomain(_baseItemInfo.Influence));

            var fossilAffixes = _fossilFactory.Fossils.SelectMany(x => x.AddedAffixes).ToList();
            _affixManager = new AffixManager(_baseItem, itemAffixes, fossilAffixes);
            _currencyValues = _currencyValueFactory.GetCurrencyValues(financeInfo.League);
        }

        public Task<SimulationArtifacts> Start()
        {
            if (Status != SimulationStatus.Stopped)
            {
                throw new InvalidOperationException("Invalid status. The simulation cannot be started. Create a new simulation");
            }

            Status = SimulationStatus.Running;
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(_timeout);
            var token = _cancellationTokenSource.Token;
            token.Register(() => Status = SimulationStatus.Cancelled);
            _task = new Task<SimulationArtifacts>(() => Run(token), token);
            _task.Start();
            return _task;
        }

        private SimulationArtifacts Run(CancellationToken ct)
        {
            // Pay for the first item
            _simulationArtifacts.CostInChaos += _baseItemInfo.ItemCost;

            for (Progress = 0; Progress < 100; Progress = GetSimulationProgress())
            {

                      // Craft item
                      var item = _itemFactory.ToEquipment(_baseItem, _baseItemInfo.ItemLevel, InfluenceToDomain(_baseItemInfo.Influence));
                      var results = _craftingManager.Craft(CraftingStepsToDomain(_craftingInfo.CraftingSteps), item, _affixManager, ct, _financeInfo.BudgetInChaos - _simulationArtifacts.CostInChaos);
      
                      bool saved = false;
                      // Update item results
                      foreach (var craftingTarget in _craftingInfo.CraftingTargets)
                      {
                          if (craftingTarget.Condition != null && _conditionResolution.IsValid(ConditionToDomain(craftingTarget.Condition), results.Result))
                          {
                              _simulationArtifacts.MatchingGeneratedItems[craftingTarget].Add(EquipmentToClient(results.Result));
                              saved = true;
                              break;
                          }
                      }
      
                      _simulationArtifacts.AllGeneratedItems.Add(EquipmentToClient(results.Result));
      
                      // Update crafting cost
                      foreach (var result in results.CraftingStepMetadata)
                      {
                          foreach (var currency in result.Value.CurrencyAmounts)
                          {
                              if (!_simulationArtifacts.CurrencyUsed.ContainsKey(currency.Key))
                              {
                                  _simulationArtifacts.CurrencyUsed.Add(currency.Key, 0);
                              }
      
                              int amountOfCurrencyUsed = currency.Value * result.Value.TimesModified;
                              _simulationArtifacts.CurrencyUsed[currency.Key] += amountOfCurrencyUsed;
                              _simulationArtifacts.CostInChaos += amountOfCurrencyUsed * _currencyValues[currency.Key];
                          }
                      }
      
                      if (GetSimulationProgress() < 100)
                      {
                          // Get a new item ready. 
                          if (saved ||
                              results.Result.Corrupted ||
                              results.Result.Rarity != EquipmentRarity.Normal &&
                              _baseItemInfo.ItemCost <= _currencyValues[CurrencyNames.ScouringOrb])
                          {
                              _simulationArtifacts.CostInChaos += _baseItemInfo.ItemCost;
                          }
                          else if (results.Result.Rarity != EquipmentRarity.Normal)
                          {
                              _simulationArtifacts.CostInChaos += _currencyValues[CurrencyNames.ScouringOrb];
                          }
                      }

                // Update progress results
                if (OnProgressUpdate != null)
                {
                    var args = new ProgressUpdateEventArgs
                    {
                        Progress = Progress
                    };

                    OnProgressUpdate(args);
                }
            }

            if (OnSimulationComplete != null)
            {
                Status = SimulationStatus.Completed;

                var args = new SimulationCompleteEventArgs
                {
                    SimulationArtifacts = _simulationArtifacts
                };

                OnSimulationComplete(args);
            }

            return _simulationArtifacts;
        }

        private double GetSimulationProgress()
        {
            return _simulationArtifacts.CostInChaos / _financeInfo.BudgetInChaos * 100;
        }

        public void Cancel()
        {
            if (_task != null && _task.Status == TaskStatus.Running)
            {
                _cancellationTokenSource.Cancel();
            }
        }
        private List<Influence> InfluenceToDomain(List<Model.Items.Influence> influence)
        {
            return influence.Select(x => _clientToDomain.Map<Model.Items.Influence, Entities.Items.Influence>(x)).ToList();
        }

        private List<ICraftingStep> CraftingStepsToDomain(List<Model.Crafting.Steps.ICraftingStep> craftingSteps)
        {
            return craftingSteps.Select(x => _clientToDomain.Map<Model.Crafting.Steps.ICraftingStep, Crafting.CraftingSteps.ICraftingStep>(x)).ToList();
        }

        private CraftingCondition ConditionToDomain(Model.Crafting.CraftingCondition condition)
        {
            return _clientToDomain.Map<Model.Crafting.CraftingCondition, Entities.Crafting.CraftingCondition>(condition);
        }

        private Model.Items.Equipment EquipmentToClient(Entities.Items.Equipment equipment)
        {
            return _domainToClient.Map<Entities.Items.Equipment, Model.Items.Equipment>(equipment);
        }
    }

    public class SimulationCompleteEventArgs
    {
        public SimulationArtifacts SimulationArtifacts { get; set; }
    }

    public class ProgressUpdateEventArgs
    {
        public double Progress { get; set; }
    }
}
