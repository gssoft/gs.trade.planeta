using System;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using GS.ConsoleAS;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using TcpIpSockets.Extensions;

namespace TcpIpSockets.TcpClientHandler03
{
    public partial class TcpClientHandler
    {
        public string OutcomeServerToClientStr => $"{{{ServerName}}} => {{{ClientName}}}";
        public string IncomeServerFromClientStr => $"{{{ServerName}}} <= {{{ClientName}}}";
        public string OutcomeClientToServerStr => $"{{{ClientName}}} => {{{ServerName}}}";
        public string IncomeClientFromServerStr => $"{{{ClientName}}} <= {{{ServerName}}}";

        private void TcpServerMessageRecievedProcessing(string[] dto)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //Console.WriteLine($"{IncomeServerFromClientStr} {dto.MessageToString()}");
            ConsoleSync.WriteLineT($"{IncomeServerFromClientStr} {dto.MessageToString()}");
            MessageStats.IncIncomeMessageCnt();
            switch (dto[0].TrimUpper())
            {
                // Client -> Server
                case "CLIENTNAME":
                case "USERNAME":
                    if (ClientFullName.HasValue())
                    {
                        // Register(this);
                        break;
                    }
                    ClientFullName = dto[2];
                    MainActionTask.ClientName = TcpClientKey;
                    if (dto[1].HasNoValue())
                        SendMessage("ServerName", ServerFullName, ClientFullName);
                    //  Register(this);
                    Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, $"{ParentTypeName}.{TypeName}", TcpClientKey,
                        $"{m}", $"Client to Server Message:{dto.MessageToString()}", ToString());                    
                    break;
                case "PING":
                    SendPong();
                    MessageStats.IncIncomePingCnt();
                    break;
                case "PONG":
                    MessageStats.IncIncomePongCnt();
                    break;
                case "CLOSE":
                    // StopRequest = StopRequestEnum.Remote;
                     Close();
                    break;
                case "SUBSCRIBE":
                    TcpServer.Subscribe(dto[1], SendMessage, this);
                    break;
                case "UNSUBSCRIBE":
                    TcpServer.UnSubscribe(SendMessage);
                    break;
            }
        }
        private void TcpClientMessageRecievedProcessing(string[] dto)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //Console.WriteLine($"{IncomeClientFromServerStr} {dto.MessageToString()}");
            ConsoleSync.WriteLineT($"{IncomeClientFromServerStr} {dto.MessageToString()}");
            MessageStats.IncIncomeMessageCnt();
            switch (dto[0].TrimUpper())
            {
                // Server -> Client
                case "SERVERNAME":
                    ServerFullName = dto[1];
                    MainActionTask.ClientName = TcpClientKey;
                    if (dto[2].HasNoValue())
                        SendMessage("ClientName", ServerFullName, ClientFullName);
                    else
                        IsEnabled = true;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, $"{ParentTypeName}.{TypeName}", TcpClientKey,
                        $"{m}", $"Server to Client Message:{dto.MessageToString()}",
                        $"Server:{ServerName} Client:{ClientName}");
                    SubscribeTopic();
                    break;
                case "PING":
                    SendPong();
                    MessageStats?.IncIncomePingCnt();
                    break;
                case "PONG":
                    MessageStats?.IncIncomePongCnt();
                    break;
                case "CLOSE":
                    // StopRequest = StopRequestEnum.Remote;
                     Close();
                    break;
                case "SUBSCRIBED":
                    Subscribed(dto);
                    break;
            }
        }
    }
}
