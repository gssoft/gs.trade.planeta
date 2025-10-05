using System;
using System.IO;
using GS.Trade.Interfaces;

namespace GS.Trade.EventLog
{
    public class FileEventLog :IEventLog
    {
        private readonly string _fileName;
        public FileEventLog(string fileName)
        {
            if (!string.IsNullOrEmpty(fileName))
                _fileName = fileName;
            else
            {
                throw new NullReferenceException("File Name Should be Not Null ");
            }
        }
        public void AddItem(EnumEventLog result, string operation, string description)
        {
            var s = String.Format("{0:G} {1} {2} {3}", DateTime.Now, result, operation, description);
            try
            {
                using (var file = new System.IO.StreamWriter(_fileName, true, System.Text.Encoding.GetEncoding(1251)))
                {
                    file.WriteLine(s);
                    file.Close();
                }
            }
            catch (Exception e)
            {
                throw new FileNotFoundException(_fileName + e.Message);
            }
        }
        public void AddItem(EnumEventLog result, EnumEventLogSubject subject,
                                string source, string operation, string description, string objects)
        {
            var s = String.Format("{0:G} {1} {2} {3} {4} {5} {6}",
                DateTime.Now, result, subject, source, operation, description, objects);
            try
            {
                using (var file = new System.IO.StreamWriter(_fileName, true, System.Text.Encoding.GetEncoding(1251)))
                {
                    file.WriteLine(s);
                    file.Close();
                }
            }
            catch (System.Exception e)
            {
                throw new FileNotFoundException(_fileName + e.Message);
            }
        }
    }
}
