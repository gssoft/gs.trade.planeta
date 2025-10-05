using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.Data.Bars;


namespace GS.Trade.Data
{
    public class BarSeries : IBars //: ABarSeries
    {
        public long ID { get; set; }

        [XmlIgnore]
        public const string TypeName = "Bar";
        
        [XmlIgnore]
        public ITicker Ticker;

        public string Code { get; set; }
        public string Name { get; set; }

        private Bar _lastBar;
        [XmlIgnore]
        public DateTime LastTickDT { get; private set; }
        [XmlIgnore]
        public long TickCount { get; private set; }

        public long Complete;
        public DateTime SyncDT;

        public int TimeIntSeconds { get; set; }
        public int ShiftIntSeconds { get; set; }

        public Bar FirstItem
        {
            get { return BarCollection.Count > 0 ? BarCollection[BarCollection.Count - 1] : null; }
        }
        public double FirstBaseValue
        {
            get { return BarCollection.Count > 0 ? BarCollection[BarCollection.Count - 1].Open : 0; }
        }

        public bool IsNoValid02(int index)
        {
           //throw new NotImplementedException();
            return false;
        }
        public bool IsNoValid01(int index)
        {
            //throw new NotImplementedException();
            return false;
        }

        public IBar LastItem
        {
            get { return BarCollection.Count > 0 ? BarCollection[0] : null; }
        }
        public IBar LastItemCompleted
        {
            get { return BarCollection.Count > 1 ? BarCollection[1] : null; }
        }

        public double Close
        {
            get { return LastItemCompleted != null ? LastItemCompleted.Close : 0d; }
        }
        public double LastCompletedClose => LastItemCompleted?.Close ?? 0d;

        public double High
        {
            get { return LastItemCompleted != null ? LastItemCompleted.High : 0d; }
        }
        public double Low
        {
            get { return LastItemCompleted != null ? LastItemCompleted.Low : 0d; }
        }
        public double Open
        {
            get { return LastItemCompleted != null ? ((IBar)LastItemCompleted).Open : 0d; }
        }
        public double Volume
        {
            get { return LastItemCompleted != null ? ((IBar)LastItemCompleted).Volume : 0d; }
        }
        public bool IsBlack => Count > 1 && LastItemCompleted.IsBlack;
        public bool IsWhite => Count > 1 && LastItemCompleted.IsWhite;

        public bool IsDoj => Count > 1 && LastItemCompleted.IsDoj;

        public bool IsFaded => Count > 1 && LastItemCompleted.IsFaded;

        [XmlIgnore]
        public List<Bar> BarCollection = new List<Bar>();

        private readonly Object _lockAddBar = new Object();

        public string Key { get { return String.Format("TypeName={0};TimeIntSeconds={1}", TypeName, TimeIntSeconds); } }

        public int Count
        {
            get { return BarCollection.Count; }
        }
        public IBar Bar(int i)
        {
            return this[i];
        }

        /*
        public double TrueRange { get { return TrueHigh - TrueLow; } }
        private double TrueHigh
        {
            get
            {
                if (BarCollection.Count > 1)
                    return this[1].High > this[0].High ? this[1].High : this[0].High;
                return BarCollection.Count > 0 ? this[0].High : 0;
            }
        }
        private double TrueLow
        {
            get
            {
                if (BarCollection.Count > 1)
                    return this[1].Low < this[0].Low ? this[1].Low : this[0].Low;
                return BarCollection.Count > 0 ? this[0].Low : 0;
            }
        }
        */
        public double TrueRange( int index)
        {
            if( BarCollection.Count > index + 1 )
            {
                var truehigh = this[index+1].Close > this[index].High ? this[index+1].Close : this[index].High;
                var truelow = this[index+1].Close < this[index].Low ? this[index+1].Close : this[index].Low;
                return truehigh - truelow;
            }
            if (BarCollection.Count > index)
            {
                return this[index].High - this[index].Low;
            }
            return 0;
        }

