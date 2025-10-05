using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade
{
    #region IReceivers Interfaces

    public interface IReceiver { }

    public interface IConnectionStatusReceiver : IReceiver
    {
        //void EventLogItemAdd(string trans);
        void NewConnectionStatus(string newconstatus);
    }
    public interface ITransactionReplyReceiver : IReceiver
    {
        //void EventLogItemAdd(string trans);
        void TransactionReply(int result, UInt32 transId, long orderNumber);
        void NewOrder(ulong transId, long orderNumber);
        void NewTransaction(string trans);
    }
    public interface IOrderStatusReceiver : IReceiver
    {
        //void EventLogItemAdd(string trans);
        void TransactionReply(int result, UInt32 transId, ulong orderNumber);

        void RegisterOrder(ulong number, DateTime dt, ulong transId,
                                  string account, string strategy, string ticker,
                                  OrderOperationEnum operation, OrderTypeEnum ordertype,
                                  double stopprice, double limitprice, long quantity, long rest,
                                  string comment);

        void NewOrderStatus(string order);
        void NewOrderStatus(double dNumber, int iDate, int iTime,
                                string sAcc, string sStrat, string sSecCode,
                                int iIsSell, int iQty, int iRest, double dPrice, int iStatus, uint dwTransId);

        void NewOrderStatus(double dNumber, int iDate, int iTime, int nMode,
                                int iActivationTime, int iCancelTime, int iExpireDate,
                                string sAcc, string sStrat, string sClassCode, string sSecCode,
                                int iIsSell, int iQty, int iRest, double dPrice, int iStatus, uint dwTransId, string comment);
    }
    public interface ITradeStatusReceiver : IReceiver
    {
        //void EventLogItemAdd(string trans);
        void NewTrade(string trade);
        void NewTrade(double dNumber, int iDate, int iTime, int nMode,
                                string sAcc, string sStrat, string sClassCode, string sSecCode,
                                int iIsSell, int iQty, double dPrice, string sComment, double orderNumber,
                                double dCommissionTs);
    }
    #endregion
}
