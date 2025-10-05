using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.Trades
{
    public class PositionTotal2 : Position2, IPositionTotal2
    {
        public string TotalPnLString => CurrencyPnL.ToString(TickerFormatM);
        public string TotalPointPnLString => PointPnL.ToString(TickerFormatM);

        public decimal PointPnL => (Price2 - Price1) * Quantity;
        public decimal CurrencyPnL => (Price2 - Price1) * Quantity;

        public override decimal DailyPnLAvg => TotalDays != 0
            ? PointPnL / TotalDays
            : 0;

        public string RealPoints => ((Price2 - Price1) * Quantity).ToString(TickerFormatAvg);
        public string RealPnL => ((Price2 - Price1) * Quantity).ToString(TickerFormatM);
        public IPosition Position2 { get; set; }
        public PositionTotal2()
        {
            Status = PosStatusEnum.Closed;
        }
        public PositionTotal2(Position2 p)
        {
            //Account = p.Account;
            //Ticker = p.Ticker;

            Strategy = p.Strategy;
          
            //TickerStr = (Ticker != null) ? Ticker.Code : p.TickerStr;

            Operation = p.Operation;
            Quantity = p.Quantity;
            //Amount = amount;
            PnL = 0;
            FirstTradeNumber = p.FirstTradeNumber;
            FirstTradeDT = p.FirstTradeDT;
            // CloseTradeNumber = maxnumber; DT_Close = maxdt;
            LastTradeNumber = p.LastTradeNumber;
            LastTradeDT = p.LastTradeDT;
            //LastTradePrice = t.Price;

            if (p.IsLong)
            {
                Price1 = p.Price1;
                Price2 = p.Price2;
            }
            else if (p.IsShort)
            {
                Price1 = p.Price2;
                Price2 = p.Price1;
            }

            //Count = p.Quantity;
            //OpenCloseFlag = 1;

            Status = PosStatusEnum.Closed;
            StrategyKeyHash = p.StrategyKeyHash;
        }
        public override void Clear()
        {
            FirstTradeDT = DateTime.MinValue;
            FirstTradeNumber = 0;
            LastTradeDT = DateTime.MinValue;
            LastTradeNumber = 0;

            Quantity = 0;

            Operation = PosOperationEnum.Neutral;
            Status = PosStatusEnum.Closed;

            LastChangedResult = PositionChangedEnum.Closed;

            Price1 = 0;
            Price2 = 0;
            PnL = 0;
            PnL3 = 0;

            Count = 0;

            Comment = "";
            FireTotalPositionUpdateEvent(this);
            //Strategy.OnChangedEvent(new Events.EventArgs
            //{
            //    Category = "UI.Positions",
            //    Entity = "Total",
            //    Operation = "Update",
            //    Object = this,
            //    Sender = this
            //});
        }

        //public void Update(IPosition2 p)
        //{
        //    if (p.IsLong)
        //    {
        //        Price1 = (Price1 * Quantity + p.Price1 * p.Quantity) / (Quantity + p.Quantity);
        //        Price2 = (Price2 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
        //    }
        //    else if (p.IsShort)
        //    {
        //        Price1 = (Price1 * Quantity + p.Price2 * p.Quantity) / (Quantity + p.Quantity);
        //        Price2 = (Price2 * Quantity + p.Price1 * p.Quantity) / (Quantity + p.Quantity);
        //    }

        //    if (Quantity == 0)
        //    {
        //        FirstTradeDT = p.FirstTradeDT;
        //        FirstTradeNumber = p.FirstTradeNumber;
        //    }
        //    Quantity += p.Quantity;
        //    PnL2 += (p.Price2 - p.Price1) * p.Pos;

        //    LastTradeDT = p.LastTradeDT;
        //    LastTradeNumber = p.LastTradeNumber;

        //    // Price2 = p.Price2;
        //}
    }
}
