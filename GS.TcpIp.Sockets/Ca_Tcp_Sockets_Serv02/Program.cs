using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ca_Tcp_Sockets_Serv02;
using GS.Serialization;
using Newtonsoft.Json;
using TcpIpSockets;
using TcpIpSockets.Extensions;

namespace CA_Tcp_Sockets_Serv02
{
    public class ClientHandler
    {
        public string UserName { get; set; }

        public ClientHandler()
        {
            _rand = new Random(DateTime.Now.TimeOfDay.Milliseconds);
        }
        public TcpClient ClientSocket;

        private Random _rand;

        private enum ReadFromSocketMode { BuffferToStringBuilder, StreamReaderToString}

        public void RunClient01()
        {
            // var readMode = ReadFromSocketMode.StreamReaderToString;

            var networkStream = ClientSocket.GetStream();

            StreamReader readerStream  = new StreamReader(ClientSocket.GetStream());
            NetworkStream writerStream = ClientSocket.GetStream();

            string returnData = readerStream.ReadLine();
            string userName = returnData;

            Console.WriteLine($"Welcome {userName} to the Server");

            while (true)
            {
                try
                {
                    // var str = ReadFromNetworkStream(networkStream);
                    returnData = networkStream.ReadFrStreamReader(1024,
                                        Console.WriteLine,
                                        e=>Console.WriteLine(e?.ToString()));

                    if (returnData == null)
                    {
                        Console.WriteLine($"{userName}: Failure in Socket Stream Reading");
                        break;
                    }
                    if (returnData.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        Console.WriteLine("Bye bye " + userName);
                        break;
                    }

                    networkStream.Write(returnData, e=> Console.WriteLine(e.ToString()));

                    // returnData = readerStream.ReadLine();
                   
                   // Console.WriteLine($"{userName}:{returnData}");
                   // returnData += Environment.NewLine;
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }           
            ClientSocket.Close();
        }
        public void RunClient02()
        {
            // var readMode = ReadFromSocketMode.StreamReaderToString;

            var networkStream = ClientSocket.GetStream();

            //StreamReader readerStream = new StreamReader(ClientSocket.GetStream());
            //NetworkStream writerStream = ClientSocket.GetStream();

            string returnData = networkStream.ReadFrStrBuilder(1024);
            string userName = returnData;

            Console.WriteLine($"Welcome {userName} to the Server");

            while (true)
            {
                try
                {
                    returnData = networkStream.ReadFrStrBuilder(1024,
                                        Console.WriteLine,
                                        e => Console.WriteLine(e?.ToString()));

                    if (returnData == null)
                    {
                        Console.WriteLine($"{userName}: Failure in Socket Stream Reading");
                        break;
                    }
                    if (returnData.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var str = "Bye bye " + userName;
                        Console.WriteLine(str);
                        networkStream.Write(str);
                        break;
                    }

                    networkStream.Write(returnData, e => Console.WriteLine(e.ToString()));
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            Console.WriteLine($"UserName: {userName} ClientSocket Closing ... ");
            ClientSocket.Close();
        }

        public void RunClient03()
        {
            // var readMode = ReadFromSocketMode.StreamReaderToString;

            var networkStream = ClientSocket.GetStream();

            //StreamReader readerStream = new StreamReader(ClientSocket.GetStream());
            //NetworkStream writerStream = ClientSocket.GetStream();

            // Tuple<string, int> returnData;

            var ret = networkStream.ReadFrStrBuilderTpl(1024);
            var userName = ret.Item1;

            Console.WriteLine($"Welcome {userName} to the Server");

            while (true)
            {
                try
                {
                    ret = networkStream.ReadFrStrBuilderTpl(1024,
                                        Console.WriteLine,
                                        e => Console.WriteLine(e?.ToString()));
                    var returnData = ret.Item1;
                    var retCode = ret.Item2;

                    if (retCode < 0 )
                    {
                        Console.WriteLine($"{userName}: Failure in Socket Stream Reading");
                        break;
                    }
                    if (returnData.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    {
                        var str = "Bye bye " + userName;
                        Console.WriteLine(str);
                        networkStream.Write(str);
                        break;
                    }
                    networkStream.Write(returnData, e => Console.WriteLine(e.ToString()));

                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            Console.WriteLine($"UserName: {userName} ClientSocket Closing ... ");
            ClientSocket.Close();
        }

        public void RunPusher01()
        {
            // var readMode = ReadFromSocketMode.StreamReaderToString;

            var mesages = new[]
            {
                @"`1234567890-=\][poiuytrewqasdfghjkl;'/.,mnbvcxz",
                @"/.,mnbvcxzasdfghjkl;'][poiuytrewq`1234567890-=\",
                @"\=-0987654321`qwertyuiop[]';lkjhgfdsazxcvbnm,./",
                @"zxcvbnm,./';lkjhgfdsaqwertyuiop[]\=-0987654321ё",
                @"ё1234567890-=\ъхзщшгнекуцйфывапролджэ.юбьтимсчя",
                @".юбьтимсчяфывапролджэъхзщшгнекуцйё1234567890-=\",
                @"\=-0987654321ёйцукенгшщзхъэждлорпавыфячсмитьбю.",
                @"ячсмитьбю.эждлорпавыфйцукенгшщзхъ\=-0987654321ё"
            };

            var networkStream = ClientSocket.GetStream();

            //StreamReader readerStream = new StreamReader(ClientSocket.GetStream());
            //NetworkStream writerStream = ClientSocket.GetStream();

            // Tuple<string, int> returnData;

            var ret = networkStream.ReadFrStrBuilderTpl(1024);
            var userName = ret.Item1;

            Console.WriteLine($"Welcome {userName} to the Server");

            while (true)
            {
                try
                {
                    //ret = networkStream.ReadFrStrBuilderTpl(1024,
                    //                    Console.WriteLine,
                    //                    e => Console.WriteLine(e?.ToString()));
                    //var returnData = ret.Item1;
                    //var retCode = ret.Item2;

                    //if (retCode < 0)
                    //{
                    //    Console.WriteLine($"{userName}: Failure in Socket Stream Reading");
                    //    break;
                    //}
                    //if (returnData.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    //{
                    //    var str = "Bye bye " + userName;
                    //    Console.WriteLine(str);
                    //    networkStream.Write(str);
                    //    break;
                    //}

                    var index = _rand.Next(0, 7);
                    var data = mesages[index];

                    var sendDelay = _rand.Next(100, 1*1000);

                    networkStream.Write(data, Console.WriteLine, e => Console.WriteLine(e.ToString()));
                    Thread.Sleep(sendDelay);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            Console.WriteLine($"UserName: {userName} ClientSocket Closing ... ");
            ClientSocket.Close();
        }

        public void RunPusher02()
        {
            // var readMode = ReadFromSocketMode.StreamReaderToString;

            var mesages = new[]
            {
                @"`1234567890-=\][poiuytrewqasdfghjkl;'/.,mnbvcxz",
                @"/.,mnbvcxzasdfghjkl;'][poiuytrewq`1234567890-=\",
                @"\=-0987654321`qwertyuiop[]';lkjhgfdsazxcvbnm,./",
                @"zxcvbnm,./';lkjhgfdsaqwertyuiop[]\=-0987654321ё",
                @"ё1234567890-=\ъхзщшгнекуцйфывапролджэ.юбьтимсчя",
                @".юбьтимсчяфывапролджэъхзщшгнекуцйё1234567890-=\",
                @"\=-0987654321ёйцукенгшщзхъэждлорпавыфячсмитьбю.",
                @"ячсмитьбю.эждлорпавыфйцукенгшщзхъ\=-0987654321ё"
            };

            var networkStream = ClientSocket.GetStream();

            //StreamReader readerStream = new StreamReader(ClientSocket.GetStream());
            //NetworkStream writerStream = ClientSocket.GetStream();

            // Tuple<string, int> returnData;

            var ret = networkStream.ReadFrStrBuilderTpl(1024);
            var userName = ret.Item1;

            Console.WriteLine($"Welcome {userName} to the Server");

            while (true)
            {
                try
                {
                    //ret = networkStream.ReadFrStrBuilderTpl(1024,
                    //                    Console.WriteLine,
                    //                    e => Console.WriteLine(e?.ToString()));
                    //var returnData = ret.Item1;
                    //var retCode = ret.Item2;

                    //if (retCode < 0)
                    //{
                    //    Console.WriteLine($"{userName}: Failure in Socket Stream Reading");
                    //    break;
                    //}
                    //if (returnData.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    //{
                    //    var str = "Bye bye " + userName;
                    //    Console.WriteLine(str);
                    //    networkStream.Write(str);
                    //    break;
                    //}

                    var index = _rand.Next(0, 7);
                    var data = mesages[index];

                    //var dto = new TcpDataDto
                    //{
                    //    RouteKey = "Quotes",
                    //    Operation = "Add",

                    //};

                    var dto = new[] {"RouteKey", "Operation", mesages[index]};
                    // var dtoBytes = BinarySerialization.SerializeToByteArray(dto);
                    var dtoBytes = JsonConvert.SerializeObject(dto);
                    Console.WriteLine(dtoBytes);

                  //  Thread.Sleep(5000);

                    var sendDelay = _rand.Next(1000, 5 * 1000);

                    networkStream.Write(dtoBytes, Console.WriteLine, e => Console.WriteLine(e.ToString()));
                    Thread.Sleep(sendDelay);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            Console.WriteLine($"UserName: {userName} ClientSocket Closing ... ");
            ClientSocket.Close();
        }

        public void RunPusher03()
        {
            // var readMode = ReadFromSocketMode.StreamReaderToString;

            var mesages = new[]
            {
                @"`1234567890-=\][poiuytrewqasdfghjkl;'/.,mnbvcxz",
                @"/.,mnbvcxzasdfghjkl;'][poiuytrewq`1234567890-=\",
                @"\=-0987654321`qwertyuiop[]';lkjhgfdsazxcvbnm,./",
                @"zxcvbnm,./';lkjhgfdsaqwertyuiop[]\=-0987654321ё",
                @"ё1234567890-=\ъхзщшгнекуцйфывапролджэ.юбьтимсчя",
                @".юбьтимсчяфывапролджэъхзщшгнекуцйё1234567890-=\",
                @"\=-0987654321ёйцукенгшщзхъэждлорпавыфячсмитьбю.",
                @"ячсмитьбю.эждлорпавыфйцукенгшщзхъ\=-0987654321ё"
            };

            var networkStream = ClientSocket.GetStream();

            //StreamReader readerStream = new StreamReader(ClientSocket.GetStream());
            //NetworkStream writerStream = ClientSocket.GetStream();

            // Tuple<string, int> returnData;

            var ret = networkStream.ReadFrStrBuilderTpl(1024);
            UserName = ret.Item1;

            Console.WriteLine($"Welcome {UserName} to the Server");

            var sendValue = 0;

            while (true)
            {
                try
                {
                    //ret = networkStream.ReadFrStrBuilderTpl(1024,
                    //                    Console.WriteLine,
                    //                    e => Console.WriteLine(e?.ToString()));
                    //var returnData = ret.Item1;
                    //var retCode = ret.Item2;

                    //if (retCode < 0)
                    //{
                    //    Console.WriteLine($"{userName}: Failure in Socket Stream Reading");
                    //    break;
                    //}
                    //if (returnData.IndexOf("QUIT", StringComparison.OrdinalIgnoreCase) > -1)
                    //{
                    //    var str = "Bye bye " + userName;
                    //    Console.WriteLine(str);
                    //    networkStream.Write(str);
                    //    break;
                    //}

                    var index = _rand.Next(0, 7);
                    var data = mesages[index];

                    var guid = Guid.NewGuid();

                    //var dto = new TcpDataDto
                    //{
                    //    RouteKey = "Quotes",
                    //    Operation = "Add",

                    //};

                    // var dto = new[] { "RouteKey", "Operation", mesages[index] };

                    var dto = new[] { "RouteKey", "Operation", $"[{(++sendValue)}] {guid}" };
                    Console.WriteLine($"User:{UserName} Data:{dto[0]},{dto[1]},{dto[2]}");
                    var dtoBytes = BinarySerialization.SerializeToByteArray(dto);
                    // var dtoBytes = JsonConvert.SerializeObject(dto);
                    // Console.WriteLine(dtoBytes);

                    //  Thread.Sleep(5000);

                    var sendDelay = _rand.Next(1*1000, 2*1000);

                    networkStream.Write(dtoBytes, Console.WriteLine, e => Console.WriteLine(e.ToString()));
                    Thread.Sleep(sendDelay);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}");
                }
            }
            Console.WriteLine($"UserName: {UserName} ClientSocket Closing ... ");
            ClientSocket.Close();
        }

        private string ReadFromNetworkStream(NetworkStream netstream)
        {
            if (netstream.CanRead)
            {
                byte[] myReadBuffer = new byte[1024];
                StringBuilder myCompleteMessage = new StringBuilder();

                // Incoming message may be larger than the buffer size.
                do
                {
                    var numberOfBytesRead = netstream.Read(myReadBuffer, 0, myReadBuffer.Length);
                    myCompleteMessage.AppendFormat("{0}", Encoding.UTF8.GetString(myReadBuffer, 0, numberOfBytesRead));
                }
                while (netstream.DataAvailable);

                // Print out the received message to the console.
                Console.WriteLine("Server: " + myCompleteMessage);
                return myCompleteMessage.ToString();
            }
            {
                Console.WriteLine("Sorry. You cannot read from this NetworkStream.");
                return null;
            }
        }
    }
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
                    ClientHandler cHandler = new ClientHandler {ClientSocket = client};

                    //Thread clientTread = new Thread(new ThreadStart(cHandler.RunClient));
                    //clientTread.Start();

                    // Task.Factory.StartNew(cHandler.RunClient03);

                    // cHandler.RunPusher02(); json

                    Task.Factory.StartNew(cHandler.RunPusher03);
                    // cHandler.RunPusher03(); // Binary
                }
                clientListener.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
    }
}
