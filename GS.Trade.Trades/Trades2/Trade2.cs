using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.Trades.Trades2
{
    public class Trade2 : ITrade2
   {
        //public enum OperationEnum : short { Sell = -1, Buy = +1 }

        public Trades Trades;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        
        // public int StrategyKeyHash { get; set; }
 
        public IStrategy Strategy { get; set; }

        public IAccount Account { get { return Strategy != null ? Strategy.Account : null; } }
        public ITicker Ticker { get { return Strategy != null ? Strategy.Ticker : null; } }

        public string StrategyCode { get { return Strategy != null ? Strategy.Code : "Unknown"; } }

        public string TickerBoard { get { return Ticker != null ? Ticker.ClassCode : ClassCodeEx; } }
        public string TickerCode { get { return Ticker != null ? Ticker.Code : TickerEx; } }

        public string AccountCode { get { return Account != null ? Account.Code : AccountEx; } }

        public long Number { get; set; }


        public DateTime DT { get; set; }
        

        public long Quantity { get; set; }
        public TradeOperationEnum Operation { get; set; }
        public TradeOperationEnum TradeOperation { get { return (TradeOperationEnum) Operation; } }

        public long Position { get { return Quantity * (short)Operation; } }
        public Decimal Price { get; set; }
        public Decimal Amount { get { return Quantity * Price; } }

        public IOrder Order { get; set; }
        public Decimal OrderPrice { get; set; }
        public long OrderNumber { get; set; }

        public TradeStatusEnum Status { get; set; }

        public string AccountEx { get; set; }
        public string StrategyEx { get; set; }
        public string ClassCodeEx { get; set; }
        public string TickerEx { get; set; }

        public int TimeInterval { get; set; }
        public int Mode { get; set; }
        public string Comment { get; set; }
        public decimal CommissionTs { get; set; }

        public int MyStatus { get; set; }
        public long MyIndex { get; set; }

        public DateTime Registered { get; set; }

        public string DateTimeString { get { return DT.ToString("G"); } }
        public string TimeDateString
        {
            get { return DT.ToString("T") + ' ' + DT.ToString("d"); }
        }
        public string OperationString { get { return Operation > 0 ? "Buy" : (Operation < 0 ? "Sell" : "Unknown"); } }
        public string PositionString { get { return Position.ToString("N0"); } }
        public string AmountString { get { return Amount.ToString("N2"); } }
        public string PriceString { get { return Price.ToString("N2"); } }
        public string OrderPriceString { get { return OrderPrice.ToString("N2"); } }
        public string ModeString { get { return (Mode == 0) ? "New" : (Mode == 1) ? "Init" : "End Init"; } }

        public string StrategyKey { get { return General.StrategyKey(AccountEx,StrategyEx,TickerEx); } }

        public string OrderKey {
            get { return (OrderNumber + "." + AccountCode).TrimUpper(); }
        }

        public string Key { get { return (Number + "." + AccountCode).TrimUpper(); } }

       // public string TradeKey {get { return Key; } }
        public string StratTicker {
            get { return Strategy + "." + Ticker; }
        }

        public string ShortInfo {
            get { return String.Format("{0} {1}{2}@ {3} tr#:{4}ord#:{5}",
                        OperationString, Ticker, Quantity.ToString(CultureInfo.InvariantCulture).WithSqBrackets(),
                        PriceString, Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets(), 
                        OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets());
            }
        }
       
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
                DT, AccountCode, StrategyCode, TickerCode, OperationString, Position, Price, Quantity * Price, Number, OrderNumber, MyIndex);


        }
    }
}
