using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DataJson.Entities;
using Newtonsoft.Json;
using PoeCrafting.Data.Query;

namespace DataJson.Query
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