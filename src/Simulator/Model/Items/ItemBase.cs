using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeCraftLib.Simulator.Model.Items
{
    public class ItemBase {
        public string Name { get; set; }
        public string ItemClass { get; set; }
        public string Type { get; set; }
        public int RequiredLevel { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
}
}
