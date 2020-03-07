using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PoeCraftLib.Data.Entities;

namespace PoeCraftLib.Data.Query
{
    public class FetchMasterMods : IFetchMasterMods
    {
        private static readonly HashSet<string> MasterBlacklist = new HashSet<string>()
        {
            { "Zana" }
        };


        public List<CraftingBenchJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource($"Assets\\ggpk\\crafting_bench_options.json", assem);
            var deserialized = JsonConvert.DeserializeObject<List<CraftingBenchJson>>(json);

            return deserialized
                .Where(x => !MasterBlacklist.Contains(x.Master))
                .Select(x => { x.FullName = x.ModId; return x; })
                .ToList();
        }
    }

    public interface IFetchMasterMods : IQueryObject<List<CraftingBenchJson>>
    {
    }
}
