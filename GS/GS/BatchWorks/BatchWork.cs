using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using GS.BatchWorks;
using GS.Elements;
using GS.Interfaces;
using GS.Serialization;

namespace GS.BatchWorks
{
    public class BatchWork : Element1<string>
    {
        public List<IBatchWorkItem> Works;

        public BatchWork()
        {
            Works = new List<IBatchWorkItem>();
        }

        public void Init(string batchWorksFileName)
        {
            Evlm2(EvlResult.INFO, EvlSubject.INIT, FullCode, Code, "Init() Start", "", "");
            Works.Clear();
            var ws = Builder.DeSerializeCollection<IBatchWorkItem>(batchWorksFileName, "Works");
            foreach(var w in ws)
                AddWork(w);
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, FullCode, Code, "Init() Completed","","" );
            //DeSerializeWorks(batchWorksFileName);
        }

        public void AddWork(IBatchWorkItem work)
        {
            if (work == null)
                return;
            // work.Works = this;
            work.Parent = this;
            Works.Add(work);
            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, FullCode, work.FullCode, "AddWork()", "", work.ToString());
        }
        public void ClearWorks()
        {
            Works.Clear();
        }

        public void DoWorks()
        {
            Evlm2(EvlResult.INFO, EvlSubject.INIT, FullCode, Code, "DoWork() Start", "", "");
            foreach (var w in Works.Where(w=>w.IsEnabled))
            {
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, w.FullCode, w.Code, "Start", "", "");
                var dt = DateTime.Now;
                w.DoWork();
                var tsp = DateTime.Now - dt;
                Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, w.FullCode, w.Code, " Finish", "Elapsed Time: " + tsp, "");
                // EvlMessage(EvlResult.INFO, "Finish WorkItem:", "Elapsed Time: " + tsp);
                // EvlMessage(EvlResult.INFO, "----------------", "------------------------------");
            }
            Evlm2(EvlResult.INFO, EvlSubject.INIT, FullCode, Code, "DoWork() Complete", "", "");
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
            Evlm2(result,EvlSubject.TECHNOLOGY, FullCode, Code, operation, message,"");
        }
        public static StringWriter Serialize(IBatchWorkItem w)
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
        private IBatchWorkItem WorkItemFactory2(XElement xe)
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
        public IBatchWorkItem DeSerialize(XElement xe)
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
                    x = xds.Deserialize(sr) as IBatchWorkItem;
                    if( x == null)
                        EvlMessage(EvlResult.FATAL, "DeSerialize.", t.FullName + " Is Not Support IWorkItem");
                }
            }
            catch (Exception e)
            {
                EvlMessage(EvlResult.FATAL, "DeSerialize: " + xe, e.Message);
            }

            return x as IBatchWorkItem;
        }

        // private Action<string> err = (s) => EvlMessage(EvlResult.FATAL, "DeSerialize: ", s);

        public override string Key
        {
            get { return Code; }
        }
    } 
}
