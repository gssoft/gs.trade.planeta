using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.EventLog.Dto
{
    public class EventLogDto
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"[ID={ID}; Code={Code}; Name={Name}; Description={Description}]";
        }
    }

    public class EventLogItemDto // : IEventLogItem
    {
        public long ID { get; set; }
        public int EventLogID { get; set; }
        public string EventLog { get; set; }
        public DateTime DT { get; set; }
        public EvlResult ResultCode { get; set; }
        public EvlSubject Subject { get; set; }
        public string Source { get; set; }
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Description { get; set; }
        public string Object { get; set; }
        public long Index { get; set; }

        public override string ToString()
        {
            return string.Format("ID: {0}, EventLogID: {1}, EventLog: {10}, DT:{2}, Result: {3}, Subject: {4}," +
                                 "Source: {5}, Entity: {6}, Operation: {7}, " +   
                                 "Description: {8}, Object: {9}", 
                                 ID, EventLogID, DT.ToString("G"), ResultCode, Subject,
                                 Source, Entity, Operation, Description, Object, EventLog);
        }

        public long Key => ID;
    }
}
