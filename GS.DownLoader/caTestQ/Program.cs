using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using GS.Trade.DB.Q;
using GS.Trade.QuoteDownLoader;
using SG.Trade.EventLog;
using Bar = GS.Trade.DB.Q.Bar;
using Ticker = GS.Trade.DB.Q.Ticker;

namespace caTestQ
{
    class Program
    {
        private static Bar _lastBar;
        private static Ticker _ticker;
        private static Series _series;

        private static readonly List<Bar> DownLoadBars = new List<Bar>();

        private static readonly Dictionary<string, SeriesDef>  SeriesToDownLoad = new Dictionary<string, SeriesDef>();

        static void MyConsole(string ticker, string timeint, DateTime dt,
                                            double open, double high, double low, double close,
                                            decimal volume)
        {
            Console.WriteLine("{0} {1} {2} {3} {4} {5} {6} {7}",
                ticker, timeint, dt, open, high, low, close, volume);
        }
        static void SaveBar (string ticker, string timeint, DateTime dt,
                                            double open, double high, double low, double close,
                                            decimal volume)
        {
            var bar = new Bar
                          {
                              SeriesID = _series.ID,
                              DT = dt,
                              Open = open,
                              High = high,
                              Low = low,
                              Close = close,
                              Volume = volume
                          };
            DownLoadBars.Add(bar);
        }
        private static int FillSeriesToDownLoad()
        {
            var xE = XDocument.Load("SeriesToDownLoad.xml");
            IEnumerable<XElement> xSeries = xE.Descendants("Series");
            foreach (var xs in xSeries)
            {
                if( xs == null) continue;
                if ( xs.Attribute("Ticker") == null || xs.Attribute("TimeInt") == null || xs.Attribute("FirstDate") == null) continue;

                var tname = xs.Attribute("Ticker").Value.Trim().ToUpper();
                var timeint = xs.Attribute("TimeInt").Value.Trim().ToUpper();
                var firstDateStr = xs.Attribute("FirstDate").Value.Trim();

                DateTime firstDate;
                if (!DateTime.TryParse(firstDateStr, out firstDate)) continue;

                var s = new SeriesDef {Ticker = tname, TimeInt = timeint, FirstDateTime = firstDate};
                SeriesDef series;
                if (SeriesToDownLoad.TryGetValue(s.Key, out series)) continue;

                SeriesToDownLoad.Add(s.Key,s);
            }

            return SeriesToDownLoad.Count;
        }

        static void Main(string[] args)
        {
            var consoleLog = new ConsoleEventLog();
            var fileLog = new FileEventLog("MyDownLoad.log");

            consoleLog.AddItem(EnumEventLog.SUCCESS, "DownLoad Bars", "Start");
            fileLog.AddItem(EnumEventLog.SUCCESS, "DownLoad Bars", "Start");

            var cnt = FillSeriesToDownLoad();
            if( cnt <= 0) return;

            var fin = new FinamDownLoader();
            fin.SaveBarItem += SaveBar;

            var q = new QuoteDataBase();

            foreach (var sd in SeriesToDownLoad.Values)
            {
                DateTime fromDateTime;
                DateTime toDateTime;
               
                var ret = q.GetSeries(sd.Ticker, sd.TimeInt, out _ticker, out _series);
                if( ret < 0)
                {
                    if (ret == -1)
                    {
                        consoleLog.AddItem(EnumEventLog.FATAL, "LookUp Ticker in DataBase",
                            String.Format("TickerName={0} not Found in the DataBase",sd.Ticker)  );
                        fileLog.AddItem(EnumEventLog.FATAL, "LookUp Ticker in DataBase",
                            String.Format("TickerName={0} not Found in the DataBase", sd.Ticker));
                    }
                    if (ret == -2)
                    {
                        consoleLog.AddItem(EnumEventLog.FATAL, "LookUp Series in DataBase",
                            String.Format("TimeIntervalName={0} not Found in the DataBase", sd.TimeInt));
                        fileLog.AddItem(EnumEventLog.FATAL, "LookUp Series in DataBase",
                            String.Format("TimeIntervalName={0} not Found in the DataBase", sd.TimeInt));
                    }
                    continue;
                } 

                _lastBar = (_series.Bars.OrderBy(b => b.DT)).LastOrDefault();
                if (_lastBar == null)
                {
                   fromDateTime = sd.FirstDateTime;
                   
                    consoleLog.AddItem(EnumEventLog.WARNING, "LookUp LastBar",
                        String.Format("Series: TickerName={0};TimeIntName={1} IsEmpty. Let's begin it from this Date={2}",
                    sd.Ticker, sd.TimeInt, fromDateTime));
                    fileLog.AddItem(EnumEventLog.WARNING, "LookUp LastBar",
                        String.Format("Series: TickerName={0};TimeIntName={1} IsEmpty. Let's begin it from this Date={2}",
                    sd.Ticker, sd.TimeInt, fromDateTime));
                }
                else
                {
                    fromDateTime = _lastBar.DT;
                }

                toDateTime = DateTime.Now;
                fin.DownLoad(sd.Ticker, sd.TimeInt, fromDateTime, toDateTime);

                var needToSubmit = false;
                if (DownLoadBars.Count > 0)
                {
                    needToSubmit = true;
                    if (_lastBar != null)
                    {
                        var firstDownLoadBar = DownLoadBars[0];
                        if (_lastBar.DT.CompareTo(firstDownLoadBar.DT) == 0)
                        {
                            _lastBar.High = firstDownLoadBar.High;
                            _lastBar.Low = firstDownLoadBar.Low;
                            _lastBar.Close = firstDownLoadBar.Close;
                            _lastBar.Volume = firstDownLoadBar.Volume;
                            DownLoadBars.RemoveAt(0);
                        }
                        toDateTime = _lastBar.DT;
                    }
                }
                if (needToSubmit)
                {
                    q.InsertBars(DownLoadBars);
                    q.Submit();
                }
                if (DownLoadBars.Count > 0) toDateTime = DownLoadBars[DownLoadBars.Count - 1].DT;

                consoleLog.AddItem(EnumEventLog.SUCCESS,"DownLoad Series",
                    String.Format("TickerName={0};TimeInt={1};BarsCount={2};From={3};To={4}",
                    _ticker.Code, _series.TimeInt.Name, DownLoadBars.Count, fromDateTime, toDateTime));
                fileLog.AddItem(EnumEventLog.SUCCESS, "DownLoad Series",
                    String.Format("TickerName={0};TimeInt={1};BarsCount={2};From={3};To={4}",
                    _ticker.Code, _series.TimeInt.Name, DownLoadBars.Count, fromDateTime, toDateTime));

                DownLoadBars.Clear();
            }
            consoleLog.AddItem(EnumEventLog.SUCCESS, "DownLoad Bars", "Finish");
            fileLog.AddItem(EnumEventLog.SUCCESS, "DownLoad Bars", "Finish");

           // Console.ReadLine();
        }
    }
}
