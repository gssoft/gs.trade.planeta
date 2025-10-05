using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GS.Extension;

namespace GS.Trade.Web.Mvc_07.ViewModel
{
    public class ViewDeal
    {
        public long Id  { get; set; }
        public string Account { get; set; }
        public string Ticker { get; set; }
        public string Strategy { get; set; }
        public int  TimeInt { get; set; }
        public PosOperationEnum Side { get; set; }
        public long  Qty { get; set; }
        public string QtyStr { get { return ((int) Side > 0 ? "+ " : "-- ") + Qty; }
    }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public decimal PnL {
            get { return (int)Side*(Price2 - Price1)*Qty; }
        }

        public DateTime FirstTradeDT { get; set; }
        public DateTime LastTradeDT { get; set; }
        public string Format { get; set; }
        public string FormatAvg { get; set; }

        public string PriceStr1 
        {
            get { return Price1.ToString(Format.HasValue() ? Format : "N2"); }
        }
        public string PriceStr2
        {
            get { return Price2.ToString(Format.HasValue() ? Format : "N2"); }
        }
        public string PnLStr
        {
            get { return PnL.ToString(Format.HasValue() ? Format : "N2"); }
        }
        public string PriceAvgStr1
        {
            get { return Price1.ToString(FormatAvg.HasValue() ? FormatAvg : "N3"); }
        }
        public string PriceAvgStr2
        {
            get { return Price2.ToString(FormatAvg.HasValue() ? FormatAvg : "N3"); }
        }
        public string PnLAvgStr
        {
            get { return PnL.ToString(FormatAvg.HasValue() ? FormatAvg : "N3"); }
        }
    }
}