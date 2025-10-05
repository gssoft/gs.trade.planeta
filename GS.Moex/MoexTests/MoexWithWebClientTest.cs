using System;
using System.CodeDom;
using System.Net;
using GS.ConsoleAS;
using GS.Extension;
using GS.Serialization;
// using GS.Trade.Moex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;
using GS.Moex.Interfaces;
using WebClients;

namespace MoexTests
{
    [TestClass]
    public class MoexWithWebClientTest
    {
        public MoexTest Moex;

        [TestInitialize]
        public void SetUp()
        {
            TestInitMoexInstance();
        }
        [TestMethod]
        public void TestInitMoexInstance()
        {
            //Moex = Builder.Build2<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Moex = Builder.Build3<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Assert.IsNotNull(Moex, "Should Be Not Null");
            Assert.IsTrue(Moex.FuturesIssWebApiPrefix.HasValue());
            Assert.IsTrue(Moex.OptionsIssWebApiPrefix.HasValue());
            Assert.IsTrue(Moex.SecurityRowXPath.HasValue());
            Assert.IsNotNull(Moex.WebClient02);
            Assert.IsInstanceOfType(Moex.WebClient02, typeof (WebClient02));
            Moex.Init();
            Assert.IsNotNull(Moex.WebClient02, "Mex.WeClient is Not Null");
            ConsoleSync.WriteLineT("Moex.Init() Ok");
            // Assert.AreEqual(Moex.ApiPrefix, @"iss/engines/futures/markets/");
            // Assert.AreEqual(WebClient.BaseAddress, @"https://iss.moex.com/");
        }

        [TestMethod]
        public void TestGetTickerFuturesFromMoex()
        {
            var tickercode = "SiZ0";
            var tickerclass = "SPBFUT";
            var ticker = Moex.GetTicker(tickerclass, tickercode);
            Assert.IsNotNull(ticker);
            Assert.AreEqual(tickercode, ticker.SecId);
            Assert.AreEqual(tickercode, ticker.LatName);
            Assert.AreEqual("Si", ticker.SecType);
            Assert.AreEqual("Si-12.20", ticker.ShortName);
            Assert.AreEqual("RFUD", ticker.BoardId);
            Assert.AreEqual(0, ticker.Decimals);
            Assert.AreEqual(1, ticker.MinStep);
            Assert.AreEqual(1d, ticker.StepPrice);
            Assert.IsTrue(ticker.IsFutures);
            Assert.IsFalse(ticker.IsOption);
            Assert.AreEqual(MoexTickerTypeEnum.Futures, ticker.TickerType);
            ConsoleSync.WriteLineT(ticker.ToString());

            tickercode = "BRZ9";
            ticker = Moex.GetTicker(tickerclass, tickercode);
            Assert.IsNotNull(ticker);
            Assert.AreEqual(tickercode, ticker.SecId);
            Assert.AreEqual(tickercode, ticker.LatName);
            Assert.AreEqual("BR", ticker.SecType);
            Assert.AreEqual("BR-12.19", ticker.ShortName);
            Assert.AreEqual("RFUD", ticker.BoardId);
            Assert.AreEqual(2, ticker.Decimals);
            Assert.AreEqual(0.01, ticker.MinStep);
            //  Assert.AreEqual(1d, ticker.StepPrice);
            Assert.IsTrue(ticker.IsFutures);
            Assert.IsFalse(ticker.IsOption);
            Assert.AreEqual(MoexTickerTypeEnum.Futures, ticker.TickerType);
            ConsoleSync.WriteLineT(ticker.ToString());

            var ticker1 = Moex.GetTicker(tickercode);
            Assert.IsNotNull(ticker1);
            ConsoleSync.WriteLineT(ticker1.ToString());

            Assert.AreEqual(ticker.ToString(), ticker1.ToString(), "Match 2 Tickers");
            ConsoleSync.WriteLineT("Match Ok");

            tickercode = "BRZ6";
            ticker = Moex.GetTicker(tickerclass, tickercode);
            Assert.IsNull(ticker);

        }
       // [TestMethod]
        //public void GetTickerOptionFromMoexTest()
        //{
        //    var tickercode = "Si64500BX0";
        //    var tickerclass = "SPBOPT";

