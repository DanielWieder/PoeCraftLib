using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataJson.Entities;
using Newtonsoft.Json;

namespace DataJson.Query
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
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\mods.json");
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
