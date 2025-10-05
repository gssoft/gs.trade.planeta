using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using GS.Elements;
//using GS.EventLog;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies;

namespace GS.Trade.Data
{
    #region  -------- TimeSeriesSeries ----------------- 
    
    public abstract class TimeSeries : Element1<string>, ITimeSeries, IEnumerable
    {
        private const int CapasityValue = 1024;
        private const int CapasityLimitValue = 1024;

        protected bool DebudAddITem { get; set; }

        public IEventLog Evl { get; private set; }

        public IEventLog FileEvl { get; set; }
        public long Id { get; set; }
        public string TypeName { get; set; }

        //public string Code { get; set; }
        //public string Name { get; set; }

        //public abstract string Key { get; }

        public int TimeInterval { get; set; }
        public int TimeShift { get; set; }

        public int Capasity { get; set; }
        public int CapasityLimit { get; set; }

        public ITicker   Ticker { get; set; }
        //public string   Key { get; protected set; }

        public ITimeSeries SyncSeries { get; set; }
        public Bars.Bars Bars { get; set; }

        public int TimeIntSeconds { get; set; }
        public int ShiftIntSecond { get; set; }

        protected long Complete;

        public DateTime SyncDT { get; protected set; }
        public DateTime LastTickDT { get; protected set; }
        public long TickCount { get; protected set; }

        public int ItemsDailyCount { get; protected set; }

        public virtual ITimeSeriesItem LastItem => this[0];

        public ITimeSeriesItem FirstItem => this[Count - 1];

        public virtual ITimeSeriesItem LastItemCompleted => this[1];
        public virtual ITimeSeriesItem PrevItemCompleted => this[2];
        public DateTime LastItemDT => 
            Items.Count > 0  && LastItem != null ? LastItem.DT : DateTime.MinValue;

        public DateTime LastItemdDT =>
           Count > 0
               ? (LastItem?.DT ?? DateTime.MinValue)
               : DateTime.MinValue;
        public DateTime LastItemCompletedDT => 
            Count > 1 
                ? (LastItemCompleted?.DT ?? DateTime.MinValue)
                : DateTime.MinValue;
        public DateTime PrevItemCompletedDT =>
             Count > 2 
                ? (PrevItemCompleted?.DT ?? DateTime.MinValue)
                : DateTime.MinValue;

        protected bool NeedToCalcUnCompletedBar;

        public int Count => Items.Count;

        public virtual ITimeSeriesItem this[int index]
        {
            get { return /*Items.Count > 0 &&*/ index >= 0 && index < Items.Count ? Items[index] : null; }
        }
        protected object AddItemLocker = new object();

        public readonly List<TimeSeriesItem> Items = new List<TimeSeriesItem>();

        protected TimeSeries(string typename, string name, ITicker ticker, int timeIntSeconds)
        {
            Name = name;
            TypeName = typename;
            Ticker = ticker;
            TimeIntSeconds = timeIntSeconds;

            Capasity = CapasityValue;
            CapasityLimit = CapasityLimitValue;
        }
        protected TimeSeries(string name, ITicker ticker, int timeIntSeconds)
        {
            Name = name;
            Ticker = ticker;
            TimeIntSeconds = timeIntSeconds;

            Capasity = CapasityValue;
            CapasityLimit = CapasityLimitValue;
        }
        protected TimeSeries(string name, ITicker ticker, int timeIntSeconds, int shiftSeconds)
        {
            //Code = String.Format("{0}_Min", timeIntSeconds);
            Name = name;
            Ticker = ticker;
            TimeIntSeconds = timeIntSeconds;
            ShiftIntSecond = shiftSeconds;

            Capasity = CapasityValue;
            CapasityLimit = CapasityLimitValue;
        }
        protected TimeSeries(string code, string name, ITicker ticker, int timeIntSeconds, int shiftSeconds)
        {
            Code = code;
            Name = name;
            Ticker = ticker;
            TimeIntSeconds = timeIntSeconds;
            ShiftIntSecond = shiftSeconds;

            Capasity = CapasityValue;
            CapasityLimit = CapasityLimitValue;
        }

        protected TimeSeries()
        {
            Capasity = CapasityValue;
            CapasityLimit = CapasityLimitValue;
        }
       
        public void SetEventLog(IEventLog evl)
        {
            if(evl == null) throw new NullReferenceException("Eventlog == Null");
            Evl = evl;
        }

