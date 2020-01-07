using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataJson.Entities;
using Newtonsoft.Json;

namespace DataJson.Query
{
    public class FetchItems : IFetchBaseItems
    {
        private static readonly HashSet<string> ItemClassBlacklist = new HashSet<string>()
        {
            { "Active Skill Gem" },
            { "Support Skill Gem" },
            { "DivinationCard" },
            { "HybridFlask" },
            { "LifeFlask" },
            { "ManaFlask" },
            { "UtilityFlask" },
            { "UtilityFlaskCritical" }
        };

        public List<BaseItemJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\base_items.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, BaseItemJson>>(json);

            return deserialized
                .Where(IsReleased)
                .Where(IsSupportedType)
                .Select(x => { x.Value.FullName = x.Key; return x; })
                .Select(x => x.Value)
                .ToList();
        }

        private static bool IsReleased(KeyValuePair<string, BaseItemJson> x)
        {
            return x.Value.ReleaseState.Equals("released");
        }

        private static bool IsSupportedType(KeyValuePair<string, BaseItemJson> x)
        {
            return !ItemClassBlacklist.Contains(x.Value.ItemClass);
        }
    }

    public interface IFetchBaseItems : IQueryObject<List<BaseItemJson>>
    {
    }
}
 