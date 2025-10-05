using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GS.Web.Service01
{
    public class EventArgs : System.EventArgs
    {
        public EventArgs(){}
        public string Category { get; set; }
        public string Entity { get; set; }
        public bool IsHighPriority { get; set; }
        public string Key { get; }
        public string Message { get; set; }
        [XmlIgnore]
        public object Object { get; set; }
        public string Operation { get; set; }
        public string OperationKey { get; }
        public string Process { get; set; }
        //public Action<IEventArgs1> ProcessingAction { get; set; }
        public string Sender { get; set; }
        public string FullTypeName { get; set; }

        public override string ToString()
        {
            return $"Category:{Category}, Entity:{Entity}, IsHighPriority:{IsHighPriority}, " +
                   $"Key:{Key}, Message:{Message}, Operation:{Operation}, OperationKey:{OperationKey}, " +
                   $"Process:{Process}, Sender:{Sender}, FullTypeName: {FullTypeName}";
        }
    }

    [Serializable]
    public class EventArgsBytes : System.EventArgs
    {
        public EventArgsBytes() { }
        public string Category { get; set; }
        public string Entity { get; set; }
        public bool IsHighPriority { get; set; }
        public string Key { get; }
        public string Message { get; set; }
        [XmlIgnore]
        public object Object { get; set; }
        public string Operation { get; set; }
        public string OperationKey { get; }
        public string Process { get; set; }
        //public Action<IEventArgs1> ProcessingAction { get; set; }
        public string Sender { get; set; }
        public string FullTypeName { get; set; }

        public override string ToString()
        {
            return $"Category:{Category}, Entity:{Entity}, IsHighPriority:{IsHighPriority}, " +
                   $"Key:{Key}, Message:{Message}, Operation:{Operation}, OperationKey:{OperationKey}, " +
                   $"Process:{Process}, Sender:{Sender}, FullTypeName: {FullTypeName}";
        }
    }

}
