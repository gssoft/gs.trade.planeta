using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Interfaces;
using EventArgs = GS.Events.EventArgs;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {

        public  BlockingCollection<IQuikTransaction> _orderTransactionBlQueue;

        public Queue<string> OrderQueue { get; set; }
        public Queue<IQuikTransaction> OrderTransactionQueue { get; set; }

        public const int MaxTransactionNumber = 10;

        private void EnQueOrder(string orderStr)
        {
            lock (((ICollection)OrderQueue).SyncRoot)
            {
                OrderQueue.Enqueue(orderStr);
            }
        }
        public IEnumerable<string> DequeOrders(int count)
        {
            if (OrderQueue.Count == 0)
                return null;
            var l = new List<string>();
            lock (((ICollection)OrderQueue).SyncRoot)
            {
                var cnt = Math.Min(OrderQueue.Count, count);
                for (var i = 0; i < cnt; i++)
                    l.Add(OrderQueue.Dequeue());
                //l.AddRange(OrderQueue.Take(10));
            }
            return l;
        }

        public void SendOrdersFromQueue()
        {
            //Test_FireTransaction();
            _orderLockedCount.Clear();

            int orderCount;
            lock (((ICollection)OrderQueue).SyncRoot)
            {
                orderCount = OrderQueue.Count;
            }
            if (orderCount == 0)
                return;
            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "OrderQueue", "OrderQueue", "Count=" + orderCount, DllNamePath2QuikPair, "");

            var ordersToSend = DequeOrders(MaxTransactionNumber);
            if (ordersToSend == null)
                return;
            var i = 0;
            if (!IsConnectedNow)
                return;
            foreach (var o in ordersToSend)
            {
                var ret = SendAsyncTransaction(o);
                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "OrderQueue", "OrderQueue: " + ++i + " " + Name, "SendOrderFromQueue: " + ret, o, DllNamePath2QuikPair);
            }
            // Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "OrderQueueCount= " + Name, "SOrderFromQueue: " + ret, o, DllNamePath2QuikPair);
        }
        // ************* OrderTransaction Queue
        private void EnQueTransaction(IQuikTransaction t)
        {
            lock (((ICollection)OrderTransactionQueue).SyncRoot)
            {
                OrderTransactionQueue.Enqueue(t);
            }
        }
        public IList<IQuikTransaction> DequeTransactions(int count)
        {
            //if (OrderTransactionQueue.Count == 0)
            //    return null;
            var l = new List<IQuikTransaction>();
            lock (((ICollection)OrderTransactionQueue).SyncRoot)
            {
                var cnt = Math.Min(OrderTransactionQueue.Count, count);
                for (var i = 0; i < cnt; i++)
                    l.Add(OrderTransactionQueue.Dequeue());
                //l.AddRange(OrderQueue.Take(10));
            }
            return l;
        }
        public void SendOrderTransactionsFromQueue()
        {
            _orderLockedCount.IsEnabled = false;

            var ordToSend = MaxTransactionNumber - _orderLockedCount.Count;
            if (ordToSend <= 0)
            {
                _orderLockedCount.IsEnabled = false; // норму сделали по предаче - подождите - теперь только в очередь
                _orderLockedCount.Clear();
                return;
            }

            int orderCount;
            //lock (((ICollection)OrderTransactionQueue).SyncRoot)
            //{
            //    orderCount = OrderTransactionQueue.Count;
            //}
            //if (orderCount <= 0)
            //{
            //    _orderLockedCount.Clear();
            //    return;
            //}

            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                    "OrderTransactionQueue", "Count=" + orderCount, DllNamePath2QuikPair, "");

            var ordersToSend = DequeTransactions(ordToSend);
            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                    "OrderTransactionQueue", "Count=" + ordersToSend.Count(),
            //                    "LockerCount="+_orderLockedCount.Count , DllNamePath2QuikPair);

            //if (ordersToSend == null)
            //{
            //    _orderLockedCount.Clear();
            //    return;
            //}

            var i = 0;
            foreach (var t in ordersToSend)
            {
                if (IsConnectedNow)
                {
                    t.Comment += "SendAsyncQueued # " + i;
                    //_orderLockedCount.Inc();
                    var ret = SendAsyncTransaction(t);
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Name, "TransactionQueue: " + ++i + " " + Name,
                        "SendOrderFromQueue: " + ret, t.ToString(), DllNamePath2QuikPair);
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

                    Transactions.Remove(t);
                    // TransactionEventFire(t);
                    //OnTradeEntityChangedEvent( new Events.EventArgs
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Transactions",
                                    Entity = "Transaction",
                                    Operation = "Add",
                                    Object = t
                    });

                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, DllNamePath2QuikPair, "TransactionQueue",
                        "Clear OrderQueue: Item." + ++i, "Can't SendOrderFromQueue. TradeTerminal Is Not Connected", t.ToString());
                }
            }
            orderCount = OrderTransactionQueue.Count;
            if (orderCount > 0)
            {
                _orderLockedCount.IsEnabled = false;
                _orderLockedCount.Clear();
            }
            else
            {
                _orderLockedCount.IsEnabled = true;
                _orderLockedCount.Clear();
            }
            // Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "OrderQueueCount= " + Name, "SOrderFromQueue: " + ret, o, DllNamePath2QuikPair);
        }
        public void SendOrderTransactionsFromQueue2()
        {
            _orderLockedCount.IsEnabled = false;
            _orderLockedCount.Clear();

            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                    "OrderTransactionQueue", "Count=" + orderCount, DllNamePath2QuikPair, "");

            var ordersToSend = DequeTransactions(MaxTransactionNumber);
            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "OrderTransactionQueue",
                                "OrderTransactionQueue", "Count=" + ordersToSend.Count(),
                                "LockerCount=" + _orderLockedCount.Count, DllNamePath2QuikPair);
            //  _orderLockedCount.Clear();

            //if (ordersToSend == null)
            //{//_orderLockedCount.Clear();
            //    return;
            //}
            var i = 0;
            foreach (var t in ordersToSend)
            {
                if (IsConnectedNow)
                {
                    t.Comment += "SendAsyncQueued # " + i;

                    _orderLockedCount.Inc();
                    var ret = SendAsyncTransaction(t);
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Name, "TransactionQueue: " + ++i + " " + Name,
                        "SendOrderFromQueue: " + ret, t.ToString(), DllNamePath2QuikPair);
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

                    Transactions.Remove(t);
                    // TransactionEventFire(t);
                    //OnTradeEntityChangedEvent(new Events.EventArgs
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Transactions",
                        Entity = "Transaction",
                        Operation = "Add",
                        Object = t
                    });

                    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, DllNamePath2QuikPair, "TransactionQueue",
                        "Clear OrderQueue: Item." + ++i, "Can't SendOrderFromQueue. TradeTerminal Is Not Connected", t.ToString());
                }
            }
            _orderLockedCount.IsEnabled = true;
            // Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "OrderQueueCount= " + Name, "SOrderFromQueue: " + ret, o, DllNamePath2QuikPair);
        }

        private void ClearOrderTransactionQueue()
        {
            var l = new List<IQuikTransaction>();
            lock (((ICollection)OrderTransactionQueue).SyncRoot)
            {
                var count = OrderTransactionQueue.Count;
                for (var i = 0; i < count; i++)
                    l.Add(OrderTransactionQueue.Dequeue());
            }
            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "QuikTerminal:",  Name,
                        "Clear OrderQueue. TotalItems:" + l.Count, "", DllNamePath2QuikPair);

            foreach (var t in l)
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
                Transactions.Remove(t);
                //TransactionEventFire(t);
                //OnTradeEntityChangedEvent(new Events.EventArgs
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "Transactions",
                    Entity = "Transaction",
                    Operation = "Add",
                    Object = t
                });
            }
        }

        public void EnQueBlTransaction(IQuikTransaction t )
        {
                _orderTransactionBlQueue.Add(t);
        }
        // LAST WORKING METHOD 14.03.25
        public void SendOrderTransactionsFromBlQueue()
        {
            try
            {
                var dt = DateTime.Now;
                _orderLockedCount2.NewTime(dt);

                var cnt = _orderTransactionBlQueue.Count;
                if (cnt <= 0)
                    return;
                
                if (!_orderLockedCount2.Inc())
                    return;
                var i = 0;
                //foreach (var t in _orderTransactionBlQueue.GetConsumingEnumerable())
                //{
                    IQuikTransaction t;
                    while( _orderTransactionBlQueue.TryTake(out t))
                    {
                    try
                    {
                        if (IsConnectedNow)
                        {
                            t.Comment += "SendAsyncQueued #" + ++i + "From:" + cnt;
                            var ret = SendAsyncTransaction(t);
                            if(ret)
                            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, "TransactionQueue:",
                                "TransactionQueue: " + i + " " + "From:" + cnt,
                                "SendOrderFromQueue: " + ret, t.ToString(), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);
                            else
                            {
                                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "TransactionQueue:",
                                "TransactionQueue: " + i + " " + "From:" + cnt,
                                "SendOrderFromQueue: " + ret, t.ToString(), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);
                                
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

                            Transactions.Remove(t);
                            // TransactionEventFire(t);
                            //OnTradeEntityChangedEvent(new Events.EventArgs
                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "Transactions",
                                Entity = "Transaction",
                                Operation = "Add",
                                Object = t
                            });

                            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, DllNamePath2QuikPair, "TransactionQueue",
                                "Clear OrderQueue: Item." + ++i, "Can't SendOrderFromQueue. TradeTerminal Is Not Connected", t.ToString());
                        }
                        if (!_orderLockedCount2.Inc())
                            break;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("SendFromOrderBlockinCollection().ItemProcess Failure");
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("SendFromOrderBlockinCollection Failure");
            }
        }
        public void SendOrderTransactionsFromBlQueue2()
        {
            try
            {
                var dt = DateTime.Now;
                _orderLockedCount2.NewTime(dt);

                var cnt = _orderTransactionBlQueue.Count;
                //if (cnt <= 0)
                //    return;
                var i = 0;
                if (!_orderLockedCount2.Inc())
                    return;
                foreach (var t in _orderTransactionBlQueue.GetConsumingEnumerable())
                {
                    try
                    {
                        if (IsConnectedNow)
                        {
                            t.Comment += "SendAsyncQueued #" + ++i + "From:" + cnt;
                            var ret = SendAsyncTransaction(t);
                            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Name,
                                "TransactionQueue: " + i + " " + Name,
                                "SendOrderFromQueue: " + ret, t.ToString(), DllNamePath2QuikPair);
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

                            Transactions.Remove(t);
                            // TransactionEventFire(t);
                            //OnTradeEntityChangedEvent(new Events.EventArgs
                            OnChangedEvent(new Events.EventArgs
                            {
                                Category = "Transactions",
                                Entity = "Transaction",
                                Operation = "Add",
                                Object = t
                            });

                            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, DllNamePath2QuikPair, "TransactionQueue",
                                "Clear OrderQueue: Item." + ++i, "Can't SendOrderFromQueue. TradeTerminal Is Not Connected", t.ToString());
                        }
                        if (!_orderLockedCount2.Inc())
                            break;
                    }
                    catch (Exception e)
                    {
                        throw new Exception("SendFromOrderBlockinCollection().ItemProcess Failure");
                    }

                }
            }
            catch (Exception e)
            {
                throw new Exception("SendFromOrderBlockinCollection Failure");
            }
        }

    }
}
