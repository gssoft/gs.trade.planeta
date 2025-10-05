using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.ProcessTasks;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.Trades.CheckMinMaxNumber;

using GS.Trade.Trades.Orders3;

using GS.Trade.Trades.Trades3;
using sg_TradeTerminal02;
using EventArgs = GS.Events.EventArgs;
using GS.Serialization;
using GS.Status;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal : Element1<string>,  ITradeTerminal, 
                                IQuikTradeTerminal, ITrans2QuikReceiver
    {
        public override string Key => Code.HasValue() ?  Code : GetType().Name;
        public TradeTerminalType Type { get; private set; }

        private IOrders _orders;
        private ITrades _trades;
        private ITradeContext _tx;

        //public event EventHandler<Events.EventArgs> TransactionEvent;
        //public event EventHandler<Events.EventArgs> OrderEvent;
        //public event EventHandler<Events.EventArgs> TradeEvent;

        public event EventHandler<Events.IEventArgs> TradeEntityChangedEvent;

        //private void OnTradeEntityChangedEvent(IEventArgs e)
        //{
        //    EventHandler<IEventArgs> handler = TradeEntityChangedEvent;
        //    if (handler != null) handler(this, e);
        //}

        public Trades3 TradesToProcess { get; set; }
        public Trades3 TradesProcessed { get; set; }

        public Orders3 OrdersActivated { get; set; }
        public Orders3 OrdersCompleted { get; set; }
        public Orders3 OrdersUnknown { get; set; }

        private readonly OrderLockedCount _orderLockedCount;
        private readonly OrderLockedCount2 _orderLockedCount2;

        private readonly CheckMinMaxNumber _checkMaxTradeNumber;

        public QuikTransactions Transactions;
        public QuikTransactions CancelOrderTransactions;
        
        private readonly string _path2Quik;
        public string Path2Quik => _path2Quik;

        public bool IsConnectedNow { get; private set; }

        //private  IEventLog _eventLog;
        private readonly TransID _transID = new TransID(999);

        //private readonly TransactionReplyCallback _transactionReplyCallback;

        public delegate void TransactionReplyEventHandler(int result, ulong trid, ulong ordernumber );
        public event TransactionReplyEventHandler TransactionReplyEvent;

        //private const int MaxT2QCount = 3;
        //private static readonly List<QuikTradeTerminal> QuikTerminals = new List<QuikTradeTerminal>();
        private readonly ITrans2Quik _t2Q;
        public string DllNamePath2QuikPair => _path2Quik + " " + _t2Q.GetTrans2QuikDllName;

        public QuikTradeTerminal( string path2Quik, ITrans2Quik trans2Quik )
        {
            _path2Quik = path2Quik ?? throw new ArgumentNullException(nameof(path2Quik));
            _t2Q = trans2Quik ?? throw new ArgumentNullException(nameof(trans2Quik));
            
            Type = TradeTerminalType.Quik;

            OrderQueue = new Queue<string>();
            OrderTransactionQueue = new Queue<IQuikTransaction>();
            _orderTransactionBlQueue = new BlockingCollection<IQuikTransaction>();

            TransactionQueue = new QuikTransactionQueue();
            CancelOrderTransactionQueue = new QuikTransactionQueue();

            _orderLockedCount = new OrderLockedCount { MaxCount = MaxTransactionNumber };
            _orderLockedCount2 = new OrderLockedCount2 { MaxCount = MaxTransactionNumber };
            
            OrdersActivated = new Orders3();
            OrdersCompleted = new Orders3();
            OrdersUnknown = new Orders3();
 
            TradesToProcess = new Trades3();
            TradesProcessed = new Trades3();

            _checkMaxTradeNumber = new CheckMinMaxNumber();

            //GetAttr();
        }

        private void GetAttr()
        {
            var t = _t2Q as Trans2Quik01;
            AttributeCollection ac = TypeDescriptor.GetAttributes(t,false);
            foreach (var att in ac)
            {
                //DataEntityAttribute  -- ur attribute class name
                DllImportAttribute da = att as DllImportAttribute;
                var df = da.Value;
                //Console.WriteLine(da.field1);  //initially it shows MESSAGE_STAGING
                //da.field1 = "Test_Message_Staging";
            }
        }

        public void Init(IOrders orders, ITrades trades, IEventLog evl, ITradeContext txContext, ITradeTerminals tts)
        {
            _orders = orders;
            _trades = trades;
            //_eventLog = evl;

            _tx = txContext;

            _t2Q.SetT2QReceiver(this);
            Parent = tts;

            try
            {
                _t2Q.Init();
            }
            catch (Exception ex)
            {
                SendExceptionMessage3(Code, "TradeTerminal", "Init()", ToString(), ex);
                // throw;
            }

            Transactions = Builder.Build<QuikTransactions>(@"Init\QuikTransactions.xml", "QuikTransactions");
            Transactions.Parent = this;
            Transactions.WhoAreYou();

            CancelOrderTransactions = Builder.Build<QuikTransactions>(@"Init\QuikTransactions.xml", "QuikTransactions");
            CancelOrderTransactions.Parent = this;
            CancelOrderTransactions.WhoAreYou();

            // 2018.05.13 ProcessTask
            IsProcessTaskInUse = Transactions.IsProcessTaskInUse;

            // SetupProcessTask();

            WhoAreYou();
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, FullName, Key, "Init()", "Init " + Code, ToString());
        }

        public void SetEventLog(IEventLog eventlog)
        {
            if (eventlog == null) throw new NullReferenceException("Eventlog Reference is null");
            //_eventLog = eventlog;
        }
        private string GetTransID()
        {
            return _transID.GetTransID();
        }

        #region Connect
        public bool Connect()
        {
            if (IsConnect2Quik()) return true;
            // Evlm(EvlResult.SUCCESS, EvlSubject.DIAGNOSTIC, ShortName , "Try Connect to DLL",  DllNamePath2QuikPair, ToString());

            var result = _t2Q.Connect2Quik(_path2Quik);

            var success =  result == T2QResults.TRANS2QUIK_SUCCESS || 
                           result == T2QResults.TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK;
            return success;
        }
        public bool DisConnect()
        {
            var result = _t2Q.Disconnect();
            var success = (result == (Int32)T2QResults.TRANS2QUIK_SUCCESS);

            Evlm(success ? EvlResult.SUCCESS : EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                ShortName, "QuikTerminal" ,"DisConnect", ((T2QResults)result).ToString(), ToString());

            IsConnectedNow =  result != (Int32)T2QResults.TRANS2QUIK_SUCCESS;
            return IsConnectedNow;
        }
        public bool IsConnect2Quik()
        {
            if (_t2Q == null) return false;
            var result = _t2Q.IsConnected2Quik();
            var rslt = (result & 255) == (Int32)T2QResults.TRANS2QUIK_DLL_CONNECTED;
           // Evlm(success ? EvlResult.SUCCESS : EvlResult.WARNING, EvlSubject.TECHNOLOGY,
           //                   ShortName, "IS DLL Connected?", ((T2QResults)result).ToString(), ToString());
            return rslt;
        }

        public bool IsConnect2Server()
        {
            if (_t2Q == null) return false;
            var result = _t2Q.IsConnected2Server();
            var rslt = (result & 255) == (Int32) T2QResults.TRANS2QUIK_QUIK_CONNECTED;
            // Evlm(success ? EvlResult.SUCCESS : EvlResult.WARNING, EvlSubject.TECHNOLOGY, ShortName,
            //                    "IS Connect to TradeServer ?", ((T2QResults)result).ToString(), ToString());
            return rslt;
        }

        public int IsConnected()
        {
            if (_t2Q == null){ IsConnectedNow = false; return 0;}
            var result = _t2Q.IsConnected2Server(); // Check Server Connection
            switch (result & 255)
            {
                case TRANS2QUIK_QUIK_CONNECTED:
                    IsConnectedNow = true;
                    return 1;
                case TRANS2QUIK_QUIK_NOT_CONNECTED:
                    IsConnectedNow = false;
                    return -1;
                case TRANS2QUIK_DLL_NOT_CONNECTED:
                    IsConnectedNow = false;
                    return -2;
                default:
                    IsConnectedNow = false;
                    return 0;
            }
        }
        //public bool IsWellConnected()
        //{
        //    return IsConnect2Quik() && IsConnect2Server();
        //}
        public bool IsWellConnected => IsConnect2Quik() && IsConnect2Server();
        
        public bool SetConnectionStatusCallback(ConnectionStatusCallback64 conectionStatusCallback )
        {
            if (conectionStatusCallback == null)
                throw new ArgumentNullException(nameof(conectionStatusCallback));

            var result = _t2Q.SetConnectionStatusCallback(conectionStatusCallback);
            var success = (result & 255) == (Int32) T2QResults.TRANS2QUIK_SUCCESS;
            Evlm( success ? EvlResult.SUCCESS : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                ShortName, "QuikTerminal", "SetConnectionStatusCallback", ((T2QResults)result).ToString(), ToString());
            return success;
        }
        public void ConnectionStatusChanged(Int32 nConnectionEvent, Int32 nExtendedErrorCode, byte[] infoMessage)
        {
            nConnectionEvent = nConnectionEvent & 255; 
            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ShortName, "QuikTerminal" ,"Connection Status Changed",
                $"{(T2QResults) nConnectionEvent} {nExtendedErrorCode}", ByteToString(infoMessage));

            // Check Results Code from func() IS_QUIK_CONNECTED - connection with BROKER SERVER

            //if ((nConnectionEvent != (Int32) T2QResults.TRANS2QUIK_DLL_CONNECTED) &&
            //    (nConnectionEvent != (Int32) T2QResults.TRANS2QUIK_QUIK_CONNECTED))
            //{
            //    IsConnectedNow = false;
            //    return;
            //}

            //IsConnectedNow = nConnectionEvent == (Int32)T2QResults.TRANS2QUIK_QUIK_CONNECTED;

            IsConnectedNow = IsConnect2Quik() && IsConnect2Server();

            if (! IsConnectedNow)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ShortName, "QuikTerminal", "Is Not Connected",
                $"{(T2QResults)nConnectionEvent} {nExtendedErrorCode}", ByteToString(infoMessage));
                // ClearOrderTransactionQueue();

                ClearTransactionFromQueue();
                // ClearOrderTransactionQueue3();
                FireTradeEntityChangedEvent("TradeTerminal", "ConnectionStatusChanged", "NotConnected","");

                return;
            }
            FireTradeEntityChangedEvent("TradeTerminal", "ConnectionStatusChanged", "Connected", "");

            //UnSubscribeOrders();
            //UnSubscribeTrades();

            //SubscribeOrders("", "");
            //SubscribeTrades("", "");

            //_t2Q.StartOrders();
            //_t2Q.StartTrades();

            ReStart();
        }
        public void ConnectionStatusChanged(long nConnectionEvent, long nExtendedErrorCode, byte[] infoMessage)
        {
            nConnectionEvent = nConnectionEvent & 255;
            Evlm2(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ShortName, "QuikTerminal", "Connection Status Changed",
                $"{(T2QResults)nConnectionEvent} {nExtendedErrorCode}", ByteToString(infoMessage));

           //IsConnectedNow = nConnectionEvent == (Int32)T2QResults.TRANS2QUIK_QUIK_CONNECTED;

            IsConnectedNow = IsConnect2Quik() && IsConnect2Server();

            if (!IsConnectedNow)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, ShortName, "QuikTerminal", "Is Not Connected",
                    $"{(T2QResults)nConnectionEvent} {nExtendedErrorCode}", ByteToString(infoMessage));
                // ClearOrderTransactionQueue();

                ClearTransactionFromQueue();
                // ClearOrderTransactionQueue3();
                FireTradeEntityChangedEvent("TradeTerminal", "ConnectionStatusChanged", "NotConnected", "");

                return;
            }

            FireTradeEntityChangedEvent("TradeTerminal", "ConnectionStatusChanged", "Connected", "");
            ReStart();
        }
        public void ReStart()
        {
            if (IsProcessTaskInUse)
            {
                var eargs = new EventArgs1
                {
                    Process = "QuikConnectionProcess",
                    Category = "QuikTransactions",
                    Entity = "TransactionClear",
                    Operation = "AddOrUpdate",
                    ProcessingAction = ReStart
                };
                if(ProcessTask != null)
                    ProcessTask.EnQueue(eargs);
                else
                    ReStart(null);
            }
            else
                ReStart(null);
        }
        private void ReStart(IEventArgs1 args)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            var connect = IsWellConnected;

            FireTradeEntityChangedEvent("TradeTerminal", "ConnectionStatusChanged", "ReStart", "Good Bye");

            Evlm2(EvlResult.SUCCESS, EvlSubject.INIT, ShortName, "QuikTerminal", method, $"IsConnected: {connect}", "");

            UnSubscribeOrders();
            UnSubscribeTrades();

            SubscribeOrders("", "");
            SubscribeTrades("", "");

            _t2Q.StartOrders();
            _t2Q.StartTrades();
        }
        #endregion

