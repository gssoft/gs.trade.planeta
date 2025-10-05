using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using GS.Containers;
using GS.Elements;
using GS.Events;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Interfaces;

namespace GS.Trade
{
    public interface IStrategyBase : Containers5.IHaveKey<string>, IHaveId<int> //, IContainerItem<string>
    {
        //string Name { get; set; }
        //string Code { get; set; }
        //string Alias { get; set; }
    }

    public interface IStrategy :  IStrategyBase, IElement1<string>, IHaveInit
    {
        long Contracts { get; }
        string StrategyTickerString { get; }
        string StrategyTimeIntTickerString { get; }
        IStrategies Strategies { get; set; }

        IAccount Account { get; }
        ITicker Ticker { get; }

        string TradeAccountCode { get; }
        string TickerTradeBoard { get; }
        string TickerCode { get; }

        int TimeInt { get; set; }
        string TimeIntKey { get; }

        IEnumerable<IOrder3> ActiveOrders { get; }

        float Volatility { get; }

        IPosition2 Position { get; }
        IPosition2 PositionTotal { get; set; }

        bool ShortEnabled { get; set; }
        bool LongEnabled { get; set; }

        float LongEntryLevel { get; set; }
        float ShortEntryLevel { get; set; }

        void SkipTheTick(int n);
        void StartNewDayInit();

        bool IsTradeNumberValid(ulong tradeNumber);
        int RegisterNewTrade(ITrade t);
        int NewTrade(ITrade3 t);

        IEnumerable<IPosition2> GetDeals(); 

        void AddDeal(IPosition2 p);
        void PositionChanged(IPosition2 oldpos, IPosition2 newpos, PositionChangedEnum changedResult);

        void UpdateFromLastTick();

        void SetExitMode(int mode, string reason);
        void SetExitModeFromPortfolio(int mode, string reason);
        // Portfolio
        int LongSideRequest { get; }
        int ShortSideRequest { get; }

        int LongContractsRequest { get; }
        int ShortContractsRequest { get; }

        int BuyContractsRequest { get; }
        int SellContractsRequest { get; }

        bool EntryEnabled { get; set; }
        bool EntryPortfolioEnabled { get; set; }
        
        // 23.09.26
        void SetParent(IStrategies ss);
        void SetTradeContext(ITradeContext tx);
        new void Init(IEventLog evl);
        void Register(IStrategy s);

        string StrategyKey { get; }

        // --------------------------------
        void Finish();
        void CloseAll(int mode);
        int CloseAllSoft();
        string RealTickerKey { get; }
        string RealAccountKey { get; }
        string TickerKey { get; }
        string TradeAccountKey { get; }
        string TickerBoard { get; }

        bool PortfolioEnable { get; }
        string PortfolioKey { get; }

        IPortfolioRisk PortfolioRisk { get; set; }

        // 15.11.11
        string PositionInfo { get; }
        IChartDataContainer ChartDataContainer { get; }

        ITradeTerminal TradeTerminal { get; }

        // bool Working { get; set; }

        void SetWorkingStatus(bool status, string reason);
        void EnQueue(object sender, IEventArgs args);
        void FireOrderChangedEventToStrategy(IOrder3 order, string category, string entity, string operation);

        bool IsHedger { get; }
        void StartLongHedger();
        void StartShortHedger();
        void StopHedger();

        void SetLongShortEnabled(bool longenabled, bool shortenabled);

        void ClearOrderCollection();

        void CreateStat();
        decimal DailyMaxProfit { get;}
        decimal DailyMaxLoss { get; }
        DateTime DailyMaxProfitDT { get; }
        DateTime DailyMaxLossDT { get; }
    }

    public interface IStrategyDb : IStrategyBase
    {
        string Name { get; set; }
        string Code { get; set; }
        string Alias { get; set; }
    }
}
