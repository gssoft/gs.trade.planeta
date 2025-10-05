using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serializaion
{
    [Serializable]
    public class EventArgsDto
    {
        public string Key { get; set; }
        public string Entity { get; set; }
        public object DtoObject { get; set; }
        public override string ToString()
        {
            return ($"Type:{GetType().Name} Key: {Key} Dto: {DtoObject.ToString()} ");
        }
    }
    [Serializable]
    public class TradeDto
    {
        public UInt64 Number { get; set; }
        public string Strategy { get; set; }
        public string Ticker { get; set; }
        public UInt64 OrderNumber { get; set; }
        public override string ToString()
        {
            return $"{GetType().Name} Number:{Number} OrderNumber: {OrderNumber}";
        }
    }
    [Serializable]
    public class OrderDto
    {
        public UInt64 Number { get; set; }
        public string Strategy { get; set; }
        public string Ticker { get; set; }
        public Int32 Quantity { get; set; }
        public Int32 Rest { get; set; }
        public override string ToString()
        {
            return $"{GetType().Name} Number:{Number} Rest:{Rest}";
        }
    }
}


