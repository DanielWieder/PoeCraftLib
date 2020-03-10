using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using PoeCraftLib.Currency;
using PoeCraftLib.Currency.Currency;
using PoeCraftLib.Data;
using PoeCraftLib.Data.Factory;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CraftingTest
{
    [TestClass]
    public class AffixManagerTest
    {
        private readonly ItemFactory _itemFactory = new ItemFactory();
        private readonly AffixFactory _affixFactory = new AffixFactory();

        private readonly IRandom _random = new PoeRandom();

        // This test if for creating a json file with generated affix/group counts
        [TestMethod]
        public void AffixGenerationTest()
        {

            int count = 1000;

            int ilvl = 75;
            var influence = Influence.Shaper;
            String item = "Murderous Eye Jewel";

            List<Affix> affixes = new List<Affix>()
            {
                GetTestAffix("test", "SpecificWeaponColdDamage", null, "specific_weapon", "mace"),
            };
            List<Fossil> fossils = new List<Fossil>();

            var currencyAffixes = new List<Affix>();
            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            var allAffixes = _affixFactory.GetAffixesForItem(testItem.Tags, testItem.ItemClass, ilvl).ToList();

            var influenceAffixes = _affixFactory.GetAffixesByInfluence(new List<Influence> {influence}, testItem.ItemClass, ilvl);
            var influenceSpawnTags = _affixFactory.GetInfluenceSpawnTags(testItem.ItemClass);

            AffixManager affixManager = new AffixManager(testItem, allAffixes, currencyAffixes, influenceAffixes, influenceSpawnTags);

            List<Affix> generated = new List<Affix>();
            for (int i = 0; i < count; i++) {
                for (int x = 0; x < 6; x++)
                {
                    var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                        new List<string>(),
                        new List<string>(), 
                        ilvl,
                        new Dictionary<string, int>());

                    generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), affixes, EquipmentRarity.Rare, _random));
                }
            }

            var description = new
            {
                Item = item,
                Faction = Enum.GetName(typeof(Influence), influence),
                Ilvl = ilvl,
                Count = count,
                Affixes = affixes.Select(x => new {x.Group, x.AddsTags}),
                Fossils = fossils,
            };

               var groups = generated.GroupBy(x => x.Group).ToDictionary(x => x.Key, x => x.Count()).ToList();

              var affixNames = generated.GroupBy(x => x.FullName).ToDictionary(x => x.Key, x => x.Count()).ToList();

              var bundle = new {Description = description, GroupCount = groups, AffixCount = affixNames};

             var bundleJson = JsonConvert.SerializeObject(bundle);

            //  Assert.IsNotNull(groups);
        }

        [TestMethod]
        public void AffixGenerationDefaultTest()
        {
            int count = 10;

            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);


            List<Affix> allAffixes = new List<Affix>();
            var fossilAffixes = new List<Affix>();

            for (int i = 0; i < count; i++)
            {
                allAffixes.Add(GetTestAffix("test" + i, "test" + i, new Dictionary<string, int>() {{ defaultTag, 100}}));
            }
            AffixManager affixManager = new AffixManager(testItem, allAffixes, fossilAffixes, new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();

            IRandom random = SetupRandom().Object;
            for (var i = 0; i < count; i++)
            {
                var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                    new List<string>(),
                    new List<string>(),
                    100,
                    new Dictionary<string, int>());

                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), new List<Affix>(), EquipmentRarity.Rare, _random));
            }

            Assert.AreEqual(count, generated.Count);
        }

        [TestMethod]
        public void AffixGenerationUniqueGroupTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";
            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();
            var fossilAffixes = new List<Affix>();

            var itemAffix = GetTestAffix("test_d1", "duplicate", new Dictionary<string, int>() {{defaultTag, 100}});
            var duplicate = GetTestAffix("test_d2", "duplicate", new Dictionary<string, int>() { { defaultTag, 100 } });
            allAffixes.Add(itemAffix);
            allAffixes.Add(duplicate);

            for (int i = 0; i < 4; i++)
            {
                allAffixes.Add(GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }));
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, fossilAffixes, new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;
            for (var i = 0; i < 10; i++)
            {
                var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                    new List<string>(),
                    new List<string>(),
                    100,
                    new Dictionary<string, int>());

                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), new List<Affix> { itemAffix }, EquipmentRarity.Rare, _random));
            }

            Assert.IsTrue(!generated.Contains(itemAffix));
            Assert.IsTrue(!generated.Contains(duplicate));
        }

        [TestMethod]
        public void AffixGenerationWeightTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";
            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();
            var fossilAffixes = new List<Affix>();

            var testAffix = GetTestAffix("test", "test", new Dictionary<string, int>() { { defaultTag, 100 } });
            testAffix.GenerationWeights = new Dictionary<string, int>() { { defaultTag , 200 }};
            allAffixes.Add(testAffix);

            for (int i = 0; i < 8; i++)
            {
                allAffixes.Add(GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }));
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, fossilAffixes, new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;
            for (var i = 0; i < 10; i++)
            {
                var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                    new List<string>(),
                    new List<string>(),
                    100,
                    testAffix.GenerationWeights);

                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), new List<Affix>(), EquipmentRarity.Rare, random));
            }

            Assert.AreEqual(generated[0], testAffix);
            Assert.AreEqual(generated[1], testAffix);
        }

        [TestMethod]
        public void CurrencyWeightModifierTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";
            String fossilTag = "abyss_jewel";
            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();
            var fossilAffixes = new List<Affix>();

            var testAffix = GetTestAffix("test", "test", new Dictionary<string, int>() { { fossilTag, 100 } });
            allAffixes.Add(testAffix);

            for (int i = 0; i < 8; i++)
            {
                allAffixes.Add(GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }));
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, fossilAffixes, new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;

            CurrencyModifiers currencyModifiers = new CurrencyModifiers(
                null,
                new Dictionary<string, double>() { { fossilTag, 200 } },
                null,
                null);

            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, currencyModifiers, new List<Affix>(), EquipmentRarity.Rare, random));
            }

            Assert.AreEqual(generated[0], testAffix);
            Assert.AreEqual(generated[1], testAffix);
        }

        [TestMethod]
        public void AddedAffixesTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            var testAffix = GetTestAffix("test", "test", new Dictionary<string, int>() { { defaultTag, 100 } });

            var addedAffixes = new List<Affix> { testAffix };

            CurrencyModifiers currencyModifiers = new CurrencyModifiers(
                addedAffixes, 
                null,
                null,
                null);

            for (int i = 0; i < 9; i++)
            {
                allAffixes.Add(GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }));
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, addedAffixes, new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;

            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, currencyModifiers, new List<Affix>(), EquipmentRarity.Rare, random));
            }

            Assert.IsTrue(generated.Contains(testAffix));
        }

        [TestMethod]
        public void AffixMasterModNoCasterTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";
            String casterTag = "caster";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            var testAffix = GetTestAffix("test", "test", new Dictionary<string, int>() { { defaultTag, 100 }, { casterTag, 100 } });
            var metamod = GetTestAffix("metamod", "ItemGenerationCannotRollCasterAffixes", new Dictionary<string, int>());

            for (int i = 0; i < 9; i++)
            {
                allAffixes.Add(GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }));
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, new List<Affix>(), new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;
            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), new List<Affix>() {metamod}, EquipmentRarity.Rare, _random));
            }

            Assert.IsFalse(generated.Contains(testAffix));
        }

        [TestMethod]
        public void AffixMasterModNoAttackTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";
            String attackTag = "attack";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            var testAffix = GetTestAffix("test", "test", new Dictionary<string, int>() { { defaultTag, 100 }, { attackTag, 100 } });
            var metamod = GetTestAffix("metamod", "ItemGenerationCannotRollAttackAffixes", new Dictionary<string, int>());

            for (int i = 0; i < 9; i++)
            {
                allAffixes.Add(GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }));
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, new List<Affix>(), new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;
            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), new List<Affix>() {metamod}, EquipmentRarity.Rare, _random));
            }

            Assert.IsFalse(generated.Contains(testAffix));
        }

        [TestMethod]
        public void AffixNoPrefixesWhenFullRareTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            var p1 = GetTestAffix("test_p1", "test_p1", new Dictionary<string, int>() { { defaultTag, 100 }});
            var p2 = GetTestAffix("test_p2", "test_p2", new Dictionary<string, int>() { { defaultTag, 100 } });
            var p3 = GetTestAffix("test_p3", "test_p3", new Dictionary<string, int>() { { defaultTag, 100 } });

            p1.GenerationType = "prefix";
            p2.GenerationType = "prefix";
            p3.GenerationType = "prefix";

            var existing = new List<Affix>() {p1, p2, p3};
            allAffixes.AddRange(existing);

            for (int i = 0; i < 10; i++)
            {
                Affix affix = GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() {{defaultTag, 100}});
                affix.GenerationType = i < 5 ? "prefix" : "suffix";
                allAffixes.Add(affix);
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, new List<Affix>(), new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;

            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                    new List<string>(),
                    new List<string>(),
                    100,
                    new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), existing, EquipmentRarity.Rare, _random));
            }


            Assert.IsFalse(generated.Any(x => x.GenerationType == "prefix"));
        }

        [TestMethod]
        public void AffixNoPrefixesWhenFullMagicTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            var p1 = GetTestAffix("test_p1", "test_p1", new Dictionary<string, int>() { { defaultTag, 100 } });

            p1.GenerationType = "prefix";

            var existing = new List<Affix>() { p1 };
            allAffixes.AddRange(existing);

            for (int i = 0; i < 10; i++)
            {
                Affix affix = GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } });
                affix.GenerationType = i < 5 ? "prefix" : "suffix";
                allAffixes.Add(affix);
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, new List<Affix>(), new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;

            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), existing, EquipmentRarity.Magic, _random));
            }

            Assert.IsFalse(generated.Any(x => x.GenerationType == "prefix"));
        }

        [TestMethod]
        public void AffixNoPrefixesWhenFullNormalTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            for (int i = 0; i < 10; i++)
            {
                Affix affix = GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } });
                affix.GenerationType = i < 5 ? "prefix" : "suffix";
                allAffixes.Add(affix);
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, new List<Affix>(), new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;
            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), new List<Affix>(), EquipmentRarity.Normal, _random));
            }

            Assert.IsTrue(generated.All(x => x == null));
        }

        [TestMethod]
        public void AffixNoSuffixesWhenFullTest()
        {
            String item = "Murderous Eye Jewel";
            String defaultTag = "abyss_jewel_melee";

            var testItem = _itemFactory.Jewel.First(x => x.Name == item);
            List<Affix> allAffixes = new List<Affix>();

            var p1 = GetTestAffix("test_p1", "test_p1", new Dictionary<string, int>() { { defaultTag, 100 } });
            var p2 = GetTestAffix("test_p2", "test_p2", new Dictionary<string, int>() { { defaultTag, 100 } });
            var p3 = GetTestAffix("test_p3", "test_p3", new Dictionary<string, int>() { { defaultTag, 100 } });

            p1.GenerationType = "suffix";
            p2.GenerationType = "suffix";
            p3.GenerationType = "suffix";

            var existing = new List<Affix>() { p1, p2, p3 };
            allAffixes.AddRange(existing);

            for (int i = 0; i < 10; i++)
            {
                Affix affix = GetTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } });
                affix.GenerationType = i < 5 ? "prefix" : "suffix";
                allAffixes.Add(affix);
            }

            AffixManager affixManager = new AffixManager(testItem, allAffixes, new List<Affix>(), new Dictionary<Influence, List<Affix>>(), new Dictionary<Influence, string>());

            List<Affix> generated = new List<Affix>();
            IRandom random = SetupRandom().Object;
            var equipmentModifiers = new EquipmentModifiers(new List<Influence>(),
                new List<string>(),
                new List<string>(),
                100,
                new Dictionary<string, int>());

            for (var i = 0; i < 10; i++)
            {
                generated.Add(affixManager.GetAffix(equipmentModifiers, CurrencyModifier(), existing, EquipmentRarity.Rare, _random));
            }

            Assert.IsFalse(generated.Any(x => x.GenerationType == "suffix"));
        }

        private static Mock<IRandom> SetupRandom()
        {
            Mock<IRandom> random = new Mock<IRandom>();
            random.SetupSequence(x => x.NextDouble())
                .Returns(.1).Returns(.2).Returns(.3).Returns(.4).Returns(.5)
                .Returns(.6).Returns(.7).Returns(.8).Returns(.9).Returns(1);
            return random;
        }

        private Affix GetTestAffix(string name, string group, Dictionary<string, int> spawnTagWeights, params string[] tags)
        {
            return new Affix {FullName = name, Name = name, Group = group, Tags = new HashSet<string>(), AddsTags = tags.ToList(), SpawnWeights = spawnTagWeights, GenerationWeights = new Dictionary<string, int>(), GenerationType = "prefix"};
        }

        private ItemBase GetTestItem()
        {
            return _itemFactory.Armour.First();
        }

        private static CurrencyModifiers CurrencyModifier()
        {
            return new CurrencyModifiers(null, null, null, null);
        }
    }
}
