using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Interfaces;

namespace GS.Trade.Data.Bars
{
    public class Bars001 : Bars
    {
        //private Bar _lastBar;

        private DateTime _dt;
        private double _last;
        private double _volume;

        private DateTime _tickBarDt;

        public Bars001(string name, ITicker ticker, int timeIntSeconds, int shiftSeconds)
            : base(name, ticker, timeIntSeconds, shiftSeconds)
        {
            NeedToCalcUnCompletedBar = true;
        }
        public Bars001(string code, string name, Ticker ticker, int timeIntSeconds, int shiftSeconds)
            : base(code, name, ticker, timeIntSeconds, shiftSeconds)
        {
            NeedToCalcUnCompletedBar = true;
        }

        public Bars001()
        {

        }
        public override void Init()
        {
           // DebudAddITem = true;
        }

        public override void Tick(DateTime dt, double last, double volume)
        {
            TickCount++;
            LastTickDT = dt; // LastTick DT very Important!

            _dt = dt;
            _last = last;
            _volume = volume;

            var m = dt.Millisecond;
            _tickBarDt = TimeSeries.GetBarDateTime(dt, TimeIntSeconds);

            if (Count > 0)
            {
                var lastBarDT = this[0].DT;
                var lastBar = this[0];
                
                if ((_tickBarDt - dt).Seconds > TimeIntSeconds)
                {
                    Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Sync Error", "Sync Error", "New Tick",
                                "DT: " + dt.ToString("hh:mm:ss:fff") + " Bar: " + _tickBarDt.ToString("hh:mm:ss:fff"), "");
                    throw new ArgumentOutOfRangeException(
                        "TimeInt: " + TimeIntSeconds + " DT: " + dt.ToString("hh:mm:ss:fff") + " Bar: " + _tickBarDt.ToString("hh:mm:ss:fff"));
                }
                var nTickBarDt = DtToLong(_tickBarDt);
                var nlastBarDt = DtToLong(lastBar.DT);
                /*
                if (Count > 1 &&
                    _lastBar.DT.CompareTo((this[0]).DT) != 0 && _lastBar.Close.CompareTo(last) != 0 && nlastBarDt == nTickBarDt)
                {
                    _lastBar.LastDT = dt;
                    AddItem(_lastBar);
                }
                 */
                if (nlastBarDt >= nTickBarDt)
                {
                    if (nlastBarDt == nTickBarDt)
                    {
                        Calculate2(0,0,1);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "BarSeries.Tick", "BarSeries.Tick", "New Tick",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", this[0].DT, dt), this[0].ToString());
                        throw new NullReferenceException("Sync is Lost");
                    }
                }
                else // New Bar should be Added
                {
                    /*
                    if (Count > 1 && _lastBar.IsFaded && _lastBar.Close.CompareTo(last) == 0)
                    {
                        _lastBar = new Bar(_tickBarDt, last, last, last, last, volume) { LastDT = dt };
                        return;
                    }
                    */
                    AddNewItem();
                    InitItem(0, 0);
                }
            }
            else
            {
                AddNewItem();
                InitItem(0, 0);
            }
        }

        public override void InitUpdate(DateTime dt)
        {
        }
        
        public void InitItem(int ibar, int ilast)
        {
            var lastItem = (Bar)Items[ilast];            

            lastItem.DT = _tickBarDt;

            lastItem.LastDT = _dt;
            lastItem.SyncDT = _dt;

            lastItem.Open = _last;
            lastItem.High = _last;
            lastItem.Low = _last;
            lastItem.Close = _last;

            lastItem.Volume = _volume;
            lastItem.Ticks = 1;

            if (Count < 2)
                return;

            var previouseBar = (Bar)Items[ilast + 1];
            if (previouseBar.DT.Date < lastItem.DT.Date)
            {
                ItemsDailyCount = 0;
            }
            else
            {
                ++ItemsDailyCount;
            }
        }

        public  void CopyItem(int ibar, int ilast, int previouse)
        {
            throw new NotImplementedException();
        }

