using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Xml.Serialization;
using GS.Containers;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Trade.Trades;

namespace GS.Trade.Strategies.Portfolio
{
    //public class Portfolios : SetContainer<string, IPortfolio>
    //{
    //    public IPortfolio Register(IPortfolio p)
    //    {
    //        return AddNew(p);
    //    }
    //}
    //public class Portfolio : ListContainer<string, IStrategy>, IPortfolio
    //{
    //    public string Code { get; set; }
    //    public string Name { get; set; }

    //    private int _position;
    //    public int Position
    //    {
    //        get
    //        {
    //            return ItemCollection.Aggregate(0, (current, s) => (int) (current + (((IStrategy)s).Position.Pos)));
    //        }
    //    }

    //    public  string Key
    //    {
    //        get { return Code; }
    //    }

    //    public IStrategy Register(IStrategy strategy)
    //    {
    //        return AddNew(strategy);
    //    }

    //    public bool IsShort { get { return Position < 0; } }
    //    public bool IsLong { get { return Position > 0; } }
    //    public bool IsNeutral { get { return Position == 0; } }

    //    public override string ToString()
    //    {
    //        return string.Format("[Code={0}; Name={1}; Key={2}]", Code, Name, Key);
    //    }
    //}

    // Move to GS.Trade with Name PortfolioRisk
    //public interface IPortfolio : IElement2<string, IStrategy>, IHaveInit
    //{
    //    //IEnumerable<IStrategy> Collection { get; }
    //    //IEnumerable<IStrategy> Collection { get; }
    //    void Init();
    //    void Refresh();
    //    IPosition2 Position { get; }

    //    bool IsLongEnabled { get; }
    //    bool IsShortEnabled { get; }

    //    long MaxOneSidePositionSize { get; }


    //    float MaxSideInitValue { get; set; }
    //    int MaxSideCount { get; }

    //    long LongsCount { get; }
    //    long ShortsCount { get; }

    //    //int LongRequests { get; set; }
    //    //int ShortRequests { get; set; }
    //}

    public class Portfolio1 : Element2<string, IStrategy, Containers5.ListContainer<string, IStrategy>>, IPortfolioRisk
    {
        [XmlIgnore]
        public IStrategy SellRequestStrategy { get; private set; }
        [XmlIgnore]
        public IStrategy BuyRequestStrategy { get; private set; }
        [XmlIgnore]
        public IStrategy BuySellRequestStrategy { get; private set; }

        private long _maxContracts;
        // private const float MaxOneSideSizePrcnt = 2f/3f;
        public float MaxOneSideSizePrcnt { get; set; }

        public float MaxSideInitValue { get; set; }

        public long MaxOneSidePositionSize =>
            (long) Math.Floor(_maxContracts * MaxOneSideSizePrcnt);

        public int MaxSideCount
        {
            get
            {
                var cnt = Collection.Count;
                return (int) Math.Floor(
                    cnt > 1
                        ? cnt * MaxSideInitValue
                        : 1);
            }
        }
        public override string Key => Code;

        [XmlIgnore]
        public IPosition2 Position { get; private set; }

        //public IEnumerable<IStrategy> Collection 
        //{
        //    get { return Collection.Items; }
        //}

        //public int LongRequests { get; set; }
        //public int ShortRequests { get; set; }

        public long Count => Collection.Count;

        public long LongsCount
        {
            get { return Collection.Items.Count(s => s.Position.IsLong); }
        }

        public long ShortsCount
        {
            get { return Collection.Items.Count(s => s.Position.IsShort); }
        }
        public bool IsBuySellRequestEnabled(IStrategy s)
        {
            if (BuySellRequestStrategy != null)
                return BuySellRequestStrategy.Key.Equals(s.Key, StringComparison.InvariantCultureIgnoreCase);

            BuySellRequestStrategy = s;
            return true;
        }
        public bool IsSellRequestEnabled(IStrategy s)
        {
            if (SellRequestStrategy != null)
                return SellRequestStrategy.Key.Equals(s.Key, StringComparison.InvariantCultureIgnoreCase);

            SellRequestStrategy = s;
            return true;
        }
        public bool IsBuyRequestEnabled(IStrategy s)
        {
            if (BuyRequestStrategy != null)
                return BuyRequestStrategy.Key.Equals(s.Key, StringComparison.InvariantCultureIgnoreCase);

            BuyRequestStrategy = s;
            return true;
        }
        public bool IsSellRequestEnabled()
        {
            return true;
        }
        public bool IsBuyRequestEnabled()
        {
            return true;
        }

