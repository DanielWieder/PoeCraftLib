using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataJson.Entities;
using Newtonsoft.Json;

namespace DataJson.Query
{
    public class FetchModType : IFetchModType
    {
        public List<ModTypeJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\mod_types.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, ModTypeJson>>(json);

            return deserialized
                .Select(x => { x.Value.FullName = x.Key; return x.Value; })
                .ToList();
        }
    }

    public interface IFetchModType : IQueryObject<List<ModTypeJson>>
    {
    }
}