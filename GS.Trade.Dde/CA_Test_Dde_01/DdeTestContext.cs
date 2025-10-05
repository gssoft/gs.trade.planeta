using System;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using GS.Contexts;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Serialization;
// using IDde = GS.Trade.Dde.IDde;
using IDde = GS.Interfaces.Dde.IDde;

namespace CA_Test_Dde_01
{
    public class DdeTestContext : Context2
    {
        public IDde DdeServer { get; set; }
        public XDocument XDoc { get; set; }

        public override void Init()
        {
            Evlm2(EvlResult.INFO, EvlSubject.INIT, 
                MethodBase.GetCurrentMethod()?.Name + " Begin", "", ToString() );

            base.Init();
            base.Start();
            Thread.Sleep(1000);

            XDoc = XDocument.Load(@"Init\Dde.xml");
            DdeServer = Builder.Build2<IDde, string, ITopicItem>(XDoc, "Dde");
            DdeServer.Parent = this;
            DdeServer.Init();

            Evlm52(EvlResult.INFO, EvlSubject.INIT, 
                MethodBase.GetCurrentMethod()?.Name + " Finish", "", ToString());
        }

        public override void Start()
        {
            base.Start();
            DdeServer.Start();
        }

        public override void Stop()
        {
            // DdeServer.Close();
            DdeServer.Stop();
            base.Stop();
        }
        public override void DeQueueProcess()
        {
           // throw new NotImplementedException();
        }
    }
}
