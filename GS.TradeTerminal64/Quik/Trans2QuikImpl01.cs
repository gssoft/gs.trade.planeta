using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace GS.Trade.TradeTerminals64.Quik
{
    internal partial class Trans2Quik01
    {
        #region Transaction, Order, Trade CallBacks
        #region TransactionReplyCallBackImpl
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

        #region OrderReplyCallBackImpl
        private static void order_status_callback_impl(
                    Int32 nMode, UInt32 dwTransID, Double dNumber, string classCode, string secCode,
                    Double dPrice, Int32 nBalance, Double dValue, Int32 nIsSell, Int32 nStatus, IntPtr pOrderDescriptor)
        {
            String mainString = "Mode=" + nMode + " TransId=" + dwTransID + " Num=" + dNumber +
                 " Class=" + classCode + " Sec=" + secCode + " Price=" + dPrice +
                 " Balance=" + nBalance + " Value=" + dValue + " IsSell=" + nIsSell + " Status=" + nStatus;
            String addString = "";
            String strString = "";

            addString = " Qty=" + TRANS2QUIK_ORDER_QTY(pOrderDescriptor) +
                " Date=" + TRANS2QUIK_ORDER_DATE(pOrderDescriptor) +
                " Time=" + TRANS2QUIK_ORDER_TIME(pOrderDescriptor) +
                " ActTime=" + TRANS2QUIK_ORDER_ACTIVATION_TIME(pOrderDescriptor) +
                " WDTime=" + TRANS2QUIK_ORDER_WITHDRAW_TIME(pOrderDescriptor) +
                " Expiry=" + TRANS2QUIK_ORDER_EXPIRY(pOrderDescriptor) +
                " Accruedint=" + TRANS2QUIK_ORDER_ACCRUED_INT(pOrderDescriptor) +
                " Yield=" + TRANS2QUIK_ORDER_YIELD(pOrderDescriptor) +
                " UID=" + TRANS2QUIK_ORDER_UID(pOrderDescriptor);
            try
            {
                strString = ""
                      + " USERID=" + TRANS2QUIK_ORDER_USERID(pOrderDescriptor)
                      + " Account=" + TRANS2QUIK_ORDER_ACCOUNT(pOrderDescriptor)
                      + " Brokerref=" + TRANS2QUIK_ORDER_BROKERREF(pOrderDescriptor)
                      + " ClientCode=" + TRANS2QUIK_ORDER_CLIENT_CODE(pOrderDescriptor)
                      + " Firmid=" + TRANS2QUIK_ORDER_FIRMID(pOrderDescriptor)
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
        #endregion
        #region Create Trade, Orders String
        public string CreateOrderStr(Int32 nMode, UInt32 dwTransId, Double dNumber,
            string classCode, string secCode,
            Double dPrice, Int64 nBalance, Double dValue,
            Int32 nIsSell, Int32 nStatus,
            IntPtr pOrderDescriptor)
        {
            var sb = new StringBuilder();
            sb.Append("dwTransId: ");
            sb.Append(dwTransId);
            sb.Append(" dNumber: ");
            sb.Append(dNumber);
            sb.Append(" OrderDate: ");
            sb.Append(TRANS2QUIK_ORDER_DATE(pOrderDescriptor));
            sb.Append(" OrderTime: ");
            sb.Append(TRANS2QUIK_ORDER_TIME(pOrderDescriptor));
            sb.Append(" SecCode: ");
            sb.Append(secCode);
            sb.Append(" IsSell: ");
            sb.Append(nIsSell);
            sb.Append(" dPrice: ");
            sb.Append(dPrice);
            sb.Append(" Qty: ");
            sb.Append(TRANS2QUIK_ORDER_QTY(pOrderDescriptor));
            sb.Append(" Status: ");
            sb.Append(nStatus);
            sb.Append(" Account: ");
            sb.Append(TRANS2QUIK_ORDER_ACCOUNT(pOrderDescriptor));
            sb.Append("BrokerStr: ");
            sb.Append(TRANS2QUIK_ORDER_BROKERREF(pOrderDescriptor));
            sb.Append(";");

            return sb.ToString();
        }
        public string CreateTradeStr(int nMode, double dNumber, double dOrderNumber, string classCode, string secCode, double dPrice,
            int nQty, double dValue, int nIsSell, int nTradeDescriptor)
        {
            throw new NotImplementedException();
        }

        public string CreateOrderStr(int nMode, uint dwTransId, double dNumber, string classCode, string secCode, double dPrice,
            int nBalance, double dValue, int nIsSell, int nStatus, int nOrderDescriptor)
        {
            throw new NotImplementedException();
        }
        public string CreateTradeStr(Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
            Double dPrice, Int64 nQty, Double dValue, Int32 nIsSell, IntPtr nTradeDescriptor)
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

        #endregion

        private static void trade_status_callback_impl(
        Int32 nMode, Double dNumber, Double dOrderNumber, string classCode, string secCode,
        Double dPrice, Int32 nQty, Double dValue, Int32 nIsSell, IntPtr nTradeDescriptor)
        {
            String mainString = "Mode=" + nMode + " TradeNum=" + dNumber + " OrderNum=" + dOrderNumber +
                 " Class=" + classCode + " Sec=" + secCode +
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
                using (System.IO.StreamWriter file = new System.IO.StreamWriter
                           (@"trades.log", true, System.Text.Encoding.GetEncoding(1251)))
                {
                    file.WriteLine(mainString + addString + strString);
                    file.Close();
                    Console.WriteLine("TRADE_CALLBACK:\n" + mainString + addString + strString);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
