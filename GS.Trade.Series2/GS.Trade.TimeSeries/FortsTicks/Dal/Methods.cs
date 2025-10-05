using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using GS.Trade.DataBase.Model;
using GS.Trade.TimeSeries.FortsTicks.Model;
using GS.Extension;
using EntityFramework.BulkInsert.Extensions;
using GS.Trade.TimeSeries.General;
using GS.Trade.TimeSeries.Interfaces;
using Ticker = GS.Trade.TimeSeries.FortsTicks.Model.Ticker;

namespace GS.Trade.TimeSeries.FortsTicks.Dal
{
    public enum TickFieldsEnum  : int {Code = 0, Contract=1,Price=2,Amount=3,DateTime=4, TradeId=5, NoSystem=6}

    public partial class FortsTicksContext
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
                            TickerID = crntTicker.ID,
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
                            var file = SrcFiles.Add(new SrcFile
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
                                t.SrcFileID = file.ID;
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
            return SrcFiles.Select(f => f.Name).ToList();
        }

        public DateTime? GetTicksLastDate(string tickercode)
        {
            var ti = GetTicker(tickercode);
            if (ti == null)
                return null;
            var tstat = Stats
                .FirstOrDefault(ts => ts.TickerID == ti.ID && 
                                ts.Period == TimeSeriesStatEnum.All && 
                                ts.Type == TickBarTypeEnum.Ticks);
            if(tstat==null)
                return null;
            return tstat.LastDT.Date;
        }

        public Model.Ticker GetTicker(string tickercode)
        {
            return  Tickers.FirstOrDefault(t => t.Code.Equals(tickercode, StringComparison.InvariantCultureIgnoreCase))
                     ?? Tickers.FirstOrDefault(t => t.Contract.Equals(tickercode, StringComparison.InvariantCultureIgnoreCase));
        }

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
        public IEnumerable<Bar> GetBarsDaily(int tickerId, DateTime date)
        {
            var dts = date.OneDay();
            var bs = Bars
                        .Where(t => t.TickerID == tickerId && t.DT > dts.Key && t.DT < dts.Value)
                        .AsNoTracking()
                        .OrderBy(t => t.DT);
            return bs;
        }
        public IEnumerable<Bar> GetBars(string tickercode, DateTime dt1, DateTime dt2)
        {
            var ti = GetTicker(tickercode);
            return ti == null
                ? Enumerable.Empty<Bar>()
                : GetBars(ti.ID, dt1, dt2);
        }
        public IEnumerable<Bar> GetBars(int tickerId, DateTime dt1, DateTime dt2)
        {
            var bs = Bars
                        .Where(t => t.TickerID == tickerId && t.DT > dt1 && t.DT < dt2)
                        .AsNoTracking()
                        .OrderBy(t => t.DT);
            return bs;
        }

        public Stat GetTickerTotalStat(string tickercode)
        {
            var ti = GetTicker(tickercode);
            return ti == null
                ? null
                : Stats.FirstOrDefault(s => s.TickerID == ti.ID &&
                                            s.Period == TimeSeriesStatEnum.All &&
                                            s.Type == TickBarTypeEnum.Bars);
        }
        public IEntityTimeSeriesStat GetTickerTotalBarsStat1(string tickercode)
        {
            var ti = GetTicker(tickercode);
            if (ti == null)
                return null;
            var sts = Stats.Include(s => s.Ticker)
                .FirstOrDefault(s => s.TickerID == ti.ID &&
                                     s.Period == TimeSeriesStatEnum.All &&
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
                                     s.Period == TimeSeriesStatEnum.All &&
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
                                    s.Period == TimeSeriesStatEnum.Daily &&
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

        public IEnumerable<KeyValuePair<int, int>> GetTickerDatesNeedToCreateBars()
        {

            var tstats = Stats
                .Where(s => s.Type == TickBarTypeEnum.Ticks && s.Period == TimeSeriesStatEnum.Daily)
                .Select(s => new
                {
                    s.TickerID,
                    s.LastDate,
                    // Key = s.TickerID + "@" + s.LastDate
                })
                .ToList();

            var bstats = Stats
                .Where(s => s.Type == TickBarTypeEnum.Bars && s.Period == TimeSeriesStatEnum.Daily)
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

        public void InitTickers()
        {
            if (Tickers.Count() != 0)
                return;

            Ticker t;
            t = new Ticker
            {
                Code = "RIH6",
                Contract = "RTS-3.16"
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SiH6",
                Contract = "Si-3.16"
            };
            Tickers.Add(t);
            t = new Ticker
            {
                Code = "SRH6",
                Contract = "SBRF-3.16"
            };
            Tickers.Add(t);

            SaveChanges();
        }
    }
}
