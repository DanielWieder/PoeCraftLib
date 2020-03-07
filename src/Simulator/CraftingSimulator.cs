using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using PoeCraftLib.Crafting;
using PoeCraftLib.Crafting.CraftingSteps;
using PoeCraftLib.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;
using PoeCraftLib.Simulator.Model.Simulation;
using CraftingCondition = PoeCraftLib.Entities.Crafting.CraftingCondition;
using Equipment = PoeCraftLib.Simulator.Model.Items.Equipment;

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

        private readonly FossilFactory _fossilFactory;
        private readonly MasterModFactory _masterModFactory;
        private readonly EssenceFactory _essenceFactory;

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
        private const int DefaultTimeout = 60;
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
        ItemFactory itemFactory = new ItemFactory();
        AffixFactory affixFactory = new AffixFactory();
        _fossilFactory = new FossilFactory(affixFactory);
        _masterModFactory = new MasterModFactory(affixFactory, itemFactory);
        _essenceFactory = new EssenceFactory(itemFactory, affixFactory);

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

        public Task<SimulationArtifacts> Start(CancellationToken token)
        {
            if (Status != SimulationStatus.Stopped)
            {
                throw new InvalidOperationException("Invalid status. The simulation cannot be started. Create a new simulation");
            }

            Status = SimulationStatus.Running;

            token.Register(() => Status = SimulationStatus.Cancelled);
            _task = new Task<SimulationArtifacts>(() => Run(token), token);
            _task.Start();
            return _task;
        }

        public Task<SimulationArtifacts> Start(double timeout = DefaultTimeout)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(timeout));
            var token = _cancellationTokenSource.Token;

            return Start(token);
        }

        private SimulationArtifacts Run(CancellationToken ct)
        {
            // Check for recursive/duplicate crafting steps
            if (RecursionCheck(new HashSet<Model.Crafting.Steps.ICraftingStep>(),  _craftingInfo.CraftingSteps))
            {
                throw new ArgumentException("Crafting steps are infinitely recursive");
            }

            // Pay for the first item
            _simulationArtifacts.CostInChaos += _baseItemInfo.ItemCost;

            double previousProgress = -1;

            var baseInfluence = InfluenceToDomain(_baseItemInfo.Influence);
            var craftingSteps = CraftingStepsToDomain(_craftingInfo.CraftingSteps);
            var craftingTargets = _craftingInfo.CraftingTargets.Select(x =>

                new PoeCraftLib.Entities.Items.CraftingTarget()
                {
                    Name = x.Name,
                    Value = x.Value,
                    Condition = ConditionToDomain(x.Condition)
                }).ToList();

            for (ProgressManager progressManager = GetProgressManager(); progressManager.Progress < 100; previousProgress = progressManager.Progress)
            {

                // Craft item
                var item = _itemFactory.ToEquipment(_baseItem, _baseItemInfo.ItemLevel, baseInfluence);
                      var results = _craftingManager.Craft(craftingSteps, item, _affixManager, ct, progressManager);
      
                      bool saved = false;
  
                      // No normal items are evaluated since that would cause a lot of clutter
                      if (results.Result.Rarity != EquipmentRarity.Normal)
                      {
                          var equipment = EquipmentToClient(results.Result);
                          _simulationArtifacts.AllGeneratedItems.Add(equipment);

                          foreach (var craftingTarget in craftingTargets)
                          {
                              if (craftingTarget.Condition != null &&
                                  _conditionResolution.IsValid(craftingTarget.Condition, results.Result))
                              {
                                  if (!_simulationArtifacts.MatchingGeneratedItems.ContainsKey(craftingTarget.Name))
                                  {
                                      _simulationArtifacts.MatchingGeneratedItems.Add(craftingTarget.Name, new List<Equipment>());
                                  }

                                  _simulationArtifacts.MatchingGeneratedItems[craftingTarget.Name].Add(equipment);
                                  saved = true;
                                  break;
                              }
                          }
                      }

                      // Update crafting cost
                      foreach (var result in results.CraftingStepMetadata)
                      {
                          foreach (var currency in result.Value.CurrencyAmounts)
                          {
                              if (!_simulationArtifacts.CurrencyUsed.ContainsKey(currency.Key))
                              {
                                  _simulationArtifacts.CurrencyUsed.Add(currency.Key, 0);
                              }
                              // The progress manager is updated with this information in the craft manager
                              int amountOfCurrencyUsed = currency.Value * result.Value.TimesModified;
                              _simulationArtifacts.CurrencyUsed[currency.Key] += amountOfCurrencyUsed;
                              _simulationArtifacts.CostInChaos += amountOfCurrencyUsed * _currencyValues[currency.Key];
                          }
                      }
      
                      if (progressManager.Progress < 100)
                      {
                          // Get a new item ready. 
                          if (saved ||
                              results.Result.Corrupted ||
                              results.Result.Rarity != EquipmentRarity.Normal &&
                              _baseItemInfo.ItemCost <= _currencyValues[CurrencyNames.ScouringOrb])
                          {
                              _simulationArtifacts.CostInChaos += _baseItemInfo.ItemCost;
                              progressManager.AddCost(_baseItemInfo.ItemCost);
                          }
                          else if (results.Result.Rarity != EquipmentRarity.Normal)
                          {
                              _simulationArtifacts.CostInChaos += _currencyValues[CurrencyNames.ScouringOrb];
                              progressManager.SpendCurrency(CurrencyNames.ScouringOrb, 1);
                          }
                      }

                      // Check for no crafting steps
                      if (Math.Abs(previousProgress - progressManager.Progress) < double.Epsilon)
                      {
                          throw new ArgumentException("Crafting steps do not spend currency");
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

        private ProgressManager GetProgressManager()
        {
            return new ProgressManager(_currencyValues, _financeInfo.BudgetInChaos, i =>
            {
                if (OnProgressUpdate != null)
                {
                    var args = new ProgressUpdateEventArgs
                    {
                        Progress = i
                    };

                    OnProgressUpdate(args);
                }
            });
        }

        public void Cancel()
        {
            if (_task != null && _task.Status == TaskStatus.Running)
            {
                _cancellationTokenSource.Cancel();
            }
        }

        private bool RecursionCheck(
            HashSet<Model.Crafting.Steps.ICraftingStep> parents,
                List<Model.Crafting.Steps.ICraftingStep> children)
        {
            foreach (var child in children)
            {
                if (parents.Contains(child)) return true;

                if (child.Children != null && child.Children.Any())
                {
                    var childHash = parents.ToHashSet();
                    childHash.Add(child);

                    if (RecursionCheck(childHash, child.Children)) return true;
                }
            }

            return false;
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
