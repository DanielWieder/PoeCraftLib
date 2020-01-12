using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class BlessedOrb : ICurrency 
    {
        private IRandom Random { get; }

        public string Name => CurrencyNames.BlessedOrb;

        private readonly Dictionary<string, int> _currency = new Dictionary<string, int>() { { CurrencyNames.BlessedOrb, 1 } };

        public BlessedOrb(IRandom random)
        {
            Random = random;
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (Random == null)
            {
                throw new InvalidOperationException("The random number generator is uninitialized");
            }

            if (item.Corrupted || item.Implicit == null)
            {
                return new Dictionary<string, int>();
            }

            StatFactory.Reroll(Random, item, item.Implicit);

            return _currency;
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
