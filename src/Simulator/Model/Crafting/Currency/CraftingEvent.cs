using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCraftLib.Simulator.Model.Crafting.Currency
{
    public class CraftingEvent
    {
        public CraftingEventType Type { get; set; }

        public String Name { get; set; }

        public List<String> Children { get; set; }
    }
}
