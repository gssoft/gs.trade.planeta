using System;
using System.IO;
using GS.Interfaces;

namespace GS.EventLog
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
        public void AddItem(EvlResult result, string operation, string description)
        {
            var s = String.Format("{0:G} {1} {2} {3}", DateTime.Now, result, operation, description);
            try
            {
                using (var file = new System.IO.StreamWriter(_fileName, true, System.Text.Encoding.GetEncoding(1251)))
                {
                    file.WriteLine(s);
                }
            }
            catch (Exception e)
            {
                throw new FileNotFoundException(_fileName + e.Message);
            }
        }
        public void AddItem(EvlResult result, EvlSubject subject,
                                string source, string operation, string description, string objects)
        {
            var s = String.Format("{0:G} {1} {2} {3} {4} {5} {6}",
                DateTime.Now, result, subject, source, operation, description, objects);
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
    }
}