#region Transaction

        public long SendSyncTransaction(string transactionStr)
        {
            var orderNumber = 0d;
            var result = _t2Q.SendSyncTransaction(transactionStr, ref orderNumber);
            if ( result == (Int32)T2QResults.TRANS2QUIK_SUCCESS)
                return (long) orderNumber;
            return -1;
        }
        public bool SetTransactionReplyCallback(TransactionReplyCallback transactionReplyCallback)
        {
            if (transactionReplyCallback == null)
                throw new ArgumentNullException(nameof(transactionReplyCallback));

            var result = _t2Q.SetTransactionReplyCallback(transactionReplyCallback);
            return (result == (Int32)T2QResults.TRANS2QUIK_SUCCESS);
        }
        public bool SendAsyncTransaction(string transactionString)
        {
            if (transactionString == null) return false;

            var result = _t2Q.SendAsyncTransaction(transactionString);
            return result == (long) T2QResults.TRANS2QUIK_SUCCESS;
        }
        public bool SendAsyncTransaction2(string transactionString)
        {
            if (transactionString == null) return false;
            var t = new QuikTransaction
            {
                TransID = 1,
                SendedDT = DateTime.Now,
                CompletedDT = DateTime.Now,
                Status = TransactionStatus.TryToSend,
                Result = TransactionResult.Warning,
                QuikResult = T2QResults.TRANS2QUIK_SUCCESS,
                ExtendedErrorCode = 253,
                ReplyCode = 25,
                TransactionString = transactionString
            };
            
            var result = _t2Q.SendAsyncTransaction(transactionString);
            return result == (long)T2QResults.TRANS2QUIK_SUCCESS;
        }
        public bool SendAsyncTransaction(IQuikTransaction t)
        {
            if (t == null)
                return false;

            if (t.TransactionString == null)
                return false;

            int extendedErrorCode;

            Transactions.Add(t);

            t.Status = TransactionStatus.TryToSend;

            var result = _t2Q.SendAsyncTransaction(t.TransactionString, out extendedErrorCode);

            IsConnectedNow =    (result != (Int32)T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED) &&
                                (result != (Int32)T2QResults.TRANS2QUIK_DLL_NOT_CONNECTED);

            t.QuikResult = (T2QResults)result;
            if (result == (int) T2QResults.TRANS2QUIK_SUCCESS )
            {
                t.Result = TransactionResult.Success;
                t.ExtendedErrorCode = extendedErrorCode;

                t.Status = TransactionStatus.Sended;
                t.SendedDT = DateTime.Now; 

                if (t.Action > 0 && t.Order != null)
                {
                    t.Order.Status = OrderStatusEnum.Sended;
                    t.Order.Sended = DateTime.Now;
                }
            }
            // NOT SENDED 
            else
            {
                t.Status = TransactionStatus.NotSended;
                // t.SendedDT = DateTime.Now; 

                t.Result = TransactionResult.Failure;
                t.ExtendedErrorCode = extendedErrorCode;

                if (t.Action > 0 && t.Order != null)
                {
                    t.Order.Status = OrderStatusEnum.Rejected;
                    t.Order.ErrorMsg = OrderErrorMsg.NotSended;
                }

                Transactions.Remove(t);
                OnChangedEvent(new Events.EventArgs
                {
                    Category = "Transactions",
                    Entity = "Transaction",
                    Operation = "Add",
                    Object = t
                });
            }
            return result == (long)T2QResults.TRANS2QUIK_SUCCESS;
        }
        // 02.05.2018
        public bool SendAsyncTransaction32(IQuikTransaction t)
        {
            if (t == null)
                return false;

            if (t.TransactionString == null)
                return false;

            int extendedErrorCode;

            if (IsWellConnected)
            {
                Transactions.Add(t);

                t.Status = TransactionStatus.TryToSend;

                var result = _t2Q.SendAsyncTransaction(t.TransactionString, out extendedErrorCode);

                IsConnectedNow = (result != (Int32) T2QResults.TRANS2QUIK_QUIK_NOT_CONNECTED) &&
                                 (result != (Int32) T2QResults.TRANS2QUIK_DLL_NOT_CONNECTED);

                t.QuikResult = (T2QResults) result;
                if (result == (int) T2QResults.TRANS2QUIK_SUCCESS)
                {
                    t.Result = TransactionResult.Success;
                    t.ExtendedErrorCode = extendedErrorCode;

                    t.Status = TransactionStatus.Sended;
                    t.SendedDT = DateTime.Now;

                    if (t.Action > 0 && t.Order != null)
                    {
                        t.Order.Status = OrderStatusEnum.Sended;
                        t.Order.Sended = DateTime.Now;
                    }
                }
                // NOT SENDED 
                else
                {
                    t.Status = TransactionStatus.NotSended;
                    // t.SendedDT = DateTime.Now; 

                    t.Result = TransactionResult.Failure;
                    t.ExtendedErrorCode = extendedErrorCode;

                    if (t.Action > 0 && t.Order != null)
                    {
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NotSended;
                    }

                    Transactions.Remove(t);
                    OnChangedEvent(new Events.EventArgs
                    {
                        Category = "Transactions",
                        Entity = "Transaction",
                        Operation = "Add",
                        Object = t
                    });
                }
                return result == (long) T2QResults.TRANS2QUIK_SUCCESS;
            }

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

            OnChangedEvent(new Events.EventArgs
            {
                Category = "Transactions",
                Entity = "Transaction",
                Operation = "Add",
                Object = t
            });
            return false;
            //Evlm2(EvlResult.FATAL, EvlSubject.TRADING, DllNamePath2QuikPair, "TransactionQueue",
            //    "Clear OrderQueue: Item." + i, "Can't SendOrderFromQueue. TradeTerminal Is Not Connected", t.ToString());
        }

        // ******************* TRANSACTION ********************
        public void TransactionReply01(Int32 nResult, Int32 nExtendedErrorCode, Int32 nReplyCode,
                    UInt32 dwTransId, Double dOrderNum,
                    [MarshalAs(UnmanagedType.LPStr)] string replyMessage)
        {

            var ordNum = Convert.ToUInt64(dOrderNum);
            TransactionReplyEvent?.Invoke(nResult, dwTransId, ordNum);

            //_orders.TransactionReply(nResult, dwTransId, (long)dOrderNum);

            var t = Transactions.GetByKey(dwTransId);
            if (t == null)
            {
                //t = CancelOrderTransactions.GetByKey((long)dwTransId);
                //if (t == null)
                //{
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Quik.TransactionReply",
                        $"{dwTransId} Orders Transaction  NOT FOUND",
                        replyMessage,
                        $"Result={(T2QResults) nResult}, " +
                        $"ExtErrCode={nExtendedErrorCode}, " +
                        $"ReplyCode={nReplyCode}, " +
                        $"TransID={dwTransId}, " +
                        $"Order={dOrderNum}, " +
                        $"Mess={replyMessage}", DllNamePath2QuikPair);
                    // throw new NullReferenceException("TransactionReply: Transaction is Not Found");
                //    return;
                //}
                return;
            }

            //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply", replyMessage,
            //    String.Format("Result={0},ExtErrCode={1},ReplyCode={2},TransID={3},Order={4},Mess={5}",
            //        ((T2QResults)nResult).ToString(), nExtendedErrorCode, nReplyCode, dwTransId, dOrderNum, replyMessage), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

            t.Comment += "TransReply 1 ";
            t.OrderNumber = Convert.ToUInt64(dOrderNum);

            t.Status = TransactionStatus.Completed;
            t.CompletedDT = DateTime.Now; 

            t.Result = (T2QResults) nResult == T2QResults.TRANS2QUIK_SUCCESS
                ? TransactionResult.Success
                : TransactionResult.Failure;

            t.Message = replyMessage;
            t.QuikResult = (T2QResults)nResult;
            t.ExtendedErrorCode = nExtendedErrorCode;
            t.ReplyCode = nReplyCode;

            if (t.Order != null &&
                (t.Action == QuikTransactionActionEnum.SetLimit ||
                t.Action == QuikTransactionActionEnum.SetStop ||
                t.Action == QuikTransactionActionEnum.SetStopLimit)
               )
            {
                t.OrderNumber = Convert.ToUInt64(dOrderNum) ;
                t.Order.Number = t.OrderNumber;
                t.Order.Status = OrderStatusEnum.Confirmed;
                t.Order.TrMessage = t.Message;

                if (t.OrderNumber != 0)
                    t.Order.ErrorMsg = OrderErrorMsg.Ok;
                else
                {
                    t.Order.Status = OrderStatusEnum.Rejected;
                    t.Order.ErrorMsg = OrderErrorMsg.Unknown;
                }

                switch (nReplyCode)
                {
                    case 3:
                        t.Order.Status = OrderStatusEnum.Confirmed;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        break;
                    default:
                        if (t.OrderNumber == 0)
                        {
                            t.Order.Status = OrderStatusEnum.Rejected;
                            t.Order.ErrorMsg = OrderErrorMsg.Unknown;
                        }
                        break;
                }
                
                Evlm2( t.Order.Number==0 ? EvlResult.FATAL : EvlResult.SUCCESS,
                    EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    $"Order {t.Order.Status}.{t.Order.ErrorMsg} #{t.OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()}",
                    replyMessage,
                    $"{t.RegisteredTimeStr} {t.SendedTimeStr} {t.CompletedTimeStr}");

                if (t.Order.Status == OrderStatusEnum.Confirmed)
                {
                    //OnTradeEntityChangedEvent(new EventArgs
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "ADD",
                        Object = t.Order
                    });

                    OrdersActivated.Add(t.Order);
                }
                else
                {   // Rejected
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "ADD",
                        Object = t.Order
                    });

                    OrdersCompleted.Add(t.Order);
                }
                if(t.Order.Number != 0)
                    _tx.TradeStorage.SaveChanges(StorageOperationEnum.AddOrUpdate, t.Order);
                //_tx.Save(t.Order);
            
            // Kill Order
            }
            else if (t.Order != null &&
                (t.Action == QuikTransactionActionEnum.KillLimit ||
                t.Action == QuikTransactionActionEnum.KillStop ||
                t.Action == QuikTransactionActionEnum.KillStopLimit))
            {
                t.OrderNumber = t.Order.Number;
                t.Order.Status = OrderStatusEnum.Canceled;
                t.Order.TrMessage = t.Message;

                switch (nReplyCode)
                {
                    case 3:
                        t.Order.Status = OrderStatusEnum.Canceled;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        break;
                }

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    $"Order Canceled #{t.Order.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()}",
                    replyMessage, t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

                OnOrderEvent(t.Order,"Canceled"); // Fire Event Order.Status.Confirmed to Add Orders in Common Orders in TradeContext
            }
            else
            {
               Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                   "FATAL Transaction NOT PROCESSED.",
                   $"Result={((T2QResults) nResult).ToString()}, " +
                   $"ExtErrCode={nExtendedErrorCode}, " +
                   $"ReplyCode={nReplyCode}, " +
                   $"TransID={dwTransId}, " +
                   $"Order={dOrderNum}, " +
                   $"Mess={replyMessage}", t.ToString());
               
                t.Result = TransactionResult.Failure; 
            }
            if (t.SendedDT == t.CompletedDT)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply", "FATAL SendedDT=ReplyDT",
                    $"Result={((T2QResults) nResult).ToString()}, " +
                    $"ExtErrCode={nExtendedErrorCode}, " +
                    $"ReplyCode={nReplyCode}, " +
                    $"TransID={dwTransId}, " +
                    $"Order={dOrderNum}, " +
                    $"Mess={replyMessage}", t.ToString());

                t.Result = TransactionResult.Failure;
            }

            Transactions.Remove(t);

            OnChangedEvent(new Events.EventArgs
            {
                Category = "Transactions",
                Entity = "Transaction",
                Operation = "Add",
                Object = t
            });

            Evlm2(EvlResult.INFO, EvlSubject.TECHNOLOGY, Name, "Transaction", "Count()=" + Transactions.Count,"","");
        }

        public void TransactionReply2(Int32 nResult, Int32 nExtendedErrorCode, Int32 nReplyCode,
                    UInt32 dwTransId, Double dOrderNum,
                    [MarshalAs(UnmanagedType.LPStr)] string replyMessage)
        {

            var orderNumber = Convert.ToUInt64(dOrderNum);
            
            TransactionReplyEvent?.Invoke(nResult, dwTransId, orderNumber);

            //_orders.TransactionReply(nResult, dwTransId, (long)dOrderNum);

            var transId = Convert.ToUInt64(dwTransId);

            var t = Transactions.GetByKey(transId);
            if (t == null)
            {
                t = CancelOrderTransactions.GetByKey(transId);
                if (t == null)
                {
                    Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "TransactionReply",
                    $"{(uint)dwTransId} Transaction NOT FOUND",
                    replyMessage,
                    $"Result={(T2QResults)nResult}, " +
                    $"ExtErrCode={nExtendedErrorCode}, " +
                    $"ReplyCode={nReplyCode}, " +
                    $"TransID={transId}, " +
                    $"Order={orderNumber}, " +
                    $"Mess={replyMessage}", DllNamePath2QuikPair);
                    // throw new NullReferenceException("TransactionReply: Transaction is Not Found");
                    return;
                }
                // return;
            }

            //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply", replyMessage,
            //    String.Format("Result={0},ExtErrCode={1},ReplyCode={2},TransID={3},Order={4},Mess={5}",
            //        ((T2QResults)nResult).ToString(), nExtendedErrorCode, nReplyCode, dwTransId, dOrderNum, replyMessage), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

            t.Comment += "TransReply 1 ";
            t.OrderNumber = orderNumber;

            t.Status = TransactionStatus.Completed;
            t.CompletedDT = DateTime.Now;

            t.Result = (T2QResults)nResult == T2QResults.TRANS2QUIK_SUCCESS
                ? TransactionResult.Success
                : TransactionResult.Failure;

            t.Message = replyMessage;
            t.QuikResult = (T2QResults)nResult;
            t.ExtendedErrorCode = nExtendedErrorCode;
            t.ReplyCode = nReplyCode;

            if (t.Order != null && t.IsSetOrderTransaction
                //&&
                //(t.Action == QuikTransactionActionEnum.SetLimit ||
                //t.Action == QuikTransactionActionEnum.SetStop ||
                //t.Action == QuikTransactionActionEnum.SetStopLimit)
               )
            {
                Transactions.Remove(t);

                t.OrderNumber = orderNumber;
                t.Order.Number = orderNumber;
                t.Order.Status = OrderStatusEnum.PendingToActivate;
                t.Order.TrMessage = t.Message;

                if (orderNumber != 0)
                    t.Order.ErrorMsg = OrderErrorMsg.Ok;
                else
                {
                    t.Order.Status = OrderStatusEnum.Rejected;
                    t.Order.ErrorMsg = OrderErrorMsg.OrderNumberIsEmpty;
                }
                switch (nReplyCode)
                {
                    case 3:
                        t.Order.Status = OrderStatusEnum.PendingToActivate;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        break;
                    default:
                        //if (t.OrderNumber == 0)
                        //{
                        //    t.Order.Status = OrderStatusEnum.Rejected;
                        //    t.Order.ErrorMsg = OrderErrorMsg.OrderNumberIsEmpty;
                        //}

                        // ????????????????????????????????????????????????????????????????
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.UnknownReplyCode;
                        break;
                }

                Evlm2(t.Order.Number == 0 ? EvlResult.FATAL : EvlResult.SUCCESS,
                    EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    $"Order {t.Order.Status}.{t.Order.ErrorMsg} #{t.OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()}",
                    replyMessage,
                    $"{t.RegisteredTimeStr} {t.SendedTimeStr} {t.CompletedTimeStr}");

                if (t.Order.Status == OrderStatusEnum.PendingToActivate)
                {
                    //OnTradeEntityChangedEvent(new EventArgs
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "ADD",
                        Object = t.Order
                    });

                    OrdersActivated.Add(t.Order);
                }
                else
                {   // Rejected
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "ADD",
                        Object = t.Order
                    });

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
                CancelOrderTransactions.Remove(t);

                t.OrderNumber = t.Order.Number;
                t.Order.Status = OrderStatusEnum.PendingToCancel;
                t.Order.TrMessage = t.Message;

                switch (nReplyCode)
                {
                    case 3:
                        t.Order.Status = OrderStatusEnum.PendingToCancel;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        break;
                    default:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.UnknownReplyCode;
                        break;
                }

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    $"Order Canceled #{t.Order.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()}",
                    replyMessage, t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

                OnOrderEvent(t.Order, "Canceled"); // Fire Event Order.Status.Confirmed to Add Orders in Common Orders in TradeContext
            }
            else
            {
                // Remove from both
                Transactions.Remove(t);
                CancelOrderTransactions.Remove(t);

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
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    "FATAL SendedDT=ReplyDT",
                    $"Result={((T2QResults)nResult).ToString()}, " +
                    $"ExtErrCode={nExtendedErrorCode}, " +
                    $"ReplyCode={nReplyCode}, " +
                    $"TransID={dwTransId}, " +
                    $"Order={dOrderNum}, " +
                    $"Mess={replyMessage}", t.ToString());

                t.Result = TransactionResult.Failure;
            }
            OnChangedEvent(new Events.EventArgs
            {
                Category = "Transactions",
                Entity = "Transaction",
                Operation = "Add",
                Object = t
            });

            Evlm2(EvlResult.FATAL, EvlSubject.TRADING, "TransactionReply",
                   $"OrderPendingTransactions: {Transactions.Count}",
                   $"CancelPendingTransactios: {CancelOrderTransactions.Count}", "", "");
        }

        // TransactionReply3 2018.05.14 was TransactionReply Last
        public void TransactionReply3(Int32 nResult, Int32 nExtendedErrorCode, Int32 nReplyCode,
        //public void TransactionReply(Int32 nResult, Int32 nExtendedErrorCode, Int32 nReplyCode,
                    UInt32 dwTransId, Double dOrderNum,
                    [MarshalAs(UnmanagedType.LPStr)] string replyMessage)
        {
            var orderNum = Convert.ToUInt64(dOrderNum);
            var transId = Convert.ToUInt64(dwTransId);

            if (IsProcessTaskInUse)
            {
                var tri = new TransactionReplyItem
                {
                    TransId = transId,
                    OrderNum = orderNum,
                    Result = (T2QResults)nResult,
                    ExtendedErrorCode = nExtendedErrorCode,
                    ReplyCode = nReplyCode,
                    ReplyMessage = replyMessage
                };
                SendTransactionReplyEventArgs(tri, TransactionReplyResolve);
                return;
            }

            TransactionReplyEvent?.Invoke(nResult, transId, orderNum);

            var t = Transactions.GetByKey(transId);
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

            //Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply", replyMessage,
            //    String.Format("Result={0},ExtErrCode={1},ReplyCode={2},TransID={3},Order={4},Mess={5}",
            //        ((T2QResults)nResult).ToString(), nExtendedErrorCode, nReplyCode, dwTransId, dOrderNum, replyMessage), t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

            t.Comment += "TransReply 1 ";
            t.OrderNumber = orderNum;

            t.Status = TransactionStatus.Completed;
            t.CompletedDT = DateTime.Now;

            t.Result = (T2QResults)nResult == T2QResults.TRANS2QUIK_SUCCESS
                ? TransactionResult.Success
                : TransactionResult.Failure;

            t.Message = replyMessage;
            t.QuikResult = (T2QResults)nResult;
            t.ExtendedErrorCode = nExtendedErrorCode;
            t.ReplyCode = nReplyCode;

            if (t.Order != null && t.IsSetOrderTransaction
               //&&
               //(t.Action == QuikTransactionActionEnum.SetLimit ||
               //t.Action == QuikTransactionActionEnum.SetStop ||
               //t.Action == QuikTransactionActionEnum.SetStopLimit)
               )
            {
                Transactions.Remove(t);

                t.OrderNumber = orderNum;
                t.Order.Number = t.OrderNumber;
                t.Order.Status = OrderStatusEnum.PendingToActivate;
                t.Order.TrMessage = t.Message;

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
                        t.Order.Status = OrderStatusEnum.PendingToActivate;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        break;
                    default:
                        //if (t.OrderNumber == 0)
                        //{
                        //    t.Order.Status = OrderStatusEnum.Rejected;
                        //    t.Order.ErrorMsg = OrderErrorMsg.OrderNumberIsEmpty;
                        //}

                        // ????????????????????????????????????????????????????????????????
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.UnknownReplyCode;
                        break;
                }

                Evlm2(t.Order.Number == 0 ? EvlResult.FATAL : EvlResult.SUCCESS,
                    EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    $"Order {t.Order.Status}.{t.Order.ErrorMsg} #{t.OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()}",
                    replyMessage,
                    $"{t.RegisteredTimeStr} {t.SendedTimeStr} {t.CompletedTimeStr}");

                if (t.Order.Status == OrderStatusEnum.PendingToActivate)
                {
                    //OnTradeEntityChangedEvent(new EventArgs
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "ADD",
                        Object = t.Order
                    });

                    OrdersActivated.Add(t.Order);
                }
                else
                {   // Rejected
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER.COMPLETED",
                        Operation = "ADD",
                        Object = t.Order
                    });

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
                t.Order.Status = OrderStatusEnum.PendingToCancel;
                t.Order.TrMessage = t.Message;

                switch (nReplyCode)
                {
                    case 3:
                        t.Order.Status = OrderStatusEnum.PendingToCancel;
                        t.Order.ErrorMsg = OrderErrorMsg.Ok;
                        break;
                    case 4:
                    case 6:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.NoMargin;
                        break;
                    case 5:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.CannotCancel;
                        break;
                    default:
                        t.Order.Status = OrderStatusEnum.Rejected;
                        t.Order.ErrorMsg = OrderErrorMsg.UnknownReplyCode;
                        break;
                }

                Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply",
                    $"Order Canceled #{t.Order.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets()}",
                    replyMessage, t.RegisteredTimeStr + " " + t.SendedTimeStr + " " + t.CompletedTimeStr);

                OnOrderEvent(t.Order, "Canceled"); // Fire Event Order.Status.Confirmed to Add Orders in Common Orders in TradeContext
            }
            // Not SetOrder and NOT KillOrder
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
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, t.StrategyStr, "TransactionReply", "FATAL SendedDT=ReplyDT",
                    $"Result={((T2QResults)nResult).ToString()}, " +
                    $"ExtErrCode={nExtendedErrorCode}, " +
                    $"ReplyCode={nReplyCode}, " +
                    $"TransID={dwTransId}, " +
                    $"Order={dOrderNum}, " +
                    $"Mess={replyMessage}", t.ToString());

                t.Result = TransactionResult.Failure;
            }
            OnChangedEvent(new Events.EventArgs
            {
                Category = "Transactions",
                Entity = "Transaction",
                Operation = "Add",
                Object = t
            });

            // if(Transactions.Count > 0)
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, ParentName, Name, "TransactionReply",
                   $"OrderPendingTransactions: {Transactions.Count}", "");
        }


        // ********* ProcessTask02 ********************
       
        // TransactionReply new 2018.05.14 was TransactionReply3 Last; work with TransactionReplyResolve  
       
        // Should rename to TransactionReply
        private void FireChangedEventToStrategy(IOrder3 order, string category, string entity, string operation)
        {
            order?.Strategy?.EnQueue(this, new Events.EventArgs
            {
                Category = order.Strategy.StrategyTimeIntTickerString.TrimUpper(),
                Entity = entity.TrimUpper(),
                Operation = operation.TrimUpper(),
                Object = order,
                Sender = this
            });
        }
        private void FireChangedEventToStrategy(
            IStrategy strategy, string category, string entity, string operation, object obj )
        {
            strategy?.EnQueue(this, new Events.EventArgs
            {
                Category = strategy.StrategyTimeIntTickerString.TrimUpper(),
                Entity = entity.TrimUpper(),
                Operation = operation.TrimUpper(),
                Object = obj,
                Sender = this
            });
        }
        public void FireChangedEvent(object obj, string category, string entity, string operation)
        {
            OnChangedEvent(new Events.EventArgs
                {
                    Category = category.TrimUpper(),
                    Entity = entity.TrimUpper(),
                    Operation = operation.TrimUpper(),
                    Object = obj
                }
            );
        }
        private void OnOrderEvent(IOrder3 o, string operation)
        {
            TradeEntityChangedEvent?.Invoke(this, new Events.EventArgs
            {
                Category = "Order",
                Entity = "Status",
                Operation = operation,
                Object = o
            });
        }
        #endregion

        #region SubScribe
        public bool SubscribeTrades(string classCode, string securityCode)
        {
            var result = _t2Q.SubscribeTrades(classCode, securityCode);
            var success = result == (Int32)T2QResults.TRANS2QUIK_SUCCESS;
            Evlm(success ? EvlResult.SUCCESS : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                            ShortName, "Trades", "Subscribe Trades", ResultToString(result), ToString());
            return success;
        }
        public bool UnSubscribeTrades()
        {
            var result = _t2Q.UnSubscribeTrades();
            var success = result == (Int32)T2QResults.TRANS2QUIK_SUCCESS;
            Evlm(success ? EvlResult.SUCCESS : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                            ShortName, "Trades","UnSubscribe Trades", ResultToString(result), ToString());
            return success;
        }
