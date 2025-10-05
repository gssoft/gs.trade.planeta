using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GS;
using GS.Containers5;
using GS.Extension;
using GS.Interfaces;

namespace GS.Trade.DataBase.Model
{
  //  [DataContract]
    public class Account : IAccountDb
    {
        public Account()
        {
            Strategies = new List<Strategy>();
        }

        public int Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }

        public string Code { get; set; }
        public string TradePlace { get; set; }

        [DataType(DataType.Currency)]
        public decimal Balance { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        
        //[NotMapped]
        //public string StrKey { get { return TradePlace.TrimUpper().WithRight("@" + Code.TrimUpper()); } }
        
        //    [XmlIgnore]
        
        public List<Strategy> Strategies { get; set; }

        public override string ToString()
        {
            return String.Format("[Id={0}; Key={6}; Name={1}; Alias={2}; Code={3}; TradePlace={4}; Balance={5}]",
                Id, Name, Alias, Code, TradePlace, Balance, Key);
        }
    }
   // [DataContract]
    public partial class Ticker : ITickerDb
    {
        public Ticker()
        {
            Strategies = new List<Strategy>();
        }

        public int Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }

        public int Decimals { get; set; }
        public float MinMove { get; set; }

        public string TradeBoard { get; set; }
        public string Code { get; set; }
        public string BaseContract { get; set; }

        [DataType(DataType.Currency)]
        public decimal Margin { get; set; }
        public float PriceLimit { get; set; }

        public string FormatF { get; private set; }
        public string Format { get; private set; }
        public string FormatAvg { get; private set; }
        public string FormatM { get; private set; }

        //[DisplayFormat(DataFormatString = "{0:}")]
        //public float LastPrice { get; set; }
        //public DateTime LastPriceDateTime { get; set; }     

        //  [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime LaunchDate { get; set; }
      //  [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }

        public List<Strategy> Strategies { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [NotMapped]
        public string StrKey => TradeBoard.TrimUpper().WithRight("@" + Name.TrimUpper());

        public override string ToString() => 
            $"[Id: {Id} Name: {Name} Alias: {Alias} TradeBoard: {TradeBoard}  Code: {Code} BaseContract: {BaseContract} Decimals: {Decimals} MinMove {MinMove} Launch {LaunchDate} Expire {ExpireDate} StrKey {StrKey}]";
    }
    // [DataContract]
    public class Strategy : IStrategyDb
    {
        public List<Order> Orders;
        public List<Trade> Trades;
        public List<Deal> Deals;

        public Strategy()
        {
            Deals = new List<Deal>();
            Trades = new List<Trade>();
            Orders = new List<Order>();
        }

        public int Id { get; set; }
        public string Key { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }
        public string Code { get; set; }

        public int TimeInt { get; set; }
        public string TimeIntKey {
            get { return TimeInt.ToString(CultureInfo.InvariantCulture); }
        }

        //[NotMapped]
        //public string StrKey { get { return Code; } }

     //   [NotMapped]
    //    public string TradeKey { get { return Account.StrKey + Ticker.StrKey + StrKey; } }
        
        [ForeignKey("AccountId")]
        public virtual Account Account { get; set; }
        public int AccountId { get; set; }
        
        [ForeignKey("TickerId")]
        public virtual Ticker Ticker { get; set; }
        public int TickerId { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        //[XmlIgnore]
        //public List<Deal> Deals { get; set; }
        //[XmlIgnore]
        //public List<Trade> Trades { get; set; }
        //[XmlIgnore]
        //public List<Order> Orders { get; set; }
    }

    public class Trade : ITradeDb
    {
        public int Id { get; set; }
        public string Key { get; set; }

        public DateTime DT { get; set; }
        // public decimal Number { get; set; }
        public decimal Number { get; set; }

        public TradeOperationEnum Operation { get; set; }

        public long Quantity { get; set; }
        public decimal Price { get; set; }

        [NotMapped]
        public decimal Amount => Price * Quantity;

        public decimal OrderNumber { get; set; }
     
        //      [NotMapped]
  //  //    public string StrKey { get { return Strategy.Account.StrKey + Number; } }

  //      [NotMapped]
  //  //    public string TradeKey { get { return Strategy.TradeKey; } }
  //      [NotMapped]
  // //     public string AccountKey { get { return Strategy.Account.StrKey; } }
  //      [NotMapped]
  // //     public string TickerKey { get { return Strategy.Ticker.StrKey; } }

  ////      [NotMapped]
  ////      public int AccountId { get { return Strategy.Account.Id; } }
  //      [NotMapped]
  ////      public int TickerId { get { return Strategy.Ticker.Id; } }

