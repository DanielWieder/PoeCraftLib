using System;
using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Constants;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Currency.Currency
{
    public class BlessedOrb : ICurrency 
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.BlessedOrb;

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { Name, 1 } };

        public BlessedOrb(IRandom random)
        {
            Random = random;
        }

        public bool Execute(Equipment item, AffixManager affixManager)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.Corrupted || item.Implicit == null)
            {
                return false;
            }

            StatFactory.Reroll(Random, item, item.Implicit);

            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            return false;
        }

        public bool IsError(ItemStatus status)
        {
            return !status.HasImplicit || status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            return status;
        }
    }
}
