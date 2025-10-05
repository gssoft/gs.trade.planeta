using System;
using System.Runtime.InteropServices;

namespace GS.Trade.TradeTerminals64.Quik
{
    public interface ITrans2QuikReceiver
    {
        // x86
        void ConnectionStatusChanged(Int32 nConnectionEvent, Int32 nExtendedErrorCode, byte[] message);
        // x64
        void ConnectionStatusChanged(long nConnectionEvent, long nExtendedErrorCode, byte[] message);

        void TransactionReply(int nTransactionResult,
                              int nTransactionExtendedErrorCode,
                              int nTransactionReplyCode,
                              uint dwTransId,
                              double dOrderNum,
                              [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage);
        void TransactionReply64(long nTransactionResult,
                                long nTransactionExtendedErrorCode,
                                long nTransactionReplyCode,
                                ulong dwTransId, // DWORD dwTransID
                                ulong dOrderNum, // uint64 dOrderNum
                                [MarshalAs(UnmanagedType.LPStr)] string transactionReplyMessage);
        // 210808
        // nBalance from Int32 to Int64
        //void NewOrderStatus(double dNumber,
        //                    Int32 iDate, Int32 iTime,
        //                    Int32 iMode,
        //                    Int32 iActivationTime, Int32 iCancelTime, Int32 iExpireDate,
        //                    string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
        //                    Int32 iIsSell, long iQty, Int64 nBalance, double dPrice,
        //                    Int32 iStatus, UInt32 dwTransId,
        //                    string sClientCode);

        // x86
        void NewOrderStatus(double dNumber,
            Int32 iDate, Int32 iTime,
            Int32 iMode,
            Int32 iActivationTime, Int32 iCancelTime, Int32 iExpireDate,
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            Int32 iIsSell, long iQty, Int64 nBalance, double dPrice,
            Int32 iStatus, UInt32 dwTransId,
            string sClientCode);

        // x64
        void NewOrderStatus(ulong dNumber,
            long iDate, long iTime,
            long iMode,
            long iActivationTime, long iCancelTime, long iExpireDate,
            string sAcc, string sBrokerRef, string sClassCode, string sSecCode,
            long iIsSell, long iQty, long nBalance, double dPrice,
            long iStatus, ulong dwTransId,
            string sClientCode);

        void NewTrade(double dNumber,
                      long iDate, long iTime, long iMode,
                      string sAcc, string sStrat, string sClassCode, string sSecCode,
                      long nIsSell, long iQty, double dPrice,
                      string sClientCode,
                      double dOrderNumber, double dCommissionTs);
    }
}