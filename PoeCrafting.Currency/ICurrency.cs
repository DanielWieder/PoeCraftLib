using System.Collections.Generic;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Items;


namespace PoeCrafting.Currency
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
