using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using GS.Trade.DataBase.Model;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Extension;
using EntityFramework.BulkInsert.Extensions;
using GS.Trade.TimeSeries.FortsTicks3.Model;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Interfaces;

using Ticker = GS.Trade.TimeSeries.FortsTicks3.Model.Ticker;
using Tick = GS.Trade.TimeSeries.FortsTicks3.Model.Tick;
using FileSeries = GS.Trade.TimeSeries.FortsTicks3.Model.FileSeries;
using Bar = GS.Trade.TimeSeries.FortsTicks3.Model.Bar;
using Stat = GS.Trade.TimeSeries.FortsTicks3.Model.Stat;

namespace GS.Trade.TimeSeries.FortsTicks3.Dal
{
    using BarSeries = Model.BarSeries;

    public enum TickFieldsEnum  : int {Code = 0, Contract=1,Price=2,Amount=3,DateTime=4, TradeId=5, NoSystem=6}

    public partial class FortsTicksContext3
    {
        
        public void AddTick(DateTime dt, long tradeId,
                                string code, string contract,
                                double amount, double price)
        {
            
        }
        public void AddTicks(string filename)
        {
            var contractlist = Tickers.Select(t=>t.Contract).ToList();
            var tickers = Tickers.ToList();
            if (tickers.Count == 0)
                return;

            var tickList = new List<Tick>();

            var lst = new List<String[]>();
            var dlmt = new char[] { ';' };
            using (var sr = new StreamReader(filename))
            {
                try
                {
                    var line = sr.ReadLine(); // headers
                    while ((line = sr.ReadLine()) != null)
                    {
                        var s = line.Split(dlmt);
                        if (int.Parse(s[(int)TickFieldsEnum.NoSystem].Trim()) != 0)
                                      continue;

                        if (contractlist.Contains(s[(int)TickFieldsEnum.Contract].Trim()))
                                lst.Add(s);                      
                    }
                    var lstCnt = lst.Count;
                    if (lst.Count <= 0)
                        return;

                    var firstTickDT = DateTime.Parse(lst.First()[(int)TickFieldsEnum.DateTime]);
                    var lastTickDT = DateTime.Parse(lst.Last()[(int)TickFieldsEnum.DateTime]);

                    var crntTicker = tickers.FirstOrDefault();
                    if (crntTicker == null)
                        return;
                    
                    // var lastTickDateTime = new DateTime();
                    
                    foreach (var s in lst.OrderBy(s=>s[(int)TickFieldsEnum.Contract].Trim()))
                    {
                        var contract = s[(int) TickFieldsEnum.Contract].Trim();
                        if (crntTicker.Contract != contract)
                        {
                            crntTicker = Tickers.FirstOrDefault(ti => ti.Contract == contract);
                            if (crntTicker == null)
                                throw new NullReferenceException(
                                    String.Format("Ticker with ContractName: {0} is Null", contract));
                        }
                        var t = new Tick
                        {
                            //TickerID = crntTicker.ID,
                            DT = DateTime.Parse(s[(int)TickFieldsEnum.DateTime]),
                            Price = s[(int)TickFieldsEnum.Price].ToDouble(),
                            Amount = s[(int)TickFieldsEnum.Amount].ToDouble(),
                            TradeID = long.Parse(s[(int)TickFieldsEnum.TradeId].Trim())
                        };
                        tickList.Add(t);

                        // lastTickDateTime = t.DateTime;
                    }
                    var tickCount = tickList.Count;
                    if (tickCount <= 0)
                        return;
                    try
                    {
                        using (var transactionScope = new TransactionScope())
                        {
                            //var file = Files.Add(new File
                            //{
                            //    // Date = lastTickDateTime.DateToInt(),
                            //    // Date = firstTickDT.DateToInt(),
                            //    FirstDT = firstTickDT,
                            //    LastDT = lastTickDT,
                            //    Count = tickCount,
                            //    Name = filename,
                            //    // ModifiedDT = DateTime.Now
                            //});
                            SaveChanges();
                            foreach (var t in tickList)
                            {
                               // t.SrcFileID = file.ID;
                            }

                            this.BulkInsert((IEnumerable<Tick>) tickList);

                            //SrcFiles.Add(new SrcFile
                            //{
                            //    // Date = lastTickDateTime.DateToInt(),
                            //    Date = firstTickDT.DateToInt(),
                            //    FirstDT = firstTickDT,
                            //    LastDT = lastTickDT,
                            //    Count = tickCount,
                            //    Name = filename,
                            //    ModifiedDT = DateTime.Now
                            //});
                            SaveChanges();
                            transactionScope.Complete();
                        }
                        

                    }
                    catch(Exception e)
                    {
                        throw new Exception(e.Message);
                    }
                }
                catch (Exception e)
                {
                   // Works.EvlMessage(EvlResult.FATAL, fileName, e.Message);
                    throw new Exception(e.Message);
                }
                finally
                {
                    sr.Close();
                }
            }
        }

