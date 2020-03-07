using System.Collections.Generic;
using PoeCraftLib.Entities.Items;


namespace PoeCraftLib.Currency
{
    public interface ICurrency
    {
        string Name { get; }

        Dictionary<string, int> Execute(Equipment equipment, AffixManager affixManager);
    }
}