using System;
using System.Reflection;
using System.Xml.Linq;
using GS.ConsoleAS;
using GS.Extension;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;
using WebClients;
using GS.Moex.Interfaces;
// using GS.Trade.Moex;

namespace MoexTests
{
    [TestClass]
    public class MoexGetTickerFromXDoc
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
            var methodname = MethodBase.GetCurrentMethod()?.Name + "()";
            //Moex = Builder.Build2<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Moex = Builder.Build3<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Assert.IsNotNull(Moex, "Should Be Not Null");
            Assert.IsTrue(Moex.FuturesIssWebApiPrefix.HasValue());
            Assert.IsTrue(Moex.OptionsIssWebApiPrefix.HasValue());
            Assert.IsTrue(Moex.SecurityRowXPath.HasValue());
            Assert.IsNotNull(Moex.WebClient02);
            Assert.IsInstanceOfType(Moex.WebClient02, typeof(WebClient02));
            Moex.Init();
            Assert.IsNotNull(Moex.WebClient02, "Mex.WeClient is Not Null");

            // Assert.AreEqual(Moex.ApiPrefix, @"iss/engines/futures/markets/");
            // Assert.AreEqual(WebClient.BaseAddress, @"https://iss.moex.com/");

            ConsoleSync.WriteLineT(methodname + " Ok");
        }
        [TestMethod]
        public void MoexGetFuturesFromXmlFile()
        {
            var methodname = MethodBase.GetCurrentMethod()?.Name + "()";
            {
                var xmlfile = @"Init\SiZ9.xml";
                var tickercode = "SiZ9";
                var shortname = "Si-12.19";
                var sectype = "Si";
                var board = "RFUD";
                var secname = "Фьючерсный контракт Si-12.19";
                var decimals = 0;
                var minstep = 1;
                var stepprice = 1;
                var lotvolume = 1000.0d;
                var initialmargin = 4294.29;
                var firsttradedatestr = "2017-12-15";
                var lasttradedatestr = "2019-12-19";
                var firsttradedate = new DateTime(2017, 12, 15);
                var lasttradedate = new DateTime(2019, 12, 19);
                
                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsTrue(ticker.IsFutures, "Futures");
                Assert.IsFalse(ticker.IsOption);
                Assert.AreEqual(MoexTickerTypeEnum.Futures, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                Assert.AreEqual(sectype, ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(sectype, ticker.AssetCode, "AssetCode");
                Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume");
                Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin");
                Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            {
                var xmlfile = @"Init\BRZ9.xml";
                var tickercode = "BRZ9";
                var shortname = "BR-12.19";
                var sectype = "BR";
                var board = "RFUD";
                var secname = "Фьючерсный контракт BR-12.19";
                var decimals = 2;
                var minstep = 0.01;
                var stepprice = 6.39373;
                var lotvolume = 10.0d;
                var initialmargin = 6498.32;
                var firsttradedatestr = "2018-11-27";
                var lasttradedatestr = "2019-12-02";
                var firsttradedate = new DateTime(2018, 11, 27);
                var lasttradedate = new DateTime(2019, 12, 02);

                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsTrue(ticker.IsFutures, "Futures");
                Assert.IsFalse(ticker.IsOption);
                Assert.AreEqual(MoexTickerTypeEnum.Futures, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                Assert.AreEqual(sectype, ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(sectype, ticker.AssetCode, "AssetCode");
                Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume");
                Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin");
                Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            ConsoleSync.WriteLineT(methodname + " Ok");
        }
        [TestMethod]
        public void MoexGetOptionsFromXmlFile()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            {
                // GMKR
                var xmlfile = @"Init\GM205000BO0.xml";
                var tickercode = "GM205000BO0";
                var shortname = "GMKR-3.20M180320PA205000";
                var sectype = ""; // Is Absent in Options
                var board = "ROPD";
                var secname = "Марж. амер. Put 205000 с исп. 18 марта на фьюч. контр. GMKR-3.20";
                var assetcode = "GMKR";
                var decimals = 0;
                var minstep = 1;
                //var stepprice = 0.0;
                //var lotvolume = 00.0d; // Is Absent
                //var initialmargin = 0.0; // is Absent
                //var firsttradedatestr = "2018-11-27";
                //var lasttradedatestr = "2019-11-26";
                //var firsttradedate = new DateTime(2018, 11, 27);
                //var lasttradedate = new DateTime(2019, 11, 26);

                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsFalse(ticker.IsFutures, "Futures");
                Assert.IsTrue(ticker.IsOption, "Options");
                Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                //Assert.AreEqual(sectype, ticker.SecType, "SecType"); // Is Absent in Options
                Assert.IsNull(ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(assetcode, ticker.AssetCode, "AssetCode");
                //Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume"); // Is Absent in Options
                //Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin"); // Is Absent in Options
                //Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                //Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                //Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                //Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                //Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            {
                var xmlfile = @"Init\Si64500BJ9.xml";
                var tickercode = "Si64500BJ9";
                var shortname = "Si-12.19M171019CA64500";
                var sectype = ""; // Is Absent in Options
                var board = "ROPD";
                var secname = "Марж. амер. Call 64500 с исп. 17 окт. на фьюч. контр. Si-12.19";
                var assetcode = "Si";
                var decimals = 0;
                var minstep = 1;
                var stepprice = 0.0;
                var lotvolume = 00.0d; // Is Absent
                var initialmargin = 0.0; // is Absent
                var firsttradedatestr = "2017-12-15";
                var lasttradedatestr = "2019-10-17";
                var firsttradedate = new DateTime(2017, 12, 15);
                var lasttradedate = new DateTime(2019, 10, 17);

                // var prevPrice = 898.0;

                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsFalse(ticker.IsFutures, "Futures");
                Assert.IsTrue(ticker.IsOption, "Options");
                Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                //Assert.AreEqual(sectype, ticker.SecType, "SecType"); // Is Absent in Options
                Assert.IsNull(ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(assetcode, ticker.AssetCode, "AssetCode");
                Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume"); // Is Absent in Options
                Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin"); // Is Absent in Options
                Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                //Assert.AreEqual(prevPrice, ((MoexTicker)ticker).PrevPrice);

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            {
                var xmlfile = @"Init\Si64500BV9.xml";
                var tickercode = "Si64500BV9";
                var shortname = "Si-12.19M171019PA64500";
                var sectype = ""; // Is Absent in Options
                var board = "ROPD";
                var secname = "Марж. амер. Put 64500 с исп. 17 окт. на фьюч. контр. Si-12.19";
                var assetcode = "Si";
                var decimals = 0;
                var minstep = 1;
                var stepprice = 0.0;
                var lotvolume = 00.0d; // Is Absent
                var initialmargin = 0.0; // is Absent
                var firsttradedatestr = "2017-12-15";
                var lasttradedatestr = "2019-10-17";
                var firsttradedate = new DateTime(2017, 12, 15);
                var lasttradedate = new DateTime(2019, 10, 17);

                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsFalse(ticker.IsFutures, "Futures");
                Assert.IsTrue(ticker.IsOption, "Options");
                Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                //Assert.AreEqual(sectype, ticker.SecType, "SecType"); // Is Absent in Options
                Assert.IsNull(ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(assetcode, ticker.AssetCode, "AssetCode");
                Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume"); // Is Absent in Options
                Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin"); // Is Absent in Options
                Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            {
                // BR
                var xmlfile = @"Init\BR55BJ9.xml";
                var tickercode = "BR55BJ9";
                var shortname = "BR-11.19M281019CA55";
                var sectype = ""; // Is Absent in Options
                var board = "ROPD";
                var secname = "Марж. амер. Call 55 с исп. 28 окт. на фьюч. контр. BR-11.19";
                var assetcode = "BR";
                var decimals = 2;
                var minstep = 0.01;
                var stepprice = 0.0;
                var lotvolume = 00.0d; // Is Absent
                var initialmargin = 0.0; // is Absent
                var firsttradedatestr = "2018-05-25";
                var lasttradedatestr = "2019-10-28";
                var firsttradedate = new DateTime(2018, 05, 25);
                var lasttradedate = new DateTime(2019, 10, 28);

                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsFalse(ticker.IsFutures, "Futures");
                Assert.IsTrue(ticker.IsOption, "Options");
                Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                //Assert.AreEqual(sectype, ticker.SecType, "SecType"); // Is Absent in Options
                Assert.IsNull(ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(assetcode, ticker.AssetCode, "AssetCode");
                Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume"); // Is Absent in Options
                Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin"); // Is Absent in Options
                Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            {
                // BR
                var xmlfile = @"Init\BR55BW9.xml";
                var tickercode = "BR55BW9";
                var shortname = "BR-12.19M261119PA55";
                var sectype = ""; // Is Absent in Options
                var board = "ROPD";
                var secname = "Марж. амер. Put 55 с исп. 26 нояб. на фьюч. контр. BR-12.19";
                var assetcode = "BR";
                var decimals = 2;
                var minstep = 0.01;
                var stepprice = 0.0;
                var lotvolume = 00.0d; // Is Absent
                var initialmargin = 0.0; // is Absent
                var firsttradedatestr = "2018-11-27";
                var lasttradedatestr = "2019-11-26";
                var firsttradedate = new DateTime(2018, 11, 27);
                var lasttradedate = new DateTime(2019, 11, 26);

                var ticker = Moex.GetTickerFromFile(xmlfile);

                Assert.IsNotNull(ticker, "Ticker is Not Null");
                Assert.IsFalse(ticker.IsFutures, "Futures");
                Assert.IsTrue(ticker.IsOption, "Options");
                Assert.AreEqual(MoexTickerTypeEnum.Option, ticker.TickerType, "Instrument");
                Assert.AreEqual(tickercode, ticker.SecId, "SecId");
                Assert.AreEqual(board, ticker.BoardId, "BoardId");
                Assert.AreEqual(shortname, ticker.ShortName, "ShortName");
                Assert.AreEqual(secname, ticker.SecName, "SecName");
                Assert.AreEqual(decimals, ticker.Decimals, "Decimals");
                Assert.AreEqual(minstep, ticker.MinStep, "MinStep");
                //Assert.AreEqual(sectype, ticker.SecType, "SecType"); // Is Absent in Options
                Assert.IsNull(ticker.SecType, "SecType");
                Assert.AreEqual(tickercode, ticker.LatName, "LatName");
                Assert.AreEqual(assetcode, ticker.AssetCode, "AssetCode");
                Assert.AreEqual(lotvolume, ticker.LotVolume, "LotVolume"); // Is Absent in Options
                Assert.AreEqual(initialmargin, ticker.InitialMargin, "InitialMargin"); // Is Absent in Options
                Assert.AreEqual(stepprice, ticker.StepPrice, "StepPrice");
                Assert.AreEqual(firsttradedatestr, ticker.FirstTradeDateStr, "FirstTradeDateStr");
                Assert.AreEqual(lasttradedatestr, ticker.LastTradeDateStr, "LastTradeDateStr");
                Assert.AreEqual(firsttradedate, ticker.FirstTradeDate, "FirstTradeDate");
                Assert.AreEqual(lasttradedate, ticker.LastTradeDate, "LastTradeDate");

                ConsoleSync.WriteLineT(ticker.ToString());
            }
            
            ConsoleSync.WriteLineT(methodname + " Ok");
        }
    }
}
