using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS;
using GS.Extension;

namespace GS.Trade.DataBase.Model
{
  //  [DataContract]
    public class Account
    {
        public Account()
        {
            Strategies = new List<Strategy>();
          }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }

        public string Code { get; set; }
        public string TradePlace { get; set; }

        public decimal Balance { get; set; }
        
        [NotMapped]
        public string StrKey { get { return TradePlace.TrimUpper().WithRight("@" + Code.TrimUpper()); } }
    //    [XmlIgnore]
        
         public List<Strategy> Strategies { get; set; }
    }
   // [DataContract]
    public class Ticker
    {
        public Ticker()
        {
            Strategies = new List<Strategy>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }

        public int Decimals { get; set; }
        public float MinMove { get; set; }

        public string TradeBoard { get; set; }
        public string Code { get; set; }
        public string BaseContract { get; set; }

        [NotMapped]
        public string StrKey { get { return TradeBoard.TrimUpper().WithRight("@" + Name.TrimUpper()); } }

        public List<Strategy> Strategies { get; set; }
     
    }
    // [DataContract]
    public class Strategy
    {
        public Strategy()
        {
            //Deals = new List<Deal>();
            //Trades = new List<Trade>();
            //Orders = new List<Order>();
        }

        public int Id { get; set; }

        public string Name { get; set; }
        public string Alias { get; set; }

        public string Code { get; set; }

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

        //[XmlIgnore]
        //public List<Deal> Deals { get; set; }
        //[XmlIgnore]
        //public List<Trade> Trades { get; set; }
        //[XmlIgnore]
        //public List<Order> Orders { get; set; }

    }

    public class Trade
    {
        public int Id { get; set; }
        public long Number { get; set; }

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

        //[ForeignKey("StrategyId")]
        //public virtual Strategy Strategy { get; set; }
        //public int StrategyId { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public long Number { get; set; }


        //[NotMapped]
        //public string StrKey { get { return Strategy.Account.StrKey + Number; } }

        //[NotMapped]
        //public string TradeKey { get { return Strategy.TradeKey; } }

        //[ForeignKey("StrategyId")]
        //public virtual Strategy Strategy { get; set; }
        //public int StrategyId { get; set; }
    }

    public class Deal
    {
        public int Id { get; set; }
        public long Number { get; set; }
        public decimal Price1 { get; set; }
        public decimal Price2 { get; set; }
        public long FirstTradeNumber { get; set; }
        public long LastTradeNumber { get; set; }


        //[NotMapped]
        //public string StrKey { get { return Strategy.Account.StrKey + Number; } }

        //[NotMapped]
        //public string TradeKey { get { return Strategy.TradeKey; } }
       
        //[ForeignKey("StrategyId")]
        //public virtual Strategy Strategy { get; set; }
        //public int StrategyId { get; set; }

    }
}
