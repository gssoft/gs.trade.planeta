using System;
using System.Collections.Generic;
using System.IO;
using GS.Events;
using GS.Interfaces;

namespace GS.EventLog
{
    public class FileEventLog : Evl, IEventLog
    {
        public string FileName { get; set; }

        private string _fileName;

        public FileEventLog() 
        {
        }

        public FileEventLog(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
                _fileName = fileName;
            else
            {
                throw new NullReferenceException("File Name Should be Not Null ");
            }
        }

        public IEventLog Primary { get { return this; }}

        public override void Init()
        {
            _fileName = FileName;
        }
       
        public void AddItem(EvlResult result, string operation, string description)
        {
            var s = String.Format("{0:G} {1} {2} {3}", DateTime.Now, result, operation, description);
            WriteFile(s);
            //try
            //{
            //    using (var file = new System.IO.StreamWriter(_fileName, true, System.Text.Encoding.GetEncoding(1251)))
            //    {
            //        file.WriteLine(s);
            //    }
            //}
            //catch (Exception e)
            //{
            //    throw new FileNotFoundException(_fileName + e.Message);
            //}
        }
        public void AddItem(EvlResult result, EvlSubject subject,
                                string source, string operation, string description, string objects)
        {
            var s = String.Format("{0:G} {1} {2} {3} {4} {5} {6}",
                DateTime.Now, result, subject, source, operation, description, objects);

            WriteFile(s);
            //try
            //{
            //    using (var file = new System.IO.StreamWriter(_fileName, true, System.Text.Encoding.GetEncoding(1251)))
            //    {
            //        file.WriteLine(s);
            //    }
            //}
            //catch (System.Exception e)
            //{
            //    throw new FileNotFoundException(_fileName + e.Message);
            //}
        }
        public void AddItem(EvlResult result, EvlSubject subject,
                                string source, string entity, string operation, string description, string objects)
        {
            var s = String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7}",
                DateTime.Now, result, subject, source, entity, operation, description, objects);

            WriteFile(s);
            
        }

        public event EventHandler<IEventArgs> EventLogItemsChanged;

        protected virtual void OnEventLogItemsChanged(IEventArgs e)
        {
            EventHandler<IEventArgs> handler = EventLogItemsChanged;
            if (handler != null) handler(this, e);
        }

        public override IEnumerable<IEventLogItem> Items {
            get { throw new NotImplementedException(); }
        }

        public void AddItem(IEventLogItem i)
        {
            var s = String.Format("{0:G} {1} {2} {3} {4} {5} {6} {7}",
               i.DT, i.ResultCode, i.Subject, i.Source, i.Entity, i.Operation, i.Description, i.Object);

            WriteFile(s);
        }

        private void WriteFile(string s)
        {
            try
            {
                using (var file = new System.IO.StreamWriter(_fileName, true, System.Text.Encoding.GetEncoding(1251)))
                {
                    file.WriteLine(s);
                }
            }
            catch (System.Exception e)
            {
                throw new FileNotFoundException(_fileName + e.Message);
            }
        }


        public void ClearSomeData(int count)
        {
        }
        public override string ToString()
        {
            return string.Format("[Type:{0}, Name:{1}, FileName:{2}, ASync:{3}, Enable:{4}]",
                                    GetType(), Name, _fileName,  IsAsync, IsEnabled);
        }
    }
}
