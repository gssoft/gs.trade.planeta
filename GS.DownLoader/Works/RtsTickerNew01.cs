using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.DB.TickModel.FT1203;


namespace GS.Trade.QuoteDownLoader.Works
{
    public class RtsTickerNew01 : IWorkItem
    {
        public string Name { get; set; }
        public bool IsEnabled { get; set; }
        public string RtsTickersUri { get; set; }
        [XmlIgnore]
        public WorkContainer Works { get; set; }

        //private const string FileName = "Tickers_RTS01.txt";
        private readonly Dictionary<string, TickerStore> _tickers = new Dictionary<string, TickerStore>();

        public void DoWork()
        {
            SymbolAdd();
            foreach (var t in _tickers.Values)
            {
                Console.WriteLine(t);
            }
          //  TickersToDB();
        }
        private void AddTicker(TickerStore t)
        {
            if (!_tickers.ContainsKey(t.LongCode)) _tickers.Add(t.LongCode, t);
        }
        private void SymbolAdd()
        {
            if( !File.Exists(RtsTickersUri))
            {
                Works.EvlMessage(EvlResult.FATAL, String.Format("File {0} is not Exist", RtsTickersUri), "");
                return;
            }
            _tickers.Clear();
            var dlmt = new char[] { '\t' };
            try
            {
                using (var sr = new StreamReader(RtsTickersUri, Encoding.Default))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var s = line.Split(dlmt);
                        var l = s.Length;
                        // Console.WriteLine(line);
                        if (l <= 5)
                        {
                            if( string.IsNullOrWhiteSpace(line.Trim()) ) continue;
                            Works.EvlMessage(EvlResult.FATAL, "Parse line", line);
                            continue;
                        }
                        var code = s[0];
                        var longcode = s[1];
                        var lastdayStr = s[l - 3];
                        var expdayStr = s[l - 2];
                        var name = s[2];

                        DateTime lastday, expday;
                        if (!DateTime.TryParse(lastdayStr, out lastday) || !DateTime.TryParse(expdayStr, out expday))
                        {
                            Works.EvlMessage( EvlResult.FATAL, "Parse Ticker Dates", line) ;
                            continue;
                        }
                        const char delimiter = '_';
                        var keycode = longcode.Contains("-")
                                ? longcode.Substring(0, longcode.IndexOf('-')) + delimiter + expday.ToString("yyyy-MM-dd")
                                : longcode + delimiter + expday.ToString("yyyy-MM-dd");
                        var ti = new TickerStore
                        {
                            Code = code,
                            LongCode = longcode,
                            KeyCode = keycode,
                            D2 = lastday,
                            D3 = expday,
                            Description = name,
                            SymbolID = 1
                        };
                        AddTicker(ti);
                    }
                }
            }
            catch (Exception e)
            {
                Works.EvlMessage(EvlResult.FATAL, "GetTickers from: " + RtsTickersUri, e.Message);
            }
        }
        private void TickersToDB()
        {
            using (var context = new M1203())
            {
                try
                {
                    foreach (var t in _tickers.Values)
                    {
                        Console.WriteLine(t.ToString());
                        context.TickerStores.Add(t);
                    }
                    context.SaveChanges();

                }
                catch (Exception e)
                {
                    Console.Write(e.Message);
                }

            }
        }
    }
}
