using PoeCraftLib.Entities;
using System;
using System.Collections.Generic;

namespace PoeCraftLib.Currency.CurrencyV2
{
    public class CurrencyModifier
    {
        public List<Affix> AddedExplicits { get; set; } = new List<Affix>();

        public Dictionary<string, int> ExplicitWeightModifiers { get; set; } = new Dictionary<string, int>();

        public int? ItemLevelRestriction { get; set; } = 100;

        public Boolean RollsLucky { get; set; } = false;
    }
}