using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Interfaces;

namespace GS.Trade.TradeTerminals64.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public void TransactionReplyItemTracking(IEventArgs1 arg)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(arg.Object is TransactionReplyItem tri))
            {
                Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, Key,
                    "TransactionReplyItem is Null", m, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, tri.ShortInfo(), m,
                tri.ShortDescription(),tri.ToString());
        }
        public void OrderStatusChangedReplyTracking(IEventArgs1 arg)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(arg.Object is OrderStatusReply reply))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, Key,
                    "OrderStatusReply is Null", m, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, reply.ShortInfo(), m,
                        reply.ShortDescription(), reply.ToString());
        }
        public void TradeReplyTracking(IEventArgs1 arg)
        {
            var m = System.Reflection.MethodBase.GetCurrentMethod()?.Name + "()";

            if (!(arg.Object is NewTradeReply reply))
            {
                Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, Key,
                    "TradeReply is Null", m, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, FullName , reply.ShortInfo(), m,
                        reply.ShortDescription(), reply.ToString());
        }
    }
}