        public int BuyRequestCount { get; }
        public int SellRequestCount { get; }
        public long LongRequestsCount => Collection.Items.Count(s => s.LongContractsRequest > 0);
        public long ShortRequestsCount => Collection.Items.Count(s => s.ShortContractsRequest > 0);

        public long LongsAvailable =>
            MaxSideCount - LongsCount - LongRequestsCount > 0
                ? MaxSideCount - LongsCount - LongRequestsCount
                : 0;
        public long ShortsAvailable =>
            MaxSideCount - ShortsCount - ShortRequestsCount > 0
                ? MaxSideCount - ShortsCount - ShortRequestsCount
                : 0;

        public Portfolio1()
        {
            Collection = new Containers5.ListContainer<string, IStrategy>();
            Position = new Position2
            {
                Strategy = null,
                StrategyKeyEx = Code,
                AccountCodeEx = Code,
                TickerCodeEx = Code,
                Status = PosStatusEnum.Closed,
                Operation = PosOperationEnum.Neutral,
                EventLog = EventLog,
                PositionTotal = new PositionTotal2
                {
                    Strategy = null,
                    Status = PosStatusEnum.Closed,
                    Operation = PosOperationEnum.Neutral,
                    EventLog = EventLog
                }
            };
        }
        public override void GetNotifyEvent(Events.IEventArgs ea)
        {
            try
            {
                if (!(ea.Object is ITrade3))
                    throw new ArgumentException("ITrade");

                switch (ea.OperationKey)
                {
                    case "TRADES.TRADE.ADD":
                    case "TRADES.TRADE.ADDNEW":
                        ReCalcPosition(ea.Object as ITrade3);
                        break;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, this.GetType().ToString(), "GetNotifyEvent(ITrade3)", ea.ToString(), e);
                throw;
            }
        }

        public void Refresh()
        {
            ReCalc();
        }

