using System.Collections.Generic;

namespace PoeCraftLib.Entities.Items
{
    public class Stat
    {
        public Affix Affix { get; set; }
        public int? Value1 { get; set; } = null;
        public int? Value2 { get; set; } = null;
        public int? Value3 { get; set; } = null;

        public List<int> Values
        {
            get
            {
                var list = new List<int>();
                if (Value1 != null)
                {
                    list.Add(Value1.Value);
                }
                if (Value2 != null)
                {
                    list.Add(Value2.Value);
                }
                if (Value3 != null)
                {
                    list.Add(Value3.Value);
                }
                return list;
            }
        }
    }
}
