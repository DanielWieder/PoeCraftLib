using System.Collections.Generic;
using System.Linq;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Data;
using PoeCrafting.Entities;

namespace PoeCrafting.Currency
{
    public class CurrencyFactory
    {
        public readonly List<ICurrency> Currency;
        private readonly IFetchCurrencyValues _currencyFetch;
        public CurrencyFactory(
            IFetchCurrencyValues currencyValueFetch,
            TransmutationOrb transmutation,
            AlterationOrb alteration,
            AugmentationOrb augmentation,
            AlchemyOrb alchemy,
            ChaosOrb chaos,
            RegalOrb regal,
            BlessedOrb blessed,
            ChanceOrb chance,
            DivineOrb divine,
            ExaltedOrb exalted,
            MasterCraft masterCraft,
            ScouringOrb scouring,
         //   VaalOrb vaal,
            AnullmentOrb anull
        )
        {
            Currency = new List<ICurrency>
            {
                transmutation,
                alteration,
                augmentation,
                alchemy,
                chaos,
                regal,
                 blessed,
                chance,
                divine,
                exalted,
                 masterCraft,
                scouring,
             //   vaal,
                anull
            };

            _currencyFetch = currencyValueFetch;
        }

        public ICurrency GetCurrencyByName(string name)
        {
            return Currency.First(x => x.Name == name);
        }

        public ICurrency GetMasterCraftByName(string name)
        {
            return Currency.First(x => x.Name == name);
        }

        public ICurrency GetEssenceByName(string name)
        {
            return Currency.First(x => x.Name == name);
        }

        public ICurrency GetFossilByName(string name)
        {
            return Currency.First(x => x.Name == name);
        }

        public List<ICurrency> GetValidCurrency(ItemStatus status)
        {
            return Currency.Where(x => !x.IsError(status)).ToList();
        }
    }
}
