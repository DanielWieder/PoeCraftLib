using System;
using System.Collections.Generic;
using System.Linq;
using DataJson.Factory;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CurrencyTest
{
    [TestClass]
    public class AffixFactoryTest
    {
        readonly AffixFactory _affixFactory = new AffixFactory();
        readonly ItemFactory _itemFactory = new ItemFactory();
        readonly EssenceFactory _essenceFactory = new EssenceFactory();

        [TestMethod]
        public void ShaperItemHasShaperMods()
        {
            var item = GetTestItem();
            var influence = new List<Influence> { Influence.Shaper };
            var affixes = _affixFactory.GetAffixesForItem(item.Tags, item.ItemClass, 100, influence);
            
            Assert.IsTrue(affixes.Any(x => x.SpawnWeights.Any(y => y.Key.Contains("shaper"))));
            Assert.IsTrue(!affixes.Any(x => x.SpawnWeights.Any(y => y.Key.Contains("elder"))));
        }

        [TestMethod]
        public void ElderItemHasShaperMods()
        {
            var item = GetTestItem();
            var influence = new List<Influence> { Influence.Elder };
            var affixes = _affixFactory.GetAffixesForItem(item.Tags, item.ItemClass, 100, influence);

            Assert.IsTrue(!affixes.Any(x => x.SpawnWeights.Any(y => y.Key.Contains("shaper"))));
            Assert.IsTrue(affixes.Any(x => x.SpawnWeights.Any(y => y.Key.Contains("elder"))));
        }

        [TestMethod]
        public void FactionlessItemHasNoShaperOrElderMods()
        {
            var item = GetTestItem();
            var affixes = _affixFactory.GetAffixesForItem(item.Tags, item.ItemClass, 100, new List<Influence>());

            Assert.IsTrue(!affixes.Any(x => x.SpawnWeights.Any(y => y.Key.Contains("shaper"))));
            Assert.IsTrue(!affixes.Any(x => x.SpawnWeights.Any(y => y.Key.Contains("elder"))));
        }

        [TestMethod]
        public void LevelOneItemHasAllTypesOfMods()
        {
            var item = GetTestItem();
            var affixes = _affixFactory.GetAffixesForItem(item.Tags, item.ItemClass, 1);

            Assert.IsTrue(affixes.Count(x => x.GenerationType == "prefix") > 0);
            Assert.IsTrue(affixes.Count(x => x.GenerationType == "suffix") > 0);
            Assert.IsTrue(affixes.Count(x => x.GenerationType == "corrupted") > 0);
        }

        [TestMethod]
        public void HigherLevelItemHasMoreMods()
        {
            var item = GetTestItem();
            var lowLevelItem = _affixFactory.GetAffixesForItem(item.Tags, item.ItemClass, 1);
            var highLevelItem = _affixFactory.GetAffixesForItem(item.Tags, item.ItemClass, 100);

            Assert.IsTrue(lowLevelItem.Count < highLevelItem.Count);
        }

        [TestMethod]
        public void HasEssenceMod()
        {
            var item = GetTestItem();

            Essence essence = _essenceFactory.Essence.First();

            var affix = essence.ItemClassToMod[item.ItemClass];

            Assert.IsNotNull(affix);
        }

        private ItemBase GetTestItem()
        {
            return _itemFactory.Armour.First();
        }

        private static Func<ItemBase, bool> HasTag(params string[] tags)
        {
            return x => x.Tags.Intersect(tags).Any();
        }
    }
}
