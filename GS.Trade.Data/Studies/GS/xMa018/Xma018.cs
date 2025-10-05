using System;
using System.Linq;
using System.Windows.Forms;
using GS.Extension;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Triggers;

namespace GS.Trade.Data.Studies.GS.xMa018
{
    public partial class Xma018 : TimeSeries,   ILevelCollection
    {
        private readonly int _maLength;
    //    private readonly int _atrLength;

        private readonly int _atrLength1;
        //private readonly int _atrLength2 = 350;
        private readonly int _atrLength2;

        private float _kAtr;
        private readonly int _xMode=1;

        private IBars _bars;
        private Atr _atrs;
        private Atr _atrs2;
        private Atr _atrs3;
        private double _trHigh5;
        private double _trLow5; // = double.MaxValue;
        //private TimeSeries _atrs;

        public int BarsDailyCount => SyncSeries?.ItemsDailyCount ?? 0;

        public bool FirstSwingAfterBrDown { get; private set; }
        public bool FirstSwingAfterBrUp { get; private set; }

        public bool IsTrend55Up => Trend55?.Value > 0;
        public bool IsTrend55Down => Trend55?.Value < 0;

        public bool IsTrend55HighBreakUp => IsTrend55Up && Ma.IsGreaterThan(TrHigh1);

        public bool IsTrend55LowBreakDown => IsTrend55Down && Ma.IsLessThan(TrLow1);

        public Trigger<int> Trend55;

        public double Ma => Count > 1 ? ((Item)LastItemCompleted).Ma : 0.0;
        public double High => Count > 1 ? ((Item)LastItemCompleted).High : double.MaxValue;
        public double Low => Count > 1 ? ((Item)LastItemCompleted).Low : 0.0;

        public double PrevMa => Count > 2 ? ((Item)PrevItemCompleted).Ma : 0.0;
        public double PrevHigh => Count > 2 ? ((Item)PrevItemCompleted).High : double.MaxValue;
        public double PrevLow => Count > 2 ? ((Item)PrevItemCompleted).Low : 0.0;

        public double High2
        {
            get { return Count > 1 ? ((Item)LastItemCompleted).High2 : double.MaxValue; }
        }
        public double Low2
        {
            get { return Count > 1 ? ((Item)LastItemCompleted).Low2 : 0; }
        }
        public double High3
        {
            get { return Count > 1 ? ((Item)LastItemCompleted).High3 : double.MaxValue; }
        }
        public double Low3
        {
            get { return Count > 1 ? ((Item)LastItemCompleted).Low3 : 0; }
        }
        public int Impulse => Count > 1 ? ((Item)LastItemCompleted).Impulse : 0;
        public bool IsImpulse => Impulse == 1;
        public bool IsFlat => Impulse == 0;

        public bool IsImpulseUp => IsImpulse && IsUp;
        public bool IsImpulseDown => IsImpulse && IsDown;

        public int Trend
        {
            get { return Count > 1 ? ((Item) LastItemCompleted).Trend : 0; }
        }
        public bool IsUp => Trend > 0;
        public bool IsDown => Trend < 0;

        public bool BreakUp => IsUp && Ma.IsGreaterThan(High2);

        public bool BreakDown => IsDown && Ma.IsLessThan(Low2);

        public bool BreakUp5 {
            get { return TrHigh1.IsNoEquals(0d) && Ma.IsGreaterThan(TrHigh1); }
        }
        public bool BreakDown5 {
            get { return TrLow1.IsNoEquals(0d) && Ma.IsLessThan(TrLow1); }
        }
        public int Trend5 { get; private set; }

        public bool Trend55Changed { get; private set; }

        public int TrendHigh5 => 
            TrHigh1.IsGreaterThan(TrHigh2)
            ? +1
            : (TrHigh1.IsLessThan(TrHigh2)
                ? -1
                : 0);

        public int TrendLow5 => 
            TrLow1.IsLessThan(TrLow2)
            ? -1
            : ( TrLow1.IsGreaterThan(TrLow2)
                ? +1
                : 0);

        public double TrHigh1 { get; private set; }
        public double TrLow1 { get; private set; }
        public double TrHigh2 { get; private set; }
        public double TrLow2 { get; private set; }
        public double TrHigh12 {
            get { return TrHigh1.IsGreaterThan(TrHigh2) ? TrHigh1 : TrHigh2; }
        }
        public double TrLow12
        {
            get { return TrLow1.IsLessThan(TrLow2) ? TrLow1 : TrLow2; }
        }

        public double TrPos1 {
            get
            {
                return TrHigh1.IsNoEquals(0.0) && TrLow1.IsNoEquals(0.0) && TrHigh1.IsNoEquals(TrLow1)
                    ? (Ma - TrLow1)/(TrHigh1 - TrLow1)*100d
                    : 0d;
            }
        }
        public double TrPos2 => 
            TrHigh2.IsNoEquals(0.0) && TrLow2.IsNoEquals(0.0) && TrHigh2.IsNoEquals(TrLow2)
                ? (Ma - TrLow2) / (TrHigh2 - TrLow2) * 100d
                : 0d;

