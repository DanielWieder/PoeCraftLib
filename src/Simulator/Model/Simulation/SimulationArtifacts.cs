using System.Collections.Generic;
using PoeCraftLib.Simulator.Model.Items;

namespace PoeCraftLib.Simulator.Model.Simulation
{
    public class SimulationArtifacts
    {
        public Dictionary<string, List<Equipment>> MatchingGeneratedItems { get; set; } = new Dictionary<string, List<Equipment>>();

        public List<Equipment> AllGeneratedItems = new List<Equipment>();

        public Dictionary<string, int> CurrencyUsed { get; set; } = new Dictionary<string, int>();

        public double CostInChaos { get; set; } = 0;
    }
}
