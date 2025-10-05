using System;

namespace GS.Trade.EntityWebClient
{
    public class PositionDto
    {
        public PositionDto()
        {
        }
        public long Id { get; set; }

        public DateTime DT { get; set; }

        public string Account { get; set; }
        public long AlgoID { get; set; }
        public string Algorithm { get; set; }

        public string Symbol { get; set; }
        //public int Side { get; set; }
        //public double Qty { get; set; }
        public double Current { get; set; }
        public long TradeId { get; set; }
        public string Comment { get; set; }
        public int Side {
            get { return Current.CompareTo(0d) > 0 ? +1 : (Current.CompareTo(0d) < 0 ? -1 : 0); }
        }
        public double Qty {
            get { return Math.Abs(Current); }
        }
        //public double Current {
        //    get { return Side*Qty; }
        //}

        public override string ToString()
        {
            return string.Format("Id:{0}; Dt:{1}; Acc:{2}; AlgoId:{3}; Algo:{4}; Symbol:{5}; Current:{6}; TradeId:{7}; Comment:{8}",
                                    Id,DT,Account,AlgoID,Algorithm,Symbol,Current,TradeId,Comment);
        }
    }
    public class PositionDto2
    {
        public PositionDto2()
        {
        }
        public long Id { get; set; }

        public DateTime DT { get; set; }

        public string Account { get; set; }
        public long AlgoID { get; set; }
        public string Algorithm { get; set; }

        public string Symbol { get; set; }
        public int Side { get; set; }
        public double EntryQty { get; set; }
        public double ExitQty { get; set; }
        public double Qty { get; set; }
        public double EntryPrice { get; set; }
        public double ExitPrice { get; set; }
        public double Pips { get; set; }
        public double PnL { get; set; }
        public double Costs { get; set; }
        public long TradeId { get; set; }
        public string Comment { get; set; }
    }
}
