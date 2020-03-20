using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class CurrencyStepFactory
    {
        private readonly CurrencyStepExecutor _currencyStepExecutor;

        public CurrencyStepFactory(CurrencyStepExecutor currencyStepExecutor)
        {
            this._currencyStepExecutor = currencyStepExecutor;
        }

        public Action<Equipment, AffixManager, CurrencyModifiers> GetCurrencyStep(String key, object value)
        {
            switch (key)
            {
                case "SetRarity":
                    var setRarityArgs = (RarityOptions)Enum.Parse(typeof(RarityOptions), value.ToString());
                    return _currencyStepExecutor.SetRarity(setRarityArgs);
                case "AddExplicits":
                    var addExplicitsArgs = (DistributionOptions)Enum.Parse(typeof(DistributionOptions), value.ToString());
                    return _currencyStepExecutor.AddExplicits(addExplicitsArgs);
                case "AddExplicit":
                    var addExplicitArgs = (ExplicitOptions)Enum.Parse(typeof(ExplicitOptions), value.ToString());
                    return _currencyStepExecutor.AddExplicit(addExplicitArgs);
                case "RemoveExplicits":
                    var removeExplicitsArgs = (ExplicitsOptions)Enum.Parse(typeof(ExplicitsOptions), value.ToString());
                    return _currencyStepExecutor.RemoveExplicits(removeExplicitsArgs);
                case "RemoveExplicit":
                    var removeExplicitArgs = (ExplicitOptions)Enum.Parse(typeof(ExplicitOptions), value.ToString());
                    return _currencyStepExecutor.RemoveExplicit(removeExplicitArgs);
                case "RandomSteps":
                    var tempArgs = (List<KeyValuePair<int, List<KeyValuePair<string, object>>>>)value;
                    var randomStepArgs = tempArgs.Select(x => new KeyValuePair<int, List<Action<Equipment, AffixManager, CurrencyModifiers>>>(
                        x.Key,
                        x.Value.Select(y => GetCurrencyStep(y.Key, y.Value)).ToList()
                    )).ToList();

                    return _currencyStepExecutor.RandomSteps(randomStepArgs);
                case "RerollExplicits":
                    return _currencyStepExecutor.RerollExplicits();
                case "RerollImplicits":
                    return _currencyStepExecutor.RerollImplicits();
                case "Corrupt":
                    return _currencyStepExecutor.Corrupt();
                case "RemoveImplicits":
                    return _currencyStepExecutor.RemoveImplicits();
                case "AddImplicit":
                    var addImplicitArgs = (ImplicitTypes)Enum.Parse(typeof(ImplicitTypes), value.ToString());
                    return _currencyStepExecutor.AddImplicits(addImplicitArgs);
                case "AddInfluence":
                    var addInfluenceArgs = (InfluenceOptions)Enum.Parse(typeof(InfluenceOptions), value.ToString());
                    return _currencyStepExecutor.AddInfluence(addInfluenceArgs);
                case "SetQualityType":
                    var setQualityTypeArg = (QualityType)Enum.Parse(typeof(QualityType), value.ToString());
                    return _currencyStepExecutor.SetQualityType(setQualityTypeArg);
                case "AddQuality":
                    return _currencyStepExecutor.AddQuality();
                case "RemoveCatalystQuality":
                    var removeCatalystQualityArg = int.Parse(value.ToString());
                    return _currencyStepExecutor.RemoveCatalystQuality(removeCatalystQualityArg);
                case "DestroyItem":
                    return _currencyStepExecutor.DestroyItem();
                case "ResetItem":
                    return _currencyStepExecutor.ResetItem();
                default: throw new InvalidOperationException("Unknown currency step");
            }
        }
    }
}
