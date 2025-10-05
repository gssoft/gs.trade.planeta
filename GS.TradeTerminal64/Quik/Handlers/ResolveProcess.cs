using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;
using GS.Interfaces;
using EventArgs = GS.Events.EventArgs;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        private const int WaitOrderResolveTimeInSec = 10;
        private const int WaitTradeResolveTimeInsec = 15;

        public void OrderResolveProcess()
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            Evlm1(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                    methodname,"We are working inside IdlingCycle", "Thank you");
            try
            {
                foreach (var o in OrdersActivated.Items)
                {
                    if (o.Strategy != null)
                        continue;

                    var s = _tx.GetStrategyByKey(o.Key);
                    if (s == null)
                    {
                        //var t = (DateTime.Now - o.Activated).Seconds;
                        //if (t < WaitOrderResolveTimeInSec)
                        //    continue;
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, DllNamePath2QuikPair,o.ShortInfo, methodname + 
                               " Try to Resolve OrderActivated", o.ShortDescription, o.ToString());

                        s = _tx.RegisterDefaultStrategy("Default", "Default",
                            o.AccountCode, o.TickerBoard, o.TickerCode, 60, Type.ToString(), Path2Quik);
                        if (s == null)
                        {
                            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, DllNamePath2QuikPair,o.ShortInfo, methodname +
                                " Failure in Register Default Strategy", o.ShortDescription, o.ToString());

                            OrdersActivated.Remove(o);
                            continue;
                        }
                    }
                    o.Strategy = s;
                    o.ErrorMsg = OrderErrorMsg.Ok;
                    o.Comment += " Resolved from Strategies.";

                    _tx.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, o);

                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "UPDATE",
                        Object = o
                    });
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "OrderResolveProcess()",
                                    "OrderResolveProcess() in OrdersActivated", "", e);
                // 01.05.2018
                // throw;
            }
            try
            {
                foreach (var o in OrdersCompleted.Items)
                {
                    if (o.Strategy != null || o.Number == 0)
                        continue;

                    var s = _tx.GetStrategyByKey(o.Key);
                    if (s == null)
                    {
                        //var t = (DateTime.Now - o.Activated).Seconds;
                        //if (t > WaitOrderResolveTimeInSec)
                        //{
                        //    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, o.ShortInfo, methodname,
                        //        $"Can't Resolve Order Completed in {WaitOrderResolveTimeInSec}Sec", o.ShortDescription ,o.ToString());
                        //    OrdersCompleted.Remove(o);
                        //    continue;
                        //}
                        Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, DllNamePath2QuikPair, o.ShortInfo, methodname +
                                " Try to Resolve OrderCompleted", o.ShortDescription, o.ToString());

                        s = _tx.RegisterDefaultStrategy("Default", "Default",
                            o.AccountCode, o.TickerBoard, o.TickerCode, 60, Type.ToString(), Path2Quik);
                        if (s == null)
                        {
                            Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, DllNamePath2QuikPair,o.ShortInfo, methodname +
                                " Failure in Register Default Strategy", o.ShortDescription, o.ToString());
                            OrdersCompleted.Remove(o);
                            continue;
                        }
                    }
                    o.Strategy = s;
                    o.ErrorMsg = OrderErrorMsg.Ok;
                    o.Comment += " Resolved from Strategies";

                    _tx.TradeStorage?.SaveChanges(StorageOperationEnum.AddOrUpdate, o);

                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "UPDATE",
                        Object = o
                    });
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "OrderResolveProcess()",
                                    "OrderResolveProcess() in OrdersCompleted", "", e);
                // throw;
            }
        }
        public void TradeResolveProcess2()
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";

            //Evlm1(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
            //    MethodBase.GetCurrentMethod().Name,
            //    "We are working inside IdlingCycle", "Thank you");
            try
            {
                foreach (var t in TradesToProcess.TradeConfirmed)
                {
                    var res = TryToResolveTrade(t);
                    if (!res)
                        continue;

                    // trade is resolved: t.Strategy != null
                    TradesToProcess.Remove(t);

                    //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                    //    "TradeResolveProcess2",
                    //    "Trades2: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                    //    "New Trade Confirmed by Order: " + t.OrderKey.WithSqBrackets0(), "");
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, DllNamePath2QuikPair,
                        t.Strategy.StrategyTimeIntTickerString, 
                        $"{m} Trade Resolved", t.ShortInfo, t.ToString());
                    /*
                    if (!t.Strategy.IsTradeNumberValid(t.Number))
                        // if (t.Number <= t.Strategy.Position.LastTradeNumber)
                    {
                        //Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                        //    t.Strategy.StrategyTickerString,
                        //    "Trade: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                        //    "TradeProcess3.NewTrade()",
                        //    "Old Trade Detected: " + t.Number, t.ToString());

                        t.Comment += " Old.";
                        OnChangedEvent(new Events.EventArgs
                        {
                            Category = "UI.Trades",
                            Entity = "Trade",
                            Operation = "Update",
                            Object = t
                        });
                        continue;
                    }
                    t.Comment += " New.";
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.Trades",
                        Entity = "Trade",
                        Operation = "Update",
                        Object = t
                    });
                    */
                    // t.Strategy?.NewTrade(t);
                    FireChangedEventToStrategy(t.Strategy,
                        t.Strategy.StrategyTimeIntTickerString, "TradeReply", "New", t);

                    if (TradesToProcess.Count > 0)
                        Evlm2(EvlResult.WARNING, EvlSubject.PROGRAMMING, ParentTypeName, TypeName,
                            MethodBase.GetCurrentMethod()?.Name,
                            $"TradesToResolve.Count: {TradesToProcess.Count} > 0", "");
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, m, m, "", e);
            }
        }
        private bool TryToResolveTrade(ITrade3 t)
        {
            var m = MethodBase.GetCurrentMethod()?.Name + "()";
            try
            {
                if (t.Status == TradeStatusEnum.Resolved && t.Strategy != null)
                    return true;

                // Try to Resolve in Orders Activated
                var ord = OrdersActivated.GetByKey(t.OrderKey);
                if (ord?.Strategy != null)
                {
                    t.Order = ord;
                    t.Strategy = ord.Strategy;
                    t.Comment += "Resolved from Orders.Activated";
                    t.Status = TradeStatusEnum.Resolved;
                    t.Resolved = DateTime.Now;
                    return true;
                }
                // Try to Resolve in Orders Completed
                ord = OrdersCompleted.GetByKey(t.OrderKey);
                if (ord?.Strategy != null)
                {
                    t.Order = ord;
                    t.Strategy = ord.Strategy;
                    t.Comment += "Resolved from Orders.Completed";
                    t.Status = TradeStatusEnum.Resolved;
                    t.Resolved = DateTime.Now;
                    return true;
                }
                if (t.Confirmed.AddSeconds(WaitTradeResolveTimeInsec).IsLessThan(DateTime.Now))
                {
                    Evlm2(EvlResult.WARNING, EvlSubject.PROGRAMMING, TypeName, t.GetType().Name,
                        MethodBase.GetCurrentMethod().Name,
                        $"Trade Does Not Resolve After {WaitTradeResolveTimeInsec}sec. Remove it:{t.Status}", t.ToString());

                    TradesToProcess.Remove(t);
                    return false;
                }
                //Evlm2(EvlResult.WARNING, EvlSubject.PROGRAMMING, TypeName, t.GetType().Name,
                //        MethodBase.GetCurrentMethod().Name,
                //        $"Trade Does Not Resolve, Try Resolve in 1 Second: {t.Status}", t.ToString());

                return false;
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, t.GetType().FullName, m, t.ToString(), e);
                return false;
            }
        }
    }
}
