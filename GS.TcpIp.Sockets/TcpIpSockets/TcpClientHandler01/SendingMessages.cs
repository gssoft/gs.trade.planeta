using System;
using System.Reflection;
using GS.ConsoleAS;

namespace TcpIpSockets.TcpClientHandler01
{
    public partial class TcpClientHandler
    {
        private string[] CreateMeassage(string route, string operation, string data)
        {
            return new[] { route, operation, data };
        }
        public void SendMessage(string[] message)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (IsClientSocketConnected)
            {
                EnQueue(message);
                // MessageStats.LastInteractionDateTime = DateTime.Now;
                return;
            }
            ConsoleSync.WriteLineT($"{m}: Socket is not connected");
        }
        public bool SendMessage(string routeKey, string operation, string data)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            if (IsClientSocketConnected)
            {
                var answer = new[] { routeKey, operation, data };
                EnQueue(answer);
                // MessageStats.LastInteractionDateTime = DateTime.Now;

                ConsoleSync.WriteLineT(IsTcpServer
                   ? $"{OutcomeServerToClientStr} {routeKey}, {operation}, {data}"
                   : $"{OutcomeClientToServerStr} {routeKey}, {operation}, {data}");
                return true;
            }
            ConsoleSync.WriteLineT($"{m}: Socket is not connected");
            return false;
        }
        public void SendPing()
        {
            // SendMessage("Ping", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"), "");
            var m = MethodBase.GetCurrentMethod().Name + "()";
            var mark = MessageStats.GetNextPingMarker();
            var result = SendMessage("Ping", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"), mark.ToString());
            if (result)
                MessageStats.IncOutcomePingCnt();
        }
        public void SendPong()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // var mark = MessageStats.GetNextPingMarker();
            var result = SendMessage("Pong", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"), "");
            if (result)
                MessageStats.IncOutcomePongCnt();
        }
        public void SendPingProcess()
        {
            // if (!IsEnabled) return;
            var m = MethodBase.GetCurrentMethod().Name + "()";
            
            if (!MessageStats.IsNeedToSendPing()) return;
            //var mark = MessageStats.GetNextPingMarker();
            //var result = SendMessage("Ping", DateTime.Now.TimeOfDay.ToString(@"hh\:mm\:ss\.fff"), mark.ToString());
            //if(result)
            //    MessageStats.IncOutcomePingCnt();

            SendPing();

            //ConsoleSync.WriteLineT($"{m} Stats {MessageStats.PingStatStr}");
            //ConsoleSync.WriteLineT($"{m} Stats {MessageStats.PongStatStr}");
            //ConsoleSync.WriteLineT($"{m} Stats {MessageStats.MessageStatStr}");
        }
    }
}
