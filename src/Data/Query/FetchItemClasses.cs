using System.Collections.Generic;
using System.IO;
using System.Linq;
using DataJson.Entities;
using Newtonsoft.Json;
using PoeCraftLib.Entities;

namespace DataJson.Query
{
    public class FetchItemClass : IFetchItemClass
    {
        private const string CrusaderTag = "crusader_tag";
        private const string ElderTag = "elder_tag";
        private const string HunterTag = "hunter_tag";
        private const string RedeemerTag = "redeemer_tag";
        private const string ShaperTag = "shaper_tag";
        private const string WarlordTag = "warlord_tag";

        public List<ItemClassJson> Execute()
        {
            var json = File.ReadAllText(
                "C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\item_classes.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

            return deserialized
                .Select(x => new ItemClassJson()
                {
                    FullName = x.Key,
                    InfluenceTags = new Dictionary<Influence, string>()
                    {
                        { Influence.Crusader, x.Value[CrusaderTag] },
                        { Influence.Elder, x.Value[ElderTag] },
                        { Influence.Hunter, x.Value[HunterTag] },
                        { Influence.Redeemer, x.Value[RedeemerTag] },
                        { Influence.Shaper, x.Value[ShaperTag] },
                        { Influence.Warlord, x.Value[WarlordTag] }
                    }
                }).Where(x => x.InfluenceTags.All(y => y.Value != null))
                .ToList();
        }
    }

    public interface IFetchItemClass : IQueryObject<List<ItemClassJson>>
    {
    }
}