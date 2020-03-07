using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities.Items;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PoeCraftLib.Currency.CurrencyV2
{
    public class CurrencyStepFactory
    {
        private CurrencyStepExecutor currencyStepExecutor;

        public CurrencyStepFactory(CurrencyStepExecutor currencyStepExecutor)
        {
            this.currencyStepExecutor = currencyStepExecutor;
        }

        public Action<Equipment, AffixManager> GetCurrencyStep(String key, object value)
        {
            switch (key)
            {
                case "SetRarity":
                    var setRarityArgs = (RarityOptions)Enum.Parse(typeof(RarityOptions), value.ToString());
                    return currencyStepExecutor.SetRarity(setRarityArgs);
                case "AddExplicits":
                    var addExplicitsArgs = (DistributionOptions)Enum.Parse(typeof(DistributionOptions), value.ToString());
                    return currencyStepExecutor.AddExplicits(addExplicitsArgs);
                case "AddExplicit":
                    var addExplicitArgs = (ExplicitOptions)Enum.Parse(typeof(ExplicitOptions), value.ToString());
                    return currencyStepExecutor.AddExplicit(addExplicitArgs);
                case "RemoveExplicits":
                    var removeExplicitsArgs = (ExplicitsOptions)Enum.Parse(typeof(ExplicitsOptions), value.ToString());
                    return currencyStepExecutor.RemoveExplicits(removeExplicitsArgs);
                case "RemoveExplicit":
                    var removeExplicitArgs = (ExplicitOptions)Enum.Parse(typeof(ExplicitOptions), value.ToString());
                    return currencyStepExecutor.RemoveExplicit(removeExplicitArgs);
                case "RandomSteps":
                    var tempArgs = (List<KeyValuePair<int, List<KeyValuePair<string, object>>>>)value;
                    var randomStepArgs = tempArgs.Select(x => new KeyValuePair<int, List<Action<Equipment, AffixManager>>>(
                        x.Key,
                        x.Value.Select(y => GetCurrencyStep(y.Key, y.Value)).ToList()
                    )).ToList();

                    return currencyStepExecutor.RandomSteps(randomStepArgs);
                case "RerollExplicits":
                    return currencyStepExecutor.RerollExplicits();
                case "RerollImplicits":
                    return currencyStepExecutor.RerollImplicits();
                case "Corrupt":
                    return currencyStepExecutor.Corrupt();
                case "RemoveImplicits":
                    return currencyStepExecutor.RemoveImplicits();
                case "AddImplicit":
                    var addImplicitArgs = (ImplicitTypes)Enum.Parse(typeof(ImplicitTypes), value.ToString());
                    return currencyStepExecutor.AddImplicits(addImplicitArgs);
                default: throw new InvalidOperationException("Unknown currency step");
            }
        }
    }
}