        [ForeignKey("StrategyId")]
        public virtual Strategy Strategy { get; set; }
        public int StrategyId { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class Order : IOrderDb
    {
        public long Id { get; set; }
        public string Key { get; set; }

        public decimal Number { get; set; }

        public string OrderKey { get; set; }
        public string StrategyKey { get; set; }

        public OrderStatusEnum Status { get; set; }

        public OrderOperationEnum Operation { get; set; }
        public OrderTypeEnum OrderType { get; set; }

        public long Quantity { get; set; }
        public long Rest { get; set; }

        public double StopPrice { get; set; }
        public double LimitPrice { get; set; }
        public double FilledPrice { get; set; }

        [NotMapped]
        public double Amount => FilledPrice * Quantity;

        public string TrMessage { get; set; }

        //[NotMapped]
        //public string StrKey { get { return Strategy.Account.StrKey + Number; } }

        //[NotMapped]
        //public string TradeKey { get { return Strategy.TradeKey; } }

        [ForeignKey("StrategyId")]
        public virtual Strategy Strategy { get; set; }
        public int StrategyId { get; set; }

        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
    }

    public class Deal : IDealDb, IReportItem
    {
        public long Id { get; set; }
        public string Key { get; set; }

        public DateTime DT { get; set; }
        public decimal Number { get; set; }

        public PosOperationEnum Operation { get; set; }
        public PosStatusEnum Status { get; set; }

        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }

        public long Quantity { get; set; }

        public DateTime FirstTradeDT { get; set; }
        public decimal FirstTradeNumber { get; set; }

        public DateTime LastTradeDT { get; set; }
        public decimal LastTradeNumber { get; set; }

        public long Pos => Quantity * (short)Operation;

        public decimal PnL1 => Pos * (Price2 - Price1);

        public decimal PnL2 => PnL1;

        public decimal PnL => PnL1;

        public decimal Costs => 1;

        [NotMapped]
        public decimal Amount1 => Price1 * Quantity;
        [NotMapped]
        public decimal Amount2 => Price2 * Quantity;

        //[NotMapped]
        //public string StrKey { get { return Strategy.Account.StrKey + Number; } }

        //[NotMapped]
        //public string TradeKey { get { return Strategy.TradeKey; } }
       
        [ForeignKey("StrategyId")]
        public virtual Strategy Strategy { get; set; }
        public int StrategyId { get; set; }
    }

    public class Position : IPositionDb
    {
        public long Id { get; set; }
        public string Key { get; set; }

        [ForeignKey("StrategyId")]
        public virtual Strategy Strategy { get; set; }
        public int StrategyId { get; set; }

        public PosOperationEnum Operation { get; set; }
        public PosStatusEnum Status { get; set; }

        public long Quantity { get; set; }

        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }

        public decimal Price3 =>
            Price1 - (!IsNeutral && Quantity != 0 ?  (short) Operation*PnL3/Quantity : 0);

        public decimal PnL { get; set; }
        public decimal PnL3 { get; set; }

        public DateTime FirstTradeDT { get; set; }
        // [NotMapped]
        // public ulong FirstTradeNumberUlong => FirstTradeNumber.ToUint64();
        public decimal FirstTradeNumber { get; set; }
        public DateTime LastTradeDT { get; set; }
        // [NotMapped]
        // public ulong LastTradeNumberUlong => LastTradeNumber.ToUint64();
        public decimal LastTradeNumber { get; set; }
      
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }

        [NotMapped]
        public Position Position2 { get; set; }

        public long Pos => Quantity * (short) Operation;

        public decimal PnL1 => Pos * (Price2 - Price1);

        public string PnlFormated => PnL1.ToString(FormatStr);

        public decimal PnL2 => PnL1;

        public bool IsOpened => Status == PosStatusEnum.Opened;
        public bool IsClosed => Status == PosStatusEnum.Closed;

        public bool IsLong => Operation == PosOperationEnum.Long;
        public bool IsShort => Operation == PosOperationEnum.Short;
        public bool IsNeutral => Operation == PosOperationEnum.Neutral;

        public string FormatStr
        {
            get
            {
                if (Strategy?.Ticker != null && Strategy.Ticker.Format != null)
                    return Strategy.Ticker.Format;
               return "N4"; }
        }
        public string FormatAvgStr
        {
            get
            {
                if (Strategy?.Ticker != null && Strategy.Ticker.FormatAvg != null)
                    return Strategy.Ticker.FormatAvg;
                return "N5";
            }
        }

        public PosOperationEnum FlipOperation => IsLong
            ? PosOperationEnum.Short
            : (IsShort
                ? PosOperationEnum.Long
                : PosOperationEnum.Neutral);
    }

    public class Total : Position
    {
        //public decimal PnL { get; set; }
    }
}
