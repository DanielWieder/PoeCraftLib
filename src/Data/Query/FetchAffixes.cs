using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PoeCraftLib.Data.Entities;

namespace PoeCraftLib.Data.Query
{
    public class FetchAffixes : IFetchAffixes
    {
        private static readonly HashSet<string> GenerationTypeBlacklist = new HashSet<string>()
        {
            { "blight_tower" },
            { "unique" },
            { "tempest" },
            { "enchantment" }
        };

        private static readonly HashSet<string> DomainBlacklist = new HashSet<string>()
        {
            { "flask" },
            { "area" },
            { "atlas" }
        };


        public List<ModsJson> Execute()
        {
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\mods.json", assem);
            var temp = JsonConvert.DeserializeObject<Dictionary<string, ModsJson>>(json)
                .Where(x => !GenerationTypeBlacklist.Contains(x.Value.GenerationType))
                .Where(x => !DomainBlacklist.Contains(x.Value.Domain))
                .Select(x => { x.Value.FullName = x.Key; return x; })
                .Select(x => x.Value)
                .ToList();

            var temp2 = temp.Select(x => x.AddsTags).SelectMany(x => x).Distinct().ToList();

            return temp;
        }
    }

    public interface IFetchAffixes : IQueryObject<List<ModsJson>>
    {
    }
}