        public void AddTicks2(string filename)
        {
            var contractlist = Tickers.Select(t => t.Contract).ToList();

            var cntrlist = Tickers.Select(t => new
            {
                Contract = t.Contract.Trim(),
                t.MinStoreDateTime
            });
            var tickers = Tickers.ToList();
            if (tickers.Count == 0)
                return;

            var fileTicks = new List<FileTick>();
            var dlmt = new char[] {';'};

            using (var sr = new StreamReader(filename))
            {
                try
                {
                    var line = sr.ReadLine(); // headers
                    while ((line = sr.ReadLine()) != null)
                    {
                        var s = line.Split(dlmt);

                        if (int.Parse(s[(int) TickFieldsEnum.NoSystem].Trim()) != 0)
                            continue;

                        // Only Existin Contract and only DaysToStore clause: dt > t.MinStoreDateTime 

                        var contract = s[(int)TickFieldsEnum.Contract].Trim();
                        var dt = DateTime.Parse(s[(int) TickFieldsEnum.DateTime]);

                        //var r = tickers.Any(t =>
                        //            t.Contract.Equals(contract, StringComparison.InvariantCultureIgnoreCase) &&
                        //            dt > t.MinStoreDateTime);

                        if (tickers.Any(t =>
                                t.Contract.Equals(contract, StringComparison.InvariantCultureIgnoreCase) &&
                                dt > t.MinStoreDateTime)
                           )
                             fileTicks.Add(new FileTick
                            {
                                TradeID = long.Parse(s[(int) TickFieldsEnum.TradeId].Trim()),
                                Code = s[(int) TickFieldsEnum.Code].Trim(),
                                Contract = contract,
                                Price = s[(int) TickFieldsEnum.Price].ToDouble(),
                                Amount = s[(int) TickFieldsEnum.Amount].ToDouble(),
                                DateTime = dt
                            });
                    }
                }
                catch (Exception e)
                {

                }
            }
            var fileTicksCnt = fileTicks.Count;
            if (fileTicksCnt <= 0)
                return;

           // var file = RegisterFileSeries(filename, fileTicks);

            var fileTickSeries = (from t in fileTicks
                .Select(t => new {t.Contract, t.DateTime})
                group t by new
                {
                    t.Contract,
                    dt = t.DateTime.Date
                }
                into g
                select new FileTickStat()
                {
                    TickerName = g.Key.Contract,
                    Date = g.Key.dt,
                    FirstDT = g.Min(t => t.DateTime),
                    LastDT = g.Max(t => t.DateTime),
                    Count = g.Count()
                }).ToList();

           
            //Populate Series NOT IN DataBAse(NEW SESRIES)
            var dbtickseries = new List<TickSeries>();
            foreach (var fts in fileTickSeries)
            {
                var dbts = GetTickSeries(fts.TickerName, fts.Date.Date);
                if (dbts != null)
                {
                    dbts.Count += fts.Count;
                    dbts.FirstDT = dbts.FirstDT > fts.FirstDT ? fts.FirstDT : dbts.FirstDT;
                    dbts.LastDT = dbts.LastDT < fts.LastDT ? fts.LastDT : dbts.LastDT;

                    dbts.Modified = DateTime.Now;
                    dbts.Processed = DateTime.Now;

                    dbtickseries.Add(dbts);
                }
                else
                {
                    var ticker = GetTicker(fts.TickerName);
                    var ts = new TickSeries
                    {
                        Code = $"{ticker.Contract}, {fts.Date.ToString("d")}",
                        Description = $"TickSeries: Ticker: {ticker.Contract}, Date: {fts.Date.ToString("d")}",
                        TickerID = ticker.ID,
                        Date = fts.Date,
                        TimeInt = 0,
                        Count = fts.Count,
                        FirstDT = fts.FirstDT,
                        LastDT = fts.LastDT,
                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        //Processed = (DateTime) SqlDateTime.MinValue,
                        Processed = DateTime.Now,
                    };
                    TimeSeries.Add(ts);
                    SaveChanges();
                    dbtickseries.Add(ts);
                }
            }          

            var ticks = new List<Tick>();
            foreach (var t in fileTicks)
            {
                var ts =
                    dbtickseries.FirstOrDefault(srs =>
                            srs.Ticker.Contract.Equals(t.Contract, StringComparison.InvariantCultureIgnoreCase) &&
                            srs.Date.Date == t.DateTime.Date);
                if (ts != null)
                {
                    //if (t.DateTime > ts.Ticker.ExpirationDateTime.AddDays(-ts.Ticker.DaysToStore))

                        ticks.Add(new Tick
                        {
                            SeriesID = ts.ID,
                            DT = t.DateTime,
                            Amount = t.Amount,
                            Price = t.Price,
                            TradeID = t.TradeID
                        });
                }
                else
                {
                    throw new Exception(
                        $"TickSeries with Contract: {t.Contract}, " +
                        $"Date: {t.DateTime.Date.ToString("g")}, " +
                        $"File: {filename} Should be Exists");
                }
            }

            var tickCount = ticks.Count;
            if (tickCount <= 0)
                return;

            RegisterFileSeries(filename, fileTickSeries);

            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    

                    this.BulkInsert((IEnumerable<Tick>) ticks);
                  
                    SaveChanges();
                    transactionScope.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }    
        public void AddBars(IEnumerable<Bar> bars)
        {
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    this.BulkInsert(bars);
                    SaveChanges();
                    transactionScope.Complete();
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Join(" ",
                    System.Reflection.MethodBase.GetCurrentMethod().Name,
                    e.Message));
            }
        }

        public void AddBarsFromTicks()
        {
            var tickSeries = TickSeriesSet.Where(t => t.Count > 0 ).ToList();
            foreach (var bs in tickSeries.Select(ts => GetBarSeries(ts) ?? CreateBarSeries(ts)))
            {
                TimeSeries.Add(bs);
                SaveChanges();
            }
            // var barSeries = BarSeriesSet.Where(bs => bs.TimeInt == 1).ToList();

            foreach (var ts in tickSeries )
            {
                var bs = GetBarSeries(ts);
                if(bs == null)
                    throw new Exception("BarSeries is Not Found");

                if (bs.Processed >= ts.Processed)
                    continue;
                var bars = CreateBarsFromTicks(ts, bs);
               // AddBars(bars);

                var cnt = bars.Count;
                if (cnt <= 0)
                    continue;

                AddBars(bars);

                bs.FirstDT = bars.First().DT;
                bs.LastDT = bars.Last().DT;
                bs.Count = cnt;
                bs.Processed = DateTime.Now;

                SaveChanges();
            }
        }

        private List<Bar> CreateBarsFromTicks(TickSeries ts, Model.BarSeries bs)
        {
            DateTime dt = DateTime.MinValue;
            var ticks = GetTicks(ts);
            var bars = new List<Bar>();
            Bar b = null;
            foreach (var t in ticks)
            {
                var tdt = t.DT.TruncateMilliSeconds().IncSeconds();
                
                if( dt.Equals(tdt) && b != null)
                    BarUpdate(b,t);
                else
                {
                    if (b != null)
                        bars.Add(b);

                    dt = tdt;
                    b = NewBar(bs.ID, t,  tdt);
                }
            }
            // Last Bar DT is Not Updated and NOT Added in the Bars List
            // Lost Last Bar
            if(b != null && bars.Count > 0 && b.DT > bars.Last().DT)
                bars.Add(b);
            return bars;
        }

        private BarSeries CreateBarSeries(TickSeries ts)
        {
            return new BarSeries
            {
                TickerID = ts.TickerID,
                Code = $"{ts.Ticker.Contract},{ts.Date.ToString("d")}",
                Name = $"{ts.Ticker.Contract},{ts.Date.ToString("d")}",
                Date = ts.Date,
                Description = $"BarSeries: Ticker: {ts.Ticker.Contract}, Date: {ts.Date.ToString("d")}, TimeInt: 1 Sec",
                TimeInt = 1
            };
        }

        public IEnumerable<string> GetFiles()
        {
            return FileSeriesSet.Select(f => f.Description.Trim()).ToList();
        }

        //public DateTime? GetTicksLastDate(string tickercode)
        //{
        //    var ti = GetTicker(tickercode);
        //    if (ti == null)
        //        return null;
        //    var tstat = Stats
        //        .FirstOrDefault(ts => ts.TickerID == ti.ID && 
        //                        ts.Period == StatTimeIntType.All && 
        //                        ts.Type == TickBarTypeEnum.Ticks);
        //    if(tstat==null)
        //        return null;
        //    return tstat.LastDT.Date;
        //}

        public Ticker GetTicker(string tickercode)
        {
            return  Tickers.FirstOrDefault(t => t.Code.Equals(tickercode, StringComparison.InvariantCultureIgnoreCase))
                     ?? Tickers.FirstOrDefault(t => t.Contract.Equals(tickercode, StringComparison.InvariantCultureIgnoreCase));
        }
        public Model.FileSeries GetFileSeries(string fileName)
        {
            return FileSeriesSet.FirstOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                        ?? FileSeriesSet.FirstOrDefault(f => f.Code.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                        ?? FileSeriesSet.FirstOrDefault(f => f.Description.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
        }
        public FileSeries GetFileSeries(string fileName, string tickername, DateTime date )
        {
            // var ticker = GetTicker(tickername);
            return FileSeriesSet.FirstOrDefault(f =>
                f.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase) &&
                f.Ticker.Contract.Equals(tickername, StringComparison.InvariantCultureIgnoreCase) &&
                f.Date.Equals(date.Date))
                   ?? FileSeriesSet.FirstOrDefault(f =>
                       f.Code.Equals(fileName, StringComparison.InvariantCultureIgnoreCase) &&
                       f.Ticker.Contract.Equals(tickername, StringComparison.InvariantCultureIgnoreCase) &&
                       f.Date.Equals(date.Date))
                   ?? FileSeriesSet.FirstOrDefault(f =>
                       f.Description.Equals(fileName, StringComparison.InvariantCultureIgnoreCase) &&
                       f.Ticker.Contract.Equals(tickername, StringComparison.InvariantCultureIgnoreCase) &&
                       f.Date.Equals(date.Date));
        }

        public Model.FileSeries RegisterFileSeries(string filename)
        {
            var f = GetFileSeries(filename);
            if (f != null)
                return f;
            f = GetFileWithStat(filename);
            TimeSeries.Add(f);
            SaveChanges();
            return f;
        }

        private static FileSeries GetFileWithStat(string filename)
        {
            var lines = System.IO.File.ReadLines(filename);
            var enumerable = lines as string[] ?? lines.ToArray();

            var first = enumerable.Skip(1).First();
            var last = enumerable.Last();

            var dlmt = new [] { ';' };

            var s = ((string)first).Split(dlmt);
            var firstDT = DateTime.Parse(s[(int) TickFieldsEnum.DateTime]);

            s = ((string)last).Split(dlmt);
            var lastDT = DateTime.Parse(s[(int)TickFieldsEnum.DateTime]);

            return new FileSeries
            {
                Code = Path.GetFileName(filename),
                Name = Path.GetFileName(filename),
                Description = filename,
                Count = enumerable.LongCount(),
                FirstDT = firstDT,
                LastDT = lastDT,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Processed = DateTime.Now
            };
        }

        public Model.FileSeries RegisterFileSeries(string filename, List<FileTick> fticks)
        {
            var f = GetFileSeries(filename);
            if (f != null)
                return f;
            f = GetFileWithStat(filename, fticks);
            TimeSeries.Add(f);
            SaveChanges();
            return f;
        }

        private static FileSeries GetFileWithStat(string filename, List<FileTick> fticks )
        {
            return new FileSeries
            {
                Code = Path.GetFileName(filename),
                Name = filename,
                Count = fticks.LongCount(),
                FirstDT = fticks.First().DateTime,
                LastDT = fticks.Last().DateTime,
                Created = DateTime.Now,
                Modified = DateTime.Now,
                Processed = DateTime.Now
            };
        }

        public void RegisterFileSeries(string filename, List<FileTickStat> fstats)
        {
            foreach (var fst in fstats)
            {
                var f = GetFileSeries(filename, fst.TickerName, fst.Date.Date);
                if (f == null)
                {
                    var t = GetTicker(fst.TickerName);
                    if (t == null)
                        throw new Exception ($"Ticker: {fst.TickerName} is NOT Found");

                    var fs = new FileSeries
                    {
                        Code = $"{fst.TickerName}, {fst.Date.ToString("d")}",
                        Name = Path.GetFileName(filename),
                        Description = filename,

                        TickerID = t.ID,
                        Date = fst.Date.Date,

                        Count = fst.Count,
                        FirstDT = fst.FirstDT,
                        LastDT = fst.LastDT,

                        Created = DateTime.Now,
                        Modified = DateTime.Now,
                        Processed = DateTime.Now
                    };
                    TimeSeries.Add(fs);
                }
                else
                {
                    f.FirstDT = fst.FirstDT;
                    f.LastDT = fst.LastDT;

                    f.Created = DateTime.Now;
                    f.Modified = DateTime.Now;
                    f.Processed = DateTime.Now;
                }
                SaveChanges();
            }
        }

        //private static FileSeries GetFileWithStat(string filename, List<FileTickStat> fticks)
        //{
        //    return new FileSeries
        //    {
        //        Code = Path.GetFileName(filename),
        //        Name = filename,
        //        Count = fticks.LongCount(),
        //        FirstDT = fticks.First().DateTime,
        //        LastDT = fticks.Last().DateTime,
        //        Created = DateTime.Now,
        //        Modified = DateTime.Now,
        //        Processed = DateTime.Now
        //    };
        //}

        public TickSeries GetTickSeries(string ticker, DateTime dt)
        {
            return TickSeriesSet.FirstOrDefault(ts =>
                ts.Ticker.Contract.Equals(ticker, StringComparison.InvariantCultureIgnoreCase) &&
                ts.Date == dt.Date);
        }

        public TickSeries FindTickSeries(int seriesId)
        {
           return TickSeriesSet.FirstOrDefault(s=>s.ID == seriesId);
        }
        public Model.BarSeries FindBarSeries(int seriesId)
        {
            return BarSeriesSet.FirstOrDefault(s => s.ID == seriesId);
        }

        public Model.BarSeries GetBarSeries(string ticker, DateTime dt)
        {
            return BarSeriesSet.FirstOrDefault(ts =>
                ts.Ticker.Contract.Equals(ticker, StringComparison.InvariantCultureIgnoreCase) &&
                ts.Date == dt.Date);
        }

        public Model.BarSeries GetBarSeries(Model.TickSeries tms)
        {
            return BarSeriesSet.FirstOrDefault(bs =>
                        bs.TickerID == tms.TickerID &&
                        bs.Date == tms.Date &&
                        bs.TimeInt == 1);
                    // && bs.Processed < tms.Processed);
        }

        public void VerifySeries()
        {
            var tickseries = TickSeriesSet.ToList();

            var fileTickSeries = (from t in FileSeriesSet
                .Select(t => new { t.Ticker, t.Date, t.FirstDT, t.LastDT, t.Count })
                                  group t by new
                                  {
                                      t.Ticker,
                                      dt = t.Date
                                  }
                                  into g
                                  select new
                                  {
                                      Ticker = g.Key.Ticker,
                                      Date = g.Key.dt,
                                      FirstDT = g.Min(t => t.FirstDT),
                                      LastDT = g.Max(t => t.LastDT),
                                      Count = g.Sum(t=>t.Count)
                                  }).ToList();

            foreach (var ts in tickseries)
            {
                try
                {
                    var fts = fileTickSeries.FirstOrDefault(fs =>
                                fs.Ticker.ID == ts.Ticker.ID &&
                                fs.Date.Equals(ts.Date));

                    if (fts == null)
                        throw new Exception($"TimeSeries: {ts}\r\n File Series is Not Found");
                    if (ts.Count != fts.Count)
                        throw new Exception($"TimeSeries: {ts}\r\n Count is Not Equals: TimeSeries: {ts.Count}, FileSeries: {fts.Count}");
                    if (ts.FirstDT != fts.FirstDT)
                        throw new Exception($"TimeSeries: {ts}\r\n FirtsDT is Not Equals: TimeSeries: {ts.FirstDT}, FileSeries: {fts.FirstDT}");
                    if (ts.LastDT != fts.LastDT)
                        throw new Exception($"TimeSeries: {ts}\r\n FirtsDT is Not Equals: TimeSeries: {ts.LastDT}, FileSeries: {fts.LastDT}");

                    Console.WriteLine($"OK: {ts}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Exception: {e.Message}" );
                }
            }

        }

        
        public IEnumerable<Tick> GetTicksDaily(string tickercode, DateTime date)
        {
            var ti = GetTicker(tickercode);
            return ti == null 
                ? Enumerable.Empty<Tick>()
                : GetTicksDaily(ti.ID, date.Date);
        }
        public IEnumerable<Tick> GetTicksDaily(long tickerid, DateTime date)
        {
            //var dts = date.OneDay();
            var tms = TickSeriesSet.FirstOrDefault(ts => ts.TickerID == tickerid && ts.Date == date.Date);
            if (tms == null)
                return Enumerable.Empty<Tick>();
            var tks = Ticks
                        .Where(t => t.SeriesID == tms.ID)
                        .AsNoTracking()
                        .OrderBy(t => t.DT);
            return tks;
        }
        public IEnumerable<Tick> GetTicks(long seriesId)
        {
            var tks = Ticks
                        .Where(t => t.SeriesID == seriesId)
                        .AsNoTracking()
                        .OrderBy(t => t.DT);
            return tks;
        }
        public IEnumerable<Tick> GetTicks(TickSeries ts)
        {
            var tks = Ticks
                        .Where(t => t.SeriesID == ts.ID)
                        .AsNoTracking()
                        .OrderBy(t => t.DT);
            return tks;
        }
        public IEnumerable<Bar> GetBarsDaily(string tickercode, DateTime date)
        {
            var ti = GetTicker(tickercode);
            return ti == null 
                ? Enumerable.Empty<Bar>() 
                : GetBarsDaily(ti.ID, date);
        }

        public IEnumerable<Bar> GetBarsDaily(long tickerId, DateTime date)
        {
            var tms = BarSeriesSet.FirstOrDefault(ts => ts.TickerID == tickerId && ts.Date == date.Date);
            if (tms == null)
                return Enumerable.Empty<Bar>();
            var bs = Bars
                        .Where(t => t.SeriesID == tms.ID)
                        .AsNoTracking()
                        .OrderBy(t => t.DT);
            return bs;
        }
        /*
        public IEnumerable<Bar> GetBars(string tickercode, DateTime dt1, DateTime dt2)
        {
            var ti = GetTicker(tickercode);
            return ti == null
                ? Enumerable.Empty<Bar>()
                : GetBars(ti.ID, dt1, dt2);
        }
        */
        //public IEnumerable<Bar> GetBars(int tickerId, DateTime dt1, DateTime dt2)
        //{
        //    var bs = Bars
        //                .Where(t => t.TickerID == tickerId && t.DT > dt1 && t.DT < dt2)
        //                .AsNoTracking()
        //                .OrderBy(t => t.DT);
        //    return bs;
        //}
        /*
        public Stat GetTickerTotalStat(string tickercode)
        {
            var ti = GetTicker(tickercode);
            return ti == null
                ? null
                : Stats.FirstOrDefault(s => s.TickerID == ti.ID &&
                                            s.Period == StatTimeIntType.All &&
                                            s.Type == TickBarTypeEnum.Bars);
        }
        
        public IEntityTimeSeriesStat GetTickerTotalBarsStat1(string tickercode)
        {
            var ti = GetTicker(tickercode);
            if (ti == null)
                return null;
            var sts = Stats.Include(s => s.Ticker)
                .FirstOrDefault(s => s.TickerID == ti.ID &&
                                     s.Period == StatTimeIntType.All &&
                                     s.Type == TickBarTypeEnum.Bars);
            if (sts == null)
                return null;
            return new EntityTimeSeriesStat
            {
                EntityID = sts.TickerID,
                EntityCode = sts.Ticker.Code,
                EntityName = sts.Ticker.Contract,
                Count = sts.Count,
                FirstDT = sts.FirstDT,
                LastDT = sts.LastDT
            };
        }
        

        public IEntityTimeSeriesStat GetTickerTotalTicksStat(string tickercode)
        {
            var ti = GetTicker(tickercode);
            if (ti == null)
                return null;
            var sts = Stats.Include(s => s.Ticker)
                .FirstOrDefault(s => s.TickerID == ti.ID &&
                                     s.Period == StatTimeIntType.All &&
                                     s.Type == TickBarTypeEnum.Ticks);
            if (sts == null)
                return null;
            return new EntityTimeSeriesStat
            {
                EntityID = sts.TickerID,
                EntityCode = sts.Ticker.Code,
                EntityName = sts.Ticker.Contract,
                Count = sts.Count,
                FirstDT = sts.FirstDT,
                LastDT = sts.LastDT
            };
        }
        
        public IEnumerable<IEntityTimeSeriesStat> GetTickerDailyTicksStats(string tickercode)
        {
            var ti = GetTicker(tickercode);
            if (ti == null)
                return null;
            var sts = Stats
                        .Include(s => s.Ticker)
                        .Where(s => s.TickerID == ti.ID &&
                                    s.Period == StatTimeIntType.Daily &&
                                    s.Type == TickBarTypeEnum.Ticks)
                                    .AsNoTracking()
                                    .Select(s => new EntityTimeSeriesStat
            {
                EntityID = s.TickerID,
                EntityCode = s.Ticker.Code,
                EntityName = s.Ticker.Contract,
                Count = s.Count,
                FirstDT = s.FirstDT,
                LastDT = s.LastDT
            } )
            .OrderBy(d=>d.LastDT)
            .ToList();
            
            return sts;
            }
            */
        /*
    public IEnumerable<KeyValuePair<int, int>> GetTickerDatesNeedToCreateBars()
    {

        var tstats = Stats
            .Where(s => s.Type == TickBarTypeEnum.Ticks && s.Period == StatTimeIntType.Daily)
            .Select(s => new
            {
                s.TickerID,
                s.LastDate,
                // Key = s.TickerID + "@" + s.LastDate
            })
            .ToList();

        var bstats = Stats
            .Where(s => s.Type == TickBarTypeEnum.Bars && s.Period == StatTimeIntType.Daily)
            .Select(s => new
            {
                s.TickerID,
                s.LastDate,
                // Key = s.TickerID + "@" + s.LastDate
            })
            .ToList();
        // Get LastDates for every Tickers
        return tstats.Except(bstats).ToList()
            .Select(n => new KeyValuePair<int, int>(n.TickerID, n.LastDate));

    }
    */
        private Bar NewBar(Tick t)
        {
            return new Bar
            {
                SeriesID = t.SeriesID,
                DT = t.DT.ToLongInSec().ToDateTime().IncSeconds(),
                Open = t.Price,
                High = t.Price,
                Low = t.Price,
                Close = t.Price,
                Volume = t.Amount
            };
        }
        private Bar NewBar(int seriesId, Tick t, DateTime dt)
        {
            return new Bar
            {
                SeriesID = seriesId,
                DT = dt,
                Open = t.Price,
                High = t.Price,
                Low = t.Price,
                Close = t.Price,
                Volume = t.Amount
            };
        }

        private void BarUpdate(Bar b, Tick t)
        {
            b.Close = t.Price;
            b.Volume += t.Amount;

            if (t.Price.IsGreaterThan(b.High))
                b.High = t.Price;
            else if (t.Price.IsLessThan(b.Low))
                b.Low = t.Price;
        }
        public void InitTickers()
        {
            if (Tickers.Count() != 0)
                return;

            Ticker t;

            // 12.15
            t = new Ticker
            {
                Code = "RIZ5",
                Contract = "RTS-12.15",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2015-12-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiZ5",
                Contract = "Si-12.15",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2015-12-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRZ5",
                Contract = "SBRF-12.15",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2015-12-15 18:45")
            };
            Tickers.Add(t);

            // 3.16
            t = new Ticker
            {
                Code = "RIH6",
                Contract = "RTS-3.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-03-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiH6",
                Contract = "Si-3.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-03-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRH6",
                Contract = "SBRF-3.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-03-17 18:45")
            };
            Tickers.Add(t);

            // 6.16
            t = new Ticker
            {
                Code = "RIM6",
                Contract = "RTS-6.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-06-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiM6",
                Contract = "Si-6.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-06-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRM6",
                Contract = "SBRF-6.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-06-16 18:45")
            };
            Tickers.Add(t);

            // 9.16
            t = new Ticker
            {
                Code = "RIU6",
                Contract = "RTS-9.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-09-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiU6",
                Contract = "Si-9.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-09-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRU6",
                Contract = "SBRF-9.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-09-15 18:45")
            };
            Tickers.Add(t);

            // 12.16
            t = new Ticker
            {
                Code = "RIZ6",
                Contract = "RTS-12.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-12-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiZ6",
                Contract = "Si-12.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-12-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRZ6",
                Contract = "SBRF-12.16",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2016-12-15 18:45")
            };
            Tickers.Add(t);

            // 3.17
            t = new Ticker
            {
                Code = "RIH7",
                Contract = "RTS-3.17",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2017-03-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiH7",
                Contract = "Si-3.17",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2017-03-15 18:45")
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRH7",
                Contract = "SBRF-3.17",
                DaysToStore = 120,
                ExpirationDateTime = DateTime.Parse("2017-03-16 18:45")
            };
            Tickers.Add(t);

            SaveChanges();
        }
    }
    public class FileTickStat
    {
        public string TickerName { get; set; }
        public DateTime Date { get; set; }
        public long Count { get; set; }
        public DateTime FirstDT { get; set; }
        public DateTime LastDT { get; set; }
    }
}