        //    var ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNotNull(ticker);
        //    Assert.AreEqual(tickercode, ticker.SecId);
        //    Assert.AreEqual(tickercode, ticker.LatName);
        //    //Assert.AreEqual("Si", ticker.SecType);
        //    //Assert.AreEqual("Si-12.20", ticker.ShortName);
        //    Assert.IsTrue(ticker.ShortName.HasValue());
        //    Assert.AreEqual("ROPD", ticker.BoardId);
        //    Assert.AreEqual(0, ticker.Decimals, "Decimals");
        //    Assert.AreEqual(1, ticker.MinStep, "MinStep");
        //    // Assert.AreEqual(1d, ticker.StepPrice, "StepPrice"); // StepPrice is Absent in the XmlRow
        //    Assert.IsFalse(ticker.IsFutures);
        //    Assert.IsTrue(ticker.IsOption);
        //    Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType);
        //    ConsoleSync.WriteLineT(ticker.ToString());

        //    tickercode = "Si64500BI0";
        //    tickerclass = "SPBOPT";

        //    ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNotNull(ticker);
        //    Assert.AreEqual(tickercode, ticker.SecId);
        //    Assert.AreEqual(tickercode, ticker.LatName);
        //    //Assert.AreEqual("Si", ticker.SecType);
        //    //Assert.AreEqual("Si-12.20", ticker.ShortName);
        //    Assert.IsTrue(ticker.ShortName.HasValue());
        //    Assert.AreEqual("ROPD", ticker.BoardId);
        //    Assert.AreEqual(0, ticker.Decimals, "Decimals");
        //    Assert.AreEqual(1, ticker.MinStep, "MinStep");
        //    // Assert.AreEqual(1d, ticker.StepPrice, "StepPrice"); // StepPrice is Absent in the XmlRow
        //    Assert.IsFalse(ticker.IsFutures);
        //    Assert.IsTrue(ticker.IsOption);
        //    Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType);
        //    ConsoleSync.WriteLineT(ticker.ToString());

        //    // Errors
        //    tickercode = "Si64500BU9";
        //    tickerclass = "SPBOPT";
        //    ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNull(ticker);

        //    tickercode = "Si64500BU9";
        //    tickerclass = "";
        //    ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNull(ticker);

        //    tickercode = "";
        //    tickerclass = "SPBOPT";
        //    ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNull(ticker);

        //    //tickercode = "BR000059BJ9";
        //    //tickerclass = "SPBOPT";
        //    //ticker = Moex.GetTicker(tickerclass, tickercode);
        //    //Assert.IsNotNull(ticker);
        //    //ConsoleSync.WriteLineT(ticker.ToString());

        //    tickercode = "BR59BJ9";
        //    tickerclass = "SPBOPT";
        //    ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNotNull(ticker);
        //    ConsoleSync.WriteLineT(ticker.ToString());

        //    //tickercode = "BR000059BV9";
        //    //tickerclass = "SPBOPT";
        //    //ticker = Moex.GetTicker(tickerclass, tickercode);
        //    //Assert.IsNotNull(ticker);
        //    //ConsoleSync.WriteLineT(ticker.ToString());

        //    tickercode = "BR59BV9";
        //    tickerclass = "SPBOPT";
        //    ticker = Moex.GetTicker(tickerclass, tickercode);
        //    Assert.IsNotNull(ticker);
        //    ConsoleSync.WriteLineT(ticker.ToString());

        //    tickercode = "BR59BV9";
        //    ticker = Moex.GetTicker(tickercode);
        //    Assert.IsNotNull(ticker);
        //    ConsoleSync.WriteLineT(ticker.ToString());
        //}
     }
}
