﻿using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PoeCraftLib.Data.Query;

namespace PoeCraftLib.DataTest
{
    [TestClass]
    public class PoeNinjaTest
    {
        private readonly String _defaultLeague = "Standard";

        [TestMethod]
        public void FetchCurrencyValuesTest()
        {
            IFetchCurrencyValues currencyValues = new FetchCurrencyValues();
            currencyValues.League = _defaultLeague;
            var data = currencyValues.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 1);
            Assert.IsTrue(data.Keys.All(x => !string.IsNullOrEmpty(x)));
            Assert.IsTrue(data.Values.All(x => x != 0));
        }

        [TestMethod]
        public void FetchFossilValuesTest()
        {
            IFetchFossilValues fossilValues = new FetchFossilValues();
            fossilValues.League = _defaultLeague;
            var data = fossilValues.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 1);
            Assert.IsTrue(data.Keys.All(x => !string.IsNullOrEmpty(x)));
            Assert.IsTrue(data.Values.All(x => x != 0));
        }

        [TestMethod]
        public void FetchResonatorValuesTest()
        {
            IFetchResonatorValues currencyValues = new FetchResonatorValues();
            currencyValues.League = _defaultLeague;
            var data = currencyValues.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 1);
            Assert.IsTrue(data.Keys.All(x => !string.IsNullOrEmpty(x)));
            Assert.IsTrue(data.Values.All(x => x != 0));
        }

        [TestMethod]
        public void FetchEssenceValuesTest()
        {
            IFetchEssenceValues essenceValues = new FetchEssenceValues();
            essenceValues.League = _defaultLeague;
            var data = essenceValues.Execute();

            Assert.IsNotNull(data);
            Assert.IsTrue(data.Count > 1);
            Assert.IsTrue(data.Keys.All(x => !string.IsNullOrEmpty(x)));
            Assert.IsTrue(data.Values.All(x => x != 0));
        }
    }
}
