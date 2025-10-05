using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Events;
using GS.Interfaces;

namespace GS.Trade.TradeTerminals.Quik
{
    public sealed partial class QuikTradeTerminal
    {
        public void TransactionReplyItemTracking(IEventArgs1 arg)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            var tri = arg.Object as TransactionReplyItem;
            if (tri == null)
            {
                Evlm2(EvlResult.FATAL, EvlSubject.DIAGNOSTIC, Key,
                    "TransactionReplyItem is Null", methodname, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, tri.ShortInfo(), methodname,
                tri.ShortDescription(),tri.ToString());
        }
        public void OrderStatusChangedReplyTracking(IEventArgs1 arg)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            var reply = arg.Object as OrderStatusReply;
            if (reply == null)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, Key,
                    "OrderReply is Null", methodname, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, reply.ShortInfo(), methodname,
                        reply.ShortDescription(), reply.ToString());
        }
        public void TradeReplyTracking(IEventArgs1 arg)
        {
            var methodname = System.Reflection.MethodBase.GetCurrentMethod().Name + "()";

            var reply = arg.Object as NewTradeReply;
            if (reply == null)
            {
                Evlm2(EvlResult.WARNING, EvlSubject.DIAGNOSTIC, Key,
                    "TradeReply is Null", methodname, $"{arg.Entity}, {arg.Operation}", arg.OperationKey);
                return;
            }
            Evlm2(EvlResult.INFO, EvlSubject.DIAGNOSTIC, FullName , reply.ShortInfo(), methodname,
                        reply.ShortDescription(), reply.ToString());
        }
    }
}