        public  void Calculate2(int isync, int ilast, int iprev)
        {
            if (Count < 1) return;
            
            var last = ((Bar)Items[ilast]);
            //var prev = ((Bar)Items[iprev]);

            last.DT = _tickBarDt;

            if (last.High < _last) last.High = _last;
            if (last.Low > _last)  last.Low = _last;

            last.Close = _last;

            last.Volume += _volume;
            last.Ticks++;

            last.LastDT = _dt;
            last.SyncDT = _dt;
        }
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var t = (IBarSimple)tsi;
            if (Count > 0)
            {
                //var b = LastItem;
                
                FireChangedEvent("Bars", "Bar", "Add", LastItem);
            }
            AddItem(
                new Bar
                    {
                        SeriesID = Id,
                        DT = itemDt,
                        LastDT = tsi.LastDT,
                        SyncDT = syncDt,

                        Open = t.Open,
                        High = t.High,
                        Low = t.Low,
                        Close = t.Close,
                        Volume = t.Volume,
                        Ticks = t.Ticks
                    }
                );
        }
        public override void AddNewItem()
        {
           // if (Count > 0)
           //     Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING, Name, "New", LastItem.ToString(), "");

            if (IsFadedBar()) return;
            AddItem(new Bar());
        }
        private bool IsFadedBar()
        {
            if (Count < 2) return false;

            var lastBar = (Bar) LastItem;
            if (!lastBar.IsFaded || lastBar.Close.CompareTo(_last) != 0) return false;
           
            lastBar.SyncDT = _dt;
            return true;
        }

        /*
        public override void Calculate(TimeSeriesItem tsi)
        {
            if (Count > 1)
            {
                var b = (Bar) tsi;
                var lastItem = (Bar) LastItem;

                if (lastItem.High < b.High) lastItem.High = b.High;
                if (lastItem.Low > b.Low) lastItem.Low = b.Low;
                lastItem.Close = b.Close;

                lastItem.Volume += b.Volume;
                lastItem.Ticks += b.Ticks;

                lastItem.LastDT = b.DT;
                lastItem.SyncDT = b.DT;
            }
            else if (Count > 0)
                    InitItem(tsi);
        }
        public override void InitItem(TimeSeriesItem tsi)
        {
            var b = (Bar)tsi;
            var lastItem = (Bar)LastItem;

            var syncDt = GetBarDateTime2(tsi.DT, TimeIntSeconds);
            lastItem.DT = syncDt;
            lastItem.LastDT = syncDt;
            lastItem.SyncDT = syncDt;

            lastItem.Open = b.Open;
            lastItem.High = b.High;
            lastItem.Low = b.Low;
            lastItem.Close = b.Close;
            lastItem.Volume = b.Volume;

            lastItem.Ticks = b.Ticks;
        }
        */
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            /*
            if( tsi is Tick )
            {
                var t = (Tick)tsi; 
                var lastItem = ((Bar)Items[ilast]);

                if (lastItem.High < t.Last) lastItem.High = t.Last;
                if (lastItem.Low > t.Last) lastItem.Low = t.Last;

                lastItem.Close = t.Last;

                lastItem.Volume += t.Volume;
                lastItem.Ticks++;
                
            }
            */ 
            //else if ( tsi is IBar )
            //{
                var b = (IBarSimple) tsi;
                var lastItem = ((Bar)Items[ilast]);

                if (lastItem.High < b.High) lastItem.High = b.High;
                if (lastItem.Low > b.Low) lastItem.Low = b.Low;
                lastItem.Close = b.Close;

                lastItem.Volume += b.Volume;
                lastItem.Ticks += b.Ticks;
            //}
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var lastItem = ((Bar)Items[ilast]);
            if (tsi is Tick)
            {
                var t = (Tick)tsi;
            
                lastItem.Open = t.Last;
                lastItem.High = t.Last;
                lastItem.Low = t.Last;
                lastItem.Close = t.Last;

                lastItem.Volume = t.Volume;
                lastItem.Ticks = 1;
            }
            else if (tsi is IBar)
            {
                var b = (IBar)tsi;
            
                lastItem.Open = b.Open;
                lastItem.High = b.High;
                lastItem.Low = b.Low;
                lastItem.Close = b.Close;

                lastItem.Volume = b.Volume;

                lastItem.Ticks = b.Ticks;
            }
        }

        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            throw new NotImplementedException();
        }

        public override string Key
        {
            get
            {
                //return String.Format("[Type={0};Ticker={1};Code={2};Name={3};TimeIntSeconds={4};ShiftIntSecond={5}]",
                //                        GetType(),Ticker.Code, Code, Name,TimeIntSeconds, ShiftIntSecond);
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3}]",
                                        GetType(),Ticker.Key,TimeIntSeconds,ShiftIntSecond);
            }
        }
        /*
        public override string ToString()
        {
            return String.Format("[Type={0};Name={1};Ticker={2};TimeInt={3};ShiftTimeInt={4};Count={5}]",
                                 GetType(), Name, Ticker.Code, TimeIntSeconds, ShiftIntSecond, Count);
        }
        */
        public override string ToString()
        {
            return String.Format("[Type={0};Ticker={1};Code={2};Name={3};TimeIntSeconds={4};ShiftIntSecond={5};Count={6}]",
                                        GetType(), Ticker.Code, Code, Name,TimeIntSeconds, ShiftIntSecond, Count);
        }     
    }
}
