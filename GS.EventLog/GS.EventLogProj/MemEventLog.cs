using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers;
using GS.Interfaces;

namespace GS.EventLog
{
    public class MemEventLog : ListContainer<MemEventLogItem>
    {
        public EventHandler<EventLogEvent> EventLogItemEvent; 

        public long Index { get; private set; }
        public int Capasity { get; private set; }

        public MemEventLog()
        {
            Capasity = 100;
        }

        public void AddItem(EvlResult result, EvlSubject subject,
                                    string source, string entity, string operation, string description, string sobject)
        {
            var evli = new MemEventLogItem
            {
                DT = DateTime.Now,
                ResultCode = result,
                Subject = subject,
                Source = source,
                Entity = entity,
                Operation = operation,
                Description = description,
                Object = sobject,
                Index = ++Index,

                MemEventLog = this
            };
            AddNew(evli);
            
            //KeepCapasity();
            ClearSomeData();

            if(EventLogItemEvent != null)
                EventLogItemEvent( this, new EventLogEvent{ Operation = "New", EventLogItem = evli});
        }

        private void KeepCapasity()
        {
            var cnt = Items.Count();
            for (var i = 0; i < cnt - Capasity; i++)
                RemoveAt(i);
        }
        private void ClearSomeData()
        {

                while (Count > Capasity)
                {
                    RemoveAt(0);
                }
            
        }
    }

    public class MemEventLogItem : ContainerItem, IEventLogItem
    {
        public DateTime DT { get; set; }
        public EvlResult ResultCode { get; set; }
        public EvlSubject Subject { get; set; }
        public string Source { get; set; }
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Description { get; set; }
        public string Object { get; set; }
        public long Index { get; set; }

        public MemEventLog MemEventLog { get; set; }

        public string DateTimeString { get { return DT.ToString("G"); } }
        public string TimeDateString
        {
            get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
        }
        public override string Key
        {
            get { return Index.ToString("F0"); }
        }
        public override string ToString()
        {
            return String.Format("[DT={0:G}; Result={1}; Subject={2}; Source={3}; Entity={4}; Operation={5}; Description={6}; Object={7}, Index={8}",
                DT, ResultCode.ToString().ToTitleCase(), Subject, Source, Entity, Operation, Description, Object, Index);
        }
    }
}
