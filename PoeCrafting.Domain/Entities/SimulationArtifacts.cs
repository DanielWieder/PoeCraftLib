using System.Collections.Generic;
using PoeCrafting.Entities.Items;

namespace PoeCrafting.Domain.Entities
{
    public class SimulationArtifacts
    {
        public Dictionary<CraftingTarget, List<Equipment>> MatchingGeneratedItems { get; set; } = new Dictionary<CraftingTarget, List<Equipment>>();

        public List<Equipment> AllGeneratedItems = new List<Equipment>();

        public Dictionary<string, int> CurrencyUsed { get; set; } = new Dictionary<string, int>();

        public double CostInChaos { get; set; } = 0;
    }
}
