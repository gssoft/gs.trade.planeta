using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using TcpIpSockets.TcpClientHandlerLst;

namespace Ca_TcpServer03
{
    class Program
    {
        const int Port = 8082;
        const string IpAddress = "127.0.0.1";
        static void Main(string[] args)
        {
            RunTcpServerSrv01();
            // TcpServer_Test02();
        }
        private static void RunTcpServerSrv01()
        {
            var server = new TcpServerSrv04(IpAddress, Port);
            server.Init();
            server.Start();
            ConsoleSync.WriteReadLine("Press any key to Stop");
            server.Stop();
            ConsoleSync.WriteReadLine("Press any key to Exit");
        }
        private static void TcpServer_Test02()
        {
            //var server = new TcpServer02();
            //server.Start();
            //            ConsoleSync.WriteReadLineT("Press any key to Start");
            var server = new TcpServerSrv04(IpAddress, Port);
            server.Init();
            server.Start();

            var rand = new Random(DateTime.Now.Millisecond);
            foreach (int i in Enumerable.Range(1, 100))
            {
                //server.SendMessage("Ping",
                //    $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}", i.ToString());
                //var server = new TcpServerSrv01(IpAddress, Port);
                //server.Init();
                //server.Start();
                Thread.Sleep(rand.Next(15000, 1 * 60000));
                //var rand1 = new Random(DateTime.Now.Millisecond);
                foreach (int j in Enumerable.Range(1, 26))
                {
                    server.SendMessage("Ping",
                        $"{DateTime.Now.TimeOfDay:hh\\:mm\\:ss\\.fff}", j.ToString());
                    Thread.Sleep(rand.Next(100, 1 * 1000));
                }
                Thread.Sleep(5000);
                server.CloseClients();
                Thread.Sleep(5000);
            }
            ConsoleSync.WriteReadLineT("Press any key to CloseConnections");
            server.CloseClients();
            server.Stop();
            ConsoleSync.WriteReadLineT("Press any key to Finish");
        }
    }
}
