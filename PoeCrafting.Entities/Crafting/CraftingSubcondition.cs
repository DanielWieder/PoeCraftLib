using System.Collections.Generic;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Entities.Crafting
{
    public class CraftingSubcondition
    {
        public SubconditionAggregateType AggregateType { get; set; } = SubconditionAggregateType.And;
        public int? AggregateMin { get; set; }
        public int? AggregateMax { get; set; }

        public StatValueType ValueType { get; set; }

        public List<ConditionAffix> PrefixConditions { get; set; } = new List<ConditionAffix>();
        public List<ConditionAffix> SuffixConditions { get; set; } = new List<ConditionAffix>();
        public List<ConditionAffix> MetaConditions { get; set; } = new List<ConditionAffix>();
        public string Name { get; set; }
    }
}
