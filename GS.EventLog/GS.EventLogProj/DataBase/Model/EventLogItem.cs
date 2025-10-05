using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.EventLog.DataBase.Model
{
    public abstract class Component
    {
        public string Alias { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class EventLog : Component
    {
        public EventLog()
        {
            Items = new List<EventLogItem>();
        }

        public int EventLogID { get; set; }
        public virtual ICollection<EventLogItem> Items { get; set; }
    }


    public class EventLogItem
    {
        public long EventLogItemID { get; set; }
        public DateTime DT { get; set; }
        public EvlResult ResultCode { get; set; }
        public EvlSubject Subject { get; set; }
        public string Source { get; set; }
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Description { get; set; }
        public string Object { get; set; }
        public long Index { get; set; }

        public int EventLogID { get; set; }
        public virtual EventLog EventLog { get; set; }

        public string DateTimeString { get { return DT.ToString("G"); } }
        public string TimeDateString
        {
            get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
        }
       
        public override string ToString()
        {
            return String.Format("[ID={0}; DT={1:G}; Result={2}; Subject={3}; Source={4}; Entity={5}; Operation={6}; Description={7}; Object={8}, Index={9}]",
                EventLogItemID, DT, ResultCode.ToString().ToTitleCase(), Subject, Source, Entity, Operation, Description, Object, Index);
        }
    }
}