        public Bar this[int index]
        {
            get { return Count > index ? BarCollection[index] : null; }
        }

        public BarSeries()
        {
            // need for Serialization
        }

        public BarSeries(long seriesId, string name, ITicker ticker, int timeIntSeconds, int shiftIntSeconds)
        {
            ID = seriesId;
            Name = name;

            Ticker = ticker;

            ShiftIntSeconds = shiftIntSeconds;
            TimeIntSeconds = timeIntSeconds;

           // BarCollection = new List<Bar>();
        }
        public static long DtToLong(DateTime dt)
        {
            return  dt.Second +
                    dt.Minute*  100 +
                    dt.Hour*    10000 +
                    dt.Day*     1000000 +
                    dt.Month*   100000000 +
                    dt.Year*    10000000000
                   ;
        }

        public void AddBar(Bar b)
        {
            lock (_lockAddBar)
            {
                BarCollection.Insert(0,b);
            }
        }
        //public void FillBars( IEnumerable<DB.Q.Bar> bars)
        //{
        //    if (bars == null) return;
        //    BarCollection.Clear();
        //    foreach (var b in bars.OrderBy(d => d.DT))
        //    {
        //        var ba = new Bar
        //        {
        //            DT = b.DT,
        //            Open = b.Open,
        //            High = b.High,
        //            Low = b.Low,
        //            Close = b.Close,
        //            Volume = (double)b.Volume,
        //            SeriesID = ID
        //        };
        //        AddBar(ba);
        //    }
        //    _lastBar = LastItem as Bar;
        //}
        //public void FillBars(IEnumerable<DownLoader.Bar> bars)
        //{
        //    if (bars == null) return;
        //    BarCollection.Clear();
        //    foreach (var b in bars.OrderBy(d => d.DT))
        //    {
        //        var ba = new Bar
        //        {
        //            DT = b.DT,
        //            Open = b.Open,
        //            High = b.High,
        //            Low = b.Low,
        //            Close = b.Close,
        //            Volume = (double)b.Volume,
        //            SeriesID = ID
        //        };
        //        AddBar(ba);
        //    }
        //    _lastBar = LastItem as Bar;
        //}
        
