using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;
using GS.Trade.Data.Studies.Averages;


namespace GS.Trade.Data.Studies.GS
{
    public class Cci : TimeSeries //, IBandSeries
    {
        private readonly int _xMaLength;
        private readonly int _xMaSmoothLength;
        private readonly float _kDev;

        private IBars _bars;
        private XAverage _xAverage;

      //  private Item _lastItem;

        public Cci(string name, Ticker ticker, int timeIntSeconds, int xMaLength, int xMaSmoothLength, float kDev)
            : base(name, ticker, timeIntSeconds)
        {
            _xMaLength = xMaLength;
            _xMaSmoothLength = xMaSmoothLength;
            _kDev = kDev;
        }

        public override void Init()
        {
            if (SyncSeries != null) return;
            _xAverage =
                Ticker.RegisterTimeSeries(new XAverage("xAvg", Ticker, TimeIntSeconds, BarValue.Median, _xMaLength, 3))
                as XAverage;
            //SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            if (_xAverage != null)
                SyncSeries = _xAverage.SyncSeries;
            if (SyncSeries == null || _xAverage == null)
                throw new NullReferenceException("Init Bar == null or xAverage == null");

            _bars = SyncSeries as IBars;

            DebudAddITem = true;

            // UpToDate();
        }
        /*
        public override void Update(DateTime sync)
        {
            if (IsUpToSyncTime(sync)) return; // If this Series already UpDate for this Sync
            if (_bars.Count < 1 || _xAverage.Count < 1) return;
            if (_bars.TickCount <= TickCount) return;
            TickCount = _bars.TickCount;
            LastTickDT = _bars.LastTickDT;

            _xAverage.Update(sync);
            
            if (Items.Count > 0)
            {
                var lastBarDt = _bars.LastItem.DT;
                var lastItemDt = Items[0].DT;

                var nLastBarDt = DtToLong(lastBarDt);
                var nlastItemDt = DtToLong(lastItemDt);

                if (nlastItemDt >= nLastBarDt)
                {
                    Items[0].SyncDT = LastTickDT;

                    if (nlastItemDt == nLastBarDt)
                    {
                        Calculate(0, 0, Count > 1 ? 1 : 0);
                    }
                }
                else // New Item Should Add
                {
                        if (Count > 1 && DtToLong(lastItemDt) < DtToLong(_bars[1].DT))
                        {
                            var lostItem = this[0];
                            var i = 0;
                            while (_bars[i].DT > lastItemDt) i++; 
                            for (var j = i; j >= 0; j--)
                            {
                                Calculate(j, 0, 1);
                                LastItem.DT = _bars[j].DT;
                                LastItem.SyncDT = _bars[j].DT;

                                if (j > 0) AddItem(new Item());
                            }
                            // _lastItem = LastItem as Item;

                            Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Cci.New", "Lost[" + i + "]: " + sync.ToString("H:mm:ss.fff"),
                                 "Cci: " + lostItem.DT.ToString("H:mm:ss.fff") + " Bar[0]: " + _bars[0].DT.ToString("H:mm:ss.fff") + " Bar[1]: " + _bars[1].DT.ToString("H:mm:ss.fff"),
                                        "Cci: " + lostItem + " Bar: " + _bars[0]);
                        }
                        else
                        {
                            AddItem(new Item
                                            {   DT = lastBarDt,
                                                DAvg = ((Item)Items[0]).DAvg,
                                                Cci = ((Item)Items[0]).Cci,
                                                SyncDT = LastTickDT
                                            });
                        }
                }
            }
            else
            {
                //_lastItem = new Item { SyncDT = LastTickDT };
                AddItem(new Item { SyncDT = LastTickDT });
                InitItem(0, 0);

            }
        }
        */

        public override void InitUpdate(DateTime dt)
        {
            _xAverage.Update(dt);
        }
        /*
        public override void InitItem(int ibar, int ilast)
        {
            var bar = _bars.Bar(ibar);
            var lastItem = (Item)(Items[ilast]);

            lastItem.DT = bar.DT;
            lastItem.SyncDT = bar.LastDT;

            lastItem.DAvg = 0.0f;
            lastItem.Cci = 0.0f;
        }
        public override void Calculate2(int ibar, int ilast, int iprev)
        {
            if (Count > 1)
            {
                var bar = (IBar)(_bars.Bar(ibar));
                var last = ((Item) Items[ilast]);
                var prev = ((Item) Items[iprev]);

                last.LastDT = bar.LastDT;
                last.SyncDT = bar.SyncDT;

                var avg = ((XAverage.Item) (_xAverage[ibar])).Ma;
                var d = _kDev*(float) Math.Max(Math.Abs(bar.High - avg), Math.Abs(bar.Low - avg));
                last.DAvg = XAverage.Calculate(_xMaSmoothLength, d, prev.DAvg);
                
                if ( last.DAvg.CompareTo(0.0f) != 0)
                    last.Cci = (float) (100*(bar.Close - avg) / last.DAvg);
                else
                    last.Cci = 0.0f;
            }
            else if (Count > 0)
                InitItem(ibar, 0);

            // ((Item) (Items[last])).Cci = (float) ccidbl;
        }
        public override void CopyItem(int ibar, int ilast, int iprev)
        {
            if (Count < 2) return;

            var bar = _bars.Bar(ibar);
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            last.DT = bar.DT;
            last.SyncDT = bar.LastDT;

            last.DAvg = prev.DAvg;
            last.Cci = prev.Cci;
        }
        */
        public override void AddNewItem()
        {
            AddItem(new Item());
        }
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            AddItem(
                new Item
                {
                    DT = itemDt,
                    LastDT = tsi.LastDT,
                    SyncDT = syncDt,

                    DAvg = iprev > 0 ? ((Item)(Items[ilast])).DAvg : 0f,
                    Cci = iprev > 0 ? ((Item)(Items[ilast])).Cci : 0f
                }
                );
        }

