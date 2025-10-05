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
using GS.Interfaces;
using GS.Process;
using GS.Tasks;
using TcpIpSockets.TcpClientHandler01;
using TcpClientHandler = TcpIpSockets.TcpClientHandlerLst.TcpClientHandler;

namespace TcpIpSockets.TcpClientHandlerLst
{
    public class TcpClientHandlerStarter : Element1<string>
    {
        protected Task Task { get; set; }
        public ITcpServer TcpServer { get; set; }
        public TcpClient TcpClient { get; set; }

        public string[] DdeTopics = 
        {
            "QuikDdeServer.Quotes",
            "QuikDdeServer.TickerInfo",
            "QuikDdeServer.OptionDesk",
            //"QuikDde.OptionDesk",
            //"QuikDde.MarketQuotes",
            //"QuikDde.SymbolGo"
        };
        
        public void Start()
        {
            Task = Task.Factory.StartNew(arg=>TcpHandlerCreate(new object[] {TcpClient, TcpServer}),
                new object[]
                {
                    TcpClient, TcpServer
                });
        }
        private void TcpHandlerCreate(object[] arg)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            ConsoleSync.WriteLineT($"{m}: Try to Create TcpClientHandler");
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m, "Start", Key);
            try
            {
                var args = (object[]) arg;
                var tcpClient = (TcpClient) args[0];
                var tcpServer = (ITcpServer) args[1];
           
                var tcpClientHandler = new TcpClientHandler
                {
                    TcpServer = tcpServer,
                    Parent = tcpServer,
                    ClientSocket = tcpClient,
                    NetworkStream = tcpClient.GetStream(),
                    ServerFullName = tcpServer.ServerFullName,
                    IsPingEnabled = false
                };
                tcpClientHandler.Init();
                tcpClientHandler.SocketClosed += (sender, s) => tcpServer.UnRegisterTcpClient(s);

                // Subscribe(tcpClientHandler);

                tcpClientHandler.Start();
                Thread.Sleep(1000);

                //var i = TcpServer.GetClientNameAttemptsNumber;
                //while (!tcpClient.Connected && i-- > 0)
                //{
                //    Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                //    "Wait Socket Connection", Key);
                //    Thread.Sleep(1000);
                //}
                //Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName, m,
                //   $"Connection is {tcpClient.Connected} {tcpClientHandler.IsClientSocketConnected}", Key);

                var k = TcpServer.GetClientNameAttemptsNumber;
                ConsoleSync.WriteLineT($"{m} Try to Get ClientFullName");

                var rand = new Random(DateTime.Now.TimeOfDay.Milliseconds);
                var ms = rand.Next(500, 1500);
                Thread.Sleep(TimeSpan.FromMilliseconds(ms));
                while (tcpClientHandler.ClientFullName.HasNoValue() && k-- > 0)
                {
                    var i = 0;
                    Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentAndMyTypeName,
                        $"{tcpClientHandler.TcpClientKey}", m,$"Try to Get ClientFullName. Attemts:{++i}", Key);

                    tcpClientHandler
                        .SendMessage("ServerName",  tcpClientHandler.ServerFullName,
                                                    tcpClientHandler.ClientFullName);
                    var mseconds = rand.Next(500, 5000);
                    Thread.Sleep(TimeSpan.FromMilliseconds(ms));
                }
                //if (tcpClientHandler.ClientFullName.HasValue())
                //{
                //    tcpClientHandler
                //        .SendMessage("ServerName", tcpClientHandler.ServerFullName,
                //                                    tcpClientHandler.ClientFullName);
                //}
                if(tcpClientHandler.ClientFullName.HasNoValue())
                {
                    ConsoleSync.WriteLineT($"{m} Get ClientFullName Failure ...");
                    Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ParentAndMyTypeName, $"{tcpClientHandler.TcpClientKey}", m,
                    "Try to Get ClientFullName Failure", Key);
                    tcpClientHandler.Stop();
                    return;
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ParentAndMyTypeName, $"{tcpClientHandler.TcpClientKey}",
                    m, "Get ClientFullName Ok", Key);

                var client = TcpServer.Register(tcpClientHandler);
                if (client != null)
                    ConsoleSync.WriteLineT($"{tcpClientHandler.TcpClientKey} {m} TcpClientRegisered:{tcpClientHandler}");
                else
                {
                    ConsoleSync.WriteLineT(
                        $"{tcpClientHandler.TcpClientKey} {m} Close TcpClientHandler. Register Failure");
                    tcpClientHandler.Stop();
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ParentAndMyTypeName,
                    $"{tcpClientHandler.TcpClientKey}", m, "Finish Ok", tcpClientHandler.ToString());
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
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
        public Task GetClientFullNameActionAsync(TcpClientHandler tcpClientHandler)
        {
            return Task.Factory.StartNew(() =>
            {
                while (tcpClientHandler.ClientFullName.HasNoValue())
                {
                    ConsoleSync.WriteLineT($"Try to Get Client Full Name");
                    tcpClientHandler
                        .SendMessage("ServerName", tcpClientHandler.ServerFullName, "");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            });
        }
        // public Task GetClientFullNameActionWithTokenAsync(
        public Task GetClientFullNameActionAsync(
            TcpClientHandler tcpClientHandler, CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //return Task.Factory.StartNew(() => 
            //    GetClientFullNameAction(tcpClientHandler, canceltoken) ,canceltoken);
            var t = Task.Factory.StartNew(() => 
                  GetClientFullNameAction(tcpClientHandler, canceltoken) ,canceltoken);
            Task.Factory.StartNew(() =>
            {
                while (!t.IsCompleted && !t.IsCanceled && !t.IsFaulted)
                {
                    ConsoleSync.WriteLineT($"{m} Task:{t.Status} Waiting for Completing GetClientName ...");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
                ConsoleSync.WriteLineT($"{m} Task:{t.Status} GetClientName Completed ...");
            });
            return t;
        }
        private void GetClientFullNameAction(TcpClientHandler tcpClientHandler, CancellationToken canceltoken)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";

            while (tcpClientHandler.ClientFullName.HasNoValue())
            {
                ConsoleSync.WriteLineT($"{m} Try to Get Client Full Name");
                tcpClientHandler
                    .SendMessage("ServerName", tcpClientHandler.ServerFullName, "");
                Thread.Sleep(TimeSpan.FromSeconds(1));

                if (canceltoken.IsCancellationRequested)
                {
                    // canceltoken.ThrowIfCancellationRequested();
                    ConsoleSync.WriteLineT($"{m} Cancelation Token Received. Try to Complete Work ...");
                    break;
                }
            }
            ConsoleSync.WriteLineT(tcpClientHandler.ClientFullName.HasValue()
                ? $"{m} ClientFullName is {tcpClientHandler.TcpClientKey} Success"
                : $"{m} ClientFullName is Not Received");
            ConsoleSync.WriteLineT($"{m} Work is Completed");
        }

        private static void WorkAction(CancellationToken t)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            ConsoleSync.WriteLineT($"{m}");
            var _working = true;
            while (_working)
            {
                ConsoleSync.WriteLineT("Working ....");
                Thread.Sleep(1 * 1000);
                if (t.IsCancellationRequested)
                {
                    ConsoleSync.WriteLineT("Working Complete ...");
                    break;
                }
            }
            ConsoleSync.WriteLineT("Working Completed");
        }
        //private void Subscribe(TcpClientHandler tcpClient)
        //{
        //    var m = MethodBase.GetCurrentMethod().Name + "()";
        //    if (!TcpServer.IsSubscribeAvailable) return;
        //    try
        //    {
        //        foreach (var i in DdeTopics)
        //        {
        //            TcpServer?.Subscribe(i, tcpClient.SendMessage);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        SendException(e);
        //    }     
        //}
        public override string Key => Code.HasValue() ? Code : GetType().ToString();
    }
}
