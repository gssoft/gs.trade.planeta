using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GS.Interfaces;

namespace GS.EventLog.DataBase1.Model
{
    //public abstract class Component
    //{
    //    public string Alias { get; set; }
    //    public string Code { get; set; }
    //    public string Name { get; set; }
    //    public string Description { get; set; }

    //    [XmlIgnore]
    //    public string MyKey {
    //        get { return Code; }
    //    }
    //}
    [Table("EventLogs")]
  //  [XmlInclude(typeof(List<DbEventLogItem>))]
    public class DbEventLog //: Component
    {
        public DbEventLog()
        {
           // EventLogItems = new List<DbEventLogItem>();
            ModifiedDT = DateTime.Now;
        }
        [Key]
        public int EventLogID { get; set; }
        public int ApplicationID { get; set; }
        public string Alias { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ModifiedDT { get; set; }
        //  [XmlIgnore]
        public virtual ICollection<DbEventLogItem> EventLogItems { get; set; }

        public override string ToString()
        {
            return String.Format("[EventLogID={0}; ApplicationID={6}; Alias={1}; Code={2}; Name={3}; Description={4}, Modified: {5}]",
                                    EventLogID, Alias, Code, Name, Description, ModifiedDT, ApplicationID);
        }
    }

    [Table("EventLogItems")]
    public class DbEventLogItem
    {
        [Key]
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
   //     [XmlIgnore]
        [ForeignKey("EventLogID")]
       public virtual DbEventLog DbEventLog { get; set; }

        public string DateTimeString { get { return DT.ToString("G"); } }
        public string TimeDateString
        {
            get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
        }
       
        public override string ToString()
        {
            return String.Format("[ID={0}; DT={1:G}; Result={2}; Subject={3}; Source={4}; Entity={5}; Operation={6}; Description={7}; Object={8}, Index={9}]",
                EventLogItemID, DT, ResultCode /*.ToString().ToTitleCase()*/,
                Subject, Source, Entity, Operation, Description, Object, Index);
        }
    }
}
