using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using TcpIpSockets;
using TcpIpSockets.TcpClient03;
using TcpIpSockets.TcpClientHandler03;
using TcpIpSockets.TcpServer03;

namespace Ca_TcpClient02
{
    class Program
    {
        const int Port = 8082;
        const string IpAddress = "127.0.0.1";

        static void Main(string[] args)
        {
            // RunTcpCLient03_01();
            RunTcpHandlerSrv01();

        }
        private static void RunTcpCLient03_01()
        {
            var tcp = new TcpClient03();
            tcp.Init();
            tcp.Start();
            ConsoleSync.WriteReadLine("Press any key to Stop");
            tcp.Stop();
            ConsoleSync.WriteReadLine("Press any key to Exit");
        }
        private static void RunTcpHandlerSrv01( )
        {
            var tcp = new TcpClientSrv01(IpAddress, Port);
            tcp.Init();
            tcp.Start();
            ConsoleSync.WriteReadLine("Press any key to Stop");
            tcp.Stop();
            ConsoleSync.WriteReadLine("Press any key to Exit");
        }
       
    }
}
