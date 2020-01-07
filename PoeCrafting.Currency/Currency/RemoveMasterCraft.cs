using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class RemoveMasterCraft : ICurrency
    {
        public string Name => "Remove Master Craft";

        public Dictionary<string, int> GetCurrency() => new Dictionary<string, int>() { { CurrencyNames.ScouringOrb, 1 } };

        public bool Execute(Equipment equipment, AffixManager affixManager)
        {
            var crafted = equipment.Stats.Where(x => x.Affix.GenerationType == "crafted").ToList();

            if (!crafted.Any()) return false;

            equipment.Stats = equipment.Stats.Except(crafted).ToList();
            return true;
        }

        public bool IsWarning(ItemStatus status)
        {
            // I do not currently track if a master mod is on an item in the status
            return true;
        }

        public bool IsError(ItemStatus status)
        {
            if (status.Completed || status.IsCorrupted) return true;
            if (status.Rarity == EquipmentRarity.Normal) return true;
            if (status.MinAffixes + status.MinSuffixes == 1) return true;

            return false;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status)) return status;

            // This case is if an item is metamodded with all crafted affixes
            // This would be significantly improved with better master mod/meta mod status tracking
            status.MinSuffixes = 0;
            status.MinAffixes = 0;

            return status;
        }
    }
}
