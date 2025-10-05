using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Extensions.DateTime;
using GS.Interfaces;
using GS.Trade.Trades.Trades3;
using EventArgs = GS.Events.EventArgs;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public class NewTradeReply
        {
            public double TradeNumber { get; set; }
            public int Date { get; set; }
            public int Time { get; set; }
            public int Mode { get; set; }
            public string Account { get; set; }
            public string Strategy { get; set; }
            public string ClassCode { get; set; }
            public string Ticker { get; set; }
            public TradeOperationEnum Side { get; set; }
            // 210807 from int32
            public long Qty { get; set; }
            public double Price { get; set; }
            public string ClientCode { get; set; }
            public double OrderNumber { get; set; }
            public double ComissionTs { get; set; }
            public DateTime ReplyDateTime { get; set; }

            public override string ToString()
            {
                return $"Trade:[{TradeNumber}] Ord:[{OrderNumber}] DT:{Date} {Time} Mode:{Mode} Acc:{Account}" +
                       $" Tkr:{ClassCode}@{Ticker} Side:{Side} Qty:{Qty} Prc:{Price}" +
                       $" ClnCd:{ClientCode} CmsTs:{ComissionTs} RplDt:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
            public string ShortInfo()
            {
                return $"Trade:[{TradeNumber}] DT:{Date} {Time}" +
                       $" {ClassCode}@{Ticker} Side:{Side} Qty:{Qty} Prc:{Price}";
                // $" RplDt:{ReplyDateTime.ToString("g")}";
            }
            public string ShortDescription()
            {
                return $"Trade:[{TradeNumber}] Ord:[{OrderNumber}] DT:{Date} {Time} Acc:{Account}" +
                       $" {ClassCode}@{Ticker} Side:{Side} Qty:{Qty} Prc:{Price}" +
                       $" Rpl:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
        }
        public class NewTradeReply64
        {
            public double TradeNumber { get; set; }
            public long Date { get; set; }
            public long Time { get; set; }
            public long Mode { get; set; }
            public string Account { get; set; }
            public string Strategy { get; set; }
            public string ClassCode { get; set; }
            public string Ticker { get; set; }
            public TradeOperationEnum Side { get; set; }
            // 210807 from int32
            public long Qty { get; set; }
            public double Price { get; set; }
            public string ClientCode { get; set; }
            public double OrderNumber { get; set; }
            public double ComissionTs { get; set; }
            public DateTime ReplyDateTime { get; set; }

            public override string ToString()
            {
                return $"Trade:[{TradeNumber}] Ord:[{OrderNumber}] DT:{Date} {Time} Mode:{Mode} Acc:{Account}" +
                       $" Tkr:{ClassCode}@{Ticker} Side:{Side} Qty:{Qty} Prc:{Price}" +
                       $" ClnCd:{ClientCode} CmsTs:{ComissionTs} RplDt:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
            public string ShortInfo()
            {
                return $"Trade:[{TradeNumber}] DT:{Date} {Time}" +
                       $" {ClassCode}@{Ticker} Side:{Side} Qty:{Qty} Prc:{Price}";
                // $" RplDt:{ReplyDateTime.ToString("g")}";
            }
            public string ShortDescription()
            {
                return $"Trade:[{TradeNumber}] Ord:[{OrderNumber}] DT:{Date} {Time} Acc:{Account}" +
                       $" {ClassCode}@{Ticker} Side:{Side} Qty:{Qty} Prc:{Price}" +
                       $" Rpl:{ReplyDateTime.DateTimeTickStrTodayCnd()}";
            }
        }

        private static TradeOperationEnum ConvertSideToEnum(long isSell)
        {
            switch (isSell)
            {
                case 0: return TradeOperationEnum.Buy;
                case 1: return TradeOperationEnum.Sell;
                default: return TradeOperationEnum.Unknown;
            }
        }
        private void SendTradeReplyEventArgs(
            NewTradeReply tradeReply, Action<IEventArgs1> action)
        {
            var eargs = new EventArgs1
            {
                Process = "NewTradeProcess",
                Category = "Trades",
                Entity = "NewTrade",
                Operation = "AddOrUpdate",
                Object = tradeReply,
                ProcessingAction = action
            };
            if (ProcessTask != null)
                ProcessTask.EnQueue(eargs);
            else
                action(eargs);
        }
        private void SendTradeReplyEventArgs(
            NewTradeReply64 tradeReply, Action<IEventArgs1> action)
        {
            var eargs = new EventArgs1
            {
                Process = "NewTradeProcess",
                Category = "Trades",
                Entity = "NewTrade",
                Operation = "AddOrUpdate",
                Object = tradeReply,
                ProcessingAction = action
            };
            if (ProcessTask != null)
                ProcessTask.EnQueue(eargs);
            else
                action(eargs);
        }
        public void NewTrade(double dNumber, long iDate, long iTime, long iMode,
            string sAcc, string sStrat, string sClassCode, string sSecCode,
            long nIsSell, long iQty, double dPrice,
            string sClientCode, double dOrderNumber, double dCommissionTs)
        {
            if (IsProcessTaskInUse)
            {
                var tradeReply = new NewTradeReply64
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
              
                SendTradeReplyEventArgs(tradeReply, TradeReplyTracking);
                SendTradeReplyEventArgs(tradeReply, TradeReplyProcessing);
                return;
            }
            // Old version of Trade Process
            // IF PRocessTask does not use
           // TradeProcess(dNumber, iDate, iTime, iMode,
           //     sAcc, sStrat, sClassCode, sSecCode,
           //     nIsSell, iQty, dPrice,
           //     sClientCode, dOrderNumber, dCommissionTs);
        }
        // NewTradeReple Old name
        public void TradeReplyProcessing(IEventArgs1 args)
        //double dNumber, int iDate, int iTime, int nMode,
        //string account, string strategy, string classCode, string ticker,
        //int iIsSell, int quantity, double price, string comment, double orderNumber,
        //double commissionTs)
        {
            var method = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            //var tradeReply = args.Object as NewTradeReply64;
            //if (tradeReply == null)
            //{
            //    Evlm2(EvlResult.FATAL, EvlSubject.TRADING, Code, "TradeReply",
            //            method, $"Args is not {nameof(NewTradeReply)}", "Sorry. See you Later");
            //    return;
            //}
            if (!(args.Object is NewTradeReply64 tradeReply))
            {
                Evlm2(EvlResult.FATAL, EvlSubject.TRADING, Code, "TradeReply",
                    method, $"Args is not {nameof(NewTradeReply)}", "Sorry. See you Later");
                return;
            }

            var dNumber = tradeReply.TradeNumber;
            var number = Convert.ToUInt64(dNumber);
            var account = tradeReply.Account;

            if (!_checkMaxTradeNumber.SetIfGreaterThanMe(account, number))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, Code, tradeReply.ShortInfo(),
                    $"{method} TradeNumber:[{number}] IsLessOrEquals Than Max",
                    tradeReply.ShortDescription(), tradeReply.ToString());
                return;
            }
            //var dNumber = tradeReply.TradeNumber;
            var iDate = tradeReply.Date;
            var iTime = tradeReply.Time;
            var nMode = tradeReply.Mode;
            // var account = tradeReply.Account;
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

            if (classCode == "FUTEVN")
            {
                Evlm2(EvlResult.WARNING, EvlSubject.TRADING, DllNamePath2QuikPair, t.ShortInfo,
                    $"{method} EveningSession Trade Ignore",
                    t.ShortDescription, t.ToString());
                return;
            }
            Evlm2(EvlResult.SUCCESS, EvlSubject.TRADING, DllNamePath2QuikPair,  t.ShortInfo, method,
                    t.ShortDescription, t.ToString());

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
                    // t.Strategy.NewTrade(t);
                    FireChangedEventToStrategy(t.Strategy,
                        t.Strategy.StrategyTimeIntTickerString, "TradeReply", "New", t);
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
                // Try to Resolve Later
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
    }
}