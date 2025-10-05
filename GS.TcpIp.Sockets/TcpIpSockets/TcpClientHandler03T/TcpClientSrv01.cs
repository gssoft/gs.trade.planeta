using System.Reflection;
using System.Threading;
using GS.Contexts;
using GS.Interfaces;

namespace TcpIpSockets.TcpClientHandler03T
{
    public class TcpClientSrv01<TMessage> : Context3
        where TMessage : IHaveIndex<string>
    {
        public string IpAdress { get; set; }
        public int Port { get; set; }

        public TcpClientHandler<TMessage> TcpClientHandler { get; private set; }

        // for Serialization
        public TcpClientSrv01(){}

        public TcpClientSrv01(string ipAddress, int port)
        {
            TcpClientHandler = new TcpClientHandler<TMessage>(ipAddress, port) {Parent = this};
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
            //base.Start();
            TcpClientHandler?.Start();
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
