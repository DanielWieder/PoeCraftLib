using System;
using System.Collections.Generic;
using System.IO;
using DataJson;
using DataJson.Entities;
using DataJson.Query;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace PoeCraftLib.DataJsonTest
{
    [TestClass]
    public class DataJsonTest
    {
        [TestMethod]
        public void CanReadBaseItemJsonTest()
        {
            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\base_items.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, BaseItemJson>>(json);

            Assert.IsNotNull(deserialized);

            FetchItems fetchBaseItems = new FetchItems();
            var items = fetchBaseItems.Execute();
            Assert.IsNotNull(items);}

        [TestMethod]
        public void CanReadItemClassesJsonTest()
        {
            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\RePoE\\RePoE\\data\\item_classes.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, ItemClassJson>>(json);

            Assert.IsNotNull(deserialized);
        }

        [TestMethod]
        public void CanReadModsJsonTest()
        {
            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\mods.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, ModsJson>>(json);

            Assert.IsNotNull(deserialized);
        }

        [TestMethod]
        public void CanReadCraftingBenchJsonTest()
        {
            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\crafting_bench_options.json");
            var deserialized = JsonConvert.DeserializeObject<List<CraftingBenchJson>>(json);

            Assert.IsNotNull(deserialized);
        }

        [TestMethod]
        public void CanReadEssencesJsonTest()
        {
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\essences.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, EssenceJson>>(json, jsonSerializerSettings);



            Assert.IsNotNull(deserialized);
        }
        [TestMethod]
        public void CanReadFossilJsonTest()
        {
            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\fossils.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, FossilJson>>(json);

            Assert.IsNotNull(deserialized);
        }


        [TestMethod]
        public void CanReadModTypeJsonTest()
        {
            var json = File.ReadAllText("C:\\Users\\danie\\Documents\\GitHub\\PoeSimCraft\\PoeCrafting\\Data\\mod_types.json");
            var deserialized = JsonConvert.DeserializeObject<Dictionary<string, ModTypeJson>>(json);

            Assert.IsNotNull(deserialized);
        }
    }
}
