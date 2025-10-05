using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Containers5;
using GS.Elements;
using GS.Queues;
using GS.Trade.Queues;

namespace GS.Trade.Storage2
{
    public interface ITradeBaseRepository3<in TKey, TValue, out TValue2> :
                        IElement1<string>,
                        IHaveQueue<TradeQueueEntity<TValue>>
        where TValue : class, IHaveKey<TKey>
    {
        TValue2 GetByKey(TKey key);
        TValue2 Get(TKey anyString);

        bool Add(TValue v);
        bool AddNew(TValue v);
        bool AddOrUpdate(TValue v);

        TValue AddOrGet(TValue item);
        TValue Register(TValue item);

        bool Update(TValue v);
    }
}
