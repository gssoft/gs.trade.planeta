using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.Trades
{
    public class SimulateOrders : Orders3.Orders3, ISimulateOrders
    {
        public List<IOrder3> FillerCollection;
        private readonly object _fillerLocker;

        public BackTestOrderExecutionMode BackOrderExecMode { get; set; }

        public SimulateOrders()
        {
            FillerCollection = new List<IOrder3>();
            _fillerLocker = new object();

            BackOrderExecMode = BackTestOrderExecutionMode.Optimistic;
        }

        public void ExecuteTick(DateTime dt, string tickerkey, double price, double bid, double ask)
        {
        }
    }
}
