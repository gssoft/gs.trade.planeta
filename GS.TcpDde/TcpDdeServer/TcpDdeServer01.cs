using System;
using System.Reflection;
using System.Threading;
using System.Xml.Linq;
using GS.Contexts;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Serialization;
using TcpIpSockets;
using TcpIpSockets.TcpServer03;

namespace TcpDdeServer
{
    public class TcpDdeServer01 : Context3
    {
        public string IpAdress { get; set; }
        public int Port { get; set; }
        public int SecondsWaitAfterStop { get; set; }
        public ITcpServer TcpServer { get; private set; }

        public IDde DdeServer { get; private set; }

        // for Serialization
        public TcpDdeServer01() { }

        public TcpDdeServer01(string ipAddress, int port)
        {
            TcpServer = new TcpServer03(ipAddress, port) { Parent = this };
            SecondsWaitAfterStop = 1;
        }
        public override void Init(IEventLog evl)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Start", ToString());

            // IsNeedEventHub = true;
            IsNeedEventLog = true;
            base.Init();

            var xDoc = XDocument.Load(@"Init\Dde.xml");
            DdeServer = Builder.Build2<IDde, string, ITopicItem>(xDoc, "Dde");
            DdeServer.Parent = this;
            DdeServer.Init();

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Finish",  ToString());
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // IsNeedEventHub = true;
            IsNeedEventLog = true;

            base.Init();
        }
        public override void Start()
        {
            try
            {
                //base.Start(); Start in the Init
                DdeServer?.Start();
                TcpServer?.Start();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }

        public void CloseClients()
        {
            try
            {
                TcpServer?.CloseClients();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }

        public override void Stop()
        {
            try
            {
                DdeServer?.Stop();
                Thread.Sleep(SecondsWaitAfterStop);
                TcpServer?.Stop();
                Thread.Sleep(SecondsWaitAfterStop);
                base.Stop();
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void SendMessage(string routeKey, string entity, string operation)
        {
            TcpServer?.SendMessage(routeKey, entity, operation);
        }
        public override void DeQueueProcess()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
        }
    }
}
