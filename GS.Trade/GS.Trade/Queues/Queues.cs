using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Queues;
using GS.Trade.Queues;
using Microsoft.SqlServer.Server;

namespace GS.Trade.Queues
{
    public class TradeEntityQueue<T> : QueueFifo<TradeQueueEntity<T>>
    {
    }
    public class TradeQueue : QueueFifo<Events.IEventArgs>
    {
    }

    public class TradeQueueEntity<T>
    {
        public StorageOperationEnum Operation { get; set; }
        public T Entity { get; set; }

        public TradeQueueEntity(StorageOperationEnum oper, T entity)
        {
            Operation = oper;
            Entity = entity;
        }
        public override string ToString()
        {
            return String.Format("[{0} {1}]", (StorageOperationEnum)Operation, Entity);
        }
    }

}
