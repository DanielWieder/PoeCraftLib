using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataJson.Entities;
using Newtonsoft.Json;

namespace DataJson.Query
{
    public class FetchItemClass : IFetchItemClass
    {
        public List<ItemClassJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\item_classes.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, ItemClassJson>>(json);

            return deserialized
                .Select(x => { x.Value.FullName = x.Key; return x; })
                .Select(x => x.Value)
                .ToList();
        }

    }

    public interface IFetchItemClass : IQueryObject<List<ItemClassJson>>
    {
    }
}