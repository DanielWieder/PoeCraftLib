using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using PoeCraftLib.Currency.CurrencyV2;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency
{
    public class CurrencyFactory
    {
        private readonly IRandom _random;
        private readonly Dictionary<string, Essence> _essences;
        private readonly Dictionary<string, Fossil> _fossils;

        private readonly Dictionary<string, ICurrency> _currency;
        private readonly CurrencyStepExecutor _currencyStepExecutor;
        private readonly CurrencyStepFactory _currencyStepFactory;
        private readonly CurrencyRequirementValidator _currencyRequirementValidator;
        private readonly CurrencyRequirementFactory _currencyRequirementFactory;

        private readonly Dictionary<int, String> _chaoticResonators = new Dictionary<int, string>()
        {
            {1, "Primitive Chaotic Resonator"},
            {2, "Potent Chaotic Resonator"},
            {3, "Powerful Chaotic Resonator"},
            {4, "Prime Chaotic Resonator"}
        };

        private readonly Dictionary<int, String> _alchemicalResonators = new Dictionary<int, string>()
        {
            {1, "Primitive Alchemical Resonator"},
            {2, "Potent Alchemical Resonator"},
            {3, "Powerful Alchemical Resonator"},
            {4, "Prime Alchemical Resonator"}
        };

        private readonly List<Essence> _corruptedEssences;

        public CurrencyFactory(
            IRandom random,
            EssenceFactory essenceFactory, 
            FossilFactory fossilFactory, 
            MasterModFactory masterModFactory)
        {
            _currencyStepExecutor = new CurrencyStepExecutor(random);
            _currencyStepFactory = new CurrencyStepFactory(_currencyStepExecutor);

            _currencyRequirementValidator = new CurrencyRequirementValidator();
            _currencyRequirementFactory =
                new CurrencyRequirementFactory(_currencyRequirementValidator);

            _random = random;

            var currency = GetDefaultCurrency();
            var essenceCurrency = essenceFactory.Essence.Select(EssenceToCurrency);
            var masterCraftCurrency = masterModFactory.MasterMod.GroupBy(x => x.Name).Select(MasterModToCurrency).ToList();
            masterCraftCurrency.Add(RemoveMasterCrafts());

            _corruptedEssences = essenceFactory.Essence.Where(x => x.Tier == 6).ToList();
            var fossilCurrency = fossilFactory.Fossils.Select(x => FossilsToCurrency(new List<Fossil>() { x })).ToList();

            _currency = currency.Union(fossilCurrency)
                .Union(essenceCurrency)
                .Union(masterCraftCurrency)
                .ToDictionary(x => x.Name, x => x);

            _fossils = fossilFactory.Fossils.ToDictionary(x => x.Name, x => x);

            _essences = essenceFactory.Essence.ToDictionary(x => x.Name, x => x);

        }

        public ICurrency GetCurrencyByName(string name)
        {
            if (!_currency.ContainsKey(name)) throw new ArgumentException("Unknown type of currency " + name);

            return _currency[name];
        }

        public ICurrency GetFossilCraftByNames(List<string> names)
        {
            return FossilsToCurrency(names.Select(x => _fossils[x]).ToList());
        }

        private ICurrency MasterModToCurrency(IGrouping<string, MasterMod> masterMods)
        {
            CurrencyV2.Currency currency = new CurrencyV2.Currency();

            currency.Name = masterMods.Key;

            var generationType = masterMods.First().Affix.GenerationType;

            if (masterMods.Any(x => x.Affix.GenerationType != generationType))
            {
                throw new InvalidOperationException("All master mods with the same name must have the same generation type");
            }

            var group = masterMods.First().Affix.Group;

            if (masterMods.Any(x => x.Affix.Group != group))
            {
                throw new InvalidOperationException("All master mods with the same name must have the same group");
            }

            var allItemClasses = masterMods.SelectMany(x => x.ItemClasses).ToList();
            var distinctItemClasses = allItemClasses.Distinct();

            if (allItemClasses.Count() != distinctItemClasses.Count())
            {
                throw new InvalidOperationException("All master mods with the same name must apply to different item classes");
            }

            var explicitOption = generationType == "prefix"
                ? ExplicitOptions.Prefix
                : ExplicitOptions.Suffix;

            currency.Requirements = new List<Func<Equipment, bool>>()
            {
                _currencyRequirementValidator.ValidateRarity(new List<RarityOptions>(){RarityOptions.Magic, RarityOptions.Rare}),
                _currencyRequirementValidator.ValidateOpenExplicit(explicitOption),
                _currencyRequirementValidator.ValidateMatchingGroup(group, GenericOptions.None),
                _currencyRequirementValidator.ValidateMatchingItemClasses(new HashSet<string>(allItemClasses), GenericOptions.Any),
                _currencyRequirementValidator.ValidateCanAddMasterMod()
            };

            Dictionary<string, Affix> affixesByItemClass = new Dictionary<string, Affix>();
            Dictionary<string, KeyValuePair<string, int>> currencyByItemClass = new Dictionary<string, KeyValuePair<string, int>>();
            foreach (var masterMod in masterMods)
            {
                foreach (var itemClass in masterMod.ItemClasses)
                {
                    affixesByItemClass.Add(itemClass, masterMod.Affix);
                    currencyByItemClass.Add(itemClass, new KeyValuePair<string, int>(masterMod.CurrencyType, masterMod.CurrencyCost));
                }
            }


            currency.Steps.Add(_currencyStepExecutor.AddExplicitByItemClass(affixesByItemClass));

            currency.CalculateCost = (item, s) => new Dictionary<string, int>()
            {
                { currencyByItemClass[item.ItemBase.ItemClass].Key,  currencyByItemClass[item.ItemBase.ItemClass].Value}
            };
            
            return currency;
        }

        // Todo: Add this to the misc currency file later
        private ICurrency RemoveMasterCrafts()
        {
            CurrencyV2.Currency currency = new CurrencyV2.Currency();

            currency.Name = "Remove Master Mods";

            currency.Requirements = new List<Func<Equipment, bool>>()
            {
                _currencyRequirementValidator.ValidateRarity(new List<RarityOptions>(){RarityOptions.Magic, RarityOptions.Rare}),
                _currencyRequirementValidator.ValidateHasExplicit(ExplicitOptions.MasterMod)
            };

            currency.Steps.Add(_currencyStepExecutor.RemoveExplicits(ExplicitsOptions.MasterMods));

            return currency;
        }

        private ICurrency EssenceToCurrency(Essence essence)
        {
            CurrencyV2.Currency currency = new CurrencyV2.Currency();

            currency.Name = essence.Name;

            var rarityRequirement = essence.Tier >= 6 ? 
                new List<RarityOptions>() {RarityOptions.Normal, RarityOptions.Rare} : 
                new List<RarityOptions>() { RarityOptions.Normal };

            currency.Requirements.Add(_currencyRequirementValidator.ValidateRarity(rarityRequirement));

            currency.CurrencyModifier.ItemLevelRestriction = essence.ItemLevelRestriction;

            currency.Steps = new List<Action<Equipment, AffixManager>>()
            {
                _currencyStepExecutor.SetRarity(RarityOptions.Rare),
                _currencyStepExecutor.RemoveExplicits(ExplicitsOptions.All),
                _currencyStepExecutor.AddExplicitByItemClass(essence.ItemClassToMod),
                _currencyStepExecutor.AddExplicits(DistributionOptions.RareDistribution)
            };

            return currency;
        }

        private ICurrency FossilsToCurrency(List<Fossil> fossils)
        {
            if (fossils.Count > 4 || fossils.Count == 0)
            {
                throw new InvalidOperationException("A minimum of 1 and a maximum of 4 fossils are supported");
            }

            if (fossils.Select(x => x.Name).Distinct().Count() != fossils.Count)
            {
                throw new InvalidOperationException("Every fossil must be unique");
            }

            CurrencyV2.Currency currency = new CurrencyV2.Currency();

            currency.Name = fossils.Count == 1 ? fossils[0].Name : "Fossil";

            // The cost different between Alchemical resonators and Chaotic resonators
            // compared to the cost of using a scouring orb is minor. I just assume Alchemical + scour if necessary. 
            var rarityRequirement = new List<RarityOptions>(){RarityOptions.Normal, RarityOptions.Rare};
            currency.Requirements.Add(_currencyRequirementValidator.ValidateRarity(rarityRequirement));

            currency.CurrencyModifier.ExplicitWeightModifiers = fossils
                .SelectMany(x => x.ModWeightModifiers)
                .GroupBy(x => x.Key)
                .Select(x => new KeyValuePair<string, int>(
                        x.Key, 
                        x.Select(y => y.Value).Aggregate(((product, next) => product * next / 100))))
                .ToDictionary(x => x.Key, x => x.Value);

            currency.CurrencyModifier.AddedExplicits = fossils
                .SelectMany(x => x.AddedAffixes)
                .Distinct()
                .ToList();

            currency.CurrencyModifier.RollsLucky = fossils.Any(x => x.RollsLucky);

            int corruptedEssenceChance = fossils
                .Select(x => x.CorruptedEssenceChance)
                .Sum();

            corruptedEssenceChance = Math.Min(100, corruptedEssenceChance);

            currency.Steps.Add(_currencyStepExecutor.SetRarity(RarityOptions.Rare));
            currency.Steps.Add(_currencyStepExecutor.RemoveExplicits(ExplicitsOptions.All));
            if (corruptedEssenceChance != 0)
            {
                currency.Steps.Add(ChanceToAddRandomEssenceStep(corruptedEssenceChance, _corruptedEssences));
            }
            currency.Steps.Add(_currencyStepExecutor.AddExplicits(DistributionOptions.RareDistribution));

            currency.CalculateCost = (equipment, s) =>
            {
                var cost = new Dictionary<string, int>();

                if (equipment.Rarity != EquipmentRarity.Normal)
                {
                    cost.Add(CurrencyNames.ScouringOrb, 1);
                }

                foreach (var fossil in fossils)
                {
                    cost.Add(fossil.Name, 1);
                }

                cost.Add(_alchemicalResonators[fossils.Count], 1);

                return cost;
            };

            return currency;
        }

        private Action<Equipment, AffixManager> ChanceToAddRandomEssenceStep(int addEssenceChance, List<Essence> essences)
        {
            int chancePerEssence = (int) (100f / essences.Count);
            var addRandomEssence = essences.Select(x => new KeyValuePair<int, List<Action<Equipment, AffixManager>>>(
                    chancePerEssence,
                    new List<Action<Equipment, AffixManager>>()
                    {
                        _currencyStepExecutor.AddExplicitByItemClass(x.ItemClassToMod)
                    }))
                .ToList();

            var addRandomEssenceSteps = _currencyStepExecutor.RandomSteps(addRandomEssence);

            if (addEssenceChance == 100)
            {
                return addRandomEssenceSteps;
            }

            var addCorruptedEssence = _currencyStepExecutor.RandomSteps(
                new List<KeyValuePair<int, List<Action<Equipment, AffixManager>>>>()
                {
                    new KeyValuePair<int, List<Action<Equipment, AffixManager>>>(
                        addEssenceChance,
                        new List<Action<Equipment, AffixManager>>()
                        {
                            addRandomEssenceSteps
                        }
                    )
                });

            return addCorruptedEssence;
        }

        private List<ICurrency> GetDefaultCurrency()
        {
            FetchCurrencyLogic fetchCurrencyLogic = new FetchCurrencyLogic();
            var currencyLogicList = fetchCurrencyLogic.Execute();
            var currencyList = new List<ICurrency>();

            foreach (var currencyLogic in currencyLogicList)
            {
                CurrencyV2.Currency currency = new CurrencyV2.Currency
                {
                    Name = currencyLogic.Name,
                    Requirements = currencyLogic.Requirements
                        .Select(x => _currencyRequirementFactory.GetRequirement(x.Key, x.Value))
                        .ToList(),
                    Steps = currencyLogic.Steps
                        .Select(x => _currencyStepFactory.GetCurrencyStep(x.Key, x.Value))
                        .ToList()
                };

                currencyList.Add(currency);
            }

            return currencyList;
        }
    }
}
