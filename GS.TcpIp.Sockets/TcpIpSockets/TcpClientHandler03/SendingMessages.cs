using System;
using System.Collections.Generic;
using System.Reflection;
using GS.ConsoleAS;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using TcpIpSockets.Extensions;

namespace TcpIpSockets.TcpClientHandler03
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
            ////if (IsClientSocketConnected)
            ////{
            try
            {
                EnQueue(message);
                // MessageStats.LastInteractionDateTime = DateTime.Now;
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                     ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, Key);
                SendException(e);
            }
            //}
            //ConsoleSync.WriteLineT($"{m}: Socket is not connected");
            // ReStart();
        }
        public bool SendMessage(string source, string routeKey, string operation, string data)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //if (IsClientSocketConnected)
            //{
                try
                {
                    var answer = new[] { routeKey, operation, data };
                    EnQueue(answer);
                    // MessageStats.LastInteractionDateTime = DateTime.Now;

                    ConsoleSync.WriteLineT(IsTcpServer
                       ? $"{OutcomeServerToClientStr} {routeKey}, {operation}, {data}"
                       : $"{OutcomeClientToServerStr} {routeKey}, {operation}, {data}");
                    return true;
                }
                catch (Exception e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                         ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, Key);
                    SendException(e);
                }
                return false;
            //}
            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, TypeName, source, m, "Socket is not connected", $"{routeKey} {operation} {data}");
            ConsoleSync.WriteLineT($"{source} {m}: Socket is not connected");
            return false;
        }
        public bool SendMessage(string routeKey, string operation, string data)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            //if (IsClientSocketConnected)
            //{
                try
                {
                    var answer = new[] {routeKey, operation, data};
                    EnQueue(answer);
                    // MessageStats.LastInteractionDateTime = DateTime.Now;

                    ConsoleSync.WriteLineT(IsTcpServer
                        ? $"{OutcomeServerToClientStr} {routeKey}, {operation}, {data}"
                        : $"{OutcomeClientToServerStr} {routeKey}, {operation}, {data}");
                    return true;
                }
                catch (Exception e)
                {
                    e.PrintExceptions(this, m);
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                        ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, $"{routeKey} {operation} {data}");
                SendException(e);
                }
                return false;
            //}
            //ConsoleSync.WriteLineT($"{m}: Socket is not connected");
            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                        $"{m} {TcpClientKey}", $"Socket is not connected {ClientSocket?.Connected}",
                        $"{routeKey} {operation} {data}");
            return false;          
        }
        public void SendMessage(object sender, IEventArgs args)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var msg = (string[])args.Object;
                SendMessage(msg);
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                        ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, $"{args.Object}");
            }
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
        public void WriteStringToClient(string str)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            // ConsoleSync.WriteLineT($"{m}: Try to Write To NetworkStream");
            // return;
            try
            {
                //var dto = new[] { "RouteKey", "Operation", $"{str}" };
                if (str.HasNoValue()) return;
                var dto = new[] { "RouteKey", "Operation", $"{str}" };
                Console.WriteLine($"User:{ClientName} Data:{dto[0]},{dto[1]},{dto[2]}");
                var dtoBytes = BinarySerialization.SerializeToByteArray(dto);
                NetworkStream.Write(dtoBytes, Console.WriteLine, e => Console.WriteLine(e.ToString()));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                // SendException(e);
            }
        }
        public void WriteStringToClient(string[] strarr)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var dtoBytes = BinarySerialization.SerializeToByteArray(strarr);
                NetworkStream.Write(dtoBytes, Console.WriteLine, e => Console.WriteLine(e.ToString()));

                MessageStats.IncOutcomeMessageCnt();
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e.Message}");
                // SendException(e);
            }
        }

        public void Subscribe(string topic)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    $"{m}", $"Try to Subscribe Topic:{topic}", ToString());

                var dto = new[] {"SUBSCRIBE", topic, ClientName };
                SendMessage(dto);
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    $"{m}", $"Subscribe Failure Topic:{topic}", ToString());
            }
        }

        public void Subscribed(string[] dto)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(dto[1] == "SUCCESS" ? EvlResult.SUCCESS : EvlResult.FATAL,
                EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                $"{m}", $"Subscribe {dto[1]} Topic: {dto[2]}", ToString());
        }
    }
}
