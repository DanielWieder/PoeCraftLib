using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
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
            if (value is string s)
            {
                var split = s.Split(',');

                if (split.Length > 1)
                {
                    var validations = split.Select(x => GetRequirement(key, x)).ToArray();
                    return _requirementValidator.ValidateAny(validations);
                }
            }

            switch (key)
            {
                case "HasRarity":
                    var rarityArg = (RarityOptions)Enum.Parse(typeof(RarityOptions), value.ToString());
                    return _requirementValidator.ValidateRarity(rarityArg);
                case "HasOpenExplicit":
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
                case "HasInfluence":
                    var hasInfluenceArg = (InfluenceOptions)Enum.Parse(typeof(InfluenceOptions), value.ToString());
                    return _requirementValidator.ValidateHasInfluence(hasInfluenceArg);
            }

            throw new InvalidOperationException("Currency validation requirement not recognized");
        }
    }
}
