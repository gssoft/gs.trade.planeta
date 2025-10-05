using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Extension;

namespace GS.Trade.Trades
{
    public abstract class TradeEntity
    {
        public long Id { get; set; }

        public IStrategy Strategy { get; set; }

        private IAccount _account;
        public IAccount Account
        {
            get { { return Strategy?.Account; } }
            set { _account = value; }
        }
        private ITicker _ticker;
        public ITicker Ticker
        {
            get {  return Strategy?.Ticker;  }
            set { _ticker = value; }
        }
        public int StratTimeInt => Strategy?.TimeInt ?? 0;
        public string StrategyCode => Strategy != null 
            ? Strategy.Code
            : (StrategyCodeEx.HasValue() ? StrategyCodeEx : "Unknown");
        public string StrategyTimeIntTickerString => Strategy != null
            ? Strategy.StrategyTimeIntTickerString
            : (StrategyCodeEx.HasValue() ? StrategyCodeEx : "Unknown");
        public string StrategyKey => Strategy != null 
            ? Strategy.Key
            : (StrategyKeyEx.HasValue() ? StrategyKeyEx : "Unknown");

        public string TickerBoard => Ticker != null ? Ticker.ClassCode : TradeBoardEx;
        public string TickerCode => Ticker != null ? Ticker.Code : TickerCodeEx;
        public string TickerKey => Ticker != null ? Ticker.Key : TickerKeyEx;
        public string AccountCode => Account != null ? Account.Code : AccountCodeEx;
        public string AccountKey => Account != null ? Account.Key : AccountCodeEx;

        public ulong Number { get; set; }
        public DateTime DT { get; set; }

        public string StrategyKeyEx { get; set; }
        public string StrategyCodeEx { get; set; }

        public string AccountCodeEx { get; set; }

        public string TradeBoardEx { get; set; }
        public string TickerCodeEx { get; set; }
        public string TickerKeyEx => (TradeBoardEx.HasValue() ? TradeBoardEx + "@" : "") + TickerCodeEx;
        public string Format { get; set; }
        public string FormatAvg { get; set; }
        public string FormatStr => Ticker != null
            ? Ticker.Format
            : (Format.HasValue()
                ? Format
                : "N2");

        public string FormatAvgStr => Ticker != null
            ? Ticker.FormatAvg
            : (FormatAvg.HasValue()
                ? FormatAvg
                : "N3");
    }

    public abstract class TradeEntity1 : Element1<string>
    {
        public long Id { get; set; }

        public IStrategy Strategy { get; set; }

        public IAccount Account => Strategy?.Account;
        public ITicker Ticker => Strategy?.Ticker;

        public string StrategyCode => Strategy != null
            ? Strategy.Code
            : (StrategyCodeEx.HasValue() ? StrategyCodeEx : "Unknown");

        public string StrategyKey => Strategy != null
            ? Strategy.Key
            : (StrategyKeyEx.HasValue() ? StrategyKeyEx : "Unknown");

        public string TickerBoard => Ticker != null ? Ticker.ClassCode : TradeBoardEx;
        public string TickerCode => Ticker != null ? Ticker.Code : TickerCodeEx;
        public string TickerKey => Ticker != null ? Ticker.Key : TickerKeyEx;

        public string AccountCode => Account != null ? Account.Code : AccountCodeEx;
        public string AccountKey => Account != null ? Account.Key : AccountCodeEx;

        public long Number { get; set; }
        public DateTime DT { get; set; }

        public string StrategyKeyEx { get; set; }
        public string StrategyCodeEx { get; set; }

        public string AccountCodeEx { get; set; }

        public string TradeBoardEx { get; set; }
        public string TickerCodeEx { get; set; }
        public string TickerKeyEx => TradeBoardEx + "@" + TickerCodeEx;
    }
}
