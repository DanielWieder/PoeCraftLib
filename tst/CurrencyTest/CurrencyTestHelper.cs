using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace CurrencyTest
{
    public class CurrencyTestHelper
    {
        public CurrencyFactory CurrencyFactory { get; }
        public readonly ItemFactory ItemFactory;
        public readonly AffixFactory AffixFactory;
        public readonly EssenceFactory EssenceFactory;
        public MasterModFactory MasterModFactory;
        public FossilFactory FossilFactory;
        public IRandom Random;

        private readonly int _invalid = -1;

        public CurrencyTestHelper()
        {
            ItemFactory = new ItemFactory();
            AffixFactory = new AffixFactory();
            EssenceFactory = new EssenceFactory(ItemFactory, AffixFactory);
            FossilFactory = new FossilFactory(AffixFactory);
            MasterModFactory = new MasterModFactory(AffixFactory, ItemFactory);

            Random = new PoeRandom();
            CurrencyFactory = new CurrencyFactory(
                Random,
                EssenceFactory,
                FossilFactory,
                MasterModFactory);
        }

        public Dictionary<string, int> TestCurrency(String currencyName, Equipment equipment)
        {
            var currency = CurrencyFactory.GetCurrencyByName(currencyName);
            var affixManager = GetAffixManager(equipment);
            var spent = currency.Execute(equipment, affixManager);
            return spent;
        }

        public AffixManager GetAffixManager(Equipment equipment)
        {
            var itemAffixes = AffixFactory.GetAffixesForItem(equipment.ItemBase.Tags, equipment.ItemBase.ItemClass, 84);

            var essenceAffixes = EssenceFactory.GetAffixesByItemClass(equipment.ItemBase.ItemClass);
            var fossilAffixes = FossilFactory.Fossils.SelectMany(x => x.AddedAffixes).GroupBy(x => x.FullName).Select(x => x.First());

            var currencyAffixes = essenceAffixes.Union(fossilAffixes).ToList();

            var influences = new List<Influence>((Influence[])Enum.GetValues(typeof(Influence)));
            var affixesByInfluence = AffixFactory.GetAffixesByInfluence(influences, equipment.ItemBase.ItemClass, equipment.ItemLevel);
            var influenceSpawnTags = AffixFactory.GetInfluenceSpawnTags(equipment.ItemBase.ItemClass);

            AffixManager affixManager = new AffixManager(equipment.ItemBase, itemAffixes, currencyAffixes, affixesByInfluence, influenceSpawnTags);
            return affixManager;
        }

        public Equipment GetTestItem()
        {
            var itemBase = ItemFactory.Armour.First();
            return ItemFactory.ToEquipment(itemBase, 84, new List<Influence>());
        }

        public Equipment GetTestItemByItemClass(String itemClass)
        {
            var itemBase = ItemFactory.Items.First(x => x.ItemClass == itemClass);
            return ItemFactory.ToEquipment(itemBase, 84, new List<Influence>());
        }

        public void TestRaritiesForFailure(String currencyName, params EquipmentRarity[] rarities)
        {
            var equipment = GetTestItem();
            TestRaritiesForFailure(currencyName, equipment, rarities);
        }

        public void TestRaritiesForFailure(String currencyName, Equipment equipment, params EquipmentRarity[] rarities)
        {
            foreach (var rarity in rarities)
            {
                equipment.Rarity = rarity;
                var spent = TestCurrency(currencyName, equipment);

                Assert.IsFalse(spent.ContainsKey(currencyName));
            }
        }
    }
}