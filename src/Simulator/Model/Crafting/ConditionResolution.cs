using System.Collections.Generic;

namespace PoeCraftLib.Simulator.Model.Crafting
{
    public class ConditionResolution
    {
        public bool IsPresent { get; set; }
        public bool IsMatch { get; set; }
        public List<int> Values { get; set; }
    }
}
