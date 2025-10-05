using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Xml.Linq;
using System.IO;
using System.Xml.Serialization;
//namespace sg_Trades
using GS.Extension;

namespace GS.Trade.Trades
{
    //**************************  Trade **********************************************
    /*
    public static class General
    {
    public static string StrategyKey( string account, string strategy, string ticker)
    {
    return account + strategy + ticker;
    }
    }
    */
    public class Trade : ITrade
    {
        public enum OperationEnum : short { Sell = -1, Buy = +1 }

        public Trades _Trades;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, e);
        }
        
        // public int StrategyKeyHash { get; set; }

        public ulong Number { get; set; }
        public DateTime DT { get; set; }
        public string ClassCode { get; set; }
        public string Ticker { get; set; }
        //   public int Direction {get; set;}
        public long Quantity { get; set; } // { return Amount;} }
        public TradeOperationEnum Operation { get; set; } //  { return Direction; } }
        public TradeOperationEnum TradeOperation { get { return (TradeOperationEnum) Operation; } }

        public long Position { get { return Quantity * (short)Operation; } }
        public Decimal Price { get; set; }
        public Decimal Amount { get { return Quantity * Price; } }
        public string OperationString { get { return Operation > 0 ? "Buy" : "Sell"; } }
        public Decimal OrderPrice { get; set; }
        public ulong OrderNumber { get; set; }
        public int ID_TradeInfo { get; set; }
        public string Account { get; set; }
        public string Strategy { get; set; }

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
        public string PositionString { get { return Position.ToString("N0"); } }
        public string AmountString { get { return Amount.ToString("N2"); } }
        public string PriceString { get { return Price.ToString("N2"); } }
        public string OrderPriceString { get { return OrderPrice.ToString("N2"); } }
        public string ModeString { get { return (Mode == 0) ? "New" : (Mode == 1) ? "Init" : "End Init"; } }

        public string StrategyKey { get { return General.StrategyKey(Account,Strategy,Ticker); } }
        public string Key { get { return General.Key(Account,Number); } }
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


        // *************************  Trade Method  ************************************************
        public Trade(ulong number, DateTime dt, string ticker, TradeOperationEnum operation, ulong quantity)
        {
            Number = number;
            DT = dt;
            Ticker = ticker;
            Operation = operation;
            Quantity = Convert.ToInt64(quantity);

            //  EventLogItem eli = new EventLogItem(EvlResult.SUCCESS,"Trade ADD",  "Trade is Added: " + Number.ToString() + DT.ToString() + Ticker );
            //  _Trades._EventLog.Add(eli);
        }
        public Trade(ulong number, DateTime dt, string ticker, TradeOperationEnum operation, int quantity,
                        decimal tradeprice, ulong ordernumber,
                            string account, string strategy, int idtradeinfo)
        {
            Number = number;
            DT = dt;
            Ticker = ticker;
            Operation = operation;
            Quantity = quantity;

            Price = tradeprice;

            if (!string.IsNullOrEmpty(strategy))
                Strategy = strategy;
            else
                Strategy = "999";
            OrderNumber = ordernumber;

            Account = account;
            ID_TradeInfo = idtradeinfo;

            // StrategyKeyHash = GetStrategyKeyHash();

        }
        public Trade(
            ulong number, DateTime dt, int mode,
            string acc, string strategy,
            string classCode, string secCode,
            TradeOperationEnum operation, int quantity, double price, string comment, 
            ulong orderNumber, double commissionTs)
        {
            Number = number;
            DT = dt;
            Mode = mode;
            Account = acc;
            Strategy = !string.IsNullOrWhiteSpace(strategy) ? strategy : "999";
            //Strategy = strategy;
            ClassCode = classCode;
            Ticker = secCode;
            Operation = operation;
            Quantity = quantity;
            Price = Convert.ToDecimal(price);
            Comment = comment;
            OrderNumber = orderNumber;
            CommissionTs = Convert.ToDecimal(commissionTs);

           // StrategyKeyHash = GetStrategyKeyHash();
        }
        /*
        private int GetStrategyKeyHash()
        {
            return (Account + Strategy + Ticker).GetHashCode();
        }
         */
        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8} {9} {10}",
                DT, Account, Strategy, Ticker, OperationString, Position, Price, Quantity * Price, Number, OrderNumber, MyIndex);


        }
    }
}