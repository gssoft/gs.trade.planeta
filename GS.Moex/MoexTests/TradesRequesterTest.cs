using System;
using System.Reflection;
using GS.ConsoleAS;
using GS.Extension;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;
using Moex.TradesRequester;

namespace MoexTests
{
    [TestClass]
    public class TradesRequesterTest
    {
        public TradesRequester TradesRequester { get; set; }
        private const bool IsLongTimeTestEnabled = false;

        [TestInitialize]
        public void SetUp()
        {
            TradeRequestBuildInstanceTest();
        }

        [TestMethod]
        public void TradeRequestBuildInstanceTest()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            TradesRequester = Builder.Build2<TradesRequester>(@"Init\TradesRequester.xml", "TradesRequester");
            Assert.IsNotNull(TradesRequester, "TrReq is null");
            Assert.AreEqual("RFUD", TradesRequester.TickerBoard);
            Assert.AreEqual("SiZ9", TradesRequester.TickerCode);
            Assert.AreEqual(0, TradesRequester.TradesRequestStartValue);
            Assert.AreEqual(1000, TradesRequester.TradesRequestLimitValue);

            Assert.IsNotNull(TradesRequester.MoexTest, "Moex Is Null");
            Assert.IsTrue(TradesRequester.MoexTest.TickersFuturesApiPrefix.HasValue());
            Assert.IsTrue(TradesRequester.MoexTest.TickersOptionsApiPrefix.HasValue());
        }
        [TestMethod]
        public void TradeRequestInitInstanceTest()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            Assert.IsNotNull(TradesRequester, "TrReq is null");
            Assert.IsNotNull(TradesRequester.MoexTest, "Moex Is Null");
            TradesRequester.Init();
            Assert.IsNotNull(TradesRequester.Ticker, "Ticker is null");
            ConsoleSync.WriteLineT(TradesRequester.Ticker.ToString());      
        }
        [TestMethod]
        public void TradeRequestGetTradeTest()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            if (!IsLongTimeTestEnabled) return;

            Assert.IsNotNull(TradesRequester, "TrReq is null");
            Assert.IsNotNull(TradesRequester.MoexTest, "Moex Is Null");
            TradesRequester.Init();
            Assert.IsNotNull(TradesRequester.Ticker, "Ticker is null");
            ConsoleSync.WriteLineT(TradesRequester.Ticker.ToString());
            TradesRequester.GetTrades();
        }
    }
}
