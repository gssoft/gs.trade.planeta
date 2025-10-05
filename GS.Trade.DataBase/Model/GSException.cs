using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Exceptions;

namespace GS.Trade.DataBase.Model
{
    public class GSException : IGSExceptionDb
    {
        public long Id { get; set; }
        public string Key { get; set; }

        public DateTime DateTime { get; set; }
        public string Source { get; set; }
        public string ObjType { get; set; }
        public string Operation { get; set; }
        public string ObjStr { get; set; }
        public string Message { get; set; }
        public string SourceExc { get; set; }
        public string ExcType { get; set; }
        public string TargetSite { get; set; }

        

        public string DateTimeString { get { return DateTime.ToString("G"); } }
        public string TimeDateString { get { return DateTime.ToString("T") + ' ' + DateTime.ToString("d"); }
        }
    }
}
