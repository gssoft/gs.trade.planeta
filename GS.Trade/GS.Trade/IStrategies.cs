using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using GS.Elements;
using GS.Events;
using GS.Trade.Interfaces;

namespace GS.Trade
{
    public interface IStrategies : IElement1<string>
    {
        void Init();
        void CloseAll();
        void CloseAllGently();
        void EnableEntry();
        void DisableEntry();

        // void SetParent(IStrategies s);

        ITradeContext TradeContext { get; set; }

        IStrategy GetByKey(string key);

        IEnumerable<IStrategy> StrategyCollection { get; }
        void OnStrategiesEvent(Events.EventArgs e);

        //event EventHandler<GS.Events.IEventArgs> ExceptionEvent;
        //void OnExceptionEvent(IEventArgs e);

        event EventHandler<Events.EventArgs> StrategyTradeEntityChangedEvent;
        void OnStrategyTradeEntityChangedEvent(Events.EventArgs e);

        void UpdateLastPrice();
        void Close();
        void CloseAllSoft();

        IEnumerable<IPosition2> GetPositionCurrents();
        IEnumerable<IPosition2> GetPositionTotals();
        IEnumerable<IPosition2> GetDeals();

        IStrategy RegisterDefaultStrategy(string name, string code,
                                           IAccount a, ITicker t, uint timeInt,
                                           string terminalType, string terminalKey);

        IStrategy RegisterDefaultStrategy(string name, string code,
                                            string account, string tradeboard, string tickercode,
                                            uint timeInt, string terminalType, string terminalKey);

        void SetWorkingStatus(bool status);
        void ClearOrderCollection();

        IStrategy GetStrategy(string code, string accountcode,
                                string tickerboard, string tickercode, int timeint);
        IStrategy GetDefaultStrategy(IOrder3 order);
        void SetStrategiesLongShortEnabled(bool longenabled, bool shortenabled);
        void Init(string xmlFile, string xmlElementPath);
    }
}
