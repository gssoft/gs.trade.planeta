using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.ConsoleAS;
using GS.Trade.TimeSeriesImage01.Models;
using TimeSeriesImageContext = GS.Trade.TimeSeriesImage01;

namespace CaTest01
{
    class Program
    {
        static void Main(string[] args)
        {
            var cntx = new GS.Trade.TimeSeriesImage01.Models.TimeSeriesImageContext();
            var tb2 = new TradeBoard { Code = "SPBFUT" };
            cntx.TradeBoards.Add(tb2);
            var t1 = new Ticker { Code = "RIM5", TradeBoardId = tb2.Id };
            cntx.Tickers.Add(t1);
            var ti1 = new TimeInt { Code = "5Sec", TimeInterval = 5, TimeShift = 0 };
            cntx.TimeInts.Add(ti1);
            var qpr1 = new QuoteProvider { Code = "Quik.Real" };
            cntx.QuoteProviders.Add(qpr1);
            var bytesseries = new BytesSeries
            {
                Code = "BytesSeries055",
                TickerId = t1.Id,
                TimeIntId = ti1.Id,
                QuoteProviderId = qpr1.Id
            };
            cntx.TimeSeries.Add(bytesseries);
            cntx.SaveChanges();

            var strb = new StringBuilder();
            foreach (var s in Enumerable.Range(1,150000).Select(i => Guid.NewGuid()))
            {
                strb.Append(s);
            }
            var str = strb.ToString();
            var bytes = Encoding.UTF8.GetBytes(str);
            ConsoleSync.WriteReadLineT($"BytesCount: {bytes.Count()}");

            var bytesseriesitem = new BytesSeriesItem
            {
                DT = DateTime.Now,
                Format = "CSV",
                Count = 1000,
                CheckSum = Guid.NewGuid().ToString(),
                Bytes = bytes
            };
            bytesseries.Items.Add(bytesseriesitem);

            cntx.SaveChanges();

            var maxid = cntx.BytesSeries.Max(s => s.Id);
            var item = cntx.BytesSeries.FirstOrDefault(s=>s.Id == maxid);
            var bs = item.Bytes;
            ConsoleSync.WriteReadLineT($"Read Bytes OK! BytesCount: {bytes.Count()}");
            var j = 0;
            foreach (var b in bytes)
            {
                if (b == bs[j++]) continue;
                ConsoleSync.WriteReadLineT($"Bytes non Equlal: {b} {bs[--j]}");
                break;
            }
            ConsoleSync.WriteReadLineT("Press any key");
        }
    }
}
