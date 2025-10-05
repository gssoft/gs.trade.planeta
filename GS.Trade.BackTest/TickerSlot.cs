using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Elements;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Dto;
using GS.Trade.Strategies;
using Bar = GS.Trade.Data.Bars.Bar;

namespace GS.Trade.BackTest
{
    public class TickerSlot : Element1<string>
    {
        public BackTest7 BackTest { get; set; }
        public const int MaxGetBarSeriesCnt = 15;
        public TimeSeriesStat TimeSeriesStat { get; set; }

        // private const int EndOfSessionTime = 184500;
        private const int EndOfSessionTime = 234500;

        public ITicker Ticker;
        private readonly IList<Bar> _bars = new List<Bar>();
        public DateTime DateTimeFrom { get; set; }
        public DateTime DateTimeTo { get; set; }
        public int Days { get; set; }

        private int _barsIndex;
        private DateTime _dateIndex;

        public Bar LastBar { get; set; }
        public bool EndOfSession => (LastBar != null && LastBar.DT.TimeToInt() > EndOfSessionTime) || LastBar == null;

        public bool EndOfBarSeries { get; set; }

        public List<Strategy> Strategies = new List<Strategy>();

        public override void Init()
        {
            DateTimeFrom = DateTimeTo.AddDays(-(Days-1));
            _dateIndex = DateTimeFrom;
            // 15.10.03
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "BackTest3.TickerSlot:", Key, "Init().GetTimeSeriesStat()","","");
            TimeSeriesStat = Ticker.GetTimeSeriesStat(5);
            if (TimeSeriesStat == null)
                    throw new NullReferenceException($"TickerSlot: Ticker: {Ticker.Code}; TimeSeriesStat is Null");
            Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "BackTest3.TickerSlot:", Key, "GetTimeSeriesStat()","TimeSeriesStat:", TimeSeriesStat.ToString());
            _dateIndex = TimeSeriesStat.FirstDate.Date;
            DateTimeFrom = _dateIndex;
            DateTimeTo = TimeSeriesStat.LastDate.Date;
        }

        public override void Init(GS.Interfaces.IEventLog eventLog)
        {
            base.Init(eventLog);
            Init();
        }
        public int BarsCount => _bars.Count;

        public Bar GetNextBar1()
        {
            if (_barsIndex < BarsCount)
            {
                var b = _bars[_barsIndex];
                _barsIndex++;
                return b;
            }
            return null;
            // No mach Bars
            // return GetBars2() ? _bars[_barsIndex] : null;
        }
        public Bar GetNextBar()
        {
            if (_barsIndex < BarsCount)
            {
                var b = _bars[_barsIndex];
                _barsIndex++;
                return b;
            }
            // No mach Bars
            return GetBars2() ? _bars[_barsIndex] : null;
        }

        private bool GetBars()
        {
            _bars.Clear();
            var bs = Ticker.GetSeries(5);
            if(!bs.Any())
                return false;
            foreach (var nb in bs.Select(b => new Bar
            {
                DT = b.DT,
                LastDT = b.DT,
                Open = b.Open,
                High = b.High,
                Low = b.Low,
                Close = b.Close,
                Volume = b.Volume,
                Ticks = b.Ticks
            }))
            {
                _bars.Add(nb);
            }
            _barsIndex = 0;
            return true;
        }
        private bool GetBars2()
        {
            try
            {
                _bars.Clear();

                if (_dateIndex.CompareTo(DateTimeTo) > 0)
                    return false;
                //  var bs = Ticker.GetSeries(5, _dateIndex);
                IEnumerable<IBarSimple> bs = null;
                // var tryCnt = 3;
                //while (bs == null && tryCnt-- > 0)
                //{
                //    bs = Ticker.GetSeries(5, _dateIndex);
                //}
                //_dateIndex = _dateIndex.Inc();

                // while (bs == null || !bs.Any())
                while (bs == null || bs.Count() <= 0)
                {
                    bs = Ticker.GetSeries(5, _dateIndex);
                    //if (bs == null)
                    //{
                    //    var tryCnt = MaxGetBarSeriesCnt;
                    //    while (bs == null && tryCnt-- > 0)
                    //    {
                    //        bs = Ticker.GetSeries(5, _dateIndex);
                    //    }
                    //}
                    if (bs != null && bs.Count() > 0)
                        break;

                    _dateIndex = _dateIndex.Inc();
                    if (_dateIndex.CompareTo(DateTimeTo) > 0)
                        // break;
                        // 21.04.2018
                        return false;
                }
                _dateIndex = _dateIndex.Inc();
                //_dateIndex = _dateIndex.Inc();
                // 21.04/2018
                //if (bs == null || bs.Count() <= 0)
                //    return false;

                foreach (var nb in bs.Select(b => new Bar
                {
                    DT = b.DT,
                    LastDT = b.DT,
                    Open = b.Open,
                    High = b.High,
                    Low = b.Low,
                    Close = b.Close,
                    Volume = b.Volume,
                    Ticks = b.Ticks
                }))
                {
                    _bars.Add(nb);
                }
                _barsIndex = 0;
                return true;
            }
            catch (Exception ex)
            {
                var e = ex.ExceptionMessageAgg();
                var e1 = ex.ExceptionMessage();
                SendException(ex);
                return false;
            }
        }

        public override string Key => Code + (Ticker!=null?"@"+Ticker.Code:"");

        private void FinishStrategies(Bar bar)
        {
            if (bar == null)
                return;
            var so = Strategies.Where(s => s.Position.IsOpened).ToList();
            while (so.Any())
            {
                foreach (var s in so)
                {
                    s.Finish();
                    while (s.Position.IsOpened)
                    {
                        BackTest.ExecuteBar(bar, this);

                        bar.High = bar.High + Ticker.MinMove;
                        bar.Low = bar.Low - Ticker.MinMove;
                    }
                }
                so = Strategies.Where(s => s.Position.IsOpened).ToList();
            }
        }

        public void Main()
        {
            if (EndOfBarSeries)
            {
                if (!EndOfBarSeries)
                    //continue;
                    //_doWorkStatus = false;
                    //break;
                    return;
            }
            var bar = GetNextBar();
            if (bar == null)
            {
                // Nothing Bars else in this TickerSlot
                EndOfBarSeries = true;
                FinishStrategies(LastBar);
            }
            else
            {
                if (LastBar != null && LastBar.DT.DateToInt() != bar.DT.DateToInt())
                    FinishStrategies(LastBar);

                LastBar = bar;
                // Start Execution
                BackTest.ExecuteBar(bar, this);
                Ticker.UpdateAsyncSeries2(bar);
                Ticker.UpdateTimeSeries(bar.DT);

                foreach (var s in Strategies)
                {
                    if (s.Bars != null && s.Bars.Count < BackTest.MinBarsCountToStart)
                        continue;
                    if (BackTest.TradesNumber <= 0)
                        s.MainBase();
                    else
                    {
                        if (BackTest.AllHaveSameTradesNumber)
                        {
                            if (s.PositionTotal.Quantity < BackTest.TradesNumber)
                            {
                                s.MainBase();
                            }
                            else if (s.Position.IsOpened)
                            {
                                s.Finish();
                                BackTest.ExecuteBar(LastBar, this);
                            }
                        }
                        else
                        {
                            if (EndOfSession)
                            {
                                if (!s.Position.IsOpened)
                                    continue;

                                s.Finish();
                                BackTest.ExecuteBar(LastBar, this);
                            }
                            else
                            {
                                s.EntryEnabled = true;
                                bool toContinue =
                                    Strategies.Any(sp => sp.PositionTotal.Quantity < BackTest.TradesNumber);
                                if (toContinue)
                                {
                                    s.MainBase();
                                    // ii++;
                                }
                                // Finish when Max Trades NUmber is Reached
                                else if (s.Position.IsOpened)
                                {
                                    s.Finish();
                                    BackTest.ExecuteBar(LastBar, this);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
