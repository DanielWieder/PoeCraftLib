using System;
using System.Collections.Generic;

namespace PoeCraftLib.Entities.Items
{
    public class MasterMod
    {
        public Affix Affix { get; set; }
        public string Group { get; set; }
        public int Tier { get; set; }
        public string CurrencyType { get; set; }
        public int CurrencyCost { get; set; }
        public HashSet<String> ItemClasses { get; set; }
        public String Master { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