        public void Tick(DateTime dt, double last, double volume, IEventLog evl)
        {
            //if (BarCollection.Count > 0)
            TickCount++;
            LastTickDT = dt; // LastTick DT very Important!

            var m = dt.Millisecond;
            var tickBarDt = TimeSeries.GetBarDateTime(dt, TimeIntSeconds);

            if ( BarCollection.Count > 0 )
            {
                var lastBarDT = this[0].DT;
                //var lastBarDT = _lastBar.DT;

            //    evl.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Sync", "New Tick",
            //                    "DT: " + dt.ToString("hh:mm:ss:fff") + " LastBar: " + lastBarDT.ToString("hh:mm:ss:fff"), "");

                if ((tickBarDt - dt).Seconds > TimeIntSeconds)
                {
                    evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Sync Error", "Sync Error", "New Tick",
                                "DT: " + dt.ToString("hh:mm:ss:fff") + " Bar: " + tickBarDt.ToString("hh:mm:ss:fff"),"");
                    throw new ArgumentOutOfRangeException(
                        "TimeInt: " + TimeIntSeconds + " DT: " + dt.ToString("hh:mm:ss:fff") + " Bar: " + tickBarDt.ToString("hh:mm:ss:fff"));      
                }
                /*
                if (Math.Abs((bardt - lastBarDT).Seconds) > TimeIntSeconds)
                {
                    evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Sync Error", "New Tick",
                                "TimeInt: " + TimeIntSeconds + " DT: " + lastBarDT.ToString("hh:mm:ss:fff") + " Bar: " + bardt.ToString("hh:mm:ss:fff"), "");
                    throw new ArgumentOutOfRangeException(
                       "TimeInt: " + TimeIntSeconds +
                       " LastDT: " + lastBarDT.ToString("hh:mm:ss:fff") + " Bar: " + bardt.ToString("hh:mm:ss:fff"));      

                }
                */
                var nTickBarDt = DtToLong(tickBarDt);
                var nlastBarDt = DtToLong(_lastBar.DT);

                if (Count > 1 &&
                    _lastBar.DT.CompareTo((this[0]).DT) != 0 && _lastBar.Close.CompareTo(last) != 0 && nlastBarDt == nTickBarDt)
                {
                    _lastBar.LastDT = dt;
                    AddBar(_lastBar);
                }
                

                if (nlastBarDt >= nTickBarDt)
                {
                    if (nlastBarDt == nTickBarDt)
                    {
                        if (_lastBar.High < last) _lastBar.High = last;
                        if (_lastBar.Low > last) _lastBar.Low = last;
                        _lastBar.Close = last;
                        _lastBar.Volume += volume;
                        _lastBar.LastDT = dt;
                    }
                    else
                    {
                        /*
                        if (_lastBar.High < last) _lastBar.High = last;
                        if (_lastBar.Low > last) _lastBar.Low = last;
                        _lastBar.Close = last;
                        _lastBar.Volume += volume;
                        _lastBar.LastDT = dt;
                        */
                        evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "BarSeries.Tick", "New Tick", "New Tick",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", _lastBar.DT, dt), _lastBar.ToString());
                    }
                }
                else // New Bar should be Added
                {
                    /*
                    evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "BarSeries.Tick", "New Bar", _lastBar.ToString(),
                        String.Format("TimeInt={0} SyncDt={1:T} BarDt={2:T} CalcDt={3:T} Ticks={4}",
                        TimeIntSeconds, dt, _lastBar.DT, bardt, TickCount));
                    */
                    
                   // if (Count > 1 && _lastBar.IsFaded && _lastBar.Close.CompareTo(LastItemCompleted.Close) == 0)
                    if (Count > 1 && _lastBar.IsFaded && _lastBar.Close.CompareTo(last) == 0)
                    {
                        _lastBar = new Bar( tickBarDt, last, last, last, last, volume) { LastDT = dt };
                        //_lastBar.DT = bardt;
                        //_lastBar.LastDT = bardt;

                     //   evl.AddItem(EvlResult.WARNING, EvlSubject.TECHNOLOGY, "BarSeries.Tick", "Faded Bar",
                     //               _lastBar.ToString(), last.ToString());

                        return;
                    }
                    /*
                    if (Count > 1 && ((LastItem.IsFaded && !_lastBar.IsFaded) || (LastItem.Close.CompareTo(_lastBar.Close) !=0)) )
                        AddBar(_lastBar);
                    */
                    _lastBar = new Bar(tickBarDt, last, last, last, last, volume) { LastDT = dt };
                    AddBar(_lastBar);
                }
            }
            else
            {
                _lastBar = new Bar(tickBarDt, last, last, last, last, volume) { LastDT = dt };
                AddBar(_lastBar);
            }
        }
        public void Update(Bar b)
        {
            TickCount++;
            LastTickDT = b.LastDT; // LastTick DT very Important!

            var bardt = TimeSeries.GetBarDateTime2(b.DT, TimeIntSeconds);
            if (BarCollection.Count > 0)
            {
                var ndt = DtToLong(bardt);
                var nlastdt = DtToLong(_lastBar.DT);

                if (nlastdt >= ndt)
                {
                    if (nlastdt == ndt)
                    {
                        if (_lastBar.High < b.High) _lastBar.High = b.High;
                        if (_lastBar.Low > b.Low) _lastBar.Low = b.Low;
                        _lastBar.Close = b.Close;
                        _lastBar.Volume += b.Volume;
                        _lastBar.LastDT = b.LastDT;
                    }
                    else
                    {
                        Ticker.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "BarSeries.TickBar", "New TickBar", "New TickBar",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", _lastBar.DT, b.DT), _lastBar.ToString());
                    }
                }
                else // New Bar should be Added
                {
                    /*
                    evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "BarSeries.Tick", "New Bar", _lastBar.ToString(),
                        String.Format("TimeInt={0} SyncDt={1:T} BarDt={2:T} CalcDt={3:T} Ticks={4}",
                        TimeIntSeconds, dt, _lastBar.DT, bardt, TickCount));
                    */
                    _lastBar = new Bar(bardt, b.Open, b.High, b.Low, b.Close, b.Volume) { LastDT = b.LastDT };
                    AddBar(_lastBar);
                }
            }
            else
            {
                _lastBar = new Bar(bardt, b.Open, b.High, b.Low, b.Close, b.Volume) { LastDT = b.LastDT };
                AddBar(_lastBar);
            }
        }

