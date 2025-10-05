using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using TcpIpSockets;

namespace Ca_Tcp_Sockets_Serv05
{
    class Program
    {

        static void Main(string[] args)
        {
            //TcpServer_Test01();
            TcpServer_Test02();
        }
        private static void TcpServer_Test01()
        {
            //var server = new TcpServer01();
            var server = new TcpServer02();
            server.Start();
            //Thread.Sleep(TimeSpan.FromSeconds(350));
            //server.Stop();

            // await Task.Delay(TimeSpan.FromSeconds(15));
            //Thread.Sleep(TimeSpan.FromSeconds(15));
            // server.ClearAllSocket();
            // server.CloseRequestToAllClients();
            ConsoleSync.WriteReadLineT("Press any key to Start");

            var rand = new Random(DateTime.Now.Millisecond);
            foreach (int i in Enumerable.Range(1, 50))
            {
                server.SendMessage("Ping",
                    $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}", i.ToString());
                Thread.Sleep(rand.Next(100, 1 * 1000));
            }


            ConsoleSync.WriteReadLineT("Press any key to CloseConnections");
            // server.ClearAllSocket();
            server.Stop();
            ConsoleSync.WriteReadLineT("Press any key to Finish");

        }
        private static void TcpServer_Test02()
        {
            //var server = new TcpServer02();
            //server.Start();
//            ConsoleSync.WriteReadLineT("Press any key to Start");

            var rand = new Random(DateTime.Now.Millisecond);
            foreach (int i in Enumerable.Range(1, 100))
            {
                //server.SendMessage("Ping",
                //    $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}", i.ToString());
                var server = new TcpServer02();
                server.Start();
                Thread.Sleep(rand.Next(15000, 1 * 60000));
                //var rand1 = new Random(DateTime.Now.Millisecond);
                foreach (int j in Enumerable.Range(1, 26))
                {
                    server.SendMessage("Ping",
                        $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}", j.ToString());
                    Thread.Sleep(rand.Next(100, 1 * 1000));
                }
                Thread.Sleep(5000);
                server.Stop();
                Thread.Sleep(5000);
            }
            ConsoleSync.WriteReadLineT("Press any key to CloseConnections");
            // server.ClearAllSocket();
            // server.Stop();
            ConsoleSync.WriteReadLineT("Press any key to Finish");

        }
    }
}
