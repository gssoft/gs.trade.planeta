using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.Collections;
using GS.ConsoleAS;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Tasks;
using TcpIpSockets.TcpClientHandler01;
using TcpClientHandler = TcpIpSockets.TcpClientHandler01.TcpClientHandler;

namespace TcpIpSockets
{

    public class TcpServer01 : Element32<string, IEventArgs1, 
                    DictionaryContainer<string, ITcpClientHandler>, ITcpClientHandler>, ITcpServer
    {
        const int EchoPort = 8082;
        const string EchoIpAddress = "127.0.0.1";
        public TaskBase WorkTask { get; private set; }
        public Task Task { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool IsSubscribeAvailable { get; set; }
        public int GetClientNameAttemptsNumber { get; set; }
        public string ServerName => ServerFullName.Left(8);
        public string ServerFullName { get; }

        public TcpServer01()
        {
            Code = "TcpServer";
            Port = EchoPort;
            IpAddress = EchoIpAddress;
            ServerFullName = Guid.NewGuid().ToString();
            GetClientNameAttemptsNumber = 15;
            Collection = new DictionaryContainer<string, ITcpClientHandler>();
        }
        public void Start()
        {
            var cts = new CancellationTokenSource();
            CancellationToken = cts.Token;
            Task = new Task(Main, CancellationToken);
            Task.Start();
        }

        public void Stop()
        {
        }

        public void CloseClients()
        {
            throw new NotImplementedException();
        }

        public void SendMessage(string[] m)
        {
            foreach (var i in Items)
                i.SendMessage(m);
        }

        public void SendMessage(string route, string operation, string data)
        {
            foreach(var i in Items)
                i.SendMessage(route, operation, data);
        }

        public void Subscribe(string key,  EventHandler<IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentAndMyTypeName, TypeName,
                   $"{m}", "Not Suported", ToString() );
        }

        public void Subscribe(string key, EventHandler<IEventArgs> callback, ITcpClientHandler chandler)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string key, string clientname, EventHandler<IEventArgs> callback)
        {
            throw new NotImplementedException();
        }

        public void UnSubscribe(EventHandler<IEventArgs> callback)
        {
            throw new NotImplementedException();
        }

        private void Main()
        {
            var v = MethodBase.GetCurrentMethod().Name + "()";

            Console.WriteLine($"Welcome to {ToString()}");
            TcpListener clientListener = new TcpListener(IPAddress.Parse(IpAddress), port: Port);
            clientListener.Start();
            Console.WriteLine("Waiting for connection...");

            try
            {
                while (true)
                {
                    TcpClient client = clientListener.AcceptTcpClient();
                    TcpClientHandler tcpClientHandler = new TcpClientHandler
                    {
                        ClientSocket = client,
                        // NetworkStream = client.GetStream(),
                        ServerFullName = ServerFullName,
                        TcpServer = this
                    };
                    tcpClientHandler.Init();
                    tcpClientHandler.SocketClosed += (sender, s) => UnRegisterTcpClient(s);
                    tcpClientHandler.Start();
                    Task.Factory.StartNew(p => RegisterTcpClient(tcpClientHandler), tcpClientHandler);
                }
            }
            catch (Exception e)
            {
                e.PrintExceptions(this,v);
            }
        }
        public void ClearAllSocket()
        {
            var v = MethodBase.GetCurrentMethod().Name + "()";
            PrintTcpClientCollection();
            var j = 0;
            foreach (var i in Items)
            {
                ConsoleSync.WriteLineT($"Clear TcpClient{++j}: {i.Key}");
                i.ClearSocket();
            }
            ConsoleSync.WriteLineT($"Total: {j} items");
        }
        public void CloseRequestToAllClients()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // PrintTcpClientCollection();
            var j = 0;
            foreach (var i in Items)
            {
                ConsoleSync.WriteLineT($"{m}: CloseClient{++j}: {i.Key}");
                i.SendMessage("Close","","");    
                i.Stop();           
            }
            ConsoleSync.WriteLineT($"Total: {j} items");
        }
        public void UnRegisterTcpClient(string key)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var b = Collection.Remove(key);
                ConsoleSync.WriteLineT(
                    b 
                    ? $"{m}:{key} Remove ClinetHandler Ok"
                    : $"{m}:{key} Remove ClientHandler Failure");

               // PrintTcpClientCollection();
            }
            catch (Exception e)
            {
                ConsoleSync.WriteLineT(e.Message);
            }            
        }
        private void PrintTcpClientCollection()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var j = 0;
            foreach (var i in Items)
                ConsoleSync.WriteLineT($"{m}: TcpClient{++j}: {i.Key}");
            ConsoleSync.WriteLineT($"{m}: Total: {j} items");
        }

        public void RegisterTcpClient(ITcpClientHandler tcpClient)
        {
            var tcpClientHandler = tcpClient;
            var nretries = GetClientNameAttemptsNumber;
            while (tcpClientHandler.ClientFullName.HasNoValue() && nretries-- > 0)
            {
                Console.WriteLine($"Try to Get Client Name");
                tcpClientHandler
                    .SendMessage("ServerName", tcpClientHandler.ServerFullName, "");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            if (tcpClientHandler.ClientFullName.HasValue())
            {
                Register(tcpClientHandler);
                foreach (var i in Items)
                {
                    Console.WriteLine($"Registered:{i}");
                }                
            }
            else
            {
                ConsoleSync.WriteReadLine(
                    $"Stop TcpClientHandler.Client does not answer{Environment.NewLine}Press any key");
                tcpClientHandler.ClientSocket.Close();
            }
        }
        public async Task<bool> RegisterTcpClientAsync(TcpClientHandler tcpClientHandler)
        {
            
            var nretries = GetClientNameAttemptsNumber;
            while (tcpClientHandler.ClientFullName.HasNoValue() && nretries-- > 0)
            {
                tcpClientHandler
                    .SendMessage("ServerName", tcpClientHandler.ServerFullName, "");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
            if (tcpClientHandler.ClientFullName.HasValue())
            {
                Register(tcpClientHandler);
                return true;
            }
            else
            {
                ConsoleSync.WriteReadLine(
                    $"Stop TcpClientHandler. Client does not answer{Environment.NewLine}Press any key");
                tcpClientHandler.ClientSocket.Close();
                return false;
            }
        }
        public override string Key => Code;
        public override void DeQueueProcess()
        {
            
        }
        public override string ToString()
        {
            return $"{Code}:{ServerName}";
        }

    }
}
