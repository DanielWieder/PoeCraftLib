using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeCraftLib.Entities.Items
{
    public class ItemBase : ICloneable
    {
        public string Name { get; set; }
        public string ItemClass { get; set; }
        public string Type { get; set; }
        public int RequiredLevel { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public List<Affix> Implicits { get; set; }

        public Dictionary<string, double> Properties = new Dictionary<string, double>();

        public List<string> Tags = new List<string>();

        public object Clone()
        {
            return new ItemBase()
            {
                Name = this.Name,
                ItemClass = this.ItemClass,
                Type = this.Type,
                Tags = Tags.Select(x => x).ToList(),
                Properties = Properties.ToDictionary(x => x.Key, x => x.Value)
            };
        }
    }
}
