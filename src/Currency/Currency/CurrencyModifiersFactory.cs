using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class CurrencyModifiersFactory
    {
        public CurrencyModifiers GetCurrencyModifiers(List<KeyValuePair<string, string>> modifierProperties)
        {
            if (modifierProperties == null || !modifierProperties.Any())
            {
                return new CurrencyModifiers(null, null, null, null, null);
            }

            bool rollsLucky = false;
            int itemLevelRestriction = 100;
            bool qualityAffectsExplicitOdds = false;

            foreach (var modifierProperty in modifierProperties)
            {
                switch (modifierProperty.Key)
                {
                    case "RollsLucky":
                        rollsLucky = true;
                        break;
                    case "ItemLevelRestriction":
                        itemLevelRestriction = int.Parse(modifierProperty.Value);
                        break;
                    case "QualityAffectsExplicitOdds":
                        qualityAffectsExplicitOdds = true;
                        break;
                    default:
                        throw new InvalidOperationException("Currency modifier not recognized");
                }
            }

            return new CurrencyModifiers(null, null, itemLevelRestriction, rollsLucky, qualityAffectsExplicitOdds);
        }
    }
}
