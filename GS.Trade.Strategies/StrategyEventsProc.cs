using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Extension;
using GS.Extensions.DateTime;
using GS.Interfaces;
using GS.Status;

namespace GS.Trade.Strategies
{
    public abstract partial class Strategy
    {
        // Send Transaction to Quik
        // public void SendTransactionFromQueue(IEventArgs1 args)
        // private void ClearOrderTransactionQueue3(IEventArgs1 args)
        protected void OnTradeTerminalOrderTracking(object sender, IEventArgs1 args)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";
            switch (args.Operation.TrimUpper())
            {
                case "TRANSACTION":
                case "ORDERSTATUSCHANGED":
                default:
                    break;
            }
        }
        protected void OnTradeTerminalConnectionStatusChanged(object sender, IEventArgs args)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";
            switch (args.Operation.TrimUpper())
            {
                case "CONNECTED":
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        TradeTerminalKey, method, $"{OperationEnum.Connected}", "");
                    break;
                case "NOTCONNECTED":
                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        TradeTerminalKey, method, $"{OperationEnum.NotConnected}", "");
                    if (!TradeTerminal.IsWellConnected)
                        RemoveOrdersRegistered(method, OperationEnum.NoConnection.ToString(), "");
                    break;
                case "RESTART":
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                         TradeTerminalKey, method, $"{OperationEnum.Connected}: ReStart() Relax & Enjoy", args.Object.ToString());
                    break;
                default:
                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        TradeTerminalKey, method, $"Wrong Operation: {args.Operation.TrimUpper()}", "");
                    break;
            }
        }
        protected void OnOrderRegister(object sender, IEventArgs arg)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            if (!(arg.Object is IOrder3 order))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    "Order is Null", method, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            PrintOrder(order, method, order.Status.ToString());
            //Evlm2(order.Status == OrderStatusEnum.Registered ? EvlResult.SUCCESS : EvlResult.FATAL,
            //    EvlSubject.TRADING,
            //    StrategyTimeIntTickerString,
            //    order.ShortInfo, $"{method}: {OperationEnum.NoOperation}",
            //    order.ShortDescription, order.ToString());
        }
        protected void OnOrderTransactionSend(object sender, IEventArgs arg)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(arg.Object is IOrder3 order))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    "Order is Null", method, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            switch (arg.Operation.TrimUpper())
            {
                case "SENDED":
                    OnOrderTransactionSended(order);
                    break;
                case "NOTSENDED":
                    OnOrderTransactionNotSended(order);
                    break;
                default:
                    Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        order.ShortInfo, method,
                        $"Not Found: {arg.Operation.TrimUpper()}", order.ToString());
                    return;
            }
            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
            //            order.ShortInfo, method, order.ShortDescription, order.ToString());
        }
        protected void OnOrderTransactionReply(object sender, IEventArgs arg)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(arg.Object is IOrder3 order))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    "Order is Null", System.Reflection.MethodBase.GetCurrentMethod()?.Name,
                    $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            switch (arg.Operation.TrimUpper())
            {
                //case "SENDED":
                //    OnOrderSended(order);
                //    break;
                //case "NOTSENDED":
                //    OnOrderNotSended(order);
                //    break;
                case "ACTIVATED":
                    OnOrderTransactionReplyActivated(order);
                    break;
                case "REJECTED":
                    OnOrderTransactionReplyRejected(order);
                    break;
                case "CANCELED":
                    OnOrderTransactionReplyCanceled(order);
                    break;
                case "CANNOTCANCELED":
                    OnOrderTransactionReplyCannotCanceled(order);
                    break;
                default:
                    Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        order.ShortInfo, method,
                        $"Wrong Operation: {arg.Operation.TrimUpper()}", order.ToString());
                    break;
            }
            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
            //            order.ShortInfo, System.Reflection.MethodBase.GetCurrentMethod()?.Name, 
            //            order.ShortDescription, order.ToString());
        }
        protected void OnOrderStatusChanged(object sender, IEventArgs arg)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            if (!(arg.Object is IOrder3 order))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    "Order is Null", System.Reflection.MethodBase.GetCurrentMethod()?.Name,
                    $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            switch (arg.Operation.TrimUpper())
            {
                case "COMPLETED":
                    OnOrderStatusChangedAll(order);
                    break;
                case "ACTIVATED":
                    OnOrderStatusChangedAll(order);
                    break;
                case "CANCELED":
                    OnOrderStatusChangedAll(order);
                    break;
                case "FILLED":
                    OnOrderStatusChangedAll(order);
                    break;
                case "PARTLYFIELD":
                    OnOrderStatusChangedAll(order);
                    break;
                default:
                    Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                        order.ShortInfo, method,
                        $"Not Found: {arg.Operation.TrimUpper()}", order.ToString());
                    break;
            }
            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
            //            order.ShortInfo, System.Reflection.MethodBase.GetCurrentMethod()?.Name,
            //            $"{arg.Entity}, {arg.Operation}", order.ToString());
        }
        protected void OnOrderTransReplyTest(object sender, IEventArgs arg)
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
                arg.Category, $"{arg.Entity}, {arg.Operation}", arg.OperationKey,
                arg.Object?.ToString());
        }
        protected void OnOrderStatusChangedTest(object sender, IEventArgs arg)
        {
            Evlm2(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
                arg.Category, $"{arg.Entity}, {arg.Operation}", arg.OperationKey,
                arg.Object?.ToString());
        }

        #region  SendOrderTransaction **************************************************************
        private void OnOrderTransactionNotSended(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);

            //var ord = ActiveOrderCollection.GetByKey(order.Key);
            if (ord == null)
            {
                order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                //Evlm2(EvlResult.FATAL, EvlSubject.TRADING, order.StrategyTimeIntTickerString,
                //    order.ShortInfo, method, "Order Not Found", order.ToString());
                return;
            }
            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            order.BusyStatus = BusyStatusEnum.ReadyToUse;
            ord.BusyStatus = order.BusyStatus;

            if (ord.TransactionAction == OrderTransactionActionEnum.SetOrder && 
                    ord.Status == OrderStatusEnum.Rejected)
            {
                RemoveOrder(ord, method, "NotSended");
            }
            else if (ord.TransactionAction == OrderTransactionActionEnum.CancelOrder)
            {
                // Leave Active Order. We Try Cancel Order Again
                // var orderoperation = $"{ord.TransactionAction} {ord.TransactionStatus} {ord.ErrorMsg}";
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    order.ShortInfo, $"{method} Remain in OrderList",
                    order.ShortDescription, order.ToString());

                if (ord.TransactionStatus == OrderStatusEnum.NotSended &&
                    order.ErrorMsg == OrderErrorMsg.NotConnected)
                {
                    // if(TradeTerminal.IsWellConnected)
                        KillOrder(ord, method, "ReTryCancel");
                }
            }
        }
        private void OnOrderTransactionSended(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            // var ord = ActiveOrderCollection.GetByKey(order.Key);
            if (ord == null)
            {
                // order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                return;
            }
            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            // Order Send TimeOut Expire DateTime Set
            ord.SetSendTimeOut(DateTime.Now.Add(OrderSendTimeOutExpireTimeSpan));

            //PrintOrder(ord, method, OrderStatusEnum.Sended.ToString());
            PrintOrder(ord, method, $"{ord.TransactionAction} {ord.TransactionStatus} {ord.ErrorMsg}");
            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
            //    order.ShortInfo, $"{method}",
            //    order.ShortDescription, order.ToString());
        }
        #endregion
        #region Transaction Reply Received
        private void OnOrderNotSended(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            if (ord == null)
                return;
            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            if (ord.ErrorMsg == OrderErrorMsg.UnknownSendTransFailure)
            {
                RemoveOrder(ord, method);
            }
            else if (ord.ErrorMsg == OrderErrorMsg.NotConnected)
            {
                if (ord.AttemptsToSend-- > 0)
                {
                    ord.Status = OrderStatusEnum.NeedReSend;
                }
                else
                {
                    RemoveOrder(ord, method);
                }
            }
        }
        private void OnOrderSended(IOrder3 order)
        {
            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            if (ord == null)
                return;

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                order.ShortInfo, System.Reflection.MethodBase.GetCurrentMethod()?.Name,
                order.ShortDescription, order.ToString());
        }
        private void OnOrderTransactionReplyActivated(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            if (ord == null)
            {
                order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                return;
            }
            // if (order.TransactionStatus == OrderStatusEnum.Completed)
            //        order.BusyStatus = BusyStatusEnum.ReadyToUse;

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            order.BusyStatus = BusyStatusEnum.ReadyToUse;
            ord.BusyStatus = order.BusyStatus;

            ord.ClearSendTimeOut();

            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                order.ShortInfo, $"{method} Activated",
                order.ShortDescription, order.ToString());
        }
        private void OnOrderTransactionReplyRejected(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            if (ord == null)
            {
                order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                return;
            }
            // if (order.TransactionStatus == OrderStatusEnum.Completed)

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            order.BusyStatus = BusyStatusEnum.ReadyToUse;
            ord.BusyStatus = order.BusyStatus;

            ord.ClearSendTimeOut();

            if (ord.Status == OrderStatusEnum.Rejected)
                RemoveOrder(ord, method, ord.ErrorMsg.ToString(),ord.TrMessage);
        }
        private void OnOrderTransactionReplyCanceled(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var orders = ActiveOrderCollection.Items;
            var ord = orders
                .FirstOrDefault(o => (order.Number != 0 &&  order.Number == o.Number) || o.TransId == order.TransId);
            if (ord == null)
            {
                order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                return;
            }

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            order.BusyStatus = BusyStatusEnum.ReadyToUse;
            ord.BusyStatus = order.BusyStatus;

            ord.ClearSendTimeOut();

            if (ord.Status == OrderStatusEnum.Canceled)
                RemoveOrder(ord, method, "Canceled");
        }
        private void OnOrderTransactionReplyCannotCanceled(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            //var orders = ActiveOrderCollection.Items;
            var ord = ActiveOrderCollection.Items
                .FirstOrDefault(o => (order.Number != 0 && order.Number == o.Number) || o.TransId == order.TransId);
            if (ord == null)
            {
                order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                return;
            }
            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            order.BusyStatus = BusyStatusEnum.ReadyToUse;
            ord.BusyStatus = order.BusyStatus;

            ord.ClearSendTimeOut();

            if (ord.Status == OrderStatusEnum.Rejected)
                RemoveOrder(ord, method, ord.ErrorMsg.ToString(), ord.TrMessage);
        }
        #endregion
        #region OrderStatusChanged Reply Received *****************************************
        private void OnOrderStatusChangedAll(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var ord = ActiveOrderCollection.Items
               .FirstOrDefault(o => (order.Number != 0 && order.Number == o.Number) || o.TransId == order.TransId);
            if (ord == null)
            {
                order.BusyStatus = BusyStatusEnum.ReadyToUse;
                PrintOrder(order, method, "Order Not Found in OrderList", order.ShortDescription);
                return;
            }
            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            ord.TransactionAction = order.TransactionAction;
            ord.TransactionStatus = order.TransactionStatus;

            order.BusyStatus = BusyStatusEnum.ReadyToUse;
            ord.BusyStatus = order.BusyStatus;
        
            switch (ord.Status)
            {
                case OrderStatusEnum.Filled:
                    PrintOrder(ord, method, OrderStatusEnum.Filled.ToString(), ord.ShortDescription);
                    RemoveOrder(ord, method, OrderStatusEnum.Filled.ToString());
                    break;
                case OrderStatusEnum.Canceled:
                    PrintOrder(ord, method, OrderStatusEnum.Canceled.ToString(), ord.ShortDescription);
                    RemoveOrder(ord, method, OrderStatusEnum.Canceled.ToString());
                    break;
                case OrderStatusEnum.Activated:
                    PrintOrder(ord, method, OrderStatusEnum.Activated.ToString(), ord.ShortDescription);
                    break;
                case OrderStatusEnum.PartlyFilled:
                    PrintOrder(ord, method, OrderStatusEnum.PartlyFilled.ToString(), ord.ShortDescription);
                    break;
                default:
                    PrintOrder(ord, method, $"Status Not Found: {ord.Status}", ord.ShortDescription);
                    break;
            }
        }
        private void OnOrderStatusChangedActivated(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            if (ord == null)
                return;

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                order.ShortInfo, $"{method}: {OperationEnum.NoOperation}",
                order.ShortDescription, order.ToString());
        }
        private void OnOrderStatusChangedCanceled(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.Number == order.Number);
            if (ord == null) return;

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            if (ord.Status == OrderStatusEnum.Canceled)
                RemoveOrder(ord, method);
        }
        private void OnOrderStatusChangedFilled(IOrder3 order)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name;

            var orders = ActiveOrderCollection.Items;
            var ord = orders.FirstOrDefault(o => o.TransId == order.TransId);
            if (ord == null) return;

            ord.Status = order.Status;
            ord.Number = order.Number;
            ord.TrReplyCode = order.TrReplyCode;
            ord.TrMessage = order.TrMessage;
            ord.ErrorMsg = order.ErrorMsg;

            if (ord.Status == OrderStatusEnum.Filled)
                RemoveOrder(ord, method);
            else
                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    order.ShortInfo, method, order.ShortDescription, order.ToString());
        }
        #endregion

        #region TradeReply **************************************************

        protected void OnTradeReply(object sender, IEventArgs arg)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(arg.Object is ITrade3 trade))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                    "Trade is Null", method,
                    $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                trade.ShortInfo, method, trade.ShortInfo, trade.ToString());
            NewTrade(trade);
            //switch (arg.Operation.TrimUpper())
            //{
            //    case "NEW":
            //        NewTrade(trade);
            //        break;
            //    default:
            //        Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
            //            trade.ShortInfo, method,
            //            $"Operstion Not Found: {arg.Operation.TrimUpper()}", trade.ToString());
            //        break;
            //}
        }
        #endregion

        //-------------------------------------------------------------------------------
        // RUN in MainBase After OrderStatusChanged       
        protected TimeSpan OrderRegisteredExpireTimeOutSec { get; set; }
        protected TimeSpan OrderSendedExpireTimeOutSec { get; set; }
        protected TimeSpan OrderSendTimeOutExpireTimeSpan { get; set; }
        protected void RemoveOrdersSendedTimeOut(DateTime dt)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            foreach (var o in ActiveOrderCollection.Items
                .Where(ord => (ord.IsActive || ord.IsRegistered) &&
                              ord.TransactionStatus == OrderStatusEnum.Sended)
                )
            {
                var elapsedtotalseconds = (dt - o.Sended).TotalSeconds;
                var timeoutseconds = OrderSendedExpireTimeOutSec.TotalSeconds;
                if (elapsedtotalseconds <= timeoutseconds) continue;
                //Evlm2(EvlResult.FATAL, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    o.ShortInfo,
                //    $"{method} SendTimeOut Expire. Elapsed(s):{totalseconds} TimeOut(s):{timeoutseconds}",
                //    o.ShortDescription, o.ToString());

                RemoveOrder(o, method,
                    $"SendTimeOut Expires. Elapsed(s):{ elapsedtotalseconds} TimeOut(s):{ timeoutseconds} ");

                //var r = ActiveOrderCollection.RemoveNoKey(o);
                //var resultstr = r ? "Remove" : "Can't Remove";
                //Evlm2(EvlResult.WARNING, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    o.ShortInfo, $"{method}: {resultstr}",
                //    o.ShortDescription, o.ToString());
            }
        }
        protected void RemoveOrdersSendTimeOut(DateTime dt)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            //Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
            //        "Orders", method, "", "");

            foreach (var o in ActiveOrderCollection.ActiveOrdersSoftInUse
                                .Where(o => o.IsSendTimeOutExpire(dt)))
            {
                RemoveOrder(o, method, "TimeOut Expires",
                    $"TimeOut:{o.SendTimeOutDT.DateTimeTickStrTodayCnd()} Now:{dt.DateTimeTickStrTodayCnd()} TimeOutValue: {OrderSendTimeOutExpireTimeSpan.TimeTickStr()}");
            }
            foreach (var o in ActiveOrderCollection.OrdersRegisteredInUse
                                .Where(o => o.IsSendTimeOutExpire(dt)))
            {
                RemoveOrder(o, method, "TimeOut Expires",
                    $"TimeOut:{o.SendTimeOutDT:g} Now:{dt:g}");
            }
        }
        protected void RemoveClosedOrders()
        {
            // Remove : Filled, Canceled, Rejected, NotSend
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            //Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
            //        "Orders", method, "", "");
            
            RemoveOrders(ClosedOrders, method, "Order Closed");          
        }
        protected void ReTryToCancelActiveOrders()
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            //Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
            //        "Orders", method, "", "");
            if (TradeTerminal == null) return;
            try
            {
                if (!TradeTerminal.IsWellConnected) return;

                foreach (var o in ActiveOrderCollection.ActiveOrdersSoftReadyToUse
                    .Where(ord =>
                        ord.TransactionAction == OrderTransactionActionEnum.CancelOrder &&
                        ord.TransactionStatus == OrderStatusEnum.NotSended
                        // && ord.ErrorMsg == OrderErrorMsg.NotConnected
                        ))
                {
                    KillOrder(o, method, string.Empty, "ReTryCancel when NotSended");
                }
            }
            catch (Exception e)
            {
                SendException(e);
                Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentAndMyTypeName, StrategyTimeIntTickerString,
                    method, "Exception", e.Message);
            }
        }
        private IOrder3 GetValidRegisteredOrder()
        {
            return ActiveOrderCollection.OrdersRegisteredReadyToUse
                .LastOrDefault();
        }
        public void TryToSetOrderRegisteredPending()
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            //Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
            //        "Orders", method, "", "");

            if (ActiveOrderCollection.ActiveOrdersSoft.Any()) return;
            if (ActiveOrderCollection.OrdersRegisteredInUse.Any()) return;

            var regReadyToUse = ActiveOrderCollection.OrdersRegisteredReadyToUse.ToList();
            var cnt = regReadyToUse.Count;
            if (cnt > 0 && cnt <= Contract)
            {
                var ord = regReadyToUse.LastOrDefault();
                if (ord == null) return;

                if (!TradeTerminal.IsWellConnected) return;

                TradeTerminal.SetLimitOrder(ord);
                PrintOrder(ord, method, "PocketOrder", "Try to Set Order Registered.Pending");
            }
            else if (cnt > Contract)
            {
                PrintOrders(regReadyToUse, method, $"OrderCnt:{cnt}", "Too Much Registered Pending Orders");
            }
        }
        protected void TryToSetOrderRegisteredPending1()
        {
            var order = GetValidRegisteredOrder();
            if (order == null) return;
            SetOrder2(order.Operation, order.Quantity, order.LimitPrice);
        }
        protected void VerifyOrderList()
        {
            if (Code == "Default" || 
                TradeTerminal.Type == GS.Trade.TradeTerminalType.Simulator ||
                TradeTerminalType == "Simulator") return;

            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                foreach (var o in ActiveOrderCollection.Items
                            .Where(ord => ord.Status != OrderStatusEnum.Registered))
                {
                    // OrderList First
                    var ord = TradeTerminal.GetOrderByKey(o.Key);
                    if (ord != null)
                    {
                        if (o.Status != ord.Status)
                            Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
                                o.ShortInfo, $"{method} OrderInList Missmatch OrderAct", ord.ShortInfo, o.ToString());
                    }
                    else
                    {
                        Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
                                o.ShortInfo, $"{method} OrderInList is Absent in TrTerm.Orders", o.ShortDescription, o.ToString());
                    }
                }
                // TradeTerminal Active Orders
                foreach (var o in TradeTerminal.GetStartegyActiveOrders(Key))
                {
                    var ord = ActiveOrderCollection.GetByKey(o.Key);
                    if (ord != null)
                    {
                        if (o.Status != ord.Status)
                            Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
                                o.ShortInfo, $"{method} OrderAct Missmatch OrderInList", ord.ShortInfo, o.ToString());
                    }
                    else
                    {
                        Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, StrategyTimeIntTickerString,
                                o.ShortInfo, $"{method} TrTerm.Order is Absent in OrderList", o.ShortDescription, o.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(StrategyTimeIntTickerString, e.Message, method,"", e);
            }
        }
    }
}


