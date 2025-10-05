using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using GS.Elements;
using GS.Events;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.Trades.CheckMinMaxNumber;

using GS.Trade.Trades.Orders3;

using GS.Trade.Trades.Trades3;
using sg_TradeTerminal02;
using EventArgs = GS.Events.EventArgs;
using GS.Serialization;

namespace GS.Trade.TradeTerminals.Quik
{
    public sealed partial class QuikTradeTerminal : Element1<string>, IQuikTradeTerminal, ITrans2QuikReceiver
    {
        //public string Code { get; set; }
        //public string Name { get; set; }

        public override string Key => Code.HasValue() ?  Code : "QuikTerminal";

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
        
        private readonly string _path2Quik;
        public string Path2Quik { get { return _path2Quik; } }

        public bool IsConnectedNow { get; private set; }

        //private  IEventLog _eventLog;
        private readonly TransID _transID = new TransID(999);

        //private readonly TransactionReplyCallback _transactionReplyCallback;

        public delegate void TransactionReplyEventHandler(int result, UInt32 trid, long ordernumber );
        public event TransactionReplyEventHandler TransactionReplyEvent;

        //private const int MaxT2QCount = 3;
        //private static readonly List<QuikTradeTerminal> QuikTerminals = new List<QuikTradeTerminal>();
        private readonly ITrans2Quik _t2Q;
        public string DllNamePath2QuikPair => _path2Quik + " " + _t2Q.GetTrans2QuikDllName;

        public QuikTradeTerminal( string path2Quik, ITrans2Quik trans2Quik )
        {
            if (path2Quik == null) throw new ArgumentNullException(nameof(path2Quik));
            if (trans2Quik == null) throw new ArgumentNullException(nameof(trans2Quik));

            _path2Quik = path2Quik;
            _t2Q = trans2Quik;
            Type = TradeTerminalType.Quik;

            OrderQueue = new Queue<string>();
            OrderTransactionQueue = new Queue<IQuikTransaction>();
            _orderTransactionBlQueue = new BlockingCollection<IQuikTransaction>();

            TransactionQueue = new QuikTransactionQueue();

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

        public void Init(IOrders orders, ITrades trades, IEventLog evl, ITradeContext txContext, TradeTerminals tts)
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
                throw;
                return;
            }

            Transactions = Builder.Build<QuikTransactions>(@"Init\QuikTransactions.xml", "QuikTransactions");
            //{
            //    Category = "Transactions",
            //    Entity = "Transaction"
            //};
            Transactions.Parent = this;
            Transactions.WhoAreYou();
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

            var success =  ( result == (Int32)T2QResults.TRANS2QUIK_SUCCESS || 
                                result == (Int32)T2QResults.TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK);
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
            var result = _t2Q.IsConnected2Quik();
            var success = (result & 255) == (Int32)T2QResults.TRANS2QUIK_DLL_CONNECTED;
           // Evlm(success ? EvlResult.SUCCESS : EvlResult.WARNING, EvlSubject.TECHNOLOGY,
           //                   ShortName, "IS DLL Connected?", ((T2QResults)result).ToString(), ToString());
            return success;
        }

        public bool IsConnect2Server()
        {
            var result = _t2Q.IsConnected2Server();
            var success = (result & 255) == (Int32) T2QResults.TRANS2QUIK_QUIK_CONNECTED;
            // Evlm(success ? EvlResult.SUCCESS : EvlResult.WARNING, EvlSubject.TECHNOLOGY, ShortName,
            //                    "IS Connect to TradeServer ?", ((T2QResults)result).ToString(), ToString());
            return success;
        }

        public int IsConnected()
        {
            //return IsConnect2Quik() && IsConnect2Server();
            //if (IsConnectedNow)
            //    return +1;
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
        public bool IsWellConnected()
        {
            return IsConnect2Quik() && IsConnect2Server();
        }
        public bool SetConnectionStatusCallback(ConnectionStatusCallback conectionStatusCallback )
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
            Evlm(EvlResult.WARNING, EvlSubject.TECHNOLOGY, ShortName, "QuikTerminal" ,"Connection Status Changed",
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

                ClearOrderTransactionQueue3();
                return;
            }

            UnSubscribeOrders();
            UnSubscribeTrades();

            SubscribeOrders("", "");
            SubscribeTrades("", "");

            _t2Q.StartOrders();
            _t2Q.StartTrades();
                
            //return;
        }
        #endregion

        #region Transaction