        public void AdjustLastItem()
        {
            _lastBar = LastItem as Bar;
        }
        public void UpToDateTime( BarSeries masterbs)
        {
            if( masterbs.Count < 1) return;

            BarCollection.Clear();
            var mblast = masterbs.LastItem;
            var b = new Bar
                        {
                            SeriesID = mblast.SeriesID,
                            DT = TimeSeries.GetBarDateTime2(mblast.DT, TimeIntSeconds),
                            Open = mblast.Open,
                            High = mblast.High,
                            Low = mblast.Low,
                            Close = mblast.Close,
                            Volume = mblast.Volume
                        };
            _lastBar = b;
             foreach( var mb in masterbs.BarCollection)
             {
                 if( b.DT == TimeSeries.GetBarDateTime2(mb.DT,TimeIntSeconds) )
                 {
                     b.Open = mb.Open;
                     if (b.High < mb.High) b.High = mb.High;
                     if (b.Low > mb.Low) b.Low = mb.Low;
                     b.Volume += mb.Volume;
                 }
                 else
                 {
                     BarCollection.Add(b);
                     b = new Bar
                     {
                         SeriesID = mb.SeriesID,
                         DT = TimeSeries.GetBarDateTime2(mb.DT, TimeIntSeconds),
                         Open = mb.Open,
                         High = mb.High,
                         Low = mb.Low,
                         Close = mb.Close,
                         Volume = mb.Volume
                     };
                 }
             }
        }
        public double GetOpen(int i)
        {
            return ((IBar)(BarCollection[i])).Open;
        }
        public double GetHigh(int i)
        {
            return ((IBar)(BarCollection[i])).High;
        }
        public double GetLow(int i)
        {
            return ((IBar)(BarCollection[i])).Low;
        }
        public double GetClose(int i)
        {
            return ((IBar)(BarCollection[i])).Close;
        }
        public double GetVolume(int i)
        {
            return ((IBar)(BarCollection[i])).Volume;
        }

        public double GetTypical(int i)
        {
            return ((IBar)(BarCollection[i])).TypicalPrice;
        }
        public double GetMedian(int i)
        {
            return ((IBar)(BarCollection[i])).MedianPrice;
        }
        public double GetLine(int i)
        {
            return GetClose(i);
        }
        public DateTime GetDateTime(int index)
        {
            return this[index].DT;
        }
        public int GetCount()
        {
            return Count;
        }

        public void Update(DateTime syncDT)
        {
            if (IsComplete(syncDT)) return;
        }
        public bool IsComplete(long complete)
        {
            return complete > Complete ? true : false;
        }
        protected bool IsComplete(DateTime syncDT)
        {
            return SyncDT.CompareTo(syncDT) >= 0 ? true : false;
        }
        /*
        private DateTime GetBarDateTime(DateTime dt)
        {
            var mili = dt.Millisecond;
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (sec/TimeIntSeconds + 1) * TimeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
         */ 
        public void ClearSomeData(int count)
        {
            while( Count > count )
                BarCollection.RemoveAt(Count-1);
        }

        public override string ToString()
        {
            return String.Format("Type={0};Name={1};TimeInt={2};ShiftTimeInt={3};Count={4}",
                                 GetType(), Name, TimeIntSeconds, ShiftIntSeconds, Count);
        }

    }
}