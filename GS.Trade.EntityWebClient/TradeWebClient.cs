using System;
using System.Collections.Generic;
using System.Net;
using GS.Extension;
using GS.Trade.WebClients;

namespace GS.Trade.EntityWebClient
{
    public class TradeWebClient : WebClient<TradeDto>
    {
        public TradeWebClient(string baseAddress, string mediaType, string apiprefix)
            : base(baseAddress, mediaType, apiprefix)
        {
        }

        public bool Post(
            DateTime dt, long tradeId, long orderId,
            string account, long algoid, string symbol,
            int side, double quantity, double price, double commission, string comment)
        {
            var tr = new TradeDto
            {
                Dt = dt,
                TradeId = tradeId,
                OrderId = orderId,
                Account = account,
                AlgoID = algoid,
                Symbol = symbol,
                Side = side,
                Qty = quantity,
                Price = price,
                Commission = commission,
                //Comment = comment
            };

            bool operStatus;
            try
            {
                operStatus = PostData3(tr);
            }
            catch (Exception e)
            {
                operStatus = false;
                ErrorMessage = e.Message;
            }
            return operStatus;
        }

        public IEnumerable<TradeDto> Get()
        {
            try
            {
                return GetData();
            }
            //catch (AggregateException ae)
            //{
            //    ErrorMessage = ae.AggrExceptionMessage();
            //    ErrorMessage = ae.AggrExceptionMessage();
            //}
            catch (Exception e)
            {
                ErrorMessage = e.ExceptionMessage();
            }
            return null;
        }
        public IEnumerable<TradeDto> Get(out bool operStatus)
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
        public bool Get(out IEnumerable<TradeDto> trades)
        {
            trades = default(IEnumerable<TradeDto>);
            try
            {
                trades = GetData();       
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            return HttpResponseStatus;
        }
    }
}
