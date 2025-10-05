using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GS.EventLog;
using GS.Interfaces;
using GS.Serialization;
using GS.Trade.DataBase.Model;

namespace GS.Trade.Web.Clients
{
    public class WebClients
    {
        private IEventLog _eventLog;
        public  List<WebClient> WebClientList;
        
        public WebClients()
        {
            WebClientList = new List<WebClient>();
        }

        public void Init(string uri)
        {
            //_eventLog = new EventLogs();
            //_eventLog.Init(@"Init\EventLogs.xml");

            _eventLog = Builder.Build<EventLogs>(@"Init\EventLog.xml", "EventLogs");
            _eventLog.Init();

            DeSerializationCollection(uri);
            foreach (var w in WebClientList)
            {
                w.Init(this);
            }
           

        }

        public WebClient this[string strKey]
        {
            get
            {
                return WebClientList.FirstOrDefault(w => String.Equals(w.KeyStr.Trim(), strKey.Trim(), StringComparison.CurrentCultureIgnoreCase));
            }
       }

        private bool DeSerializationCollection(string uri)
        {
            try
            {
                var xDoc = XDocument.Load(uri);
                var ss = xDoc.Descendants("WebClientList").FirstOrDefault();
                var xx = ss.Elements();
                foreach (var x in xx)
                {
                    var typeName = this.GetType().Namespace + '.' + x.Name.ToString().Trim();
                    var t = Type.GetType(typeName, false, true);
                    var s = Serialization.Do.DeSerialize(t, x, null);

                    if (s != null) WebClientList.Add((WebClient)s);
                }
                //  if (_evl != null)
                //      _evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Startegies", "Finish DeSerialization", "Startegies Count = " + Count, "");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
                //  if (_evl == null) throw new SerializationException("Serialization error");
                //  _evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Startegies", "DeSerialization", "Failure", "");

            }
            return true;
        }

        public void Evlm(EvlResult result, EvlSubject subject,
                                   string source, string entity, string operation, string description, string obj)
        {
             try
            {
            if (_eventLog != null)
                _eventLog.AddItem(result, subject, source, entity, operation, description, obj);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
