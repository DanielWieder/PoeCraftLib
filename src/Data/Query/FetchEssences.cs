using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataJson.Entities;
using Newtonsoft.Json;

namespace DataJson.Query
{
    public class FetchEssences : IFetchEssences
    {
        public List<EssenceJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\essences.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, EssenceJson>>(json);

            return deserialized
                .Select(x => { x.Value.FullName = x.Key; return x; })
                .Select(x => x.Value)
                .ToList();
        }
    }

    public interface IFetchEssences : IQueryObject<List<EssenceJson>>
    {
    }
}
 