using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.Contexts;
using GS.Interfaces;
using TcpClientHandler = TcpIpSockets.TcpClientHandlerLst.TcpClientHandler;

namespace TcpIpSockets.TcpClientHandlerLst
{
    public class TcpClientSrv02 : Context3
    {
        public string IpAdress { get; set; }
        public int Port { get; set; }

        public TcpClientHandler TcpClientHandler { get; private set; }

        // for Serialization
        public TcpClientSrv02(){}

        public TcpClientSrv02(string ipAddress, int port)
        {
            TcpClientHandler = new TcpClientHandler(ipAddress, port) {Parent = this};
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
        }
        public override void Start()
        {
            try
            {
                //base.Start();
                TcpClientHandler.Start();
            }
            catch (Exception e)
            {
                SendException(e);
            }
            
        }
        public override void Stop()
        {
            TcpClientHandler?.Stop();
            Thread.Sleep(5000);
            base.Stop();
        }
        public override void DeQueueProcess()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
        }
    }
}
