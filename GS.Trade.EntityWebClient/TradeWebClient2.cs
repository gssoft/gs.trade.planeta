using System;
using System.Collections.Generic;
using GS.Trade.WebClients;

namespace GS.Trade.EntityWebClient
{
    public class TradeWebClient2 : WebClient2<TradeDto>, ITradeWebClient
    {
        public TradeWebClient2(string baseAddress, string mediaType, string apiprefix)
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
            return PostItem(tr);
        }
        public bool Post2(
            DateTime dt, long tradeId, long orderId,
            string account, long algoid, string algoname, string symbol,
            int side, double quantity, double price, double commission, string comment)
        {
            var tr = new TradeDto
            {
                Dt = dt,
                TradeId = tradeId,
                OrderId = orderId,
                Account = account,
                AlgoID = algoid,
                AlgoName = algoname,  
                Symbol = symbol,
                Side = side,
                Qty = quantity,
                Price = price,
                Commission = commission,
                //Comment = comment
            };
            return PostItem(tr);
        }

        public new IEnumerable<TradeDto> GetItems()
        {
            return base.GetItems();
        }
        public IEnumerable<TradeDto> GetItems(string account, long algoId, string symbol)
        {
            return GetItems("sdds");
        }
        public TradeDto GetItem(long id)
        {
            return null;
        }
        public TradeDto GetItem(string account, long algoId, string symbol)
        {
            return GetItem("sfsf");
        }

        //public IEnumerable<TradeDto> Get(out bool operStatus)
        //{
        //    try
        //    {
        //        var trs = GetData(out operStatus);
        //        return trs;
        //    }
        //    catch (Exception e)
        //    {
        //        operStatus = false;
        //        ErrorMessage = e.Message;
        //    }
        //    return null;
        //}
        //public bool Get(out IEnumerable<TradeDto> trades)
        //{
        //    trades = default(IEnumerable<TradeDto>);
        //    try
        //    {
        //        trades = GetItem();
        //    }
        //    catch (Exception e)
        //    {
        //        ErrorMessage = e.Message;
        //    }
        //    return HttpResponseStatus;
        //}
    }
}
