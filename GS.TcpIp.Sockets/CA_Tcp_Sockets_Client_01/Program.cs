using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using GS.ConsoleAS;
using GS.Extension;
using GS.Serialization;
using Newtonsoft.Json;
using TcpIpSockets;
using TcpIpSockets.Extensions;
using TcpClientHandler = TcpIpSockets.TcpClientHandler01.TcpClientHandler;

namespace CA_Tcp_Sockets_Client_01
{
    public class EchoCLient
    {
        const int EchoPort = 8082;
        const string IpAddress = "127.0.0.1";

        public static void Main(string[] args)
        {
            // TcpClient01();
            // TcpClient02();
            //TcpSubscriber01();
            // TcpSubscriber02();
            // TcpSubscriber03();
            // TcpSubscriber04();
            // TcpSubscriber05();
            TcpSubscriber07();
            //try
            //{
            //    var userName = Guid.NewGuid().ToString().Substring(0, 8);
            //    TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

            //    //// Sends data immediately upon calling NetworkStream.Write.
            //    //eClient.NoDelay = true;

            //    //// Determines if the delay is enabled by using the NoDelay property.
            //    //if (eClient.NoDelay == true)
            //    //    Console.WriteLine("The delay was set successfully to " + eClient.NoDelay.ToString());


            //    StreamReader sr = new StreamReader(eClient.GetStream());
            //    NetworkStream sw = eClient.GetStream();

            //    string dataToSend = userName + Environment.NewLine;
            //    byte[] data = Encoding.UTF8.GetBytes(dataToSend);

            //    sw.Write(data, 0, data.Length);

            //    while (true)
            //    {
            //        Console.Write(userName + ":");

            //        dataToSend = Console.ReadLine();
            //        dataToSend += Environment.NewLine;

            //        data = Encoding.UTF8.GetBytes(dataToSend);
            //        sw.Write(data, 0, data.Length);

            //        if (dataToSend.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1) break;

            //        var returnData = sr.ReadLine();

            //        Console.WriteLine($"Server: {returnData}");
            //    }

            //    eClient.Close();

            //    Console.WriteLine($"{userName}: Closing...");
            //    Console.ReadLine();
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //    Console.ReadLine();
            //}
        }

