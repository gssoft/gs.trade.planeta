using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Events;
using GS.Interfaces;
using GS.Process;
// using GS.Trade.Data;
using GS.Time.TimePlan;
using GS.Trade.Trades;
using GS.Trade.Trades.Time;
using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;

namespace GS.Trade.Interfaces
{
    //public interface ITradeContext
    //{
    //    IEventLog EventLog { get; }
        
    //    Orders Orders { get; }
    //    Orders SimulateOrders { get; }

    //    Trades.Trades Trades { get;}
    //    Positions Positions { get;}
    //   // TimePlans TimePlans { get; }

    //    ProcessManager2 StrategyProcess {get;}
    //    ProcessManager2 UIProcess { get; }

    //    // GS.Trade.Data

    //    ITickers GetTickers { get; }
    //    ITicker  RegisterTicker(string tickerKey);
    //    ITimeSeries RegisterTimeSeries(ITimeSeries ts);

    //    IAccount GetAccount(string key);

    //    IStrategies GetStrategies { get; }

    //    IEventHub EventHub { get; }

    //    ITradeTerminal RegisterTradeTerminal(string tradeTerminalType, string tradeTerminalKey);
    //    Position RegisterPosition(string account, string strategy, ITicker t);
    //    IPortfolio RegisterPortfolio(string code, string name);
    //    IEntryManager RegisterEntryManager(string code, string name);

    //    ITrade3 Resolve(ITrade3 t);
    //    IOrder Resolve(IOrder o);
    //    IStrategy GetStrategyByKey(string key);

    //    IStrategy RegisterDefaultStrategy(
    //        string name, string code,
    //        string accountKey, string tickerKey, uint timeInt,
    //        string terminalType, string terminalKey);

    //    void Save(IOrder o);
    //    void Save(ITrade3 t);

    //    // GS.Trade.Trades
    //  //  TimePlan RegisterTimePlanStatusChangedEventHandler(string timePlanKey, TimePlan.TimeStatusIsChangedEventHandler a);
    //    TimePlan RegisterTimePlanEventHandler(string timePlanKey, EventHandler<TimePlanEventArgs> a);

    //    //ITrOrders Orders { get; }
    //    //ITrades Trades { get; }
    //    void Evlm(EvlResult result, EvlSubject subject,
    //              string source, string operation, string description, string obj);
    //    void Evlm(EvlResult result, EvlSubject subject,
    //              string source, string entity, string operation, string description, string obj);
    //}
}
