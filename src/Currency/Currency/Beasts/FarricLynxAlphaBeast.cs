using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency.Beasts
{
    public class FarricLynxAlphaBeast : ICurrency
    {
        public string Name { get; }
        public Dictionary<string, int> Execute(Equipment equipment, AffixManager affixManager)
        {
            throw new NotImplementedException();
        }

        public bool IsWarning(ItemStatus status)
        {
            throw new NotImplementedException();
        }

        public bool IsError(ItemStatus status)
        {
            throw new NotImplementedException();
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            throw new NotImplementedException();
        }
    }
}
