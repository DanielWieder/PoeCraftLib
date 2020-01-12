using System;
using System.Linq;
using AutoMapper;
using PoeCraftLib.Crafting.CraftingSteps;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities.Crafting;
using PoeCraftLib.Entities.Items;
using PoeCraftLib.Simulator.Model.Crafting.Currency;
using PoeCraftLib.Simulator.Model.Crafting.Steps;
using EndCraftingStep = PoeCraftLib.Crafting.CraftingSteps.EndCraftingStep;
using ICraftingStep = PoeCraftLib.Crafting.CraftingSteps.ICraftingStep;
using IfCraftingStep = PoeCraftLib.Crafting.CraftingSteps.IfCraftingStep;
using WhileCraftingStep = PoeCraftLib.Crafting.CraftingSteps.WhileCraftingStep;

namespace PoeCraftLib.Simulator
{
    public class ClientToDomainMapper
    {
        private readonly ItemFactory _itemFactory;
        private readonly CurrencyFactory _currencyFactory;
        private readonly CraftingStepConverter _craftingStepConverter;

        public ClientToDomainMapper(ItemFactory itemFactory, CurrencyFactory currencyFactory)
        {
            _itemFactory = itemFactory;
            _currencyFactory = currencyFactory;
            _craftingStepConverter = new CraftingStepConverter(_currencyFactory);
        }

        public IMapper GenerateMapper()
        {
            var configuration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Model.Items.ItemBase, Entities.Items.ItemBase>()
                    .ConvertUsing(src => _itemFactory.Items.Find(item => src.FullName == item.FullName));

                cfg.CreateMap<Model.Crafting.Steps.ICraftingStep, Crafting.CraftingSteps.ICraftingStep>()
                    .ConvertUsing(_craftingStepConverter);

                cfg.CreateMap<Model.Items.EquipmentRarity, Entities.Items.EquipmentRarity>();
                cfg.CreateMap<Model.Items.Influence, Entities.Items.Influence>();
                cfg.CreateMap<Model.Crafting.AffixRestriction, Entities.Crafting.AffixRestriction>();
                cfg.CreateMap<Model.Crafting.AffixType, Entities.Crafting.AffixType>();
                cfg.CreateMap<Model.Crafting.ConditionAffix, Entities.Crafting.ConditionAffix>();
                cfg.CreateMap<Model.Crafting.ConditionResolution, Entities.Crafting.ConditionResolution>();
                cfg.CreateMap<Model.Crafting.CraftingCondition, Entities.Crafting.CraftingCondition>();
                cfg.CreateMap<Model.Crafting.CraftingStepStatus, Entities.Crafting.CraftingStepStatus>();
                cfg.CreateMap<Model.Crafting.CraftingSubcondition, Entities.Crafting.CraftingSubcondition>();
                cfg.CreateMap<Model.Crafting.StatValueType, Entities.Crafting.StatValueType>();
                cfg.CreateMap<Model.Crafting.SubconditionAggregateType, Entities.Crafting.SubconditionAggregateType>();
                cfg.CreateMap<Model.Crafting.TierType, Entities.Crafting.TierType>();
                cfg.CreateMap<Model.Crafting.CraftingTarget, Entities.Items.CraftingTarget>();

                cfg.CreateMap<Model.Crafting.Currency.CraftingEvent, Currency.ICurrency>();
            });

            return configuration.CreateMapper();
        }



        public class CraftingStepConverter : ITypeConverter<Model.Crafting.Steps.ICraftingStep,
            ICraftingStep>
        {
            private readonly CurrencyFactory _currencyFactory;

            public CraftingStepConverter(CurrencyFactory currencyFactory)
            {
                _currencyFactory = currencyFactory;
            }

            public ICraftingStep Convert(Model.Crafting.Steps.ICraftingStep source,
                ICraftingStep destination, ResolutionContext context)
            {

                switch (source)
                {
                    case Model.Crafting.Steps.WhileCraftingStep _:
                    {
                        WhileCraftingStep step = new WhileCraftingStep();
                        step.Children = source.Children.Select(x => context.Mapper.Map<ICraftingStep>(x)).ToList();
                        step.Condition = context.Mapper.Map<CraftingCondition>(source.Condition);
                        return step;
                    }
                    case Model.Crafting.Steps.IfCraftingStep _:
                    {
                        IfCraftingStep step = new IfCraftingStep();
                        step.Children = source.Children.Select(x => context.Mapper.Map<ICraftingStep>(x)).ToList();
                        step.Condition = context.Mapper.Map<CraftingCondition>(source.Condition);
                        return step;
                    }
                    case Model.Crafting.Steps.EndCraftingStep _:
                    {
                        return new EndCraftingStep();
                    }
                    case Model.Crafting.Steps.CraftingEventStep step:
                    {
                        var currency = GetCraftingEvent(step.CraftingEvent);
                        return new CurrencyCraftingStep(currency);
                    }
                    default: throw new ArgumentException("Unknown type of crafting step");
                }
            }

            public ICurrency GetCraftingEvent(CraftingEvent craftingEvent)
            {
                switch (craftingEvent.Type)
                {
                    case CraftingEventType.Currency:
                    {
                        return _currencyFactory.GetCurrencyByName(craftingEvent.Name);
                    }
                    case CraftingEventType.BeastCraft:
                    {
                        throw new NotImplementedException("Beast-crafting has not yet been implemented");
                    }
                    case CraftingEventType.Essence:
                    {
                        return _currencyFactory.GetEssenceByName(craftingEvent.Name);
                    }
                    case CraftingEventType.Fossil:
                    {
                        return _currencyFactory.GetFossilCraftByNames(craftingEvent.Children);
                        }
                    case CraftingEventType.MasterCraft:
                    {
                        return _currencyFactory.GetMasterCraftByName(craftingEvent.Name);
                    }
                    case CraftingEventType.RemoveMasterCraft:
                    {
                        return new RemoveMasterCraft();
                    }
                    default: throw new ArgumentException("Unknown type of craft");
                }
            }
        }
    }
}
