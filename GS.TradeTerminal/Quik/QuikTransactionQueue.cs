using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Queues;

namespace GS.Trade.TradeTerminals.Quik
{
    public class QuikTransactionQueue : QueueFifo<IQuikTransaction>
    {
    }
}