        public double TrPos12 => 
            TrHigh12.IsNoEquals(0.0) && TrLow12.IsNoEquals(0.0) && TrHigh12.IsNoEquals(TrLow12)
                ? (Ma - TrLow12) / (TrHigh12 - TrLow12) * 100d
                : 0d;

        public int FlatCount { get; private set; }

        public bool Glue { get; private set; }
        public bool PrevGlue { get; private set; }
        /*
        public IBars Bars
        {
            get { return _bars; }
        }
        */
        public double Trend2PrevHigh { get; private set; }
        public double Trend2PrevLow { get; private set; }
        public Atr Atr => _atrs3;

        public Atr Atr1 => _atrs;
        public Atr Atr2 => _atrs2;

        public double AtrValue => _atrs2.LastAtrCompleted < _atrs.LastAtrCompleted 
            ? _atrs.LastAtrCompleted 
            : _atrs2.LastAtrCompleted;

        public int AtrDiff =>
            _atrs.LastAtrCompleted.IsGreaterThan(_atrs2.LastAtrCompleted)
            ? +1
            : -1;

        public bool IsAtrValueEnough =>
            _atrs.LastAtrCompleted.IsGreaterThan(_atrs2.LastAtrCompleted);

        /*
        public int TrendChanged
        {
            get { return Count > 2 ? ((Item)Items[1]).Trend != ((Item)Items[2]).Trend ? ((Item)Items[1]).Trend : 0 : 0; }
        }
        */
        public float VolatilityUnit { get; private set; }

        // 2018.08.07
        public bool TrendChanged => Count > 2 && ((Item)Items[1]).Trend != ((Item)Items[2]).Trend;
        //public bool TrendChanged => Count > 2 && 
        //            ((Item)Items[1]).Trend != ((Item)Items[2]).Trend &&
        //            ((Item)Items[1]).DT.Date == ((Item)Items[2]).DT.Date &&
        //            ((Item)Items[2]).DT.TimeOfDay > new TimeSpan(10,0,0) 
        //            // && ((Item)Items[1]).DT.TimeOfDay > new TimeSpan(10, 0, 0)
        //    ;
        public DateTime CurrentDateTime => LastItemDT;

        public Xma018(string name, ITicker ticker, int timeIntSeconds, int malength, int atrlength, float katr, int xMode)
            : base("Xma018", name, ticker, timeIntSeconds)
        {
            _maLength = malength;
            _atrLength1 = atrlength;
            _kAtr = katr;
            _xMode = xMode;
        }
        public Xma018(string name, ITicker ticker, int timeIntSeconds, int malength, int atrlength1, int atrlength2, float katr, int xMode)
            : base("Xma018", name, ticker, timeIntSeconds)
        {
            _maLength = malength;
            _atrLength1 = atrlength1;
            _atrLength2 = atrlength2;
            _kAtr = katr;
            _xMode = xMode;
        }
        public override void Init()
        {
            Trend55 = new TriggerInt();
            Trend55.Reset();

            if (SyncSeries != null) return;
            //_bars = Ticker.GetBarSeries("TypeName=Bar;TimeIntSeconds=" + TimeIntSeconds);
            SyncSeries = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));
            // _bars = Ticker.GetBarSeries("TypeName=Bar;TimeIntSeconds=15");
         //   if( _atrLength != 0)
         //       _atrs = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, TimeIntSeconds, _atrLength1));
         //       _atrs2 = (Atr)Ticker.RegisterTimeSeries(new Atr("BaseAtr", Ticker, TimeIntSeconds, _atrLength2));

                _atrs3 = (Atr)Ticker.RegisterTimeSeries(new Atr2("Atr2", Ticker, TimeIntSeconds, _atrLength1, _atrLength2));

            

          //  else if (_atrLength1 != 0 && _atrLength2 != 0)
              //  _atrs = (Atr2)Ticker.RegisterTimeSeries(new Atr2("MyAtr2", Ticker, TimeIntSeconds, _atrLength1, _atrLength2));
          //      _atrs = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr", Ticker, TimeIntSeconds, _atrLength));

