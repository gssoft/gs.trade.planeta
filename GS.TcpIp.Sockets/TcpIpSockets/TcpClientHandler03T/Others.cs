using System;
using System.Reflection;
using GS.Elements;
using GS.Interfaces;

namespace TcpIpSockets.TcpClientHandler03T
{
    public partial class TcpClientHandler<TMessage>
    {
        public void SubscribeTopic()
        {
            var m = MethodBase.GetCurrentMethod().Name + "()";
            try
            {
                var rand = new Random(DateTime.Now.TimeOfDay.Milliseconds);
                var i = rand.Next(0, DdeTopics.Length);
                var topic = DdeTopics[i];
                SendMessage(new[] {"SUBSCRIBE", topic});
                Evlm2(EvlResult.INFO, EvlSubject.TESTING, ParentTypeName, TypeName,
                    $"{m}", $"Try to Subscribe Topic: {topic}", ToString());
            }
            catch (Exception e)
            {
                SendException(e);
            }
        }
    }
}
