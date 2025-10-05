using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moex;

namespace MoexTests
{
    [TestClass]
    public class MoexSecXmlReplyTest
    {
        public MoexTest MoexTest;

        public string Link =  "https://iss.moex.com/iss/engines/futures/markets/forts/securities/SiZ9.xml";

        public string[] ReplyFilesCorrect =
        {
            @"SecuritySamples\BRZ9.xml",
            @"SecuritySamples\SiZ9.xml",
            @"SecuritySamples\Si64500BI9D.xml",
            @"SecuritySamples\Si64500BU9D.xml",
            @"SecuritySamples\BRH0.xml",
        };

        public string[] ReplyFilesNotCorrect =
        {
            @"SecuritySamples\BRZ0.xml",
            @"SecuritySamples\SiZ0.xml",
            @"SecuritySamples\Si65000BI9D.xml"
        };

        public string XmlPathToSecElementCorrect = "/document/data[@id='securities']/rows/row";
        public string XmlPathToSecElementNoCorrect = "/document/data[id='securities']/rows/row";

        [TestInitialize]
        public void SetUp()
        {
            MoexTest = new MoexTest();
        }

        [TestMethod]
        public void GetSecurityFromXmlFile1()
        {
            foreach (var f in ReplyFilesCorrect)
            {
                var ret = MoexTest.GetXmlElementInXmlReply(f, XmlPathToSecElementCorrect);
                Assert.IsNotNull(ret, "Should Be Not Null");
                var sec = MoexTest.TickerDeserialize(ret);
                Assert.IsNotNull(sec, "Should Be Not Null");
            }
            foreach (var f in ReplyFilesNotCorrect)
            {
                var ret = MoexTest.GetXmlElementInXmlReply(f, XmlPathToSecElementCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
            foreach (var f in ReplyFilesCorrect)
            {
                var ret = MoexTest.GetXmlElementInXmlReply(f, XmlPathToSecElementNoCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
            foreach (var f in ReplyFilesNotCorrect)
            {
                var ret = MoexTest.GetXmlElementInXmlReply(f, XmlPathToSecElementNoCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
        }
        [TestMethod]
        public void GetSecurityFromXmlFile2()
        {
            foreach (var f in ReplyFilesCorrect)
            {
                var ret = MoexTest.GetTickerFromFile(f, XmlPathToSecElementCorrect);
                Assert.IsNotNull(ret, "Should Be Not Null");
            }
            foreach (var f in ReplyFilesNotCorrect)
            {
                var ret = MoexTest.GetTickerFromFile(f, XmlPathToSecElementCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
            foreach (var f in ReplyFilesCorrect)
            {
                var ret = MoexTest.GetTickerFromFile(f, XmlPathToSecElementNoCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
            foreach (var f in ReplyFilesNotCorrect)
            {
                var ret = MoexTest.GetTickerFromFile(f, XmlPathToSecElementNoCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
        }
        [TestMethod]
        public void GetSecFromXDoc()
        {
            foreach (var f in ReplyFilesCorrect)
            {
                var xdoc = XDocument.Load(f);
                Assert.IsNotNull(xdoc, "Should Be Not Null");
                var ret = MoexTest.GetTickerFromXDoc(xdoc, XmlPathToSecElementCorrect);
                Assert.IsNotNull(ret, "Should Be Not Null");
            }
            foreach (var f in ReplyFilesNotCorrect)
            {
                var xdoc = XDocument.Load(f);
                Assert.IsNotNull(xdoc, "Should Be Not Null");
                var ret = MoexTest.GetTickerFromXDoc(xdoc, XmlPathToSecElementCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
            foreach (var f in ReplyFilesCorrect)
            {
                var xdoc = XDocument.Load(f);
                Assert.IsNotNull(xdoc, "Should Be Not Null");
                var ret = MoexTest.GetTickerFromXDoc(xdoc, XmlPathToSecElementNoCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
            foreach (var f in ReplyFilesNotCorrect)
            {
                var xdoc = XDocument.Load(f);
                Assert.IsNotNull(xdoc, "Should Be Not Null");
                var ret = MoexTest.GetTickerFromXDoc(xdoc, XmlPathToSecElementNoCorrect);
                Assert.IsNull(ret, "Should Be Null");
            }
        }

        [TestMethod]
        public void GetSecFromWeb()
        {
        }
    }
}
