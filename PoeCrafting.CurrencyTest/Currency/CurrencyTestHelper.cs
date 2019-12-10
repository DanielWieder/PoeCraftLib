using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using PoeCrafting.Currency;
using PoeCrafting.Currency.Currency;
using PoeCrafting.Entities;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.CurrencyTest.Currency
{
    public class CurrencyTestHelper
    {
        private Mock<IRandom> _random;

        private AlchemyOrb _alchemy;
        private TestEquipmentFactory _factory;
        private TransmutationOrb _transmutation;

        public CurrencyTestHelper()
        {
            _factory = new TestEquipmentFactory();

            _random = new Mock<IRandom>();
            _random.Setup(x => x.Next()).Returns(0);
            _random.Setup(x => x.NextDouble()).Returns(0);

            _alchemy = new AlchemyOrb(_random.Object);
            _transmutation = new TransmutationOrb(_random.Object);
        }

        public bool CanUseOnNormal(ICurrency currency)
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = CreateAffixManager(item.ItemBase);
            return currency.Execute(item, affixManager);
        }

        public bool CanUseOnMagic(ICurrency currency)
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            return currency.Execute(item, affixManager);
        }

        public bool CanUseOnRare(ICurrency currency)
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = CreateAffixManager(item.ItemBase);
            _alchemy.Execute(item, affixManager);
            return currency.Execute(item, affixManager);
        }

        public bool CanUseOnCorruptedNormal(ICurrency currency)
        {
            var item = _factory.GetNormal();
            item.Corrupted = true;
            AffixManager affixManager = CreateAffixManager(item.ItemBase);
            return currency.Execute(item, affixManager);
        }

        public bool CanUseOnCorruptedMagic(ICurrency currency)
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = CreateAffixManager(item.ItemBase);
            _transmutation.Execute(item, affixManager);
            item.Corrupted = true;
            return currency.Execute(item, affixManager);
        }

        public bool CanUseOnCorruptedRare(ICurrency currency)
        {
            var item = _factory.GetNormal();
            AffixManager affixManager = CreateAffixManager(item.ItemBase);
            _alchemy.Execute(item, affixManager);
            item.Corrupted = true;
            return currency.Execute(item, affixManager);
        }

        public AffixManager CreateAffixManager(ItemBase itemBase)
        {
            List<Affix> allAffixes = new List<Affix>();

            string defaultTag = itemBase.Tags.First();

            for (int i = 0; i < 10; i++)
            {
                var generationType = i < 5 ? "prefix" : "suffix";
                Affix affix = getTestAffix("test_" + i, "test" + i, new Dictionary<string, int>() { { defaultTag, 100 } }, generationType);
                allAffixes.Add(affix);
            }

            return new AffixManager(itemBase, allAffixes, new List<Affix>());

        }

        private Affix getTestAffix(string name, string group, Dictionary<string, int> spawnTagWeights, string generationType, params string[] tags)
        {
            return new Affix { FullName = name, Name = name, Group = group, Tags = new HashSet<string>(), AddsTags = tags.ToList(), SpawnWeights = spawnTagWeights, GenerationWeights = new Dictionary<string, int>(), GenerationType = generationType };
        }
    }
}
