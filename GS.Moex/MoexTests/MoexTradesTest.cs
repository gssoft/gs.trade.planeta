using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.ConsoleAS;
using GS.Extension;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;
using WebClients;

namespace MoexTests
{
    [TestClass]
    public class MoexTradesTest
    {
        public bool IsLongTimeTestEnabled { get; set; }

        public MoexTest Moex;
        private int _tradesLimitForWebRequest = 500;
        private int _tradesLimitForWebRequestReal = 0; 
        private string _tradeRowsXPath = @"/document/data[@id='trades']/rows";
        private string _xDocTradesXmlFilePath = @"Samples\Trades_SIZ9.xml";
        private XDocument _xDoc;
        private XElement _tradesXElement;
        private readonly List<MoexTrade> _tradesList;
        private XNode _tradesXmlDocReply;

        private int RealTradesReceivedCount => _tradesLimitForWebRequestReal > 0
            ? _tradesLimitForWebRequestReal
            : _tradesLimitForWebRequest;
        public MoexTradesTest()
        {
            _tradesList = new List<MoexTrade>();
        }
        [TestInitialize]
        public void SetUp()
        {
            TestInitMoexInstance();
            LoadMoexTradesXMlFileTest();
            GetTradesXmlElementFromXDocTest();

            IsLongTimeTestEnabled = false;
        }
        [TestMethod]
        public void TestInitMoexInstance()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            //Moex = Builder.Build2<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Moex = Builder.Build3<MoexTest>(@"Init\Moex.xml", "MoexTest");
            Assert.IsNotNull(Moex, "Should Be Not Null");
            Assert.IsTrue(Moex.FuturesIssWebApiPrefix.HasValue());
            Assert.IsTrue(Moex.OptionsIssWebApiPrefix.HasValue());
            Assert.IsTrue(Moex.SecurityRowXPath.HasValue());
            Assert.IsTrue(Moex.TradeRowsXPath.HasValue());
            Assert.AreEqual(@"/document/data[@id='trades']/rows", Moex.TradeRowsXPath);
            Assert.IsTrue(Moex.TradeApiPostfix.HasValue());
            Assert.AreEqual(@"/trades.xml", Moex.TradeApiPostfix);
            Assert.IsTrue(Moex.TradeApiParams.HasValue());
            Assert.AreEqual(@"?start=StartValue&limit=LimitValue", Moex.TradeApiParams);
            Assert.IsTrue(Moex.TradesLimitForWebRequest >= 100);
            Assert.IsNotNull(Moex.WebClient02);
            Assert.IsInstanceOfType(Moex.WebClient02, typeof(WebClient02));
            Moex.Init();
            Assert.IsNotNull(Moex.WebClient02, "Mex.WeClient is Not Null");

            // Assert.AreEqual(Moex.ApiPrefix, @"iss/engines/futures/markets/");
            // Assert.AreEqual(WebClient.BaseAddress, @"https://iss.moex.com/");

            Assert.AreEqual(Moex.TradeRowsXPath, _tradeRowsXPath);

