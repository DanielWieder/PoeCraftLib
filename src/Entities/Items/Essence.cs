using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoeCraftLib.Entities.Items
{
    public class Essence
    {
        public string Name { get; set; }

        public string FullName { get; set; }

        public string Description { get; set; }

        public int RequiredLevel { get; set; }

        public int Tier { get; set; }
        public int Level { get; set; }

        public Dictionary<String, Affix> ItemClassToMod = new Dictionary<string, Affix>();
    }
}
