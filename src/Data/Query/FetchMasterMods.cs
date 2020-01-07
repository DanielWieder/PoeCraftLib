using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataJson.Entities;
using Newtonsoft.Json;

namespace DataJson.Query
{
    public class FetchMasterMods : IFetchMasterMods
    {
        public List<CraftingBenchJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\MasterMods.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, CraftingBenchJson>>(json);

            return deserialized
                .Select(x => { x.Value.FullName = x.Key; return x; })
                .Select(x => x.Value)
                .ToList();
        }
    }

    public interface IFetchMasterMods : IQueryObject<List<CraftingBenchJson>>
    {
    }
}
