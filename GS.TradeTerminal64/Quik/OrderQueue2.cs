using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Interfaces;
using GS.Queues;
using GS.Status;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public QuikTransactionQueue TransactionQueue;
        public QuikTransactionQueue CancelOrderTransactionQueue;

        // ProcessTask 2018.05.14
        public void SendTransaction(IQuikTransaction t)
        {
            if (t == null)
                return;

            if (IsProcessTaskInUse)
            {
                var eargs = new EventArgs1
                {
                    Process = "QuikTransactionsProcess",
                    Category = "QuikTransactions",
                    Entity = "TransactionSend",
                    Operation = "AddOrUpdate",
                    Object = t,
                    ProcessingAction = SendTransactionFromQueue
                };
                if (ProcessTask != null)
                    ProcessTask?.EnQueue(eargs);
                else
                    TransactionQueue.Push(t);
            }
            else
                TransactionQueue.Push(t);
        }
        public void ClearTransactionFromQueue()
        {
            if (IsProcessTaskInUse)
            {
                var eargs = new EventArgs1
                {
                    Process = "QuikTransactionsProcess",
                    Category = "QuikTransactions",
                    Entity = "TransactionClear",
                    Operation = "AddOrUpdate",
                    ProcessingAction = ClearOrderTransactionQueue3
                };
                if (ProcessTask != null)
                    ProcessTask.EnQueue(eargs);
                else
                    ClearOrderTransactionQueue3(null);
            }
            else
                ClearOrderTransactionQueue3(null);
        }
        public void SendTransaction2(IQuikTransaction t)
        {
            if(t.IsSetOrderTransaction)
                TransactionQueue.Push(t);
            else if(t.IsCancelOrderTransaction)
                CancelOrderTransactionQueue.Push(t);
        }
        // was before 30.04.2018
        public void SendOrderTransactionsFromQueue3()
        {
            try
            {
                if (TransactionQueue.IsEmpty)
                    return;

                // CheckForTransNotCompleted();

                var cnt = TransactionQueue.Count;
                var i = 0;
                while (_orderLockedCount2.Inc2() && TransactionQueue.Get(out var t))
                {
                    i++;
                    try
                    {
                        if (IsConnectedNow)
                        {
                            t.Comment += "SendAsyncQueued #" + i + "From:" + cnt;
                            var ret = SendAsyncTransaction(t);
                            if (ret)
                                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "TransactionQueue:",
                                    "TransactionQueue: " + i + " " + "From:" + cnt,
                                    "SendOrderFromQueue: " + true, t.ToString(), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);
                            else
                            {
                                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "TransactionQueue:",
                                "TransactionQueue: " + i + " " + "From:" + cnt,
                                "SendOrderFromQueue: " + false, t.ToString(), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);
                            }
                        }
                        else
                        {
                            t.QuikResult = T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED;
                            t.Status = TransactionStatus.NotSended;

                            t.Result = TransactionResult.Failure;
                            //t.ExtendedErrorCode = extendedErrorCode;

                            if (t.Action > 0)
                            {
                                t.Order.Status = OrderStatusEnum.Rejected;
                                t.Order.ErrorMsg = OrderErrorMsg.NotSended;
                            }

                            t.Comment += "SendAsyncQueued Transaction Failure: TradeTerminal Is Not Connected";

                            // ????????????????????
                            Transactions.Remove(t);
                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "Transactions",
                                Entity = "Transaction",
                                Operation = "Add",
                                Object = t
                            });

                            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, DllNamePath2QuikPair, "TransactionQueue",
                                "Clear OrderQueue: Item." + i, "Can't SendOrderFromQueue. TradeTerminal Is Not Connected", t.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName, t.GetType().ToString(), "SendOrdersFromQueue()", t.ToString(),e);
                        //throw;
                    }
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "SendOrdersFromQueue()", ToString(), e);
                //throw;
            }
        }

        public void SendOrderTransactionsFromQueue32()
        {
            try
            {
                if (TransactionQueue.IsEmpty)
                    return;

                // CheckForTransNotCompleted();

                var cnt = TransactionQueue.Count;
                var i = 0;
                while (_orderLockedCount2.Inc2() && TransactionQueue.Get(out var t))
                {
                    i++;
                    try
                    {
                        
                        t.Comment += "SendAsyncQueued #" + i + "From:" + cnt;
                        var ret = SendAsyncTransaction32(t);
                        if (ret)
                            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "TransactionQueue:",
                                "TransactionQueue: " + i + " " + "From:" + cnt,
                                "SendOrderFromQueue: " + true, t.ToString(),
                                t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);
                        else
                        {
                            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "TransactionQueue:",
                            "TransactionQueue: " + i + " " + "From:" + cnt,
                            "SendOrderFromQueue: " + false, t.ToString(),
                            t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);
                        }
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName, t.GetType().ToString(), "SendOrdersFromQueue()", t.ToString(), e);
                        //throw;
                    }
                }             
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "SendOrdersFromQueue()", ToString(), e);
                //throw;
            }
        }

        // 02.05/201 in Use
        public void SendOrderTransactionsFromQueue33()
        {
            try
            {
                if (TransactionQueue.IsEmpty)
                    return;

                // CheckForTransNotCompleted();
                var cnt = TransactionQueue.Count;

                Evlm2(EvlResult.INFO, EvlSubject.TRADING, "SendOrderFromQueue",
                "Transactions.Count Before: " + cnt, "", "", "");

                IQuikTransaction t = null;
                
                var i = 0;
                while (_orderLockedCount2.Inc2() )
                {
                    i++;
                    
                    try
                    {
                        if(!TransactionQueue.Get(out t))
                                    break;

                       // t.Comment += "SendAsyncQueued #" + i + "From:" + cnt;
                        t.Comment = "SendTransRoot; ";

                        if (IsWellConnected)
                        {
                            Transactions.Add(t);

                            t.Status = TransactionStatus.TryToSend;

                            var result = _t2Q.SendAsyncTransaction(t.TransactionString, out var extendedErrorCode);

                            IsConnectedNow = result != (Int32) T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED &&
                                             result != (Int32) T2QResults.TRANS2QUIK_DLL_NOT_CONNECTED;

                            t.QuikResult = (T2QResults) result;
                            {
                                if (result == (int) T2QResults.TRANS2QUIK_SUCCESS)
                                {
                                    t.Comment += "Part 1; ";
                                    t.Result = TransactionResult.Success;
                                    t.ExtendedErrorCode = extendedErrorCode;

                                    t.Status = TransactionStatus.Sended;
                                    t.SendedDT = DateTime.Now;

                                    if (t.Action > 0 && t.Order != null)
                                    {
                                        t.Order.Status = OrderStatusEnum.Sended;
                                        t.Order.Sended = DateTime.Now;
                                    }

                                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "SendOrderFromQueue: " + true,
                                        "TransactionQueue: " + i + " " + "From:" + cnt,
                                        ResultToString(result), $"ExtErrCode: {extendedErrorCode}", t.ToString());
                                }
                                // NOT SENDED BY ANY REASON
                                else
                                {
                                    t.Comment += "Part 2; ";
                                    t.Status = TransactionStatus.NotSended;
                                    t.SendedDT = DateTime.Now;

                                    t.Result = TransactionResult.Failure;
                                    t.ExtendedErrorCode = extendedErrorCode;

                                    if (t.Action > 0 && t.Order != null)
                                    {
                                        t.Order.Status = OrderStatusEnum.Rejected;
                                        t.Order.ErrorMsg = OrderErrorMsg.NotSended;
                                    }

                                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "SendOrderFromQueue: " + false,
                                        "TransactionQueue: " + i + " " + "From:" + cnt,
                                        ResultToString(result), $"ExtErrCode: {extendedErrorCode}", t.ToString());

                                    Transactions.Remove(t);
                                    OnChangedEvent(new Events.EventArgs
                                    {
                                        Category = "Transactions",
                                        Entity = "Transaction",
                                        Operation = "Add",
                                        Object = t
                                    });
                                }
                            }
                            // continue;
                        }
                        else
                        {
                            t.Comment += "Part 3";
                            t.QuikResult = T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED;
                            t.Status = TransactionStatus.NotSended;
                            t.SendedDT = DateTime.Now;

                            t.Result = TransactionResult.Failure;
                            //t.ExtendedErrorCode = extendedErrorCode;

                            if (t.Action > 0)
                            {
                                t.Order.Status = OrderStatusEnum.Rejected;
                                t.Order.ErrorMsg = OrderErrorMsg.NotSended;
                            }

                            t.Comment += "SendAsyncQueued Transaction Failure: TradeTerminal Is Not Connected";

                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "Transactions",
                                Entity = "Transaction",
                                Operation = "Add",
                                Object = t
                            });

                            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "SendOrderFromQueue: " + false,
                                "TransactionQueue: " + i + " " + "From:" + cnt,
                                T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED.ToString(), "", t.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName, t?.GetType().ToString(),
                                                "SendOrdersFromQueue()", t?.ToString(), e);
                        //throw;
                    }
                }
                Evlm2(EvlResult.INFO, EvlSubject.TRADING, "SendOrderFromQueue: " + false,
                                "Transactions.Count After: " + Transactions.Count, "", "", "");
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "SendOrdersFromQueue()", ToString(), e);
                //throw;
            }
        }

        public void SendOrderTransactionsFromQueue33(QuikTransactionQueue queue,
                                                        QuikTransactions sendedTransactions)
        {
            try
            {
                if (queue.IsEmpty)
                    return;

                var cnt = queue.Count;

                Evlm2(EvlResult.INFO, EvlSubject.TRADING, "SendOrderFromQueue",
                $"TransactionsQueue.Count Before: {cnt}",
                $"Transactions.Count Before: {sendedTransactions.Count}", "", "");

                IQuikTransaction t = null;

                var i = 0;
                while (_orderLockedCount2.Inc2())
                {
                    i++;
                    try
                    {
                        if (!queue.Get(out t))
                            break;

                        // t.Comment += "SendAsyncQueued #" + i + "From:" + cnt;
                        t.Comment = "SendTransRoot; ";

                        if (IsWellConnected)
                        {
                            sendedTransactions.Add(t);

                            t.Status = TransactionStatus.TryToSend;

                            var result = _t2Q.SendAsyncTransaction(t.TransactionString, out var extendedErrorCode);

                            IsConnectedNow = (result != (Int32)T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED) &&
                                             (result != (Int32)T2QResults.TRANS2QUIK_DLL_NOT_CONNECTED);

                            t.QuikResult = (T2QResults)result;
                            {
                                if (result == (int)T2QResults.TRANS2QUIK_SUCCESS)
                                {
                                    t.Comment += "Part 1; ";
                                    t.Result = TransactionResult.Success;
                                    t.ExtendedErrorCode = extendedErrorCode;

                                    t.Status = TransactionStatus.Sended;
                                    t.SendedDT = DateTime.Now;

                                    if (t.Action > 0 && t.Order != null)
                                    {
                                        t.Order.Status = OrderStatusEnum.Sended;
                                        t.Order.Sended = DateTime.Now;
                                    }

                                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "SendOrderFromQueue: " + true,
                                        "TransactionQueue: " + i + " " + "From:" + cnt,
                                        ResultToString(result), $"ExtErrCode: {extendedErrorCode}", t.ToString());
                                }
                                // NOT SENDED BY ANY REASON
                                else
                                {
                                    sendedTransactions.Remove(t);

                                    t.Comment += "Part 2; ";
                                    t.Status = TransactionStatus.NotSended;
                                    t.SendedDT = DateTime.Now;

                                    t.Result = TransactionResult.Failure;
                                    t.ExtendedErrorCode = extendedErrorCode;

                                    if (t.Action > 0 && t.Order != null)
                                    {
                                        t.Order.Status = OrderStatusEnum.NotSended;
                                        t.Order.ErrorMsg = OrderErrorMsg.AnyReason;
                                    }

                                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "SendOrderFromQueue: " + false,
                                        "TransactionQueue: " + i + " " + "From:" + cnt,
                                        ResultToString(result), $"ExtErrCode: {extendedErrorCode}", t.ToString());
 
                                    OnChangedEvent(new Events.EventArgs
                                    {
                                        Category = "Transactions",
                                        Entity = "Transaction",
                                        Operation = "Add",
                                        Object = t
                                    });
                                }
                            }
                            // continue;
                        }
                        else
                        {
                            t.Comment += "Part 3";
                            t.QuikResult = T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED;
                            t.Status = TransactionStatus.NotSended;
                            t.SendedDT = DateTime.Now;

                            t.Result = TransactionResult.Failure;
                            //t.ExtendedErrorCode = extendedErrorCode;

                            if (t.Action > 0)
                            {
                                t.Order.Status = OrderStatusEnum.NotSended;
                                t.Order.ErrorMsg = OrderErrorMsg.NotConnected;
                            }

                            t.Comment += "SendAsyncQueued Transaction Failure: TradeTerminal Is Not Connected";

                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "Transactions",
                                Entity = "Transaction",
                                Operation = "Add",
                                Object = t
                            });

                            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "SendOrderFromQueue: " + false,
                                "TransactionQueue: " + i + " " + "From:" + cnt,
                                T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED.ToString(), "", t.ToString());
                        }
                    }
                    catch (Exception e)
                    {
                        SendExceptionMessage3(FullName, t?.GetType().ToString(),
                                                "SendOrdersFromQueue()", t?.ToString(), e);
                        //throw;
                    }
                }
                Evlm2(EvlResult.INFO, EvlSubject.TRADING, "SendOrderFromQueue()",
                    $"TransactionsQueue.Count After: {queue.Count}",
                    $"Transactions.Count Before: {sendedTransactions.Count}", "", "");
                    
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, GetType().ToString(), "SendOrdersFromQueue()", ToString(), e);
                //throw;
            }
        }

        public string TransactionSendResultStr(int result, int extendedErrorCode)
        {
            return $"Result: {ResultToString(result)}, ExtErrCode: {extendedErrorCode}";
        }

        // 03.05.2018
        public void SendTransactionsFromQueue()
        {
            if (!Transactions.IsEmpty || !CancelOrderTransactions.IsEmpty)
                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "SendTransactionsFromQueue",
                    $"OrderPendingTransactions: {Transactions.Count}",
                    $"CancelPendingTransactios: {CancelOrderTransactions.Count}", "", "");

            if (!TransactionQueue.IsEmpty || !CancelOrderTransactionQueue.IsEmpty)
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, "SendTransactionsFromQueue",
                    $"OrderTransactionQueue: {TransactionQueue.Count}",
                    $"CancelOrderTransactionQueue: {CancelOrderTransactionQueue.Count}", "", "");

            if (!CancelOrderTransactionQueue.IsEmpty && CancelOrderTransactions.IsEmpty)
                SendOrderTransactionsFromQueue33(CancelOrderTransactionQueue, CancelOrderTransactions);

            if (!TransactionQueue.IsEmpty && CancelOrderTransactions.IsEmpty  && Transactions.IsEmpty)
                SendOrderTransactionsFromQueue33(TransactionQueue, Transactions);
            
            //if(!Transactions.IsEmpty || !CancelOrderTransactions.IsEmpty)
            //    Evlm2(EvlResult.INFO, EvlSubject.TRADING, "SendTransactionsFromQueue",
            //        $"OrderTransactions: {Transactions.Count}",
            //        $"Cancel: {CancelOrderTransactions.Count}","","");           
        }

        // 2018.05.13
        public void SendTransactionFromQueue(IEventArgs1 args)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(args?.Object is IQuikTransaction t))
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, ParentName, Name,
                       m, "Transaction is Null", "Good Bye");
                return;
            }
            if (t.Order == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, t.StrategyStr,
                        t.ShortInfo, $"{m} Order is Null", t.ShortDescription, t.ToString());
                return;
            }
            t.Comment = "SendTransRoot; ";
            if (IsWellConnected)
            {
                Transactions.Add(t);

                t.Status = TransactionStatus.TryToSend;

                var result = _t2Q.SendAsyncTransaction(t.TransactionString, out var extendedErrorCode);

                IsConnectedNow = (result != (Int32) T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED) &&
                                 (result != (Int32) T2QResults.TRANS2QUIK_DLL_NOT_CONNECTED);

                t.QuikResult = (T2QResults) result;

                if (result == (int) T2QResults.TRANS2QUIK_SUCCESS)
                {
                    t.Comment += "Part 1; ";
                    t.Result = TransactionResult.Success;
                    t.ExtendedErrorCode = extendedErrorCode;

                    t.Status = TransactionStatus.Sended;
                    t.SendedDT = DateTime.Now;
                    
                    t.Order.TransactionStatus = OrderStatusEnum.Sended;
                    t.Order.ErrorMsg = OrderErrorMsg.Ok;
                    t.Order.Sended = t.SendedDT;

                    var orderClone = t.Order.Clone();
                    FireChangedEventToStrategy(orderClone, "Orders", "ORDER.TRANSSEND", "Sended");
                    
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, orderClone.StrategyTimeIntTickerString,
                        orderClone.ShortInfo, $"{m} {t.QuikResult}",
                        TransactionSendResultStr(result, extendedErrorCode), t.ToString());

                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Transactions",
                        Entity = "Transaction",
                        Operation = "Add",
                        Object = t
                    });
                }
                // NOT SENDED BY ANY REASON
                else
                {
                    Transactions.Remove(t);

                    t.Comment += "Part 2; ";
                    t.Status = TransactionStatus.NotSended;
                    t.SendedDT = DateTime.Now;

                    t.Result = TransactionResult.Failure;
                    t.ExtendedErrorCode = extendedErrorCode;
                    t.ErrorReason = ErrorReason.UnknownReason;
                    
                    t.Order.TransactionStatus = OrderStatusEnum.NotSended;
                    t.Order.ErrorMsg = OrderErrorMsg.UnknownSendTransFailure;

                    if (t.Order.TransactionAction == OrderTransactionActionEnum.SetOrder)
                    {
                        t.Order.Status = OrderStatusEnum.Rejected;
                    }
                    FireChangedEventToStrategy(t.Order, "Orders", "ORDER.TRANSSEND", "NotSended");

                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, t.Order?.StrategyTimeIntTickerString,
                        t.Order?.ShortInfo, $"{m} {t.QuikResult}",
                        TransactionSendResultStr(result, extendedErrorCode), t.ToString());
                    
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Transactions",
                        Entity = "Transaction",
                        Operation = "Add",
                        Object = t
                    });
                }
            }
            // NOT CONNECTION
            else
            {
                t.Comment += "Part 3";
                t.QuikResult = T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED;
                t.Status = TransactionStatus.NotSended;
                t.SendedDT = DateTime.Now;

                t.Result = TransactionResult.Failure;
                t.ErrorReason = ErrorReason.NotConnected;
                
                t.Order.TransactionStatus = OrderStatusEnum.NotSended;
                t.Order.ErrorMsg = OrderErrorMsg.NotConnected;

                // CancelOrder Transaction does not tauch Order in ActiveOrders
                // Only Set Order should be Rejected
                if (t.Order.TransactionAction == OrderTransactionActionEnum.SetOrder)
                {
                    t.Order.Status = OrderStatusEnum.Rejected;
                }
                FireChangedEventToStrategy(t.Order, "Orders", "ORDER.TRANSSEND", "NotSended");

                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, t.Order?.StrategyTimeIntTickerString,
                        t.Order?.ShortInfo, $"{m} {t.QuikResult}",
                        T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED.ToString(), t.ToString());
                
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "Transactions",
                    Entity = "Transaction",
                    Operation = "Add",
                    Object = t
                });               
            }
        }
        // Work with CheckConnection when Connection Lost We Are Clear Transaction Queue
        private void ClearOrderTransactionQueue3(IEventArgs1 args)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (IsWellConnected) return;

            var l = TransactionQueue.GetItems().ToList();            
            
            if (l.Count <= 0) return;

            foreach (var t in l)
            {
                t.QuikResult = T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED;
                t.Status = TransactionStatus.NotSended;
                t.Result = TransactionResult.Failure;
                t.ErrorReason = ErrorReason.NotConnected;

                if (t.Order != null)
                {
                    t.Order.TransactionStatus = OrderStatusEnum.NotSended;
                    t.Order.ErrorMsg = OrderErrorMsg.NotConnected;

                    // CancelOrder Transaction does not tauch Order in ActiveOrders
                    // Leave Active Orders to Cancel as Active
                    // Only Set Order should be Rejected
                    if (t.Order.TransactionAction == OrderTransactionActionEnum.SetOrder)
                    {
                        t.Order.Status = OrderStatusEnum.Rejected;
                    }
                    FireChangedEventToStrategy(t.Order, "Orders", "ORDER.TRANSSEND", "NotSended");
                }
                Transactions.Remove(t);

                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, t.Order?.StrategyTimeIntTickerString,
                    t.Order?.ShortInfo, $"{m} {OperationEnum.Remove}",
                    "TotalItems:" + l.Count, DllNamePath2QuikPair);

                OnChangedEvent(new Events.EventArgs
                {
                    Category = "Transactions",
                    Entity = "Transaction",
                    Operation = "Add",
                    Object = t
                });
            }
        }

        private void CheckForTransNotCompleted()
        {
            var dt = DateTime.Now;
            var trs = Transactions.Items
                        .Where(t => t.IsSended && !t.IsCompleted && t.SendedDT.AddSeconds(5) < dt);
            foreach(var t in trs)
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, "QuikTerminal", "SendTransaction()",
                    "Transaction NOT Completed", t.ToString() );          
        }
        public bool IsQueueEnabled { get; private set; }
        public void DeQueueProcess()
        {
            // SendOrderTransactionsFromQueue3();
            // 02.05.2018
            //SendOrderTransactionsFromQueue33();
            // 03.05.2018
            // SendTransactionsFromQueue();
            //04.05.2018

            SendOrderTransactionsFromQueue33(TransactionQueue, Transactions);
        }
    }
}
