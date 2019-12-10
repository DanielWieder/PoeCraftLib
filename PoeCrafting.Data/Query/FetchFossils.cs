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
    public class FetchFossils : IFetchFossils
    {
        public List<FossilJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\fossils.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, FossilJson>>(json);

            return deserialized
                .Select(x => { x.Value.FullName = x.Key; return x; })
                .Select(x => x.Value)
                .ToList();
        }
    }

    public interface IFetchFossils : IQueryObject<List<FossilJson>>
    {
    }
}

