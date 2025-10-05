using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using TcpIpSockets;
using TcpClientHandler = TcpIpSockets.TcpClientHandler03.TcpClientHandler;
namespace Ca_TcpClient01
{
    class TestTcpClient01
    {
        const int Port = 8082;
        const string IpAddress = "127.0.0.1";

        static void Main(string[] args)
        {
            // TcpClient02_Test();
            TcpClientHandler03_Test();
        }
        private static void TcpClient02_Test()
        {
            var tcpClient = new TcpClient02();
            tcpClient.Init();
            tcpClient.Start();
            ConsoleSync.WriteReadLine("Press to Close");
            tcpClient.Stop();
            ConsoleSync.WriteReadLine("Press to Exit...");
        }
        private static void TcpClientHandler03_Test()
        {
            var tcpClient = new TcpClientHandler(IpAddress, Port);
            // tcpClient.Init();
            tcpClient.Start();
            ConsoleSync.WriteReadLine("Press to Close");
            tcpClient.Stop();
            ConsoleSync.WriteReadLine("Press to Exit...");
        }
    }
}
