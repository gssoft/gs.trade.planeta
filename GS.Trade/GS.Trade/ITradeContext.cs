using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Configurations;
using GS.Elements;
using GS.Events;
using GS.Interfaces;
// using GS.Interfaces.Configurations;
using GS.Process;
// using GS.Trade.Data;
using GS.Time.TimePlan;
//using GS.Trade.Trades;
//using GS.Trade.Trades.Time;
using TimePlanEventArgs = GS.Time.TimePlan.TimePlanEventArgs;

namespace GS.Trade.Interfaces
{
    public interface ITradeContext : IElement1<string>
    {
        //event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        //void SendExceptionMessage3(string source, string objtype, string operation, string objstr, Exception e);

        //void FireChangedEvent(string category, string entity, string operation, object o);

        void Init();
        void Open();
        void Close();
        void Start();
        void Stop();

        ITradeWebClient TradeWebClient { get; }

        void OpenChart();

        //IEventLog EventLog { get; set; }

        ITradeTerminals TradeTerminals { get; }

        IOrders Orders { get; }
        IOrders SimulateOrders { get; }

        ITrades Trades { get;}
        IPositions Positions { get;}
       // TimePlans TimePlans { get; }

       // GS.Trade.Strategies.Portfolio.IPortfolios Portfolios { get; set; }

        //bool IsLongEnabled { get; }
        //bool IsShortEnabled { get; }

        void CloseStrategies();
        void ClearOrderCollection();
        void EnableEntries();
        void DisableEntries();

        void SetStrategiesLongShortEnabled(bool longenabled, bool shortenabled);

        ProcessManager2 StrategyProcess {get;}
        ProcessManager2 UIProcess { get; }

        // GS.Trade.Data

        ITickers Tickers { get; }
        IEnumerable<IStrategy> StrategyCollection { get; }

        ITickers GetTickers { get; }
        ITicker  RegisterTicker(string tickerKey);
        ITicker RegisterTicker(string tradeboard, string tickerKey);
        //ITicker RegisterTicker2(string tickerBoard, string tickerCode);
        ITimeSeries RegisterTimeSeries(ITimeSeries ts);

        IAccount GetAccount(string key);
        //IAccount GetAccount2(string key);
        IAccount RegisterAccount(string key);

        ITicker GetTicker(string key);

        IStrategies GetStrategies { get; }

        IEventHub EventHub { get; }

       // ITradeStorage Storage { get; }
        ITradeStorage TradeStorage { get; }

        IConfigurationResourse2 ConfigurationResourse { get; }
        IConfigurationResourse21 ConfigurationResourse1 { get; }
        ITradeTerminal RegisterTradeTerminal(string tradeTerminalType, string tradeTerminalKey);
        IPosition RegisterPosition(string account, string strategy, ITicker t);
        IPosition2 RegisterPosition(IStrategy s);

        IPortfolio RegisterPortfolio(string code, string name);
        IEntryManager RegisterEntryManager(string code, string name);

        ITrade3 Resolve(ITrade3 t);
        IOrder3 Resolve(IOrder3 o);
        IStrategy GetStrategyByKey(string key);

        IStrategy RegisterDefaultStrategy(
            string name, string code,
            string accountKey, string tickerBoard, string tickerKey,
            uint timeInt,
            string terminalType, string terminalKey);

        void Save(IOrder3 o);
        void Save(ITrade3 t);

        #region Publish Entities
        void Publish(IOrder3 o);
        void Publish(ITrade3 t);
        void Publish(IPosition2 p);
        void Publish(IPositionTotal2 p);
        void Publish(IDeal d);

        #endregion
        IDeals BuildDeals();

        IEnumerable<IPosition2> GetPositionCurrents();
        IEnumerable<IPosition2> GetPositionTotals();
        IEnumerable<IPosition2> GetDeals();

            // GS.Trade.Trades
      //  TimePlan RegisterTimePlanStatusChangedEventHandler(string timePlanKey, TimePlan.TimeStatusIsChangedEventHandler a);
        ITimePlan RegisterTimePlanEventHandler(string timePlanKey, EventHandler<ITimePlanEventArgs> a);

        void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask);
        //ITrOrders Orders { get; }
        //ITrades Trades { get; }
        
        //void Evlm(EvlResult result, EvlSubject subject,
        //          string source, string operation, string description, string obj);

        //void Evlm(EvlResult result, EvlSubject subject,
        //          string source, string entity, string operation, string description, string obj);
        void SetWorkingStatus(bool b);
        void SetPortfolioMaxSideSize(int maxsidesize);
    }
}
