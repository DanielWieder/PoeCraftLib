using System;

namespace PoeCraftLib.Simulator.Model.Items
{
    [Flags]
    public enum EquipmentRarity
    {
        Normal = 1 << 0,
        Magic = 1 << 1,
        Rare = 1 << 2,
        Unique = 1 << 3,
    }
}
