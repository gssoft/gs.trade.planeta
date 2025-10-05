using System;
using System.Runtime.InteropServices;

namespace GS.Trade.TradeTerminals.Quik
{
    public interface ITrans2QuikReceiver
    {
        void ConnectionStatusChanged(Int32 nConnectionEvent, Int32 nExtendedErrorCode, byte[] message);

        void TransactionReply(Int32 nTransactionResult,
                              Int32 nTransactionExtendedErrorCode,
                              Int32 nTransactionReplyCode,
                              UInt32 dwTransId,
                              double dOrderNum,
                              [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage);

        void NewOrderStatus(double dNumber,
                            Int32 iDate, Int32 iTime,
                            Int32 iMode,
                            Int32 iActivationTime, Int32 iCancelTime, Int32 iExpireDate,
                            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
                            Int32 iIsSell, Int32 iQty, Int32 nBalance, double dPrice,
                            Int32 iStatus, UInt32 dwTransId,
                            string sClientCode);

        void NewTrade(double dNumber,
                      Int32 iDate, Int32 iTime, Int32 iMode,
                      string sAcc, string sStrat, string sClassCode, string sSecCode,
                      Int32 nIsSell, Int32 iQty, double dPrice,
                      string sClientCode,
                      double dOrderNumber, double dCommissionTs);
    }
}