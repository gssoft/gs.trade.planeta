using System;
using System.Security.Cryptography.X509Certificates;
using GS.Interfaces;
using GS.Queues;

namespace GS.Trade.TradeTerminals64.Quik
{
        public interface IQuikTradeTerminal :  IHaveQueue, INeedOrderResolve 
        {
            //event EventHandler<Events.EventArgs> TransactionEvent;
            //event EventHandler<Events.EventArgs> OrderEvent;
            //event EventHandler<Events.EventArgs> TradeEvent;

            event EventHandler<Events.IEventArgs> TradeEntityChangedEvent;
            string Path2Quik { get; }
            string DllNamePath2QuikPair { get; }

            bool IsConnectedNow { get; }
            bool IsWellConnected { get; }

            bool Connect();
            bool DisConnect();
            int IsConnected();
           // string IsConnectedResult();
            bool SetConnectionStatusCallback(ConnectionStatusCallback64 conectionStatusCallback);
            long SendSyncTransaction(string transactionStr);
            bool SetTransactionReplyCallback(TransactionReplyCallback cb);
            bool SendAsyncTransaction(string transactionString);
            bool SendAsyncTransaction(IQuikTransaction t);

            bool SubscribeTrades(string classCode, string securityCode);
            bool UnSubscribeTrades();
         //   bool StartTradesListenProcess(TradeStatusCallback tradeStatusCallback);
         //   bool StartTradesListenProcess(ITradeStatusReceiver tradesStatusReceiver);

            bool SubscribeOrders(string classCode, string securityCode);
            bool UnSubscribeOrders();

            //IOrder3 GetOrderByKey(string key);

            //   bool StartOrdersListenProcess(OrderStatusCallback orderStatusCallback);
            //   bool StartOrdersListenProcess(IOrderStatusReceiver ordersStatusReceiver);
            /*
            void NewOrderStatus(double dNumber,
                    Int32 iDate, Int32 iTime,
                    Int32 iMode,
                    Int32 iActivationTime, Int32 iCancelTime, Int32 iExpireDate,
                    string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
                    Int32 iIsSell, Int32 iQty, Int32 nBalance, double dPrice,
                    Int32 iStatus, UInt32 dwTransId,
                    string sClientCode);
            void NewTrade(double dNumber,
                    Int32 iDate, Int32 iTime, Int32 iMode,
                    string sAcc, string sStrat, string sClassCode, string sSecCode,
                    Int32 nIsSell, Int32 iQty, double dPrice,
                    string sClientCode,
                    double dOrderNumber, double dCommissionTs);

            void NewConnectionStatus(Int32 nConnectionEvent, Int32 nExtendedErrorCode, string message);
            */
            /*
            long SetMarketOrLimtOrder( string account, string strategy, string classcode, string seccode,
                QuikTradeTerminal.OrderType ordertype, QuikTradeTerminal.OrderOperation operation,
                double price, long quantity);

            int SetStopLimitOrder( string account, string strategy, string classcode, string seccode,
                QuikTradeTerminal.OrderOperation operation,
                double stopprice, double price,
                long quantity);

            int SetStopLimitWithLinkedProfitOrder(
                        string account, string strategy, string classcode, string seccode,
                        QuikTradeTerminal.OrderOperation operation,
                        double stopprice, double price, double takeprofit,
                        long quantity);

            int SetMarketOrLimitWithStopLimit(string account, string strategy, string classcode, string seccode,
                QuikTradeTerminal.OrderType ordertype, QuikTradeTerminal.OrderOperation operation,
                double price, double stopprice, double limitprice,
                long quantity,
                ref long entryOrderNumber, ref long stopOrderNumber);

            int KillOrder(string classcode, string seccode, long orderkey);
            bool KillStopOrder(string classcode, string seccode, long orderkey);

            int KillAllFuturesOrders(string account, string classcode, string seccode, string basecontract);
            int KillAllStopOrders(string account, string classcode, string seccode, string basecontract);

            void SetEventLog(IEventLog eventlog );
         */
        } ;

        #region IReceivers Interfaces
        /*
        public interface IReceiver { }

        public interface IConnectionStatusReceiver : IReceiver
        {
            //void EventLogItemAdd(string trans);
            void NewConnectionStatus(string newconstatus);
        }
        public interface ITransactionReplyReceiver : IReceiver
        {
            //void EventLogItemAdd(string trans);
            void TransactionReply(int result, UInt32 transId, long orderNumber);
            void NewOrder(ulong transId, long orderNumber);
            void NewTransaction(string trans);
        }
        public interface IOrdersStatusReceiver : IReceiver
        {
            //void EventLogItemAdd(string trans);
            void TransactionReply(int result, UInt32 transId, long orderNumber);
            
            void RegisterOrder(long number, DateTime dt, long transId,
                                      string account, string strategy, string ticker,
                                      OperationEnum operation, OrderTypeEnum ordertype,
                                      double stopprice, double limitprice, long quantity, long rest,
                                      string comment);
            
            void NewOrderStatus(string order);
            void NewOrderStatus(    double dNumber, int iDate, int iTime, 
                                    string sAcc, string sStrat, string sSecCode, 
                                    int iIsSell, int iQty, int iRest, double dPrice, int iStatus, uint dwTransId);

            void NewOrderStatus(    double dNumber, int iDate, int iTime, int nMode,
                                    int iActivationTime, int iCancelTime, int iExpireDate,
                                    string sAcc, string sStrat, string sClassCode, string sSecCode,
                                    int iIsSell, int iQty, int iRest, double dPrice, int iStatus, uint dwTransId, string comment);
        }
        public interface ITradesStatusReceiver : IReceiver
        {
            //void EventLogItemAdd(string trans);
            void NewTrade(string trade);
            void NewTrade(double dNumber, int iDate, int iTime, int nMode,
                                    string sAcc, string sStrat, string sClassCode, string sSecCode,
                                    int iIsSell, int iQty, double dPrice, string sComment, double orderNumber, 
                                    double dCommissionTs);
        }
        */ 
        #endregion}
}
