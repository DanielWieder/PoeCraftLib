using System;
using System.Collections.Generic;
using System.Linq;
using PoeCraftLib.Data.Entities;
using PoeCraftLib.Data.Query;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Data.Factory
{
    public class ItemFactory
    {
        private readonly IFetchBaseItems _fetchBaseItems = new FetchItems();

        public List<ItemBase> Currency;

        public List<ItemBase> Items { get; }

        public List<ItemBase> Armour { get; }

        public List<ItemBase> Jewelry { get; }

        public List<ItemBase> Weapons { get; }

        public List<ItemBase> Essence { get; }
        public List<ItemBase> Fossil { get; }
        public List<ItemBase> Jewel { get; }

        public ItemFactory()
        {
            Items = _fetchBaseItems.Execute().Select(CreateBaseItem).ToList();

            Armour = Items.Where(HasTag("armour")).ToList();

            Jewelry = Items.Where(HasTag("ring", "amulet", "belt", "quiver")).ToList();

            Weapons = Items.Where(HasTag("weapon")).ToList();

            Currency = Items.Where(HasTag("currency")).ToList();

            Essence = Currency.Where(HasTag("essence")).ToList();

            Fossil = Currency.Where(x => x.Name.Contains("Fossil")).ToList();

            Jewel = Items.Where(x => x.ItemClass == "AbyssJewel" || x.ItemClass == "Jewel").ToList();
        }

        public Equipment ToEquipment(ItemBase itemBase, int itemLevel, List<Influence> influence)
        {
            if (itemBase == null) throw new InvalidOperationException("The equipment factory must be initialized before it can produce equipment");

            return new Equipment
            {
                ItemLevel = itemLevel,
                ItemBase = (ItemBase)itemBase.Clone(),
                Influence = influence
                //     Implicit = _itemBase != null ? StatFactory.AffixToStat(_random, _baseImplicit) : null,
            };
        }

        private ItemBase CreateBaseItem(BaseItemJson baseItemJson)
        {
            ItemBase itemBase = new ItemBase();
            itemBase.Tags = baseItemJson.Tags;
            itemBase.RequiredLevel = (int)(baseItemJson?.Requirements?.Level ?? 0);
            itemBase.Name = baseItemJson.Name;
            itemBase.FullName = baseItemJson.FullName;
            itemBase.ItemClass = baseItemJson.ItemClass;
            itemBase.Properties = baseItemJson.Properties
                .Where(x => x.Key != "description")
                .Where(x => x.Key != "directions")
                .Where(x => x.Key != "full_stack_turns_into")
                .Where(x => !string.IsNullOrEmpty(x.Value))
                .Where(x => double.TryParse(x.Value, out _))
                .ToDictionary(x => x.Key, x => double.Parse(x.Value));
            itemBase.Implicits = baseItemJson.Implicits;

            itemBase.Description = baseItemJson.Properties.ContainsKey("description") ? baseItemJson.Properties["description"] : String.Empty;
            return itemBase;
        }

        private static Func<ItemBase, bool> HasTag(params string[] tags)
        {
            return x => x.Tags.Intersect(tags).Any();
        }
    }
}
