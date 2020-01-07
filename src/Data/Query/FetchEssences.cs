using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DataJson.Entities;
using Newtonsoft.Json;
using PoeCrafting.Data.Query;

namespace DataJson.Query
{
    public class FetchEssences : IFetchEssences
    {
        public List<EssenceJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\essences.json", assem);
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
 