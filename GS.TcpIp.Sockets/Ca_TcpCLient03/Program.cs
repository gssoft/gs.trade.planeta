using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using TcpIpSockets.TcpClient03;
using TcpIpSockets.TcpClientHandler03;
using TcpIpSockets.TcpClientHandlerLst;

namespace Ca_TcpCLient03
{
    class Program
    {
        const int Port = 8082;
        const string IpAddress = "127.0.0.1";

        static void Main(string[] args)
        {
            // RunTcpCLient03_01();
            RunTcpHandlerSrv02();

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
        private static void RunTcpHandlerSrv02()
        {
            var tcp = new TcpClientSrv02(IpAddress, Port);
            tcp.Init();
            tcp.Start();
            ConsoleSync.WriteReadLine("Press any key to Stop");
            tcp.Stop();
            ConsoleSync.WriteReadLine("Press any key to Exit");
        }

    }
}
