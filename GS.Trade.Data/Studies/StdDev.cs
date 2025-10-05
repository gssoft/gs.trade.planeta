using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies.Averages;

namespace GS.Trade.Data.Studies
{
    public class StdDev : TimeSeries
    {
        private readonly Int32 _length;
        private readonly Int32 _smoothDevLength;

        private Average _avg;
        private Bars001 _bars;

        private Func<int, double> GetBarValue;

        private readonly BarValue _devBarValue;

        public StdDev(string name, ITicker ticker, Average avg, BarValue devBarValue, int devLength, int smoothDevLength)
            : base(name, ticker, avg.TimeIntSeconds, avg.ShiftIntSecond)
        {
            SyncSeries = avg;
            _avg = avg;
            _bars = avg.SyncSeries as Bars001;
            if( _avg == null || _bars == null)
                throw new NullReferenceException("bars is Null");

            _devBarValue = devBarValue;
            _length = devLength == 0 ? _avg.Length : devLength;
            _smoothDevLength = smoothDevLength;

            GetBarValue = _bars.GetFuncGetItemValue(_devBarValue);

            DebudAddITem = true;
        }

        public StdDev(string name, ITicker ticker, int timeIntSeconds, int length, int smoothDevLength)
            : base(name, ticker, timeIntSeconds)
        {
            _devBarValue = BarValue.Typical;
            _length = length;
            _smoothDevLength = smoothDevLength;

            GetBarValue = _bars.GetFuncGetItemValue(_devBarValue);
        }
        public override void Init()
        {
            if (SyncSeries != null) return;
            SyncSeries = Ticker.RegisterTimeSeries(new SAverage("xAvg", Ticker, TimeIntSeconds, _devBarValue, _length, _smoothDevLength)) as SAverage;
            if (SyncSeries == null)
                throw new NullReferenceException("SAverage == null");
            _avg = SyncSeries as Average;
            _bars = SyncSeries.SyncSeries as Bars001;
            if( _avg == null || _bars == null)
                throw new NullReferenceException("XAverage or Bars == null");

            
            DebudAddITem = true;
        }
        public override void InitUpdate(DateTime dt)
        {
            _avg.Update(dt);
        }

        public override void AddNewItem()
        {
            AddItem(new Item());
        }
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)_bars[isync];
            //var last = (Item)(Items[ilast]);
            if( iprev > 0)
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            StdDev = ((Item)(Items[ilast])).StdDev,
                            Ma = ((Item)(Items[ilast])).Ma
                        }
                );
            else
            {
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            StdDev = ((float)b.High - (float)b.Low),
                            Ma = ((Average.Item)(_avg[isync])).Ma
                        }
                );
            }
        }
        /*
        private void Calculate(int isync, int ilast)
        {
            var b = (IBar)_bars[isync];
            var last = (Item)(Items[ilast]);

            var avg = _avg.GetMa(isync);
            var cnt = Math.Min(_bars.Count, _length);
            double s = 0;
            for (var i = isync; i < isync + cnt; i++)
                s += (((IBar)(_bars[i])).MedianPrice - avg) * (((IBar)(_bars[i])).MedianPrice - avg);

            last.StdDev = (float)Math.Sqrt(s / cnt);
            last.Avrg = (float)avg;
        }
        */
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            // var b = (IBar)_bars[isync];
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            var avg = _avg.GetMa(isync);
            var cnt = Math.Min(_bars.Count, _length + isync);
            double s = 0;
            var j = 0;
            for (var i = isync; i < cnt; i++)
            {
                j++;
                var v = GetBarValue(i);
                s += (v - avg) * (v - avg);
            }
            var stdDev = j > 0 ? (float)Math.Sqrt(s / j) : prev.StdDev;
            last.StdDev = _smoothDevLength == 0
                              ? stdDev
                              : XAverage.Calculate(_smoothDevLength, stdDev, prev.StdDev);
            last.Ma = (float)avg;
            last.Cnt = j;
        }
        public  void Calculate0(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
           // var b = (IBar)_bars[isync];
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            var avg = _avg.GetMa(isync);
            var cnt = Math.Min(_bars.Count, _length+isync);
            double s = 0;
            var j = 0;
            switch (_devBarValue)
            {
                case BarValue.Median:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (((IBar)(_bars[i])).MedianPrice - avg) * (((IBar)(_bars[i])).MedianPrice - avg);
                    }
                    break;
                case BarValue.Typical:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (((IBar)(_bars[i])).TypicalPrice - avg) * (((IBar)(_bars[i])).TypicalPrice - avg);
                    }
                    break;
                case BarValue.Close:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (((IBar)(_bars[i])).Close - avg) * (((IBar)(_bars[i])).Close - avg);
                    }
                    break;
                default:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (((IBar)(_bars[i])).Close - avg) * (((IBar)(_bars[i])).Close - avg);
                    }
                    break;
            }

            var stdDev = j > 0 ? (float) Math.Sqrt(s/j) : prev.StdDev;
            last.StdDev = _smoothDevLength == 0
                              ? stdDev
                              : XAverage.Calculate(_smoothDevLength, stdDev, prev.StdDev);
            last.Ma = (float)avg;
            last.Cnt = j;
        }

        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var b = (IBar)_bars[isync];
            var last = (Item)(Items[ilast]);
            
            last.StdDev = ((float) b.High - (float) b.Low); // /2f;
            last.Ma = ((Average.Item) (_avg[isync])).Ma;
        }

        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            last.StdDev = prev.StdDev;
            last.Ma = prev.Ma;
            last.Cnt = prev.Cnt;
        }

        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};" + 
                                                   "Length={4};Smooth={5}]",
                                       GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond,
                                                  _length, _smoothDevLength);
            }
        }
        public override string ToString()
        {
            return String.Format("[Key={0};Count={1}]", Key, Count);
        }

        public class Item : TimeSeriesItem
        {
            public float StdDev { get; set; }
            public float Ma { get; set; }
            public int Cnt { get; set; }

            public override string ToString()
            {
                return String.Format("[Type={0};DT={1:T};StdDev={2:N3};Ma={3:N3};Cnt={4};SyncTime={5:T}",
                                        GetType(), DT, StdDev, Ma, Cnt, SyncDT);
            }
        }
    }
}
