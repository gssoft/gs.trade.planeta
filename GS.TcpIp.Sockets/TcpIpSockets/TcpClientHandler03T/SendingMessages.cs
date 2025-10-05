using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Reflection;
using GS.ConsoleAS;
using GS.EventHubs;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Serialization;
using TcpIpSockets.Extensions;

namespace TcpIpSockets.TcpClientHandler03T
{
    public partial class TcpClientHandler<TMessage>
    {      
        public void SendMessage(object sender, TMessage message)
        {
            ProcessTask.EnQueue(message);
        }
        public void SendMessage(TMessage message)
        {
            ProcessTask.EnQueue(message);
        }
        public bool SendMessage(params string[] strarr)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                if (typeof (TMessage) == typeof (string[]))
                    ProcessTask.EnQueue(strarr);
                else
                {
                    var lst = new List<string>();
                    lst.AddRange(strarr);
                    ProcessTask.EnQueue(lst);
                }
                var s = strarr.MessageToString();
                ConsoleSync.WriteLineT(IsTcpServer
                    ? $"{OutcomeServerToClientStr} {s}"
                    : $"{OutcomeClientToServerStr} {s}");
                return true;
            }
            catch (Exception e)
            {
                e.PrintExceptions(this, m);
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                    ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, $"{strarr?.MessageToString()}");
                SendException(e);
            }
            return false;
        }
        //public bool SendMessage(string routeKey, string operation, string data)
        //{
        //    var m = MethodBase.GetCurrentMethod().Name + "()";
        //    //if (IsClientSocketConnected)
        //    //{
        //        try
        //        {
        //            var answer = new[] {routeKey, operation, data};
        //            EnQueue(answer);
        //            // MessageStats.LastInteractionDateTime = DateTime.Now;

        //            ConsoleSync.WriteLineT(IsTcpServer
        //                ? $"{OutcomeServerToClientStr} {routeKey}, {operation}, {data}"
        //                : $"{OutcomeClientToServerStr} {routeKey}, {operation}, {data}");
        //            return true;
        //        }
        //        catch (Exception e)
        //        {
        //            e.PrintExceptions(this, m);
        //            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
        //                ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, $"{routeKey} {operation} {data}");
        //        SendException(e);
        //        }
        //        return false;
        //    //}
        //    //ConsoleSync.WriteLineT($"{m}: Socket is not connected");
        //    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
        //                $"{m} {TcpClientKey}", $"Socket is not connected {ClientSocket?.Connected}",
        //                $"{routeKey} {operation} {data}");
        //    return false;          
        //}
        //public void SendMessage(object sender, IEventArgs args)
        //{
        //    var m = MethodBase.GetCurrentMethod().Name + "()";
        //    try
        //    {
        //        var msg = (string[])args.Object;
        //        SendMessage(msg);
        //    }
        //    catch (Exception e)
        //    {
        //        SendException(e);
        //        Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY,
        //                ParentAndMyTypeName, e.GetType().ToString(), m, e.Message, $"{args.Object}");
        //    }
        //}
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
        public void WriteStringToClient(List<string> lst)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            
            try
            {
                var dtoBytes = BinarySerialization.SerializeToByteArray(lst);
                NetworkStream.Write(dtoBytes);
                //NetworkStream.Write(dtoBytes, Console.WriteLine, e => Console.WriteLine((string)e.ToString()));
            }
            catch (Exception e)
            {
                // Console.WriteLine($"{e.Message}");
                SendException(e);
            }
        }
        public void WriteStringToClient(string[] strarr)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var dtoBytes = BinarySerialization.SerializeToByteArray(strarr);
                NetworkStream.Write(dtoBytes);
                //NetworkStream.Write(dtoBytes, Console.WriteLine, e => Console.WriteLine((string) e.ToString()));
            }
            catch (Exception e)
            {
                // Console.WriteLine($"{e.Message}");
                SendException(e);
            }
        }
        public void WriteStringToClient(TMessage strarr)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var dtoBytes = BinarySerialization.SerializeToByteArray(strarr);
                NetworkStream.Write(dtoBytes);

                MessageStats.IncOutcomeMessageCnt();
            }
            catch (Exception e)
            {
                //Console.WriteLine($"{e.Message}");
                SendException(e);
            }
        }
        public void WriteStringToClient(object msg)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var dtoBytes = BinarySerialization.SerializeToByteArray(msg);
                NetworkStream.Write(dtoBytes);

                MessageStats.IncOutcomeMessageCnt();
            }
            catch (Exception e)
            {
                SendException(e);
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
        public void Subscribed(TMessage dto)
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            Evlm2(dto[1] == "SUCCESS" ? EvlResult.SUCCESS : EvlResult.FATAL,
                EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                $"{m}", $"Subscribe {dto[1]} Topic: {dto[2]}", ToString());
        }
        
    }
}
