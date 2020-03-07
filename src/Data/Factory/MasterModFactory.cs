using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Entities;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Factory
{
    public class MasterModFactory
    {
        readonly FetchMasterMods _fetchMasterMods = new FetchMasterMods();

        readonly ItemFactory _itemFactory;

        readonly AffixFactory _affixFactory;

        public List<MasterMod> MasterMod { get; }

        public MasterModFactory(AffixFactory affixFactory, ItemFactory itemFactory)
        {
            _affixFactory = affixFactory;
            _itemFactory = itemFactory;

            var masterMods = _fetchMasterMods.Execute();

            Dictionary<CraftingBenchJson, int> modTiers = new Dictionary<CraftingBenchJson, int>();

            var modsByGroup = masterMods.GroupBy(x => x.BenchGroup);
            foreach (var modsInGroup in modsByGroup)
            {
                var orderedGroup = modsInGroup.OrderByDescending(x => x.BenchTier).ToList();

                for (int i = 0; i < orderedGroup.Count; i++)
                {
                    modTiers.Add(orderedGroup[i], i + 1);
                }
            }

            MasterMod = masterMods.Select(x => CreateMasterMod(x, modTiers))
                .ToList();
        }

        private MasterMod CreateMasterMod(CraftingBenchJson masterModJson, Dictionary<CraftingBenchJson, int> modTiers)
        {
            MasterMod masterMod = new MasterMod();
            masterMod.Name = masterModJson.BenchGroup + masterModJson.BenchTier;
            masterMod.Master = masterModJson.Master;
            masterMod.Group = masterModJson.BenchGroup;
            masterMod.ItemClasses = new HashSet<string>(masterModJson.ItemClasses);
            masterMod.CurrencyType = _itemFactory.Items.First(x => x.FullName == masterModJson.Cost.First().Key).Name;
            masterMod.CurrencyCost = (int)masterModJson.Cost.First().Value;
            masterMod.Tier = modTiers[masterModJson];
            masterMod.FullName = masterModJson.ModId;
            masterMod.Affix = _affixFactory.GetMasterAffix(masterModJson.ModId, modTiers[masterModJson]);
            return masterMod;
        }
    }
}
