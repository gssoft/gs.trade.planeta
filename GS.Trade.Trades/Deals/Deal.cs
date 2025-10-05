using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Identity;

namespace GS.Trade.Trades.Deals
{
    public class Deal : Position2, IDeal, IReportItem
    {
        protected static DateTimeNumberIdentity Identity = new DateTimeNumberIdentity(1000000);
        public Deal()
        {
            Number = Identity.Next();
            DT = DateTime.Now;
        }

        public string Amount1String => Amount1.ToString(TickerFormatAvg);
        public string Amount2String => Amount2.ToString(TickerFormatAvg);

        public string Price1String => Price1.ToString(TickerFormatAvg);
        public string Price2String => Price2.ToString(TickerFormatAvg);
        public string LastPriceString => LastPrice.ToString(TickerFormat);

        public bool IsFinResultPositive => PnL >= 0;
        public bool IsFinResultNegative => PnL < 0;
        // IReport Item

    }
}
