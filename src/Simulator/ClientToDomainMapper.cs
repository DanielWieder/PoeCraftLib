using System;
using System.Linq;
using AutoMapper;
using PoeCraftLib.Currency;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities.Crafting;
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
                    case Model.Crafting.Steps.CurrencyCraftingStep step:
                    {
                        var currency = GetCraftingEvent(step);
                        return new Crafting.CraftingSteps.CurrencyCraftingStep(currency);
                    }
                    default: throw new ArgumentException("Unknown type of crafting step");
                }
            }

            public ICurrency GetCraftingEvent(Model.Crafting.Steps.CurrencyCraftingStep craftingStep)
            {
                if (craftingStep.SocketedCurrency != null)
                {
                    return _currencyFactory.GetFossilCraftByNames(craftingStep.SocketedCurrency);
                }

                return _currencyFactory.GetCurrencyByName(craftingStep.Name);
            }
        }
    }
}
