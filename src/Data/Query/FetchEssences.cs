using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PoeCraftLib.Data.Entities;

namespace PoeCraftLib.Data.Query
{
    public class FetchEssences : IFetchEssences
    {
        public List<EssenceJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\ggpk\\essences.json", assem);
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
 