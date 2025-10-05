using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace GS.Trade.TradeTerminals64.Quik
{
    internal partial class Trans2Quik01
    {
        #region trade_descriptor_functions

        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_DATE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern long TRANS2QUIK_TRADE_DATE(IntPtr nTradeDescriptor); // Trade Date
        // ----------------------------------------------------------------------------------------                                                                          
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_DATE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern long TRANS2QUIK_TRADE_SETTLE_DATE(IntPtr nTradeDescriptor); // Settle Date
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_TIME",
            CallingConvention = CallingConvention.StdCall)]
        public static extern long TRANS2QUIK_TRADE_TIME(IntPtr nTradeDescriptor); // Trade Time
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_IS_MARGINAL",
            CallingConvention = CallingConvention.StdCall)]
        public static extern long TRANS2QUIK_TRADE_IS_MARGINAL(IntPtr nTradeDescriptor); // IsMarginal
        // ----------------------------------------------------------------------------------------

        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_ACCRUED_INT", 
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_ACCRUED_INT(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_YIELD", 
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_YIELD(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_TS_COMMISSION",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_TS_COMMISSION(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_CLEARING_CENTER_COMMISSION(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_EXCHANGE_COMMISSION",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_EXCHANGE_COMMISSION(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_TRADING_SYSTEM_COMMISSION(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_PRICE2",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_PRICE2(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_REPO_RATE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_REPO_RATE(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_REPO_VALUE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_REPO_VALUE(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_REPO2_VALUE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_REPO2_VALUE(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_ACCRUED_INT2",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_ACCRUED_INT2(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_REPO_TERM",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_TRADE_REPO_TERM(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_START_DISCOUNT",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_START_DISCOUNT(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_LOWER_DISCOUNT",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_LOWER_DISCOUNT(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_UPPER_DISCOUNT",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_TRADE_UPPER_DISCOUNT(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_BLOCK_SECURITIES",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_TRADE_BLOCK_SECURITIES(IntPtr nTradeDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_CURRENCY",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_CURRENCY_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_CURRENCY(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CURRENCY_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------

        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_CURRENCY",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(IntPtr nTradeDescriptor);

        public static string TRANS2QUIK_TRADE_SETTLE_CURRENCY(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CURRENCY_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_SETTLE_CODE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(IntPtr nTradeDescriptor);

        public static string TRANS2QUIK_TRADE_SETTLE_CODE(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_SETTLE_CODE_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_ACCOUNT",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_ACCOUNT_IMPL(IntPtr nTradeDescriptor);

        public static string TRANS2QUIK_TRADE_ACCOUNT(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_ACCOUNT_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_BROKERREF",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_BROKERREF_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_BROKERREF(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_BROKERREF_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_CLIENT_CODE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_CLIENT_CODE(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_CLIENT_CODE_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_USERID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_USERID_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_USERID(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_USERID_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_FIRMID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_FIRMID_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_FIRMID(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_FIRMID_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_PARTNER_FIRMID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_PARTNER_FIRMID(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_PARTNER_FIRMID_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_EXCHANGE_CODE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_EXCHANGE_CODE(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_EXCHANGE_CODE_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_TRADE_STATION_ID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_TRADE_STATION_ID_IMPL(IntPtr nTradeDescriptor);
        public static string TRANS2QUIK_TRADE_STATION_ID(IntPtr nTradeDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_TRADE_STATION_ID_IMPL(nTradeDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        #endregion

        #region order_descriptor_functions
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_QTY",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int64 TRANS2QUIK_ORDER_QTY(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_DATE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_ORDER_DATE(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_TIME",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_ORDER_TIME(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_ACTIVATION_TIME",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_ORDER_ACTIVATION_TIME(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_WITHDRAW_TIME",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_ORDER_WITHDRAW_TIME(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_EXPIRY",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_ORDER_EXPIRY(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_ACCRUED_INT",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_ORDER_ACCRUED_INT(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_YIELD",
            CallingConvention = CallingConvention.StdCall)]
        public static extern double TRANS2QUIK_ORDER_YIELD(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_UID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern Int32 TRANS2QUIK_ORDER_UID(IntPtr pOrderDescriptor);
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_USERID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_ORDER_USERID_IMPL(IntPtr pOrderDescriptor);
        public static string TRANS2QUIK_ORDER_USERID(IntPtr pOrderDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_USERID_IMPL(pOrderDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_ACCOUNT",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_ORDER_ACCOUNT_IMPL(IntPtr pOrderDescriptor);
        public static string TRANS2QUIK_ORDER_ACCOUNT(IntPtr pOrderDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_ACCOUNT_IMPL(pOrderDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_BROKERREF",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_ORDER_BROKERREF_IMPL(IntPtr pOrderDescriptor);
        public static string TRANS2QUIK_ORDER_BROKERREF(IntPtr pOrderDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_BROKERREF_IMPL(pOrderDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_CLIENT_CODE",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(IntPtr pOrderDescriptor);
        public static string TRANS2QUIK_ORDER_CLIENT_CODE(IntPtr pOrderDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_CLIENT_CODE_IMPL(pOrderDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        [DllImport(Trans2QuikDllName, EntryPoint = "TRANS2QUIK_ORDER_FIRMID",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr TRANS2QUIK_ORDER_FIRMID_IMPL(IntPtr pOrderDescriptor);
        public static string TRANS2QUIK_ORDER_FIRMID(IntPtr pOrderDescriptor)
        {
            return Marshal.PtrToStringAnsi(TRANS2QUIK_ORDER_FIRMID_IMPL(pOrderDescriptor));
        }
        // ----------------------------------------------------------------------------------------
        #endregion
    }
}
