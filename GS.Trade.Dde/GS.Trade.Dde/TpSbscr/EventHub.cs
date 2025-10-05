using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.EventHubs.EventHubPrTskT1;
using GS.Interfaces;
using GS.Interfaces.Dde;
using GS.Serialization;

namespace GS.Trade.Dde.TpSbscr
{
    // using EventHub = GS.EventHubs.EventHubPrTskT1.EventHub<List<string>>; 
    public partial class Dde
    {
        [XmlIgnore]
        public EventHub<List<string>> EventHubLstStr { get; private set; }

        [XmlIgnore]
        public EventHub<string> EventHubStr { get; private set; }

        private void InitEventHub()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (Mode == ChangesSendMode.Table || Mode == ChangesSendMode.Mixed)
                {
                    EventHubLstStr = Builder.Build<EventHub<List<string>>>(@"Init\EventHubT1_ItemPrTsk04.xml",
                        "EventHubOfListOfString");
                    EventHubLstStr.Init();
                }
                else if (Mode == ChangesSendMode.Line)
                {
                    EventHubStr = Builder.Build<EventHub<string>>(@"Init\EventHubT1_ItemPrTsk01.xml",
                        "EventHubOfListOfString");
                    EventHubStr.Init();
                }
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
        public void Subscribe(string key, EventHandler<List<string>> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                EventHubLstStr.Subscribe(key, callback);
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
        public void Subscribe(string key, EventHandler<string> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                EventHubStr.Subscribe(key, callback);
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
        public void UnSubscribe(string key, EventHandler<List<string>> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                EventHubLstStr.UnSubscribe(key, callback);
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
        public void UnSubscribe(string key, EventHandler<string> callback)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                EventHubStr.UnSubscribe(key, callback);
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
        private void StartEventHub()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (Mode == ChangesSendMode.Table || Mode == ChangesSendMode.Mixed)
                    EventHubLstStr.Start();
                else
                    EventHubStr.Start();
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
        private void StopEventHub()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (Mode == ChangesSendMode.Table || Mode == ChangesSendMode.Mixed)
                    EventHubLstStr.Stop();
                else
                    EventHubStr.Stop();
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.INIT, e.GetType().Name, $"{TypeName}.{m}", e.Message, ToString());
            }
        }
    }
}
