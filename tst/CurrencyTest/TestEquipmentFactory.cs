using System;
using System.Collections.Generic;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.CurrencyTest
{
    public class TestEquipmentFactory
    {
        public Equipment GetNormal()
        {
            var prefixes = getTestPrefixes();
            var suffixes = getTestSuffixes();
            var affixes = new List<Affix>();
            affixes.AddRange(prefixes);
            affixes.AddRange(suffixes);

            return new Equipment()
            {
                ItemLevel = 1,
                ItemBase = getTestBase()
            };
        }

        private ItemBase getTestBase()
        {
            return new ItemBase()
            {
                RequiredLevel = 0,
                Name = "Test",
                ItemClass = "Test",
                Type = "Test",
                Tags = new List<string>() { "default" }
            };
        }

        private List<Affix> getTestPrefixes()
        {
            return new List<Affix>()
            {
                getTestAffix(1, 1, "prefix"),
                getTestAffix(2,2, "prefix"),
                getTestAffix(3,3, "prefix"),
                getTestAffix(4,4, "prefix"),
            };
        }

        private List<Affix> getTestSuffixes()
        {
            return new List<Affix>()
            {
                getTestAffix(1,1, "suffix"),
                getTestAffix(2,2, "suffix"),
                getTestAffix(3,3, "suffix"),
                getTestAffix(4,4, "suffix"),
            };
        }

        private Affix getTestAffix(int index, int group, String type)
        {
            return new Affix
            {
                Group = "Test" + type + index,
                RequiredLevel = 0,
                FullName = "Test" + type + index,
                Name = "Test",
                Tier = 0,
                GenerationType = type,
                StatName1 = "Test",
                StatMin1 = 0,
                StatMax1 = 1
            };
        }
    }
}