#endregion

#region Orders
        public bool SubscribeOrders(string classCode, string securityCode)
        {
            var result = _t2Q.SubscribeOrders(classCode, securityCode);
            var success = result == (Int32) T2QResults.TRANS2QUIK_SUCCESS;
            Evlm(success ? EvlResult.SUCCESS : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                            ShortName, "Orders", "Subscribe Orders", ResultToString(result), ToString());
            return success;
        }
        public bool UnSubscribeOrders()
        {
            var result = _t2Q.UnSubscribeOrders();
            var success = result == (Int32)T2QResults.TRANS2QUIK_SUCCESS;
            Evlm(success ? EvlResult.SUCCESS : EvlResult.FATAL, EvlSubject.TECHNOLOGY,
                                            ShortName, "Orders", "UnSubscribe Orders",
                                            ResultToString(result), ToString());
            return success;
        }
        // Old Until 2018.05.16
        public void NewOrderStatusOld(double dNumber, int iDate, int iTime, int iMode, 
            int iActivationTime, int iCancelTime, int iExpireDate, 
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            int iIsSell, int iQty, int nBalance, double dPrice,
            int iStatus, uint dwTransId, string sClientCode)
        {

            OrderStatusProcess(dNumber, iDate, iTime, iMode,
                                    iActivationTime, iCancelTime, iExpireDate,
                                    sAcc, sBrokerRef, sClassCode, sSecCode,
                                    iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);
        }
        //private void SendOrderStatusReplyEventArgs(OrderStatusReply orderstatusreply)
        //{
        //    if (!IsProcessTaskInUse)
        //        return;

        //    var eargs = new EventArgs1
        //    {
        //        Process = "OrderStatusProcess",
        //        Category = "Orders",
        //        Entity = "OrderStatus",
        //        Operation = "AddOrUpdate",
        //        Object = orderstatusreply,
        //        ProcessingAction = OrderStatusReplyProcessing
        //    };
        //    ProcessTask?.EnQueue(eargs);
        //}
        //new from 2018.05.15

        // Update 210808 rest from int to Int64
        // Before 2018.05
        // Currently Work version !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public void OrderStatusProcess(
           double dNumber, int iDate, int iTime, int mode,
           int iActivationTime, int iCancelTime, int iExpireDate,
           string account, string strategy, string classCode, string ticker,
           int iIsSell, long quantity, Int64 rest, double price,
           int iStatus, uint dwTransId, string comment)
        {
            var number = Convert.ToUInt64(dNumber);
            if (number == 0)
                return;

            if (classCode == "FUTEVN") return;

            //if (ClassCodeToRemoveLogin.HasValue())
            //    if (classCode.Contains(ClassCodeToRemoveLogin))
            //        strategy = strategy.Replace(LoginToRemove, "");

            //var number = Convert.ToInt64(dNumber);
            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");
            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            var sActTime = iActivationTime.ToString("D6");
            var activeTime = new TimeSpan(
                int.Parse(sActTime.Substring(0, 2)),
                int.Parse(sActTime.Substring(2, 2)),
                int.Parse(sActTime.Substring(4, 2)));

            var sCancTime = iCancelTime.ToString("D6");
            var cancelTime = new TimeSpan(
                int.Parse(sCancTime.Substring(0, 2)),
                int.Parse(sCancTime.Substring(2, 2)),
                int.Parse(sCancTime.Substring(4, 2)));

            var sExpireDate = iExpireDate.ToString("D8");
            var i1 = int.Parse(sExpireDate.Substring(0, 4));
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
                // ACTIVATED OR CONFIRMED OR PendingToActivate

                var stOld = ord.Status;
                var trIdOld = ord.TransId;

                //ord.Quantity = quantity;
                ord.Rest = rest;
                ord.Status = status;
                //ord.SetStatus(status, string.Empty);
                ord.ExCancelTime = cancelTime;
                ord.ExActivateTime = activeTime;

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
                    OrdersActivated.Remove(ord.Key);
                    OrdersCompleted.Add(ord);
                }
                if (status == OrderStatusEnum.Activated) // && stOld == OrderStatusEnum.Confirmed)
                {
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.ORDERS",
                        Entity = "ORDER",
                        Operation = "UPDATE",
                        Object = ord
                    });
                    OrdersActivated.Update(ord.Key, ord);
                }

                Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                            "Order=" + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                            ord.Status + " " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                            ord.Strategy != null ? ord.Strategy.StrategyTickerString : "", ord.ToString());

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
                    // Not Found in Completed
                    // New Order
                    var oper = Order.OperationToEnum2(operation);
                    if (oper == OrderOperationEnum.Unknown)
                        throw new NullReferenceException("Order Operation Unknown");

                    var o = new Order3
                    {
                        AccountKey = account,
                        TickerBoardKey = classCode,
                        TickerKey = ticker,
                        TransId = (uint)transId,
                        Number = number,
                        DateTime = dt,
                        Created = DateTime.Now,
                        Operation = oper,
                        OrderType = OrderTypeEnum.Limit,
                        Status = status,
                        StopPrice = 0,
                        LimitPrice = price,
                        Quantity = quantity
                    };

                    if (status == OrderStatusEnum.Activated)
                    {
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

                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                            "Order=" + o.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                            o.Status + " " + o.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                            o.Strategy != null ? o.Strategy.StrategyTickerString : "Strategy", o.ToString());
                }
                else // Already Filled
                {
                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, Code,
                            "Order=" + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                            ord.Status + " " + ord.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()
                                + "; Is Aready Filled", ord.Strategy != null ? ord.Strategy.StrategyTickerString : "Strategy",
                                ord.ToString());

                    if (ord.Strategy != null)
                        _tx.TradeStorage?.SaveChanges(StorageOperationEnum.AddOrUpdate, ord);
                    // OrdersCompleted.Remove(ord);
                }
                //AddOrder(o);
            }
        }
        //public class OrderStatusReply
        //{
        //    public double OrderNumber { get; set; }
        //    public int Date { get; set; }
        //    public int Time { get; set; }
        //    public int Mode { get; set; }
        //    public int ActivationTime { get; set; }
        //    public int CancelTime { get; set; }
        //    public int ExpireDate { get; set; }
        //    public string Account { get; set; }
        //    public string BrokerRef { get; set; }
        //    public string ClassCode { get; set; }
        //    public string Ticker { get; set; }
        //    public int IsSell { get; set; }
        //    public int Qty { get; set; }
        //    public int Rest { get; set; }
        //    public double Price { get; set; }
        //    public int Status { get; set; }
        //    public uint TransId { get; set; }
        //    public string ClientCode { get; set; }

        //    public override string ToString()
        //    {
        //        return $"[{OrderNumber}] TrId:{TransId} Dt:{Date} Tm:{Time} Mode:{Mode}" +
        //               $" ActTm:{ActivationTime} CncTm:{CancelTime} ExpDt:{ExpireDate}" +
        //               $" Acc:{Account} BrRf:{BrokerRef} ClsCd:{ClassCode} Tkr:{Ticker}" +
        //               $" Side:{IsSell} Qty:{Qty} Rst:{Rest} Price:{Price} St:{Status}" +
        //               $" ClntCd:{ClientCode}";
        //    }
        //    public string ShortInfo()
        //    {
        //        return $"[{OrderNumber}] [{TransId}] DtTm:{Date} {Time}" +
        //               $" ActTm:{ActivationTime} CncTm:{CancelTime}" +
        //               $" Acc:{Account} Tkr:{Ticker}" +
        //               $" Side:{IsSell} Qty:{Qty} Price:{Price} St:{Status}";
        //    }
        //}
        // 19.08.28, 04.05.2018
        
        private void SetBusyStatusWnenStratEmpty(IOrder3 ord, BusyStatusEnum status)
        {
            if (ord.Strategy == null && ord.BusyStatus == BusyStatusEnum.Unknown)
                ord.BusyStatus = status;
        }
        #endregion
