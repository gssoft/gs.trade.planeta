using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpIpSockets;
using TcpClientHandler = TcpIpSockets.TcpClientHandler01.TcpClientHandler;

namespace Ca_Tcp_Sockets_Serv04
{
    public class EchoServer
    {
        const int EchoPort = 8082;
        public static int NClients = 0;
        public static void Main(string[] args)
        {
            try
            {
                TcpListener clientListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port: EchoPort);
                clientListener.Start();
                Console.WriteLine("Waiting for connection");

                while (true)
                {
                    TcpClient client = clientListener.AcceptTcpClient();
                    TcpClientHandler tcpClientHandler = new TcpClientHandler
                    {
                        ClientSocket = client,
                        // NetworkStream = client.GetStream(),
                        ServerFullName = Guid.NewGuid().ToString()
                    };
                    tcpClientHandler.Init();

                    //Thread clientTread = new Thread(new ThreadStart(cHandler.RunClient));
                    //clientTread.Start();

                    // Task.Factory.StartNew(cHandler.RunClient03);

                    // cHandler.RunPusher02(); json

                    // Task.Factory.StartNew(cHandler.RunPusher03);
                    // cHandler.RunPusher03(); // Binary

                    tcpClientHandler.Start();

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    tcpClientHandler
                        .SendMessage("ServerName", tcpClientHandler.ServerFullName,
                                                        tcpClientHandler.ClientFullName);
                    Thread.Sleep(TimeSpan.FromSeconds(3));

                    var t = Task.Factory.StartNew(p => SendMessageToClient((TcpClientHandler)p), tcpClientHandler);

                }
                clientListener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
        private static void SendMessageToClient(TcpClientHandler chandler)
        {
            var cHandler = (TcpClientHandler)chandler;
            var rand = new Random(DateTime.Now.Millisecond);
            foreach (var i in Enumerable.Range(1, 100))
            {
                //var dto = new[] { "Message", "Info", $"{cHandler.ClientName} Hello: {i}" };
                //cHandler.EnQueue(dto);
                cHandler.SendMessage("Message", "Info", $"{cHandler.ClientName} Hello: {i}");
                Thread.Sleep(TimeSpan.FromMilliseconds(rand.Next(1*100, 1 * 1000)));
                // Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
    }
}
