using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using GS.Trade.DataBase.Model;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Extension;
using EntityFramework.BulkInsert.Extensions;
using GS.Trade.TimeSeries.FortsTicks2.Model;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Interfaces;

using Ticker = GS.Trade.TimeSeries.FortsTicks2.Model.Ticker;
using Tick = GS.Trade.TimeSeries.FortsTicks2.Model.Tick;
using File = GS.Trade.TimeSeries.FortsTicks2.Model.File;
using Bar = GS.Trade.TimeSeries.FortsTicks2.Model.Bar;
using Stat = GS.Trade.TimeSeries.FortsTicks2.Model.Stat;

namespace GS.Trade.TimeSeries.FortsTicks2.Dal
{
    public enum TickFieldsEnum  : int {Code = 0, Contract=1,Price=2,Amount=3,DateTime=4, TradeId=5, NoSystem=6}

    public partial class FortsTicksContext2
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
                            var file = Files.Add(new File
                            {
                                // Date = lastTickDateTime.DateToInt(),
                                // Date = firstTickDT.DateToInt(),
                                FirstDT = firstTickDT,
                                LastDT = lastTickDT,
                                Count = tickCount,
                                Name = filename,
                                // ModifiedDT = DateTime.Now
                            });
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

                        if (contractlist.Contains(s[(int) TickFieldsEnum.Contract].Trim()))
                            fileTicks.Add(new FileTick
                            {
                                TradeID = long.Parse(s[(int) TickFieldsEnum.TradeId].Trim()),
                                Code = s[(int) TickFieldsEnum.Code].Trim(),
                                Contract = s[(int) TickFieldsEnum.Contract].Trim(),
                                Price = s[(int) TickFieldsEnum.Price].ToDouble(),
                                Amount = s[(int) TickFieldsEnum.Amount].ToDouble(),
                                DateTime = DateTime.Parse(s[(int) TickFieldsEnum.DateTime])
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

            var file = RegisterFile(filename);

            var fileSeries = (from t in fileTicks
                .Select(t => new {t.Contract, t.DateTime})
                group t by new
                {
                    t.Contract,
                    dt = t.DateTime.Date
                }
                into g
                select new FileSeries()
                {
                    Contract = g.Key.Contract,
                    File = filename,
                    FirstDT = g.Min(t => t.DateTime),
                    LastDT = g.Max(t => t.DateTime),
                    Count = g.Count()
                }).ToList();

            //var dbSeries = TickSeries.Include(ts => ts.Ticker).Include(ts => ts.File)
            //    .Where(ts => !fileSeries.Select(fs => fs.Contract).Contains(ts.Ticker.Contract) &&
            //                 !fileSeries.Select(fs => fs.File).Contains(filename) &&
            //                 !fileSeries.Select(fs => fs.Date).Contains(ts.Date))
            //    .ToList();
            //var dbSrsList = new List<TickSeries>();
            //foreach (var f in fileSeries)
            //{
                
            //}

            // Populate Series NOT IN DataBAse (NEW SESRIES)
            foreach (var tickSeries in from fs in fileSeries
                let ts = GetTickSeries(fs.Contract, filename, fs.Date.Date)
                where ts == null
                let ticker = GetTicker(fs.Contract)
                select new Model.TickSeries
                {
                    Code = $"{ticker.Contract}, {fs.Date.ToString("d")}",
                    Description = $"TickSeries: Ticker: {ticker.Contract}, Date: {fs.Date.ToString("d")}, File: {file.Name}",
                    TickerID = ticker.ID,
                    FileID = file.ID,
                    Date = fs.Date,
                    TimeInt = 0,
                    FirstDT = (DateTime)SqlDateTime.MinValue,
                    LastDT = (DateTime)SqlDateTime.MinValue,
                    Created = DateTime.Now,
                    Modified = DateTime.Now
                })
            {
                TickSeries.Add(tickSeries);
                SaveChanges();
            }


            var dbtickseries = TickSeries
                .Where(t => t.File.Name == filename)
                .ToList();

            var ticks = new List<Tick>();
            foreach (var t in fileTicks)
            {
                var ts =
                    dbtickseries.FirstOrDefault(srs =>
                            srs.Ticker.Contract.Equals(t.Contract, StringComparison.InvariantCultureIgnoreCase) &&
                            srs.Date.Date == t.DateTime.Date);
                if (ts != null)
                {
                    if (t.DateTime > ts.Ticker.ExpirationDateTime.AddDays(-ts.Ticker.DaysToStore))

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
            try
            {
                using (var transactionScope = new TransactionScope())
                {
                    //var file = Files.Add(new File
                    //{
                    //    // Date = lastTickDateTime.DateToInt(),
                    //    // Date = firstTickDT.DateToInt(),
                    //    //FirstDT = firstTickDT,
                    //    //LastDT = lastTickDT,
                    //    Count = tickCount,
                    //    Name = filename,
                    //    // ModifiedDT = DateTime.Now
                    //});
                    //SaveChanges();
                   

                    this.BulkInsert((IEnumerable<Tick>) ticks);

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

        public IEnumerable<string> GetFiles()
        {
            return Files.Select(f => f.Name).ToList();
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
        public Model.File GetFile(string fileName)
        {
            return Files.FirstOrDefault(f => f.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
        }

        public Model.File RegisterFile(string filename)
        {
            var f = GetFile(filename);
            if (f != null)
                return f;
            f = GetFileWithStat(filename);
            Files.Add(f);
            SaveChanges();
            return f;
        }

        private static File GetFileWithStat(string filename)
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

            return new File
            {
                Name = filename,
                Count = enumerable.Count(),
                FirstDT = firstDT,
                LastDT = lastDT,
                Created = DateTime.Now,
                Modified = DateTime.Now
            };
        }

        public TickSeries GetTickSeries(string ticker, string file, DateTime dt)
        {
            return TickSeries.FirstOrDefault(ts =>
                ts.Ticker.Contract.Equals(ticker, StringComparison.InvariantCultureIgnoreCase) &&
                ts.File.Name.Equals(file,StringComparison.InvariantCultureIgnoreCase) &&
                ts.Date == dt.Date);
        }

        public TickSeries FindTickSeries(int seriesId)
        {
            return TickSeries.Find(seriesId);
        }

        /*
        public IEnumerable<Tick> GetTicks(string tickercode, DateTime date)
        {
            var ti = GetTicker(tickercode);
            return ti == null 
                ? Enumerable.Empty<Tick>()
                : GetTicks(ti.ID, date);
        }
        public IEnumerable<Tick> GetTicks(int tickerid, DateTime date)
        {
            var dts = date.OneDay();
            var tks = Ticks
                        .Where(t => t.TickerID == tickerid && t.DT > dts.Key && t.DT < dts.Value)
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
        */
        //public IEnumerable<Bar> GetBarsDaily(int tickerId, DateTime date)
        //{
        //    var dts = date.OneDay();
        //    var bs = Bars
        //                .Where(t => t.TickerID == tickerId && t.DT > dts.Key && t.DT < dts.Value)
        //                .AsNoTracking()
        //                .OrderBy(t => t.DT);
        //    return bs;
        //}
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
                ExpirationDateTime = DateTime.Parse("2016-03-15 18:45")
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
                ExpirationDateTime = DateTime.Parse("2016-06-15 18:45")
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
                ExpirationDateTime = DateTime.Parse("2017-03-15 18:45")
            };
            Tickers.Add(t);

            SaveChanges();
        }
    }
}
