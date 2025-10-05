using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Counters;
using GS.Extension;
using GS.Identity;

namespace GS.Exceptions
{
    public class GSException : IGSException
    {
        protected static DateTimeNumberIdentity Identity = new DateTimeNumberIdentity(1000000);
        //private static readonly LongCounter Counter = new LongCounter();

        public long Id { get; set; }

        public DateTime DateTime { get; set; }
        public string Source { get; set; }
        public string ObjType { get; set; }
        public string Operation { get; set; }
        public string ObjStr { get; set; }
        public string Message { get; set; }
        public string SourceExc { get; set; }
        public string ExcType { get; set; }
        public string TargetSite { get; set; }
        public string StackTrace { get; set; }

        public string Key { get; private set; }

        public GSException()
        {
            Key = Identity.Next().ToString(CultureInfo.InvariantCulture);
            DateTime = DateTime.Now;
        }

        public string DateTimeString => DateTime.ToString("G");

        public string TimeDateString => DateTime.ToString("T") + ' ' + DateTime.ToString("d");


        public override string ToString()
        {
            return
                $"[{Id}, {DateTime.ToString("g")}, {Source}, {ObjType}, {Operation}, {ObjStr}, {Message}, {SourceExc}, {ExcType}, {TargetSite}, {Key}]";
        }

        public void SaveInFile(string dir)
        {
            System.IO.File.WriteAllText(dir + Key + ".txt", ToString());
        }

        public static void SaveInFile(string dir, Events.IEventArgs ea)
        {
            if (ea.OperationKey == "UI.Exceptions.Exception.Add".TrimUpper())
            {
                var ex = (GSException) ea.Object;
                var sb = new StringBuilder();
                sb.AppendLine(ex.Key);
                sb.AppendLine(ex.DateTimeString);
                sb.AppendLine(ex.Source);
                sb.AppendLine(ex.ObjType);
                sb.AppendLine(ex.ObjStr);
                sb.AppendLine(ex.Operation);
                sb.AppendLine(ex.Message);
                sb.AppendLine(ex.ExcType);
                sb.AppendLine(ex.SourceExc);
                sb.AppendLine(ex.TargetSite);
                sb.AppendLine(ex.StackTrace);

                System.IO.File.WriteAllText(dir + ex.Key + ".txt", sb.ToString());
                //return;
            }
            else if (ea.OperationKey == "UI.Exceptions.Exception.AddMany".TrimUpper())
            {
                var sb = new StringBuilder();
                foreach (var ex in (IEnumerable<GSException>) ea.Object)
                {
                    sb.AppendLine(ex.Key);
                    sb.AppendLine(ex.DateTimeString);
                    sb.AppendLine(ex.Source);
                    sb.AppendLine(ex.ObjType);
                    sb.AppendLine(ex.ObjStr);
                    sb.AppendLine(ex.Operation);
                    sb.AppendLine(ex.Message);
                    sb.AppendLine(ex.ExcType);
                    sb.AppendLine(ex.SourceExc);
                    sb.AppendLine(ex.TargetSite);
                    sb.AppendLine(ex.StackTrace);
                    sb.AppendLine();
                }
                System.IO.File.WriteAllText(dir + ((List<GSException>)ea.Object)[0].Key + ".txt", sb.ToString());            
            }
        }
    }
}