        protected void AddItem(TimeSeriesItem item)
        {
            lock (AddItemLocker)
            {
                item.Series = this;
                Items.Insert(0,item);

                if (Capasity != 0 && (CapasityLimit + Capasity) < Count)
                    ClearSomeData(Capasity);
            }
            if (DebudAddITem && Count > 1)
                Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING, Name,
                    String.Format("{0}:[{1:T}]", Name, LastItemCompleted.DT), "New", LastItemCompleted.ToString(), "");
        }
        protected bool IsComplete(long complete)
        {
            return complete <= Complete;
        }
        /*
        protected bool IsComplete(DateTime syncDT)
        {
            return SyncDT.CompareTo(syncDT) >= 0 ? true : false;
        }
        */
        public bool IsUpToSyncTime(DateTime syncDT)
        {
            if (SyncDT.CompareTo(syncDT) >= 0) return true;
            SyncDT = syncDT;
            return false;
        }
        public bool IsAlreadyUpdate(DateTime sync)
        {
            return true;
        }

        public static long DtToLong(DateTime dt)
        {
            return dt.Second +
                    dt.Minute * 100 +
                    dt.Hour * 10000 +
                    dt.Day * 1000000 +
                    dt.Month * 100000000 +
                    dt.Year * 10000000000
                   ;
        }
        public static DateTime GetBarDateTime(DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (sec / timeIntSeconds + 1) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
        public static DateTime GetBarDateTime2(DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (int)(Math.Ceiling((double)sec / timeIntSeconds)) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }
        public static DateTime GetBarDateTime3(DateTime dt, int timeIntSeconds)
        {
            var sec = dt.Hour * 3600 + dt.Minute * 60 + dt.Second;
            var hsec = (sec / timeIntSeconds) * timeIntSeconds;
            return (dt.AddSeconds(hsec - sec)).AddMilliseconds(-dt.Millisecond);
        }



        public abstract void Init();
        
       // public abstract void Update(DateTime syncDT, IEventLog evl);
       // public abstract void Update(DateTime syncDT);

        public virtual void Tick( DateTime dt, double last, double volume)
        {
        }

        public virtual void Update(DateTime dt)
        {
            if (IsUpToSyncTime(dt)) return;   // prevent double time calculation 
            InitUpdate(dt);

            if (SyncSeries == null || SyncSeries.Count < 1) return;

            var barsTickCount = SyncSeries.TickCount;
            if (barsTickCount <= TickCount) return;
            TickCount = barsTickCount;

            LastTickDT = SyncSeries.LastTickDT;
            var barDt = SyncSeries.LastItem.DT;

            if (Count > 0)
            {
                var nbardt = DtToLong(barDt);
                var nlastItemDt = DtToLong(LastItem.DT);

                if (nlastItemDt >= nbardt)
                {
                    if (nlastItemDt == nbardt)
                    {
                        if (Count > 1)
                        {
                            if (NeedToCalcUnCompletedBar)
                                            // Calculate2(0, 0, 1);
                                Calculate(dt, barDt, SyncSeries.LastItem, 0, 0, 1);
                        }
                        else
                            //InitItem(0, 0);
                            InitItem(dt, barDt, SyncSeries.LastItem, 0, 0);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Atr", "Atr", "Update",
                                    String.Format("SyncTime={0:T}, LastBarTime={1:T} < LastTickTime={2:T}", dt, barDt, LastItem.DT), "");
                        throw new NullReferenceException("Sync is Lost");
                    }
                }
                else
                { // new Item Add
                  //  if (SyncSeries.Count > 1 && DtToLong(this[0].DT) < DtToLong(SyncSeries[1].DT))
                  //      UpToDate2(dt, SyncSeries);
                  //  else
                  //  {
                        if (NeedToCalcUnCompletedBar)
                        {
                            //AddNewItem();
                            // Calculate2(0, 0, 1);
                            //Calculate(dt, barDt, SyncSeries.LastItem, 0, 0, 1);
                            //CopyItem(dt, barDt, SyncSeries.LastItem, 0, 0, 1);
                            AddNewItem(dt, barDt, SyncSeries.LastItem, 0, 0, 1);
                        }
                        else
                        {
                            //Calculate2(1, 0, 1);
                            //AddNewItem();
                            //CopyItem(0, 0, 1);
                            if( Count > 1)
                                Calculate(dt, barDt, SyncSeries.LastItemCompleted, 1, 0, 1);
                            else
                                InitItem(dt, SyncSeries.LastItemCompletedDT, SyncSeries.LastItem, 1, 0);
                            //if (Count == 1)
                            //    LastItem.DT = SyncSeries[1].DT;
                            
                            //AddNewItem();
                            //CopyItem(dt, barDt, SyncSeries.LastItem, 0, 0, 1);
                            AddNewItem(dt, barDt, SyncSeries.LastItem, 0, 0, 1);
                            
                            //if (Count > 1)
                            //    Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING,
                             //       String.Format("{0}:[{1:T}]", Name, LastItemCompleted.DT), "New", LastItemCompleted.ToString(), "");
                        }
                    //}
                }
            }
            else
            {
                //AddNewItem();
                //InitItem(dt, barDt, SyncSeries.LastItem, 0, 0);
                AddNewItem(dt, barDt, SyncSeries.LastItem, 0, 0, 0);
               // if (Count > 1)
               //     Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING,
               //                     String.Format("{0}:[{1:T}]", Name, LastItemCompleted.DT), "New", LastItemCompleted.ToString(), "");
            }
        }
        public void Update(Bar b)
        {
            TickCount++;
            LastTickDT = b.DT; // LastTick DT very Important!
            Bar lastItem;
            var syncDt = GetBarDateTime2(b.DT, TimeIntSeconds);
            if (Count > 0)
            {
                lastItem = (Bar) LastItem;

                var nSyncDt = DtToLong(syncDt);
                var nlastItemDt = DtToLong(lastItem.DT);

                if (nlastItemDt >= nSyncDt)
                {
                    if (nlastItemDt == nSyncDt)
                    {
                        /*
                        if (lastItem.High < b.High) lastItem.High = b.High;
                        if (lastItem.Low > b.Low) lastItem.Low = b.Low;
                        lastItem.Close = b.Close;
                        lastItem.Volume += b.Volume;

                        lastItem.LastDT = b.DT;
                        lastItem.SyncDT = b.DT;
                        */
                        // Calculate2(0, 0, 1);
                        //if( Count > 1)
                            Calculate(b.DT, syncDt, b, 0, 0, 1);
                        //else
                        //    InitItem(b.LastDT, syncDt, b, 0, 0);
                    }
                    else
                    {
                        Ticker.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "BarSeries.TickBar", "New TickBar", "New TickBar",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", lastItem.DT, b.DT), lastItem.ToString());
                        throw new NullReferenceException(String.Format("LastBarTime={0:T} > LastTickTime={1:T}; {2}", lastItem.DT, b.DT, lastItem));
                    }
                }
                else // New Bar should be Added
                {
                    /*
                    evl.AddItem(EvlResult.SUCCESS, EvlSubject.PROGRAMMING, "BarSeries.Tick", "New Bar", _lastBar.ToString(),
                        String.Format("TimeInt={0} SyncDt={1:T} BarDt={2:T} CalcDt={3:T} Ticks={4}",
                        TimeIntSeconds, dt, _lastBar.DT, bardt, TickCount));
                    */
                    //lastItem = new Bar(syncDt, b.Open, b.High, b.Low, b.Close, b.Volume) { LastDT = b.LastDT };

                    AddNewItem();
                    /*
                    lastItem = (Bar)LastItem;

                    lastItem.DT = syncDt;
                    lastItem.LastDT = syncDt;
                    lastItem.SyncDT = syncDt;

                    lastItem.Open = b.Open;
                    lastItem.High = b.High;
                    lastItem.Low = b.Low;
                    lastItem.Close = b.Close;
                    lastItem.Volume = b.Volume;

                    lastItem.Ticks = 1;
                    */
                    InitItem(b.LastDT, syncDt, b, 0, 0);
                }
            }
            else
            {
                AddNewItem();
                /*
                lastItem = (Bar)LastItem;

                lastItem.DT = syncDt;
                lastItem.LastDT = syncDt;
                lastItem.SyncDT = syncDt;

                lastItem.Open = b.Open;
                lastItem.High = b.High;
                lastItem.Low = b.Low;
                lastItem.Close = b.Close;
                lastItem.Volume = b.Volume;

                lastItem.Ticks = 1;
                */
                InitItem(b.LastDT, syncDt, b, 0, 0);
            }
        }
        public void Update(ITimeSeriesItem t)
        {
            TickCount++;

            LastTickDT = t.DT; // LastTick DT very Important!
            var dt = t.DT;

            var tickBarDt = t.GetTimeSeriesItemDateTime(dt, TimeIntSeconds);

            if (Count > 0)
            {
                var lastBarDT = this[0].DT;
                var lastBar = this[0];

                if ((tickBarDt - dt).Seconds > TimeIntSeconds)
                {
                    Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "Sync Error", "New Tick", "New Tick",
                                "DT: " + dt.ToString("hh:mm:ss:fff") + " Bar: " + tickBarDt.ToString("hh:mm:ss:fff"), "");
                   // throw new ArgumentOutOfRangeException(
                   //     "TimeInt: " + TimeIntSeconds + " DT: " + dt.ToString("hh:mm:ss:fff") + " Bar: " + tickBarDt.ToString("hh:mm:ss:fff"));
                }
                var nTickBarDt = DtToLong(tickBarDt);
                var nlastBarDt = DtToLong(lastBar.DT);
                
                if (nlastBarDt >= nTickBarDt)
                {
                    if (nlastBarDt == nTickBarDt)
                    {
                        //Calculate2(0, 0, 1);
                        Calculate(t.DT, tickBarDt, t, 0, 0 ,1);
                    }
                    else
                    {
                        Evl.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "BarSeries.Tick", "New Tick", "New Tick",
                                    String.Format("LastBarTime={0:T} > LastTickTime={1:T}", this[0].DT, dt), this[0].ToString());
                      //  throw new NullReferenceException("Sync is Lost");
                        // throw new NullReferenceException(string.Format("Sync is Lost: BarDt={0:H:mm:ss.fff};TickDT={1:H:mm:ss.fff}", lastBarDT.TimeOfDay, t.DT.TimeOfDay));
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
                    AddNewItem(t.DT, tickBarDt, t, 0, 0, 0);
                    //InitItem(0, 0);
                    //InitItem(t.DT, tickBarDt, t, 0, 0);

                   // if (Count > 1)
                   //     Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING,
                   //         String.Format("{0}:[{1:T}]", Name, LastItemCompleted.DT), "New", LastItemCompleted.ToString(), "");
                }
            }
            else
            {
                AddNewItem(t.DT, tickBarDt, t, 0, 0, 0);
                //InitItem(0, 0);
                // InitItem(t.DT, tickBarDt, t, 0, 0);

             //   if (Count > 1)
             //       Evl.AddItem(EvlResult.INFO, EvlSubject.PROGRAMMING,
             //           String.Format("{0}:[{1:T}]", Name, LastItemCompleted.DT), "New", LastItemCompleted.ToString(), "");
            }
            if( Count > 1 && LastItem.DT.CompareTo(new DateTime(1958,5,23,12,0,0)) < 0)
                throw new NullReferenceException(LastItem.DT.ToString());
        }
        /*
        public void UpdateItem(TimeSeriesItem tsi)
        {
            TickCount++;
            LastTickDT = tsi.LastDT; // LastTick DT very Important!

            PreUpdate(tsi);

            TimeSeriesItem lastItem;
            var syncDt = GetBarDateTime2(tsi.DT, TimeIntSeconds);
            if (Count > 0)
            {
                lastItem = LastItem;

                var nSyncDt = DtToLong(syncDt);
                var nlastItemDt = DtToLong(lastItem.DT);

                if (nlastItemDt >= nSyncDt)
                {
                    if (nlastItemDt == nSyncDt)
                    {
                        Calculate(tsi);
                    }
                    else
                    {
                      //  Ticker.EventLog.AddItem(EvlResult.FATAL, EvlSubject.TECHNOLOGY, "BarSeries.TickBar", "New TickBar",
                      //              String.Format("LastBarTime={0:T} > LastTickTime={1:T}", lastItem.DT, b.DT), lastItem.ToString());
                      //  throw new NullReferenceException(String.Format("LastBarTime={0:T} > LastTickTime={1:T}; {2}", lastItem.DT, b.DT, lastItem));
                        throw new NullReferenceException("Sync is Lost"  + Name + Code);
                    }
                }
                else // New Bar should be Added
                {
                    AddNewItem();
                    InitItem(tsi);
                }
            }
            else
            {
                AddNewItem();
                InitItem(tsi);
            }
        }
        */
        public abstract void InitUpdate(DateTime dt);
        /*
        public abstract void InitItem(int ibar, int ilast);
        public abstract void CopyItem(int ibar, int ilast, int previouse);
        public abstract void Calculate2(int ibar, int ilast, int iprevious);
        */
        public virtual  void AddNewItem()
        {
        }

        public virtual void InitUpToDate(){}

        public void UpToDate2(DateTime dt, TimeSeries bars) // UpToDate in the Update
        {
            var lostItem = LastItem;
            var i = 0;
            var cnt = bars.Count;
            while( i < cnt-1 && bars[i].DT > LastItem.DT) i++;
            for (var j = i; j >= 0; j--)
            {
               // InitUpdate(bars[j].DT);

                // !!!!!!!!!!!!!!!!!!!!! Calculate2(j, 0, 1);

                //Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, "Atr.New", "Do Sync",
                //        LastItem.ToString(), _bars[j].ToString());
                if (j > 0) AddNewItem();
            }
            //_lastItem = LastItem as Item;

            Evl.AddItem(EvlResult.FATAL, EvlSubject.PROGRAMMING, Name, Name, "Lost[" + i + "]: " + dt.ToString("H:mm:ss.fff"),
                 "Item: " + lostItem.DT.ToString("H:mm:ss.fff") + " Bar[0]: " + bars[0].DT.ToString("H:mm:ss.fff") + " Bar[1]: " + bars[1].DT.ToString("H:mm:ss.fff"),
                        "Item: " + lostItem + " Bar: " + bars[0]);
        } 

        public virtual void UpToDate()
        {
            var bars = SyncSeries;
            if (bars.Count == 0) return;
            if (bars.Count <= Count) return;

            InitUpToDate();

            Items.Clear();
            AddNewItem();

            // !!!!!!!!!!!!!!!!!!! InitItem(Count-1,0);

            var cnt = bars.Count - 2;
            for (var i = cnt; i >= 0; i--)
            {
                AddNewItem();

               // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Calculate2(i, 0, 1);
            }
            Ticker.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "UpToDate", ToString(), "Count=" + bars.Count);
        }

        //public void FillBars(IEnumerable<DownLoader.Bar> bars)
        //{
        //    if (bars == null) return;
        //    Items.Clear();
        //    foreach (var b in bars.OrderBy(d => d.DT))
        //    {
        //        var ba = new Bar
        //        {
        //            DT = b.DT,
        //            LastDT = b.DT,
        //            SyncDT = b.DT,
        //            Open = b.Open,
        //            High = b.High,
        //            Low = b.Low,
        //            Close = b.Close,
        //            Volume = (double)b.Volume,
        //            SeriesID = Id
        //        };
        //        AddItem(ba);
        //    }
        //    TickCount = bars.Count();
        //}
        public void UpToDateTime(Bars001 masterbs)
        {
            if (masterbs.Count < 1) return;

            Items.Clear();
            var mblast = (IBarSimple)masterbs.LastItem;
            var dt = GetBarDateTime2(mblast.DT, TimeIntSeconds);
            var b = new Bar
            {
               // SeriesID = mblast.SeriesID,
                DT = dt,
                LastDT = dt,
                SyncDT = dt,
                Open = mblast.Open,
                High = mblast.High,
                Low = mblast.Low,
                Close = mblast.Close,
                Volume = mblast.Volume
            };
            foreach (Bar mb in masterbs.Items)
            {
                if (b.DT == TimeSeries.GetBarDateTime2(mb.DT, TimeIntSeconds))
                {
                    b.Open = mb.Open;
                    if (b.High < mb.High) b.High = mb.High;
                    if (b.Low > mb.Low) b.Low = mb.Low;
                    b.Volume += mb.Volume;
                }
                else
                {
                    AddItem(b);
                    dt = GetBarDateTime2(mb.DT, TimeIntSeconds);
                    b = new Bar
                    {
                        SeriesID = mb.SeriesID,
                        DT = dt,
                        LastDT = dt,
                        SyncDT = dt,
                        Open = mb.Open,
                        High = mb.High,
                        Low = mb.Low,
                        Close = mb.Close,
                        Volume = mb.Volume
                    };
                }
            }
        }

        // TimeSeries UpdateItem(TimeSeries tsi)
        /*
        public virtual void PreUpdate(TimeSeriesItem tsi)
        {
        }
        public virtual void Calculate(TimeSeriesItem tsi)
        {
        }
        public virtual void InitItem(TimeSeriesItem tsi)
        {
        }
        */
        // TimeSeries UpdateItem(TimeSeries tsi)

        
        public abstract void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev);
        public abstract void InitItem(ITimeSeriesItem tsi, int isync, int ilast);
        public virtual void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev){}

        public virtual void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
        }

        public virtual void Calculate(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            //if (Count > 1)
            //{
                var last = Items[ilast];

                last.LastDT = tsi.LastDT;
                last.SyncDT = syncDt;

                Calculate(tsi, isync, ilast, iprev);
            //}
          //  else if (Count > 0)
          //      InitItem(syncDt, itemDt, tsi, isync, ilast);
        }
        public virtual void InitItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast)
        {
            var last = Items[ilast];

            last.DT = itemDt;

            last.LastDT = tsi.LastDT;
            last.SyncDT = syncDt;

            InitItem(tsi, isync, ilast);

            if (ilast + 1 >= Count)
                return;

            var previouse = Items[ilast + 1];
            if (previouse.DT.Date < last.DT.Date)
            {
                ItemsDailyCount = 0;
            }
            else
            {
                ++ItemsDailyCount;
            }
        }
        public virtual void CopyItem(DateTime syncDt, DateTime itemDt, TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            if (Count < 2) return;

            var lastItem = Items[ilast];

            lastItem.DT = itemDt;

            lastItem.LastDT = tsi.LastDT;
            lastItem.SyncDT = syncDt;

            CopyItem(tsi, isync, ilast, iprev);
        }

        // Chart Support Functions
        public DateTime GetDateTime(int index)
        {
            return this[index].DT;
        }
        public int GetCount()
        {
            return Count;
        }

        public virtual IList<ILineSeries> ChartLines { get { return null; } }
        public virtual IList<IBandSeries> ChartBands { get { return null; } }
        public virtual IList<ILevel> ChartLevels { get { return null; } }
        public virtual IList<IChartText> ChartTexts { get { return null; } }

        public abstract override string ToString();
        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        public void ClearSomeData(int count)
        {
            while( Count > count )
                Items.RemoveAt(Count-1);
            
            // 2018.05.25
            //Evl.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, Name, "TimeSeries", "ClearSomeData()", 
            //    $"Ticker={Ticker.Code}; TimeInt={TimeIntSeconds}; Capasity={Capasity}; Limit={CapasityLimit}; ItemsCount={Count}", ToString());
        }

        public void Clear()
        {
            Items.Clear();
            SyncDT = DateTime.MinValue;
        }
    }
    public abstract class TimeSeriesNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DateTime _lastSyncDT;
        public DateTime LastSyncDT 
        { 
            get { return _lastSyncDT; }
            set 
            { 
                _lastSyncDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LastSyncTimeStr"));
            }
        }

        private DateTime _lastTickDT;
        public DateTime LastTickDT
        {
            get { return _lastTickDT; }
            set
            {
                _lastTickDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LastTickTimeStr"));
            }
        }

        private DateTime _lastItemDT;
        public DateTime LastItemDT
        {
            get { return _lastItemDT; }
            set
            {
                _lastItemDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LastItemTimeStr"));
            }
        }

        private DateTime _lastItemCompletedDT;
        public DateTime LastItemCompletedDT
        {
            get { return _lastItemCompletedDT; }
            set
            {
                _lastItemCompletedDT = value;
                OnPropertyChanged(new PropertyChangedEventArgs("LastItemCompletedTimeStr"));
            }
        }

        public string LastSyncTimeStr
        {
            get { return _lastSyncDT.ToString("T"); }
        }
        public string LastTickTimeStr
        {
            get { return _lastTickDT.ToString("T"); }
        }
        public string LastItemTimeStr
        {
            get { return _lastItemDT.ToString("T"); }
        }
        public string LastItemCompletedTime
        {
            get { return _lastItemCompletedDT.ToString("T"); }
        }

        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null) PropertyChanged(this, e);
        }
    }
    public abstract class TimeSeriesItem : ITimeSeriesItem
    {
        public long ID { get; protected set; }
        public long SeriesID { get; set; }
        public TimeSeries Series {get;set;}

        public DateTime DT { get; set; }
        public virtual DateTime LastDT { get; set; }
        public virtual DateTime SyncDT { get; set; }

        public TimeSeriesItem SyncItem { get; set; }

        public abstract override string ToString();
        public virtual DateTime GetTimeSeriesItemDateTime(DateTime dt, int timeIntSeconds)
        {
            return TimeSeries.GetBarDateTime2(dt, timeIntSeconds);
        }

        public string Key
        {
            get { return SeriesID + "@" + DT.ToString("s"); }
        }
    }      
#endregion
}
