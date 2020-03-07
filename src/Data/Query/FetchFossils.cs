using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PoeCraftLib.Data.Entities;

namespace PoeCraftLib.Data.Query
{
    public class FetchFossils : IFetchFossils
    {
        public List<FossilJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\ggpk\\fossils.json", assem);
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

