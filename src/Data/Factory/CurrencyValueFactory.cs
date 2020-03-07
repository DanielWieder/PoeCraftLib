using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Query;

namespace PoeCraftLib.Data.Factory
{
    public class CurrencyValueFactory
    {
        public Dictionary<string, double> GetCurrencyValues(string league)
        {
            FetchCurrencyValues fetchCurrencyValues = new FetchCurrencyValues {League = league};
            FetchEssenceValues fetchEssenceValues = new FetchEssenceValues {League = league};
            FetchFossilValues fetchFossilValues = new FetchFossilValues {League = league};
            FetchResonatorValues fetchResonatorValues = new FetchResonatorValues {League = league};

            var currency = fetchCurrencyValues.Execute();
            var essences = fetchEssenceValues.Execute();
            var fossils = fetchFossilValues.Execute();
            var resonators = fetchResonatorValues.Execute();

            return currency
                .Union(essences)
                .Union(fossils)
                .Union(resonators)
                .ToDictionary(x => x.Key, x => x.Value);

        }
    }
}
