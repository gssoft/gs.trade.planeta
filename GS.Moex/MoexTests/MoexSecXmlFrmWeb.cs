using System;
using System.Xml.Linq;
using GS.ConsoleAS;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;
using WebClients;

namespace MoexTests
{
    [TestClass]
    public class MoexSecXmlFrmWeb
    {
        public WebClient02 WebClient;

        [TestInitialize]
        public void SetUp()
        {
            WebClient = Builder.Build2<WebClient02>(@"Init\WebClient.xml", "WebClient02");
            Assert.IsNotNull(WebClient, "Should Be Not Null");
            WebClient.Init();
            Assert.AreEqual(WebClient.ApiPrefix, @"iss/engines/futures/markets/");
            Assert.AreEqual(WebClient.BaseAddress, @"https://iss.moex.com/");
        }
        [TestMethod]
        public void WebClientInitTest()
        {
            WebClient = Builder.Build2<WebClient02>(@"Init\WebClient.xml", "WebClient02");
            Assert.IsNotNull(WebClient, "Should Be Not Null");
            WebClient.Init();
            Assert.AreEqual(WebClient.ApiPrefix, @"iss/engines/futures/markets/");
            Assert.AreEqual(WebClient.BaseAddress, @"https://iss.moex.com/");
        }
        [TestMethod]
        public void GetSecXmlElementFromWeb()
        {
            Assert.IsNotNull(WebClient);
            var xmlsecstrm = WebClient.GetItemInStream(@"forts/securities/SiZ3.xml");
            Assert.IsNotNull(xmlsecstrm);
            var xdoc = XDocument.Load(xmlsecstrm);
            Assert.IsNotNull(xdoc);
            var moex = new MoexTest();
            var sec = moex.GetTickerFromXDoc(xdoc, "/document/data[id='securities']/rows/row");
            Assert.IsNull(sec);
            sec = moex.GetTickerFromXDoc(xdoc, "/document/data[@id='securities']/rows/row");
            Assert.IsNotNull(sec);
            ConsoleSync.WriteLineT(sec.ToString());
        }
    }
}
