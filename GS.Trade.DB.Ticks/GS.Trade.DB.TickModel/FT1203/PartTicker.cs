using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.DB.TickModel.FT1203
{
    public partial class TickerStore
    {
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                                 Code, LongCode, KeyCode, Name, Description, D1, D2, D3);
        }
    }
    public partial class Ticker
    {
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7}",
                                 Symbol.Code, Symbol.LongCode, Code, LongCode, KeyCode, D1, D2, Alias );
        }
    }
    /*
    public partial class Tick
    {
        public override string ToString()
        {
            return String.Format("Code:{0}, LongCode:{1}, Price:{2}, Amount:{3}, DT:{4}, TradeNo:{5}",
                Code, LongCode, Price, Amount, DateTime, TradeNo);
        }
    }
    */
}
