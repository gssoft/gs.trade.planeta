using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.EventLog;
using GS.Interfaces;
using GS.Trade.TradeContext;

namespace Test_TradeContext
{
    class Program
    {
        private static TradeContext _tx;
        private static IEventLog _evl;
        static void Main(string[] args)
        {
            _tx = new TradeContext();
            _tx.Init();

            _evl = new ConsoleEventLog();
            _evl.AddItem(EvlResult.SUCCESS,"TradeContext Init", "Good Job");
            Console.ReadLine();

            foreach(var t in _tx.TickerCollection)
                Console.WriteLine(t.ToString());
            Console.ReadLine();

            foreach (var s in _tx.StrategyCollection)
                Console.WriteLine(s.ToString());
            Console.ReadLine();

            Console.WriteLine("Trade Terminals");
            foreach (var tt in _tx.TradeTerminals.TradeTerminalCollection.Values)
            {
                Console.WriteLine(tt.ToString());
                var connect = tt.Connect();
            }
            Console.ReadLine();
            
            Console.WriteLine("Orders=" + _tx.Orders.OrderCollection.Count);
            Console.WriteLine("Trades=" + _tx.Trades.TradeDictionary.Count);
            Console.ReadLine();

            _tx.Start();
            Console.ReadLine();
            _tx.Stop();
            Console.ReadLine();

        }
    }
}
