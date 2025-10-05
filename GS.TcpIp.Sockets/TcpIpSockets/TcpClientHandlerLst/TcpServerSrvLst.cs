using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.Contexts;
using GS.Interfaces;

using TcpClientHandler = TcpIpSockets.TcpClientHandlerLst.TcpClientHandler;
using TcpServer = TcpIpSockets.TcpClientHandlerLst.TcpServer04;

namespace TcpIpSockets.TcpClientHandlerLst
{
    public class TcpServerSrv04 : Context3
    {
        public string IpAdress { get; set; }
        public int Port { get; set; }

        public int SecondsWaitAfterStop { get; set; }

        public ITcpServer TcpServer { get; private set; }

        // for Serialization
        public TcpServerSrv04() { }

        public TcpServerSrv04(string ipAddress, int port)
        {
            TcpServer = new TcpServer04(ipAddress, port) { Parent = this };
            SecondsWaitAfterStop = 1;
        }
        public override void Init(IEventLog evl)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // IsNeedEventHub = true;
            IsNeedEventLog = true;
            base.Init();
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // IsNeedEventHub = true;
            IsNeedEventLog = true;
            
            base.Init();
            TcpServer.Init();
        }
        public override void Start()
        {
            //base.Start(); Start in the Init
            TcpServer?.Start();
        }

        public void CloseClients()
        {
            TcpServer?.CloseClients();
        }

        public override void Stop()
        {
            TcpServer?.Stop();
            Thread.Sleep(SecondsWaitAfterStop);
            base.Stop();
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
