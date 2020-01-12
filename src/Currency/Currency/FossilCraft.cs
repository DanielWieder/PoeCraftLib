using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoeCraftLib.Entities.Constants;
using PoeCraftLib.Entities;
using PoeCraftLib.Entities.Items;

namespace PoeCraftLib.Currency.Currency
{
    public class FossilCraft : ICurrency
    {
        private Dictionary<string, int> _normalCurrency;
        private Dictionary<string, int> _rareCurrency;


        private readonly IRandom _random;

        public string Name => "Fossil";

        public List<Fossil> Fossils { get; set; }

        private List<Essence> _corruptionOnlyEssences;

        private Dictionary<int, String> _chaoticResonators = new Dictionary<int, string>()
        {
            {1, "Primitive Chaotic Resonator"},
            {2, "Potent Chaotic Resonator"},
            {3, "Powerful Chaotic Resonator"},
            {4, "Prime Chaotic Resonator"}
        };

        private Dictionary<int, String> _alchemicalResonators = new Dictionary<int, string>()
        {
            {1, "Primitive Alchemical Resonator"},
            {2, "Potent Alchemical Resonator"},
            {3, "Powerful Alchemical Resonator"},
            {4, "Prime Alchemical Resonator"}
        };

        public FossilCraft(IRandom random, List<Fossil> fossils, List<Essence> essences)
        {
            _random = random;
            this.Fossils = fossils;
            this._corruptionOnlyEssences = essences.Where(x => x.Tier == 6).ToList();

            if (Fossils.Count <= 0 || Fossils.Count > 4) throw new ArgumentException("A FossilCraft must have between 1-4 fossils");

            this._normalCurrency = Fossils.ToDictionary(x => x.Name, x => 1);
            this._normalCurrency.Add(_alchemicalResonators[Fossils.Count], 1);

            this._rareCurrency = Fossils.ToDictionary(x => x.Name, x => 1);
            this._normalCurrency.Add(_chaoticResonators[Fossils.Count], 1);
        }

        public Dictionary<string, int> Execute(Equipment item, AffixManager affixManager)
        {
            if (item.Corrupted || item.Rarity == EquipmentRarity.Magic || item.Rarity == EquipmentRarity.Unique)
            {
                return new Dictionary<string, int>();
            }

            var currency = item.Rarity == EquipmentRarity.Normal ? _normalCurrency : _rareCurrency;

            var corruptedEssenceChance = Fossils.Max(x => x.CorruptedEssenceChance);

            bool addCorruptedEssence = false;

            if (corruptedEssenceChance == 100)
            {
                addCorruptedEssence = true;
            }
            else if (corruptedEssenceChance > 0)
            {
                var chance = _random.Next(100);
                if (chance >= corruptedEssenceChance)
                {
                    addCorruptedEssence = true;
                }
            }

            item.Stats.Clear();

            if (addCorruptedEssence)
            {
                var essence = _corruptionOnlyEssences[_random.Next(_corruptionOnlyEssences.Count)];

                StatFactory.AddExplicit(_random, item, essence.ItemClassToMod[item.ItemBase.ItemClass]);
            }

            StatFactory.AddExplicits(_random, item, affixManager, StatFactory.RareAffixCountOdds);

            return currency;
        }

        public bool IsWarning(ItemStatus status)
        {
            return !IsError(status) && status.Rarity != EquipmentRarity.Rare && status.Rarity != EquipmentRarity.Normal;
        }

        public bool IsError(ItemStatus status)
        {
            return (status.Rarity & EquipmentRarity.Rare) != EquipmentRarity.Rare && 
                   (status.Rarity & EquipmentRarity.Normal) != EquipmentRarity.Normal || 
                   status.IsCorrupted;
        }

        public ItemStatus GetNextStatus(ItemStatus status)
        {
            if (IsError(status))
            {
                return status;
            }
            if (IsWarning(status))
            {
                status.MinPrefixes = Math.Min(1, status.MinPrefixes);
                status.MinSuffixes = Math.Min(1, status.MinSuffixes);
                status.MinAffixes = Math.Min(4, status.MinAffixes);

                status.MaxPrefixes = Math.Max(3, status.MaxPrefixes);
                status.MaxSuffixes = Math.Max(3, status.MaxSuffixes);
                status.MaxAffixes = Math.Max(6, status.MaxAffixes);
            }
            else
            {
                status.MinPrefixes = 1;
                status.MinSuffixes = 1;
                status.MinAffixes = 4;

                status.MaxPrefixes = 3;
                status.MaxSuffixes = 3;
                status.MaxAffixes = 6;
            }

            return status;
        }
    }
}
