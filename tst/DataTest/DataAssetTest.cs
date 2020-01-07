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
    public class DataAssetTest
    {
        [TestMethod]
        public void CanFetchAffixes()
        {
            var fetch = new FetchAffixes();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }

        [TestMethod]
        public void CanFetchEssences()
        {
            var fetch = new FetchEssences();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }

        [TestMethod]
        public void CanFetchFossils()
        {
            var fetch = new FetchFossils();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }

        [TestMethod]
        public void CanFetchItems()
        {
            var fetch = new FetchItems();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }

        [TestMethod]
        public void CanFetchMasterMods()
        {
            var fetch = new FetchMasterMods();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }

        [TestMethod]
        public void CanFetchModTypes()
        {
            var fetch = new FetchModType();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }

        [TestMethod]
        public void CanFetchItemClass()
        {
            var fetch = new FetchItemClass();
            var data = fetch.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 0);
        }
    }
}
