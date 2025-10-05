using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using GS.ConsoleAS;
using GS.Extension;
using GS.Serialization;
//using GS.Trade.Moex;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;
using GS.Moex.Interfaces;
using WebClients;

namespace MoexTests
{
    [TestClass]
    public class MoexTickersTest
    {
        public MoexTest Moex;

        //<!-- Moex Tickers-->
        //<TickersFuturesApiPrefix>futures/markets/forts/securities</TickersFuturesApiPrefix>
        //<TickersOptionsApiPrefix>futures/markets/options/securities</TickersOptionsApiPrefix>
        //<TickersApiPostfix>.xml</TickersApiPostfix>
        //<TickersRowsXPath>/document/data[@id = 'securities']/rows</TickersRowsXPath>

        private string _tickersFuturesApiPrefix = @"futures/markets/forts/securities";
        private string _tickersOptionsApiPrefix = @"futures/markets/options/securities";
        private string _tickersApiPostfix = @".xml";
        private string _tickersFuturesLink = @"futures/markets/forts/securities.xml";
        private string _tickersOptionsLink = @"futures/markets/options/securities.xml";
        private string _tickersRowsXPath = @"/document/data[@id='securities']/rows";

        private string _tickersFuturesXDocPath = @"Samples\TickersFutures_01.xml";
        private string _tickersOptionsXDocPath = @"Samples\TickersOptions_01.xml";
        private XElement _tickersXmlRows;

        private XDocument _xDoc;
        private XElement _tradesXElement;
        private readonly List<MoexTrade> _tradesList;
        private XNode _tradesXmlDocReply;

        //[TestInitialize]
        //public void SetUp()
        //{
        //    TestInitMoexInstance();
        //}
        [TestInitialize]
        public void TestInitMoexInstance()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            //Moex = Builder.Build2<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Moex = Builder.Build3<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Assert.IsNotNull(Moex, "Moex Is Null");

            Assert.IsTrue(Moex.TickersFuturesApiPrefix.HasValue());
            Assert.AreEqual(_tickersFuturesApiPrefix, Moex.TickersFuturesApiPrefix, "TickersFuturesApiPrefix is Wrong");
            Assert.IsTrue(Moex.TickersOptionsApiPrefix.HasValue());
            Assert.AreEqual(_tickersOptionsApiPrefix, Moex.TickersOptionsApiPrefix, "TickersOptionsApiPrefix is Wrong");
            Assert.IsTrue(Moex.TickersApiPostfix.HasValue());
            Assert.AreEqual(_tickersApiPostfix, Moex.TickersApiPostfix, "TickersApiPostfix is Wrong");
            Assert.IsTrue(Moex.TickersRowsXPath.HasValue());
            Assert.AreEqual(_tickersRowsXPath, Moex.TickersRowsXPath, "TickersRowXPath is Wrong");
            Assert.AreEqual(true, Moex.Verbose, "Verbose!=true");
            Assert.IsNotNull(Moex.WebClient02);
            Assert.IsInstanceOfType(Moex.WebClient02, typeof (WebClient02));
            Moex.Init();
            Assert.IsNotNull(Moex.WebClient02, "Moex.WeClient is Not Null");

            Moex.MessageEvent += PrintMessage;

            ConsoleSync.WriteLineT(methodname + " Ok");
        }

        [TestCleanup]
        public void CleanUp()
        {
            Moex.MessageEvent -= PrintMessage;
            Moex.WebClient02 = null;
            Moex = null;
        }