        private void ReCalc()
        {
            var items = Collection.Items;
            var code = Code;

            decimal s1 = 0;
            decimal s2 = 0;
            long k = 0;
            long p = 0;
            var maxNumber = Position.LastTradeNumber;
            var maxDT = Position.LastTradeDT;
            long nLong = 0, nShort = 0;
            decimal sum = 0;
            decimal lastPrice = 0;

            Position.PosPnLFixed = 0;
            Position.DailyPnLFixed = 0;

            var strategies = items as IStrategy[] ?? items.ToArray();
            foreach (var i in strategies)
            {
                s1 += i.Position.Price1*i.Position.Quantity;
                s2 += i.Position.Price2*i.Position.Quantity;
                k += i.Position.Quantity;
                p += i.Position.Pos;
                maxNumber = Position.LastTradeNumber > maxNumber ? Position.LastTradeNumber : maxNumber;
                maxDT = Position.LastTradeDT.IsGreaterThan(maxDT) ? Position.LastTradeDT : maxDT;

                var pr = i.Position.LastPrice.IsEquals(0) ? i.Position.Price2 : i.Position.LastPrice;
                lastPrice = pr;
                //if (i.Position.IsLong)
                //    sum += pr - i.Position.Price1;
                //else if (i.Position.IsShort)
                //    sum += i.Position.Price1 - pr;
                sum += (pr - i.Position.Price1)*i.Position.Pos;

                Position.PosPnLFixed += i.Position.PosPnLFixed;
                Position.DailyPnLFixed += i.Position.DailyPnLFixed;

                //Position.DailyProfitLimit = i.Account != null ? i.Account.DailyProfitLimit : 0m;
            }
            //if (k > 0)
            //{
            //    Position.Price1 = s1/k;
            //    Position.Price2 = s2/k;
            //}
            if (p != 0)
            {
                //var pr = Position.LastPrice.IsEquals(0) ? Position.Price2 : Position.LastPrice;  
                Position.Price1 = lastPrice - sum/p;
                Position.Price2 = lastPrice;
                Position.LastPrice = lastPrice;
            }
            else
            {
                Position.Price1 = 0;
                Position.Price2 = 0;
            }
            Position.Quantity = Math.Abs(p);
            Position.Operation = p == 0
                ? PosOperationEnum.Neutral
                : (p > 0 ? PosOperationEnum.Long : PosOperationEnum.Short);
            Position.Status = p == 0 ? PosStatusEnum.Closed : PosStatusEnum.Opened;

            Position.LastTradeDT = maxDT;
            Position.LastTradeNumber = maxNumber;

            Position.StrategyKeyEx = Code;
            var ar = Code.Split('@');
            Position.AccountCodeEx = ar[0];
            Position.TickerCodeEx = ar[1];

            //Position.Comment = String.Format(
            //    "Longs: {4} Rqsts: {6} {0} {7}; "+
            //    "Shorts: {5} Rqsts: {8} {1} {9}; MaxSideCnt: {2}; MaxSideSize: {3}",
            //    IsLongEnabled ? "Enabled:" : "Disabled", IsShortEnabled ? "Enabled:" : "Disabled",
            //    MaxSideCount, MaxOneSidePositionSize, LongsCount, ShortsCount, 
            //    LongRequestsCount, LongsAvailable, ShortRequestsCount, ShortsAvailable );

            Position.Comment = $"Longs: {LongsCount} Available: {LongsAvailable}; " +
                               $"Shorts: {ShortsCount} Available: {ShortsAvailable}; " +
                               $"MaxSideCnt: {MaxSideCount}; MaxSideSize: {MaxOneSidePositionSize}; " +
                               $"Count: {Count}";

            FireChangedEvent("UI.Portfolio", "Position", "AddOrUpdate", Position);

            DailyProfitLimitReached();
            CheckLimits();
        }
        public string ShortDescription => Position?.Comment ?? "Portfolio POsition is Null";
        public string BuyRequestShortInfo => "";//$"BuysAvailable:{BuysAvailable} MaxSideSize:{MaxSideSize} PortfPos:{PortfolioPosition} BuyRqst:{BuyRequestCount}";
        public string SellRequestShortInfo => "";
        public void SetMaxSideSize(int maxsidesize)
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            MaxSideSize = maxsidesize;
            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, Code, "MaxSideSize",
                method, $"MaxSideSize:{MaxSideSize}", ToString());
        }
        public int MaxSideSize { get; set; }
