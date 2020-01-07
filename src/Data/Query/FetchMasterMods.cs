using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataJson.Entities;
using Newtonsoft.Json;
using PoeCrafting.Data.Query;

namespace DataJson.Query
{
    public class FetchMasterMods : IFetchMasterMods
    {
        public List<CraftingBenchJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource($"Assets\\crafting_bench_options.json", assem);
            var deserialized = JsonConvert.DeserializeObject<List<CraftingBenchJson>>(json);

            return deserialized
                .Select(x => { x.FullName = x.ModId; return x; })
                .ToList();
        }
    }

    public interface IFetchMasterMods : IQueryObject<List<CraftingBenchJson>>
    {
    }
}
