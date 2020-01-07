using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;


namespace PoeCraftLib.Currency
{
    public interface ICurrency
    {
        string Name { get; }

        bool Execute(Equipment equipment, AffixManager affixManager);

        bool IsWarning(ItemStatus status);

        bool IsError(ItemStatus status);

        ItemStatus GetNextStatus(ItemStatus status);
        Dictionary<string, int> GetCurrency();
    }
}
