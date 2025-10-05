using GS.Interfaces;
//using GS.Storages;
using GS.Trade.Trades;
using GS.Trade.Trades.Storage;

namespace GS.Trade.TradeStorage
{
    public class TradeStore : Store<string, ITradeStorage>, IHaveUri
    {
        public TradeStore()
        {
        }
    }
}
