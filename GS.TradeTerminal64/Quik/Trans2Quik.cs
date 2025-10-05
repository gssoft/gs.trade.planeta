using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GS.Trade.TradeTerminals64.Quik
{
    //public enum T2QResults : int
    //{
    //    TRANS2QUIK_SUCCESS = 0,
    //    TRANS2QUIK_FAILED = 1,
    //    TRANS2QUIK_QUIK_TERMINAL_NOT_FOUND = 2,
    //    TRANS2QUIK_DLL_VERSION_NOT_SUPPORTED = 3,
    //    TRANS2QUIK_ALREADY_CONNECTED_TO_QUIK = 4,
    //    TRANS2QUIK_WRONG_SYNTAX = 5,
    //    TRANS2QUIK_QUIK_NOT_CONNECTED = 6,
    //    TRANS2QUIK_DLL_NOT_CONNECTED = 7,
    //    TRANS2QUIK_QUIK_CONNECTED = 8,
    //    TRANS2QUIK_QUIK_DISCONNECTED = 9,
    //    TRANS2QUIK_DLL_CONNECTED = 10,
    //    TRANS2QUIK_DLL_DISCONNECTED = 11,
    //    TRANS2QUIK_MEMORY_ALLOCATION_ERROR = 12,
    //    TRANS2QUIK_WRONG_CONNECTION_HANDLE = 13,
    //    TRANS2QUIK_WRONG_INPUT_PARAMS = 14
    //};
    public enum T2QResults : long
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

    #region Trans2Quik Delegates

    #region ConnectionStatusCallback

    // x86
    //public delegate void ConnectionStatusCallback(
    //    Int32 nConnectionEvent,
    //    Int32 nExtendedErrorCode,
    //    byte[] lpstrInfoMessage);

    // x64 ## 6.10.11 page 52
    // void __stdcall TRANS2QUIK_CONNECTION_STATUS_CALLBACK(
    //      long nConnectionEvent,
    //      long nTransactionExtendedErrorCode,
    //      LPSTR lpstrInfoMessage);
    public delegate void ConnectionStatusCallback64(
        long nConnectionEvent,
        long nExtendedErrorCode,
        byte[] lpstrInfoMessage);
    #endregion

    #region TransactionReplyCallback

    //  void __stdcall TRANS2QUIK_TRANSACTION_REPLY_CALLBACK(
    //              long nTransactionResult,
    //              long nTransactionExtendedErrorCode,
    //              long nTransactionReplyCode,
    //              DWORD dwTransId,
    //              unsigned__int64 dOrderNum,
    //              LPSTR lpstrTransactionReplyMessage,
    //              intptr_t transReplyDescriptor)
    // x86
    public delegate void TransactionReplyCallback(
        Int32 nTransactionResult,
        Int32 nTransactionExtendedErrorCode,
        Int32 nTransactionReplyCode,
        UInt32 dwTransId,
        UInt64 dOrderNum,
        [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage,
        IntPtr transReplyDescriptor);
    // 64
    public delegate void TransactionReplyCallback64(
        long nTransactionResult,
        long nTransactionExtendedErrorCode,
        long nTransactionReplyCode,
        ulong dwTransId,
        ulong dOrderNum,
        [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage,
        IntPtr transReplyDescriptor);

    #endregion

    #region OrderStatusCallBack

    //    void _stdcall TRANS2QUIK_ORDER_STATUS_CALLBACK(
    //                  long nMode,
    //                  DWORD dwTransID,
    //                  unsigned__int64 dNumber,
    //                  LPSTR lpstrClassCode,
    //                  LPSTR lpstrSecCode,
    //                  double dPrice,
    //                  __int64 nBalance,
    //                  double dValue,
    //                  long nIsSell,
    //                  long nStatus,
    //                  intptr_t nOrderDescriptor)
    // x86
    //public delegate void OrderStatusCallback(
    //    Int32 nMode,
    //    UInt32 dwTransId, // in example Int32 Error
    //    UInt64 dNumber,
    //    [MarshalAs(UnmanagedType.LPStr)] string sClassCode,
    //    [MarshalAs(UnmanagedType.LPStr)] string sSecCode,
    //    Double dPrice,
    //    Int64 nBalance, // Rest
    //    Double dValue, // Size
    //    Int32 nIsSell, // 0 = Buy, else = Sell
    //    Int32 nStatus, // Active = 1; Canceled = 2, else Filled
    //    IntPtr nOrderDescriptor);
    // x64
    public delegate void OrderStatusCallback(
        long nMode,
        ulong dwTransId, 
        ulong dNumber,
        [MarshalAs(UnmanagedType.LPStr)] string sClassCode,
        [MarshalAs(UnmanagedType.LPStr)] string sSecCode,
        double dPrice,
        long nBalance, // Rest
        double dValue, // Size
        long nIsSell, // 0 = Buy, else = Sell
        long nStatus, // Active = 1; Canceled = 2, else Filled
        IntPtr nOrderDescriptor);

    /*  Trans2Quik 32
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
    #endregion

    #region TradeStatusCallBack

    //    long __stdcall TRANS2QUIK_TRADE_STATUS_CALLBACK(
    //              long nMode,
    //              unsigned__int64 dNumber,
    //              unsigned__int64 dOrderNum,
    //              LPSTR lpstrClassCode,
    //              LPSTR lpstrSecCode,
    //              double dPrice,
    //              __int64 nQty,
    //              double dValue,
    //              long nIsSell,
    //              intptr_t nTradeDescriptor)
    // 23.10.13
    //public delegate void TradeStatusCallback(
    //    Int32 nMode,
    //    UInt64 dNumber,
    //    UInt64 dOrderNumber,
    //    [MarshalAs(UnmanagedType.LPStr)] string classCode,
    //    [MarshalAs(UnmanagedType.LPStr)] string secCode,
    //    double dPrice,
    //    Int64 nQty,
    //    double dValue,
    //    Int32 nIsSell,
    //    IntPtr nTradeDescriptor);
    // 23.10.13
    public delegate long TradeStatusCallback(
        long nMode,
        ulong dNumber,
        ulong dOrderNumber,
        [MarshalAs(UnmanagedType.LPStr)] string classCode,
        [MarshalAs(UnmanagedType.LPStr)] string secCode,
        double dPrice,
        long nQty,
        double dValue,
        long nIsSell,
        IntPtr nTradeDescriptor);
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

    #endregion

    internal partial class Trans2Quik01 : ITrans2Quik
    {
        // private  const string Trans2QuikDllName = @"D:\MTS\Trans2QuikDll\Trans2Quik_1.dll";
        private const string Trans2QuikDllName = "trans2quik.dll";

        //public IQuikTradeTerminal QuikTerminal { get; set; }
        private static ITrans2QuikReceiver _t2QReceiver;


        #region Event Handlers

        private ConnectionStatusCallback64 _connectionStatusCallback;
        private TransactionReplyCallback64 _transactionReplyCallback;
        private TradeStatusCallback _tradeStatusCallback;
        private OrderStatusCallback _orderStatusCallback;

        #endregion

        public string GetTrans2QuikDllName => Trans2QuikDllName;
        public Trans2Quik01()
        {
        }
        public void Init()
        {
            _connectionStatusCallback = new ConnectionStatusCallback64(ConnectionStatusDefaultCallback);
            _transactionReplyCallback = new TransactionReplyCallback64(TransactionReplyDefaultCallback64);
            _tradeStatusCallback = new TradeStatusCallback(TradeStatusDefaultCallback);
            _orderStatusCallback = new OrderStatusCallback(OrderStatusDefaultCallback);

            SetConnectionStatusCallback(_connectionStatusCallback);
            SetTransactionReplyCallback64(_transactionReplyCallback);
        }

        #region IReceivers

        public void SetT2QReceiver(ITrans2QuikReceiver trans2QuikReceiver)
        {
            _t2QReceiver = trans2QuikReceiver as ITrans2QuikReceiver;
            if (_t2QReceiver == null) throw new ArgumentNullException(nameof(trans2QuikReceiver));
        }

        #endregion

        #region Connect / Disconnect
        // x86
        //public Int32 Connect2Quik(string pathToQuik)
        //{
        //    var errorMessage = new Byte[50];
        //    const uint errorMessageSize = 50;
        //    var extendedErrorCode = 0;

        //    var result = connect(pathToQuik, ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

        //    //Console.WriteLine("Connect\t\t{0} {1} ", result & 255, ResultToString(result & 255));
        //    //Console.WriteLine("Connect\t\t{0} {1} ", ((T2QConnectionResults)(result & 255)), ResultToString(result & 255));
        //    //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSizez={2}", (ExtendedErrorCode & 255), ErrorMessage, ErrorMessageSize);

        //    //if ( result == T2QResults.TRANS2QUIK_FAILED )
        //    return result;
        //}
        // x64
        public int Connect2Quik(string pathToQuik)
        {
            var errorMessage = new Byte[50];
            const uint errorMessageSize = 50;
            var extendedErrorCode = 0L;

            var result = connect(pathToQuik, ref extendedErrorCode, errorMessage, errorMessageSize) & 255;

            //Console.WriteLine("Connect\t\t{0} {1} ", result & 255, ResultToString(result & 255));
            //Console.WriteLine("Connect\t\t{0} {1} ", ((T2QConnectionResults)(result & 255)), ResultToString(result & 255));
            //Console.WriteLine(" ExtendedErrorCode={0}, ErrorMessage={1}, ErrorMessageSizez={2}", (ExtendedErrorCode & 255), ErrorMessage, ErrorMessageSize);

            //if ( result == T2QResults.TRANS2QUIK_FAILED )
            return (int)result;
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
        public long Disconnect()
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
        // x86
        //public Int32 SetConnectionStatusCallback(ConnectionStatusCallback cb)
        //{
        //    // _connectionStatusCallback += cb;

        //    var errorMessage = new Byte[50];
        //    const Int32 errorMessageSize = 50;
        //    var extendedErrorCode = 0;

        //    var result = set_connection_status_callback(_connectionStatusCallback, ref extendedErrorCode, errorMessage,
        //        errorMessageSize);
        //    return result & 0xff;
        //}
        //private static void ConnectionStatusDefaultCallback(Int32 nConnectionEvent, Int32 nExtendedErrorCode,
        //    byte[] lpstrInfoMessage)
        //{
        //    //if (_t2QReceiver != null)
        //        _t2QReceiver.ConnectionStatusChanged(nConnectionEvent, nExtendedErrorCode, lpstrInfoMessage);
        //}

        private static void ConnectionStatusDefaultCallback( long nConnectionEvent, long nExtendedErrorCode, byte[] lpstrInfoMessage)
        {
            //if (_t2QReceiver != null)
            _t2QReceiver.ConnectionStatusChanged(nConnectionEvent, nExtendedErrorCode, lpstrInfoMessage);
        }

        public long SetConnectionStatusCallback(ConnectionStatusCallback64 cb)
        {
            var errorMessage = new byte[50];
            const ulong errorMessageSize = 50;
            long extendedErrorCode = 0;

            var result = set_connection_status_callback(cb, ref extendedErrorCode, errorMessage,
                errorMessageSize);
            return result & 0xff;
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

            var result = send_sync_transaction(transactionStr, ref replyCode, ref transId, ref orderNum, resultMessage,
                resultMessageSize,
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

            var result =
                send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
            return result & 255;
        }

        public Int32 SendAsyncTransaction(string transactionString, out int extendedErrCode)
        {
            var errorMessage = new Byte[50];
            const uint errorMessageSize = 50;
            var extendedErrorCode = 0;

            var result =
                send_async_transaction(transactionString, ref extendedErrorCode, errorMessage, errorMessageSize);
            extendedErrCode = extendedErrorCode;
            return result & 255;
        }
        // --------------------------------------------------------------------------
        // x86
        public Int32 SetTransactionReplyCallback(TransactionReplyCallback cb)
        {
            var errorMessage = new Byte[50];
            const uint errorMessageSize = 50;
            var extendedErrorCode = 0;

            var result = set_transaction_reply_callback(cb, ref extendedErrorCode, errorMessage, errorMessageSize);
            return result;
        }
        // x86
        private static void TransactionReplyDefaultCallback(
            Int32 nTransactionResult,
            Int32 nTransactionExtendedErrorCode,
            Int32 nTransactionReplyCode,
            uint dwTransId, ulong dOrderNum,
            [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage,
            IntPtr transReplyDescriptor
        )
        {
            _t2QReceiver?.TransactionReply(nTransactionResult, nTransactionExtendedErrorCode, nTransactionReplyCode,
                dwTransId, dOrderNum, transactionReplyMessage);
        }
        
        // --------------------------------------------------------------------------
        // x64
        public long SetTransactionReplyCallback64(TransactionReplyCallback64 callback)
        {
            var errorMessage = new byte[150];
            const long errorMessageSize = 150;
            long extendedErrorCode = 0;

            var result = set_transaction_reply_callback(callback, ref extendedErrorCode, errorMessage, errorMessageSize);
            return result;
        }
        // x64
        private static void TransactionReplyDefaultCallback64(
            long nTransactionResult,
            long nTransactionExtendedErrorCode,
            long nTransactionReplyCode,
            ulong dwTransId, ulong dOrderNum,
            [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage,
            IntPtr transReplyDescriptor
        )
        {
            _t2QReceiver?.TransactionReply64(nTransactionResult, nTransactionExtendedErrorCode, nTransactionReplyCode,
                dwTransId, dOrderNum, transactionReplyMessage);

        }

        #endregion

        #endregion

        #region Orders

        public long SubscribeOrders(string classCode, string securityCode)
        {
            var result = subscribe_orders(classCode, securityCode);
            return result;
        }

        public long UnSubscribeOrders()
        {
            var result = unsubscribe_orders();
            return result;
        }

        public void StartOrders()
        {
            if (_orderStatusCallback != null)
            {
                start_orders(_orderStatusCallback);
            }
            else throw new NullReferenceException("OrderStatusCallback");
        }

        #endregion

        #region Trades

        public long SubscribeTrades(string classCode, string securityCode)
        {
            var result = subscribe_trades(classCode, securityCode);
            return result;
        }
        public long UnSubscribeTrades()
        {
            var result = unsubscribe_trades();
            return result;
        }

        public void StartTrades()
        {
            if (_tradeStatusCallback != null)
            {
                start_trades(_tradeStatusCallback);
            }
            else throw new NullReferenceException("TradeStatusCallback");
        }

        //public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
        //                         Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, Int32 nTradeDescriptor);

        #endregion

        //  #region Trans2Quik Prototypes TRANS2QUIK.DLL

        // ===================================================

        #region Connect / Disconnect

        #region Connect

        // long __stdcall TRANS2QUIK_CONNECT(
        //          LPCSTR lpcstrConnectionParamsString,
        //          long* nTransactionExtendedErrorCode, LPSTR lpstrErrorMessage,
        //          DWORD dwErrorMessageSize)
        // [DllImport(Trans2QuikDllName,
        //          EntryPoint = "_TRANS2QUIK_CONNECT@16",
        //          CallingConvention = CallingConvention.StdCall)]
        [DllImport(Trans2QuikDllName,
            EntryPoint = "TRANS2QUIK_CONNECT",
            CallingConvention = CallingConvention.StdCall)]
        // x86
        //private static extern Int32 connect(
        //    string lpcstrConnectionParamsString,
        //    ref Int32 pnExtendedErrorCode,
        //    byte[] lpstrErrorMessage,
        //    UInt32 dwErrorMessageSize);

        // x64
        private static extern long connect(
            string lpcstrConnectionParamsString,
            ref long pnExtendedErrorCode,
            byte[] lpstrErrorMessage,
            ulong dwErrorMessageSize);

        #endregion

        #region Disconnect

        // long __stdcall TRANS2QUIK_DISCONNECT(
        //          long* nTransactionExtendedErrorCode,
        //          LPSTR lpstrErrorMessage,
        //          DWORD dwErrorMessageSize)
        // [DllImport(Trans2QuikDllName,
        //          EntryPoint = "_TRANS2QUIK_DISCONNECT@12",
        //          CallingConvention = CallingConvention.StdCall)]
        [DllImport(Trans2QuikDllName,
            EntryPoint = "TRANS2QUIK_DISCONNECT",
            CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 disconnect(
            ref Int32 pnExtendedErrorCode,
            byte[] lpstrErrorMessage,
            UInt32 dwErrorMessageSize);

        #endregion

        #region Is_Quik_Connected

        // long __stdcall TRANS2QUIK_IS_QUIK_CONNECTED(
        //      long* nTransactionExtendedErrorCode,
        //      LPSTR lpstrErrorMessage,
        //      DWORD dwErrorMessageSize)
        // [DllImport(Trans2QuikDllName,
        //      EntryPoint = "_TRANS2QUIK_IS_QUIK_CONNECTED@12",
        //      CallingConvention = CallingConvention.StdCall)]
        [DllImport(Trans2QuikDllName,
            EntryPoint = "TRANS2QUIK_IS_QUIK_CONNECTED",
            CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 is_quik_connected(
            ref Int32 pnExtendedErrorCode,
            byte[] lpstrErrorMessage,
            UInt32 dwErrorMessageSize);

        #endregion

        #region Is_dll_Connected

        // long __stdcall TRANS2QUIK_IS_DLL_CONNECTED(
        //      long* nTransactionExtendedErrorCode,
        //      LPSTR lpstrErrorMessage, DWORD dwErrorMessageSize)
        //[DllImport(Trans2QuikDllName,
        //      EntryPoint = "_TRANS2QUIK_IS_DLL_CONNECTED@12",
        //      CallingConvention = CallingConvention.StdCall)]
        [DllImport(Trans2QuikDllName,
            EntryPoint = "TRANS2QUIK_IS_DLL_CONNECTED",
            CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 is_dll_connected(
            ref Int32 pnExtendedErrorCode,
            byte[] lpstrErrorMessage,
            UInt32 dwErrorMessageSize);

        #endregion

        #region x86 set_connection_status_callback  Doc 6.10.12 pp 46

        //  long __stdcall TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK(
        //      TRANS2QUIK_CONNECTION_STATUS_CALLBACK pfConnectionStatusCallback,
        //      long* nTransactionExtendedErrorCode,
        //      LPSTR lpstrErrorMessage,
        //      DWORD dwErrorMessageSize)
        //  [DllImport(Trans2QuikDllName,
        //      EntryPoint = "_TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK@16",
        //      CallingConvention = CallingConvention.StdCall)]
        //[DllImport(Trans2QuikDllName,
        //    EntryPoint = "TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK",
        //    CallingConvention = CallingConvention.StdCall)]
        //private static extern Int32 set_connection_status_callback(
        //    ConnectionStatusCallback pfConnectionStatusCallback,
        //    ref Int32 pnExtendedErrorCode,
        //    byte[] lpstrErrorMessage,
        //    UInt32 dwErrorMessageSize);
        #endregion

        #region x64 set_connection_status_callback  Doc 6.10.12 pp 52

        [DllImport(Trans2QuikDllName,
            EntryPoint = "TRANS2QUIK_SET_CONNECTION_STATUS_CALLBACK",
            CallingConvention = CallingConvention.StdCall)]
        private static extern long set_connection_status_callback(
            ConnectionStatusCallback64 pfConnectionStatusCallback,
            ref long pnTransactionExtendedErrorCode,
            byte[] lpstrErrorMessage,
            ulong dwErrorMessageSize);

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

        #region send_async_transaction x86
        /*
        public delegate void transaction_reply_callback(
            Int32 nTransactionResult,
            Int32 nTransactionExtendedErrorCode,
            Int32 nTransactionReplyCode,
            UInt32 dwTransId,
            Double dOrderNum,
            [MarshalAs(UnmanagedType.LPStr)] string TransactionReplyMessage);
        */
        [DllImport(Trans2QuikDllName,
            EntryPoint = "_TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK@16",
            CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 set_transaction_reply_callback(
            TransactionReplyCallback pfTransactionReplyCallback,
            ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
            byte[] lpstrErrorMessage,
            UInt32 dwErrorMessageSize);

        [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_SEND_ASYNC_TRANSACTION@16",
            CallingConvention = CallingConvention.StdCall)]
        private static extern Int32 send_async_transaction(
            [MarshalAs(UnmanagedType.LPStr)] string transactionString,
            ref Int32 pnExtendedErrorCode, // ????????????????????????????????????
            byte[] lpstrErrorMessage,
            UInt32 dwErrorMessageSize);

        #endregion

        // x64
        #region send_async_transaction x64
       
        [DllImport(Trans2QuikDllName,
            EntryPoint = "TRANS2QUIK_SET_TRANSACTIONS_REPLY_CALLBACK",
            CallingConvention = CallingConvention.StdCall)]
        private static extern long set_transaction_reply_callback(
            TransactionReplyCallback64 pfTransactionReplyCallback,
            ref long pnExtendedErrorCode,
            byte[] lpstrErrorMessage,
            ulong dwErrorMessageSize);

        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_SEND_ASYNC_TRANSACTION",
            CallingConvention = CallingConvention.StdCall)]
        private static extern long send_async_transaction(
            [MarshalAs(UnmanagedType.LPStr)] string transactionString,
            ref long pnTransactionExtendedErrorCode, 
            byte[] lpstrErrorMessage,
            ulong dwErrorMessageSize);

        #endregion

        #endregion

        #region Orders Subscribe / Unsubscribe

        #region Orders Subscribe

        // long __stdcall TRANS2QUIK_SUBSCRIBE_ORDERS(
        //      LPSTR lpstrClassCode,
        //      LPSTR lpstrSeccodes)
        [DllImport(Trans2QuikDllName,
            // EntryPoint = "_TRANS2QUIK_SUBSCRIBE_ORDERS@8",
            EntryPoint = "TRANS2QUIK_SUBSCRIBE_ORDERS",
            CallingConvention = CallingConvention.StdCall)]
        private static extern long subscribe_orders(
            [MarshalAs(UnmanagedType.LPStr)] string classCode,
            [MarshalAs(UnmanagedType.LPStr)] string secCode);

        #endregion

        #region Orders UnSubscribe

        // long __stdcall TRANS2QUIK_UNSUBSCRIBE_ORDERS()
        [DllImport(Trans2QuikDllName,
            // EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_ORDERS@0",
            EntryPoint = "TRANS2QUIK_UNSUBSCRIBE_ORDERS",
            CallingConvention = CallingConvention.StdCall)]
        private static extern long unsubscribe_orders();

        #endregion

        #endregion

        #region Orders Start

        // void __stdcall TRANS2QUIK_START_ORDERS(
        //      TRANS2QUIK_ORDER_STATUS_CALLBACK pfnOrderStatusCallback)
        [DllImport(Trans2QuikDllName,
            // EntryPoint = "_TRANS2QUIK_START_ORDERS@4",
            EntryPoint = "TRANS2QUIK_START_ORDERS",
            CallingConvention = CallingConvention.StdCall)]
        private static extern void start_orders(
            OrderStatusCallback pfOrderStatusCallback);

        #region OrderStatusDefaultCallback
        //    void _stdcall TRANS2QUIK_ORDER_STATUS_CALLBACK(
        //                  long nMode,
        //                  DWORD dwTransID,
        //                  unsigned__int64 dNumber,
        //                  LPSTR lpstrClassCode,
        //                  LPSTR lpstrSecCode,
        //                  double dPrice,          // Тип: Double. Цена заявки
        //                  __int64 nBalance,       // _int64. Неисполненный остаток заявки
        //                  double dValue,          // Тип: Double. Объем заявки
        //                  long nIsSell,           // Тип: Long. Направление заявки: «0» еcли «Покупка», иначе «Продажа»
        //                  long nStatus,           // Тип: Long. Состояние исполнения заявки: Значение «1» соответствует состоянию «Активна», «2» –
        //                                          // «Снята», иначе «Исполнена»
        //                  intptr_t nOrderDescriptor)
        // x64
        //private void OrderStatusDefaultCallback(
        //    Int32 nMode, UInt32 dwTransId, UInt64 dNumber,
        //    string classCode, string sSecCode,
        //    Double dPrice, Int64 nBalance, Double dValue,
        //    Int32 iIsSell, Int32 iStatus, IntPtr pOrderDescriptor)
        //{
        //    var iDate = TRANS2QUIK_ORDER_DATE(pOrderDescriptor);
        //    var iTime = TRANS2QUIK_ORDER_TIME(pOrderDescriptor);

        //    var iActivationTime = TRANS2QUIK_ORDER_ACTIVATION_TIME(pOrderDescriptor);
        //    var iCancelTime = TRANS2QUIK_ORDER_WITHDRAW_TIME(pOrderDescriptor);
        //    var iExpireDate = TRANS2QUIK_ORDER_EXPIRY(pOrderDescriptor);

        //    var iQty = TRANS2QUIK_ORDER_QTY(pOrderDescriptor);
        //    var sAcc = TRANS2QUIK_ORDER_ACCOUNT(pOrderDescriptor);
        //    var sClientCode = TRANS2QUIK_ORDER_CLIENT_CODE(pOrderDescriptor);

        //    var comment = TRANS2QUIK_ORDER_BROKERREF(pOrderDescriptor);
        //    var sStratName = comment;

        //     _t2QReceiver?.NewOrderStatus(dNumber, iDate, iTime, nMode, iActivationTime, iCancelTime, iExpireDate,
        //         sAcc, sStratName, classCode, sSecCode, iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);

        //    var orderString = CreateOrderStr(nMode, dwTransId, dNumber, classCode, sSecCode,
        //        dPrice, nBalance, dValue, iIsSell, iStatus, pOrderDescriptor);
        //}
        // -----------------------------------------------------------------------------------------
        // x64 23.10.15
        private void OrderStatusDefaultCallback(
            long nMode, ulong dwTransId, ulong dNumber,
            string classCode, string sSecCode,
            double dPrice, long nBalance, double dValue,
            long iIsSell, long iStatus, IntPtr pOrderDescriptor)
        {
            long iDate = TRANS2QUIK_ORDER_DATE(pOrderDescriptor);
            long iTime = TRANS2QUIK_ORDER_TIME(pOrderDescriptor);

            long iActivationTime = TRANS2QUIK_ORDER_ACTIVATION_TIME(pOrderDescriptor);
            long iCancelTime = TRANS2QUIK_ORDER_WITHDRAW_TIME(pOrderDescriptor);
            long iExpireDate = TRANS2QUIK_ORDER_EXPIRY(pOrderDescriptor);

            var iQty = TRANS2QUIK_ORDER_QTY(pOrderDescriptor);
            var sAcc = TRANS2QUIK_ORDER_ACCOUNT(pOrderDescriptor);
            var sClientCode = TRANS2QUIK_ORDER_CLIENT_CODE(pOrderDescriptor);

            var comment = TRANS2QUIK_ORDER_BROKERREF(pOrderDescriptor);
            var sStratName = comment;

            _t2QReceiver?.NewOrderStatus(dNumber, iDate, iTime, nMode, iActivationTime, iCancelTime, iExpireDate,
                sAcc, sStratName, classCode, sSecCode, iIsSell, iQty, nBalance, dPrice, iStatus, dwTransId, sClientCode);

            // var orderString = CreateOrderStr(nMode, dwTransId, dNumber, classCode, sSecCode,
            //    dPrice, nBalance, dValue, iIsSell, iStatus, pOrderDescriptor);
        }

        #endregion

        #endregion

        #region Trades Receiving ( Receiving Trades )

        #region subscribe_functions

        //  long __stdcall TRANS2QUIK_SUBSCRIBE_TRADES(
        //          LPSTR lpstrClassCode,
        //          LPSTR lpstrSeccodes)

        [DllImport(Trans2QuikDllName,
            // EntryPoint = "_TRANS2QUIK_SUBSCRIBE_TRADES@8",
            EntryPoint = "TRANS2QUIK_SUBSCRIBE_TRADES",
            CallingConvention = CallingConvention.StdCall)]
        private static extern long subscribe_trades(
            [MarshalAs(UnmanagedType.LPStr)] string class_code,
            [MarshalAs(UnmanagedType.LPStr)] string sec_code);

        [DllImport(Trans2QuikDllName,
            // EntryPoint = "_TRANS2QUIK_UNSUBSCRIBE_TRADES@0",
            EntryPoint = "TRANS2QUIK_UNSUBSCRIBE_TRADES",
            CallingConvention = CallingConvention.StdCall)] 
        private static extern long unsubscribe_trades();

        // [DllImport(Trans2QuikDllName, EntryPoint = "_TRANS2QUIK_START_TRADES@4",
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_START_TRADES",
            CallingConvention = CallingConvention.StdCall)]
        private static extern void start_trades(TradeStatusCallback pfnTradeStatusCallback);

        #endregion



        #endregion

        //           private readonly TradeStatusCallback _tradeStatusDefaultCallback;

        // 23.10.13
        //private void TradeStatusDefaultCallback(
        //    Int32 nMode, UInt64 dNumber, UInt64 dOrderNumber,
        //    string sClassCode, string sSecCode,
        //    Double dPrice, Int64 nQty, Double dValue, Int32 nIsSell,
        //    IntPtr nTradeDescriptor)
        //{
        //    var iDate = TRANS2QUIK_TRADE_DATE(nTradeDescriptor);
        //    var iTime = TRANS2QUIK_TRADE_TIME(nTradeDescriptor);


        //    var sAcc = TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor);
        //    var sStrat = TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor);
        //    var sClientCode = TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor);

        //    var dCommissionTs = TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor);

        //    var sTradeExchange = TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor);

        //    var tradeString = CreateTradeStr(nMode, dNumber, dOrderNumber, sClassCode, sSecCode,
        //        dPrice, nQty, dValue, nIsSell, nTradeDescriptor);

        //    _t2QReceiver?.NewTrade(dNumber, iDate, iTime, nMode,
        //        sAcc, sStrat, sClassCode, sSecCode, nIsSell, nQty, dPrice, sClientCode,
        //        dOrderNumber, dCommissionTs);
        //}
        // 23.10.13
        private long TradeStatusDefaultCallback(
            long nMode, ulong dNumber, ulong dOrderNumber,
            string sClassCode, string sSecCode,
            double dPrice, long nQty, double dValue, long nIsSell,
            IntPtr nTradeDescriptor)
        {
            var iDate = TRANS2QUIK_TRADE_DATE(nTradeDescriptor);
            var iTime = TRANS2QUIK_TRADE_TIME(nTradeDescriptor);


            var sAcc = TRANS2QUIK_TRADE_ACCOUNT(nTradeDescriptor);
            var sStrat = TRANS2QUIK_TRADE_BROKERREF(nTradeDescriptor);
            var sClientCode = TRANS2QUIK_TRADE_CLIENT_CODE(nTradeDescriptor);

            var dCommissionTs = TRANS2QUIK_TRADE_TS_COMMISSION(nTradeDescriptor);

            var sTradeExchange = TRANS2QUIK_TRADE_EXCHANGE_CODE(nTradeDescriptor);

            // var tradeString = CreateTradeStr(nMode, dNumber, dOrderNumber, sClassCode, sSecCode,
             //   dPrice, nQty, dValue, nIsSell, nTradeDescriptor);

            _t2QReceiver?.NewTrade(dNumber, iDate, iTime, nMode,
                sAcc, sStrat, sClassCode, sSecCode, nIsSell, nQty, dPrice, sClientCode,
                dOrderNumber, dCommissionTs);
            return 1;
        }

        // #endregion

    }


}