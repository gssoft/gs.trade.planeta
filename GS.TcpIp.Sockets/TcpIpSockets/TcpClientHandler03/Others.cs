using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace TcpIpSockets.TcpClientHandler03
{
    public partial class TcpClientHandler
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
