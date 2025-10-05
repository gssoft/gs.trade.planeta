using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using GS.Extension;
using GS.Interfaces;
using GS.Status;
using GS.Trade.Trades;

namespace GS.Trade.TradeTerminals.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public IQuikTransaction CreateQuikTransaction(IOrder3 o, IStrategy strategy, long transID,
                                                                QuikTransactionActionEnum action,
                                                                string transactionStr)
        {
            var dt = DateTime.Now;
            return new QuikTransaction
            {
                TransID = transID,
                Order = o,
                Strategy = strategy,
                Action = action,
                TradeTerminalKey = DllNamePath2QuikPair,
                DT = dt,
                RegisteredDT = dt,
                SendedDT = dt,
                CompletedDT = dt,
                Status = TransactionStatus.Registered,
                Result = TransactionResult.Unknown,
                QuikResult = T2QResults.TRANS2QUIK_DLL_CONNECTED,
                ExtendedErrorCode = 0,
                ReplyCode = 0,
                TransactionString = transactionStr
            };
        }
        /*
        public enum OrderAction
        {
            NewOrder = 1,
            NewStopOrder = 2,
            MoveOrder = 3,
            KillOrder = 4,
            KillStopOrder = 6,
            KillAllOrders = 7,
            KillAllStopOrders = 8,
            KillAllFuturesOrders = 9
        } ;
        public enum OrderType  { Market = 1, Limit = 2, StopLimit = 3, Stop = 5 }
        public enum OrderOperation { Buy = 1, Sell = 2}
        */
        private const string MarketOrLimitOrderStr=
"TRANS_ID=TransID;ACTION=NEW_ORDER;ACCOUNT=Account;CLIENT_CODE=ClientCode;CLASSCODE=ClassCode;SECCODE=SecCode;OPERATION=Operation;QUANTITY=Quantity;PRICE=Price;TYPE=Type;";

        private const string StopLimitOrderStr =
"TRANS_ID=TransID;ACTION=NEW_STOP_ORDER;ACCOUNT=Account;CLIENT_CODE=ClientCode;CLASSCODE=ClassCode;SECCODE=SecCode;OPERATION=Operation;QUANTITY=Quantity;STOPPRICE=StopPrice;PRICE=Price;";

        private const string StopLimitWithLinkedProfitOrderStr =
"TRANS_ID=TransID;ACTION=NEW_STOP_ORDER;ACCOUNT=Account;CLIENT_CODE=ClientCode;CLASSCODE=ClassCode;SECCODE=SecCode;OPERATION=Operation;QUANTITY=Quantity;STOPPRICE=StopPrice;PRICE=Price;STOP_ORDER_KIND=WITH_LINKED_LIMIT_ORDER;LINKED_ORDER_PRICE=TakeProfit;KILL_IF_LINKED_ORDER_PARTLY_FILLED=NO;";

        private const string KillOrderStr =
"TRANS_ID=TransID;ACTION=KILL_ORDER;CLASSCODE=ClassCode;SECCODE=SecCode;ORDER_KEY=OrderKey;";

        private const string KillStopOrderStr =
"TRANS_ID=TransID;ACTION=KILL_STOP_ORDER;CLASSCODE=ClassCode;SECCODE=SecCode;STOP_ORDER_KEY=StopOrderKey;";

        private const string KillAllFuturesOrdersStr =
"TRANS_ID=TransID;ACCOUNT=Account;CLASSCODE=ClassCode;SECCODE=SecCode;BASE_CONTRACT=BaseContract;ACTION=KILL_ALL_FUTURES_ORDERS;";
//       private const string KillAllFuturesOrdersStr =
//    "TRANS_ID=TransID;CLASSCODE=ClassCode;ACTION=KILL_ALL_FUTURES_ORDERS;";

        private const string KillAllStopOrdersStr =
"TRANS_ID=TransID;ACCOUNT=Account;CLASSCODE=ClassCode;SECCODE=SecCode;BASE_CONTRACT=BaseContract;ACTION=KILL_ALL_STOP_ORDERS;";

        private const string AllFuturesOrdersStr =
