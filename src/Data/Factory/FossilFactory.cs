using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Entities;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Factory
{
    public class FossilFactory
    {
        private readonly IFetchFossils _fetchFossils = new FetchFossils();
        private readonly AffixFactory _affixFactory = new AffixFactory();

        public List<Fossil> Fossils { get; set; }

        public FossilFactory()
        {
            Fossils = _fetchFossils.Execute().Select(CreateFossil).ToList();
        }

        private Fossil CreateFossil(FossilJson fossilJson)
        {
            Fossil fossil = new Fossil();
            fossil.FullName = fossilJson.FullName;
            fossil.Name = fossilJson.Name;
            fossil.CorruptedEssenceChance = (int)fossilJson.CorruptedEssenceChance;
            fossil.AddedAffixes = _affixFactory.GetFossilAffixes(fossilJson.AddedMods);
            fossil.AllowedTags = fossilJson.AllowedTags;
            fossil.ForbiddenItemClasses = new HashSet<string>(fossilJson.ForbiddenTags);
            fossil.ModWeightModifiers = fossilJson.NegativeModWeights.Union(fossilJson.PositiveModWeights).ToDictionary(x => x.Tag, x => (int)x.Weight);
            fossil.RollsLucky = fossilJson.RollsLucky;

            return fossil;
        }
    }
}
