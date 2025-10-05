using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using GS.ConsoleAS;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Tasks;
using TcpIpSockets.Extensions;
using TcpClientHandler = TcpIpSockets.TcpClientHandler01.TcpClientHandler;

namespace TcpIpSockets.TcpClient03
{
    public partial class TcpClient03
    {
        private const int PortPar = 8082;
        private const string IpAddressPar = "127.0.0.1";
        private const int ConnectionTimeOut = 15000;
        private const int TryToConnectTimeIntervalPar = 15;

        public TcpClient TcpClient { get; private set; }
        public ITcpClientHandler TcpClientHandler { get; set; }
        public int Port { get; set; }
        public string IpAddress { get; set; }
        public int TryToConnectTimeInterval { get; set; }

        public TaskBase02 MainActionTask { get; private set; }
        public override string Key => Code.HasValue() ? Code : GetType().Name;

        private bool _isNeedRestartAfterSocketClosed;
        public TcpClient03()
        {
            Port = PortPar;
            IpAddress = IpAddressPar;
            TryToConnectTimeInterval = TryToConnectTimeIntervalPar;           
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            IsNeedEventHub = true;
            IsNeedEventLog = true;
            base.Init();
            //ChangedEvent += (sender, args) =>
            //{
            //    ConsoleSync.WriteLineT($"********** {m} {args.Key} {args.Operation} {args.Object.ToString()}");
            //};
            EventHub.Subscribe("TcpClientHandler", "Socket", SocketEventsProcessing);
        }
        private void SocketEventsProcessing(object sender, IEventArgs eventArgs)
        {
            switch (eventArgs.Operation)
            {
                case "CLOSED":
                ReStart(CancellationToken.None);
                break;
            }
        }
        public void CreateTcpClientHandler(CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var userFullName = Guid.NewGuid().ToString();
            var userName = userFullName.Left(8);
            ConsoleSync.WriteLineT($"{Key} {m} Try Connect to {IpAddress} {Port}");

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                             $"{Key} Try Connect to {IpAddress} {Port}", ToString());
            while (true)
            {
                try
                {
                    if (canceltoken.IsCancellationRequested)
                        canceltoken.ThrowIfCancellationRequested();

                    TcpClient = new TcpClient(IpAddress, Port);
                    ConsoleSync.WriteLineT($"{Key} {m} Connected Ok to {IpAddress} {Port}");
                    Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                             $"{Key} Connected Ok to {IpAddress} {Port}", ToString());

                    TcpClientHandler = new TcpClientHandler
                    {
                        Parent = this,
                        ClientSocket = TcpClient,
                       //  NetworkStream = TcpClient.GetStream(),
                        ClientFullName = Guid.NewGuid().ToString(),
                        IsPingEnabled = true,
                        TimeIntervalForTaskCompleting = 15
                    };
                    // TcpClientHandler.SocketClosed += OnSocketClosed;
                    TcpClientHandler.Init();
                    TcpClientHandler.Start();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    break;
                }
                catch (SocketException e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, $"Can't Connect to {IpAddress} {Port}", e.Message);
                    TcpClient?.Close();
                }
                catch (OperationCanceledException e)
                {
                    e.PrintExceptions(this, m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString() ,m , $"Can't Connect to {IpAddress} {Port}", e.Message);
                    TcpClient?.Close();
                    break;
                }
                catch (Exception e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, $"Can't Connect to {IpAddress} {Port}", e.Message);
                    //                  
                    break;
                }
                Thread.Sleep(TimeSpan.FromSeconds(TryToConnectTimeInterval));
            }
            ConsoleSync.WriteLineT($"{m} Finished");
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, $"Finished. Connection:{TcpClient?.Connected}", ToString());
        }
        public override void Start()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{Key} {m} Try to Start");
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                 "Try to Start", ToString());
            _isNeedRestartAfterSocketClosed = true;
            MainActionTask = new TaskBase02
            {
                ClientName = Code,
                TimeIntervalForMainTaskCompletingSeconds = 60,
                // ContinueAction = ReStart
            };
            MainActionTask.Start(CreateTcpClientHandler);
        }
        public override void Stop()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{Key} {m} Try to Stop");

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                 "Try to Stop without Restart", ToString());

            _isNeedRestartAfterSocketClosed = false;
            MainActionTask?.Stop();
            TcpClientHandler?.Stop();

            base.Stop();

            // Thread.Sleep(TimeSpan.FromSeconds(5));
            // ConsoleSync.WriteReadLineT($"{m} Press to Start again");
            // Start();
        }
        public void SendMessage(string[] message)
        {
            TcpClientHandler?.SendMessage(message);
        }
        public void MessageReceived(object sender, string[] s)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}:{s.MessageToString()}");
        }

        private void SubscribeEvents()
        {
            TcpClientHandler.SocketClosed += OnSocketClosed;
        }

        private void OnSocketClosed(object s, string msg)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m} Event Received");
            if (_isNeedRestartAfterSocketClosed)
            {
                ConsoleSync.WriteLineT($"{Key} {m} Socket Closed. Try to Restart");
                // Thread.Sleep(TimeSpan.FromSeconds(5));
                Start();
            }
            else
                ConsoleSync.WriteLineT($"{Key} {m} Socket Closed. Try to Finish");

            if (TcpClientHandler != null)
                TcpClientHandler.SocketClosed -= OnSocketClosed;
        }

        private void ReStart(CancellationToken token)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (_isNeedRestartAfterSocketClosed)
            {
                ConsoleSync.WriteLineT($"{Key} {m} Socket Closed. Try to Restart");
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                m, "Socket Closed. Try to Restart with Start()", ToString());
                // Thread.Sleep(TimeSpan.FromSeconds(5));
                Start();
            }
            else
                ConsoleSync.WriteLineT($"{Key} {m} Socket Closed. Try to Finish");

            if (TcpClientHandler != null)
                TcpClientHandler.SocketClosed -= OnSocketClosed;
        }
    }
}