#region Trade
        // Move to TradeReply.cs
        // work until 2019.09.20
        // Trans2Quik Handler
        public void NewTrade1(double dNumber, int iDate, int iTime, int iMode,
            string sAcc, string sStrat, string sClassCode, string sSecCode,
            int nIsSell, int iQty, double dPrice, 
            string sClientCode, double dOrderNumber, double dCommissionTs)
        {
            if (IsProcessTaskInUse)
            {
                var tradeReply = new NewTradeReply
                {
                    TradeNumber = dNumber,
                    Date = iDate,
                    Time = iTime,
                    Mode = iMode,
                    Account = sAcc,
                    Strategy = sStrat,
                    ClassCode = sClassCode,
                    Ticker = sSecCode,
                    Side = ConvertSideToEnum(nIsSell),
                    Qty = iQty,
                    Price = dPrice,
                    ClientCode = sClientCode,
                    OrderNumber = dOrderNumber,
                    ComissionTs = dCommissionTs,
                    ReplyDateTime = DateTime.Now
                };
                var eargs = new EventArgs1
                {
                    Process = "NewTradeProcess",
                    Category = "Trades",
                    Entity = "NewTrade",
                    Operation = "AddOrUpdate",
                    Object = tradeReply,
                    ProcessingAction = NewTradeReplyProcessing1
                };
                ProcessTask?.EnQueue(eargs);
                return;
            }
            // Old version of Trade Process
            // IF PRocessTask does not use
            //TradeProcess(dNumber, iDate, iTime, iMode,
            //            sAcc, sStrat, sClassCode, sSecCode,
            //            nIsSell, iQty, dPrice,
            //            sClientCode, dOrderNumber, dCommissionTs);
        }
        // work until 2019.09.20 but the Same with Current/ Remain only for the copy save
        // work since 2018.05.16
        public void NewTradeReplyProcessing1(IEventArgs1 args)
                               //double dNumber, int iDate, int iTime, int nMode,
                               //string account, string strategy, string classCode, string ticker,
                               //int iIsSell, int quantity, double price, string comment, double orderNumber,
                               //double commissionTs)
        {
            var tradeReply = args.Object as NewTradeReply;
            if (tradeReply == null)
            {
                //Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, ParentName, Name,
                //        MethodBase.GetCurrentMethod().Name,
                //        $"Args IS NOT {typeof(NewTradeReply).Name}", "Sorry");
                return;
            }
            var dNumber = tradeReply.TradeNumber;
            var iDate = tradeReply.Date;
            var iTime = tradeReply.Time;
            var nMode = tradeReply.Mode;
            var account = tradeReply.Account;
            var strategy = tradeReply.Strategy;
            var classCode = tradeReply.ClassCode;
            var ticker = tradeReply.Ticker;
            var side = tradeReply.Side;
            var quantity = tradeReply.Qty;
            var price = tradeReply.Price;
            var comment = tradeReply.ClientCode;
            var orderNumber = tradeReply.OrderNumber;
            var commissionTs = tradeReply.ComissionTs;
            var replyDateTime = tradeReply.ReplyDateTime;

            var number = Convert.ToUInt64(dNumber);

            if (!_checkMaxTradeNumber.SetIfGreaterThanMe(account, number))
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING,
                //               "QuikTerminal", "Trades2", "New Trade: CheckMaxNumber",
                //               String.Format("Trade: # {0} Account: {1} is Less or Equal Than Last # {2} {3}",
                //               number, account, _checkMaxTradeNumber.GetByKey(account).Number, account), "");
                return;
            }
           
            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");
            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            // var operation = iIsSell == 0 ? Trade.TradeOperationEnum.Buy : Trade.TradeOperationEnum.Sell;

            var ordNumber = Convert.ToUInt64(orderNumber);

            var t = new Trade3
            {
                DT = dt,
                Number = number,

                Registered = dt,
                Confirmed = replyDateTime,
                ToResolveDT = dt,
                Resolved = dt,
                
                Status = TradeStatusEnum.Confirmed,

                Mode = nMode,
                AccountEx = account,
                StrategyEx = strategy,
                ClassCodeEx = classCode,
                TickerEx = ticker,
                Operation = side,
                Quantity = quantity,
                Price = (decimal)price,
                OrderNumber = ordNumber,
                CommissionTs = (decimal)commissionTs
            };

            //t.Status = TradeStatusEnum.Confirmed;

            //Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                    "QuikTerminal", "Trades2: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
            //                    "New Trade: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0() 
            //                     + "; Order: " + t.OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(), "", t.ToString());

            if (classCode == "FUTEVN")
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, "Trades2",
                    "Trades2", "Evening Session Trade", t.ToString(),
                    "Ignore Positions Calculation");
                return;
            }

            //t.Status = TradeStatusEnum.ToResolve;
            //t.ToResolveDT = DateTime.Now;

            var isResolved = TryToResolveTrade(t);
            if (isResolved)
            {
                if (t.Strategy.IsTradeNumberValid(t.Number))
                {
                    t.Comment += " New.";
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.Trades",
                        Entity = "Trade",
                        Operation = "Add",
                        Object = t
                    });
                    // ******** Strategy.NewTrade.Process may be long Time
                    t.Strategy.NewTrade(t);
                }
                else
                {
                    t.Comment += " Old.";
                    OnChangedEvent(new EventArgs
                    {
                        Category = "UI.Trades",
                        Entity = "Trade",
                        Operation = "Add",
                        Object = t
                    });
                }
            }
            else
            {
                // Try to Resove Later
                TradesToProcess.Add(t);
                OnChangedEvent(new EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Add",
                    Object = t
                });
            }
            // Try to Resolve rest of UnResolved Trades
            // ????????
            TradeResolveProcess2();
        }
        // work until 2018.05.16
      

        // Try to Resolve Strategy from the Trade Order
    
        public void TradeResolveProcess()
        {
            Evlm1(EvlResult.INFO, EvlSubject.TECHNOLOGY, ParentTypeName, TypeName,
                MethodBase.GetCurrentMethod()?.Name,
                "We are working inside IdlingCycle", "Thank you" );
            try
            {
                foreach (var t in TradesToProcess.TradeRegistered)
                {
                    if (t.Strategy != null)
                    {
                        t.Status = TradeStatusEnum.Confirmed;
                        continue;
                    }
                    var ord = OrdersActivated.GetByKey(t.OrderKey);
                    if (ord == null)
                    {
                        ord = OrdersCompleted.GetByKey(t.OrderKey);
                        if (ord == null || ord.Strategy == null)
                            continue;

                        t.Order = ord;
                        t.Strategy = ord.Strategy;
                        t.Comment += "Resolved from Core Orders.";
                    }
                    else
                    {
                        if (ord.Strategy == null)
                            continue;

                        t.Order = ord;
                        t.Strategy = ord.Strategy;
                        t.Comment += "Resolved from Core Orders.";
                    }

                    t.Status = TradeStatusEnum.Confirmed;
                    TradesToProcess.Remove(t);

                    Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                        "QuikTerminal.TradeProcess3", "Trades2: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                        "New Trade Confirmed by Order: " + t.OrderKey.WithSqBrackets0(),
                        "OrderActivateCount=" + OrdersActivated.Count, t.ToString());


                    if (t.Number <= t.Strategy.Position.LastTradeNumber)
                    {
                        Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY,
                            t.Strategy.StrategyTickerString,
                            "Trade: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                            "TradeProcess3.NewTrade()",
                            "Old Trade Detected: " + t.Number, t.ToString());

                        t.Comment += " Old.";
                        OnChangedEvent(new EventArgs
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

                    if (t.Strategy.NewTrade(t) <= 0)
                        continue;
                }
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName, "TradeResolveProcess()", "TradeResolveProcess()", "", e);
                // throw;
            }
        }

        #endregion

        public void NewConnectionStatus(int nConnectionEvent, int nExtendedErrorCode, string message)
        {
            if (nConnectionEvent == (int) T2QResults.TRANS2QUIK_SUCCESS)
            {
                IsConnectedNow = true;
                Evlm(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, ShortName,
                    DllNamePath2QuikPair, "Connected", message, nExtendedErrorCode.ToString());
            }
            else
            {
                IsConnectedNow = false;
                Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ShortName,
                    DllNamePath2QuikPair, "Is NOT Connected", message, nExtendedErrorCode.ToString());
            }
        }

        //private void Evlm(EvlResult result, EvlSubject subject,
        //                            string source, string entity, string operation, string description, string obj )
        //{
        //    if( _eventLog != null)
        //        _eventLog.AddItem(result, subject, source, entity, operation, description, obj);
        //}

        public override string ToString() => 
            $"[Type: {GetType().FullName};" +
            $" PathToQuik: {Path2Quik};" +
            $" Trans2QuikDllName: {_t2Q.GetTrans2QuikDllName}]";

        public string ShortName => $"{Code}: {DllNamePath2QuikPair}";

        //private void TradeEventFire(EventArgs e)
        //{
        //    EventHandler<EventArgs> handler = TradeEvent;
        //    if (handler != null) handler(this, e);
        //}    

        private void OnTradeEntityChangedEvent(IEventArgs e)
        {
            TradeEntityChangedEvent?.Invoke(this, e);
        }
        public void FireTradeEntityChangedEvent(string category, string entity, string operation, object obj)
        {
            OnTradeEntityChangedEvent(new Events.EventArgs
            {
                Category = category.TrimUpper(),
                Entity = entity.TrimUpper(),
                Operation = operation.TrimUpper(),
                Object = obj,
                Sender = this
            });
        }
    }
}