            ConsoleSync.WriteLineT(methodname + " Ok");
        }
        public void LoadMoexTradesXMlFileTest()
        {
            _xDoc = XDocument.Load(_xDocTradesXmlFilePath);
            Assert.IsNotNull(_xDoc, "LoadTradesXmlFile");
        }
        [TestMethod]
        public void GetTradesXmlElementFromXDocTest()
        {
            Assert.IsNotNull(_xDoc, "LoadTradesXmlFile");
            _tradesXElement = Moex.GetXmlElementFromXDoc(_xDoc, Moex.TradeRowsXPath);
            Assert.IsNotNull(_tradesXElement, "TradeRowsXmlElement");
            Assert.IsTrue(_tradesXElement.HasElements, "XmlTrades Has Elements");
            var cnt = ((IEnumerable<XElement>)_tradesXElement.Elements()).Count();
            Assert.AreEqual(500,cnt);
        }
        [TestMethod]
        public void DeSerializeTradesFormXElementTest1()
        {
            Assert.IsNotNull(_tradesXElement, "TradeRowsXmlElement");
            _tradesList.Clear();
            foreach (var x in _tradesXElement.Elements())
            {
                var trade = Moex.Deserialize<MoexTrade>(x);
                trade.Init();
                _tradesList.Add(trade);
            }
            Assert.AreEqual(500, _tradesList.Count, "TradeListCount != 500");
            //foreach (var t in list)
            //    ConsoleSync.WriteLineT(t.ToString());
           // SerializeTradesFormXElementTest();
        }
        [TestMethod]
        public void DeSerializeTradesFormXElementTest2()
        {
            try
            {
                Assert.IsNotNull(_tradesXElement, "TradeRowsXmlElement");
                var tradelist = Moex.Deserialize<TradeList>(_tradesXElement);
                Assert.IsNotNull(tradelist, "TradeList Not DeSerialized");
                Assert.AreEqual(500, tradelist.Items.Count, "TradeListCount != 500");
                tradelist.InitItems();
                //foreach(var t in tradelist.Items)
                //    Console.WriteLine(t.ToString());
                //moexSec?.Init();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        [TestMethod]
        public void DeSerializeTradesFormXElementTest3()
        {
            try
            {
                Assert.IsNotNull(_tradesXElement, "TradeRowsXmlElement");
                var holder = Moex.Deserialize<TradeHolder>(_tradesXElement);
                Assert.IsNotNull(holder, "TradeList Not DeSerialized");
                Assert.AreEqual(500, holder.Items.Count, "TradeListCount != 500");
                // tradelist.InitItems();
                var lst = holder.Items; holder.Clear();
                Moex.InitItems(lst);
                //foreach (var t in tradelist.Items)
                //    Console.WriteLine(t.ToString());
                //moexSec?.Init();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        // Get Trades from Web: Moex.ISS
        [TestMethod]
        public void CreateTradesLinkTest()
        {
            var link1 = @"futures/markets/forts/securities/SiZ9/trades.xml?start=0&limit=1000";
            Assert.IsNotNull(Moex, "Should Be Not Null");
            var tradeslink = Moex.CreateTradesLink("SpbFut", "SiZ9", 0, 1000);
            Assert.AreEqual(link1, tradeslink, "TradeLink Wrong");
        }
        [TestMethod]
        public void GetTradesXmlDocFromWebTest()
        {
            Assert.IsNotNull(Moex, "Should Be Not Null");
            _tradesXmlDocReply = Moex.GetTradesXmlDoc("SPBFUT", "SiZ9", 0, Moex.TradesLimitForWebRequest);
            Assert.IsNotNull(_tradesXmlDocReply, "XNode is Null");
        }
        [TestMethod]
        public void GetTradesRowsFromWebTest()
        {
            Assert.IsNotNull(Moex, "Should Be Not Null");
            if (_tradesXmlDocReply == null) GetTradesXmlDocFromWebTest();
            _tradesXElement = Moex.GetXmlElementFromXDoc(_tradesXmlDocReply, Moex.TradeRowsXPath);
            Assert.IsNotNull(_tradesXElement, "TradeRowsXmlElement");
            if (!_tradesXElement.HasElements)
            {
                ConsoleSync.WriteLineT("TradesXmlRows is Empty");
                return;
            }
            Assert.IsTrue(_tradesXElement.HasElements, "XmlTrades Has Elements");
            _tradesLimitForWebRequestReal = 0;
            var cnt = _tradesXElement.Elements().Count();
            if (cnt != _tradesLimitForWebRequest)
            {
                ConsoleSync.WriteLineT($"Trades Count:{cnt} WebRequestLimit: {_tradesLimitForWebRequest}");
                _tradesLimitForWebRequestReal = cnt;
                return;
            }
            Assert.AreEqual(RealTradesReceivedCount, cnt , "Count != TradesWebRequestLimit");
        }
        [TestMethod]
        public void GetTradesFromWebTest()
        {
            Assert.IsNotNull(Moex, "Should Be Not Null");
            if (_tradesXElement == null) GetTradesRowsFromWebTest();
            var holder = Moex.Deserialize<TradeHolder>(_tradesXElement);
            Assert.IsNotNull(holder, "TradeList Not DeSerialized"); 
            if(!holder.Items.Any()) { ConsoleSync.WriteLineT("TradesList is Empty"); return;}
            Assert.AreEqual(RealTradesReceivedCount, holder.Items.Count, $"TradeListCount != {RealTradesReceivedCount}");
            var lst = holder.Items; holder.Clear();
            Moex.InitItems(lst);
        }
        [TestMethod]
        public void GetTradesStartLimitTest()
        {
            Assert.IsNotNull(Moex, "Should Be Not Null");
            var trades1 = Moex.GetTradesAdOnce("SPBFUT", "SiZ9", 0, 1000);
            Assert.IsNotNull(trades1, "Trades isNull");
            // Assert.AreEqual(1000, trades1.Count);
            var trades2 = Moex.GetTradesAdOnce("SPBFUT", "SiZ9", 1000, 500);
            Assert.IsNotNull(trades2, "Trades isNull");
            // Assert.AreEqual(500, trades2.Count);
            if (trades1.Any() && trades2.Any())
            {
                var t11 = trades1.FirstOrDefault();
                var t21 = trades2.FirstOrDefault();
                Assert.AreNotEqual(t11?.TradeNumber, t21?.TradeNumber);
                ConsoleSync.WriteLineT(t11?.ToString());
                ConsoleSync.WriteLineT(t21?.ToString());
            }
        }
        //[TestMethod]
        //public void GetTradesTest()
        //{
        //    Assert.IsNotNull(Moex, "Moex is Null");
        //    var board = "SPBFUT";
        //    var tickercode = "SiZ9";
        //    var start = 0;
        //    var limit = Moex.TradesLimitForWebRequest;
        //    var pass = 0;var tradesall = new List<MoexTrade>();
        //    while (true)
        //    {
        //        var trades = Moex.GetTradesPack(board, tickercode, start, limit, PrintMessage);
        //        if(trades == null || !trades.Any()) break;
        //        tradesall.AddRange(trades);

        //        ConsoleSync.WriteLineT($"Pass: {++pass} Trades Count: {trades.Count}");
        //        start = start + limit;
        //    }
        //    ConsoleSync.WriteLineT("Trades All Count: " + tradesall.Count);
        //}
        [TestMethod]
        public void GetTradesTest()
        {
            if (!IsLongTimeTestEnabled) return;

            Assert.IsNotNull(Moex, "Should Be Not Null");
            var board = "SPBFUT";
            var tickercode = "SiZ9";
            var trades = Moex.GetTrades(board, tickercode, 0, 1000);
            Assert.IsNotNull(trades);
            // Assert.IsTrue(trades.Any(),"Trades Count=0");
            ConsoleSync.WriteLineT("Trades All Count: " + trades.Count);
        }
        [TestMethod]
        public void TestList()
        {
            var a = new List<int> {1, 2, 3, 4, 5};
            var b = a;
            Assert.AreEqual(5,b.Count, "Count != 5");
            a = new List<int> {1, 2, 3, 4};
            Assert.IsNotNull(b, "B is Not Null");
            Assert.AreEqual(5,b.Count, "Count != 5");
            Assert.AreEqual(4, a.Count, "Count != 4");
            foreach (var i in a) ConsoleSync.WriteLineT($"a={i}");
            foreach (var i in b) ConsoleSync.WriteLineT($"b={i}");
        }
        public void SerializeTradesFormXElement()
        {
            Assert.IsNotNull(Moex, "Should Be Not Null");
            var filename = @"Samples\TradesSerialize_SiZ9.xml";
            var file = new XDocument(filename);
            //TextWriter writer = new StreamWriter(filename);
            //XmlSerializer ser = new XmlSerializer(typeof(List<MoexTrade>));
            //ser.Serialize(writer, _tradesList );
            //writer?.Close();
            Moex.Serialize<MoexTrade>(file, _tradesList);
        }
        private static void PrintMessage(string e)
        {
            ConsoleSync.WriteLineT(e);
        }
    }
}