        [TestMethod]
        public void GetTickersXmlRowsFromXDocTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            XDocument xdoc = null;
            try
            {
                xdoc = XDocument.Load(_tickersFuturesXDocPath);
                Assert.IsNotNull(xdoc, "XDocument is Null");
                _tickersXmlRows = Moex.GetXmlElementFromXDoc(xdoc, Moex.TickersRowsXPath);
                Assert.IsNotNull(_tickersXmlRows, "Tickers XmlRows is Null");
                Assert.IsTrue(_tickersXmlRows.HasElements, "Tickers XmlRows Has No Elements");
            }
            catch (Exception e)
            {
                ConsoleSync.WriteLineT(e.ToString());
                Assert.IsNotNull(xdoc, "XDocument is Null");
            }
        }
        [TestMethod]
        public void MainGetFuturesXmlRowsFromXDocTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            if (_tickersXmlRows == null)
                GetTickersXmlRowsFromXDocTest();
            Assert.IsNotNull(_tickersXmlRows, "TickerRowsXmlElement is null");
            //var lst = new List<MoexTicker>();
            var cnt = 0;
            foreach (var e in _tickersXmlRows.Elements())
            {
                var secid = e.Attribute("SECID");
                var boardid = e.Attribute("BOARDID");
                var ticker = Moex.TickerDeserialize(e);
                Assert.IsNotNull(ticker, $"Ticker {secid} {boardid} is null");
                // lst.Add(ticker);
                ConsoleSync.WriteLineT($"{++cnt} {ticker}");
            }
            ConsoleSync.WriteLineT($"Test Finished.TickersCnt:{cnt}");
        }
        [TestMethod]
        public void MainGetOptionsXmlRowsFromXDocTest()
        {
            var xdoc = XDocument.Load(_tickersOptionsXDocPath);
            Assert.IsNotNull(xdoc, "XDocument is Null");
            var optionsXmlRows = Moex.GetXmlElementFromXDoc(xdoc, Moex.TickersRowsXPath);
            Assert.IsNotNull(optionsXmlRows, "Tickers XmlRows is Null");
            Assert.IsTrue(optionsXmlRows.HasElements, "Tickers XmlRows Has No Elements");
            Assert.IsNotNull(Moex, "Moex Is Null");

            Assert.IsNotNull(optionsXmlRows, "TickerRowsXmlElement is null");
            //var lst = new List<MoexTicker>();
            var cnt = 0;
            foreach (var e in optionsXmlRows.Elements())
            {
                var secid = e.Attribute("SECID");
                var boardid = e.Attribute("BOARDID");
                var ticker = Moex.TickerDeserialize(e);
                Assert.IsNotNull(ticker, $"Ticker {secid} {boardid} is null");
                // lst.Add(ticker);
                ConsoleSync.WriteLineT($"{++cnt} {ticker}");
            }
            ConsoleSync.WriteLineT($"Test Finished.TickersCnt:{cnt}");
        }
        [TestMethod]
        public void GetTickersFromRowsXmlElement()
        {
            if (_tickersXmlRows == null)
                GetTickersXmlRowsFromXDocTest();
            Assert.IsNotNull(_tickersXmlRows, "Tickers XmlRows is Null");
            var tickerlist = Moex.Deserialize<TickersHolder>(_tickersXmlRows);
            Assert.IsNotNull(tickerlist, "Tickers Deserialization Failure");
            var lst = tickerlist.Items;
            tickerlist.Clear();
            Assert.IsTrue(lst.Any());
            ConsoleSync.WriteLineT($"Tickers Count:{lst.Count}");
            foreach (var t in lst) t.Init();
            foreach (var t in lst) ConsoleSync.WriteLineT(t.ToString());
        }
        [TestMethod]
        public void CreateLinkTickersTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            {
                var futureslink1 = Moex.CreateTickersLink("SPBFUT");
                Assert.IsNotNull(futureslink1);
                Assert.IsTrue(futureslink1.HasValue(), "Futures Link is Empty");
                var futureslink2 = Moex.CreateTickersLink(MoexTickerTypeEnum.Futures);
                Assert.IsNotNull(futureslink2);
                Assert.IsTrue(futureslink2.HasValue(), "Futures Link is Empty");
                Assert.AreEqual(futureslink1, _tickersFuturesLink, "FuturesLink is Wrong");
                Assert.AreEqual(futureslink1, futureslink2);

                var optionslink1 = Moex.CreateTickersLink("SPBOPT");
                Assert.IsNotNull(optionslink1);
                Assert.IsTrue(optionslink1.HasValue(), "Options Link is Empty");
                var optionslink2 = Moex.CreateTickersLink(MoexTickerTypeEnum.Option);
                Assert.IsNotNull(optionslink2);
                Assert.IsTrue(optionslink2.HasValue(), "Options Link is Empty");
                Assert.AreEqual(optionslink1, _tickersOptionsLink, "OptionLink is Wrong");
                Assert.AreEqual(optionslink1, optionslink2);
            }
            {
                var futureslink1 = Moex.CreateTickersLink("RFUD");
                Assert.IsNotNull(futureslink1);
                Assert.IsTrue(futureslink1.HasValue(), "Futures Link is Empty");
                var futureslink2 = Moex.CreateTickersLink(MoexTickerTypeEnum.Futures);
                Assert.IsNotNull(futureslink2);
                Assert.IsTrue(futureslink2.HasValue(), "Futures Link is Empty");
                Assert.AreEqual(futureslink1, _tickersFuturesLink, "FuturesLink is Wrong");
                Assert.AreEqual(futureslink1, futureslink2);

                var optionslink1 = Moex.CreateTickersLink("ROPD");
                Assert.IsNotNull(optionslink1);
                Assert.IsTrue(optionslink1.HasValue(), "Options Link is Empty");
                var optionslink2 = Moex.CreateTickersLink(MoexTickerTypeEnum.Option);
                Assert.IsNotNull(optionslink2);
                Assert.IsTrue(optionslink2.HasValue(), "Options Link is Empty");
                Assert.AreEqual(optionslink1, _tickersOptionsLink, "OptionLink is Wrong");
                Assert.AreEqual(optionslink1, optionslink2);
            }
        }
        [TestMethod]
        public void GetTickersXmlDocReplyFromMoexIssTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var xdoc1 = Moex.GetTickersXmlDoc("SpBfUt");
            Assert.IsNotNull(xdoc1, "TradesXmlDoc is Null");
            var xdoc2 = Moex.GetTickersXmlDoc("SpBoPt");
            Assert.IsNotNull(xdoc2, "TradesXmlDoc is Null");
        }
        [TestMethod]
        public void GetFuturesTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var tickers = Moex.GetTickers("SpbfUt");
            Assert.IsNotNull(tickers, "Tickers is Null");
            Assert.IsTrue(tickers.Any(), "Tickers is Empty");
            foreach(var t in tickers) ConsoleSync.WriteLineT(t.ToString());
        }
        [TestMethod]
        public void GetOptionsTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var tickers = Moex.GetTickers("rOpD");
            Assert.IsNotNull(tickers, "Tickers is Null");
            Assert.IsTrue(tickers.Any(), "Tickers is Empty");
            foreach (var t in tickers) ConsoleSync.WriteLineT(t.ToString());
        }
        //[TestMethod]
        //public void GetOptionsTestPrevPrice()
        //{
        //    Assert.IsNotNull(Moex, "Moex Is Null");
        //    var elements = Moex.GetTickersPrevPrice("rOpD");
        //    foreach (var e in elements)
        //    {
        //        var seqid = e.Attribute("SECID");
        //        var prevprice = e.Attribute("PREVPRICE");
        //        var str = $"{seqid?.ToString()} {prevprice?.ToString()}";
        //        ConsoleSync.WriteLineT(str);
        //    }
        //    // foreach (var t in tickers) ConsoleSync.WriteLineT(t.ToString());
        //}
        [TestMethod]
        public void GetFuturesOnlySiTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var tickers = Moex.GetTickers("SpbfUt");
            Assert.IsNotNull(tickers, "TIckers is Null");
            Assert.IsTrue(tickers.Any(), "Tickers is Empty");
            var onlySiTickers = tickers.Where(t => t.AssetCode == "Si");

            foreach (var t in onlySiTickers) ConsoleSync.WriteLineT(t.ToString());
        }
        [TestMethod]
        public void GetFuturesSiCurrentTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var tickers = Moex.GetTickers("SpbfUt");
            Assert.IsNotNull(tickers, "TIckers is Null");
            Assert.IsTrue(tickers.Any(), "Tickers is Empty");
            var currentSiTickers = tickers.
                Where(t => t.AssetCode == "Si" && t.LastTradeDate > DateTime.Now.Date)
                .OrderBy(t=>t.LastTradeDate).FirstOrDefault();

            ConsoleSync.WriteLineT($"CurrentTickerSI:{currentSiTickers}");
        }
        [TestMethod]
        public void GetFuturesSiNextTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var tickers = Moex.GetTickers("SpbfUt");
            Assert.IsNotNull(tickers, "TIckers is Null");
            Assert.IsTrue(tickers.Any(), "Tickers is Empty");
            var ts = tickers.
                Where(t => t.AssetCode == "Si" && t.LastTradeDate >= DateTime.Now.Date)
                .OrderBy(t => t.LastTradeDate).ToList();

            ConsoleSync.WriteLineT(ts.Count < 2 ? $"Only One Item Found: {ts[0]}" : $"Next Contract Si:{ts[1]}");
        }
        [TestMethod]
        public void GetCurrentFuturesContract()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var ascode1 = "Si";
            var ascode2 = "BR";
            var ascode3 = "RTS";

            var t1 = Moex.GetCurrentFuturesContract(ascode1, null);
            var t2 = Moex.GetCurrentFuturesContract(ascode2, null);
            var t3 = Moex.GetCurrentFuturesContract(ascode3, null);
            
            ConsoleSync.WriteLineT($"CurrentTicker:{ascode1} {t1}");
            ConsoleSync.WriteLineT($"CurrentTicker:{ascode2} {t2}");
            ConsoleSync.WriteLineT($"CurrentTicker:{ascode3} {t3}");
        }
        [TestMethod]
        public void GetSmartCurrentFuturesContract()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            var ascode1 = "Si";
            var ascode2 = "BR";
            var ascode3 = "RI";

            var t1 = Moex.GetCurrentFuturesContractSmart(ascode1, null);
            var t2 = Moex.GetCurrentFuturesContractSmart(ascode2, null);
            var t3 = Moex.GetCurrentFuturesContractSmart(ascode3, null);

            ConsoleSync.WriteLineT($"CurrentTicker:{ascode1} {t1}");
            ConsoleSync.WriteLineT($"CurrentTicker:{ascode2} {t2}");
            ConsoleSync.WriteLineT($"CurrentTicker:{ascode3} {t3}");
        }
        [TestMethod]
        public void GetFuturesWithPredicatTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");
            
            var assetcodes = new [] {"Si", "BR", "RTS"};

            foreach (var assetcode in assetcodes)
            {
                var tickers = Moex.GetTickers("SpbfUt", t => t.AssetCode == assetcode);
                Assert.IsNotNull(tickers, "TIckers is Null");
                Assert.IsTrue(tickers.Any(), "Tickers is Empty");
                foreach (var t in tickers)
                {
                    Assert.AreEqual(assetcode, t.AssetCode,
                        $"Ticker:{t.SecId} AssetCode: {t.AssetCode} isWrong");
                }
                foreach (var t in tickers) ConsoleSync.WriteLineT(t.ToString());
            }
        }
        [TestMethod]
        public void GetFuturesAndGetTickerWithPredicatTest()
        {
            Assert.IsNotNull(Moex, "Moex Is Null");

            var assetcodes = new[] { "Si", "BR", "RTS" };

            foreach (var assetcode in assetcodes)
            {
                var tickers = Moex.GetTickers("SpbfUt", t => t.AssetCode == assetcode);
                Assert.IsNotNull(tickers, "TIckers is Null");
                Assert.IsTrue(tickers.Any(), "Tickers is Empty");
                foreach (var t in tickers)
                {
                    Assert.AreEqual(assetcode, t.AssetCode,
                        $"Ticker:{t.SecId} AssetCode: {t.AssetCode} isWrong");
                    var ticker = Moex.GetTicker(t.BoardId, t.SecId);
                    Assert.IsNotNull(ticker, $"GetTicker() Ticker:{t.SecId} is Null");
                    Assert.AreEqual(t.ToString(),ticker.ToString(), $"Tickers AreNotEqual");
                }
                foreach (var t in tickers) ConsoleSync.WriteLineT(t.ToString());
            }
        }

        public void PrintMessage(object sender, string s)
        {
            ConsoleSync.WriteLineT(s);
        }
    }
}
