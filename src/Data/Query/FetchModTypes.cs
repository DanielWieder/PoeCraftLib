using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PoeCraftLib.Data.Entities;

namespace PoeCraftLib.Data.Query
{
    public class FetchModType : IFetchModType
    {
        public List<ModTypeJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\mod_types.json", assem);
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