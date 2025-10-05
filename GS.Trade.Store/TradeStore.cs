//using GS.Storages;
using GS.Interfaces;

namespace GS.Trade.Store
{
    public class TradeStore : Store<string, ITradeStorage>, IHaveUri
    {
        public TradeStore()
        {
        }
    }
}
