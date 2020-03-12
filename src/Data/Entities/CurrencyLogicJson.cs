using System.Collections.Generic;

namespace PoeCraftLib.Data.Entities
{
    public class CurrencyLogicJson
    {
        public string Name { get; set; }
        public List<KeyValuePair<string, string>> Requirements { get; set; }
        public List<KeyValuePair<string, string>> Modifiers { get; set; }
        public List<KeyValuePair<string, object>> Steps { get; set; }
    }
}
