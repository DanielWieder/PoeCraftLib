using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DataJson.Entities;
using Newtonsoft.Json;
using PoeCrafting.Data.Query;

namespace DataJson.Query
{
    public class FetchFossils : IFetchFossils
    {
        public List<FossilJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\fossils.json", assem);
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

