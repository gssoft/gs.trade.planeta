using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Interfaces;

namespace GS.Trade.TradeTerminals.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public class TransactionReplyItem //: IProccesingAction<TransactionReplyItem>
        {
            public long TransId { get; set; }
            public ulong OrderNum { get; set; }
            public T2QResults Result { get; set; }
            public int ExtendedErrorCode { get; set; }
            public int ReplyCode { get; set; }
            public string ReplyMessage { get; set; }
            public DateTime ReplyDateTime { get; set; }
            public override string ToString()
            {
                return $"Trans:[{TransId}] Ord:[{OrderNum}] Rslt:{Result}" +
                       $" Err:{ExtendedErrorCode} ReplyCd:{ReplyCode} Msg:{ReplyMessage} RplDt:{ReplyDateTime}";
            }
            public string ShortInfo()
            {
                return $"Trans:[{TransId}] Rslt:{Result}" +
                       $" Err:{ExtendedErrorCode} ReplyCd:{ReplyCode} Msg:{ReplyMessage}";            }
            public string ShortDescription()
            {
                return ToString();
            }
        }
        private void SendTransactionReplyEventArgs(
           TransactionReplyItem transactionReply, Action<IEventArgs1> action)
        {
            var eargs = new EventArgs1
            {
                Process = "QuikTransactionsProcess",
                Category = "QuikTransactions",
                Entity = "TransactionReplyItem",
                Operation = "AddOrUpdate",
                Object = transactionReply,
                //ProcessingAction = TransactionReplyResolve
                ProcessingAction = action
            };
            if (ProcessTask != null)
                ProcessTask?.EnQueue(eargs);
            else
                action(eargs);
        }
        public void TransactionReply(Int32 nResult, Int32 nExtendedErrorCode, Int32 nReplyCode,
           UInt32 dwTransId, Double dOrderNum, [MarshalAs(UnmanagedType.LPStr)] string replyMessage)
        {
            var tri = new TransactionReplyItem
            {
                TransId = (long)dwTransId,
                // OrderNum = (long)dOrderNum,
                OrderNum = Convert.ToUInt64(dOrderNum),
                Result = (T2QResults)nResult,
                ExtendedErrorCode = nExtendedErrorCode,
                ReplyCode = nReplyCode,
                ReplyMessage = replyMessage,
                ReplyDateTime = DateTime.Now
            };
            SendTransactionReplyEventArgs(tri, TransactionReplyResolve);
            SendTransactionReplyEventArgs(tri, TransactionReplyItemTracking);
        }
        public void TransactionReplyResolve(IEventArgs1 args)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            var trReply = (TransactionReplyItem)args.Object;
            var dwTransId = trReply.TransId;
            var replyMessage = trReply.ReplyMessage;
            var nResult = trReply.Result;
            var nExtendedErrorCode = trReply.ExtendedErrorCode;
            var nReplyCode = trReply.ReplyCode;
            var dOrderNum = trReply.OrderNum;

            TransactionReplyEvent?.Invoke((int)nResult, (uint)dwTransId, dOrderNum);

            var t = Transactions.GetByKey((long)dwTransId);
            if (t == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TransactionReply",
                $"{(uint)dwTransId} Transaction NOT FOUND",
                replyMessage,
                $"Result={(T2QResults)nResult}, " +
                $"ExtErrCode={nExtendedErrorCode}, " +
                $"ReplyCode={nReplyCode}, " +
                $"TransID={dwTransId}, " +
                $"Order={dOrderNum}, " +
                $"Mess={replyMessage}", DllNamePath2QuikPair);
                // throw new NullReferenceException("TransactionReply: Transaction is Not Found");
                return;
            }
            t.Comment += "TransReply 1 ";
            t.OrderNumber = dOrderNum;

            t.Status = TransactionStatus.Completed;
            t.CompletedDT = DateTime.Now;

            t.Result = (T2QResults)nResult == T2QResults.TRANS2QUIK_SUCCESS
                ? TransactionResult.Success
                : TransactionResult.Failure;

            t.Message = replyMessage;
            t.QuikResult = (T2QResults)nResult;
            t.ExtendedErrorCode = nExtendedErrorCode;
            t.ReplyCode = nReplyCode;

            if (t.Order != null)
                t.Order.TransactionStatus = OrderStatusEnum.Completed;

            if (t.Order != null && t.IsSetOrderTransaction
               //&&
               //(t.Action == QuikTransactionActionEnum.SetLimit ||
               //t.Action == QuikTransactionActionEnum.SetStop ||
               //t.Action == QuikTransactionActionEnum.SetStopLimit)
               )
            {
                Transactions.Remove(t);

                t.OrderNumber = dOrderNum;
                t.Order.Number = t.OrderNumber;
                t.Order.Status = OrderStatusEnum.PendingToActivate;
                t.Order.TrMessage = t.Message;
                t.Order.TrReplyCode = nReplyCode;

                t.Order.TransactionStatus = OrderStatusEnum.Completed;

                if (t.OrderNumber != 0)
                    t.Order.ErrorMsg = OrderErrorMsg.Ok;
                else
                {
                    t.Order.Status = OrderStatusEnum.Rejected;
                    t.Order.ErrorMsg = OrderErrorMsg.OrderNumberIsEmpty;
                }
                switch (nReplyCode)
                {
                    case 3:
                        // t.Order.Status = OrderStatusEnum.PendingToActivate;
                        t.Order.Status = OrderStatusEnum.Activated;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "ACTIVATED");
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "REJECTED");
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        // Oredrs NOT Found Or Order is Filled or Canceled 
                        t.Order.ErrorMsg = OrderErrorMsg.Unknown;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "REJECTED");
                        break;
                    default:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.UnknownReplyCode;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "REJECTED");

                        Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, t.Order.StrategyTimeIntTickerString,
                        t.Order.ShortInfo, $"{methodname} {OrderErrorMsg.UnknownReplyCode}: {t.Order.TrReplyCode}",
                        t.Order.ShortDescription, t.Order.ToString());
                        break;
                }

                //Evlm2(t.Order.Number == 0 ? EvlResult.FATAL : EvlResult.SUCCESS,
                //    EvlSubject.TECHNOLOGY, t.StrategyStr,
                //    "TransactionReply",
                //    $"Order {t.Order.Status} {t.Order.ErrorMsg} #{t.OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()}",
                //    replyMessage,
                //    $"{t.RegisteredTimeStr} {t.SendedTimeStr} {t.CompletedTimeStr}");

                var orderClone = t.Order.Clone();
                Evlm2(orderClone.Number == 0 ? EvlResult.FATAL : EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                        t.StrategyStr, orderClone.ShortInfo, $"{methodname}", replyMessage,
                        t.ShortDescription);

                // if (t.Order.Status == OrderStatusEnum.PendingToActivate)
                if (t.Order.Status == OrderStatusEnum.Activated)
                {
                    FireChangedEvent(t.Order, "UI.ORDERS", "ORDER", "ADD");
                    //OnChangedEvent(new EventArgs
                    //{
                    //    Category = "UI.ORDERS",
                    //    Entity = "ORDER",
                    //    Operation = "ADD",
                    //    Object = t.Order
                    //});
                    //FireChangedEventToStrategy(t.Order, "ORDERS", "ORDER.TRANSREPLY", "ACTIVATED");
                    //OnChangedEvent(new EventArgs
                    //{
                    //    Category = "ORDERS",
                    //    Entity = "ORDER.ACTIVATED",
                    //    Operation = "ADD",
                    //    Object = t.Order
                    //});
                    OrdersActivated.Add(t.Order);
                }
                else
                {   // Rejected
                    FireChangedEvent(t.Order, "UI.ORDERS", "ORDER.COMPLETED", "ADD");
                    //OnChangedEvent(new EventArgs
                    //{
                    //    Category = "UI.ORDERS",
                    //    Entity = "ORDER.COMPLETED",
                    //    Operation = "ADD",
                    //    Object = t.Order
                    //});
                    //FireChangedEventToStrategy(t.Order, "ORDERS", "ORDER.TRANSREPLY", "REJECTED");
                    //OnChangedEvent(new EventArgs
                    //{
                    //    Category = "ORDERS",
                    //    Entity = "ORDER.REJECTED",
                    //    Operation = "ADD",
                    //    Object = t.Order
                    //});

                    OrdersCompleted.Add(t.Order);
                }
                if (t.Order.Number != 0)
                    _tx.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, t.Order);
                //_tx.Save(t.Order);

                // Kill Order
            }
            else if (t.Order != null && t.IsCancelOrderTransaction)
            //(t.Action == QuikTransactionActionEnum.KillLimit ||
            //t.Action == QuikTransactionActionEnum.KillStop ||
            //t.Action == QuikTransactionActionEnum.KillStopLimit))
            {
                // CancelOrderTransactions.Remove(t);
                Transactions.Remove(t);

                t.OrderNumber = t.Order.Number;
                // t.Order.Status = OrderStatusEnum.PendingToCancel;
                t.Order.TrMessage = t.Message;
                t.Order.TrReplyCode = nReplyCode;

                switch (nReplyCode)
                {
                    case 3:
                        // t.Order.Status = OrderStatusEnum.PendingToCancel;
                        t.Order.Status = OrderStatusEnum.Canceled;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "CANCELED");
                        //For Future don't forget
                        //OrdersActivated.Remove(t.Order);
                        //OrdersCompleted.Add(t.Order);
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        // t.Order.ErrorMsg = OrderErrorMsg.AnyReason;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "CANNOTCANCELED");
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "CANNOTCANCELED");
                        break;
                    default:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.UnknownReplyCode;

                        Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, t.Order.StrategyTimeIntTickerString,
                        t.Order.ShortInfo, $"{methodname} {OrderErrorMsg.UnknownReplyCode}: {t.Order.TrReplyCode}",
                        t.Order.ShortDescription, t.Order.ToString());

                        FireChangedEventToStrategy(t.Order.Clone(), "ORDERS", "ORDER.TRANSREPLY", "CANNOTCANCELED");
                        break;
                }
                //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                //    $"Order: {t.Order.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()} {t.Order.Status} {t.Order.ErrorMsg}",
                //    replyMessage, t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

                var orderClone = t.Order.Clone();
                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr,
                    orderClone.ShortInfo, $"{methodname}", replyMessage, t.ShortDescription);

                // OnOrderEvent(t.Order, "Canceled"); // Fire Event Order.Status.Confirmed to Add Orders in Common Orders in TradeContext
            }
            else
            {
                // Remove from both
                Transactions.Remove(t);
                // CancelOrderTransactions.Remove(t);

                t.Result = TransactionResult.Failure;
                t.Message = "Transactions IS Not SET or CANCEL ORDER";

                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    "Transactions IS Not SET or CANCEL ORDER",
                    $"Result={((T2QResults)nResult).ToString()}, " +
                    $"ExtErrCode={nExtendedErrorCode}, " +
                    $"ReplyCode={nReplyCode}, " +
                    $"TransID={dwTransId}, " +
                    $"Order={dOrderNum}, " +
                    $"Mess={replyMessage}", t.ToString());
            }
            if (t.SendedDT == t.CompletedDT)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, t.StrategyStr,
                    "TransactionReply", "FATAL SendedDT=ReplyDT",
                    $"Result={((T2QResults)nResult).ToString()}, " +
                    $"ExtErrCode={nExtendedErrorCode}, " +
                    $"ReplyCode={nReplyCode}, " +
                    $"TransID={dwTransId}, " +
                    $"Order={dOrderNum}, " +
                    $"Mess={replyMessage}", t.ToString());

                t.Result = TransactionResult.Failure;
            }
            FireChangedEvent(t, "Transactions", "Transaction", "Add");
            //OnChangedEvent(new Events.EventArgs
            //{
            //    Category = "Transactions",
            //    Entity = "Transaction",
            //    Operation = "Add",
            //    Object = t
            //});

            if (Transactions.Count > 0)
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, ParentName, Name, "TransactionReply",
                   $"OrderPendingTransactions: {Transactions.Count}", "");
        }
    }
}
