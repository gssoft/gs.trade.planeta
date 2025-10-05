using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Extension;

namespace GS.EnableDisable
{
   
    class EnableDisable : Element1<string>
    {
        public DateTime DT { get; private set; }
        public bool Status { get; private set; }
        public string Reason { get; private set; }

        public override string Key
        {
            get { return Code.HasValue() ? Code : GetType().FullName; }
        }

        public void Set(bool value, string reason)
        {
            Status = value;
            DT = DateTime.Now;
            Reason = reason;
        }
        
        public override string ToString()
        {
            return String.Format("Code: {0}; DT: {1}; Status: {2}, Reason: {3}",
                Code, DT, Status, Reason);
        }
    }
}
