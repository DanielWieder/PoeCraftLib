using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeCraftLib.Currency.CurrencyV2
{
    public class CurrencyRequirementFactory
    {
        private readonly CurrencyRequirementValidator _requirementValidator;

        public CurrencyRequirementFactory(CurrencyRequirementValidator requirementValidator)
        {
            this._requirementValidator = requirementValidator;
        }

        public Func<Equipment, bool> GetRequirement(String key, object value)
        {
            switch (key)
            {
                case "Rarity":
                    var rarityArg = value.ToString().Split(',').Select(x => (RarityOptions)Enum.Parse(typeof(RarityOptions), x)).ToList();
                    return _requirementValidator.ValidateRarity(rarityArg);
                case "OpenExplicit":
                    var openExplicitArg = (ExplicitOptions)Enum.Parse(typeof(ExplicitOptions), value.ToString());
                    return _requirementValidator.ValidateOpenExplicit(openExplicitArg);
                case "HasImplicit":
                    var hasImplicitArg = (ImplicitOptions)Enum.Parse(typeof(ImplicitOptions), value.ToString());
                    return _requirementValidator.ValidateImplicit(hasImplicitArg);
                case "HasTags":
                    var hasTagsArgs = new HashSet<string>((String[])value);
                    return _requirementValidator.ValidateHasTags(hasTagsArgs);
                case "HasExplicit":
                    var hasExplicitArgs = (ExplicitOptions)Enum.Parse(typeof(ExplicitOptions), value.ToString());
                    return _requirementValidator.ValidateHasExplicit(hasExplicitArgs);
            }

            throw new InvalidOperationException("Currency validation requirement not recognized");
        }
    }
}
