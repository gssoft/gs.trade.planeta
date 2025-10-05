using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using GS.Containers;
using GS.Containers1;
using GS.Events;
using GS.Extension;
using sg_TradeTerminal02;
using GS.Collections;
using GS.DateTimeExt;
using GS.Events;
using GS.Extensions.DateTime;
using GS.Status;
using GS.Trade.TradeTerminals64.Quik;

namespace GS.Trade.TradeTerminals64
{
    public enum TransactionStatus { Registered = 1, TryToSend = 2, Sended = 3, Confirmed = 4,  Completed = 5, NotSended = -3}
    public enum TransactionResult { Success = 1,   Unknown = 0, Failure = -1,   Warning = -2 }

    public interface ITransaction : Containers5.IHaveKey<ulong>
    {
        DateTime DT { get; }
        DateTime RegisteredDT { get; set; }
        DateTime SendedDT { get; set; }
        DateTime CompletedDT { get; set; }
        TimeSpan ElapsedTime { get;  }

        TransactionResult Result { get; set; }
        TransactionStatus Status { get; set; }
        ErrorReason ErrorReason { get; set; }
        string Comment { get; set; }

        string ShortInfo { get; }
        string ShortDescription { get; }
    }

    public interface IQuikTransaction : ITransaction, IProcessingAction<IQuikTransaction>
    {
        ulong TransID { get; }

        IOrder3 Order { get; }

        QuikTransactionActionEnum Action { get; set; }

        string TradeTerminalKey { get; set; }
        string TransactionString { get;  }

        ulong OrderNumber { get; set; }

        T2QResults QuikResult { get; set; }
        string Message { get; set; }
        Int32 ExtendedErrorCode { get; set; }

        Int32 ReplyCode { get; set; }

        IStrategy Strategy { get; }
        string StrategyStr { get; }

        string StatusStr { get; }
        string ResultStr { get;}

        string QuikResultStr { get; }

        string DateTimeTimeStr { get; }
        string RegisteredTimeStr { get; }
        string SendedTimeStr { get ;}
        string CompletedTimeStr { get ;}
        string ElapsedTimeStr { get ; }

        TimeSpan CompletedTime { get; }

        // bool IsNotCompleted { get; }
        bool IsCompleted { get; }
        bool IsSended { get; }

        bool IsCancelOrderTransaction { get; }
        bool IsSetOrderTransaction { get; }
    }

    //public enum QuikTransactionResult
    //{
    //    Succsess = 1, WrongSyntax = -1, DllNotConnected = -2, QuikNotConnected = -3, Failed = -4 
    //}

    public class QuikTransactions : DictionaryCollection< ulong, IQuikTransaction>// Containers5.DictionaryContainer<long, IQuikTransaction>
    {
        public bool IsProcessTaskInUse { get; set; }

        public QuikTransactions()
        {
            Code = "QuikTransactions";
            Name = "QuikTransactions";

            Category = "Transactions";
            Entity = "Transaction";

            IsEnabled = true;
            IsEvlEnabled = true;
        }

        //UInt32 x = (uint) 4235959001;
        public void GetFireEvent(object sender, Events.IEventArgs args)
        {
            var t = args.Object as IQuikTransaction;
            if (t == null)
                return;
            switch (args.Operation.TrimUpper())
            {
                case "ADD":
                    Add(t);
                    break;
            }
        }
    }
    public class QuikTransaction : IQuikTransaction
    {
        private TransactionStatus _status;
        public ulong TransID { get; set; }

        public IOrder3 Order { get; set; }
        public ulong OrderNumber { get; set; }

        public QuikTransactionActionEnum Action { get; set; }

        public bool IsSetOrderTransaction => Action > 0;
        public bool IsCancelOrderTransaction => Action < 0;

        public string TradeTerminalKey { get; set; }

        public DateTime DT { get; set; }

        public DateTime RegisteredDT { get; set; }
        public DateTime SendedDT { get; set; }
        public DateTime CompletedDT { get; set; }

        public TransactionResult Result { get; set; }

        public TransactionStatus Status
        {
            get { return _status; }
            set
            {
                //if (_status == value) return;
                _status = value;
               // UpdateStatusDateTime(_status);
            }
        }
        public ErrorReason ErrorReason { get; set; }
        public string Comment { get; set; }
        public T2QResults QuikResult { get; set; }
        public string Message { get; set; }
        public Int32 ExtendedErrorCode { get; set; }
        public Int32 ReplyCode { get; set; }

        public string TransactionString { get; set;}

        public IStrategy Strategy { get; set; }

        public TimeSpan ElapsedTime => IsCompleted ? CompletedDT - SendedDT : TimeSpan.Zero;
        public ulong Key => TransID;

        // public bool IsNotCompleted => CompletedDT == DateTime.MinValue;
        // public bool IsNotCompleted => !IsCompleted;
        public bool IsCompleted => Status == TransactionStatus.Completed;
        public bool IsSended => Status == TransactionStatus.Sended;
        public string StrategyStr => Strategy != null ? Strategy.StrategyTimeIntTickerString : "Unknown";
        public string StatusStr => Status.ToString();
        public string ResultStr => Result.ToString();
        public string QuikResultStr => QuikResult.ToString();
        //public string SendedTimeStr { get { return SendedDT.ToString("T"); } }

        public string DateTimeTimeStr => DT.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
        public string RegisteredTimeStr => RegisteredDT.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
        public string SendedTimeStr => SendedDT.TimeOfDay.ToString(@"hh\:mm\:ss\.fff");
        public TimeSpan CompletedTime => IsCompleted ? CompletedDT.TimeOfDay : TimeSpan.Zero;
        public string CompletedTimeStr => CompletedTime.ToString(@"hh\:mm\:ss\.fff");
        public string ElapsedTimeStr => ElapsedTime.ToString(@"hh\:mm\:ss\.fff");

        public Action<IQuikTransaction> ProcessingAction { get; set; }
        //public QuikTransaction()
        //{
        //    DT = RegisteredDT = SendedDT = CompletedDT = DateTime.Now;
        //}
        private void UpdateStatusDateTime(TransactionStatus status)
        {
            switch (status)
            {
                case TransactionStatus.Registered:
                    RegisteredDT = DateTime.Now;
                    DT = RegisteredDT; 
                    break;
                case TransactionStatus.NotSended:
                case TransactionStatus.Sended:
                    SendedDT = DateTime.Now;
                    DT = SendedDT;
                    break;
                case TransactionStatus.Completed:
                    CompletedDT = DateTime.Now;
                    DT = CompletedDT;
                    break;
            }
        }

        public override string ToString()
        {
            return
                $"[{GetType()}; TransID: {TransID}; Action: {Action}; QResult: {QuikResult}; " +
                $"ExtError: {ExtendedErrorCode}; ReplyCode: {ReplyCode};" +
                $" TransString: {TransactionString}; " +
                $"Elapsed: {ElapsedTime}; Registered: {RegisteredDT} Sended: {SendedDT}; " +
                $"Completed: {CompletedDT}; Strat: {StrategyStr}]";
        }

        public string ShortInfo => $"{DT.DateTimeTickStrTodayCnd()} [{TransID}] {Action} {Status} {Result} {ErrorReason}";
        public string ShortDescription =>
            $"{DT.DateTimeTickStrTodayCnd()} [{TransID}] {Action} {Status} {QuikResultStr} {ExtendedErrorCode} {ReplyCode} {ErrorReason} {Message}";
    }
}
