using System;
using GS.Trade.TradeTerminals64.Quik;

namespace GS.Trade.TradeTerminals64.Quik
{
    // x86
        //public interface ITrans2Quik
        //{
        //    string GetTrans2QuikDllName { get; }

        //    Int32 Connect2Quik(string path2Quik);
        //    Int32 Disconnect();
        //    Int32 IsConnected2Quik();
        //    Int32 IsConnected2Server();

        //    void SetT2QReceiver(ITrans2QuikReceiver receiver);
        //    void Init();

        ////Int32 SetConnectionStatusCallback(ConnectionStatusCallback callback);
        //    long SetConnectionStatusCallback(ConnectionStatusCallback64 callback);
        //    Int32 SendSyncTransaction(string transactionStr, ref double orderNum);
        //    Int32 SetTransactionReplyCallback(TransactionReplyCallback cb);
        //    Int32 SendAsyncTransaction(string transactionString);
        //    Int32 SendAsyncTransaction(string transactionString, out int extendedErrorCode);

        //    long SubscribeOrders(string classCode, string securityCode);
        //    long UnSubscribeOrders();
        //    void StartOrders();
        // //   Int32 StartOrdersListenProcess(OrderStatusCallback orderStatusCallback);
        // //   Int32 StartOrdersListenProcess(IOrderStatusReceiver orderStatusReceiver);

        //    long SubscribeTrades(string classCode, string secCode);
        //    long UnSubscribeTrades();
        //    void StartTrades();
        // //   Int32 StartTradesListenProcess(TradeStatusCallback tradeStatusCallback);
        // //   Int32 StartTradesListenProcess(ITradeStatusReceiver tsr);

        //    string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
        //                          Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor);

        //    string CreateOrderStr(Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string secCode,
        //                                 Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus,
        //                                 Int32 nOrderDescriptor);


        //    #region Connect


        //    #endregion

        //}

    // x64
    public interface ITrans2Quik
    {
        string GetTrans2QuikDllName { get; }

        long Connect2Quik(string path2Quik);
        long Disconnect();
        long IsConnected2Quik();
        long IsConnected2Server();

        void SetT2QReceiver(ITrans2QuikReceiver receiver);
        void Init();

        //Int32 SetConnectionStatusCallback(ConnectionStatusCallback callback);
        long SetConnectionStatusCallback(ConnectionStatusCallback64 callback);
        Int32 SendSyncTransaction(string transactionStr, ref double orderNum);
        Int32 SetTransactionReplyCallback(TransactionReplyCallback cb);
        Int32 SendAsyncTransaction(string transactionString);
        Int32 SendAsyncTransaction(string transactionString, out int extendedErrorCode);

        long SubscribeOrders(string classCode, string securityCode);
        long UnSubscribeOrders();
        void StartOrders();
        //   Int32 StartOrdersListenProcess(OrderStatusCallback orderStatusCallback);
        //   Int32 StartOrdersListenProcess(IOrderStatusReceiver orderStatusReceiver);

        long SubscribeTrades(string classCode, string secCode);
        long UnSubscribeTrades();
        void StartTrades();
        //   Int32 StartTradesListenProcess(TradeStatusCallback tradeStatusCallback);
        //   Int32 StartTradesListenProcess(ITradeStatusReceiver tsr);

        string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
                              Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor);

        string CreateOrderStr(Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string secCode,
                                     Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus,
                                     Int32 nOrderDescriptor);


        #region Connect


        #endregion

    }
}