        // TimeSeries UpdateItem(TimeSeries tsi)
        /*
        public override void PreUpdate(TimeSeriesItem tsi)
        {
            _xAverage.UpdateItem(tsi);
        }
        public override void Calculate(TimeSeriesItem tsi)
        {
            if (Count > 1)
            {
                var b = (Bar)tsi;
                var last = (Item)LastItem;
                var prev = (Item)LastItemCompleted;

                last.LastDT = b.DT;
                last.SyncDT = b.DT;

                var avg = ((XAverage.Item)(_xAverage[0])).Ma;
                var d = _kDev * (float)Math.Max(Math.Abs(b.High - avg), Math.Abs(b.Low - avg));
                last.DAvg = XAverage.Calculate(_xMaSmoothLength, d, prev.DAvg);

                if (last.DAvg.CompareTo(0.0f) != 0)
                    last.Cci = (float)(100 * (b.Close - avg) / last.DAvg);
                else
                    last.Cci = 0.0f;
            }
            else if (Count > 0)
                InitItem(tsi);
        }
        public override void InitItem(TimeSeriesItem tsi)
        {
            var b = (Bar)tsi;
            var last = (Item)LastItem;

            last.DT = b.DT;
            last.LastDT = b.DT;
            last.SyncDT = b.DT;

            last.DAvg = 0.0f;
            last.Cci = 0.0f;
        }
        */
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)SyncSeries[isync];
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            var avg = ((Average.Item)(_xAverage[isync])).Ma;
            var d = _kDev * Math.Max(Math.Abs((float)b.High - avg), Math.Abs((float)b.Low - avg));
            last.DAvg = XAverage.Calculate(_xMaSmoothLength, d, prev.DAvg);

            if (last.DAvg.CompareTo(0.0f) != 0)
                last.Cci = (float)(100 * (b.Close - avg) / last.DAvg);
            else
                last.Cci = 0.0f;
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var last = (Item)(Items[ilast]);

            last.DAvg = 0.0f;
            last.Cci = 0.0f;
        }
        public override void CopyItem(TimeSeriesItem tsi, int ibar, int ilast, int iprev)
        {
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            last.DAvg = prev.DAvg;
            last.Cci = prev.Cci;
        }

        public override void InitUpToDate() { _xAverage.UpToDate(); }

        public override string ToString()
        {
            return String.Format("Type={0};Ticker={1};TimeIntSeconds={2};xMaLength={3};xMaSmoothLength={4}",
                  GetType(), Ticker.Code, TimeIntSeconds, _xMaLength, _xMaSmoothLength);
        }
        public override string Key
        {
            get
            {
                return String.Format("Type={0};Ticker={1};TimeIntSeconds={2};xMaLength={3};xMaSmoothLength={4}",
                  GetType(), Ticker.Code, TimeIntSeconds, _xMaLength, _xMaSmoothLength);
            }
        }
        
        // Chart Support
        // ILineSeries
        public double GetMa(int i)
        {
            return ((Average.Item) _xAverage[i]).Ma;
        }

        // IBandSeries
        public double GetHigh(int i)
        {
            return ((Average.Item)_xAverage[i]).Ma +((Item)Items[i]).DAvg;
        }
        public double GetLow(int i)
        {
            return ((Average.Item)_xAverage[i]).Ma - ((Item)Items[i]).DAvg;
        }
       
        public override IList<ILineSeries> ChartLines { get { return CreateChartLineList(); } }
        private IList<ILineSeries> CreateChartLineList()
        {
            return new List<ILineSeries>
                       {
                           new ChartLine
                               {
                                   Name = "L.Cci",
                                   Color = 0x00ff00,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa
                               }
                       };
        }

        public override IList<IBandSeries> ChartBands { get { return CreateChartBandList(); } }
        private IList<IBandSeries> CreateChartBandList()
        {
            return new List<IBandSeries>
                       {
                           new ChartBand
                               {
                                   BandName="B.Cci",
                                   BandColor = 0xff3333,
                                   BandLineColor = 0xff0000,
                                   BandFillColor = unchecked((int)0xc0ff1111),
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa,
                                   CallGetHigh = GetHigh,
                                   CallGetLow = GetLow
                               }
                       };
        }

        // *****************************************************************
        public class Item : TimeSeriesItem
        {
            public float DAvg { get; set; }
            public float Cci { get; set; }

            public override string ToString()
            {
                return String.Format("DT={0:HH:mm:ss:fff};Cci={1:N3};Sync={2:HH:mm:ss:fff}", DT, Cci,  SyncDT);
            }
        }

    }
}
