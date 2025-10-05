using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Tasks;
using TcpIpSockets.Extensions;
using TcpIpSockets.TcpClientHandler01;
using TcpClientHandler = TcpIpSockets.TcpClientHandler01.TcpClientHandler;

namespace TcpIpSockets
{
    public class TcpClient02 : Element1<string>
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
        public TcpClient02()
        {
            Port = PortPar;
            IpAddress = IpAddressPar;
            TryToConnectTimeInterval = TryToConnectTimeIntervalPar;         
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //ChangedEvent += (sender, args) =>
            //{
            //    ConsoleSync.WriteLineT($"********** {m} {args.Key} {args.Operation} {args.Object.ToString()}");
            //};
        }
        public void CreateTcpClientHandler(CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var userFullName = Guid.NewGuid().ToString();
            var userName = userFullName.Left(8);
            ConsoleSync.WriteLineT($"{Key} {m} Try Connect to {IpAddress} {Port}");
            while (true)
            {
                try
                {
                    if (canceltoken.IsCancellationRequested)
                        canceltoken.ThrowIfCancellationRequested();

                    TcpClient = new TcpClient(IpAddress, Port);
                    ConsoleSync.WriteLineT($"{Key} {m} Connected Ok to {IpAddress} {Port}");

                    TcpClientHandler = new TcpClientHandler
                    {
                        Parent = this,
                        ClientSocket = TcpClient,
                        // NetworkStream = TcpClient.GetStream(),
                        ClientFullName = Guid.NewGuid().ToString(),
                        IsPingEnabled = true,
                        TimeIntervalForTaskCompleting = 15
                    };
                    TcpClientHandler.SocketClosed += OnSocketClosed;
                    TcpClientHandler.Init();
                    TcpClientHandler.Start();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    break;
                }
                catch (SocketException e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    TcpClient?.Close();
                }
                catch (OperationCanceledException e)
                {
                    e.PrintExceptions(this, m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    TcpClient?.Close();
                    break;
                }
                catch (Exception e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
//                    ConsoleSync.WriteLineT($"{Key} {m} Press Key to Exit");
                    break;
                }
                Thread.Sleep(TimeSpan.FromSeconds(TryToConnectTimeInterval));
            }
            ConsoleSync.WriteLineT($"{m} Finished");
        }
        public void Start()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{Key} {m} Try to Start");
            _isNeedRestartAfterSocketClosed = true;
            MainActionTask = new TaskBase02
            {
                ClientName = Code,
                TimeIntervalForMainTaskCompletingSeconds = 60,
                // ContinueAction = ReStart
            };
            MainActionTask.Start(CreateTcpClientHandler);
        }
        public void Stop()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{Key} {m} Try to Stop");
            _isNeedRestartAfterSocketClosed = false;
            MainActionTask?.Stop();
            TcpClientHandler?.Stop();

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
