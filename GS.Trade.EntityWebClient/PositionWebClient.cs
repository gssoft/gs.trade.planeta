using System;
using System.Collections.Generic;
using GS.Trade.WebClients;

namespace GS.Trade.EntityWebClient
{
    public class PositionWebClient : WebClient<PositionDto>
    {
         public PositionWebClient(string baseAddress, string mediaType, string apiprefix)
            : base(baseAddress, mediaType, apiprefix)
        {
        }

        public bool Post(
            DateTime dt, long tradeId,
            string account, long algoid, string algo, string symbol,
            int side, double qty, string comment)
        {
            var tr = new PositionDto
            {
                DT = dt,
                TradeId = tradeId,
                Account = account,
                AlgoID = algoid,
                Algorithm = algo,
                Symbol = symbol,
                Current = side*qty,
                Comment = comment
            };

            bool operStatus;
            try
            {
                operStatus = PostData(tr);
            }
            catch (Exception e)
            {
                operStatus = false;
                ErrorMessage = e.Message;
            }
            return operStatus;
        }

        public IEnumerable<PositionDto> GetItems(out bool operStatus)
        {
            try
            {
                var trs = GetData(out operStatus);
                return trs;
            }
            catch (Exception e)
            {
                operStatus = false;
                ErrorMessage = e.Message;
            }
            return null;
        }
        public PositionDto GetItem(out bool operStatus, string account, string algo, string symb)
        {
            var parstr = @"?account=" + account + @"&algoid=" + algo + @"&symbol=" + symb;
            try
            {
                var trs = GetItem(out operStatus, parstr);
                return trs;
            }
            catch (Exception e)
            {
                operStatus = false;
                ErrorMessage = e.Message;
            }
            return null;
        }
        public bool GetItem(out PositionDto p, string account, long algo, string symb)
        {
            var parstr = @"?account=" + account + @"&algoid=" + algo + @"&symbol=" + symb;
            bool result;
            try
            {
                p = GetItem(out result, parstr);
            }
            catch (Exception e)
            {
                result = false;
                p = null;
                ErrorMessage = e.Message;
            }
            return result;
        }
    }
   
}
