using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Trades
{
    public static class General
    {
        public static string Key( string account, ulong number)
        {
            return number + account;
        }
        public static string StrategyKey( string account, string strategy, string ticker)
        {
            return account + strategy + ticker;
        }
    }
}
