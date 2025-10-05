using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;

namespace GS.Trade
{
    //public interface ITradeTerminal64 : IElement1<string>
    //{
    //    // void Init();
    //    // void SetEventLog(IEventLog evl);

    //    event EventHandler<Events.IEventArgs> TradeEntityChangedEvent;

    //    //string Code { get; set; }
    //    //string Name { get; set; }
    //    TradeTerminalType Type { get; }
    //    string ShortName { get; }
    //    void Start();
    //    void Stop();

    //    bool IsWellConnected { get; }
    //    bool IsConnectedNow { get; }

    //    bool Connect();
    //    bool DisConnect();
    //    int IsConnected();

    //    long SetMarketOrder(
    //            string account, string strategy, string tickercategory, string tickercode,
    //            OrderOperationEnum operation,
    //            double price, long quantity);

    //    long SetLimitOrder(IOrder3 o);

    //    long SetLimitOrder(
    //        string account, string strategy, string tickercategory, string tickercode,
    //        OrderOperationEnum operation,
    //        double price, long quantity,
    //        DateTime expireDateTime, string comment);

    //    long SetLimitOrder(IStrategy stra,
    //        string account, string strategy, string tickercategory, string tickercode,
    //        OrderOperationEnum operation,
    //        double price, string priceStr, long quantity,
    //        DateTime expireDateTime, string comment);

    //    long SetLimitOrderInQueue(
    //        string account, string strategy, string tickercategory, string tickercode,
    //        OrderOperationEnum operation,
    //        double price, string priceStr, long quantity,
    //        DateTime expireDateTime, string comment);


    //    long SetMarketOrLimitOrder(string account, string strategy, string tickercategory, string tickercode,
    //                                    OrderTypeEnum ordertype, OrderOperationEnum operation,
    //                                        double price, long quantity,
    //                                            DateTime expireDateTime, string comment);
    //    long SetStopLimitOrder(string account, string strategy, string tickercategory, string tickercode,
    //            OrderOperationEnum operation,
    //            double stopprice, double price, long quantity,
    //            DateTime expireDateTime, string comment);

    //    long SetMarketOrLimitWithStopLimit(string Account, string StratName, string ClassCode, string Code,
    //                                                              OrderTypeEnum ordertype, OrderOperationEnum operation,
    //                                                              double entryPrice, double stopPrice, double limitPrice,
    //                                                              long Quantity,
    //                                                              ref long limitNumber, ref long stopLimitNumber);

    //    long SetStopLimitWithLinkedProfitOrder(string Account, string StratName, string ClassCode, string Code,
    //                                                          OrderOperationEnum operation,
    //                                                          double stopPrice, double limitPrice, double takeProfit,
    //                                                          long Quantity, DateTime expire, string comment);

    //    int SetKillLimitOrderInQueue(string classcode, string seccode, ulong orderkey);
    //    int KillLimitOrder(IOrder3 o);
    //    int KillLimitOrder(IOrder3 o, IStrategy stra, string tickercategory, string tickercode, ulong orderkey);
    //    // ******************** 23.10.04 ********************************************
    //    int KillLimitOrderByTransID(string tickercategory, string tickercode, ulong orderTransId);

    //    bool KillStopOrder(string tickercategory, string tickercode, ulong orderkey);
    //    bool KillAllOrders(IStrategy stra, string strategy, string account, string tickercategory, string tickercode,
    //                            OrderTypeEnum ordtype, OrderOperationEnum operation);

    //    bool KillAllFuturesLimitOrders(string account, string tickercategory, string tickercode, string basecontract);

    //    void NewTick(DateTime dt, string tickerkey, double value);
    //    string ToString();

    //    void SendOrdersFromQueue();
    //    void SendOrderTransactionsFromQueue();
    //    void SendOrderTransactionsFromQueue3();
    //    void SendOrderTransactionsFromBlQueue();

    //    IOrder3 GetOrderByKey(string key);
    //    IEnumerable<IOrder3> GetActiveOrders();
    //    IEnumerable<IOrder3> GetStartegyActiveOrders(string stratkey);

    //    //void OrderProcess2();  // Ищет Стратегии для (Ореров без Стратегий) in Storage;
    //    //void TradeProcess3();  // Ищет для сделок ордера со Стратегией, чтобы привязаться к Стратегии в Core Orders
    //}
}
