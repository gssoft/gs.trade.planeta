using System;
using System.Threading;
using System.Xml.Linq;
using GS.ConsoleAS;
using GS.Interfaces.Dde;
using GS.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using GS.Trade.Dde;
//using IDde = GS.Trade.Dde.IDde;
using IDde = GS.Interfaces.Dde.IDde;

namespace Test_Dde_01
{
    [TestClass]
    public class DdeTest01
    {
        public IDde DdeServer { get; set; }
        public XDocument Xdoc { get; set; }

        [TestInitialize]
        public void SetUp()
        {
            //Xdoc = XDocument.Load(@"Init\Dde.xml");
            //Assert.IsNotNull(Xdoc, "Should Be Not Null");
        }
        [TestMethod]
        public void LoadXDocument()
        {
            Xdoc = XDocument.Load(@"Init\Dde.xml");
            Assert.IsNotNull(Xdoc, "Should Be Not Null");
        }
        [TestMethod]
        public void BuildDde()
        {
            LoadXDocument();
            DdeServer = Builder.Build2<IDde, string, ITopicItem>(Xdoc, "Dde");
            Assert.IsNotNull(DdeServer, "Should Be Not Null");
        }
        [TestMethod]
        public void DdeContextTest()
        {
            var dde = new DdeTestContext {IsNeedEventLog = true};
            dde.Init();
            dde.Start();
            // Thread.Sleep(15*1000);
            ConsoleSync.WriteReadLine("Press Any Key to Stop");
            dde.Stop();
            ConsoleSync.WriteReadLine("Press Any Key to Finish");
        }

    }
}
