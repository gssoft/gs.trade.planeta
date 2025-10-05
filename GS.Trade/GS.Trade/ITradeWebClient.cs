using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade
{
    public interface ITradeWebClient
    {
        bool Post(
            DateTime dt, long tradeId, long orderId,
            string account, long algoid, string symbol,
            int side, double quantity, double price, double commission, string comment);
        bool Post2(
            DateTime dt, long tradeId, long orderId,
            string account, long algoid, string algoname, string symbol,
            int side, double quantity, double price, double commission, string comment);

    }
}
