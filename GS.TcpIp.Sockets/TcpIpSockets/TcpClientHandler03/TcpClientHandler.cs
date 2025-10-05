using System;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using GS;
using GS.ConsoleAS;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Serialization;
using GS.Tasks;
using TcpIpSockets.Extensions;

namespace TcpIpSockets.TcpClientHandler03
{    
    public partial class TcpClientHandler: ElementPrTsk01<string[]>, ITcpClientHandler
    {
        // public const int PingTimeIntervalSecondsDefault = 5;

        private const int PortPar = 8082;
        private const string IpAddressPar = "127.0.0.1";
        private const int ConnectionTimeOut = 15000;
        private const int TryToConnectTimeIntervalPar = 15;

        public string IpAddress { get; private set; }
        public int Port { get; private set; }

        public ITcpServer TcpServer;
        public TcpClient ClientSocket { get; set; }
        //public NetworkStream NetworkStream => ClientSocket?.GetStream();
        public NetworkStream NetworkStream;
        public string ServerFullName { get; set; }
        public string ServerName => ServerFullName.Left(8);
        public string ClientFullName { get; set; }
        public string ClientName => ClientFullName.Left(8);
        // public string RealClientName => IsTcpServer ? ServerName : ClientName;
        //public string ConnectionFullName { get; set; }
        //public string ConnectionName => ConnectionFullName.Left(8);
        public int ProcTaskTimeInterval { get; set; }
        public ConnectionEnum ConnectionStatus { get; set; }
        protected StopRequestEnum StopRequest;
        public bool IsCloseRequest { get; private set; }
        public bool IsTcpServer => TcpServer != null;
        public bool IsPingEnabled { get; set; }

        public int TimeIntervalForTaskCompleting { get; set; }

        private string TaskStatus => Task != null
            ? $"{Task.Status} Complete:{Task.IsCompleted} Fault:{Task.IsFaulted} Cancel:{Task.IsCanceled}"
            : "Taks is null;";
        public bool IsClientSocketConnected => ClientSocket != null && ClientSocket.Connected;

        protected Task Task;
        protected TaskBase02 ConnectActionTask;
        protected TaskBase02 MainActionTask;
        protected Task WaitToCompleteTask;

        public MessageStats MessageStats;
        public int PingTimeIntervalSeconds { get; set; }

        public string[] DdeTopics =
        {
            "QuikDdeServer.Quotes",
            "QuikDdeServer.TickerInfo",
            "QuikDdeServer.OptionDesk",
            //"QuikDde.OptionDesk",
            //"QuikDde.MarketQuotes",
            //"QuikDde.SymbolGo"
        };