        public long SendSyncTransaction(string transactionStr)
        {
            double ordernumber = 0d;
            var result = _t2Q.SendSyncTransaction(transactionStr, ref ordernumber);
            if ( result == (Int32)T2QResults.TRANS2QUIK_SUCCESS)
                return (long) ordernumber;
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

            if (IsWellConnected())
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
        public void TransactionReply(Int32 nResult, Int32 nExtendedErrorCode, Int32 nReplyCode,
                    UInt32 dwTransId, Double dOrderNum,
                    [MarshalAs(UnmanagedType.LPStr)] string replyMessage)
        {

            TransactionReplyEvent?.Invoke(nResult, dwTransId, (long)dOrderNum);

            //_orders.TransactionReply(nResult, dwTransId, (long)dOrderNum);

            var t = Transactions.GetByKey((long) dwTransId);
            if (t == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Quik.TransactionReply",
                    $"{(uint) dwTransId} Transaction NOT FOUND",
                    replyMessage,
                    $"Result={(T2QResults) nResult}, " +
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
            t.OrderNumber = (long) dOrderNum;

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
                t.OrderNumber = (long)dOrderNum;
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


        private void OnOrderEvent(IOrder3 o, string oper)
        {
            TradeEntityChangedEvent?.Invoke(this, new Events.EventArgs
            {
                Category = "Order",
                Entity = "Status",
                Operation = oper,
                Object = o
            });
        }

        #endregion

        #region Trades
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
                                            ShortName, "Orders", "UnSubscribe Orders", ResultToString(result), ToString());
            return success;
        }

        public void NewOrderStatus(double dNumber, int iDate, int iTime, int iMode, 
            int iActivationTime, int iCancelTime, int iExpireDate, 
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            int iIsSell, int iQty, int nBalance, double dPrice,
            int iStatus, uint dwTransId, string sClientCode)
        {
            //throw new NotImplementedException(); 
            //_orders.NewOrderStatus( dNumber, iDate, iTime, iMode,
            //                        iActivationTime, iCancelTime, iExpireDate,
            //                        sAcc, sBrokerRef, sClassCode, sSecCode,
            //                        iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);

            OrderStatusProcess(dNumber, iDate, iTime, iMode,
                                    iActivationTime, iCancelTime, iExpireDate,
                                    sAcc, sBrokerRef, sClassCode, sSecCode,
                                    iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);
        }

        public void NewTrade(double dNumber, int iDate, int iTime, int iMode,
            string sAcc, string sStrat, string sClassCode, string sSecCode,
            int nIsSell, int iQty, double dPrice, 
            string sClientCode, double dOrderNumber, double dCommissionTs)
        {
            //throw new NotImplementedException();
            /*
            _trades.NewTrade(   dNumber, iDate, iTime, iMode,
                                sAcc, sStrat, sClassCode, sSecCode,
                                nIsSell, iQty, dPrice,
                                sClientCode, dOrderNumber, dCommissionTs);
            */
            TradeProcess(dNumber, iDate, iTime, iMode,
                        sAcc, sStrat, sClassCode, sSecCode,
                        nIsSell, iQty, dPrice,
                        sClientCode, dOrderNumber, dCommissionTs);
        }

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
        #endregion

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
        public void TradeProcess(double dNumber, int iDate, int iTime, int nMode,
                                string account, string strategy, string classCode, string ticker,
                                int iIsSell, int quantity, double price, string comment, double orderNumber, double commissionTs)
        {
            var number = Convert.ToInt64(dNumber);
            
            //var maxNumb = _checkMaxTradeNumber.GetByKey(account);
            
            //long n;
            //string a;

            //if (maxNumb != null)
            //{
            //    a = maxNumb.Account;
            //    n = maxNumb.Number;
            //}
            //else
            //{
            //    a = "a";
            //    n = 0;
            //}

            //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
            //                   "QuikTerminal", "Trades2", "New Trade: CheckMaxNumber",
            //                   String.Format("Trade: # {0} Account: {1} Last # {2} {3}",
            //                   number, account,
            //                   maxNumb == null 
            //                   ? 0 
            //                   : maxNumb.Number,
            //                   maxNumb == null 
            //                   ? "0"
            //                   : maxNumb.Account), "");

            if (! _checkMaxTradeNumber.SetIfGreaterThanMe(account, number))
            {
                //_eventLog.AddItem(EvlResult.FATAL, EvlSubject.TRADING,
                //               "QuikTerminal", "Trades2", "New Trade: CheckMaxNumber",
                //               String.Format("Trade: # {0} Account: {1} is Less or Equal Than Last # {2} {3}",
                //               number, account, _checkMaxTradeNumber.GetByKey(account).Number, account), "");
                return;
            }
            else
            {
                //long n1;
                //string a1;

                //if (maxNumb != null)
                //{
                //    a1 = maxNumb.Account;
                //    n1 = maxNumb.Number;
                //}
                //else
                //{
                //    a1 = "a1";
                //    n1 = 0;
                //}

                //_eventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TRADING,
                //                "QuikTerminal", "Trades2", "New Trade: CheckMaxNumber",
                //                String.Format("Trade: # {0} Acc: {1} == {2} {3} is > {3} {4}",
                //                number, account,n1,a,  
                //                n, a), "");
                
            }
            var sDt = iDate.ToString("D8");
            var sTm = iTime.ToString("D6");
            var dt = new DateTime(
                int.Parse(sDt.Substring(0, 4)), int.Parse(sDt.Substring(4, 2)), int.Parse(sDt.Substring(6, 2)),
                int.Parse(sTm.Substring(0, 2)), int.Parse(sTm.Substring(2, 2)), int.Parse(sTm.Substring(4, 2)));

            var operation = iIsSell == 0 ? Trade.TradeOperationEnum.Buy : Trade.TradeOperationEnum.Sell;
            var ordNumber = Convert.ToInt64(orderNumber);

            var t = new Trade3
                {
                    DT = dt,

                    Number = number,
                    Registered = dt,

                    Status = TradeStatusEnum.Registered,

                    Mode = nMode,
                    AccountEx = account,
                    StrategyEx = strategy,
                    ClassCodeEx = classCode,
                    TickerEx = ticker,
                    Operation = operation,
                    Quantity = quantity,
                    Price = (decimal) price,
                    OrderNumber = ordNumber,
                    CommissionTs = (decimal)commissionTs
                };
            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING,
                                "QuikTerminal", "Trades2: "+ t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                                "New Trade: "+ t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0()
                                + "; Order: " + t.OrderNumber.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
                                "", t.ToString());

            if (classCode != "FUTEVN")
            {
                //if (ClassCodeToRemoveLogin.HasValue())
                //    if (classCode.Contains(ClassCodeToRemoveLogin))
                //        strategy = strategy.Replace(LoginToRemove, "");

                //if (TradesToProcess.Contains(t))
                //{
                //    Evlm2(EvlResult.WARNING, EvlSubject.TRADING, t.StratTicker,
                //        "Trade", "New", t.ShortInfo + "Trade already Registered", t.ToString());
                //}
                //else
                //{
                TradesToProcess.Add(t);

                //OnTradeEntityChangedEvent(new EventArgs
                OnChangedEvent(new EventArgs
                {
                    Category = "UI.Trades",
                    Entity = "Trade",
                    Operation = "Add",
                    Object = t
                });

                //}
                //_Positions.PositionCalculate5(t);
                
            }
            else
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, "Trades2",
                                 "Trades2", "Evening Session Trade", t.ToString(), "Ignore Positions Calculation");
            }
        }

        public void OrderResolveProcess()
        {
            //Evlm2(EvlResult.WARNING, EvlSubject.TRADING,
            //                    "QuikTerminal", "TradeProcess22222222","***************************", "","");

          //  var trades = TradesToProcess.Items.Where(t => ((ITrade2)t).Status == TradeStatusEnum.Registered).ToList();
            try
            {
                foreach (var o in OrdersActivated.Items)
                {
                    if (o.Strategy != null)
                        continue;

                    var s = _tx.GetStrategyByKey(o.Key);
                    if (s == null)
                    {
                        var t = (DateTime.Now - o.Created).Seconds;
                        if( t < 15)
                            continue;
                       
                        s = _tx.RegisterDefaultStrategy("Default", "Default",
                            o.AccountCode, o.TickerBoard , o.TickerCode, 60, Type.ToString(), Path2Quik);
                        if (s == null)
                        {
                            OrdersActivated.Remove(o);
                            continue;
                        }
                    }
                    o.Strategy = s;
                    o.ErrorMsg = OrderErrorMsg.Ok;
                    o.Comment += " Resolved from Storage.";
                    
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
                        var t = (DateTime.Now - o.Created).Seconds;
                        if (t < 15)
                            continue;

                        s = _tx.RegisterDefaultStrategy("Default", "Default",
                            o.AccountCode, o.TickerBoard, o.TickerCode, 60, Type.ToString(), Path2Quik);
                        if (s == null)
                        {
                            OrdersCompleted.Remove(o);
                            continue;
                        }
                    }
                    o.Strategy = s;
                    o.ErrorMsg = OrderErrorMsg.Ok;
                    o.Comment += " Resolved from Storage.";

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

        public void TradeResolveProcess()
        {
            try
            {
                //foreach (var t in TradesToProcess.TradeRegistered)
                //{
                //    if (t.Strategy != null)
                //        continue;
                //}
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
                            t.Strategy.StrategyTickerString, "Trade: " + t.Number.ToString(CultureInfo.InvariantCulture).WithSqBrackets0(),
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
                throw;
            }
        }

        // Currently Work version !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        public void OrderStatusProcess(
           double dNumber, int iDate, int iTime, int mode,
           int iActivationTime, int iCancelTime, int iExpireDate,
           string account, string strategy, string classCode, string ticker, int iIsSell, int quantity, int rest, double price,
           int iStatus, uint dwTransId, string comment)
        {

            var number = Convert.ToInt64(dNumber);
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
                // ACTIVATED OR CONFIRMED

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

                if(ord.Strategy!=null)
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
                    if(oper == OrderOperationEnum.Unknown)
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
    }
}
