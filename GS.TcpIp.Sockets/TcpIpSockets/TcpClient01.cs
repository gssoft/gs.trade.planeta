using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Tasks;
using TcpIpSockets.Extensions;
using TcpIpSockets.TcpClientHandler01;
using TcpClientHandler = TcpIpSockets.TcpClientHandler01.TcpClientHandler;

namespace TcpIpSockets
{
    public class TcpClient01 : Element1<string>
    {
        private const int PortPar = 8082;
        private const string IpAddressPar = "127.0.0.1";
        private const int ConnectionTimeOut = 5000;

        public ITcpClientHandler TcpClientHandler { get; set; }
        public int  Port { get; set; }
        public string IpAddress { get; set; }

        //public TaskBase WorkTask { get; private set; }
        public override string Key => Code.HasValue() ? Code : GetType().FullName;

        public TcpClient01()
        {
            Port = PortPar;
            IpAddress = IpAddressPar;
            // TcpClientHandler = new TcpClientHandler();
        }
        public override void Init()
        {
            
        }
        public bool Connect()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            DisConnect();
            // TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);
            var client = new TcpClient();
            while (!client.ConnectAsync(IpAddress, Port).Wait(ConnectionTimeOut))
            // if (!client.ConnectAsync(IpAddress, Port).Wait(ConnectionTimeOut))
            {
                ConsoleSync.WriteLineT($"Can't Connect to {IpAddress} {Port}");
                ConsoleSync.WriteReadLineT($"Press Key to Exit");
                // return false;
            }
            TcpClientHandler = new TcpClientHandler
            {
                ClientSocket = client,
              //  NetworkStream = client.GetStream(),
                ClientFullName = Guid.NewGuid().ToString(),
                // IsPingEnabled = true
                IsPingEnabled = false
            };
            TcpClientHandler.MessageReceived += MessageReceived;
            ConsoleSync.WriteLineT($"{m}:Connected to Server");
            return true;
        }
        public void DisConnect()
        {

        }
        public void Start()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Connect();
            if (TcpClientHandler != null)
            {
                TcpClientHandler.Init();
                TcpClientHandler.Start();
                Thread.Sleep(TimeSpan.FromSeconds(1));

                ConsoleSync.WriteLineT($"{m}: {GetType().Name}");
            }
            else
                Connect();
        }
        public void Stop()
        {
            TcpClientHandler?.Stop();
        }
        public void SendMessage(string[] message)
        {
            TcpClientHandler?.SendMessage(message);
        }
        public void MessageReceived(object sender, string[] s)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}:{s.MessageToString()}");
        }
    }
}
