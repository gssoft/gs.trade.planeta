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
    public class TcpServer02 : Element32<string, IEventArgs1, 
                    DictionaryContainer<string, ITcpClientHandler>, ITcpClientHandler>, ITcpServer
    {
        const int EchoPort = 8082;
        const string EchoIpAddress = "127.0.0.1";
        protected TcpListener TcpListener;
        public TaskBase WorkTask { get; private set; }
        public TaskBase02 MainActionTask { get; private set; }
        public CancellationToken CancellationToken { get; private set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool IsSubscribeAvailable { get; set; }
        public int GetClientNameAttemptsNumber { get; set; }
        public string ServerName => ServerFullName.Left(8);
        public string ServerFullName { get; }
        public TcpServer02()
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
            //Task = new Task(p=>TcpListenerMainAction(CancellationToken), CancellationToken);
            //Task.Start();
            MainActionTask = new TaskBase02
            {
                ClientName = Key,
                TimeIntervalForMainTaskCompletingSeconds = 60
            };
            MainActionTask.Start(TcpListenerMainAction);
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

        

        public void Subscribe(string key, EventHandler<IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentAndMyTypeName, TypeName,
                   $"{m}", "Not Suported", ToString());
        }

        public void Subscribe(string key, EventHandler<IEventArgs> callback, ITcpClientHandler tcpHandler)
        {
            throw new NotImplementedException();
        }

        public void Subscribe(string key, string clientname, EventHandler<IEventArgs> callback)
        {
            throw new NotImplementedException();
        }

        public void UnSubscribe(EventHandler<IEventArgs> callback)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentAndMyTypeName, TypeName,
                   $"{m}", "Not Suported", ToString());
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
                        //break;
                        canceltoken.ThrowIfCancellationRequested();

                    TcpClient client = TcpListener.AcceptTcpClient();
                    var starter = new TcpClientHandlerStarter
                    {
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
            ConsoleSync.WriteLineT($"{m} {Key}");
        }
        public void CloseAllClients()
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
