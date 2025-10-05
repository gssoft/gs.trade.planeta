using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GS.Trade.TradeTerminals.Quik
{
        #region Trans2Quik Delegates
        
        public delegate void ConnectionStatusCallback(
                        Int32 nConnectionEvent,
                        Int32 nExtendedErrorCode,
                        byte[] lpstrInfoMessage);

        public delegate void TransactionReplyCallback(
                    Int32 nTransactionResult,
                    Int32 nTransactionExtendedErrorCode,
                    Int32 nTransactionReplyCode,
                    uint dwTransId,
                    Double dOrderNum,
                    [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage);

        public delegate void OrderStatusCallback(
                        Int32 nMode,
                        UInt32 dwTransId,
                        Double dNumber,
                        [MarshalAs(UnmanagedType.LPStr)]string sClassCode,
                        [MarshalAs(UnmanagedType.LPStr)]string sSecCode,
                        Double dPrice,
                        Int32 nBalance,
                        Double dValue,
                        Int32 nIsSell,
                        Int32 nStatus,
                        Int32 nOrderDescriptor);
        /*
        public delegate void OrderStatusCallback(
            double dNumber,
            Int32 iDate, Int32 iTime,
            Int32 iMode,
            Int32 iActivationTime, Int32 iCancelTime, Int32 iExpireDate,
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            Int32 iIsSell, Int32 iQty, Int32 nBalance, double dPrice,
            Int32 iStatus, UInt32 dwTransId,
            string sClientCode
        );
        */ 

        public delegate void TradeStatusCallback(
                        Int32 nMode,
                        Double dNumber,
                        Double dOrderNumber,
                        [MarshalAs(UnmanagedType.LPStr)]string ClassCode,
                        [MarshalAs(UnmanagedType.LPStr)]string SecCode,
                        Double dPrice,
                        Int32 nQty,
                        Double dValue,
                        Int32 nIsSell,
                        Int32 nOrderDescriptor);
    /*
    public delegate void TradeStatusCallback(
        double dNumber,
        Int32 iDate, Int32 iTime, Int32 iMode,
        string sAcc, string sStrat, string sClassCode, string sSecCode,
        Int32 nIsSell, Int32 iQty, double dPrice,
        string sClientCode,
        double dOrderNumber, double dCommissionTs);
    */
        #endregion

        #region Trans2QuikResults Enums
        public enum T2QResults : int 
        {
            TRANS2QUIK_SUCCESS = 0,
            TRANS2QUIK_FAILED = 1,
            TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2,
            TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3,
            TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4,
            TRANS2QUIK_WRONG_SYNTAX = 5,
            TRANS2QUIK_QUIK_NOT_CONNECTED = 6,
            TRANS2QUIK_DLL_NOT_CONNECTED = 7,
            TRANS2QUIK_QUIK_CONNECTED = 8,
            TRANS2QUIK_QUIK_DISCONNECTED = 9,
            TRANS2QUIK_DLL_CONNECTED = 10,
            TRANS2QUIK_DLL_DISCONNECTED = 11,
            TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12,
            TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13,
            TRANS2QUIK_WRONG_INPUT_PARAMS = 14
        };
        #endregion

        internal sealed class Trans2Quik01 : ITrans2Quik
        {
            // private  const string Trans2QuikDllName = @"D:\MTS\Trans2QuikDll\Trans2Quik_1.dll";
            // private const string Trans2QuikDllName = "Trans2Quik_1.dll";
            private const string Trans2QuikDllName = "trans2quik.dll";

        //public IQuikTradeTerminal QuikTerminal { get; set; }
        private static ITrans2QuikReceiver _t2QReceiver;

            #region The Same as Trans2Quik01

            #region Event Handlers
          
            private ConnectionStatusCallback _connectionStatusCallback;
            private TransactionReplyCallback _transactionReplyCallback;
            private TradeStatusCallback _tradeStatusCallback;
            private OrderStatusCallback _orderStatusCallback;

            #endregion

            public string GetTrans2QuikDllName { get { return Trans2QuikDllName; } }

            public Trans2Quik01(){}
            public void Init()
            {
                _connectionStatusCallback = new ConnectionStatusCallback(ConnectionStatusDefaultCallback);
                _transactionReplyCallback = new TransactionReplyCallback(TransactionReplyDefaultCallback);
                _tradeStatusCallback = new TradeStatusCallback(TradeStatusDefaultCallback);
                _orderStatusCallback = new OrderStatusCallback(OrderStatusDefaultCallback);

                SetConnectionStatusCallback(_connectionStatusCallback);
                SetTransactionReplyCallback(_transactionReplyCallback);
            }

            #region My Methods

            #region IReceivers

            public void SetT2QReceiver(ITrans2QuikReceiver trans2QuikReceiver)
            {
                _t2QReceiver = trans2QuikReceiver as ITrans2QuikReceiver;
                if (_t2QReceiver == null) throw new ArgumentNullException("Trans2QuikReceiver");
            }
            #endregion

            #region Connect / Disconnect

            public Int32 Connect2Quik(string pathToQuik)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = connect(pathToQuik, ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("Connect\t\t{0} {1} ", result & 255, ResultToString(result & 255));
                //Console.WriteLine("Connect\t\t{0} {1} ", ((T2QConnectionResults)(result & 255)), ResultToString(result & 255));
                //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSizez={2}", (ExtendedErrorCode & 255), ErrorMessage, ErrorMessageSize);

                //if ( result == T2QResults.TRANS2QUIK_FAILED )
                return result;
            }
            public Int32 IsConnected2Quik()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = is_dll_connected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("IsConnectedToQuik>\t{0} {1}", result, ResultToString(result & 255));
                //return result == TRANS2QUIK_DLL_CONNECTED;

                return result;
            }
            public Int32 Disconnect()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = disconnect(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("Disconnect>\t\t{0} {1}", (result), ResultToString(result));
                //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSize={2}", (ExtendedErrorCode & 255).ToString(), ByteToString(ErrorMessage), ErrorMessageSize);
                //return result == TRANS2QUIK_SUCCESS;
                return result;
            }
            public Int32 IsConnected2Server()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                Int32 result = is_quik_connected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //    Console.WriteLine("test_q.is_quik_connected_test>\t{0} {1}", (result & 255), ResultToString(result & 255));
                //return result == TRANS2QUIK_QUIK_CONNECTED;
                return result;
            }
            public Int32 SetConnectionStatusCallback(ConnectionStatusCallback cb)
            {
                // _connectionStatusCallback += cb;

                var errorMessage = new Byte[50];
                const Int32 errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = set_connection_status_callback(_connectionStatusCallback, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result & 0xff;
            }
            private static void ConnectionStatusDefaultCallback(Int32 nConnectionEvent,Int32 nExtendedErrorCode, byte[] lpstrInfoMessage)
            {
                if (_t2QReceiver != null)
                    _t2QReceiver.ConnectionStatusChanged(nConnectionEvent, nExtendedErrorCode, lpstrInfoMessage);
            }
            #endregion

            #region Transactions

            #region Sync Transaction
            public Int32 SendSyncTransaction(string transactionStr, ref double orderNum)
            {
                var errorMessage = new Byte[50];
                var resultMessage = new Byte[50];
                var extendedErrorCode = -100;
                var replyCode = 0;
                // ************** Why int TransID; May be long  ????????????????
                Int32 transId = 0;
                const uint resultMessageSize = 50;
                const uint errorMessageSize = 50;

                var result = send_sync_transaction(transactionStr, ref replyCode, ref transId, ref orderNum, resultMessage, resultMessageSize,
                    ref extendedErrorCode, errorMessage, errorMessageSize);

                //Console.WriteLine("{0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSize={2}",
                //    (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);

                //String resStr = ByteToString(resultMessage);
                //resStr = resStr.Trim();
                // Console.WriteLine(" ReplyCode={0} TransID={1}  OrderNum={2} \n ResultMessage={3}, ResultMessageSize={4}",
                //    replyCode, transId, orderNum, resStr, resultMessageSize);
                return result;
            }
            #endregion

            #region AsyncTransaction

            public Int32 SendAsyncTransaction(string transactionString)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result & 255;
            }
            public Int32 SendAsyncTransaction(string transactionString, out int extendedErrCode)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
                extendedErrCode = extendedErrorCode;
                return result & 255;
            }

            public Int32 SetTransactionReplyCallback(TransactionReplyCallback cb)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = set_transaction_reply_callback(cb, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result;
            }
            private static void TransactionReplyDefaultCallback(Int32 nTransactionResult, Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                uint dwTransId, double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage)
            {
                if ( _t2QReceiver != null) 
                    _t2QReceiver.TransactionReply(nTransactionResult, nTransactionExtendedErrorCode, nTransactionReplyCode,
                        dwTransId, dOrderNum, transactionReplyMessage); 
            }

            public static void TransactionReplyCallbackImpl(
                Int32 nTransactionResult,
                Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                UInt32 dwTransId,
                Double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage)
            {
                String strOutput = "TrRes=" + nTransactionResult +
                    // " TrResStr=" + ResultToString(nTransactionResult & 255) +
                    " TrExErrCode=" + nTransactionExtendedErrorCode +
                    " TrReplyCode=" + nTransactionReplyCode +
                    " TrID=" + dwTransId +
                    " OrderNum=" + dOrderNum +
                    " ResMsg=" + transactionReplyMessage;

                try
                {
                    /*
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"async_trans.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        file.WriteLine(strOutput);
                        Console.WriteLine("TRANS_REPLY_CALLBACK: " + strOutput);
                    }
                    */
                }
                catch (System.Exception e)
                {
                    // Console.WriteLine(e.Message);
                    throw new NullReferenceException("Quik Transaction Reply Exception");
                   
                }
            }

            #endregion

            #endregion

            #region Orders

            public Int32 SubscribeOrders(string classCode, string securityCode)
            {
                var result = subscribe_orders(classCode, securityCode);
                return result;
            }
            public Int32 UnSubscribeOrders()
            {
                var result = unsubscribe_orders();
                return result;
            }
            public Int32 StartOrders()
            {
                Int32 r;
                if (_orderStatusCallback != null)
                {
                    r = start_orders(_orderStatusCallback);
                }
                else throw new NullReferenceException("OrderStatusCallback"); 
                return r;
            }
            #endregion

            #region Trades

            public Int32 SubscribeTrades(string classCode, string securityCode)
            {
                var result = subscribe_trades(classCode, securityCode);
                return result;
            }
            public Int32 UnSubscribeTrades()
            {
                var result = unsubscribe_trades();
                return result;
            }
            public Int32 StartTrades()
            {
                Int32 r;
                if (_tradeStatusCallback != null)
                {
                    r = start_trades(_tradeStatusCallback);
                }
                else throw new NullReferenceException("TradeStatusCallback");
                return r;
            }

            //public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
            //                         Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor);
            #endregion
            
            #endregion

            #region Trans2Quik Prototypes TRANS2QUIK.DLL

            #region Connect / Disconnect

            #region connect
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_CONNECT@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 connect(string lpcstrConnectionParamsString, ref Int32 pnExtendedErrorCode,
               byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);
            #endregion

            #region is_dll_connected
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_IS_DLL_CONNECTED@12",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 is_dll_connected(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region disconnect
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_DISCONNECT@12",
              CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 disconnect(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region is_quik_connected
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_IS_QUIK_CONNECTED@12",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 is_quik_connected(
                 ref Int32 pnExtendedErrorCode,
                 byte[] lpstrErrorMessage,
                 UInt32 dwErrorMessageSize);
            #endregion
            #region connection_status_callback
            //long TRANS2QUIK_API __stdcall TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK (
            //    TRANS2QUIK_CONNECTION_STATUS_CALLBACK pfConnectionStatusCallback, 
            //    long* pnExtendedErrorCode, 
            //    LPSTR lpstrErrorMessage, 
            //    DWORD dwErrorMessageSize);          

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK@16",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 set_connection_status_callback(
                ConnectionStatusCallback pfConnectionStatusCallback,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                Int32 dwErrorMessageSize);

            #endregion

            #endregion

            #region Transactons

            #region send_sync_transaction
            //long TRANS2QUIK_API __stdcall TRANS2QUIK_SEND_SYNC_TRANSACTION (
            //    LPSTR lpstTransactionString, 
            //    long* pnReplyCode, 
            //    PDWORD pdwTransId, 
            //    double* pdOrderNum, 
            //    LPSTR lpstrResultMessage, 
            //    DWORD dwResultMessageSize, 
            //    long* pnExtendedErrorCode, 
            //    LPSTR lpstErrorMessage, 
            //    DWORD dwErrorMessageSize);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_SYNC_TRANSACTION@36",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 send_sync_transaction(
                string lpstTransactionString,
                ref Int32 pnReplyCode,
                ref Int32 pdwTransId,
                ref double pdOrderNum,
                byte[] lpstrResultMessage,
                UInt32 dwResultMessageSize,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region send_async_transaction
            /*
            public delegate void transaction_reply_callback(
                Int32 nTransactionResult,
                Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                UInt32 dwTransId,
                Double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string TransactionReplyMessage);
            */
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 set_transaction_reply_callback(
                TransactionReplyCallback pfTransactionReplyCallback,
                ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_ASYNC_TRANSACTION@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 send_async_transaction(
                [MarshalAs(UnmanagedType.LPStr)]string transactionString,
                ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);

            #endregion

            #endregion

            #region Orders Status Receiving
            
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SUBSCRIBE_ORDERS@8", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 subscribe_orders(
                [MarshalAs(UnmanagedType.LPStr)]string class_code,
                [MarshalAs(UnmanagedType.LPStr)]string sec_code);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_ORDERS@0", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 unsubscribe_orders();

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_ORDERS@4", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 start_orders(OrderStatusCallback pfOrderStatusCallback);

            #region order_descriptor_functions
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_QTY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_QTY(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_DATE(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACTIVATION_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_ACTIVATION_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_WITHDRAW_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_WITHDRAW_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_EXPIRY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_EXPIRY(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACCRUED_INT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_ORDER_ACCRUED_INT(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_YIELD@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_ORDER_YIELD(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_UID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_UID(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_USERID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_USERID_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_USERID(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_USERID_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_ACCOUNT_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_ACCOUNT(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_ACCOUNT_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_BROKERREF@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_BROKERREF_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_BROKERREF(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_BROKERREF_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_CLIENT_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_CLIENT_CODE(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_FIRMID_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_FIRMID(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_FIRMID_IMPL(nOrderDescriptor));
            }

            #endregion

            public string CreateOrderStr(Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string secCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, Int32 nOrderDescriptor)
            {
                var sb = new StringBuilder();
                sb.Append(dwTransId); sb.Append(";");
                sb.Append(dNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_DATE(nOrderDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_TIME(nOrderDescriptor)); sb.Append(";");
                sb.Append(secCode); sb.Append(";");
                sb.Append(nIsSell); sb.Append(";");
                sb.Append(dPrice); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_QTY(nOrderDescriptor)); sb.Append(";");
                sb.Append(nStatus); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor)); sb.Append(";");

                return sb.ToString();
            }
            private void OrderStatusDefaultCallback(
                        Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string sSecCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 iIsSell, Int32 iStatus, Int32 nOrderDescriptor)
            {
                var iDate = TRANS2QUIK_ORDER_DATE(nOrderDescriptor);
                var iTime = TRANS2QUIK_ORDER_TIME(nOrderDescriptor);

                var iActivationTime = TRANS2QUIK_ORDER_ACTIVATION_TIME(nOrderDescriptor);
                var iCancelTime = TRANS2QUIK_ORDER_WITHDRAW_TIME(nOrderDescriptor);
                var iExpireDate = TRANS2QUIK_ORDER_EXPIRY(nOrderDescriptor);

                var iQty =  TRANS2QUIK_ORDER_QTY(nOrderDescriptor);
                var sAcc = TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor);
                var sClientCode = TRANS2QUIK_ORDER_CLIENT_CODE(nOrderDescriptor);

                var comment = TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor);
                var sStrat = comment;

                _t2QReceiver?.NewOrderStatus(dNumber, iDate, iTime, nMode, iActivationTime, iCancelTime, iExpireDate,
                    sAcc, sStrat, classCode, sSecCode, iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);

                //var orderString = CreateOrderStr(nMode, dwTransId, dNumber, classCode, sSecCode,
                //     dPrice, nBalance, dValue, iIsSell, iStatus, nOrderDescriptor);

            }
            private static void order_status_callback_impl(
                    Int32 nMode, UInt32 dwTransID, Double dNumber, string ClassCode, string SecCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, Int32 nOrderDescriptor)
            {
                String mainString = "Mode=" + nMode + " TransId=" + dwTransID + " Num=" + dNumber +
                     " Class=" + ClassCode + " Sec=" + SecCode + " Price=" + dPrice +
                     " Balance=" + nBalance + " Value=" + dValue + " IsSell=" + nIsSell + " Status=" + nStatus;
                String addString = "";
                String strString = "";

                addString = " Qty=" + TRANS2QUIK_ORDER_QTY(nOrderDescriptor) +
                    " Date=" + TRANS2QUIK_ORDER_DATE(nOrderDescriptor) +
                    " Time=" + TRANS2QUIK_ORDER_TIME(nOrderDescriptor) +
                    " ActTime=" + TRANS2QUIK_ORDER_ACTIVATION_TIME(nOrderDescriptor) +
                    " WDTime=" + TRANS2QUIK_ORDER_WITHDRAW_TIME(nOrderDescriptor) +
                    " Expiry=" + TRANS2QUIK_ORDER_EXPIRY(nOrderDescriptor) +
                    " Accruedint=" + TRANS2QUIK_ORDER_ACCRUED_INT(nOrderDescriptor) +
                    " Yield=" + TRANS2QUIK_ORDER_YIELD(nOrderDescriptor) +
                    " UID=" + TRANS2QUIK_ORDER_UID(nOrderDescriptor);
                try
                {
                    strString = ""
                          + " USERID=" + TRANS2QUIK_ORDER_USERID(nOrderDescriptor)
                          + " Account=" + TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor)
                          + " Brokerref=" + TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor)
                          + " ClientCode=" + TRANS2QUIK_ORDER_CLIENT_CODE(nOrderDescriptor)
                          + " Firmid=" + TRANS2QUIK_ORDER_FIRMID(nOrderDescriptor)
                        ;
                }
                catch (AccessViolationException e)
                {
                    using (var errorFile = new StreamWriter(@"errors.log"))
                    {
                        errorFile.WriteLine(e.ToString());
                    }
                }
                try
                {
                    using (StreamWriter file = new System.IO.StreamWriter(@"orders.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        var mes = mainString + addString + strString;
                        Console.WriteLine("ORDER_CALLBACK:\n " + mes);
                        file.WriteLine(mes);
                        file.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            #endregion

            #region Trades Status Receiving ( Receiving Trades )

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SUBSCRIBE_TRADES@8", CallingConvention = CallingConvention.StdCall)] 
            private static extern Int32 subscribe_trades([MarshalAs(UnmanagedType.LPStr)]string class_code,
                [MarshalAs(UnmanagedType.LPStr)]string sec_code);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_TRADES@0", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 unsubscribe_trades();

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_TRADES@4", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 start_trades(TradeStatusCallback pfTradeStatusCallback);

            #region trade_descriptor_functions
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_DATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_SETTLE_DATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_TIME(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_IS_MARGINAL@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_IS_MARGINAL(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCRUED_INT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_ACCRUED_INT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_YIELD@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_YIELD(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TS_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_TS_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_EXCHANGE_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_PRICE2@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_PRICE2(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_RATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO_RATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_VALUE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO_VALUE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO2_VALUE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO2_VALUE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCRUED_INT2@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_ACCRUED_INT2(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_TERM@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_REPO_TERM(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_START_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_START_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_LOWER_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_LOWER_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_UPPER_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_UPPER_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_BLOCK_SECURITIES@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_BLOCK_SECURITIES(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CURRENCY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_CURRENCY_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_CURRENCY(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CURRENCY_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_CURRENCY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_SETTLE_CURRENCY(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_SETTLE_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_ACCOUNT_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_ACCOUNT(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_ACCOUNT_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_BROKERREF@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_BROKERREF_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_BROKERREF(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_BROKERREF_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CLIENT_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_CLIENT_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_USERID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_USERID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_USERID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_USERID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_FIRMID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_FIRMID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_FIRMID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_PARTNER_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_PARTNER_FIRMID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_EXCHANGE_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_EXCHANGE_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_STATION_ID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_STATION_ID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_STATION_ID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_STATION_ID_IMPL(nTradeDescriptor));
            }

            #endregion
            #endregion

            public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
                               Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                var sb = new StringBuilder();

                sb.Append(dNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_DATE(nTradeDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_TIME(nTradeDescriptor)); sb.Append(";");
                sb.Append(secCode); sb.Append(";");
                sb.Append(nIsSell); sb.Append(";");
                sb.Append(dPrice); sb.Append(";");
                sb.Append(nQty); sb.Append(";");
                sb.Append(dOrderNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor)); sb.Append(";");

                return sb.ToString();
            }

 //           private readonly TradeStatusCallback _tradeStatusDefaultCallback;
            private void TradeStatusDefaultCallback(
                    Int32 nMode, Double dNumber, Double dOrderNumber, string sClassCode, string sSecCode,
                    Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                var iDate = TRANS2QUIK_TRADE_DATE(nTradeDescriptor);
                var iTime = TRANS2QUIK_TRADE_TIME(nTradeDescriptor);

                
                var sAcc = TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor);
                var sStrat = TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor);
                var sClientCode = TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor);

                var dCommissionTs = TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor);

                var sTradeExchange = TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor);

                //var tradeString = CreateTradeStr(nMode, dNumber, dOrderNumber, ClassCode, SecCode,
                //    dPrice, nQty, dValue, nIsSell, nTradeDescriptor);

                _t2QReceiver?.NewTrade(dNumber, iDate, iTime, nMode,
                    sAcc, sStrat, sClassCode, sSecCode, nIsSell, nQty, dPrice, sClientCode,
                    dOrderNumber, dCommissionTs);
            }
            private static void trade_status_callback_impl(
                    Int32 nMode, Double dNumber, Double dOrderNumber, string ClassCode, string SecCode,
                    Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                String mainString = "Mode=" + nMode + " TradeNum=" + dNumber + " OrderNum=" + dOrderNumber +
                     " Class=" + ClassCode + " Sec=" + SecCode +
                     " Price=" + dPrice + " Volume=" + nQty + " Value=" + dValue + " IsSell=" + nIsSell;

                String addString = " SettleDate=" + TRANS2QUIK_TRADE_SETTLE_DATE(nTradeDescriptor) +
                    " TradeDate=" + TRANS2QUIK_TRADE_DATE(nTradeDescriptor) +
                    " TradeTime=" + TRANS2QUIK_TRADE_TIME(nTradeDescriptor) +
                    " IsMarginal=" + TRANS2QUIK_TRADE_IS_MARGINAL(nTradeDescriptor) +
                    " AccruedInt=" + TRANS2QUIK_TRADE_ACCRUED_INT(nTradeDescriptor) +
                    " Yield=" + TRANS2QUIK_TRADE_YIELD(nTradeDescriptor) +
                    " ClearingComm=" + TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(nTradeDescriptor) +
                    " ExchangeComm=" + TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(nTradeDescriptor) +
                    " TSComm=" + TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor) +
                    " TradingSysComm=" + TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(nTradeDescriptor) +
                    " Price2=" + TRANS2QUIK_TRADE_PRICE2(nTradeDescriptor) +
                    " RepoRate=" + TRANS2QUIK_TRADE_REPO_RATE(nTradeDescriptor) +
                    " Repo2Value=" + TRANS2QUIK_TRADE_REPO2_VALUE(nTradeDescriptor) +
                    " AccruedInt2=" + TRANS2QUIK_TRADE_ACCRUED_INT2(nTradeDescriptor) +
                    " RepoTerm=" + TRANS2QUIK_TRADE_REPO_TERM(nTradeDescriptor) +
                    " StartDisc=" + TRANS2QUIK_TRADE_START_DISCOUNT(nTradeDescriptor) +
                    " LowerDisc=" + TRANS2QUIK_TRADE_LOWER_DISCOUNT(nTradeDescriptor) +
                    " UpperDisc=" + TRANS2QUIK_TRADE_UPPER_DISCOUNT(nTradeDescriptor) +
                    " BlockSec=" + TRANS2QUIK_TRADE_BLOCK_SECURITIES(nTradeDescriptor);

                String strString = ""
                    + "Currency=" + TRANS2QUIK_TRADE_CURRENCY(nTradeDescriptor)
                    + " SettleCurrency=" + TRANS2QUIK_TRADE_SETTLE_CURRENCY(nTradeDescriptor)
                    + " SettleCode=" + TRANS2QUIK_TRADE_SETTLE_CODE(nTradeDescriptor)
                    + " Account=" + TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor)
                    + " Brokerref=" + TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor)
                    + " Cliencode=" + TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor)
                    + " Userid=" + TRANS2QUIK_TRADE_USERID(nTradeDescriptor)
                    + " Firmid=" + TRANS2QUIK_TRADE_FIRMID(nTradeDescriptor)
                    + " PartnFirmid=" + TRANS2QUIK_TRADE_PARTNER_FIRMID(nTradeDescriptor)
                    + " ExchangeCode=" + TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor)
                    + " StationId=" + TRANS2QUIK_TRADE_STATION_ID(nTradeDescriptor)
                    ;

                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"trades.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        file.WriteLine(mainString + addString + strString);
                        file.Close();
                        Console.WriteLine("TRADE_CALLBACK:\n" + mainString + addString + strString);
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            #endregion

            #endregion
            
        }

        internal sealed class Trans2Quik02 : ITrans2Quik
        {
            // private  const string Trans2QuikDllName = @"D:\MTS\Trans2QuikDll\Trans2Quik_1.dll";
            private const string Trans2QuikDllName = "trans2quik.dll";

            //public IQuikTradeTerminal QuikTerminal { get; set; }
            private static ITrans2QuikReceiver _t2QReceiver;

            #region The Same as Trans2Quik01

            #region Event Handlers

            private ConnectionStatusCallback _connectionStatusCallback;
            private TransactionReplyCallback _transactionReplyCallback;
            private TradeStatusCallback _tradeStatusCallback;
            private OrderStatusCallback _orderStatusCallback;

            #endregion

            public string GetTrans2QuikDllName { get { return Trans2QuikDllName; } }

            public Trans2Quik02() { }
            public void Init()
            {
                _connectionStatusCallback = new ConnectionStatusCallback(ConnectionStatusDefaultCallback);
                _transactionReplyCallback = new TransactionReplyCallback(TransactionReplyDefaultCallback);
                _tradeStatusCallback = new TradeStatusCallback(TradeStatusDefaultCallback);
                _orderStatusCallback = new OrderStatusCallback(OrderStatusDefaultCallback);

                SetConnectionStatusCallback(_connectionStatusCallback);
                //SetTransactionReplyCallback(_transactionReplyCallback);
            }

            #region My Methods

            #region IReceivers

            public void SetT2QReceiver(ITrans2QuikReceiver trans2QuikReceiver)
            {
                _t2QReceiver = trans2QuikReceiver as ITrans2QuikReceiver;
                if (_t2QReceiver == null) throw new ArgumentNullException("Trans2QuikReceiver");
            }
            #endregion

            #region Connect / Disconnect

            public Int32 Connect2Quik(string pathToQuik)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = connect(pathToQuik, ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("Connect\t\t{0} {1} ", result & 255, ResultToString(result & 255));
                //Console.WriteLine("Connect\t\t{0} {1} ", ((T2QConnectionResults)(result & 255)), ResultToString(result & 255));
                //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSizez={2}", (ExtendedErrorCode & 255), ErrorMessage, ErrorMessageSize);

                //if ( result == T2QResults.TRANS2QUIK_FAILED )
                return result;
            }
            public Int32 IsConnected2Quik()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = is_dll_connected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("IsConnectedToQuik>\t{0} {1}", result, ResultToString(result & 255));
                //return result == TRANS2QUIK_DLL_CONNECTED;

                return result;
            }
            public Int32 Disconnect()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = disconnect(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("Disconnect>\t\t{0} {1}", (result), ResultToString(result));
                //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSize={2}", (ExtendedErrorCode & 255).ToString(), ByteToString(ErrorMessage), ErrorMessageSize);
                //return result == TRANS2QUIK_SUCCESS;
                return result;
            }
            public Int32 IsConnected2Server()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                Int32 result = is_quik_connected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //    Console.WriteLine("test_q.is_quik_connected_test>\t{0} {1}", (result & 255), ResultToString(result & 255));
                //return result == TRANS2QUIK_QUIK_CONNECTED;
                return result;
            }
            public Int32 SetConnectionStatusCallback(ConnectionStatusCallback cb)
            {
                // _connectionStatusCallback += cb;

                var errorMessage = new Byte[50];
                const Int32 errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = set_connection_status_callback(_connectionStatusCallback, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result & 0xff;
            }
            private static void ConnectionStatusDefaultCallback(Int32 nConnectionEvent, Int32 nExtendedErrorCode, byte[] lpstrInfoMessage)
            {
                if (_t2QReceiver != null)
                    _t2QReceiver.ConnectionStatusChanged(nConnectionEvent, nExtendedErrorCode, lpstrInfoMessage);
            }
            #endregion

            #region Transactions

            #region Sync Transaction
            public Int32 SendSyncTransaction(string transactionStr, ref double orderNum)
            {
                var errorMessage = new Byte[50];
                var resultMessage = new Byte[50];
                var extendedErrorCode = -100;
                var replyCode = 0;
                // ************** Why int TransID; May be long  ????????????????
                Int32 transId = 0;
                const uint resultMessageSize = 50;
                const uint errorMessageSize = 50;

                var result = send_sync_transaction(transactionStr, ref replyCode, ref transId, ref orderNum, resultMessage, resultMessageSize,
                    ref extendedErrorCode, errorMessage, errorMessageSize);

                //Console.WriteLine("{0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSize={2}",
                //    (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);

                //String resStr = ByteToString(resultMessage);
                //resStr = resStr.Trim();
                // Console.WriteLine(" ReplyCode={0} TransID={1}  OrderNum={2} \n ResultMessage={3}, ResultMessageSize={4}",
                //    replyCode, transId, orderNum, resStr, resultMessageSize);
                return result;
            }
            #endregion

            #region AsyncTransaction

            public Int32 SendAsyncTransaction(string transactionString)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
                //Console.WriteLine("Send async transaction res={0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtEC={0}, EMsg={1}, EMsgSz={2}", (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);

                return result & 255;
            }
            public Int32 SendAsyncTransaction(string transactionString, out int extendedErrCode)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
                //Console.WriteLine("Send async transaction res={0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtEC={0}, EMsg={1}, EMsgSz={2}", (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);
                extendedErrCode = extendedErrorCode;
                return result & 255;
            }

            public Int32 SetTransactionReplyCallback(TransactionReplyCallback cb)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = set_transaction_reply_callback(cb, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result;
            }
            private static void TransactionReplyDefaultCallback(Int32 nTransactionResult, Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                uint dwTransId, double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage)
            {
                if (_t2QReceiver != null)
                    _t2QReceiver.TransactionReply(nTransactionResult, nTransactionExtendedErrorCode, nTransactionReplyCode,
                        dwTransId, dOrderNum, transactionReplyMessage);
            }

            public static void TransactionReplyCallbackImpl(
                Int32 nTransactionResult,
                Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                UInt32 dwTransId,
                Double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage)
            {
                String strOutput = "TrRes=" + nTransactionResult +
                    // " TrResStr=" + ResultToString(nTransactionResult & 255) +
                    " TrExErrCode=" + nTransactionExtendedErrorCode +
                    " TrReplyCode=" + nTransactionReplyCode +
                    " TrID=" + dwTransId +
                    " OrderNum=" + dOrderNum +
                    " ResMsg=" + transactionReplyMessage;

                try
                {
                    /*
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"async_trans.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        file.WriteLine(strOutput);
                        Console.WriteLine("TRANS_REPLY_CALLBACK: " + strOutput);
                    }
                    */
                }
                catch (System.Exception e)
                {
                    // Console.WriteLine(e.Message);
                    throw new NullReferenceException("Quik Transaction Reply Exception");

                }
            }

            #endregion

            #endregion

            #region Orders

            public Int32 SubscribeOrders(string classCode, string securityCode)
            {
                var result = subscribe_orders(classCode, securityCode);
                return result;
            }
            public Int32 UnSubscribeOrders()
            {
                var result = unsubscribe_orders();
                return result;
            }
            public Int32 StartOrders()
            {
                Int32 r;
                if (_orderStatusCallback != null)
                {
                    r = start_orders(_orderStatusCallback);
                }
                else throw new NullReferenceException("OrderStatusCallback");
                return r;
            }
            #endregion

            #region Trades

            public Int32 SubscribeTrades(string classCode, string securityCode)
            {
                var result = subscribe_trades(classCode, securityCode);
                return result;
            }
            public Int32 UnSubscribeTrades()
            {
                var result = unsubscribe_trades();
                return result;
            }
            public Int32 StartTrades()
            {
                Int32 r;
                if (_tradeStatusCallback != null)
                {
                    r = start_trades(_tradeStatusCallback);
                }
                else throw new NullReferenceException("TradeStatusCallback");
                return r;
            }

            //public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
            //                         Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor);
            #endregion

            #endregion

            #region Trans2Quik Prototypes TRANS2QUIK.DLL

            #region Connect / Disconnect

            #region connect
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_CONNECT@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 connect(string lpcstrConnectionParamsString, ref Int32 pnExtendedErrorCode,
               byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);
            #endregion

            #region is_dll_connected
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_IS_DLL_CONNECTED@12",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 is_dll_connected(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region disconnect
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_DISCONNECT@12",
              CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 disconnect(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region is_quik_connected
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_IS_QUIK_CONNECTED@12",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 is_quik_connected(
                 ref Int32 pnExtendedErrorCode,
                 byte[] lpstrErrorMessage,
                 UInt32 dwErrorMessageSize);
            #endregion
            #region connection_status_callback
            //long TRANS2QUIK_API __stdcall TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK (
            //    TRANS2QUIK_CONNECTION_STATUS_CALLBACK pfConnectionStatusCallback, 
            //    long* pnExtendedErrorCode, 
            //    LPSTR lpstrErrorMessage, 
            //    DWORD dwErrorMessageSize);          

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK@16",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 set_connection_status_callback(
                ConnectionStatusCallback pfConnectionStatusCallback,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                Int32 dwErrorMessageSize);

            #endregion

            #endregion

            #region Transactons

            #region send_sync_transaction
            //long TRANS2QUIK_API __stdcall TRANS2QUIK_SEND_SYNC_TRANSACTION (
            //    LPSTR lpstTransactionString, 
            //    long* pnReplyCode, 
            //    PDWORD pdwTransId, 
            //    double* pdOrderNum, 
            //    LPSTR lpstrResultMessage, 
            //    DWORD dwResultMessageSize, 
            //    long* pnExtendedErrorCode, 
            //    LPSTR lpstErrorMessage, 
            //    DWORD dwErrorMessageSize);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_SYNC_TRANSACTION@36",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 send_sync_transaction(
                string lpstTransactionString,
                ref Int32 pnReplyCode,
                ref Int32 pdwTransId,
                ref double pdOrderNum,
                byte[] lpstrResultMessage,
                UInt32 dwResultMessageSize,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region send_async_transaction
            /*
            public delegate void transaction_reply_callback(
                Int32 nTransactionResult,
                Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                UInt32 dwTransId,
                Double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string TransactionReplyMessage);
            */
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 set_transaction_reply_callback(
                TransactionReplyCallback pfTransactionReplyCallback,
                ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_ASYNC_TRANSACTION@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 send_async_transaction(
                [MarshalAs(UnmanagedType.LPStr)]string transactionString,
                ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);

            #endregion

            #endregion

            #region Orders Status Receiving

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SUBSCRIBE_ORDERS@8", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 subscribe_orders(
                [MarshalAs(UnmanagedType.LPStr)]string class_code,
                [MarshalAs(UnmanagedType.LPStr)]string sec_code);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_ORDERS@0", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 unsubscribe_orders();

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_ORDERS@4", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 start_orders(OrderStatusCallback pfOrderStatusCallback);

            #region order_descriptor_functions
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_QTY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_QTY(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_DATE(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACTIVATION_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_ACTIVATION_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_WITHDRAW_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_WITHDRAW_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_EXPIRY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_EXPIRY(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACCRUED_INT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_ORDER_ACCRUED_INT(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_YIELD@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_ORDER_YIELD(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_UID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_UID(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_USERID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_USERID_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_USERID(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_USERID_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_ACCOUNT_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_ACCOUNT(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_ACCOUNT_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_BROKERREF@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_BROKERREF_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_BROKERREF(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_BROKERREF_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_CLIENT_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_CLIENT_CODE(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_FIRMID_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_FIRMID(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_FIRMID_IMPL(nOrderDescriptor));
            }

            #endregion

            public string CreateOrderStr(Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string secCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, Int32 nOrderDescriptor)
            {
                var sb = new StringBuilder();
                sb.Append(dwTransId); sb.Append(";");
                sb.Append(dNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_DATE(nOrderDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_TIME(nOrderDescriptor)); sb.Append(";");
                sb.Append(secCode); sb.Append(";");
                sb.Append(nIsSell); sb.Append(";");
                sb.Append(dPrice); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_QTY(nOrderDescriptor)); sb.Append(";");
                sb.Append(nStatus); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor)); sb.Append(";");

                return sb.ToString();
            }
            private void OrderStatusDefaultCallback(
                        Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string sSecCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 iIsSell, Int32 iStatus, Int32 nOrderDescriptor)
            {
                var iDate = TRANS2QUIK_ORDER_DATE(nOrderDescriptor);
                var iTime = TRANS2QUIK_ORDER_TIME(nOrderDescriptor);

                var iActivationTime = TRANS2QUIK_ORDER_ACTIVATION_TIME(nOrderDescriptor);
                var iCancelTime = TRANS2QUIK_ORDER_WITHDRAW_TIME(nOrderDescriptor);
                var iExpireDate = TRANS2QUIK_ORDER_EXPIRY(nOrderDescriptor);

                var iQty = TRANS2QUIK_ORDER_QTY(nOrderDescriptor);
                var sAcc = TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor);
                var sClientCode = TRANS2QUIK_ORDER_CLIENT_CODE(nOrderDescriptor);

                var comment = TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor);
                var sStrat = comment;

                if (_t2QReceiver != null)
                    _t2QReceiver.NewOrderStatus(dNumber, iDate, iTime, nMode, iActivationTime, iCancelTime, iExpireDate,
                                  sAcc, sStrat, classCode, sSecCode, iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);

                //var orderString = CreateOrderStr(nMode, dwTransId, dNumber, classCode, sSecCode,
                //     dPrice, nBalance, dValue, iIsSell, iStatus, nOrderDescriptor);

            }
            private static void order_status_callback_impl(
                    Int32 nMode, UInt32 dwTransID, Double dNumber, string ClassCode, string SecCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, Int32 nOrderDescriptor)
            {
                String mainString = "Mode=" + nMode + " TransId=" + dwTransID + " Num=" + dNumber +
                     " Class=" + ClassCode + " Sec=" + SecCode + " Price=" + dPrice +
                     " Balance=" + nBalance + " Value=" + dValue + " IsSell=" + nIsSell + " Status=" + nStatus;
                String addString = "";
                String strString = "";

                addString = " Qty=" + TRANS2QUIK_ORDER_QTY(nOrderDescriptor) +
                    " Date=" + TRANS2QUIK_ORDER_DATE(nOrderDescriptor) +
                    " Time=" + TRANS2QUIK_ORDER_TIME(nOrderDescriptor) +
                    " ActTime=" + TRANS2QUIK_ORDER_ACTIVATION_TIME(nOrderDescriptor) +
                    " WDTime=" + TRANS2QUIK_ORDER_WITHDRAW_TIME(nOrderDescriptor) +
                    " Expiry=" + TRANS2QUIK_ORDER_EXPIRY(nOrderDescriptor) +
                    " Accruedint=" + TRANS2QUIK_ORDER_ACCRUED_INT(nOrderDescriptor) +
                    " Yield=" + TRANS2QUIK_ORDER_YIELD(nOrderDescriptor) +
                    " UID=" + TRANS2QUIK_ORDER_UID(nOrderDescriptor);
                try
                {
                    strString = ""
                          + " USERID=" + TRANS2QUIK_ORDER_USERID(nOrderDescriptor)
                          + " Account=" + TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor)
                          + " Brokerref=" + TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor)
                          + " ClientCode=" + TRANS2QUIK_ORDER_CLIENT_CODE(nOrderDescriptor)
                          + " Firmid=" + TRANS2QUIK_ORDER_FIRMID(nOrderDescriptor)
                        ;
                }
                catch (AccessViolationException e)
                {
                    using (var errorFile = new StreamWriter(@"errors.log"))
                    {
                        errorFile.WriteLine(e.ToString());
                    }
                }
                try
                {
                    using (StreamWriter file = new System.IO.StreamWriter(@"orders.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        var mes = mainString + addString + strString;
                        Console.WriteLine("ORDER_CALLBACK:\n " + mes);
                        file.WriteLine(mes);
                        file.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            #endregion

            #region Trades Status Receiving ( Receiving Trades )

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SUBSCRIBE_TRADES@8", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 subscribe_trades([MarshalAs(UnmanagedType.LPStr)]string class_code,
                [MarshalAs(UnmanagedType.LPStr)]string sec_code);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_TRADES@0", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 unsubscribe_trades();

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_TRADES@4", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 start_trades(TradeStatusCallback pfTradeStatusCallback);

            #region trade_descriptor_functions
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_DATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_SETTLE_DATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_TIME(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_IS_MARGINAL@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_IS_MARGINAL(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCRUED_INT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_ACCRUED_INT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_YIELD@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_YIELD(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TS_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_TS_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_EXCHANGE_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_PRICE2@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_PRICE2(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_RATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO_RATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_VALUE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO_VALUE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO2_VALUE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO2_VALUE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCRUED_INT2@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_ACCRUED_INT2(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_TERM@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_REPO_TERM(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_START_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_START_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_LOWER_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_LOWER_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_UPPER_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_UPPER_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_BLOCK_SECURITIES@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_BLOCK_SECURITIES(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CURRENCY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_CURRENCY_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_CURRENCY(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CURRENCY_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_CURRENCY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_SETTLE_CURRENCY(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_SETTLE_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_ACCOUNT_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_ACCOUNT(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_ACCOUNT_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_BROKERREF@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_BROKERREF_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_BROKERREF(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_BROKERREF_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CLIENT_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_CLIENT_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_USERID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_USERID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_USERID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_USERID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_FIRMID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_FIRMID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_FIRMID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_PARTNER_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_PARTNER_FIRMID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_EXCHANGE_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_EXCHANGE_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_STATION_ID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_STATION_ID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_STATION_ID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_STATION_ID_IMPL(nTradeDescriptor));
            }

            #endregion
            #endregion

            public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
                               Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                var sb = new StringBuilder();

                sb.Append(dNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_DATE(nTradeDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_TIME(nTradeDescriptor)); sb.Append(";");
                sb.Append(secCode); sb.Append(";");
                sb.Append(nIsSell); sb.Append(";");
                sb.Append(dPrice); sb.Append(";");
                sb.Append(nQty); sb.Append(";");
                sb.Append(dOrderNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor)); sb.Append(";");

                return sb.ToString();
            }

            //           private readonly TradeStatusCallback _tradeStatusDefaultCallback;
            private void TradeStatusDefaultCallback(
                    Int32 nMode, Double dNumber, Double dOrderNumber, string sClassCode, string sSecCode,
                    Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                var iDate = TRANS2QUIK_TRADE_DATE(nTradeDescriptor);
                var iTime = TRANS2QUIK_TRADE_TIME(nTradeDescriptor);


                var sAcc = TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor);
                var sStrat = TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor);
                var sClientCode = TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor);

                var dCommissionTs = TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor);

                var sTradeExchange = TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor);

                //var tradeString = CreateTradeStr(nMode, dNumber, dOrderNumber, ClassCode, SecCode,
                //    dPrice, nQty, dValue, nIsSell, nTradeDescriptor);

                if (_t2QReceiver != null)
                    _t2QReceiver.NewTrade(dNumber, iDate, iTime, nMode,
                                          sAcc, sStrat, sClassCode, sSecCode, nIsSell, nQty, dPrice, sClientCode,
                                          dOrderNumber, dCommissionTs);
            }
            private static void trade_status_callback_impl(
                    Int32 nMode, Double dNumber, Double dOrderNumber, string ClassCode, string SecCode,
                    Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                String mainString = "Mode=" + nMode + " TradeNum=" + dNumber + " OrderNum=" + dOrderNumber +
                     " Class=" + ClassCode + " Sec=" + SecCode +
                     " Price=" + dPrice + " Volume=" + nQty + " Value=" + dValue + " IsSell=" + nIsSell;

                String addString = " SettleDate=" + TRANS2QUIK_TRADE_SETTLE_DATE(nTradeDescriptor) +
                    " TradeDate=" + TRANS2QUIK_TRADE_DATE(nTradeDescriptor) +
                    " TradeTime=" + TRANS2QUIK_TRADE_TIME(nTradeDescriptor) +
                    " IsMarginal=" + TRANS2QUIK_TRADE_IS_MARGINAL(nTradeDescriptor) +
                    " AccruedInt=" + TRANS2QUIK_TRADE_ACCRUED_INT(nTradeDescriptor) +
                    " Yield=" + TRANS2QUIK_TRADE_YIELD(nTradeDescriptor) +
                    " ClearingComm=" + TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(nTradeDescriptor) +
                    " ExchangeComm=" + TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(nTradeDescriptor) +
                    " TSComm=" + TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor) +
                    " TradingSysComm=" + TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(nTradeDescriptor) +
                    " Price2=" + TRANS2QUIK_TRADE_PRICE2(nTradeDescriptor) +
                    " RepoRate=" + TRANS2QUIK_TRADE_REPO_RATE(nTradeDescriptor) +
                    " Repo2Value=" + TRANS2QUIK_TRADE_REPO2_VALUE(nTradeDescriptor) +
                    " AccruedInt2=" + TRANS2QUIK_TRADE_ACCRUED_INT2(nTradeDescriptor) +
                    " RepoTerm=" + TRANS2QUIK_TRADE_REPO_TERM(nTradeDescriptor) +
                    " StartDisc=" + TRANS2QUIK_TRADE_START_DISCOUNT(nTradeDescriptor) +
                    " LowerDisc=" + TRANS2QUIK_TRADE_LOWER_DISCOUNT(nTradeDescriptor) +
                    " UpperDisc=" + TRANS2QUIK_TRADE_UPPER_DISCOUNT(nTradeDescriptor) +
                    " BlockSec=" + TRANS2QUIK_TRADE_BLOCK_SECURITIES(nTradeDescriptor);

                String strString = ""
                    + "Currency=" + TRANS2QUIK_TRADE_CURRENCY(nTradeDescriptor)
                    + " SettleCurrency=" + TRANS2QUIK_TRADE_SETTLE_CURRENCY(nTradeDescriptor)
                    + " SettleCode=" + TRANS2QUIK_TRADE_SETTLE_CODE(nTradeDescriptor)
                    + " Account=" + TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor)
                    + " Brokerref=" + TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor)
                    + " Cliencode=" + TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor)
                    + " Userid=" + TRANS2QUIK_TRADE_USERID(nTradeDescriptor)
                    + " Firmid=" + TRANS2QUIK_TRADE_FIRMID(nTradeDescriptor)
                    + " PartnFirmid=" + TRANS2QUIK_TRADE_PARTNER_FIRMID(nTradeDescriptor)
                    + " ExchangeCode=" + TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor)
                    + " StationId=" + TRANS2QUIK_TRADE_STATION_ID(nTradeDescriptor)
                    ;

                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"trades.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        file.WriteLine(mainString + addString + strString);
                        file.Close();
                        Console.WriteLine("TRADE_CALLBACK:\n" + mainString + addString + strString);
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            #endregion

            #endregion

        }

        internal sealed class Trans2Quik03 : ITrans2Quik
        {
            // private  const string Trans2QuikDllName = @"D:\MTS\Trans2QuikDll\Trans2Quik_1.dll";
            // private const string Trans2QuikDllName = "Trans2Quik_3.dll";
            private const string Trans2QuikDllName = "trans2quik.dll";

        //public IQuikTradeTerminal QuikTerminal { get; set; }
        private static ITrans2QuikReceiver _t2QReceiver;

            #region The Same as Trans2Quik01

            #region Event Handlers

            private ConnectionStatusCallback _connectionStatusCallback;
            private TransactionReplyCallback _transactionReplyCallback;
            private TradeStatusCallback _tradeStatusCallback;
            private OrderStatusCallback _orderStatusCallback;

            #endregion

            public string GetTrans2QuikDllName { get { return Trans2QuikDllName; } }

            public Trans2Quik03() { }
            public void Init()
            {
                _connectionStatusCallback = new ConnectionStatusCallback(ConnectionStatusDefaultCallback);
                _transactionReplyCallback = new TransactionReplyCallback(TransactionReplyDefaultCallback);
                _tradeStatusCallback = new TradeStatusCallback(TradeStatusDefaultCallback);
                _orderStatusCallback = new OrderStatusCallback(OrderStatusDefaultCallback);

                SetConnectionStatusCallback(_connectionStatusCallback);
                //SetTransactionReplyCallback(_transactionReplyCallback);
            }

            #region My Methods

            #region IReceivers

            public void SetT2QReceiver(ITrans2QuikReceiver trans2QuikReceiver)
            {
                _t2QReceiver = trans2QuikReceiver as ITrans2QuikReceiver;
                if (_t2QReceiver == null) throw new ArgumentNullException("Trans2QuikReceiver");
            }
            #endregion

            #region Connect / Disconnect

            public Int32 Connect2Quik(string pathToQuik)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = connect(pathToQuik, ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("Connect\t\t{0} {1} ", result & 255, ResultToString(result & 255));
                //Console.WriteLine("Connect\t\t{0} {1} ", ((T2QConnectionResults)(result & 255)), ResultToString(result & 255));
                //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSizez={2}", (ExtendedErrorCode & 255), ErrorMessage, ErrorMessageSize);

                //if ( result == T2QResults.TRANS2QUIK_FAILED )
                return result;
            }
            public Int32 IsConnected2Quik()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = is_dll_connected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("IsConnectedToQuik>\t{0} {1}", result, ResultToString(result & 255));
                //return result == TRANS2QUIK_DLL_CONNECTED;

                return result;
            }
            public Int32 Disconnect()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = disconnect(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //Console.WriteLine("Disconnect>\t\t{0} {1}", (result), ResultToString(result));
                //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSize={2}", (ExtendedErrorCode & 255).ToString(), ByteToString(ErrorMessage), ErrorMessageSize);
                //return result == TRANS2QUIK_SUCCESS;
                return result;
            }
            public Int32 IsConnected2Server()
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                Int32 result = is_quik_connected(ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

                //    Console.WriteLine("test_q.is_quik_connected_test>\t{0} {1}", (result & 255), ResultToString(result & 255));
                //return result == TRANS2QUIK_QUIK_CONNECTED;
                return result;
            }
            public Int32 SetConnectionStatusCallback(ConnectionStatusCallback cb)
            {
                // _connectionStatusCallback += cb;

                var errorMessage = new Byte[50];
                const Int32 errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = set_connection_status_callback(_connectionStatusCallback, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result & 0xff;
            }
            private static void ConnectionStatusDefaultCallback(Int32 nConnectionEvent, Int32 nExtendedErrorCode, byte[] lpstrInfoMessage)
            {
                if (_t2QReceiver != null)
                    _t2QReceiver.ConnectionStatusChanged(nConnectionEvent, nExtendedErrorCode, lpstrInfoMessage);
            }
            #endregion

            #region Transactions

            #region Sync Transaction
            public Int32 SendSyncTransaction(string transactionStr, ref double orderNum)
            {
                var errorMessage = new Byte[50];
                var resultMessage = new Byte[50];
                var extendedErrorCode = -100;
                var replyCode = 0;
                // ************** Why int TransID; May be long  ????????????????
                Int32 transId = 0;
                const uint resultMessageSize = 50;
                const uint errorMessageSize = 50;

                var result = send_sync_transaction(transactionStr, ref replyCode, ref transId, ref orderNum, resultMessage, resultMessageSize,
                    ref extendedErrorCode, errorMessage, errorMessageSize);

                //Console.WriteLine("{0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSize={2}",
                //    (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);

                //String resStr = ByteToString(resultMessage);
                //resStr = resStr.Trim();
                // Console.WriteLine(" ReplyCode={0} TransID={1}  OrderNum={2} \n ResultMessage={3}, ResultMessageSize={4}",
                //    replyCode, transId, orderNum, resStr, resultMessageSize);
                return result;
            }
            #endregion

            #region AsyncTransaction

            public Int32 SendAsyncTransaction(string transactionString)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
                //Console.WriteLine("Send async transaction res={0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtEC={0}, EMsg={1}, EMsgSz={2}", (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);

                return result & 255;
            }
            public Int32 SendAsyncTransaction(string transactionString, out int extendedErrCode)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
                //Console.WriteLine("Send async transaction res={0} {1}", (result & 255), ResultToString(result & 255));
                //Console.WriteLine(" ExtEC={0}, EMsg={1}, EMsgSz={2}", (extendedErrorCode & 255), ByteToString(errorMessage), errorMessageSize);
                extendedErrCode = extendedErrorCode;
                return result & 255;
            }

            public Int32 SetTransactionReplyCallback(TransactionReplyCallback cb)
            {
                var errorMessage = new Byte[50];
                const uint errorMessageSize = 50;
                var extendedErrorCode = 0;

                var result = set_transaction_reply_callback(cb, ref extendedErrorCode, errorMessage, errorMessageSize);
                return result;
            }
            private static void TransactionReplyDefaultCallback(Int32 nTransactionResult, Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                uint dwTransId, double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage)
            {
                if (_t2QReceiver != null)
                    _t2QReceiver.TransactionReply(nTransactionResult, nTransactionExtendedErrorCode, nTransactionReplyCode,
                        dwTransId, dOrderNum, transactionReplyMessage);
            }

            public static void TransactionReplyCallbackImpl(
                Int32 nTransactionResult,
                Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                UInt32 dwTransId,
                Double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage)
            {
                String strOutput = "TrRes=" + nTransactionResult +
                    // " TrResStr=" + ResultToString(nTransactionResult & 255) +
                    " TrExErrCode=" + nTransactionExtendedErrorCode +
                    " TrReplyCode=" + nTransactionReplyCode +
                    " TrID=" + dwTransId +
                    " OrderNum=" + dOrderNum +
                    " ResMsg=" + transactionReplyMessage;

                try
                {
                    /*
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"async_trans.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        file.WriteLine(strOutput);
                        Console.WriteLine("TRANS_REPLY_CALLBACK: " + strOutput);
                    }
                    */
                }
                catch (System.Exception e)
                {
                    // Console.WriteLine(e.Message);
                    throw new NullReferenceException("Quik Transaction Reply Exception");

                }
            }

            #endregion

            #endregion

            #region Orders

            public Int32 SubscribeOrders(string classCode, string securityCode)
            {
                var result = subscribe_orders(classCode, securityCode);
                return result;
            }
            public Int32 UnSubscribeOrders()
            {
                var result = unsubscribe_orders();
                return result;
            }
            public Int32 StartOrders()
            {
                Int32 r;
                if (_orderStatusCallback != null)
                {
                    r = start_orders(_orderStatusCallback);
                }
                else throw new NullReferenceException("OrderStatusCallback");
                return r;
            }
            #endregion

            #region Trades

            public Int32 SubscribeTrades(string classCode, string securityCode)
            {
                var result = subscribe_trades(classCode, securityCode);
                return result;
            }
            public Int32 UnSubscribeTrades()
            {
                var result = unsubscribe_trades();
                return result;
            }
            public Int32 StartTrades()
            {
                Int32 r;
                if (_tradeStatusCallback != null)
                {
                    r = start_trades(_tradeStatusCallback);
                }
                else throw new NullReferenceException("TradeStatusCallback");
                return r;
            }

            //public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
            //                         Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor);
            #endregion

            #endregion

            #region Trans2Quik Prototypes TRANS2QUIK.DLL

            #region Connect / Disconnect

            #region connect
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_CONNECT@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 connect(string lpcstrConnectionParamsString, ref Int32 pnExtendedErrorCode,
               byte[] lpstrErrorMessage, UInt32 dwErrorMessageSize);
            #endregion

            #region is_dll_connected
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_IS_DLL_CONNECTED@12",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 is_dll_connected(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region disconnect
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_DISCONNECT@12",
              CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 disconnect(
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region is_quik_connected
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_IS_QUIK_CONNECTED@12",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 is_quik_connected(
                 ref Int32 pnExtendedErrorCode,
                 byte[] lpstrErrorMessage,
                 UInt32 dwErrorMessageSize);
            #endregion
            #region connection_status_callback
            //long TRANS2QUIK_API __stdcall TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK (
            //    TRANS2QUIK_CONNECTION_STATUS_CALLBACK pfConnectionStatusCallback, 
            //    long* pnExtendedErrorCode, 
            //    LPSTR lpstrErrorMessage, 
            //    DWORD dwErrorMessageSize);          

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK@16",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 set_connection_status_callback(
                ConnectionStatusCallback pfConnectionStatusCallback,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                Int32 dwErrorMessageSize);

            #endregion

            #endregion

            #region Transactons

            #region send_sync_transaction
            //long TRANS2QUIK_API __stdcall TRANS2QUIK_SEND_SYNC_TRANSACTION (
            //    LPSTR lpstTransactionString, 
            //    long* pnReplyCode, 
            //    PDWORD pdwTransId, 
            //    double* pdOrderNum, 
            //    LPSTR lpstrResultMessage, 
            //    DWORD dwResultMessageSize, 
            //    long* pnExtendedErrorCode, 
            //    LPSTR lpstErrorMessage, 
            //    DWORD dwErrorMessageSize);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_SYNC_TRANSACTION@36",
                CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 send_sync_transaction(
                string lpstTransactionString,
                ref Int32 pnReplyCode,
                ref Int32 pdwTransId,
                ref double pdOrderNum,
                byte[] lpstrResultMessage,
                UInt32 dwResultMessageSize,
                ref Int32 pnExtendedErrorCode,
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);
            #endregion

            #region send_async_transaction
            /*
            public delegate void transaction_reply_callback(
                Int32 nTransactionResult,
                Int32 nTransactionExtendedErrorCode,
                Int32 nTransactionReplyCode,
                UInt32 dwTransId,
                Double dOrderNum,
                [MarshalAs(UnmanagedType.LPStr)] string TransactionReplyMessage);
            */
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 set_transaction_reply_callback(
                TransactionReplyCallback pfTransactionReplyCallback,
                ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_ASYNC_TRANSACTION@16", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 send_async_transaction(
                [MarshalAs(UnmanagedType.LPStr)]string transactionString,
                ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
                byte[] lpstrErrorMessage,
                UInt32 dwErrorMessageSize);

            #endregion

            #endregion

            #region Orders Status Receiving

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SUBSCRIBE_ORDERS@8", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 subscribe_orders(
                [MarshalAs(UnmanagedType.LPStr)]string class_code,
                [MarshalAs(UnmanagedType.LPStr)]string sec_code);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_ORDERS@0", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 unsubscribe_orders();

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_ORDERS@4", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 start_orders(OrderStatusCallback pfOrderStatusCallback);

            #region order_descriptor_functions
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_QTY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_QTY(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_DATE(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACTIVATION_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_ACTIVATION_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_WITHDRAW_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_WITHDRAW_TIME(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_EXPIRY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_EXPIRY(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACCRUED_INT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_ORDER_ACCRUED_INT(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_YIELD@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_ORDER_YIELD(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_UID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_ORDER_UID(Int32 nOrderDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_USERID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_USERID_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_USERID(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_USERID_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_ACCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_ACCOUNT_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_ACCOUNT(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_ACCOUNT_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_BROKERREF@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_BROKERREF_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_BROKERREF(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_BROKERREF_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_CLIENT_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_CLIENT_CODE(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(nOrderDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_ORDER_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_ORDER_FIRMID_IMPL(Int32 nOrderDescriptor);

            public static string TRANS2QUIK_ORDER_FIRMID(Int32 nOrderDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_FIRMID_IMPL(nOrderDescriptor));
            }

            #endregion

            public string CreateOrderStr(Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string secCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, Int32 nOrderDescriptor)
            {
                var sb = new StringBuilder();
                sb.Append(dwTransId); sb.Append(";");
                sb.Append(dNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_DATE(nOrderDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_TIME(nOrderDescriptor)); sb.Append(";");
                sb.Append(secCode); sb.Append(";");
                sb.Append(nIsSell); sb.Append(";");
                sb.Append(dPrice); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_QTY(nOrderDescriptor)); sb.Append(";");
                sb.Append(nStatus); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor)); sb.Append(";");

                return sb.ToString();
            }
            private void OrderStatusDefaultCallback(
                        Int32 nMode, UInt32 dwTransId, Double dNumber, string classCode, string sSecCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 iIsSell, Int32 iStatus, Int32 nOrderDescriptor)
            {
                var iDate = TRANS2QUIK_ORDER_DATE(nOrderDescriptor);
                var iTime = TRANS2QUIK_ORDER_TIME(nOrderDescriptor);

                var iActivationTime = TRANS2QUIK_ORDER_ACTIVATION_TIME(nOrderDescriptor);
                var iCancelTime = TRANS2QUIK_ORDER_WITHDRAW_TIME(nOrderDescriptor);
                var iExpireDate = TRANS2QUIK_ORDER_EXPIRY(nOrderDescriptor);

                var iQty = TRANS2QUIK_ORDER_QTY(nOrderDescriptor);
                var sAcc = TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor);
                var sClientCode = TRANS2QUIK_ORDER_CLIENT_CODE(nOrderDescriptor);

                var comment = TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor);
                var sStrat = comment;

                if (_t2QReceiver != null)
                    _t2QReceiver.NewOrderStatus(dNumber, iDate, iTime, nMode, iActivationTime, iCancelTime, iExpireDate,
                                  sAcc, sStrat, classCode, sSecCode, iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);

                //var orderString = CreateOrderStr(nMode, dwTransId, dNumber, classCode, sSecCode,
                //     dPrice, nBalance, dValue, iIsSell, iStatus, nOrderDescriptor);

            }
            private static void order_status_callback_impl(
                    Int32 nMode, UInt32 dwTransID, Double dNumber, string ClassCode, string SecCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, Int32 nOrderDescriptor)
            {
                String mainString = "Mode=" + nMode + " TransId=" + dwTransID + " Num=" + dNumber +
                     " Class=" + ClassCode + " Sec=" + SecCode + " Price=" + dPrice +
                     " Balance=" + nBalance + " Value=" + dValue + " IsSell=" + nIsSell + " Status=" + nStatus;
                String addString = "";
                String strString = "";

                addString = " Qty=" + TRANS2QUIK_ORDER_QTY(nOrderDescriptor) +
                    " Date=" + TRANS2QUIK_ORDER_DATE(nOrderDescriptor) +
                    " Time=" + TRANS2QUIK_ORDER_TIME(nOrderDescriptor) +
                    " ActTime=" + TRANS2QUIK_ORDER_ACTIVATION_TIME(nOrderDescriptor) +
                    " WDTime=" + TRANS2QUIK_ORDER_WITHDRAW_TIME(nOrderDescriptor) +
                    " Expiry=" + TRANS2QUIK_ORDER_EXPIRY(nOrderDescriptor) +
                    " Accruedint=" + TRANS2QUIK_ORDER_ACCRUED_INT(nOrderDescriptor) +
                    " Yield=" + TRANS2QUIK_ORDER_YIELD(nOrderDescriptor) +
                    " UID=" + TRANS2QUIK_ORDER_UID(nOrderDescriptor);
                try
                {
                    strString = ""
                          + " USERID=" + TRANS2QUIK_ORDER_USERID(nOrderDescriptor)
                          + " Account=" + TRANS2QUIK_ORDER_ACCOUNT(nOrderDescriptor)
                          + " Brokerref=" + TRANS2QUIK_ORDER_BROKERREF(nOrderDescriptor)
                          + " ClientCode=" + TRANS2QUIK_ORDER_CLIENT_CODE(nOrderDescriptor)
                          + " Firmid=" + TRANS2QUIK_ORDER_FIRMID(nOrderDescriptor)
                        ;
                }
                catch (AccessViolationException e)
                {
                    using (var errorFile = new StreamWriter(@"errors.log"))
                    {
                        errorFile.WriteLine(e.ToString());
                    }
                }
                try
                {
                    using (StreamWriter file = new System.IO.StreamWriter(@"orders.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        var mes = mainString + addString + strString;
                        Console.WriteLine("ORDER_CALLBACK:\n " + mes);
                        file.WriteLine(mes);
                        file.Close();
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            #endregion

            #region Trades Status Receiving ( Receiving Trades )

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SUBSCRIBE_TRADES@8", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 subscribe_trades([MarshalAs(UnmanagedType.LPStr)]string class_code,
                [MarshalAs(UnmanagedType.LPStr)]string sec_code);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_TRADES@0", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 unsubscribe_trades();

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_TRADES@4", CallingConvention = CallingConvention.StdCall)]
            private static extern Int32 start_trades(TradeStatusCallback pfTradeStatusCallback);

            #region trade_descriptor_functions
            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_DATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_DATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_SETTLE_DATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TIME@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_TIME(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_IS_MARGINAL@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_IS_MARGINAL(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCRUED_INT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_ACCRUED_INT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_YIELD@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_YIELD(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TS_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_TS_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_EXCHANGE_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_PRICE2@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_PRICE2(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_RATE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO_RATE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_VALUE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO_VALUE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO2_VALUE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_REPO2_VALUE(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCRUED_INT2@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_ACCRUED_INT2(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_REPO_TERM@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_REPO_TERM(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_START_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_START_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_LOWER_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_LOWER_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_UPPER_DISCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern double TRANS2QUIK_TRADE_UPPER_DISCOUNT(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_BLOCK_SECURITIES@4", CallingConvention = CallingConvention.StdCall)]
            public static extern Int32 TRANS2QUIK_TRADE_BLOCK_SECURITIES(Int32 nTradeDescriptor);

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CURRENCY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_CURRENCY_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_CURRENCY(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CURRENCY_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_CURRENCY@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_SETTLE_CURRENCY(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_SETTLE_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_SETTLE_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_ACCOUNT@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_ACCOUNT_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_ACCOUNT(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_ACCOUNT_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_BROKERREF@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_BROKERREF_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_BROKERREF(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_BROKERREF_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_CLIENT_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_CLIENT_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_USERID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_USERID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_USERID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_USERID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_FIRMID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_FIRMID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_FIRMID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_PARTNER_FIRMID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_PARTNER_FIRMID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_EXCHANGE_CODE@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_EXCHANGE_CODE(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(nTradeDescriptor));
            }

            [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_TRADE_STATION_ID@4", CallingConvention = CallingConvention.StdCall)]
            public static extern IntPtr TRANS2QUIK_TRADE_STATION_ID_IMPL(Int32 nTradeDescriptor);

            public static string TRANS2QUIK_TRADE_STATION_ID(Int32 nTradeDescriptor)
            {
                return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_STATION_ID_IMPL(nTradeDescriptor));
            }

            #endregion
            #endregion

            public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
                               Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                var sb = new StringBuilder();

                sb.Append(dNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_DATE(nTradeDescriptor)); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_TIME(nTradeDescriptor)); sb.Append(";");
                sb.Append(secCode); sb.Append(";");
                sb.Append(nIsSell); sb.Append(";");
                sb.Append(dPrice); sb.Append(";");
                sb.Append(nQty); sb.Append(";");
                sb.Append(dOrderNumber); sb.Append(";");
                sb.Append(TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor)); sb.Append(";");

                return sb.ToString();
            }

            //           private readonly TradeStatusCallback _tradeStatusDefaultCallback;
            private void TradeStatusDefaultCallback(
                    Int32 nMode, Double dNumber, Double dOrderNumber, string sClassCode, string sSecCode,
                    Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                var iDate = TRANS2QUIK_TRADE_DATE(nTradeDescriptor);
                var iTime = TRANS2QUIK_TRADE_TIME(nTradeDescriptor);


                var sAcc = TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor);
                var sStrat = TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor);
                var sClientCode = TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor);

                var dCommissionTs = TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor);

                var sTradeExchange = TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor);

                //var tradeString = CreateTradeStr(nMode, dNumber, dOrderNumber, ClassCode, SecCode,
                //    dPrice, nQty, dValue, nIsSell, nTradeDescriptor);

                if (_t2QReceiver != null)
                    _t2QReceiver.NewTrade(dNumber, iDate, iTime, nMode,
                                          sAcc, sStrat, sClassCode, sSecCode, nIsSell, nQty, dPrice, sClientCode,
                                          dOrderNumber, dCommissionTs);
            }
            private static void trade_status_callback_impl(
                    Int32 nMode, Double dNumber, Double dOrderNumber, string ClassCode, string SecCode,
                    Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor)
            {
                String mainString = "Mode=" + nMode + " TradeNum=" + dNumber + " OrderNum=" + dOrderNumber +
                     " Class=" + ClassCode + " Sec=" + SecCode +
                     " Price=" + dPrice + " Volume=" + nQty + " Value=" + dValue + " IsSell=" + nIsSell;

                String addString = " SettleDate=" + TRANS2QUIK_TRADE_SETTLE_DATE(nTradeDescriptor) +
                    " TradeDate=" + TRANS2QUIK_TRADE_DATE(nTradeDescriptor) +
                    " TradeTime=" + TRANS2QUIK_TRADE_TIME(nTradeDescriptor) +
                    " IsMarginal=" + TRANS2QUIK_TRADE_IS_MARGINAL(nTradeDescriptor) +
                    " AccruedInt=" + TRANS2QUIK_TRADE_ACCRUED_INT(nTradeDescriptor) +
                    " Yield=" + TRANS2QUIK_TRADE_YIELD(nTradeDescriptor) +
                    " ClearingComm=" + TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(nTradeDescriptor) +
                    " ExchangeComm=" + TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(nTradeDescriptor) +
                    " TSComm=" + TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor) +
                    " TradingSysComm=" + TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(nTradeDescriptor) +
                    " Price2=" + TRANS2QUIK_TRADE_PRICE2(nTradeDescriptor) +
                    " RepoRate=" + TRANS2QUIK_TRADE_REPO_RATE(nTradeDescriptor) +
                    " Repo2Value=" + TRANS2QUIK_TRADE_REPO2_VALUE(nTradeDescriptor) +
                    " AccruedInt2=" + TRANS2QUIK_TRADE_ACCRUED_INT2(nTradeDescriptor) +
                    " RepoTerm=" + TRANS2QUIK_TRADE_REPO_TERM(nTradeDescriptor) +
                    " StartDisc=" + TRANS2QUIK_TRADE_START_DISCOUNT(nTradeDescriptor) +
                    " LowerDisc=" + TRANS2QUIK_TRADE_LOWER_DISCOUNT(nTradeDescriptor) +
                    " UpperDisc=" + TRANS2QUIK_TRADE_UPPER_DISCOUNT(nTradeDescriptor) +
                    " BlockSec=" + TRANS2QUIK_TRADE_BLOCK_SECURITIES(nTradeDescriptor);

                String strString = ""
                    + "Currency=" + TRANS2QUIK_TRADE_CURRENCY(nTradeDescriptor)
                    + " SettleCurrency=" + TRANS2QUIK_TRADE_SETTLE_CURRENCY(nTradeDescriptor)
                    + " SettleCode=" + TRANS2QUIK_TRADE_SETTLE_CODE(nTradeDescriptor)
                    + " Account=" + TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor)
                    + " Brokerref=" + TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor)
                    + " Cliencode=" + TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor)
                    + " Userid=" + TRANS2QUIK_TRADE_USERID(nTradeDescriptor)
                    + " Firmid=" + TRANS2QUIK_TRADE_FIRMID(nTradeDescriptor)
                    + " PartnFirmid=" + TRANS2QUIK_TRADE_PARTNER_FIRMID(nTradeDescriptor)
                    + " ExchangeCode=" + TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor)
                    + " StationId=" + TRANS2QUIK_TRADE_STATION_ID(nTradeDescriptor)
                    ;

                try
                {
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"trades.log", true, System.Text.Encoding.GetEncoding(1251)))
                    {
                        file.WriteLine(mainString + addString + strString);
                        file.Close();
                        Console.WriteLine("TRADE_CALLBACK:\n" + mainString + addString + strString);
                    }
                }
                catch (System.Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            #endregion

            #endregion

        }
}