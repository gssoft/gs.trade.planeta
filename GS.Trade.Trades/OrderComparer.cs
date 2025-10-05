using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Trades
{
    public class AccountTickerComparer : IEqualityComparer<Order>
    {
        public bool Equals(Order x, Order y)
        {
            if (Object.ReferenceEquals(x, y)) return true;

            if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null)) return false;

            return  x.Account == y.Account && x.Ticker == y.Ticker;
        }
        public int GetHashCode(Order ord)
        {
            if (Object.ReferenceEquals(ord, null)) return 0;

            //Get hash code for the Name field if it is not null.
            var hAccount = string.IsNullOrWhiteSpace(ord.Account) ? 0 : ord.Account.GetHashCode();
            var hTicker = string.IsNullOrWhiteSpace(ord.Ticker) ? 0 : ord.Ticker.GetHashCode();

            return hAccount + hTicker;
        }
    }
}
