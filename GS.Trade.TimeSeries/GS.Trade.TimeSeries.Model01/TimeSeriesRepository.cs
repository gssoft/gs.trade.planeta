using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.DataBase.Storage;
using GS.Trade.Storage2;

namespace GS.Trade.TimeSeries.Model01
{
    public interface ITimeSeriesRepository : IEntityRepository<ITimeSeriesBase, ITimeSeries>
    {
    }
    public interface IBarsRepository : IEntityRepository<IBarDb, IBar>
    {
    }

    public class BarSeriesRepository33 :
                   TradeBaseRepository33<TimeSeriesContext, string, ITimeSeries, TimeSeries, ITimeSeriesBase>,
                   ITimeSeriesRepository
    {
        public override string Key { get { return Code; } }

        protected override TimeSeries GetByKey(TimeSeriesContext cntx, string key)
        {
            var a = cntx.TimeSeries.FirstOrDefault(e => e.Key == key);
            //?? cntx.Accounts.FirstOrDefault(e => e.Code == key)
            //    ?? cntx.Accounts.FirstOrDefault(e => e.Alias == key);
            return a;
        }
        protected override TimeSeries Get(TimeSeriesContext cntx, string key)
        {
            var a = cntx.TimeSeries.FirstOrDefault(e => e.Key == key)
                            ?? cntx.TimeSeries.FirstOrDefault(e => e.Code == key)
                                    ?? cntx.TimeSeries.FirstOrDefault(e => e.Name == key);
            return a;
        }
        protected override bool AddVal(TimeSeriesContext cntx, ITimeSeries a)
        {
            var dt = DateTime.Now;
            //try
            //{
                var ac = new BarSeries
                {
                    Name = a.Name,
                    Code = a.Code,
                    Key = a.Key,
                    TickerId = a.Ticker.Id,
                    TimeInterval = a.TimeIntSeconds,
                    TimeShift = a.ShiftIntSecond,
                    CreatedDT = dt,
                    ModifiedDT = dt,
                };
                cntx.TimeSeries.Add(ac);
                cntx.SaveChanges();

                a.Id = ac.Id;
                return true;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, a.GetType().ToString(), "Add(BarSeries):", a.ToString(), e);
            //    throw;
            //}
        }
        protected override ITimeSeries Update(TimeSeriesContext cntx, ITimeSeries ve, TimeSeries vi) // AddOrGet
        {
            ve.Id = vi.Id;
            ve.Code = vi.Code;
            ve.Name = vi.Name;
            //ve.TimeInterval = vi.TimeInterval;
            //ve.ShiftIntSecond = vi.TimeShift;

            return ve;
        }
        protected override bool Update(TimeSeriesContext cntx, TimeSeries vi, ITimeSeries ve)
        {
            //try
            //{
            if (ve == null)
            {
                //throw new ArgumentNullException("ve", "AccountRepository32.Update(Account); " + "Account==Null");
                var e = new ArgumentNullException("ve", FullName + " Update(Account); " + "TimeSeries==Null");
                SendExceptionMessage3(FullName,
                                "TimeSeries", "TimeSeriesRepository33.Update(TimeSeries)", "TimeSeries", e);
                return false;
            }
                //vi.Id = ve.Id;
                vi.Code = ve.Code;
                vi.Name = ve.Name;

                vi.TimeInterval = ve.TimeIntSeconds;
                vi.TimeShift = ve.ShiftIntSecond;

                vi.ModifiedDT = DateTime.Now;
                cntx.SaveChanges();

                //if (IsUIEnabled)
                //    FireStorageChangedEvent("UI.Accounts", "Account", "Update", ve);

                //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                //        FullName, Code, "Update(Account)", ve.Code, ve.ToString());
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName,
            //        ve == null ? "Account" : ve.GetType().ToString(), "AccountRepository32.Update(Account)",
            //        ve == null ? "Account" : ve.ToString(), e);
            //    throw;
            //}
            return true;
        }

        protected override TimeSeriesContext GetContext(string dataBaseName)
        {
            return new TimeSeriesContext(dataBaseName);
        }
    }
    public class BarsRepository33 :
                TradeBaseRepository33<TimeSeriesContext, string, IBar, Bar, IBarDb>,
                IBarsRepository
    {
        public override string Key { get { return Code; } }

        protected override Bar GetByKey(TimeSeriesContext cntx, string key)
        {
            var a = cntx.Bars.FirstOrDefault(e => e.Key == key);
            //?? cntx.Accounts.FirstOrDefault(e => e.Code == key)
            //    ?? cntx.Accounts.FirstOrDefault(e => e.Alias == key);
            return a;
        }
        protected override Bar Get(TimeSeriesContext cntx, string key)
        {
            var a = cntx.Bars.FirstOrDefault(e => e.Key == key);
            return a;
        }
        protected override bool AddVal(TimeSeriesContext cntx, IBar b)
        {
            //try
            //{
            var ac = new Bar
            {
                BarSeriesId = b.SeriesID,
                DT = b.DT,
                Key = b.Key,

                Open = b.Open,
                High = b.High,
                Low = b.Low,
                Close = b.Close,

                Volume = b.Volume
            };
            cntx.Bars.Add(ac);
            cntx.SaveChanges();

            //b.Id = ac.Id;
            return true;
            //}
            //catch (Exception e)
            //{
            //    SendExceptionMessage3(FullName, ac.GetType().ToString(), "Add(BarSeries):", ac.ToString(), e);
            //    throw;
            //}
        }
        protected override IBar Update(TimeSeriesContext cntx, IBar ve, Bar vi) // AddOrGet
        {
            // ve.Id = vi.Id;
            //ve.Open = vi.Open;
            //ve.High = vi.Open;
            //ve.Low = vi.Open;
            //ve.Close = vi.Open;

            //ve.Volume = vi.Open;

            return ve;
        }
        protected override bool Update(TimeSeriesContext cntx, Bar vi, IBar ve)
        {
            try
            {
                if (ve == null)
                    throw new ArgumentNullException("ve", "AccountRepository32.Update(Account); " + "Account==Null");


                //vi.Name = ve.Name;
                //vi.ModifiedDT = DateTime.Now;

                //cntx.SaveChanges();

                //if (IsUIEnabled)
                //    FireStorageChangedEvent("UI.Accounts", "Account", "Update", ve);

                //Evlm1(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY,
                //        FullName, Code, "Update(Account)", ve.Code, ve.ToString());
            }
            catch (Exception e)
            {
                SendExceptionMessage3(FullName,
                    ve == null ? "Account" : ve.GetType().ToString(), "AccountRepository32.Update(Account)",
                    ve == null ? "Account" : ve.ToString(), e);
                throw;
            }
            return true;
        }

        protected override TimeSeriesContext GetContext(string dataBaseName)
        {
            return new TimeSeriesContext(dataBaseName);
        }
    }
}
