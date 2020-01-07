# PoeCraftLib
Crafting library for Path of Exile

# Instructions

## Quick Start

The code below simulates spending the equivalent of 1000 chaos on using alchemy orbs on Fencer Helms. 

After the simulation has finished running, you can access the generated items through sim.Result.AllGeneratedItems().

```
SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
SimCraftingInfo craftingInfo = new SimCraftingInfo();
SimFinanceInfo financeInfo = new SimFinanceInfo();

baseItemInfo.ItemName = "Fencer Helm";
financeInfo.BudgetInChaos = 1000;

IRandom random = new PoeRandom();
craftingInfo.CraftingSteps = new List<ICraftingStep>()
{
	new CurrencyCraftingStep(new AlchemyOrb(random))
};

CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);

var sim = craftingSimulator.Start();

sim.Wait();
```

## Running a crafting simulation

## Getting started

Start with the code below.
There are 3 required fields.
* baseItemInfo.ItemName
* financeInfo.BudgetInChaos
* craftingInfo.CraftingSteps

You can adjust the parameters of the simulation with [arguments](#crafting-simulation-arguments) objects.
The output can be read from the [results](#crafting-results) object.
The current status of the simulation can be checked with the [metadata](#simulator-metadata) object.
It can be cancelled early with the [cancellation](#simulator-cancellation) object.

```
SimBaseItemInfo baseItemInfo = new SimBaseItemInfo();
SimCraftingInfo craftingInfo = new SimCraftingInfo();
SimFinanceInfo financeInfo = new SimFinanceInfo();
CraftingSimulator craftingSimulator = new CraftingSimulator(baseItemInfo, financeInfo, craftingInfo);
var sim = craftingSimulator.Start();
```

### Crafting simulation arguments

[SimBaseItemInfo](#base-item-info): Contains information about the base item you are crafting.

[SimFinanceInfo](#finance-info): Contains financial information about craft.

[SimCraftingInfo](#crafting-info): Contains information about the crafting process.

#### Base item info

ItemName: The name of the base item. Item data can be acquired through the [ItemFactory](#getting-data).  (REQUIRED)

ItemLevel: The item level of the item base. All affixes have a required item level before they can be generated. (Default 100)

Faction: The faction of the item base. Some affixes are unique to certain factions. This will be either None, Shaper, or Elder. (Default None)

ItemCost: The cost of the item base. The simulation will add a new base item using this cost whenever an generated item matches a Crafting Target or is corrupted. Additionally, if a generated item does not meet a Crafting Target, the simulation will either scour and re-use the item or add a new one depending on whether the cost of the item base or the cost of souring orbs is cheaper. (Default 0)

#### Finance info

BudgetInChaos: The amount of currency in chaos that you want to spend crafting. (REQUIRED)

League: The current league. Options are Standard, Hardcore, or the current league. (Default Standard)

#### Crafting info

[CraftingSteps](#crafting-steps): The crafting process. (Default Empty)

[CraftingTargets](#crafting-targets): The list of items that you are attempting to craft. (Default Empty)

MaximumDurationInSeconds: The maximum duration of the simulation in seconds before it cancels. (Default 60)

#### Crafting results

AllGeneratedItems: A collection of all generated items.

MatchingGeneratedItems: A collection of all generated items that match a Crafting Target. Each generated item will only be assigned to a single Crafting Target, priortizing Crafting Targets with a higher value.

CurrencyUsed: The quantity of each currency type spent.

CostInChaos: The total cost in chaos. For completed simulations this will be slightly higher than the Budget.

### Simulator metadata

Status: The current status of the simulation. It has the following states
* Stopped: The initial status
* Running: After Start() is called called
* Completed: After the simulation has completed
* Cancelled: When Cancel() is called or after [MaximumDurationInSeconds](#crafting-info) has elapsed

The simulator contains a Progress property that provides a 0-100 representation of how much currency has been spent out of the budget. Whenever the progress is updated the OnProgressUpdate event handler will fire.

The simulator can be cancelled with the Cancel() function and will automatically cancel if it takes longer than the [MaximumDurationInSeconds](#crafting-info). If the simulation is cancelled the OnSimulationComplete event handler will fire.

### Simulator cancellation

The simulation executes in the Task that is returned when the simulation is started. It will end after the timeout duration has elapsed. It can be cancelled early by calling Cancel on the simulation.

## Getting Data

This section will be added at a later date.

A TLDR is that the PoeCrafting.Data project has a lot of factories that can get you properly structured data. The ones that provide information that can be passed into the simulator are listed below
* EssenceFactory
* FossilFactory
* MasterModFactory

## Crafting steps

Each crafting step will be executed sequentially. There are 4 main types of crafting steps

* CurrencyCraftingStep: This crafting step will attempt to modify the item according to the [currency](#currency) it contains. If the currency cannot be used on the item due to the item's current status then it will not modify the item. 

* IfCraftingStep: This crafting step will execute the crafting steps in it's Children property if the [CraftingCondition](#conditions) is matched.

* WhileCraftingStep: This crafting step will repeatedly execute the crafting steps in it's Children property as long as the [CraftingCondition](#conditions) is matched.

* EndCraftingStep: This crafting step will complete the item. No more crafting steps will be executed. The item will be evaluated to determine if it matches any CraftingTarget.

## Currency

The CurrencyCraftingStep requires a Currency item.

There are several different types of currency items. 

Common Currency: Due to their varied behavior, each type of common currency has it's own Currency item. You can acquire these items from the CurrencyFactory. 
* Alchemy, Alteration, ANullment, Augmnetation, Blessed, Chance, Chaos, Divine, essence, Exalted, Fossil, Regal, Scouring, Vaal

Fossil: There is a single FossilCraft Currency object. This item requires 1-4 Fossils. You can acquire fossils from the [FossilFactory](#getting-data). 

Essence: There is a single EssenceCraft Currency object. This item requires 1 Essence. You can acquire Essences from the [EssenceFactory](#getting-data).

Master: There is a single MasterCraft Currency object. This item requires 1 MasterMod. You can acquire MasterMods from the [MasterModFactory](#getting-data).

## Crafting targets

Name: The name of the crafting target

Value: The value of the crafting target. If a generated item meets the condition of multiple crafting targets then it will be allocated to the one with the highest value.
	
## Conditions

Conditions allow you to identify when generated items match specific criteria.

### Crafting condition

CraftingSubconditions: Each CraftingCondition consists of multiple CraftingSubconditions. In order for the CraftingCondition to be true, all of the subconditions must also be true. 

### Crafting subcondition

Name: The name of the subcondition

AggregateType: Determines how the subcondition will be evaluated
* And: Is true when all of the AffixConditions are true
* Count: Is true when the number of true AffixConditions is between any non-null AggregateMin and AggregateMax values
* Sum: Is true when the sum of the values in the AffixConditions is between any non-null AggregateMin and AggregateMax values
* If: Is true when all of the AffixConditions with matching affixes are true
* Not: Is true when none of the AffixConditions are true

Aggregate Min: Used for Count and Sum AggregateTypes.

Aggregate Max: Used for Count and Sum AggregateTypes.

ValueType: Determines what affix value to use when evaluating the AffixConditions
* Flat: Use the current value of the affix on the item
* Max: Use the maximum value that the affix can roll on the item
* Tier: Use the tier of the affix on the item

PrefixConditions: A list of prefix [AffixConditions](#affix-condition) 

SuffixConditions: A list of suffix [AffixConditions](#affix-condition) 

MetaConditions: A list of meta [AffixConditions](#affix-condition). A list of all supported meta affixes is listed below.
* Open PrefixConditions
* Open Suffix
* Total Energy Shield
* Total Armour
* Total Evation
* Total DPS
* Total Physical DPS
* Total Elemental DPS
* Total Elemental Resistances
* Total Resistances 

### Affix condition

In order for an AffixCondition to be true, any existing stats must be between the minimum and maximum values. A full list of affixes can be acquired from the [AffixFactory](#getting-data). This data includes all ModTypes, and the minimum and maximum that a stat can roll.

If the CraftingSubcondition ValueType is Tier then the Tier value should go in the Min1 and Min2 fields. The other fields will not be evaluated.

ModType: The mod type.

Min1: The minimum value of the first stat in the affix
Max1: The maximum value of the first stat in the affix

Min2: The minimum value of the second stat in the affix
Max2: The maximum value of the second stat in the affix

Min3: The minimum value of the third stat in the affix
Max3: The maximum value of the third stat in the affix

# Unsupported Features

This library does not currently support the following. However they are on the shortlist for new features.
* Implicits
* Item Quality: All items are treated as having 20% quality for meta affix Defense calculations
* Beastcrafting
* Elderslayer Currency

# Thanks

* Thank you brather1ng for RePoe. It converts the PoE data files into a more managable json format.
https://github.com/brather1ng/RePoE

* Thank you rasmuskl for PoeNinja. It has an API that provides up to date currency values
https://poe.ninja/

* Thank you Nick for answering questions I had about some of the trickier parts of item generation
https://github.com/NickRyder/PoECraft