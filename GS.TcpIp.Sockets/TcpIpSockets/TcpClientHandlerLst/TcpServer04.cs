using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Containers5;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Tasks;

using EventHub = GS.EventHubs.EventHub1.EventHub;
using TcpClientHandler = TcpIpSockets.TcpClientHandlerLst.TcpClientHandler;
using EventArgs = GS.Events.EventArgs;

namespace TcpIpSockets.TcpClientHandlerLst
{
    public partial class TcpServer04 : Element32<string, IEventArgs1, 
                    DictionaryContainer<string, ITcpClientHandler>, ITcpClientHandler>, ITcpServer
    {
        const int EchoPort = 8082;
        const string EchoIpAddress = "127.0.0.1";
        protected TcpListener TcpListener;
        public TaskBase WorkTask { get; private set; }
        public TaskBase02 MainActionTask { get; private set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool IsSubscribeAvailable { get; set; }
        public int GetClientNameAttemptsNumber { get; set; }
        public string ServerName => ServerFullName.Left(8);
        public string ServerFullName { get; }
        public TcpServer04()
        {
            Code = "TcpServer";
            Port = EchoPort;
            IpAddress = EchoIpAddress;
            ServerFullName = Guid.NewGuid().ToString();
            GetClientNameAttemptsNumber = 15;
            Collection = new DictionaryContainer<string, ITcpClientHandler>();
        }
        public TcpServer04(string ipAddress, int port)
        {
            Code = "TcpServer";
            Port = port;
            IpAddress = ipAddress;
            ServerFullName = Guid.NewGuid().ToString();
            GetClientNameAttemptsNumber = 15;
            Collection = new DictionaryContainer<string, ITcpClientHandler>();
        }
        public override void Init()
        {
            base.Init();
            SetupEventHub();
        }
        public void Start()
        {
            try
            {
                EventHub.Start();
                Thread.Sleep(1000);

                MainActionTask = new TaskBase02
                {
                    ClientName = Key,
                    TimeIntervalForMainTaskCompletingSeconds = 60
                };
                MainActionTask.Start(TcpListenerMainAction);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void SendMessage(List<string> dto)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                if (IsSubscribeAvailable)
                {
                    var msg = new GS.EventHubs.Message(dto);
                    EventHub.EnQueue(msg);
                }
                else
                {
                    foreach (var i in Items)
                        i.SendMessage(dto);
                }
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void SendMessage(params string[] s)
        {
            try
            {
                var lst = new List<string>();
                lst.AddRange(s);
                SendMessage(lst);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        private void TcpListenerMainAction(CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            Console.WriteLine($"Welcome to {ToString()} {IpAddress} {Port}");
            //TcpListener clientListener = new TcpListener(IPAddress.Parse(IpAddress), port: port);
            TcpListener = new TcpListener(IPAddress.Parse(IpAddress), port: Port);
            TcpListener.Start();
            Console.WriteLine("Waiting for connection...");
            try
            {
                while (true)
                {
                    if (canceltoken.IsCancellationRequested)
                        canceltoken.ThrowIfCancellationRequested();

                    TcpClient client = TcpListener.AcceptTcpClient();
                    var starter = new TcpClientHandlerStarter
                    {
                        Parent = this,
                        TcpClient = client,
                        TcpServer = this
                    };
                    starter.Start();
                }
            }
            catch (SocketException e)
            {
                e.PrintExceptions(this, m);
            }
            catch (System.IO.IOException e)
            {
                e.PrintExceptions(this, m);
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
            }
            ConsoleSync.WriteLineT($"{m} {Key} {TcpListener.LocalEndpoint} Stoped");
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
        public void Stop()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // PrintTcpClientCollection();
            var j = 0;
            foreach (var i in Items)
            {
                // ConsoleSync.WriteLineT($"{m} CloseClient {++j}: {i.Key}");
                i.Stop();
            }
            //ConsoleSync.WriteLineT($"Total: {j} items");
            //Thread.Sleep(1000);

            TcpListener?.Stop();
            Thread.Sleep(1000);
            EventHub.Stop();
            ConsoleSync.WriteLineT($"{m} {Key}");
        }
        public void CloseClients()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // PrintTcpClientCollection();
            var j = 0;
            foreach (var i in Items)
            {
                ConsoleSync.WriteLineT($"{m}: CloseClient{++j}: {i.Key}");
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
                    ? $"{key} {m} Remove ClinetHandler Ok"
                    : $"{key} {m} Remove ClientHandler Failure");

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
        public void GetClientFullNameAction(TcpClientHandler tcpClientHandler)
        {
            while (tcpClientHandler.ClientFullName.HasNoValue())
            {
                ConsoleSync.WriteLineT($"Try to Get Client Full Name");
                tcpClientHandler
                    .SendMessage("ServerName", tcpClientHandler.ServerFullName, "");
                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }
        // Call from TcpClientHandlerStarter for DIAG
        public void Subscribe(string topicKey, EventHandler<List<string>> callback)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                EventHub?.Subscribe(topicKey, callback);
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                    $"{m}", $"Topic: {topicKey}", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
        public void Subscribe(string topicKey, EventHandler<List<string>> callback, ITcpClientHandler tcpHandler)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                EventHub.UnSubscribe(callback);
                EventHub.Subscribe(topicKey, callback);
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                    $"{m}", $"CientName: {tcpHandler.ClientName} Topic: {topicKey}", ToString());
                tcpHandler.SendMessage(new []{"SUBSCRIBED", "SUCCESS", topicKey});
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }

        public void UnSubscribe(EventHandler<List<string>> callback)
        {
            // var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                EventHub?.UnSubscribe(callback);
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }

        public override string Key => $"{Code}: {ServerName}";
        public override void DeQueueProcess()
        {
            
        }
        public override string ToString()
        {
            return $"{Key}";
        }
    }
}