"TRANS_ID=TransID;ACCOUNT=Account;CLASSCODE=ClassCode;SECCODE=SecCode;BASE_CONTRACT=BaseContract;ACTION=Action;";

        public long SetMarketOrder(string account, string strategy, string classcode, string seccode,
                        OrderOperationEnum operation,
                        double price, long quantity)
        {
            var trId = GetTransID();

            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode,
            //    operation, OrderTypeEnum.Market, 0, price, quantity, quantity, "");

            var orderStr = MarketOrLimitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            // orderStr = orderStr.Replace("Action", OrderActionToStr(action));
            orderStr = orderStr.Replace("Type", OrderTypeToStr(OrderTypeEnum.Market));
            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("Price", price.ToString());
            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            // var ret = SendSyncTransaction(orderStr);
            var ret = SendAsyncTransaction(orderStr);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, strategy + "." + seccode, "Quik.SetMarketOrLimitOrder: " + ret, orderStr, DllNamePath2QuikPair);

            return ret ? +1 : -1;
        }
        public long SetLimitOrder(IOrder3 o)
        {
            o.BusyStatus = BusyStatusEnum.InUse;

            var trId = GetTransID();

            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode,
            //                            operation, OrderTypeEnum.Limit, 0, price, quantity, quantity, "");

            var orderStr = MarketOrLimitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", o.AccountCode); // .TrimUpper());
            //orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", o.TickerBoard.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", o.TickerCode.Trim());

            orderStr = orderStr.Replace("Type", OrderTypeToStr(OrderTypeEnum.Limit));
            orderStr = orderStr.Replace("Operation", OrderOperationToStr(o.Operation));

            orderStr = orderStr.Replace("Price", o.LimitPriceStringF);
            orderStr = orderStr.Replace("Quantity", o.Quantity.ToString(CultureInfo.InvariantCulture));
           
            // var ret = SendSyncTransaction(orderStr);
            //var ret = SendAsyncTransaction(orderStr);

            o.TransId = UInt32.Parse(trId);
            o.TransactionAction = OrderTransactionActionEnum.SetOrder;
            var t = CreateQuikTransaction(o, o.Strategy, long.Parse(trId),
                                                QuikTransactionActionEnum.SetLimit, orderStr);

            bool ret = true;
            //if (_orderLockedCount.IsEnabled)
            //{
            //    if (_orderLockedCount.Inc())
            //    {
            //        ret = SendAsyncTransaction(t);
            //        Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
            //            o.Strategy.StrategyTickerString, "Quik.SetLimitOrder: " + ret, orderStr, DllNamePath2QuikPair);
            //    }
            //    else
            //    {
            //        EnQueTransaction(t);
            //        ret = true;
            //        Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
            //            o.Strategy.StrategyTickerString, "Quik.SetLimitOrderInQueue: " + ret, orderStr,
            //            DllNamePath2QuikPair);
            //    }
            //}
            //else
            //{
            //    EnQueTransaction(t);
            //    ret = true;
            //    Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
            //        o.Strategy.StrategyTickerString, "Quik.SetLimitOrderInQueue: " + ret, orderStr,
            //        DllNamePath2QuikPair);
            //}

            SendTransaction(t);
            //EnQueBlTransaction(t);
            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
                       o.Strategy.StrategyTickerString, "Quik.Order", "SetLimitInQueue: " + ret, orderStr,
                        DllNamePath2QuikPair);
            return ret ? +1 : -1;
        }

        public long SetLimitOrder(IStrategy strat, string account, string strategy, string classcode, string seccode,
                       OrderOperationEnum operation,
                       double price, string priceStr, long quantity,
                       DateTime expireDateTime, string comment)
        {
            var trId = GetTransID();

            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode,
            //                            operation, OrderTypeEnum.Limit, 0, price, quantity, quantity, "");

            var orderStr = MarketOrLimitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            // orderStr = orderStr.Replace("Action", OrderActionToStr(action));
            orderStr = orderStr.Replace("Type", OrderTypeToStr(OrderTypeEnum.Limit));
            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("Price", priceStr);
            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            //var t = CreateQuikTransaction(long.Parse(trId), operation.ToString().WithRight(".") + OrderTypeEnum.Limit, orderStr);

            // var ret = SendSyncTransaction(orderStr);
            //var ret = SendAsyncTransaction(orderStr);
            var t = CreateQuikTransaction(null, strat, long.Parse(trId), QuikTransactionActionEnum.SetLimit, orderStr);
           
            //Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode,
            //    strategy + "." + seccode, "Quik.SetMarketOrLimitOrder: " + ret, orderStr, DllNamePath2QuikPair);

            //03.05.2018
            // var ret = SendAsyncTransaction(t);
            // return ret ? +1 : -1;
            return 1;
        }
        public long SetLimitOrderInQueue(string account, string strategy, string classcode, string seccode,
                      OrderOperationEnum operation,
                      double price, string priceStr, long quantity,
                      DateTime expireDateTime, string comment)
        {
            var trId = GetTransID();

            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode,
            //                            operation, OrderTypeEnum.Limit, 0, price, quantity, quantity, "");

            var orderStr = MarketOrLimitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            // orderStr = orderStr.Replace("Action", OrderActionToStr(action));
            orderStr = orderStr.Replace("Type", OrderTypeToStr(OrderTypeEnum.Limit));
            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("Price", priceStr);
            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            // var ret = SendSyncTransaction(orderStr);
            //var ret = SendAsyncTransaction(orderStr);
            EnQueOrder(orderStr);
            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, strategy + "." + seccode, "Quik.SetLimitOrderInQueue: ", orderStr, DllNamePath2QuikPair);

            return 1;
        }
        public long SetLimitOrder(string account, string strategy, string classcode, string seccode,
                      OrderOperationEnum operation,
                      double price, long quantity,
                      DateTime expireDateTime, string comment)
        {
            var trId = GetTransID();

            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode,
            //                            operation, OrderTypeEnum.Limit, 0, price, quantity, quantity, "");

            var orderStr = MarketOrLimitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            // orderStr = orderStr.Replace("Action", OrderActionToStr(action));
            orderStr = orderStr.Replace("Type", OrderTypeToStr(OrderTypeEnum.Limit));
            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("Price", price.ToString());
            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            // var ret = SendSyncTransaction(orderStr);

            //Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode,
            //    strategy + "." + seccode, "Quik.SetMarketOrLimitOrder: " + ret, orderStr, DllNamePath2QuikPair);
            // var ret = SendAsyncTransaction(orderStr);
            // return ret ? +1 : -1;

            return 1;
        }
        public long SetMarketOrLimitOrder( string account, string strategy, string classcode, string seccode,
                        OrderTypeEnum ordertype, OrderOperationEnum operation,
                        double price, long quantity, 
                        DateTime expireDateTime, string comment)
        {
            var trId = GetTransID();

            //_orders.RegisterOrder(0,DateTime.Now, UInt32.Parse(trId), account, strategy, seccode, 
            //            operation, ordertype, 0, price, quantity, quantity, "");

            var orderStr = MarketOrLimitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            // orderStr = orderStr.Replace("Action", OrderActionToStr(action));
            orderStr = orderStr.Replace("Type", OrderTypeToStr(ordertype));
            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("Price", price.ToString());
            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            // var ret = SendSyncTransaction(orderStr);
            //var ret = SendAsyncTransaction(orderStr);

            var t = CreateQuikTransaction(null, null, long.Parse(trId), QuikTransactionActionEnum.SetLimit, orderStr);
            var ret = SendAsyncTransaction(t);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, strategy + "." + seccode, "Quik.SetMarketOrLimitOrder: " + ret, orderStr, DllNamePath2QuikPair);

            return ret ? +1 : -1;
        }
        public long SetStopLimitOrder( string account, string strategy, string classcode, string seccode,
                        OrderOperationEnum operation, 
                        double stopprice, double price, long quantity,
                        DateTime expireDateTime, string comment)
        {
            var trId = GetTransID();
            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode,
            //    operation, OrderTypeEnum.StopLimit, stopprice, price, quantity, quantity, "");


            var orderStr = StopLimitOrderStr;
            
            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("StopPrice", stopprice.ToString());
            orderStr = orderStr.Replace("Price", price.ToString());
            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            var ret = SendAsyncTransaction(orderStr);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, strategy + "." + seccode, "Quik.SetStopLimitOrder: " + ret, orderStr, DllNamePath2QuikPair);

            return ret ? +1 : -1;
        }
        public long SetStopLimitWithLinkedProfitOrder(
                        string account, string strategy, string classcode, string seccode,
                        OrderOperationEnum operation,
                        double stopprice, double price, double takeprofit, long quantity,
                        DateTime expireDateTime, string comment)
        {
            var trId = GetTransID();
            //_orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(trId), account, strategy, seccode, 
            //                            operation, OrderTypeEnum.StopLimit, stopprice, price, quantity, quantity, "");
           // _orders.RegisterOrder(0, DateTime.Now, UInt32.Parse(GetTransID()), account, strategy, seccode, operation, OrderType.Limit, 0, takeprofit, quantity, quantity, "");

            var orderStr = StopLimitWithLinkedProfitOrderStr;

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());
            orderStr = orderStr.Replace("ClientCode", strategy.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());

            orderStr = orderStr.Replace("Operation", OrderOperationToStr(operation));

            orderStr = orderStr.Replace("StopPrice", stopprice.ToString());
            orderStr = orderStr.Replace("Price", price.ToString());
            orderStr = orderStr.Replace("TakeProfit", takeprofit.ToString());

            orderStr = orderStr.Replace("Quantity", quantity.ToString());

            var ret = SendAsyncTransaction(orderStr);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, strategy + "." + seccode, "Quik.SetStopLimitWithLinkedProfitOrder: " + ret, orderStr, DllNamePath2QuikPair);

            return ret ? +1 : -1;
        }
        public int SetMarketOrLimitWithStopLimit(
            string account, string strategy, string classcode, string seccode,
            OrderTypeEnum ordertype, OrderOperationEnum operation,
            double price, double stopPrice, double limitprice, long quantity,
            DateTime expireDateTime, string comment,
            ref long limitNumber, ref long stopLimitNumber )
        {
            var lim = SetMarketOrLimitOrder(account, strategy, classcode, seccode, ordertype, operation, 
                price, quantity, expireDateTime, comment);
            if (lim == 0) return -1;

            var stoplim = SetStopLimitOrder(account, strategy, classcode, seccode,
                                            operation == OrderOperationEnum.Buy ? OrderOperationEnum.Sell : OrderOperationEnum.Buy,
                                            stopPrice, limitprice, quantity, expireDateTime, comment);
            if (stoplim == 0) return -2;
            
            limitNumber = lim;
            stopLimitNumber = stoplim; 
            return 1;
        }

        //public int KillLimitOrder(string classcode, string seccode, long orderkey)
        //{
        //    var orderStr = KillOrderStr;
        //    var trId = GetTransID();
        //    orderStr = orderStr.Replace("TransID", trId);
        //    orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
        //    orderStr = orderStr.Replace("SecCode", seccode.Trim());
        //    orderStr = orderStr.Replace("OrderKey", orderkey.ToString());

        //    //SendAsyncTransaction(orderStr);
        //    var t = CreateQuikTransaction(null, long.Parse(trId), "Kill Limit # " + orderkey, orderStr);
        //    var ret = SendAsyncTransaction(t);

        //    Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, "Quik.KillLimitOrder", orderStr, DllNamePath2QuikPair);

        //    return 0;
        //}
        public int KillLimitOrder(IOrder3 o, IStrategy strategy, string classcode, string seccode,
                                                                                        ulong orderkey)
        {
            var orderStr = KillOrderStr;
            var trId = GetTransID();
            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());
            orderStr = orderStr.Replace("OrderKey", orderkey.ToString());

            //SendAsyncTransaction(orderStr);
            var t = CreateQuikTransaction(o, strategy, long.Parse(trId), QuikTransactionActionEnum.KillLimit, orderStr);
            t.OrderNumber = orderkey;

            var ret = SendAsyncTransaction(t);
            var strStr = strategy == null ? "Unknown" : strategy.StrategyTickerString;
            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, strStr, "Order", "Quik.KillLimitOrder", orderStr, DllNamePath2QuikPair);

            return 0;
        }
        public int KillLimitOrder(IOrder3 o)
        {
            o.BusyStatus = BusyStatusEnum.InUse;

            var orderStr = KillOrderStr;
            var trId = GetTransID();

            orderStr = orderStr.Replace("TransID", trId);
            orderStr = orderStr.Replace("ClassCode", o.TickerBoard.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", o.TickerCode.Trim());
            orderStr = orderStr.Replace("OrderKey", o.Number.ToString(CultureInfo.InvariantCulture));

            //SendAsyncTransaction(orderStr);
            o.TransId = UInt32.Parse(trId);
            o.TransactionAction = OrderTransactionActionEnum.CancelOrder;
            var t = CreateQuikTransaction(o, o.Strategy, long.Parse(trId), QuikTransactionActionEnum.KillLimit, orderStr);
            t.OrderNumber = o.Number;

            bool ret;
            //var ret = SendAsyncTransaction(t);

            //EnQueTransaction(t);
            //var ret = true;

            //if (_orderLockedCount.IsEnabled)
            //{
            //    if (_orderLockedCount.Inc())
            //    {
            //        ret = SendAsyncTransaction(t);
            //        Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
            //            o.Strategy.StrategyTickerString, "Quik.KillLimit: " + ret, orderStr, DllNamePath2QuikPair);
            //    }
            //    else
            //    {
            //        EnQueTransaction(t);
            //        ret = true;
            //        Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
            //            o.Strategy.StrategyTickerString, "Quik.KillLimitInQueue: " + ret, orderStr, DllNamePath2QuikPair);
            //    }
            //}
            //else
            //{
            //    EnQueTransaction(t);
            //    ret = true;
            //    Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
            //        o.Strategy.StrategyTickerString, "Quik.KillLimitInQueue: " + ret, orderStr, DllNamePath2QuikPair);
            //}
            SendTransaction(t);
            //EnQueBlTransaction(t);
            ret = true;
            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING,
                o.Strategy.StrategyTickerString, "Quik.Order", "KillLimitInQueue: " + ret, orderStr, DllNamePath2QuikPair);

            //Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, o.Strategy.StrategyTickerString, "Order", "Quik.KillLimitOrder: "+ret, orderStr, DllNamePath2QuikPair);
            return 0;
        }
       
        public int SetKillLimitOrderInQueue(string classcode, string seccode, ulong orderkey)
        {
            var orderStr = KillOrderStr;

            orderStr = orderStr.Replace("TransID", GetTransID());
            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());
            orderStr = orderStr.Replace("OrderKey", orderkey.ToString());

            //SendAsyncTransaction(orderStr);

            EnQueOrder(orderStr);
            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, seccode, "Quik.SetKillLimitOrderInQueue", orderStr, DllNamePath2QuikPair);

            return 0;
        }
        public int KillLimitOrderByTransID(string classcode, string seccode, long orderTransID)
        {
            return 0;
        }
        public bool KillStopOrder(string classcode, string seccode, ulong orderkey)
        {
            var orderStr = KillStopOrderStr;

            orderStr = orderStr.Replace("TransID", GetTransID());
            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());
            orderStr = orderStr.Replace("StopOrderKey", orderkey.ToString());

            var r = SendSyncTransaction(orderStr);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, seccode, "Quik.KillStopOrder", orderStr, DllNamePath2QuikPair);

            return r >= 0 ? true : false;
        }

        public int KillAllFuturesOrders(string account, string classcode, string seccode, string basecontract )
        {
            var orderStr = KillAllFuturesOrdersStr;

            orderStr = orderStr.Replace("TransID", GetTransID());
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());
            orderStr = orderStr.Replace("BaseContract", basecontract.Trim().ToUpper());

            var ret = SendAsyncTransaction(orderStr);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, seccode, "Quik.KillAllFuturesOrders", orderStr, DllNamePath2QuikPair);

            return ret ? +1 : -1;
        }

        public bool KillAllOrders(IStrategy stra, string strategy, string account, string tickercategory, string tickercode,
                                OrderTypeEnum ordtype, OrderOperationEnum operation)
        {
            var oo = new List<Order>();
            var key = strategy + account + tickercode;
            //_orders.GetOrders(OrderStatusEnum.Activated, key, oo);
            foreach (var o in oo.Where(o => o.Operation == operation && o.OrderType == ordtype))
                if (o.IsLimit)
                    KillLimitOrder(null, stra, tickercategory, tickercode, o.Number);
                else if (o.IsStopLimit)
                    KillStopOrder(tickercategory, tickercode, o.Number);
            return true;
        }

        public int KillAllLimitOrders(string account, string classcode, string seccode, string basecontract)
        {
            var orderStr = AllFuturesOrdersStr;
            orderStr.Replace("Action", "KILL_ALL_FUTURES_ORDERS");
            //var oo = _orders.OrderCollection.Distinct(_orders.GetAccountTickerComparer);
            //foreach(var o in oo)
            //{
            //    orderStr = orderStr.Replace("TransID", GetTransID());
            //    orderStr = orderStr.Replace("Account", account.Trim().ToUpper());

            //    orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            //    orderStr = orderStr.Replace("SecCode", seccode.Trim());
            //    orderStr = orderStr.Replace("BaseContract", basecontract.Trim().ToUpper());

            //    var ret = SendAsyncTransaction(orderStr);

            //    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, seccode, "Quik.KillAllStopOrders", orderStr, DllNamePath2QuikPair);
            //}
            return 1;
        }

        public int KillAllStopOrders(string account, string classcode, string seccode, string basecontract)
        {
            var orderStr = KillAllStopOrdersStr;

            orderStr = orderStr.Replace("TransID", GetTransID());
            orderStr = orderStr.Replace("Account", account.Trim().ToUpper());

            orderStr = orderStr.Replace("ClassCode", classcode.Trim().ToUpper());
            orderStr = orderStr.Replace("SecCode", seccode.Trim());
            orderStr = orderStr.Replace("BaseContract", basecontract.Trim().ToUpper());

            var ret = SendAsyncTransaction(orderStr);

            Evlm2(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, seccode, seccode, "Quik.KillAllStopOrders", orderStr, DllNamePath2QuikPair);

            return ret ? +1 : -1;
        }
        public long SetMarketOrLimitWithStopLimit(string account, string stratName, string classCode, string code,
            OrderTypeEnum ordertype, OrderOperationEnum operation,
            double entryPrice, double stopPrice, double limitPrice,
            long quantity, ref long limitNumber, ref long stopLimitNumber)
        {
            throw new NotImplementedException();
        }

        public int KillLimitOrderByTransID(string tickercategory, string tickercode, ulong orderTransID)
        {
            //_orders.CancelOrderByTransID(orderTransID);
            throw new NotImplementedException();
        }

        public bool KillAllFuturesLimitOrders(string account, string tickercategory, string tickercode, string basecontract)
        {
            throw new NotImplementedException();
        }

        private static string OrderTypeToStr(OrderTypeEnum ordtype )
        {
            string r="";
            switch( ordtype )
            {
                case OrderTypeEnum.Market:
                    r = "M";
                    break;
                case OrderTypeEnum.Limit:
                    r = "L";
                    break;
                case OrderTypeEnum.StopLimit:
                    r = "S";
                    break;
            }
            return r;
        }
        /*
        private static string OrderActionToStr(OrderAction ordaction)
        {
            string r = "";
            switch (ordaction)
            {
                case OrderAction.NewOrder:
                    r = "NEW_ORDER";
                    break;
                case OrderAction.NewStopOrder:
                    r = "NEW_STOP_ORDER";
                    break;
                case OrderAction.MoveOrder:
                    r = "MOVE_ORDERS";
                    break;
                case OrderAction.KillOrder:
                    r = "KILL_ORDER";
                    break;
                case OrderAction.KillStopOrder:
                    r = "KILL_STOP_ORDER";
                    break;
                case OrderAction.KillAllOrders:
                    r = "KILL_ALL_ORDERS";
                    break;
                case OrderAction.KillAllStopOrders:
                    r = "KILL_ALL_STOP_ORDERS";
                    break;
                case OrderAction.KillAllFuturesOrders:
                    r = "KILL_ALL_FUTURES_ORDERS";
                    break;
            }
            return r;
        }
        */ 
        private static string OrderOperationToStr(OrderOperationEnum ordoperation)
        {
            string r = "";
            switch (ordoperation)
            {
                case OrderOperationEnum.Buy:
                    r = "B";
                    break;
                case OrderOperationEnum.Sell:
                    r = "S";
                    break;
            }
            return r;
        }
        public void NewTick(DateTime dt, string tickerkey, double value)
        {
            throw new NotImplementedException();
        }
    }
}
