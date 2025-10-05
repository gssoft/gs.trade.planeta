using System;
using System.Collections.Generic;
using GS.Trade.WebClients;

namespace GS.Trade.EntityWebClient
{
    public class PositionWebClient2 : WebClient2<PositionDto>
    {
        public PositionWebClient2(string baseAddress, string mediaType, string apiprefix)
            : base(baseAddress, mediaType, apiprefix)
        {
        }

        public bool Post(
            DateTime dt, long tradeId,
            string account, long algoid, string algo, string symbol,
            int side, double qty, string comment)
        {
            var p = new PositionDto
            {
                DT = dt,
                TradeId = tradeId,
                Account = account,
                AlgoID = algoid,
                Algorithm = algo,
                Symbol = symbol,
                Current = side * qty,
                Comment = comment
            };
            return PostItem(p);
        }

        public new IEnumerable<PositionDto> GetItems()
        {
            return base.GetItems();
            
        }
        public PositionDto GetItem(string account, string algo, string symb)
        {
            var parstr = @"?account=" + account + @"&algoid=" + algo + @"&symbol=" + symb;
            return GetItem(parstr);
            
        }
    }
}