//$"SellsAvailable:{SellsAvailable} MaxSideSize:{MaxSideSize} PortfPos:{PortfolioPosition} SellRqst:{SellRequestCount}";
        private void ReCalcPosition(ITrade3 t)
        {
            //var items = Collection.Items;

            //decimal s1 = 0;
            //decimal s2 = 0;
            //long k = 0;
            //long p = 0;
            //foreach(var i in items)
            //{
            //    s1 += i.Position.Price1 * i.Position.Quantity;
            //    s2 += i.Position.Price2 * i.Position.Quantity;
            //    k += i.Position.Quantity;
            //    p += i.Position.Pos;
            //}

            //Position.Price1 = s1/k;
            //Position.Price2 = s2/k;
            //Position.Quantity = k;
            //Position.Operation =    p == 0 
            //                        ? PosOperationEnum.Neutral : 
            //                        (p > 0 ? PosOperationEnum.Long : PosOperationEnum.Short);

            //Position.StrategyKeyEx = Code;
            //Position.LastTradeDT = t.DT;
            //Position.LastTradeNumber = t.Number;
        }

        // 15.09.24
        // 19.04.27 override
        public override void Init()
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", Code, "Init()", "", ToString());
            foreach (var i in Collection.Items)
            {
                _maxContracts += i.Contracts;
                Position.DailyProfitLimit = i.Account?.DailyProfitLimit ?? 0m;
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Portfolio", Code, "Init()",
                $"MaxContractsToOpen: {_maxContracts}; DailyProfitLimit: {Position.DailyProfitLimit}; MaxSideCnt: {MaxSideCount}; MaxSideSize: {MaxOneSidePositionSize}",
                ToString());
        }

        public bool IsLongEnabled
        {
            get
            {
                var items = Collection.Items.ToList();
                //return Position != null && (Position.Quantity < MaxOneSidePositionSize || Position.IsShort);
                //return Collection.Items.Where(s => s.Position.IsLong).Sum(s=>s.Position.Quantity) < MaxOneSidePositionSize;
                //return Collection.Items.Count(s => s.Position.IsLong) < MaxSideCount;
                //return Collection.Items.Count(s => s.Position.IsLong) + LongRequests < MaxSideCount;
                return items.Count(s => s.Position.IsLong) +
                       items.Count(s => s.LongContractsRequest > 0) <= MaxSideCount;
            }
        }
        public bool IsShortEnabled
        {
            get
            {
                var items = Collection.Items.ToList();
                // return Position != null && (Position.Quantity < MaxOneSidePositionSize || Position.IsLong);
                // return Collection.Items.Where(s => s.Position.IsShort).Sum(s => s.Position.Quantity) < MaxOneSidePositionSize;
                // return Collection.Items.Count(s => s.Position.IsShort) < MaxSideCount;
                // return Collection.Items.Count(s => s.Position.IsShort) + ShortRequests < MaxSideCount;
                return items.Count(s => s.Position.IsShort) +
                       items.Count(s => s.ShortContractsRequest > 0) <= MaxSideCount;
            }
        }

        //public bool LongRequest()
        //{
        //    if (!IsLongEnabled)
        //        return false;
        //    ++LongRequests;
        //    return true;
        //}
        //public bool ShortRequest()
        //{
        //    if (!IsShortEnabled)
        //        return false;
        //    ++ShortRequests;
        //    return true;
        //}
        //private IStrategy GetMinSizeStrategy(int longshort)
        //{
        //    var str = Collection.Items
        //        .Where(s => longshort > 0 ? s.Position.IsLong : s.Position.IsShort)
        //        .OrderBy(s => s.Position.Quantity)
        //        .FirstOrDefault();
        //    if(str!=null)
        //        str.SetExitMode(12, "Portfolio:" + Code + "Position Limits is Exeeded");
        //    return str;
        //}
        /// <summary>
        /// For Portfolio AT Only
        /// </summary>
        /// <returns></returns>
        private bool DailyProfitLimitReached()
        {
            if (Name != "Portfolio.AT")
                return false;
            var strategies = Collection.Items;
            if (Position.IsDailyProfitLimitReached)
            {
                foreach (var s in strategies)
                {
                    s.SetExitMode(12, Code + ": Profit is Reached");
                }
                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "Portfolio", Code, "Close All()", "Daily Profit is Reached",
                    Position.ToString());
                return true;
            }
            if (Position.IsDailyLossLimitReached)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// ForPortfolio.ATT Only
        /// </summary>
        /// <returns></returns>
        private bool CheckLimits()
        {
            // Only ATT Portfolio
            if (Name != "Portfolio.ATT")
                return true;

            if (LongsCount > MaxSideCount)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, "Portfolio", Code, "CheckLimits()", "Longs Limit Exceeded",
                    "Longs: " + LongsCount + " LongsLimit: " + MaxSideCount);
                // Get Min Position Size Strategy

                var str = Collection.Items
                    .Where(s => s.Position.IsLong)
                    .OrderBy(s => s.Position.Quantity)
                    .FirstOrDefault();

                str?.SetExitModeFromPortfolio(12, "Portfolio:" + Code + "Longs Limits is Exeeded");

                // throw new NullReferenceException("Longs Limit Exceeded." + "Longs: " + LongsCount + " LongsLimit: " + MaxSideCount);
                //return false;
            }

            if (ShortsCount > MaxSideCount)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, "Portfolio", Code, "CheckLimits()",
                    "Shorts Limit Exceeded",
                    "Shorts: " + ShortsCount + " ShortsLimit: " + MaxSideCount);

                var str = Collection.Items
                    .Where(s => s.Position.IsShort)
                    .OrderBy(s => s.Position.Quantity)
                    .FirstOrDefault();

                str?.SetExitModeFromPortfolio(12, "Portfolio:" + Code + "Shorts Limits is Exeeded");
                //throw new NullReferenceException("Shorts Limit Exceeded." + "Shorts: " + ShortsCount + " ShortsLimit: " + MaxSideCount);
                // SendExceptionMessage3(new NullReferenceException("Shorts Limit Exceeded"));
                // return false;
            }
            if (ShortsCount <= MaxSideCount && LongsCount <= MaxSideCount)
            {
                //Evlm2(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, "Portfolio", Code, "CheckLimits()", "All Limit is Well",
                //     "Longs: " + LongsCount + " LongsLimit: " + MaxSideCount + 
                //     " Shorts: " + ShortsCount + " ShortsLimit: " + MaxSideCount);

                foreach (var s in Collection.Items.Where(s => s.EntryPortfolioEnabled == false))
                {
                    s.EntryPortfolioEnabled = true;
                    Evlm2(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, "Portfolio", Code, "CheckLimits()",
                        "Return EntryPortfolioEnable Success Status", s.ToString());
                }
                //return true;
            }
            return true;
        }
        public void ClearSideRequests()
        {
            BuyRequestStrategy = null;
            SellRequestStrategy = null;
        }
        public void ClearBuySellRequest(IStrategy s)
        {
            if (BuySellRequestStrategy != null &&
                BuySellRequestStrategy.Key.Equals(s.Key, StringComparison.CurrentCultureIgnoreCase))
                    BuySellRequestStrategy = null;
        }
        public void ClearBuyRequest(IStrategy s)
        {
            if(BuyRequestStrategy != null &&
               BuyRequestStrategy.Key.Equals(s.Key, StringComparison.CurrentCultureIgnoreCase))
                    BuyRequestStrategy = null;
        }
        public void ClearSellRequest(IStrategy s)
        {
            if (SellRequestStrategy != null && 
                SellRequestStrategy.Key.Equals(s.Key, StringComparison.CurrentCultureIgnoreCase))
                    SellRequestStrategy = null;
        }
        public void SkipTheTickToOthers(string key)
        {
            var stratToSkip = Items.Where(s => s.Key != key && s.Position.IsOpened);
            foreach (var s in stratToSkip)
                s.SkipTheTick(0);
        }
        public void SkipTheTickToOthers(IStrategy str)
        {
            var stratToSkip = Items
                .Where(s => s.Key != str.Key && 
                            s.Position.IsOpened && 
                            s.Position.Operation == str.Position.Operation);

            foreach (var s in stratToSkip)
                s.SkipTheTick(0);
        }
        public void SkipTheTickToOthers(IStrategy str, int skipCount)
        {
            var stratToSkip = Items
                .Where(s => s.Key != str.Key &&
                            s.Position.IsOpened &&
                            s.Position.Operation == str.Position.Operation);

            foreach (var s in stratToSkip)
                s.SkipTheTick(skipCount);
        }
        public void SkipTheTickToOthers(IStrategy str, int skipCount, bool isOpened, bool isSameSide)
        {
            IEnumerable<IStrategy> strats;
            if( isOpened && isSameSide)
                strats = Items
                    .Where(s => s.Key != str.Key &&
                            s.Position.IsOpened &&
                            s.Position.Operation == str.Position.Operation);
            else if (isOpened)
            {
                strats = Items
                    .Where(s => s.Key != str.Key && s.Position.IsOpened);
            }
            else if (isSameSide)
            {
                strats = Items
                    .Where(s => s.Key != str.Key && s.Position.Operation == str.Position.Operation);
            }
            else
            {
                strats = Items.Where(s => s.Key != str.Key);
            }
            foreach (var s in strats)
                s.SkipTheTick(skipCount);
        }
    }
}
