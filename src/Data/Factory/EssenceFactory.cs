using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Entities;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Factory
{
    public class EssenceFactory
    {
        private readonly FetchEssences _fetchEssences = new FetchEssences();

        private readonly ItemFactory _itemFactory;

        private readonly AffixFactory _affixFactory;

        private readonly Dictionary<string, ItemBase> _essenceItems;

        public List<Essence> Essence { get; }

        public EssenceFactory(ItemFactory itemFactory, AffixFactory affixFactory)
        {
            _itemFactory = itemFactory;
            _affixFactory = affixFactory;

            _essenceItems = _itemFactory.Essence.ToDictionary(x => x.Name, x => x);

            Essence = _fetchEssences.Execute()
                .Where(x => x.Name != "Remnant of Corruption")
                .Select(CreateEssence)
                .ToList();
        }

        public List<Affix> GetAffixesByItemClass(string itemClass)
        {
            return Essence.Select(x => x.ItemClassToMod[itemClass]).ToList();
        }

        private Essence CreateEssence(EssenceJson essenceJson)
        {
            Essence essence = new Essence();
            essence.FullName = essenceJson.FullName;
            essence.Name = essenceJson.Name;
            essence.ItemLevelRestriction = (int)(essenceJson?.ItemLevelRestriction ?? 100);
            essence.Description = _essenceItems[essenceJson.Name].Description;
            essence.Tier = (int)essenceJson.Type.Tier;
            essence.Level = (int) essenceJson.Level;
            essence.ItemClassToMod = _affixFactory.GetEssenceAffixes(essenceJson.Mods, (int)essenceJson.Level);

            return essence;
        }
    }
}
