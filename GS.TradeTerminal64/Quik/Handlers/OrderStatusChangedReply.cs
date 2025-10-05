using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Extension;
using GS.Extensions.DateTime;
using GS.Identity;
using GS.Interfaces;
using GS.Status;
using GS.Trade.Trades;
using GS.Trade.Trades.Orders3;
using EventArgs = GS.Events.EventArgs;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        // x86
        public class OrderStatusReply
        {
            public double OrderNumber { get; set; }
            public int Date { get; set; }
            public int Time { get; set; }
            public int Mode { get; set; }
            public int ActivationTime { get; set; }
            public int CancelTime { get; set; }
            public int ExpireDate { get; set; }
            public string Account { get; set; }
            public string BrokerRef { get; set; }
            public string ClassCode { get; set; }
            public string Ticker { get; set; }
            public long IsSell { get; set; }
            public long Qty { get; set; }
            // 210808
            // public int Rest { get; set; }
            public Int64 Rest { get; set; }
            public double Price { get; set; }
            public int Status { get; set; }
            public uint TransId { get; set; }
            public string ClientCode { get; set; }
            public DateTime ReplyDateTime { get; set; }

            private static OrderStatusEnum ConvertToStatusEnum(int status)
            {
                switch (status)
                {
                    case 0: return OrderStatusEnum.Filled;
                    case 1: return OrderStatusEnum.Activated;
                    case 2: return OrderStatusEnum.Canceled;
                    default: return OrderStatusEnum.Unknown;
                }
            }

            public override string ToString()
            {
                return $"Order:[{OrderNumber}] Trans:[{TransId}] DT:{Date} {Time} St:{ConvertToStatusEnum(Status)} Mode:{Mode}" +
                       $" ActTm:{ActivationTime} CncTm:{CancelTime} ExpDt:{ExpireDate}" +
                       $" Acc:{Account} BrRf:{BrokerRef} ClsCd:{ClassCode} Tkr:{Ticker}" +
                       $" Side:{IsSell} Qty:{Qty} Rst:{Rest} Price:{Price}" +
                       $" ClntCd:{ClientCode} RplDt:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
            public string ShortInfo()
            {
                return $"Order:[{OrderNumber}] DT:{Date} {Time} St:{ConvertToStatusEnum(Status)}" +
                       $" ActTm:{ActivationTime} CncTm:{CancelTime}" +
                       $" Tkr:{Ticker}" +
                       $" Side:{IsSell} Qty:{Qty} Price:{Price}";
            }
            public string ShortDescription()
            {
                return $"Order:[{OrderNumber}] Trans:[{TransId}] DT:{Date} {Time} St:{ConvertToStatusEnum(Status)}" +
                       $" ActTm:{ActivationTime} CncTm:{CancelTime}" +
                       $" Acc:{Account} Tkr:{Ticker}" +
                       $" Side:{IsSell} Qty:{Qty} Price:{Price} RplDt:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
        }
        
        // x64
        public class OrderStatusReply64
        {
            public double OrderNumber { get; set; }
            public long Date { get; set; }
            public long Time { get; set; }
            public long Mode { get; set; }
            public long ActivationTime { get; set; }
            public long CancelTime { get; set; }
            public long ExpireDate { get; set; }
            public string Account { get; set; }
            public string BrokerRef { get; set; }
            public string ClassCode { get; set; }
            public string Ticker { get; set; }
            public long IsSell { get; set; }
            public long Qty { get; set; }
            // 210808
            // public int Rest { get; set; }
            public long Rest { get; set; }
            public double Price { get; set; }
            public long Status { get; set; }
            public ulong TransId { get; set; }
            public string ClientCode { get; set; }
            public DateTime ReplyDateTime { get; set; }

            private static OrderStatusEnum ConvertToStatusEnum(int status)
            {
                switch (status)
                {
                    case 0: return OrderStatusEnum.Filled;
                    case 1: return OrderStatusEnum.Activated;
                    case 2: return OrderStatusEnum.Canceled;
                    default: return OrderStatusEnum.Unknown;
                }
            }
            private static OrderStatusEnum ConvertToStatusEnum(long status)
            {
                switch (status)
                {
                    case 0: return OrderStatusEnum.Filled;
                    case 1: return OrderStatusEnum.Activated;
                    case 2: return OrderStatusEnum.Canceled;
                    default: return OrderStatusEnum.Unknown;
                }
            }

            public override string ToString()
            {
                return $"Order:[{OrderNumber}] Trans:[{TransId}] DT:{Date} {Time} St:{ConvertToStatusEnum(Status)} Mode:{Mode}" +
                       $" ActTm:{ActivationTime} CncTm:{CancelTime} ExpDt:{ExpireDate}" +
                       $" Acc:{Account} BrRf:{BrokerRef} ClsCd:{ClassCode} Tkr:{Ticker}" +
                       $" Side:{IsSell} Qty:{Qty} Rst:{Rest} Price:{Price}" +
                       $" ClntCd:{ClientCode} RplDt:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
            public string ShortInfo()
            {
                return $"Order:[{OrderNumber}] DT:{Date} {Time} St:{ConvertToStatusEnum(Status)}" +
                       $" ActTm:{ActivationTime} CncTm:{CancelTime}" +
                       $" Tkr:{Ticker}" +
                       $" Side:{IsSell} Qty:{Qty} Price:{Price}";
            }
            public string ShortDescription()
            {
                return $"Order:[{OrderNumber}] Trans:[{TransId}] DT:{Date} {Time} St:{ConvertToStatusEnum(Status)}" +
                       $" ActTm:{ActivationTime} CncTm:{CancelTime}" +
                       $" Acc:{Account} Tkr:{Ticker}" +
                       $" Side:{IsSell} Qty:{Qty} Price:{Price} RplDt:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
        }
        // x86
        private void SendOrderStatusChangedReplyEventArgs(
            OrderStatusReply orderStatusReply, Action<IEventArgs1> action)
        {
            var eargs = new EventArgs1
            {
                Process = "OrderStatusProcess",
                Category = "Orders",
                Entity = "OrderStatus",
                Operation = "AddOrUpdate",
                Object = orderStatusReply,
                ProcessingAction = action
            };
            if (ProcessTask != null)
                ProcessTask.EnQueue(eargs);
            else
                action(eargs);
        }
        // x64
        private void SendOrderStatusChangedReplyEventArgs(
            OrderStatusReply64 orderStatusReply, Action<IEventArgs1> action)
        {
            var eargs = new EventArgs1
            {
                Process = "OrderStatusProcess",
                Category = "Orders",
                Entity = "OrderStatus",
                Operation = "AddOrUpdate",
                Object = orderStatusReply,
                ProcessingAction = action
            };
            if (ProcessTask != null)
                ProcessTask.EnQueue(eargs);
            else
                action(eargs);
        }
        //public void NewOrderStatus(double dNumber, int iDate, int iTime, int iMode,
        //    int iActivationTime, int iCancelTime, int iExpireDate,
        //    string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
        //    int iIsSell, int iQty, int nBalance, double dPrice,
        //    int iStatus, uint dwTransId, string sClientCode)

        // x86 Until 2023.10
        public void NewOrderStatus(double dNumber, int iDate, int iTime, int iMode,
            int iActivationTime, int iCancelTime, int iExpireDate,
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            int iIsSell, long iQty, long nBalance, double dPrice,
            int iStatus, uint dwTransId, string sClientCode)
        {
            // 210808 Rest, nBalance from int to Int64
            if (IsProcessTaskInUse)
            {
                var orderStatusReply = new OrderStatusReply
                {
                    OrderNumber = dNumber,
                    Date = iDate,
                    Time = iTime,
                    Mode = iMode,
                    ActivationTime = iActivationTime,
                    CancelTime = iCancelTime,
                    ExpireDate = iExpireDate,
                    Account = sAcc,
                    BrokerRef = sBrokerRef, // Comment, strategy
                    ClassCode = sClassCode,
                    Ticker = sSecCode,
                    IsSell = iIsSell,
                    Qty = iQty,
                    Rest = nBalance,  // rest
                    Price = dPrice,
                    Status = iStatus,
                    TransId = dwTransId,
                    ClientCode = sClientCode, // Comment, Strategy
                    ReplyDateTime = DateTime.Now
                };
                SendOrderStatusChangedReplyEventArgs(orderStatusReply, OrderStatusReplyProcessing);
                SendOrderStatusChangedReplyEventArgs(orderStatusReply, OrderStatusChangedReplyTracking);

                //var eargs = new EventArgs1
                //{
                //    Process = "OrderStatusProcess",
                //    Category = "Orders",
                //    Entity = "OrderStatus",
                //    Operation = "AddOrUpdate",
                //    Object = orderstatusreply,
                //    ProcessingAction = OrderStatusReplyProcessing
                //};
                //if(ProcessTask != null)
                //    ProcessTask.EnQueue(eargs);
                //else
                //    OrderStatusReplyProcessing(eargs);

                return;
            }
            OrderStatusProcess(dNumber, iDate, iTime, iMode,
                                    iActivationTime, iCancelTime, iExpireDate,
                                    sAcc, sBrokerRef, sClassCode, sSecCode,
                                    iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);
        }

        // x64
        //void NewOrderStatus(ulong dNumber,
        //    long iDate, long iTime,
        //    long iMode,
        //    long iActivationTime, long iCancelTime, long iExpireDate,
        //    string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
        //    long iIsSell, long iQty, long nBalance, double dPrice,
        //    long iStatus, ulong dwTransId,
        //    string sClientCode);
        
        // x64 2023.10 new
        public void NewOrderStatus(ulong dNumber, long iDate, long iTime, long iMode,
            long iActivationTime, long iCancelTime, long iExpireDate,
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            long iIsSell, long iQty, long nBalance, double dPrice,
            long iStatus, ulong dwTransId, string sClientCode)
        {
            // 210808 Rest, nBalance from int to Int64
            if (IsProcessTaskInUse)
            {
                var orderStatusReply64 = new OrderStatusReply64
                {
                    OrderNumber = dNumber,
                    Date = iDate,
                    Time = iTime,
                    Mode = iMode,
                    ActivationTime = iActivationTime,
                    CancelTime = iCancelTime,
                    ExpireDate = iExpireDate,
                    Account = sAcc,
                    BrokerRef = sBrokerRef, // Comment, strategy
                    ClassCode = sClassCode,
                    Ticker = sSecCode,
                    IsSell = iIsSell,
                    Qty = iQty,
                    Rest = nBalance,  // rest
                    Price = dPrice,
                    Status = iStatus,
                    TransId = dwTransId,
                    ClientCode = sClientCode, // Comment, Strategy
                    ReplyDateTime = DateTime.Now
                };
                SendOrderStatusChangedReplyEventArgs(orderStatusReply64, OrderStatusReplyProcessing64);
                SendOrderStatusChangedReplyEventArgs(orderStatusReply64, OrderStatusChangedReplyTracking);

                //var eargs = new EventArgs1
                //{
                //    Process = "OrderStatusProcess",
                //    Category = "Orders",
                //    Entity = "OrderStatus",
                //    Operation = "AddOrUpdate",
                //    Object = orderstatusreply,
                //    ProcessingAction = OrderStatusReplyProcessing
                //};
                //if(ProcessTask != null)
                //    ProcessTask.EnQueue(eargs);
                //else
                //    OrderStatusReplyProcessing(eargs);

                return;
            }
            // 2023.10
            // Delete this
            //OrderStatusProcess(dNumber, iDate, iTime, iMode,
            //                        iActivationTime, iCancelTime, iExpireDate,
            //                        sAcc, sBrokerRef, sClassCode, sSecCode,
            //                        iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);
        }
        //------------------------------------------------------------
        public void OrderStatusReplyProcessing(IEventArgs args)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(args.Object is OrderStatusReply orderStatusReply))
            {
                Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentName, Name,
                        m,
                        $"Args IS NOT {nameof(OrderStatusReply)}", "Sorry");
                return;
            }
            var dNumber = orderStatusReply.OrderNumber;
            var iDate = orderStatusReply.Date;
            var iTime = orderStatusReply.Time;
            var mode = orderStatusReply.Mode;
            var iActivationTime = orderStatusReply.ActivationTime;
            var iCancelTime = orderStatusReply.CancelTime;
            var iExpireDate = orderStatusReply.ExpireDate;
            var account = orderStatusReply.Account;
            var strategy = orderStatusReply.BrokerRef;
            var classCode = orderStatusReply.ClassCode;
            var ticker = orderStatusReply.Ticker;
            var iIsSell = orderStatusReply.IsSell;
            var quantity = orderStatusReply.Qty;
            var rest = orderStatusReply.Rest;
            var price = orderStatusReply.Price;
            var iStatus = orderStatusReply.Status;
            var dwTransId = orderStatusReply.TransId;
            var comment = orderStatusReply.ClientCode;

            var number = Convert.ToUInt64(dNumber);
            if (number == 0)
                return;

            if (classCode == "FUTEVN") return;

            //if (ClassCodeToRemoveLogin.HasValue())
            //    if (classCode.Contains(ClassCodeToRemoveLogin))
            //        strategy = strategy.Replace(LoginToRemove, "");

            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");

            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            var tm = new TimeSpan(int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            // ActiveTime May Be = 0, Shame upon Quik
            TimeSpan activeTime;
            if (iActivationTime != 0)
            {
                var sActTime = iActivationTime.ToString("D6");
                activeTime = new TimeSpan(
                    int.Parse(sActTime.Substring(0, 2)),
                    int.Parse(sActTime.Substring(2, 2)),
                    int.Parse(sActTime.Substring(4, 2)));
            }
            else
                activeTime = DateTime.Now.TimeOfDay;

            // CancelTime May Be = 0, Shame upon Quik
            TimeSpan cancelTime;
            if (iCancelTime != 0)
            {
                var sCancTime = iCancelTime.ToString("D6");
                    cancelTime = new TimeSpan(
                    int.Parse(sCancTime.Substring(0, 2)),
                    int.Parse(sCancTime.Substring(2, 2)),
                    int.Parse(sCancTime.Substring(4, 2)));
            }
            else
                cancelTime = DateTime.Now.TimeOfDay;

            var sExpireDate = iExpireDate.ToString("D8");
            var i1 = int.Parse(sExpireDate.Substring(0, 4));

            var activateDateTime = dt.Date.Add(activeTime);
            var cancelDateTime = dt.Date.Add(cancelTime);
            var filledDateTime = dt.Date.Add(DateTime.Now.TimeOfDay);
            // var filledDateTime = dt.Date.Add(activeTime);
            // var filledDateTime = dt.Date.Add(cancelTime);
            /*
                DateTime expireDate;
                if( i1 > 1800)
                {
                    var i2 = int.Parse(sExpireDate.Substring(4, 2));
                    var i3 = int.Parse(sExpireDate.Substring(6, 2));
                    expireDate = new DateTime(i1, i2, i3);    
                }
                */
            var operation = (short)(iIsSell == 0 ? +1 : -1);
            var transId = Convert.ToInt64(dwTransId);

            OrderStatusEnum status;
            switch (iStatus)
            {
                case 1:
                    status = OrderStatusEnum.Activated;
                    break;
                case 2:
                    status = OrderStatusEnum.Canceled;
                    break;
                default:
                    status = OrderStatusEnum.Filled;
                    break;
            }
            //var ord = GetOrderOrNull(General.Key(account, number));
            //var ord = GetOrderOrNull(account, number);

            var ord = OrdersActivated.GetByKey((number + "." + account).TrimUpper());
            if (ord != null)
            {
                // ACTIVATED OR CONFIRMED
                var area = "In Activated";

                var stOld = ord.Status;
                var trIdOld = ord.TransId;

                //ord.Quantity = quantity;
                ord.Rest = rest;
                ord.Status = status;
                //ord.SetStatus(status, string.Empty);
                switch (status)
                {
                    case OrderStatusEnum.Activated:
                        ord.ExActivateTime = activeTime;
                        ord.Activated = activateDateTime;
                        break;
                    case OrderStatusEnum.Canceled:
                        ord.ExCancelTime = cancelTime;
                        ord.Canceled = cancelDateTime;
                        break;
                    case OrderStatusEnum.Filled:
                        ord.FilledTime = DateTime.Now.TimeOfDay;
                        ord.Filled = filledDateTime;
                        break;
                }
                if (status == OrderStatusEnum.Filled || status == OrderStatusEnum.Canceled)
                {
                    //OnTradeEntityChangedEvent(new EventArgs
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "DELETE",
                        Object = ord
                    });
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "ADD",
                        Object = ord
                    });

                    SetBusyStatusWnenStratEmpty(ord, BusyStatusEnum.ReadyToUse);

                    FireChangedEventToStrategy(ord.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "COMPLETED");
                    OrdersActivated.Remove(ord.Key);
                    OrdersCompleted.Add(ord);
                }
                if (status == OrderStatusEnum.Activated) // && stOld == OrderStatusEnum.Confirmed)
                {
                    SetBusyStatusWnenStratEmpty(ord, BusyStatusEnum.ReadyToUse);
                    FireChangedEventToStrategy(ord.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "ACTIVATED");
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "UPDATE",
                        Object = ord
                    });
                    OrdersActivated.Update(ord.Key, ord);
                }

                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StrategyTimeIntTickerString,
                //            "Order: " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                //            ord.Status + " " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                //            ord.ToString(), Code);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                    ord.StrategyTimeIntTickerString, ord.ShortInfo + $" {area}", $"{m} {ord.ShortDescription}", ord.ToString(), Code);

                if (ord.Strategy != null)
                    _tx.TradeStorage?.SaveChanges(StorageOperationEnum.AddOrUpdate, ord);
            }
            else
            {
                // NOT FOUND in Activated
                // Already Filled or New // Not Found in Active

                ord = OrdersCompleted.GetByKey((number + "." + account).TrimUpper());
                if (ord == null)
                {
                    var area = "Not In Completed";
                    // Not Found in Completed
                    // New Order
                    var oper = Order.OperationToEnum2(operation);
                    if (oper == OrderOperationEnum.Unknown)
                        throw new NullReferenceException("Order Operation Unknown");

                    var dtn = DateTime.Now;
                    var o = new Order3
                    {
                        AccountKey = account,
                        TickerBoardKey = classCode,
                        TickerKey = ticker,
                        TransId = (uint)transId,
                        Number = number,
                        DateTime = dtn,
                        Created = dtn,
                        Activated = dtn,
                        Filled = dtn,
                        Canceled = dtn,
                        Operation = oper,
                        OrderType = OrderTypeEnum.Limit,
                        Status = status,
                        StopPrice = 0,
                        LimitPrice = price,
                        Quantity = quantity,
                        BusyStatus = BusyStatusEnum.ReadyToUse
                        // ExActivateTime = activeTime
                    };
                    // FireChangedEventToStrategy(o.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "UNKNOWN");
                    if (status == OrderStatusEnum.Activated)
                    {
                        o.Activated = dtn;
                        //OnTradeEntityChangedEvent(new EventArgs
                        OnChangedEvent(new EventArgs
                        {
                            Category = "UI.ORDERS",
                            Entity = "ORDER",
                            Operation = "ADD",
                            Object = o
                        });
                        OrdersActivated.Add(o);
                    }
                    else
                    {
                        //OnTradeEntityChangedEvent(new EventArgs
                        OnChangedEvent(new EventArgs
                        {
                            Category = "UI.ORDERS",
                            Entity = "ORDER.COMPLETED",
                            Operation = "ADD",
                            Object = o
                        });
                        OrdersCompleted.Add(o);
                    }

                    //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                    //        "Order=" + o.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //        o.Status + " " + o.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //        o.Strategy != null ? o.Strategy.StrategyTickerString : "Strategy", o.ToString());

                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                        o.StrategyTimeIntTickerString, o.ShortInfo + $" {area}", $"{m} {o.ShortDescription}", o.ToString(), Code);

                }
                else // Already Filled
                {
                    var area = "In Completed";
                    //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                    //        "Order: " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //        ord.Status + " " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()
                    //            + "; Is Aready Filled", ord.Strategy != null ? ord.Strategy.StrategyTickerString : "Strategy",
                    //            ord.ToString());
                    SetBusyStatusWnenStratEmpty(ord, BusyStatusEnum.ReadyToUse);
                    FireChangedEventToStrategy(ord.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "COMPLETED");
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StrategyTimeIntTickerString,
                                            ord.ShortInfo + $" {area}", $"{m} {ord.ShortDescription}",
                                            ord.ToString(), Code);
                    if (ord.Strategy != null)
                        _tx.TradeStorage?.SaveChanges(StorageOperationEnum.AddOrUpdate, ord);
                    // OrdersCompleted.Remove(ord);
                }
                //AddOrder(o);
            }
        }
        public void OrderStatusReplyProcessing64(IEventArgs args)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(args.Object is OrderStatusReply64 orderStatusReply))
            {
                Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentName, Name,
                        m,
                        $"Args IS NOT {nameof(OrderStatusReply)}", "Sorry");
                return;
            }
            var dNumber = orderStatusReply.OrderNumber;
            var iDate = orderStatusReply.Date;
            var iTime = orderStatusReply.Time;
            var mode = orderStatusReply.Mode;
            var iActivationTime = orderStatusReply.ActivationTime;
            var iCancelTime = orderStatusReply.CancelTime;
            var iExpireDate = orderStatusReply.ExpireDate;
            var account = orderStatusReply.Account;
            var strategy = orderStatusReply.BrokerRef;
            var classCode = orderStatusReply.ClassCode;
            var ticker = orderStatusReply.Ticker;
            var iIsSell = orderStatusReply.IsSell;
            var quantity = orderStatusReply.Qty;
            var rest = orderStatusReply.Rest;
            var price = orderStatusReply.Price;
            var iStatus = orderStatusReply.Status;
            var dwTransId = orderStatusReply.TransId;
            var comment = orderStatusReply.ClientCode;

            var number = Convert.ToUInt64(dNumber);
            if (number == 0)
                return;

            if (classCode == "FUTEVN") return;

            //if (ClassCodeToRemoveLogin.HasValue())
            //    if (classCode.Contains(ClassCodeToRemoveLogin))
            //        strategy = strategy.Replace(LoginToRemove, "");

            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");

            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            var tm = new TimeSpan(int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            // ActiveTime May Be = 0, Shame upon Quik
            TimeSpan activeTime;
            if (iActivationTime != 0)
            {
                var sActTime = iActivationTime.ToString("D6");
                activeTime = new TimeSpan(
                    int.Parse(sActTime.Substring(0, 2)),
                    int.Parse(sActTime.Substring(2, 2)),
                    int.Parse(sActTime.Substring(4, 2)));
            }
            else
                activeTime = DateTime.Now.TimeOfDay;

            // CancelTime May Be = 0, Shame upon Quik
            TimeSpan cancelTime;
            if (iCancelTime != 0)
            {
                var sCancelTime = iCancelTime.ToString("D6");
                cancelTime = new TimeSpan(
                int.Parse(sCancelTime.Substring(0, 2)),
                int.Parse(sCancelTime.Substring(2, 2)),
                int.Parse(sCancelTime.Substring(4, 2)));
            }
            else
                cancelTime = DateTime.Now.TimeOfDay;

            var sExpireDate = iExpireDate.ToString("D8");
            var i1 = int.Parse(sExpireDate.Substring(0, 4));

            var activateDateTime = dt.Date.Add(activeTime);
            var cancelDateTime = dt.Date.Add(cancelTime);
            var filledDateTime = dt.Date.Add(DateTime.Now.TimeOfDay);
            // var filledDateTime = dt.Date.Add(activeTime);
            // var filledDateTime = dt.Date.Add(cancelTime);
            /*
                DateTime expireDate;
                if( i1 > 1800)
                {
                    var i2 = int.Parse(sExpireDate.Substring(4, 2));
                    var i3 = int.Parse(sExpireDate.Substring(6, 2));
                    expireDate = new DateTime(i1, i2, i3);    
                }
                */
            var operation = (short)(iIsSell == 0 ? +1 : -1);
            var transId = Convert.ToInt64(dwTransId);

            OrderStatusEnum status;
            switch (iStatus)
            {
                case 1:
                    status = OrderStatusEnum.Activated;
                    break;
                case 2:
                    status = OrderStatusEnum.Canceled;
                    break;
                default:
                    status = OrderStatusEnum.Filled;
                    break;
            }
            //var ord = GetOrderOrNull(General.Key(account, number));
            //var ord = GetOrderOrNull(account, number);

            var ord = OrdersActivated.GetByKey((number + "." + account).TrimUpper());
            if (ord != null)
            {
                // ACTIVATED OR CONFIRMED
                var area = "In Activated";

                var stOld = ord.Status;
                var trIdOld = ord.TransId;

                //ord.Quantity = quantity;
                ord.Rest = rest;
                ord.Status = status;
                //ord.SetStatus(status, string.Empty);
                switch (status)
                {
                    case OrderStatusEnum.Activated:
                        ord.ExActivateTime = activeTime;
                        ord.Activated = activateDateTime;
                        break;
                    case OrderStatusEnum.Canceled:
                        ord.ExCancelTime = cancelTime;
                        ord.Canceled = cancelDateTime;
                        break;
                    case OrderStatusEnum.Filled:
                        ord.FilledTime = DateTime.Now.TimeOfDay;
                        ord.Filled = filledDateTime;
                        break;
                }
                if (status == OrderStatusEnum.Filled || status == OrderStatusEnum.Canceled)
                {
                    //OnTradeEntityChangedEvent(new EventArgs
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "DELETE",
                        Object = ord
                    });
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "ADD",
                        Object = ord
                    });

                    SetBusyStatusWnenStratEmpty(ord, BusyStatusEnum.ReadyToUse);

                    FireChangedEventToStrategy(ord.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "COMPLETED");
                    OrdersActivated.Remove(ord.Key);
                    OrdersCompleted.Add(ord);
                }
                if (status == OrderStatusEnum.Activated) // && stOld == OrderStatusEnum.Confirmed)
                {
                    SetBusyStatusWnenStratEmpty(ord, BusyStatusEnum.ReadyToUse);
                    FireChangedEventToStrategy(ord.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "ACTIVATED");
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "UPDATE",
                        Object = ord
                    });
                    OrdersActivated.Update(ord.Key, ord);
                }

                //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StrategyTimeIntTickerString,
                //            "Order: " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                //            ord.Status + " " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                //            ord.ToString(), Code);

                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                    ord.StrategyTimeIntTickerString, ord.ShortInfo + $" {area}", $"{m} {ord.ShortDescription}", ord.ToString(), Code);

                if (ord.Strategy != null)
                    _tx.TradeStorage?.SaveChanges(StorageOperationEnum.AddOrUpdate, ord);
            }
            else
            {
                // NOT FOUND in Activated
                // Already Filled or New // Not Found in Active

                ord = OrdersCompleted.GetByKey((number + "." + account).TrimUpper());
                if (ord == null)
                {
                    var area = "Not In Completed";
                    // Not Found in Completed
                    // New Order
                    var oper = Order.OperationToEnum2(operation);
                    if (oper == OrderOperationEnum.Unknown)
                        throw new NullReferenceException("Order Operation Unknown");

                    var dtn = DateTime.Now;
                    var o = new Order3
                    {
                        AccountKey = account,
                        TickerBoardKey = classCode,
                        TickerKey = ticker,
                        TransId = (uint)transId,
                        Number = number,
                        DateTime = dtn,
                        Created = dtn,
                        Activated = dtn,
                        Filled = dtn,
                        Canceled = dtn,
                        Operation = oper,
                        OrderType = OrderTypeEnum.Limit,
                        Status = status,
                        StopPrice = 0,
                        LimitPrice = price,
                        Quantity = quantity,
                        BusyStatus = BusyStatusEnum.ReadyToUse
                        // ExActivateTime = activeTime
                    };
                    // FireChangedEventToStrategy(o.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "UNKNOWN");
                    if (status == OrderStatusEnum.Activated)
                    {
                        o.Activated = dtn;
                        //OnTradeEntityChangedEvent(new EventArgs
                        OnChangedEvent(new EventArgs
                        {
                            Category = "UI.ORDERS",
                            Entity = "ORDER",
                            Operation = "ADD",
                            Object = o
                        });
                        OrdersActivated.Add(o);
                    }
                    else
                    {
                        //OnTradeEntityChangedEvent(new EventArgs
                        OnChangedEvent(new EventArgs
                        {
                            Category = "UI.ORDERS",
                            Entity = "ORDER.COMPLETED",
                            Operation = "ADD",
                            Object = o
                        });
                        OrdersCompleted.Add(o);
                    }

                    //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                    //        "Order=" + o.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //        o.Status + " " + o.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //        o.Strategy != null ? o.Strategy.StrategyTickerString : "Strategy", o.ToString());

                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                        o.StrategyTimeIntTickerString, o.ShortInfo + $" {area}", $"{m} {o.ShortDescription}", o.ToString(), Code);

                }
                else // Already Filled
                {
                    var area = "In Completed";
                    //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                    //        "Order: " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //        ord.Status + " " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()
                    //            + "; Is Aready Filled", ord.Strategy != null ? ord.Strategy.StrategyTickerString : "Strategy",
                    //            ord.ToString());
                    SetBusyStatusWnenStratEmpty(ord, BusyStatusEnum.ReadyToUse);
                    FireChangedEventToStrategy(ord.Clone(), "ORDERS", "ORDER.STATUSCHANGED", "COMPLETED");
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, ord.StrategyTimeIntTickerString,
                                            ord.ShortInfo + $" {area}", $"{m} {ord.ShortDescription}",
                                            ord.ToString(), Code);
                    if (ord.Strategy != null)
                        _tx.TradeStorage?.SaveChanges(StorageOperationEnum.AddOrUpdate, ord);
                    // OrdersCompleted.Remove(ord);
                }
                //AddOrder(o);
            }
        }
        public IOrder3 GetOrderByKey(string orderKey)
        {
            var key = orderKey.TrimUpper();
            var ord = OrdersActivated.GetByKey(key);
            return ord ?? OrdersCompleted.GetByKey(key);
        }
        public IEnumerable<IOrder3> GetActiveOrders()
        {
            return OrdersActivated.Items;
        }
        public IEnumerable<IOrder3> GetStartegyActiveOrders(string stratkey)
        {
            return OrdersActivated.Items
                .Where(o => o.Strategy != null && o.Strategy.Key == stratkey &&
                            o.Status != OrderStatusEnum.Rejected);
        }

    }
}
