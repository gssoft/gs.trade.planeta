using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.Interfaces;


namespace GS.Trade.QuoteDownLoader.Works
{
    public class WorkContainer
    {
        public List<IWorkItem> Works;

        public IEventLog FileEventLog;
        public IEventLog ConsoleEventLog;

        public WorkContainer()
        {
            Works = new List<IWorkItem>();
        }
        public void AddWork(IWorkItem work)
        {
            if (work == null) return;
            work.Works = this;
            Works.Add(work);
        }
        public void ClearWorks()
        {
            Works.Clear();
        }

        public void DoWorks()
        {
            foreach (var w in Works.Where(w=>w.IsEnabled))
            {
                EvlMessage(EvlResult.INFO, "Start WorkItem:", w.Name );
                var dt = DateTime.Now;
                w.DoWork();
                var tsp = DateTime.Now - dt;
                EvlMessage(EvlResult.INFO, "Finish WorkItem:", "Elapsed Time: " + tsp);
                EvlMessage(EvlResult.INFO, "----------------", "------------------------------");
            }
        }
        public void DoWorks(string fileName)
        {
            if (!File.Exists(fileName))
            {
                EvlMessage(EvlResult.FATAL, String.Format("File {0} is not Exist", fileName), "");
                return;
            }
            Works.Clear();
            DeSerializeWorks(fileName);
            DoWorks();
        }
        public void EvlMessage(EvlResult result, string operation, string message)
        {
            if (ConsoleEventLog != null)
                ConsoleEventLog.AddItem(result, operation, message);

            if (FileEventLog != null)
                FileEventLog.AddItem(result, operation, message);
        }
        public static StringWriter Serialize(IWorkItem w)
        {
            var sxml = new StringWriter();
            var sr = new XmlSerializer(w.GetType());
            sr.Serialize(sxml, w);
            sxml.Close();
            return sxml;
        }
        public void SerializeWorks(string xmlFileName)
        {
            using (var f = new StreamWriter(xmlFileName))
            {
                f.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                f.WriteLine(
                    "<Works xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">");

                foreach (var iw in Works)
                {
                    f.WriteLine(Serialize(iw));
                }
                f.WriteLine(@"</Works>");
            }
            //f.Close();
        }
        private void DeSerializeWorks(string xmlFileName)
        {
            var xDoc = XDocument.Load(xmlFileName);
            var ss = xDoc.Descendants("Works").FirstOrDefault();
            var ee = ss.Elements();
            //var xx = ss.Descendants();
            foreach (var xElement in ee)
            {
                //var iw = WorkItemFactory2(xElement);
                var iw = DeSerialize(xElement);
                if (iw != null) AddWork(iw);
            }
        }
        private IWorkItem WorkItemFactory(XElement xe)
        {
            var sr = new StringReader(xe.ToString());
            try
            {
                switch (xe.Name.ToString())
                {
                    case "FtpWork":
                        var xds = new XmlSerializer(typeof (FtpWork)); // DeSerializer
                        return (FtpWork) xds.Deserialize(sr);
                    case "ZipWork":
                        xds = new XmlSerializer(typeof (ZipWork)); // DeSerializer
                        return (ZipWork) xds.Deserialize(sr);
                    case "LogWork":
                        xds = new XmlSerializer(typeof(LogWork)); // DeSerializer
                        return (LogWork)xds.Deserialize(sr);
                    case "TickToDb":
                        xds = new XmlSerializer(typeof(TickToDb)); // DeSerializer
                        return (TickToDb)xds.Deserialize(sr);
                    case "TickToBar":
                        //xds = new XmlSerializer(typeof(TickToBar)); // DeSerializer
                        //return (TickToBar)xds.Deserialize(sr);
                        return DeSerialize<TickToBar>(xe, s => EvlMessage(EvlResult.FATAL, "DeSerialize: " + xe.ToString(), s));
                    case "RtsTickerGo":
                      //  xds = new XmlSerializer(typeof(RtsTickerGo)); // DeSerializer
                      //  return (RtsTickerGo)xds.Deserialize(sr);
                        return DeSerialize<RtsTickerGo>(xe, s => EvlMessage(EvlResult.FATAL, "DeSerialize: " + xe.ToString(), s));
                    case "RtsTickerNew01":
                        return DeSerialize<RtsTickerNew01>(xe, s => EvlMessage(EvlResult.FATAL, "DeSerialize: " + xe.ToString(), s));
                }    
            }
            catch(Exception e)
            {
                EvlMessage(EvlResult.FATAL,  "DeSerialization:" + xe, e.Message);
            }
            return null;
        }
        private IWorkItem WorkItemFactory2(XElement xe)
        {
            var typeName = "GS.Trade.QuoteDownLoader.Works." + xe.Name.ToString().Trim();
            var t = Type.GetType(typeName, false,true);
            if (t == null) return null;
            return null;
        }

        public static T DeSerialize<T>(XElement xe, Action<string> errMessage)
        {
            var x = default(T);
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(typeof(T)); // DeSerializer
                    x = (T)xds.Deserialize(sr);
                }
            }
            catch (Exception e)
            {
                //Works.EvlMessage(EvlResult.FATAL, "Deserialize: " + sr, e.Message);
                if (errMessage != null) errMessage(e.Message);
            }
            return x;
        }
        public  IWorkItem DeSerialize(XElement xe)
        {
            if (xe == null) return null;
            var typeName = "GS.Trade.QuoteDownLoader.Works." + xe.Name.ToString().Trim();
            object x = null;
            var t = Type.GetType(typeName, false, true);
            if (t == null)
            {
                EvlMessage(EvlResult.FATAL, "DeSerialize.", typeName + " Is Not Found");
                return null;
            }
            try
            {
                using (var sr = new StringReader(xe.ToString()))
                {
                    var xds = new XmlSerializer(t); // DeSerializer
                    x = xds.Deserialize(sr) as IWorkItem;
                    if( x == null)
                        EvlMessage(EvlResult.FATAL, "DeSerialize.", t.FullName + " Is Not Support IWorkItem");
                }
            }
            catch (Exception e)
            {
                EvlMessage(EvlResult.FATAL, "DeSerialize: " + xe, e.Message);
            }
            
            return x as IWorkItem;
        }

        // private Action<string> err = (s) => EvlMessage(EvlResult.FATAL, "DeSerialize: ", s);
     
    } 
}