            if (SyncSeries == null || _atrs3 == null)
                throw new NullReferenceException("Atr Init Bar == null or Atr == null");
            //_atrs.Init();
            _bars = SyncSeries as IBars;
            //  UpToDate();
        }
        public void SetKAtr(float katr)
        {
            _kAtr = katr;
        }

        public float KAtr => _kAtr;
        /*
          public  void UpdateOld(DateTime dt)
          {
              if (IsUpToSyncTime(dt)) return;  // If this Series already UpDate for this Sync
              _atrs.Update(dt);
              if (_bars.Count < 1 || _atrs.Count < 1) return;
              SyncDT = dt;

              if (_bars.TickCount <= TickCount) return;
              TickCount = _bars.TickCount;
              LastTickDT = _bars.LastTickDT;

              var barDt = _bars.LastItem.DT;

              if (Items.Count > 0)
              {
                  var nbardt = DtToLong(barDt);
                  var nlastdt = DtToLong(_lastItem.DT);

                  if (nlastdt >= nbardt)
                  {
                      _lastItem.SyncDT = LastTickDT;
                      if (nlastdt == nbardt)
                      {
                          if (Count > 1)
                          {
                              // Calculate();
                              _lastItem.Ma = ((Item)LastItemCompleted).Ma;
                              _lastItem.Trend = ((Item)LastItemCompleted).Trend;
                              _lastItem.Impulse = ((Item)LastItemCompleted).Impulse;
                              _lastItem.High = ((Item)LastItemCompleted).High;
                              _lastItem.Low = ((Item)LastItemCompleted).Low;
                              _lastItem.High2 = ((Item)LastItemCompleted).High2;
                              _lastItem.Low2 = ((Item)LastItemCompleted).Low2;
                              _lastItem.High3 = ((Item)LastItemCompleted).High3;
                              _lastItem.Low3 = ((Item)LastItemCompleted).Low3;
                          }
                          else
                          {
                              // Items.Count == 1

                              _lastItem.Ma = _bars[0].TypicalPrice;
                              _lastItem.Trend = +1;
                              _lastItem.Impulse = 0;
                              _lastItem.High = _lastItem.Ma + _kAtr * ((Atr.Item)_atrs[0]).Atr;
                              _lastItem.Low = _lastItem.Ma - _kAtr * ((Atr.Item)_atrs[0]).Atr;
                              _lastItem.High2 = _lastItem.High;
                              _lastItem.Low2 = _lastItem.Low;
                              _lastItem.High3 = _lastItem.High;
                              _lastItem.Low3 = _lastItem.Low;
                          }
                      }
                      else
                      {
                          Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Xma018", "Update",
                                      String.Format("SyncTime={0:T}, LastBarTime={1:T} < LastTickTime={2:T}", dt, barDt, _lastItem.DT), "");
                      }
                  }
                  else // New Item Should Add
                  {
                      if (Count > 1)
                          //Calculate(1);
                          Calculate2(1,0,1);
                      else
                      {

                          // _lastItem.Ma = _bars[1].TypicalPrice;
                          // _lastItem.Trend = +1;
                          // _lastItem.Impulse = 0;
                          // _lastItem.High = _lastItem.Ma + _kAtr * ((Atr.Item)_atrs[1]).Atr; // _atrs[1] !!!!!!!!!!!!!! or [0] - ???
                          // _lastItem.Low = _lastItem.Ma - _kAtr * ((Atr.Item)_atrs[1]).Atr;

                      }

                      if (DtToLong(_lastItem.DT) < DtToLong(_bars[1].DT))
                      {
                        //  Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Xma018.New", "Sync is Lost",
                        //              _lastItem.ToString(), _bars[1].ToString());

                          var lostItem = this[0];
                          var i = 0;
                          while (_bars[i].DT > _lastItem.DT) i++;
                          for (var j = i; j >= 0; j--)
                          {
                              Calculate2(j, 0, 1);
                              LastItem.DT = _bars[j].DT;
                              LastItem.SyncDT = _bars[j].DT;

                              if (j > 0) AddItem(new Item());

                              SetTrend2();
                          }
                          _lastItem = LastItem as Item;

                          //SetTrend2();

                          Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Xma018", "Lost[" + i + "]: " + dt.ToString("H:mm:ss.fff"),
                               "XMa018: " + lostItem.DT.ToString("H:mm:ss.fff") + " Bar[0]: " + _bars[0].DT.ToString("H:mm:ss.fff") + " Bar[1]: " + _bars[1].DT.ToString("H:mm:ss.fff"), "");
                      }
                      else
                      {

                          //if (Count > 1)
                          //    evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "Xma018.Item", "New", "Completed:" + _lastItem + " Prev:" + LastItemCompleted,
                          //    String.Format("BarDT={0:T} k={1:N3} TR={2:N3} Ticks={3}",
                          //        _bars[1].DT, _k, _bars.TrueRange(1), TickCount));

                          var ma = _lastItem.Ma;
                          var tr = _lastItem.Trend;
                          var imp = _lastItem.Impulse;
                          var hi = _lastItem.High;
                          var lo = _lastItem.Low;
                          var h2 = _lastItem.High2;
                          var l2 = _lastItem.Low2;
                          var h3 = _lastItem.High3;
                          var l3 = _lastItem.Low3;

                          _lastItem = new Item(barDt, ma, tr, imp, hi, lo)
                                          {High2 = h2, Low2 = l2, High3 = h3, Low3 = l3, SyncDT = LastTickDT};
                          AddItem(_lastItem);

                          SetTrend2();
                      }
                  }
              }
              else
              {   // FirstBar In Series
                  var ma = _bars[0].TypicalPrice;
                  var atr = _kAtr * ((Atr.Item)_atrs[0]).Atr;
                  _lastItem = new Item(barDt, ma, +1, 0, ma + atr, ma - atr) { High2 = ma + atr, Low2 = ma - atr, SyncDT = LastTickDT };
                  AddItem(_lastItem);
              }
          }
         */

        /*
        public override void UpdateBar(DateTime dt)
        {
            if (IsUpToSyncTime(dt)) return;  // If this Series already UpDate for this Sync
            _atrs.UpdateBar(dt);
            if (_bars.Count < 1 || _atrs.Count < 1) return;
            
            if (_bars.TickCount <= TickCount) return;
            TickCount = _bars.TickCount;

            LastTickDT = _bars.LastTickDT;

            var barDt = _bars.LastItem.DT;

            if (Items.Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastdt = DtToLong(_lastItem.DT);

                if (nlastdt >= nbardt)
                {
                    _lastItem.SyncDT = LastTickDT;
                    if (nlastdt == nbardt)
                    {
                        if (Count > 1)
                        {
                            if (NeedToCalcUnCompletedBar) Calculate2(0, 0, 1);
                        }
                        else
                        {
                            InitItem(0, 0); // Items.Count == 1
                        }
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Xma018", "Update",
                                    String.Format("SyncTime={0:T}, LastBarTime={1:T} < LastTickTime={2:T}", dt, barDt, _lastItem.DT), "");
                        throw new NullReferenceException("Sync is Lost");
                    }
                }
                else // New Item Should Add
                {
                    if (_bars.Count > 1 && DtToLong(_lastItem.DT) != DtToLong(_bars[1].DT))
                            Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Xma018.New", "Sync is Lost",
                                        _lastItem.ToString(), _bars[1].ToString());

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
            else
            {   // First Item
                _lastItem = new Item();
                AddItem(_lastItem);
                InitItem(0, 0);
            }
        }
        */
        /*
        private void Calculate(int i)
        {
            if (_bars[i].TypicalPrice.CompareTo(_lastItem.High) > 0 && _bars[i].TypicalPrice.CompareTo(_lastItem.Low) > 0)
            {
                // LastBarComlpeted.Price > LastItemCompleted.Band.High -> Break Up

                _lastItem.High = (_bars[i].High + _bars[i].Close) / 2;
                _lastItem.Low = _lastItem.High - 2 * _kAtr * ((Atr.Item)_atrs[i]).Atr;
                _lastItem.Ma = (_lastItem.High + _lastItem.Low) / 2;

                _lastItem.Trend = +1;
                _lastItem.Impulse = 1;

            }
            else if (_bars[i].TypicalPrice.CompareTo(_lastItem.Low) < 0 && _bars[i].TypicalPrice.CompareTo(_lastItem.High)  < 0) 
            {
                // LastBarComlpeted.Price < LastItemCompleted.Band.Low -> Break Down

                _lastItem.Low = (_bars[i].Low + _bars[i].Close) / 2;
                _lastItem.High = _lastItem.Low + 2 * _kAtr * ((Atr.Item)_atrs[i]).Atr;
                _lastItem.Ma = (_lastItem.High + _lastItem.Low) / 2;

                _lastItem.Trend = -1;
                _lastItem.Impulse = 1;

            }
            else if (_bars[i].TypicalPrice.CompareTo(_lastItem.High) > 0 && _bars[i].TypicalPrice.CompareTo(_lastItem.Low) < 0)
            {
                _lastItem.High = (_bars[i].High + _bars[i].Close) / 2;
                _lastItem.Low = (_bars[i].Low + _bars[i].Close) / 2;
                _lastItem.Ma = (_lastItem.High + _lastItem.Low) / 2;

              //  if (_lastItem.Ma.CompareTo(((Item)LastItemCompleted).Ma) > 0)
                if (_lastItem.Ma.CompareTo(((Item)Items[1]).Ma) > 0)   // !!!!!!!!!!!!!!!!!  Index warning   // was 1
                {
                    _lastItem.Trend = +1;
                    _lastItem.Impulse = 1;
                }
                else if (_lastItem.Ma.CompareTo(((Item)Items[1]).Ma) < 0) // !!!!!!!!!!!!!!!!! Index warning 
                {
                    _lastItem.Trend = -1;
                    _lastItem.Impulse = 1;
                }
                else
                {
                   // _lastItem.Trend = ((Item)Items[0]).Trend;
                    _lastItem.Impulse = 0;
                }
            }
            else
            {
                // Warning Index MAy be i - 1 or May Be CORRECT

                _lastItem.Ma = ((Item)Items[1]).Ma;
                _lastItem.Trend = ((Item)Items[1]).Trend;
                _lastItem.Impulse = 0;
                _lastItem.High = ((Item)Items[1]).High;
                _lastItem.Low = ((Item)Items[1]).Low;
                _lastItem.High2 = ((Item)Items[1]).High2;
                _lastItem.Low2 = ((Item)Items[1]).Low2;
                _lastItem.High3 = ((Item)Items[1]).High3;
                _lastItem.Low3 = ((Item)Items[1]).Low3;
            }

            if (_lastItem.Trend == ((Item) Items[1]).Trend) return;

            if (_lastItem.Trend > 0)
            {
                _lastItem.Low3 = _lastItem.Low2; _lastItem.Low2 = ((Item)Items[1]).Low;
            }
            else if (_lastItem.Trend < 0)
            {
                _lastItem.High3 = _lastItem.High2; _lastItem.High2 = ((Item)Items[1]).High;
            }
        }
        */

        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            
            if (iprev > 0) // Copy Operation Last to New Item
            {
                var last = (Item)(Items[ilast]);
                AddItem(new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            Ma = last.Ma,
                          //  Ma = (last.High + last.Low) / 2,

                            Impulse = last.Impulse,
                            Trend = last.Trend,

                            High = last.High,
                            Low = last.Low,
                            High2 = last.High2,
                            Low2 = last.Low2,
                            High3 = last.High3,
                            Low3 = last.Low3
                        }
                    );
            }
            else // Initialization Operation from SyncSeries
            {
                var bar = _bars.Bar(isync);
             //   var atr = _kAtr * ((Atr.Item)_atrs[isync]).Atr;
                var atr = _kAtr * ((Atr.Item)Atr[isync]).Atr;
                var ma = bar.TypicalPrice;
                var h = ma + atr;
                var l = ma - atr;
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            Ma = ma,
                            Impulse = 0,
                            Trend = bar.IsWhite ? +1 : bar.IsBlack ? -1 : 0,

                            High = h,
                            Low = l,
                            High2 = h,
                            Low2 = l,
                            High3 = h,
                            Low3 = l
                        }
                    );
                TrHigh1 = _trHigh5 = h;
                TrLow1 = _trLow5 = l;
                TrHigh2 = _trHigh5 = h;
                TrLow2 = _trLow5 = l;

                FirstSwingAfterBrDown = false;
                FirstSwingAfterBrUp = false;
            }
        }
        public override void InitUpdate(DateTime dt)
        {
          //  _atrs.Update(dt);
          //  _atrs2.Update(dt);
            _atrs3.Update(dt);
           
        }

        public void InitItem(int ibar, int ilast)
        {
            var bar = _bars.Bar(ibar);
            var lastItem = (Item)(Items[ilast]);

          //  var atr = _kAtr * ((Atr.Item)_atrs[0]).Atr;
            var atr = _kAtr * ((Atr.Item)Atr[0]).Atr;

        //    lastItem.DT = bar.DT;
        //    lastItem.SyncDT = bar.LastDT;

            lastItem.Ma = bar.TypicalPrice;

            lastItem.Trend = bar.IsWhite ? +1 : bar.IsBlack ? -1 : 0;
            lastItem.Impulse = 0;
            lastItem.High = lastItem.Ma + atr;
            lastItem.Low = lastItem.Ma - atr;
            lastItem.High2 = lastItem.High;
            lastItem.Low2 = lastItem.Low;
            lastItem.High3 = lastItem.High;
            lastItem.Low3 = lastItem.Low;
        }
        public void Calculate2(int i, int ilast, int iprev)
        {
            if (Count < 2) return;

            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

            var bar = _bars.Bar(i);
            var atr = _kAtr * ((Atr.Item)Atr[i]).Atr;

          //  last.DT = bar.DT;
          //  last.SyncDT = bar.LastDT;

            last.Ma = prev.Ma;
            last.Trend = prev.Trend;
            last.Impulse = 0;
            last.High = prev.High;
            last.Low = prev.Low;
            last.High2 = prev.High2;
            last.Low2 = prev.Low2;
            last.High3 = prev.High3;
            last.Low3 = prev.Low3;

            if (bar.TypicalPrice.IsGreaterThan(prev.High) && bar.TypicalPrice.IsGreaterThan(prev.Low))
            {
                // LastBarComlpeted.Price > LastItemCompleted.Band.High -> Break Up

                last.High = 0.5 * (bar.High + bar.Close);
                /*
                last.Low = last.High - 2 * _kAtr * ((Atr.Item)_atrs[i]).Atr;
                last.Ma = (last.High + last.Low) / 2;
                */
               // last.Ma += (last.High - prev.High);
              //  last.Low = last.High - 2 * _kAtr * ((Atr.Item)_atrs[i]).Atr;
                last.Low = last.High - 2 * atr;

                var newMa = 0.5 * (last.High + last.Low);
                last.Ma = newMa.IsGreaterThan(last.Ma) ? newMa : last.Ma + (last.High - prev.High);

                last.Trend = +1;
                last.Impulse = 1;

            }
            else if (bar.TypicalPrice.IsLessThan(prev.Low) && bar.TypicalPrice.IsLessThan(prev.High))
            {
                // LastBarComlpeted.Price < LastItemCompleted.Band.Low -> Break Down

                last.Low = 0.5 * (bar.Low + bar.Close);
                
               // last.High = last.Low + 2 * _kAtr * ((Atr.Item)_atrs[i]).Atr;
               // last.Ma = (last.High + last.Low) / 2;
                
              //  last.Ma -= (prev.Low - last.Low);
              //  last.High = last.Low + 2 * _kAtr * ((Atr.Item)_atrs[i]).Atr;
                last.High = last.Low + 2 * atr;

                var newMa = 0.5 * (last.High + last.Low);
                last.Ma = newMa.IsLessThan(last.Ma) ? newMa : last.Ma - (prev.Low - last.Low);

                last.Trend = -1;
                last.Impulse = 1;

            }
            else if (bar.TypicalPrice.IsGreaterThan(prev.High) && bar.TypicalPrice.IsLessThan(prev.Low))
            {
                last.High = 0.5*(bar.High + bar.Close);
                last.Low = 0.5*(bar.Low + bar.Close);
                
                // last.Ma = 0.5*(last.High + last.Low);

                //  if (_lastItem.Ma.CompareTo(((Item)LastItemCompleted).Ma) > 0)
                if (last.Ma.CompareTo(prev.Ma) > 0)   // !!!!!!!!!!!!!!!!!  Index warning   // was 1
                {
                    last.Trend = +1;
                    last.Impulse = 1;
                }
                else if (last.Ma.CompareTo(prev.Ma) < 0) // !!!!!!!!!!!!!!!!! Index warning 
                {
                    last.Trend = -1;
                    last.Impulse = 1;
                }
                else
                {
                    // last.Trend = ((Item)Items[0]).Trend;
                    last.Impulse = 0;
                }
            }
            SetTrend2(ilast);

            VolatilityUnit = (float) (last.High - last.Low) / 2f;

            // 2018.02.17
            if (_trend2 > 0 && last.Trend < 0 && prev.Trend > 0)
                Trend2PrevLow = prev.Low;
            if (_trend2 < 0 && last.Trend > 0 && prev.Trend < 0)
                Trend2PrevHigh = prev.High;

                //if (IsFlat)
                //    Evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "xMa018","IsFlat","", "");

            if (last.Trend != prev.Trend)
                SwingCount++;

            if ((_trend2 > 0 && last.Trend > 0) || (_trend2 < 0 && last.Trend < 0))
            {
                SwingCount = 0;
            }

            if (Trend5 == _trend5)
            {
                if (_trend5 > 0)
                {
                    if( last.High.IsGreaterThan(_trHigh5))
                        _trHigh5 = last.High;
                }
                else if (_trend5 < 0)
                {
                    if (last.Low.IsLessThan(_trLow5))
                            _trLow5 = last.Low;
                }
            }
            else
            {
                Trend5 = _trend5;
                if (Trend5 > 0)
                {
                    TrLow2 = TrLow1;
                    TrLow1 = _trLow5;
                    // _trLow5 = double.MaxValue;
                    _trHigh5 = last.High;

                    FirstSwingAfterBrDown = true;
                }
                else if (Trend5 < 0)
                {
                    TrHigh2 = TrHigh1;
                    TrHigh1 = _trHigh5;
                    //_trHigh5 = 0d;
                    _trLow5 = last.Low;

                    FirstSwingAfterBrUp = true;
                }
            }

            // 16.11.08

            if (IsUp && FirstSwingAfterBrUp)
                FirstSwingAfterBrUp = false;
            if (IsDown && FirstSwingAfterBrDown)
                FirstSwingAfterBrDown = false;

            // 15.10.01
            //Trend55.SetValue(Ma.IsGreaterThan(TrHigh1), +1);
            //Trend55.SetValue(Ma.IsLessThan(TrLow1), -1);

            if (Trend55.Update(Ma.IsGreaterThan(TrHigh1), +1))
                Trend55Changed = true;
            else if (Trend55.Update(Ma.IsLessThan(TrLow1), -1))
                Trend55Changed = true;
            else
                Trend55Changed = false;
            
            //if (_trend2 > 0 && _prevTrend2 <= 0 && last.Trend > 0)
            //{
            //    TrLow = Math.Min(last.Low2, last.Low3);
            //}
            //if (_trend2 < 0 && _prevTrend2 >= 0 && last.Trend < 0)
            //{
            //    TrHigh = Math.Max(last.High2, last.High3);
            //}
            CalculateTrend091();

            if (last.Impulse != 0) FlatCount = 0;
            else FlatCount++;

            if (last.Trend == prev.Trend)
            {
                Glue = !(last.High < _prevTrendLastLow || last.Low > _prevTrendLastHigh);
                // !!!!!!!!!!!!!!!!!!
                return; 
            }

            if (last.Trend > 0)
            {
                last.Low3 = last.Low2; last.Low2 = prev.Low;
            }
            else if (last.Trend < 0)
            {
                last.High3 = last.High2; last.High2 = prev.High;
            }

            _prevTrendLastHigh2 = _prevTrendLastHigh;
            _prevTrendLastMa2 = _prevTrendLastMa;
            _prevTrendLastLow2 = _prevTrendLastLow;

            _prevTrendLastHigh = prev.High;
            _prevTrendLastMa = prev.Ma;
            _prevTrendLastLow = prev.Low;

            PrevGlue = Glue;
            Glue = !(last.High < _prevTrendLastLow || last.Low > _prevTrendLastHigh);

        }
        public void CopyItem(int ibar, int ilast, int iprev)
        {
            if (Count < 2) return;

            var bar = _bars.Bar(ibar);
            var last = ((Item)Items[ilast]);
            var prev = ((Item)Items[iprev]);

        //    last.DT = bar.DT;
        //    last.SyncDT = bar.LastDT;

            last.Ma = prev.Ma;
            last.Trend = prev.Trend;
            last.Impulse = prev.Impulse;
            last.High = prev.High;
            last.Low = prev.Low;
            last.High2 = prev.High2;
            last.Low2 = prev.Low2;
            last.High3 = prev.High3;
            last.Low3 = prev.Low3;
        }

        public override void AddNewItem()
        {
            AddItem(new Item());
        }

        public override void InitUpToDate() { _atrs.UpToDate(); _atrs2.UpToDate(); }

        // TimeSeries UpdateItem(TimeSeries tsi)
        /*
        public override void PreUpdate(TimeSeriesItem tsi)
        {
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


            }
            else if (Count > 0)
                InitItem(tsi);
        }
        public override void InitItem(TimeSeriesItem tsi)
        {
            var b = (Bar)tsi;
            var lastItem = (Item)LastItem;

            lastItem.DT = b.DT;
            lastItem.LastDT = b.DT;
            lastItem.SyncDT = b.DT;

            var atr = _kAtr * ((Atr.Item)_atrs[0]).Atr;

            lastItem.Ma = b.TypicalPrice;

            lastItem.Trend = b.IsWhite ? +1 : b.IsWhite ? -1 : 0;
            lastItem.Impulse = 0;
            lastItem.High = lastItem.Ma + atr;
            lastItem.Low = lastItem.Ma - atr;
            lastItem.High2 = lastItem.High;
            lastItem.Low2 = lastItem.Low;
            lastItem.High3 = lastItem.High;
            lastItem.Low3 = lastItem.Low;
        }
        */
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            Calculate2(isync, ilast, iprev);
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            InitItem(isync, ilast);
        }
        public override void CopyItem(TimeSeriesItem tsi, int ibar, int ilast, int iprev)
        {
           // var lastItem = ((Item)Items[ilast]);
           // var prevItem = ((Item)Items[iprev]);

            CopyItem(ibar, ilast, iprev);

            // ((Item)Items[ilast]).Ma = ((Item)Items[iprev]).Ma;
        }
        /*
        public void UpToDate3()
        {
            if (_bars.Count < 1) return;
            if (_bars.Count <= Count) return;

            Items.Clear();

            var ma = _bars.FirstItem.TypicalPrice;
            var high = ma + _kAtr * ((Atr.Item)_atrs[_atrs.Count - 1]).Atr;
            var low = ma - _kAtr * ((Atr.Item)_atrs[_atrs.Count - 1]).Atr;

            _lastItem = new Item(_bars.FirstItem.DT, ma, +1,0, high, low) { High2 = high, Low2 = low, High3 = high, Low3 = low} ;
            AddItem(_lastItem);

            for (var i = _bars.Count - 2; i >= 0; i--)
            {
                var atrdt = _atrs[i].DT;

                ma = _lastItem.Ma;
                var h = _lastItem.High;
                var l = _lastItem.Low;
                var tr = _lastItem.Trend;
                var imp = _lastItem.Impulse;
                var h2 = _lastItem.High2;
                var l2 = _lastItem.Low2;
                var h3 = _lastItem.High3;
                var l3 = _lastItem.Low3;

                _lastItem = new Item { DT = _bars[i].DT, Ma = ma, High = h, Low = l, Trend = tr, Impulse = imp, High2 = h2, Low2 = l2, High3 = h3, Low3 = l3 };

                AddItem(_lastItem);
                Calculate(i);
            }
            Ticker.EventLog.AddItem(EvlResult.SUCCESS,EvlSubject.TECHNOLOGY,"Xma018","UpToDate", ToString(), "Count=" + _bars.Count );
        }
         */

        public override string Key
        {
            get
            {
                return
                    $"Type={GetType()};Ticker={Ticker.Code};TimeIntSeconds={TimeIntSeconds};MaLength={_maLength};AtrLength1={_atrLength1};AtrLength2={_atrLength2};kAtr={_kAtr};xMode={_xMode}";
            }
        }
        public override string ToString()
        {
            return
                $"Type={GetType()};Ticker={Ticker.Code};TimeIntSeconds={TimeIntSeconds};MaLength={_maLength};AtrLength1={_atrLength1};AtrLength2={_atrLength2};kAtr={_kAtr};xMode={_xMode};Ma={Ma:N3};Tr={Trend};TrChanged={TrendChanged};Imp={Impulse};Count={Count}";
        }

        public bool HaveHigher(int seconds, double value, out double high)
        {
            var lastDt = LastItemCompletedDT.AddSeconds(-seconds);
            foreach (var i in Items.Where(i => i.DT.CompareTo(lastDt) > 0))
            {
                if (value.IsGreaterOrEqualsThan(((Item) i).High2)) continue;

                high = ((Item) i).High2;
                return true;
            }
            high = 0d;
            return false;
        }
        public bool HaveLower(int seconds, double value, out double low)
        {
            
            var lastDt = LastItemCompletedDT.AddSeconds(-seconds);
            foreach (var i in Items.Where(i => i.DT.CompareTo(lastDt) > 0))
            {
                if (value.IsLessOrEqualsThan(((Item)i).Low2)) continue;

                low = ((Item)i).Low2;
                return true;
            }
            low = 0d;
            return false;
        }

        public bool IsTrend09Down => Trend09 == -2;
        public bool IsTrend09Up => Trend09 == +2;
        public int Trend09 { get; private set; }
        protected double? FirstFlatDownMa { get; private set; }
        protected double? FirstFlatUpMa { get; private set; }

        private void CalculateTrend09()
        {
            if (IsDown && Ma.IsLessThan(_prevTrendLastLow))
            {
                FirstFlatUpMa = null;
                if (IsFlat && FirstFlatDownMa == null)
                {
                    Trend09 = -1;
                    FirstFlatDownMa = Ma;
                }
                else
                {
                    if (Trend09 == -1 && FirstFlatDownMa != null && Ma.IsLessThan((double) FirstFlatDownMa))
                        Trend09 = -2;
                }
            }
            else if (IsUp && Ma.IsGreaterThan(_prevTrendLastHigh))
            {
                FirstFlatDownMa = null;
                if (IsFlat && FirstFlatUpMa == null)
                {
                    Trend09 = +1;
                    FirstFlatUpMa = Ma;
                }
                else
                {
                    if (Trend09 == +1 && FirstFlatUpMa != null && Ma.IsGreaterThan((double)FirstFlatUpMa))
                        Trend09 = +2;
                }
            }
            else
            {
                Trend09 = 0;
                FirstFlatDownMa = null;
                FirstFlatUpMa = null;
            }
        }
        private void CalculateTrend091()
        {
            //if (IsDown && Ma.IsLessThan(_prevTrendLastLow))
            if (IsDown)
            {
                FirstFlatUpMa = null;
                if (IsFlat && FirstFlatDownMa == null)
                {
                    Trend09 = -1;
                    FirstFlatDownMa = Ma;
                }
                else
                {
                    if (Trend09 == -1 && FirstFlatDownMa != null && Ma.IsLessThan((double)FirstFlatDownMa))
                        Trend09 = -2;
                }
            }
            else if (IsUp)
            {
                FirstFlatDownMa = null;
                if (IsFlat && FirstFlatUpMa == null)
                {
                    Trend09 = +1;
                    FirstFlatUpMa = Ma;
                }
                else
                {
                    if (Trend09 == +1 && FirstFlatUpMa != null && Ma.IsGreaterThan((double)FirstFlatUpMa))
                        Trend09 = +2;
                }
            }
            else
            {
                Trend09 = 0;
                FirstFlatDownMa = null;
                FirstFlatUpMa = null;
            }
        }


        public class Item : TimeSeriesItem
        {
            public double Ma { get; set; }
            public int Trend { get; set; }
            public int Impulse { get; set; }
            public double High { get; set; }
            public double Low { get; set; }

            public double High2 { get; set; }
            public double Low2 { get; set; }

            public double High3 { get; set; }
            public double Low3 { get; set; }

            public Item()
            {
            }

            public Item(DateTime dt, double last, int trend, int impulse, double high, double low)
            {
                DT = dt;
                Ma = last;
                Trend = trend;
                Impulse = impulse;
                High = high;
                Low = low;
            }
            public override string ToString()
            {
                return String.Format("DT={0:HH:mm:ss:fff};Ma={1:N3};Tr={2};Imp={3};High={4:N3};Low={5:N3};Sync={6:HH:mm:ss:fff}",
                    DT, Ma, Trend, Impulse, High, Low, SyncDT);
            }
        }
    }   
}
