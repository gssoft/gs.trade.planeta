using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GS.Extension;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;
using GS.Trade.Data.Studies.GS;

namespace GS.Trade.Data.Studies
{
    public class Atr : TimeSeries
    {
        private float _minPrcntValue = 0.000125f;

        private readonly int _length;
        public int Length => _length;

        private readonly double _k;

        // private readonly Ticker _ticker;
        private Bars001 _bars;

        private Item _lastItem;

        public virtual double LastAtr => Count > 0
                        ? (((Atr.Item) LastItem)?.Atr ?? 0)
                        : 0;
        public virtual double LastAtrCompleted => Count > 1 
                        ? (((Atr.Item) LastItemCompleted)?.Atr ?? 0)
                        : 0;

        public override string Key => 
                        string.Format("Type={3};Ticker={0};TimeIntSeconds={1};Length={2}",
                                            Ticker.Code, TimeIntSeconds, _length, typeof(Atr));

        public Atr(string name, ITicker ticker, int timeIntSeconds, int length)
            : base(name, ticker, timeIntSeconds)
        {
            _length = length;
            _k = 2.0 / (_length + 1);
        }
        public override void Init()
        {
            if (SyncSeries != null) return;
            //_bars = Ticker.GetBarSeries("TypeName=Bar;TimeIntSeconds=" + TimeIntSeconds);
            SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            if (SyncSeries == null)
                throw new NullReferenceException("Atr Init Bar == null");
            _bars = SyncSeries as Bars001;
            if( _bars == null)
                throw new NullReferenceException("SyncSeries is Not IBars == null");
            // UpToDate();

            SetMinPrcnt();

        }
        public double AtrValue( double current, double previous)
        {
            return _k * current + (1.0 - _k) * previous;
        }
        public static double Value(int length, double current, double previous)
        {
            var k = 2.0 / (length + 1);
            return k * current + (1.0 - k) * previous;
        }
        public static double Calculate( int length, double current, double previous)
        {
            var k = 2.0/(length + 1);
            return k * current + (1.0 - k) * previous;
        }