        public TcpClientHandler()
        {
        }
        public TcpClientHandler(string ipAddress, int port)
        {
            IpAddress = ipAddress;
            Port = port;
        }
        public void TcpConnectionProcess(CancellationToken canceltoken)
        {
            var m = MethodName.Is();
            var userFullName = Guid.NewGuid().ToString();
            var userName = userFullName.Left(8);
            ConsoleSync.WriteLineT($"{Key} {m} Try Connect to {IpAddress} {Port}");

            ClientFullName = Guid.NewGuid().ToString();

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                             $"{Key} Try Connect to {IpAddress} {Port}", ToString());
            while (true)
            {
                try
                {
                    if (canceltoken.IsCancellationRequested)
                        canceltoken.ThrowIfCancellationRequested();

                    ClientSocket = new TcpClient(IpAddress, Port);
                    NetworkStream = ClientSocket.GetStream();

                    ConsoleSync.WriteLineT($"{Key} {m} Connected Ok to {IpAddress} {Port}");
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                             $"{Key} Connected Ok to {IpAddress} {Port}", ToString());

                    StartTcpClientHandler();

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    break;
                }
                catch (SocketException e)
                {
                    e.PrintExceptions(m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, $"Can't Connect to {IpAddress} {Port}", e.Message);
                    ClientSocket?.Close();
                }
                catch (OperationCanceledException e)
                {
                    e.PrintExceptions(this, m);
                    ConsoleSync.WriteLineT($"{Key} {m} Can't Connect to {IpAddress} {Port}");
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, $"Can't Connect to {IpAddress} {Port}", e.Message);
                    ClientSocket?.Close();
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
                Thread.Sleep(TimeSpan.FromSeconds(TryToConnectTimeIntervalPar));
            }
            ConsoleSync.WriteLineT($"{m} Finished");
            if(IsClientSocketConnected)
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                $"Connected Ok. Connection:{ClientSocket?.Connected}", ToString());
            else
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                    $"Connecting Failure. Connection:{ClientSocket?.Connected}", ToString());
        }
        public override void Init()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            base.Init();

            // ClientFullName = Guid.NewGuid().ToString();
            // IsPingEnabled = true;
            TimeIntervalForTaskCompleting = 15;

            if (TimeIntervalForTaskCompleting <= 0) TimeIntervalForTaskCompleting = 15;

            IsProcessTaskInUse = true;
            SetupProcessTask();
            //if (PingTimeIntervalSeconds <= 0)
            //    PingTimeIntervalSeconds = PingTimeIntervalSecondsDefault;
            MessageStats = new MessageStats
            {
                PingTimeIntervalSeconds = PingTimeIntervalSeconds
            };
            MessageStats.Init();

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName, m,
                            $"{Key} ", ToString());
            Thread.Sleep(1000);
        }
        public override void SetupProcessTask()
        {
            if (!IsProcessTaskInUse)
            {
                //Evlm2(EvlResult.WARNING, EvlSubject.INIT, ParentTypeName, TypeName,
                //MethodBase.GetCurrentMethod().Name, "ProcessTask Will NOT BE USED",
                //ToString());
                return;
            }
            ProcessTask = new ProcessTask<string[]>();
            if(EventLog != null) ProcessTask.Init(EventLog);
            ProcessTask.Parent = this;
            ProcessTask.TimeInterval = 5000;
            ProcessTask.IdlingCycleAction = IsPingEnabled ? SendPingProcess : (Action) null;
            // ProcessTask.IsEveryItemPushProcessing = false;
            ProcessTask.IsEveryItemPushProcessing = true;
            ProcessTask.ItemProcessingAction = str =>
            {
                try
                {
                    WriteStringToClient(str);
                }
                catch (Exception e)
                {
                    SendException(e);
                    //Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                    //    GetType().Name, "ProcessTask", "Exception", e.Message);
                }
            };
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod().Name, "ProcessTask IS USED NOW",
                ProcessTask?.ToString());
        }
        public void Start()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                ConsoleSync.WriteLineT($"{Key} {m} Try to Start");
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                    "Try to Start", ToString());

                if (IsTcpServer)
                {
                    StartTcpClientHandler();
                    return;
                }

                ConnectionStatus = ConnectionEnum.Unknown;
                StopRequest = StopRequestEnum.Unknown;

                ClientFullName = Guid.NewGuid().ToString();

                Init();
                ConnectActionTask = new TaskBase02
                {
                    Parent = this,
                    ClientName = Code,
                    TimeIntervalForMainTaskCompletingSeconds = 60,
                };
                ConnectActionTask.Start(TcpConnectionProcess);
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                        TypeName, e.GetType().ToString(), m, $"{IpAddress} {Port}", e.Message);
                ClientSocket?.Close();
            }       
        }
        private void StartTcpClientHandler()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                ConsoleSync.WriteLineT($"{TcpClientKey} {m} Try to Start ...");
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                                 $"{Key} Try to Start", ToString());

                // ProcessTask?.Start();

                MainActionTask = new TaskBase02
                {
                    Parent = this,
                    ClientName = TcpClientKey,
                    TimeIntervalForMainTaskCompletingSeconds = 60
                };
                MainActionTask.Start(TcpNetworkStreamReadProcess);
                // Thread.Sleep(2000);

                // ProcessTask?.Start();

            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                        TypeName, e.GetType().ToString(), m, $"{IpAddress} {Port}", e.Message);
                ClientSocket?.Close();
            }
        }
        public void Stop()
        {
            StopRequest = StopRequestEnum.Local;
            Close();
        }
        public void Close()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";

           // IsEnabled = false;

            if (ConnectionStatus == ConnectionEnum.CloseRequestAccepted)
            {
                // ConsoleSync.WriteLineT($"{TcpClientKey} {m} SocketConnected:{ClientSocket.Connected} ConnStatus:{ConnectionStatus} Already in Use");
                return;
            }
            try
            {  
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                                 $"{Key} Try to Stop", ToString());

                ConsoleSync.WriteLineT($"{TcpClientKey} {m} Try to Stop ...");

                ConnectionStatus = ConnectionEnum.CloseRequestAccepted;

                SendMessage(m,"Close", "", "");

                // Thread.Sleep(1000);

                // ClientSocket?.Close();

                // ProcessTask?.Stop();
                ConnectActionTask?.Stop();
                MainActionTask?.Stop();

                Thread.Sleep(1000);
                
                // ClientSocket?.Close();

                ProcessTask?.Stop();

                ClientSocket?.Close();

                OnSocketClosed(TcpClientKey);

                var ea = new GS.Events.EventArgs
                {
                    Sender = this,
                    Category = "TcpClientHandler",
                    Entity = "Socket",
                    Operation = "Closed",
                    Object = "SocketClosed"
                    //IsHighPriority = true,
                };
                OnChangedEvent(ea);
                Thread.Sleep(TimeSpan.FromSeconds(1));

                if(!IsTcpServer && StopRequest != StopRequestEnum.Local)
                    Start();
            }
            catch (ObjectDisposedException e)
            {
                e.PrintExceptions(this, m);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, TypeName, e.GetType().ToString(), 
                        m, $"Close Failure {IpAddress} {Port}", e.Message);
                ClientSocket?.Close();
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                        TypeName, e.GetType().ToString(), m, $"Close Failure {IpAddress} {Port}", e.Message);

                ClientSocket?.Close();
            }
        }
        private void ClearTcpHandler()
        {
            ServerFullName = "";
            ClientSocket = null;
        }

        private bool _working;
        public void SetReadWorkingStatus(bool working)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            _working = working;
            ConsoleSync.WriteLineT($"{m}: Try to Silent Closing ...");
        }
        private void TcpNetworkStreamReadProcess(CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                             "Start", ToString());

            ProcessTask?.Start();

            _working = true;
            while (_working)
            {
                try
                {
                    if (canceltoken.IsCancellationRequested)
                        canceltoken.ThrowIfCancellationRequested();

                    if (!IsClientSocketConnected)
                    {
                        Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentName,
                         TypeName,  m, "ClientSocket is not Connected", Key);
                        // ReStart();
                        break;
                    }

                    var dtoBytes = NetworkStream.ReadToBytes(ClientSocket.ReceiveBufferSize,
                        Console.WriteLine, e => Console.WriteLine(e.ToString()));

                    // Console.WriteLine($"BytesCnt:{dtoBytes.Count()}");

                    if (dtoBytes == null)
                    {
                        Console.WriteLine($"Bytes Received is null");
                        if (canceltoken.IsCancellationRequested)
                            canceltoken.ThrowIfCancellationRequested();
                        continue;
                    }
                    if (dtoBytes.Length <= 0)
                    {
                        Console.WriteLine($"Bytes Received is empty");
                        if (canceltoken.IsCancellationRequested)
                            canceltoken.ThrowIfCancellationRequested();
                        continue;
                    }
                    var dto = BinarySerialization.DeSerialize<string[]>(dtoBytes);
                    // Console.WriteLine($"{ClientName}:{dto[0]},{dto[1]},{dto[2]}");
                    if (IsTcpServer)
                        TcpServerMessageRecievedProcessing(dto);
                    else
                        TcpClientMessageRecievedProcessing(dto);

                    //if (canceltoken.IsCancellationRequested)
                    //    canceltoken.ThrowIfCancellationRequested();
                }
                // Thraw in Close() when StopRequest == Local ClientSocket.CLose() HERE
                catch (SocketException e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, e.Message, Key);
                    break;
                }
                // RemoteClient break Connection 
                catch (System.IO.IOException e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, e.Message, Key);
                    break;
                }
                catch (OperationCanceledException e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, e.Message, Key);
                    break;
                }
                catch (ObjectDisposedException e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, e.Message, Key);
                    break;
                }
                catch (Exception e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         TypeName, e.GetType().ToString(), m, e.Message, Key);
                    break;
                }
            }
            Close();
            ConsoleSync.WriteLineT($"{Key} {m} Finish");
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                             $"{Key} NetworkStream.Read Process Finish.", ToString());
        }
        
        public void ClearSocket()
        {
            var m = MethodName.Is();
            ClientSocket.Close();
            OnSocketClosed(TcpClientKey);
        }
        private void WaitToCompleteTaskAction(int secondsToWait)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m} {Key} Try to Cancel Task ... ");
            //while (!Task.IsCompleted && secondsToWait-- > 0)
            //
            //while (secondsToWait-- > 0)
            // while(true)
            int i = 0;
            while (!Task.IsCompleted && secondsToWait-- > 0)
            {
            ConsoleSync.WriteLineT($"{m} {Key} Status:{TaskStatus} {ConnectionStatus} Seconds:{++i}");
            Thread.Sleep(1000);
            }
            // ConsoleSync.WriteLineT($"{m}: Status:{TaskStatus}");
            Task.Dispose();
            ConnectionStatus = ConnectionEnum.NotConnected;
            ConsoleSync.WriteLineT($"{m} {Key} TaskStatus:{TaskStatus}");
            ConsoleSync.WriteLineT($"{m} {Key} SocketConnection:{ClientSocket.Connected} ConnectionStatus:{ConnectionStatus}");
        }
        private Task WaitToCompleteActionAsync(int seconds)
        {
            return Task.Factory.StartNew(p=>WaitToCompleteTaskAction(seconds),seconds); //.WithTimeout(tm);
        }
        private void Register(TcpClientHandler tcpClientHandler)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (ServerFullName.HasValue() && ClientFullName.HasValue())
            {
                TcpServer?.Register(this);
                Console.WriteLine($"{m} Ok, Key:{TcpClientKey}");
            }
            else
                Console.WriteLine($"{m} Failure, Key:{TcpClientKey}");

            // throw new NotImplementedException();
        }
        // public string TcpClientKey => $"{{{ServerName}}}.{{{ClientName}}}";
        public string TcpClientKey => $"[{ServerName}].[{ClientName}]";
        public override string ToString()
        {
            return $"{TcpClientKey}";
        }
        public override string Key => TcpClientKey;
    }
}
