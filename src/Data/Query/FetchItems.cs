using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using PoeCraftLib.Data.Entities;

namespace PoeCraftLib.Data.Query
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
            Assembly assem = this.GetType().Assembly;
            var json = FetchHelper.GetEmbeddedResource("Assets\\base_items.json", assem);
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
 