        private static void TcpClient01()
        {
            try
            {
                var userName = Guid.NewGuid().ToString().Substring(0, 8);
                TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

                //// Sends data immediately upon calling NetworkStream.Write.
                //eClient.NoDelay = true;

                //// Determines if the delay is enabled by using the NoDelay property.
                //if (eClient.NoDelay == true)
                //    Console.WriteLine("The delay was set successfully to " + eClient.NoDelay.ToString());


                StreamReader sr = new StreamReader(eClient.GetStream());
                NetworkStream sw = eClient.GetStream();

                string dataToSend = userName + Environment.NewLine;
                byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                sw.Write(data, 0, data.Length);

                while (true)
                {
                    Console.Write(userName + ":");

                    dataToSend = Console.ReadLine();
                    dataToSend += Environment.NewLine;

                    data = Encoding.UTF8.GetBytes(dataToSend);
                    sw.Write(data, 0, data.Length);

                    if (dataToSend.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1) break;

                    var returnData = sr.ReadLine();

                    Console.WriteLine($"Server: {returnData}");
                }
                eClient.Close();

                Console.WriteLine($"{userName}: Closing...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void TcpClient02()
        {
            try
            {
                var userName = Guid.NewGuid().ToString().Substring(0, 8);
                TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

                string dataToSend = userName; // + Environment.NewLine;

                //byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                NetworkStream nstrm = eClient.GetStream();
                nstrm.Write(dataToSend);

                while (true)
                {
                    Console.Write(userName + ":");

                    dataToSend = Console.ReadLine();
                    // dataToSend += Environment.NewLine;

                    nstrm.Write(dataToSend);

                    if (dataToSend.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var ret = nstrm.ReadFrStrBuilder(1024);
                        Console.WriteLine($"Server: {ret}");
                        break;
                    }

                    var returnData = nstrm.ReadFrStrBuilder(1024,
                            Console.WriteLine, 
                            e=>Console.WriteLine(e.ToString()));

                    Console.WriteLine($"Server: {returnData}");
                }
                eClient.Close();

                Console.WriteLine($"{userName}: Closing...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void TcpSubscriber01()
        {
            try
            {
                var userName = Guid.NewGuid().ToString().Substring(0, 8);
                TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

                string dataToSend = userName; // + Environment.NewLine;

                //byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                NetworkStream nstrm = eClient.GetStream();
                nstrm.Write(dataToSend);

                while (true)
                {
                    Console.Write(userName + ":");

                    // dataToSend = Console.ReadLine();
                    // dataToSend += Environment.NewLine;

                    // nstrm.Write(dataToSend);

                    //if (dataToSend.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    //{
                    //    var ret = nstrm.ReadFrStrBuilder(1024);
                    //    Console.WriteLine($"Server: {ret}");
                    //    break;
                    //}

                    //var returnData = nstrm.ReadFrStrBuilder(1024,
                    //        Console.WriteLine,
                    //        e => Console.WriteLine(e.ToString()));

                    var returnData = nstrm.ReadFrStrBuilder(1024, null,
                            e => Console.WriteLine(e.ToString()));

                    Console.WriteLine($"Server: {returnData}");
                }
                eClient.Close();

                Console.WriteLine($"{userName}: Closing...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void TcpSubscriber02()
        {
            try
            {
                var userName = Guid.NewGuid().ToString().Substring(0, 8);
                TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

                string dataToSend = userName; // + Environment.NewLine;

                //byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                NetworkStream networkStream = eClient.GetStream();
                networkStream.Write(dataToSend);

                while (true)
                {
                    Console.Write(userName + ":");

                    // dataToSend = Console.ReadLine();
                    // dataToSend += Environment.NewLine;

                    // nstrm.Write(dataToSend);

                    //if (dataToSend.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    //{
                    //    var ret = nstrm.ReadFrStrBuilder(1024);
                    //    Console.WriteLine($"Server: {ret}");
                    //    break;
                    //}

                    //var returnData = nstrm.ReadFrStrBuilder(1024,
                    //        Console.WriteLine,
                    //        e => Console.WriteLine(e.ToString()));

                    var dtoBytes = networkStream.ReadFrStrBuilder(1024, null,
                            e => Console.WriteLine(e.ToString()));

                    //var dtoBytes = networkStream.ReadToBytes(eClient.ReceiveBufferSize,
                    //    Console.WriteLine, e => Console.WriteLine(e.ToString()));

                    if (dtoBytes == null) return;

                    // var dto = BinarySerialization.DeSerialize<string[]>(dtoBytes);
                    var dto = JsonConvert.DeserializeObject<string[]>(dtoBytes);

                    //var returnData = networkStream.ReadFrStrBuilder(1024, null,
                    //        e => Console.WriteLine(e.ToString()));
                    Console.WriteLine("Server:");
                    Console.WriteLine($"{dto[0]},{dto[1]},{dto[2]}");
                }
                eClient.Close();

                Console.WriteLine($"{userName}: Closing...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void TcpSubscriber03()
        {
            try
            {
                var userName = Guid.NewGuid().ToString().Substring(0, 8);
                TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

                string dataToSend = userName; // + Environment.NewLine;

                //byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                NetworkStream networkStream = eClient.GetStream();
                networkStream.Write(dataToSend);

                while (true)
                {
                    try
                    {
                        Console.Write(userName + ":");

                        var dtoBytes = networkStream.ReadToBytes(eClient.ReceiveBufferSize,
                            Console.WriteLine, e => Console.WriteLine(e.ToString()));

                        // Console.WriteLine($"BytesCnt:{dtoBytes.Count()}");

                        if (dtoBytes == null)
                        {
                            Console.WriteLine($"Bytes Received is null");
                            continue;
                        }
                        if (dtoBytes.Length <= 0)
                        {
                            Console.WriteLine($"Bytes Received is empty");
                            continue;
                        }
                        var dto = BinarySerialization.DeSerialize<string[]>(dtoBytes);
                        Console.WriteLine($"Data:{dto[0]},{dto[1]},{dto[2]}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}");
                    }}

                eClient.Close();
                Console.WriteLine($"{userName}: Closing...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void TcpSubscriber04()
        {
            try
            {
                
                var userFullName = Guid.NewGuid().ToString();
                var userName = userFullName.Left(8);
                TcpClient eClient = new TcpClient("127.0.0.1", EchoPort);

                string dataToSend = userName; // + Environment.NewLine;

                //byte[] data = Encoding.UTF8.GetBytes(dataToSend);

                NetworkStream networkStream = eClient.GetStream();

                var strArr = new[] { "UserName","New", $"{userFullName}" };
                Console.WriteLine($"User:{userName} Data:{strArr[0]},{strArr[1]},{strArr[2]}");
                var dbytes = BinarySerialization.SerializeToByteArray(strArr);

                networkStream.Write(dbytes);

                while (true)
                {
                    try
                    {
                        Console.Write(userName + ":");

                        var dtoBytes = networkStream.ReadToBytes(eClient.ReceiveBufferSize,
                            Console.WriteLine, e => Console.WriteLine(e.ToString()));

                        // Console.WriteLine($"BytesCnt:{dtoBytes.Count()}");

                        if (dtoBytes == null)
                        {
                            Console.WriteLine($"Bytes Received is null");
                            continue;
                        }
                        if (dtoBytes.Length <= 0)
                        {
                            Console.WriteLine($"Bytes Received is empty");
                            continue;
                        }
                        var dto = BinarySerialization.DeSerialize<string[]>(dtoBytes);
                        Console.WriteLine($"Data:{dto[0]},{dto[1]},{dto[2]}");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e.Message}");
                    }
                }

                eClient.Close();
                Console.WriteLine($"{userName}: Closing...");
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }

        private static void TcpSubscriber05()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var userFullName = Guid.NewGuid().ToString();
            var userName = userFullName.Left(8);
            TcpClient client;
            try
            {
                //TcpClient client = new TcpClient(IpAddress, EchoPort);
                client = new TcpClient();
                if (!client.ConnectAsync(IpAddress, EchoPort).Wait(5000))
                {
                    ConsoleSync.WriteLineT($"Can't Connect to {IpAddress} {EchoPort}");
                    ConsoleSync.WriteReadLineT($"Press Key to Exit");
                    return;
                }
            }
            catch (Exception e)
            {
                e.PrintExceptions(m);
                ConsoleSync.WriteLineT($"Can't Connect to {IpAddress} {EchoPort}");
                ConsoleSync.WriteReadLineT($"Press Key to Exit");
                return;
            }

            //TcpClientHandler cHandler = new TcpClientHandler(client);
            TcpClientHandler tcpClientHandler = new TcpClientHandler
            {
                ClientSocket = client,
              //  NetworkStream = client.GetStream(),
                ClientFullName = Guid.NewGuid().ToString(),
                IsPingEnabled = true,
                TimeIntervalForTaskCompleting = 15
            };

            tcpClientHandler.Init();
            tcpClientHandler.Start();
            Thread.Sleep(TimeSpan.FromSeconds(1));

            ConsoleSync.WriteReadLineT("To Continue Press any key ...");

            //tcpClientHandler
            //    .SendMessage("ClientName", tcpClientHandler.ServerFullName, tcpClientHandler.ClientFullName);

            //Thread.Sleep(TimeSpan.FromSeconds(1));

            var rand = new Random(DateTime.Now.Millisecond);
            foreach (int i in Enumerable.Range(1, 100))
            {
                tcpClientHandler.SendMessage("Ping",
                    $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}",
                    i.ToString());
                Thread.Sleep(rand.Next(100, 1 * 1000));
            }
            // ConsoleSync.WriteReadLineT("To Stop Press any key ...");
            tcpClientHandler.Stop();
            ConsoleSync.WriteReadLineT("To Close Socket Press any key ...");
            tcpClientHandler.SendMessage("Close","" , "");
            // Thread.Sleep(TimeSpan.FromSeconds(2));
            // ConsoleSync.WriteReadLineT("To Finish Press any key ...");
            tcpClientHandler.Stop();
            ConsoleSync.WriteReadLineT("To Finish Press any key ...");
            // client.Close();
        }

        private static void TcpSubscriber07()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var userFullName = Guid.NewGuid().ToString();
            var userName = userFullName.Left(8);
            TcpClient client;
            TcpClientHandler tcpClientHandler;
            while (true)
            {
                try
                {
                    client = new TcpClient(IpAddress, EchoPort);
                    tcpClientHandler = new TcpClientHandler
                    {
                        ClientSocket = client,
                        // NetworkStream = client.GetStream(),
                        ClientFullName = Guid.NewGuid().ToString(),
                        IsPingEnabled = true,
                        TimeIntervalForTaskCompleting = 15
                    };
                    tcpClientHandler.Init();
                    tcpClientHandler.Start();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    break;
                }
                catch (SocketException e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"Can't Connect to {IpAddress} {EchoPort}");
                    ConsoleSync.WriteLineT($"{m} Press Key to Exit");
                    // break;
                    // return;
                }
                catch (Exception e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"Can't Connect to {IpAddress} {EchoPort}");
                    ConsoleSync.WriteLineT($"{m} Press Key to Exit");
                    // return;
                }
                Thread.Sleep(15000);
            }

            //TcpClientHandler cHandler = new TcpClientHandler(client);
            //TcpClientHandler tcpClientHandler = new TcpClientHandler
            //{
            //    ClientSocket = client,
            //    NetworkStream = client.GetStream(),
            //    ClientFullName = Guid.NewGuid().ToString(),
            //    IsPingEnabled = true,
            //    TimeIntervalForTaskCompleting = 15
            //};
            //tcpClientHandler.Init();
            //tcpClientHandler.Start();
            //Thread.Sleep(TimeSpan.FromSeconds(1));

            ConsoleSync.WriteReadLineT("To Continue Press any key ...");

            //tcpClientHandler
            //    .SendMessage("ClientName", tcpClientHandler.ServerFullName, tcpClientHandler.ClientFullName);

            //Thread.Sleep(TimeSpan.FromSeconds(1));

            var rand = new Random(DateTime.Now.Millisecond);
            foreach (int i in Enumerable.Range(1, 100))
            {
                tcpClientHandler.SendMessage("Ping",
                    // $"{DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff")}",
                    $"{DateTime.Now.TimeOfDay:hh\\:mm\\:ss\\.fff}",
                    i.ToString());
                Thread.Sleep(rand.Next(100, 1 * 1000));
            }
            // ConsoleSync.WriteReadLineT("To Stop Press any key ...");
            tcpClientHandler.Stop();
            ConsoleSync.WriteReadLineT("To Close Socket Press any key ...");
            tcpClientHandler.SendMessage("Close", "", "");
            // Thread.Sleep(TimeSpan.FromSeconds(2));
            // ConsoleSync.WriteReadLineT("To Finish Press any key ...");
            tcpClientHandler.Stop();
            ConsoleSync.WriteReadLineT("To Finish Press any key ...");
            // client.Close();
        }
    }
}
