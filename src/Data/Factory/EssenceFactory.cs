using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Entities;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Factory
{
    public class EssenceFactory
    {
        FetchEssences _fetchEssences = new FetchEssences();

        ItemFactory _itemFactory;

        AffixFactory _affixFactory;

        private Dictionary<string, ItemBase> essenceItems;

        public List<Essence> Essence { get; }

        public EssenceFactory(ItemFactory itemFactory, AffixFactory affixFactory)
        {
            _itemFactory = itemFactory;
            _affixFactory = affixFactory;

            essenceItems = _itemFactory.Essence.ToDictionary(x => x.Name, x => x);

            Essence = _fetchEssences.Execute()
                .Where(x => x.Name != "Remnant of Corruption")
                .Select(CreateEssence)
                .ToList();
        }

        private Essence CreateEssence(EssenceJson essenceJson)
        {
            Essence essence = new Essence();
            essence.FullName = essenceJson.FullName;
            essence.Name = essenceJson.Name;
            essence.ItemLevelRestriction = (int)(essenceJson?.ItemLevelRestriction ?? 100);
            essence.Description = essenceItems[essenceJson.Name].Description;
            essence.Tier = (int)essenceJson.Type.Tier;
            essence.Level = (int) essenceJson.Level;
            essence.ItemClassToMod = _affixFactory.GetEssenceAffixes(essenceJson.Mods, (int)essenceJson.Level);

            return essence;
        }
    }
}
