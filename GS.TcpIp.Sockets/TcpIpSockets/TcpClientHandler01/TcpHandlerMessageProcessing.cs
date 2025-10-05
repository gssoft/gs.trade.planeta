using System.Reflection;
using GS.ConsoleAS;
using GS.Extension;
using TcpIpSockets.Extensions;

namespace TcpIpSockets.TcpClientHandler01
{
    public partial class TcpClientHandler
    {
        public string OutcomeServerToClientStr => $"{{{ServerName}}} => {{{ClientName}}}";
        public string IncomeServerFromClientStr => $"{{{ServerName}}} <= {{{ClientName}}}";
        public string OutcomeClientToServerStr => $"{{{ClientName}}} => {{{ServerName}}}";
        public string IncomeClientFromServerStr => $"{{{ClientName}}} <= {{{ServerName}}}";

        private void ServerMessageRecievedProcessing(string[] dto)
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
                    break;
                case "PING":
                    SendPong();
                    MessageStats.IncIncomePingCnt();
                    break;
                case "PONG":
                    MessageStats.IncIncomePongCnt();
                    break;
                case "CLOSE":
                     Stop();
                    break;
            }
        }
        private void ClientMessageRecievedProcessing(string[] dto)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //Console.WriteLine($"{IncomeClientFromServerStr} {dto.MessageToString()}");
            ConsoleSync.WriteLineT($"{IncomeClientFromServerStr} {dto.MessageToString()}");
            MessageStats.IncIncomeMessageCnt();
            switch (dto[0].TrimUpper())
            {
                // Server -> Client
                case "SERVERNAME":
                    //ServerFullName = dto[1];
                    //if (ServerFullName.HasValue() && ClientFullName.HasValue())
                    //{
                    //    IsEnabled = true;
                    //    break;
                    //}
                    ServerFullName = dto[1];
                    MainActionTask.ClientName = TcpClientKey;
                    if (dto[2].HasNoValue())
                        SendMessage("ClientName", ServerFullName, ClientFullName);
                    else
                        IsEnabled = true;
                    break;
                case "PING":
                    SendPong();
                    MessageStats.IncIncomePingCnt();
                    break;
                case "PONG":
                    MessageStats.IncIncomePongCnt();
                    break;
                case "CLOSE":
                     Stop();
                    break;
            }
        }
    }
}
