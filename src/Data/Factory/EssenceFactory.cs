using System.Collections.Generic;
using System.Linq;
using DataJson.Entities;
using DataJson.Query;
using PoeCraftLib.Data;
using PoeCraftLib.Entities.Items;

namespace DataJson.Factory
{
    public class EssenceFactory
    {
        FetchEssences _fetchEssences = new FetchEssences();

        FetchEssenceValues _fetchEssenceValues = new FetchEssenceValues();

        ItemFactory _itemFactory = new ItemFactory();

        AffixFactory _affixFactory = new AffixFactory();

        private Dictionary<string, ItemBase> essenceItems;

        public List<Essence> Essence { get; }

        public EssenceFactory()
        {
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
            essence.RequiredLevel = (int)(essenceJson?.ItemLevelRestriction ?? 100);
            essence.Description = essenceItems[essenceJson.Name].Description;
            essence.Tier = (int)essenceJson.Type.Tier;
            essence.Level = (int) essenceJson.Level;
            essence.ItemClassToMod = _affixFactory.GetEssenceAffixes(essenceJson.Mods, (int)essenceJson.Level);

            return essence;
        }
    }
}