        public void UpdateOld(DateTime dt)
        {
            if (IsUpToSyncTime(dt)) return;   // prevent double time calculation 
            if (_bars.Count < 1) return;
            // SyncDT = dt;                      // Importatnt don't touch  

            var barsTickCount = _bars.TickCount;
            if (barsTickCount <= TickCount) return;
            TickCount = barsTickCount;
         /*   
            if(TimeIntSeconds==1 && Count > 0)
                Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Atr",
                    "Sync: " + dt.ToString("H:mm:ss.fff"),
                    "Atr: " + this[0].DT.ToString("H:mm:ss.fff"),
                    "Bar: " + _bars.LastItem.DT.ToString("H:mm:ss.fff"));
         */   
            LastTickDT = _bars.LastTickDT;
            
            //var barDt = GetBarDateTime(barLastTickDt, TimeIntSeconds);
            var barDt = _bars.LastItem.DT;

            if (Items.Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastAtrDt = DtToLong(_lastItem.DT);

                if (nlastAtrDt >= nbardt)
                {
                    _lastItem.SyncDT = LastTickDT;
                    if (nlastAtrDt == nbardt)
                    {
                        if (Count > 1)
                            //_lastItem.Atr = _k * _bars.TrueRange(0) + (1.0 - _k) * ((Atr.Item)(Items[1])).Atr;
                            _lastItem.Atr = ((Item)(Items[1])).Atr;
                        else
                            _lastItem.Atr = _bars.TrueRange(0);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Atr", "Atr", "Update",
                                    String.Format("SyncTime={0:T}, LastBarTime={1:T} < LastTickTime={2:T}", dt, barDt, _lastItem.DT), "");
                    }
                }
                else
                {
                    if (Count > 1)
                    {
                        if (!_bars.IsFaded)
                            _lastItem.Atr = _k*_bars.TrueRange(1) + (1.0 - _k)*((Atr.Item) (Items[1])).Atr;
                        else
                            _lastItem.Atr = ((Item) (Items[1])).Atr;
                    }
                    //else
                    //   _lastItem.Atr = _bars.TrueRange(1);

                    double atr;
                    if (DtToLong(this[0].DT) < DtToLong(_bars[1].DT))
                    {
                        //Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Lost: " + dt.ToString("H:mm:ss.fff"),
                        //     "Atr: " + this[0].DT.ToString("H:mm:ss.fff") + " Bar: " + _bars[0].DT.ToString("H:mm:ss.fff"),
                        //            "Atr: " + _lastItem + " Bar: " + _bars[0]);
                        /*
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING,
                                        "Atr.Bar: " + _bars[0].DT.ToLongTimeString(), _lastItem.DT.ToLongTimeString(),
                                                            _lastItem.ToString(), _bars[0].ToString());
                        */
                        var lostItem = this[0];
                        var i = 0;
                        while (_bars[i].DT > _lastItem.DT) i++;
                        for (var j = i; j >= 0; j--)
                        {
                            //atr = AtrValue(_bars.TrueRange(j), LastAtrCompleted);
                            ((Item)LastItem).Atr = AtrValue(_bars.TrueRange(j), LastAtrCompleted);
                            LastItem.DT = _bars[j].DT;
                            LastItem.SyncDT = _bars[j].DT; 

                            //Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Do Sync",
                            //        LastItem.ToString(), _bars[j].ToString());
                            
                            if( j > 0)  AddItem(new Item());
                        }
                         _lastItem = LastItem as Item;

                         Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Atr.New", "Lost[" + i + "]: " + dt.ToString("H:mm:ss.fff"),
                              "Atr: " + lostItem.DT.ToString("H:mm:ss.fff") + " Bar[0]: " + _bars[0].DT.ToString("H:mm:ss.fff") + " Bar[1]: " + _bars[1].DT.ToString("H:mm:ss.fff"),
                                     "Atr: " + lostItem + " Bar: " + _bars[0]);


                         /*
                        while ( _lastItem.DT < barDt )
                        {
                            ((Item)LastItem).Atr = AtrValue(_bars.TrueRange(j), LastAtrCompleted);
                            LastItem.DT =_lastItem.SyncItem.DT;
                            LastItem.SyncDT = _lastItem.DT;

                            Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Do Sync",
                                    LastItem.ToString(), _lastItem.SyncItem.ToString());
                        }
                        */
                    }
                    else
                    {
                        /*
                        if( Count > 1)
                            evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "Atr.Item", "New", "Completed:" + _lastItem + " Prev:" + LastItemCompleted,
                            String.Format("BarDT={0:T} k={1:N3} TR={2:N3} Ticks={3}",
                                _bars[1].DT, _k, _bars.TrueRange(1), TickCount ));
                        */

                        //var atr = _k * _bars.TrueRange(0) + (1d - _k) * _lastItem.Atr;

                        atr = _lastItem.Atr; // previous
                        _lastItem = new Item(barDt, atr) {SyncDT = LastTickDT}; // current
                        AddItem(_lastItem);
                    }
                }
            }
            else
            {
                _lastItem = new Item(barDt, _bars.TrueRange(0) / 2) { SyncDT = LastTickDT };
                AddItem(_lastItem);
            }
        }
        /*
        public override void UpdateBar(DateTime dt)
        {
            if (IsUpToSyncTime(dt)) return;   // prevent double time calculation 
            if (_bars.Count < 1) return;
            // SyncDT = dt;                      // Importatnt don't touch  

            var barsTickCount = _bars.TickCount;
            if (barsTickCount <= TickCount) return;
            TickCount = barsTickCount;
           
            LastTickDT = _bars.LastTickDT;
            var barDt = _bars.LastItem.DT;

            if (Items.Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastAtrDt = DtToLong(_lastItem.DT);

                if (nlastAtrDt >= nbardt)
                {
                    _lastItem.SyncDT = LastTickDT;
                    if (nlastAtrDt == nbardt)
                    {
                        if (Count > 1)
                            if (NeedToCalcUnCompletedBar) Calculate2(0, 0, 1);
                        else
                            InitItem(0,0);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Atr", "Update",
                                    String.Format("SyncTime={0:T}, LastBarTime={1:T} < LastTickTime={2:T}", dt, barDt, _lastItem.DT), "");
                    }
                }
                else
                { // new Item Add
                    if (_bars.Count > 1 && DtToLong(this[0].DT) < DtToLong(_bars[1].DT))
                    {
                        UpToDate2(dt);
                    }
                    else
                    {
                        if (NeedToCalcUnCompletedBar)
                        {
                            _lastItem = new Item();
                            AddItem(_lastItem);
                            Calculate2(0, 0, 1);
                        }
                        else
                        {
                            Calculate2(1, 0, 1);
                            _lastItem = new Item();
                            AddItem(_lastItem);
                            CopyItem(0, 0, 1);
                        }
                    }
                }
            }
            else
            {
                _lastItem = new Item();
                AddItem(_lastItem);
                InitItem(0, 0);
            }
        }
        */

        public override void InitUpdate(DateTime dt)
        {
        }
        /*
        public void Calculate3(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            last.Atr = _k * ((Bars001)SyncSeries).TrueRange(isync) + (1.0 - _k) * prev.Atr;
        }
        */
        /*
        public override void Calculate2(int ibar, int ilast, int iprev)
        {
            if (Count > 1)
            {
                var last = ((Item)Items[ilast]);
                var prev = ((Item)Items[iprev]);

                last.DT = _bars[ibar].DT;
                last.SyncDT = _bars[ibar].LastDT;

                if (!_bars.Bar(ibar).IsFaded)
                    last.Atr = _k * _bars.TrueRange(ibar) + (1.0 - _k) * prev.Atr;
                else
                    last.Atr = prev.Atr;

                // FileEvl.AddItem(EvlResult.INFO, "Calc: " + ToString(), last.ToString());
            }
            else if (Count > 0)
                InitItem(ibar, 0);
        }
        public override void InitItem(int ibar, int ilast)
        {
            var bar = _bars[ibar];
            var lastItem = (Item)(Items[ilast]);

            lastItem.DT = bar.DT;
            lastItem.SyncDT = bar.LastDT;

            lastItem.Atr = _bars.TrueRange(ibar) / 2;

            // FileEvl.AddItem(EvlResult.INFO, "Init: " + ToString(), lastItem.ToString());
        }
        public override void CopyItem(int ibar, int ilast, int iprev)
        {
            if (Count < 2) return;

            var bar = _bars[ibar];
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            last.DT = bar.DT;
            last.SyncDT = bar.LastDT;

            last.Atr = prev.Atr;
        }
        */
        public override void AddNewItem()
        {
            AddItem(new Item());
        }

        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var bars = (IBars)SyncSeries;
            AddItem(
                new Item
                {
                    DT = itemDt,
                    LastDT = tsi.LastDT,
                    SyncDT = syncDt,

                    Atr = iprev == 0 ? bars.TrueRange(isync) / 2 : ((Item)(Items[ilast])).Atr
                }
                );
        }

        public void UpdateOld(Bar b)
        {
            if (_bars.Count < 1) return;

            var barLastTickDt = b.LastDT;
            LastTickDT = barLastTickDt;

            var barDt = GetBarDateTime2(b.DT, TimeIntSeconds);

            if (Items.Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastAtrDt = DtToLong(_lastItem.DT);

                if (nlastAtrDt >= nbardt)
                {
                    _lastItem.SyncDT = b.LastDT;
                    if (nlastAtrDt == nbardt)
                    {
                        if (Count > 1)
                            _lastItem.Atr = ((Atr.Item)(Items[1])).Atr;
                            //_lastItem.Atr = _k * _bars.TrueRange(0) + (1.0 - _k) * ((Atr.Item)(Items[1])).Atr;
                        else
                            _lastItem.Atr = _bars.TrueRange(0);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Atr", "Atr", "Update",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", _lastItem.DT, b.DT), "");
                    }
                }
                else
                {
                    if (Count > 1)
                    //_lastItem.Atr = _k * _bars.TrueRange(1) + (1.0 - _k) * ((Atr.Item)(Items[1])).Atr;
                    {
                        if (!_bars.IsFaded)
                            _lastItem.Atr = _k*_bars.TrueRange(1) + (1.0 - _k)*((Atr.Item) (Items[1])).Atr;
                        else
                            _lastItem.Atr = ((Item) (Items[1])).Atr;
                    }
                    //else
                    //   _lastItem.Atr = _bars.TrueRange(1);

                    if (DtToLong(_lastItem.DT) != DtToLong(_bars[1].DT))
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Atr.New", "Sync is Lost",
                                    _lastItem.ToString(), _bars[1].ToString());
                    /*
                    if( Count > 1)
                        evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "Atr.Item", "New", "Completed:" + _lastItem + " Prev:" + LastItemCompleted,
                        String.Format("BarDT={0:T} k={1:N3} TR={2:N3} Ticks={3}",
                            _bars[1].DT, _k, _bars.TrueRange(1), TickCount ));
                    */
                    // var atr = _k * _bars.TrueRange(0) + (1 - _k) * _lastItem.Atr;
                    var atr = _lastItem.Atr;
                    _lastItem = new Item(barDt, atr) { SyncDT = LastTickDT };
                    AddItem(_lastItem);
                }
            }
            else
            {
                _lastItem = new Item(barDt, _bars.TrueRange(0) / 2) { SyncDT = LastTickDT };
                AddItem(_lastItem);
            }
        }

        // TimeSeries UpdateItem(TimeSeries tsi)
        /*
        public override void PreUpdate(TimeSeriesItem tsi)
        {
        }
        public override void Calculate(TimeSeriesItem tsi)
        {
            if (Count > 1)
            {
                var b = (Bar) tsi;
                var last = (Item) LastItem;
                var prev = (Item) LastItemCompleted;
                var bars = (Bars001) SyncSeries;

                last.Atr = _k*bars.TrueRange(0) + (1.0 - _k)*prev.Atr;
            }
            else if (Count > 0)
                InitItem(tsi);
        }
        public override void InitItem(TimeSeriesItem tsi)
        {
            var b = (Bar)tsi;
            var last = (Item)LastItem;
            var bars = (Bars001)SyncSeries;

            var syncDt = GetBarDateTime2(tsi.DT, TimeIntSeconds);
            last.DT = syncDt;
            last.LastDT = syncDt;
            last.SyncDT = syncDt;

            last.Atr = bars.TrueRange(0) / 2;
        }
        */
        /*
        public override void Calculate(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            if (Count > 1)
            {
                Bar b;
                var bars = (Bars001)SyncSeries;
                if (tsi != null)
                {
                    isync = 0;
                    ilast = 0;
                    iprev = 1;
                    b = (Bar)tsi;
                }
                else
                    b = (Bar)bars[isync];
                Calculate3(b,isync,ilast,iprev);
            }
            else if (Count > 0)
                InitItem(tsi, isync, ilast);
        }
        */
        

        /*
        public override void InitItem(TimeSeriesItem tsi, int isync, int ilast)
        {
            DateTime syncDt;
            var bars = (Bars001)SyncSeries;
            var b = (Bar) bars[isync];

            if (tsi != null)
            {
                isync = 0;
                ilast = 0;
            }
            InitItem3(tsi, isync, ilast);
        }
        */
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            // last.Atr = _k * ((IBars)SyncSeries).TrueRange(isync) + (1.0 - _k) * prev.Atr;
           // if ( !((IBars) SyncSeries).IsNoValid01(isync) && !((IBars) SyncSeries).IsNoValid02(isync))
            if (!((IBars)SyncSeries).IsDoj)
            {
                //var atr = _k*((IBars) SyncSeries).TrueRange(isync) + (1.0 - _k)*prev.Atr;
                //var atrMinValue = ((IBars) SyncSeries).GetMedian(isync)*_minPrcntValue;
                //last.Atr = atr.IsGreaterThan(atrMinValue) ? atr : atrMinValue;

                last.Atr = _k * ((IBars)SyncSeries).TrueRange(isync) + (1.0 - _k) * prev.Atr;
            }
            else
            {
                var newAtr = _k * ((IBars)SyncSeries).TrueRange(isync) + (1.0 - _k) * prev.Atr;
                last.Atr = newAtr.IsGreaterThan(prev.Atr) ? newAtr : prev.Atr;
                //last.Atr = prev.Atr;
            }

        } 
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var bars = (IBars)SyncSeries;
            var last = (Item)(Items[ilast]);

            last.Atr = bars.TrueRange(isync) / 2;
        }
        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            last.Atr = prev.Atr;
        }

        // ChartSupport
        public double GetAtr(int i)
        {
            return ((Item)this[i]).Atr;
        }

        private IList<ILineSeries> _chartLines;
        public override IList<ILineSeries> ChartLines => _chartLines ?? CreateChartLines();

        private IList<ILineSeries> CreateChartLines()
        {
            var chl = new List<ILineSeries>
                       {
                           new ChartLine
                               {
                                   Name = $"Atr({_length},0)",
                                   Color = 0xff0000,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetAtr
                               }
                       };
            _chartLines = chl;
            return _chartLines;
        }
       
        public override string ToString()
        {
            return String.Format("Type={0};Name={1},Ticker={2},TimeInt={3},Length={4};ItemsCount={5}",
                GetType(),Name,Ticker.Code, TimeIntSeconds, _length, Items.Count);
        }
        /*
        public void UpToDate3()
        {
            if ( _bars.Count == 0 ) return;
            if ( _bars.Count <= Count) return;

            Items.Clear();
            
            _lastItem = new Item( _bars.FirstItem.DT, _bars.TrueRange( _bars.Count - 1 ) / 2);
            AddItem( _lastItem );

            for (var i = _bars.Count-2; i >=0; i-- )
            {
                var atr = _k*_bars.TrueRange(i) + (1.0 - _k) * ((Item)Items[0]).Atr;
                //var atr = AtrValue(_bars.TrueRange(i), ((Item)Items[0]).Atr);
                _lastItem = new Item( _bars[i].DT, atr);
                AddItem(_lastItem);
            }
            Ticker.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Atr", "UpToDate", ToString(), "Count=" + _bars.Count);
        }
         */
        /*
        public void UpToDate2(DateTime dt)
        {
            var lostItem = LastItem;
            var i = 0;
            while (_bars[i].DT > LastItem.DT) i++;
            for (var j = i; j >= 0; j--)
            {
                Calculate2(j,0,1);
                //Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Do Sync",
                //        LastItem.ToString(), _bars[j].ToString());
                if (j > 0) AddItem(new Item());
            }
            _lastItem = LastItem as Item;

            Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Lost[" + i + "]: " + dt.ToString("H:mm:ss.fff"),
                 "Atr: " + lostItem.DT.ToString("H:mm:ss.fff") + " Bar[0]: " + _bars[0].DT.ToString("H:mm:ss.fff") + " Bar[1]: " + _bars[1].DT.ToString("H:mm:ss.fff"),
                        "Atr: " + lostItem + " Bar: " + _bars[0]);
        }
        */

        private void SetMinPrcnt()
        {
            float a;
            if( TimeIntSeconds >= 15)
            {
                a = (float)Math.Sqrt(TimeIntSeconds / 15f);
            }
            else
            {
                a = (float) (1f / Math.Sqrt( 15f / TimeIntSeconds ));
            }
            _minPrcntValue = _minPrcntValue * a;
        }

        public class Item : TimeSeriesItem
        {
            public double Atr { get; set; }
            public Item(){}
            public Item(DateTime dt, double last)
            {
                DT = dt;
                Atr = last;
            }
            public override string ToString()
            {
                return $"[Type={GetType()};DT={DT:T};Atr={Atr:N3};SyncTime={SyncDT:T}]";
            }
        }
        public class ItemNotify : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private DateTime _syncDT;
            public DateTime SyncDT
            {
                get { return _syncDT; }
                set
                {
                    _syncDT = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SyncTimeString"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("Atr"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("AtrCompleted"));
                }
            }
            private DateTime _dt;
            public DateTime DT
            {
                get { return _dt; }
                set
                {
                    _dt = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("TimeString"));
                }
            }

            private double _atr;
            public double Atr
            {
                get { return _atr; }
                set
                {
                    _atr = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("AtrString"));
                }
            }
            private double _atrCompleted;
            public double AtrCompleted
            {
                get { return _atrCompleted; }
                set 
                {
                    _atrCompleted = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("AtrCompletedString"));
                }
            }
            public string TimeString
            {
                get { return _dt.ToString("T"); }
            }
            public string SyncTimeString
            {
                get { return _syncDT.ToString("T"); }
            }
            public string AtrString { get { return _atr.ToString("N3"); } }
            public string AtrCompletedString { get { return AtrCompleted.ToString("N3"); } }
           
            public ItemNotify()
            {}

            public void OnPropertyChanged(PropertyChangedEventArgs e)
            {
                if (PropertyChanged != null) PropertyChanged(this, e);
            }
        }
    }
}
