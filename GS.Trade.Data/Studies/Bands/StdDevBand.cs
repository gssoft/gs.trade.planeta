using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies.Averages;

namespace GS.Trade.Data.Studies.Bands
{
    public class StdDevBand : Band2
    {
        private Bars001 _bars;

        private Func<int, double> _getBarValue;

        public StdDevBand(string name, Ticker ticker, int timeIntSeconds, Average avg, float kDevUp, float kDevDown)
            : base(name, ticker, timeIntSeconds)
        {
            if (avg == null)
                throw new NullReferenceException();

            Avrg = avg;
            KDevDown = kDevDown;
            KDevUp = kDevUp;

            MaBarValue = Avrg.BarValue;
            MaType = Avrg.MaType;
            MaLength = Avrg.Length;
            MaSmoothLength = Avrg.SmoothLength;

            DevBarValue = Avrg.BarValue;
            DevLength = Avrg.Length;
            DevSmoothLength = 0;
        }
        public StdDevBand(string name, Ticker ticker, int timeIntSeconds, Average avg, 
                                           BarValue devBarValue, int devLength, int devSmoothLength, float kDevUp, float kDevDown)
            : base(name, ticker, timeIntSeconds)
        {
            if (avg == null)
                throw new NullReferenceException();

            Avrg = avg;
            KDevDown = kDevDown;
            KDevUp = kDevUp;

            MaBarValue = Avrg.BarValue;
            MaType = Avrg.MaType;
            MaLength = Avrg.Length;
            MaSmoothLength = Avrg.SmoothLength;

            DevBarValue = devBarValue;
            DevLength = devLength;
            DevSmoothLength = devSmoothLength;
        }
        public StdDevBand(string name, ITicker ticker, int timeIntSeconds,
                                        BarValue maBarValue, MaType maType, int maLength, int maSmoothLength,
                                        BarValue devBarValue, int devLength, int devSmoothLength,
                                        float kDevUp, float kDevDown)
            : base(name, ticker, timeIntSeconds)
        {
            KDevDown = kDevDown;
            KDevUp = kDevUp;
            Avrg = (Average)Ticker.RegisterTimeSeries(
                                        Average.GetInstance(maType, "Avg", Ticker, timeIntSeconds,
                                                                 maBarValue, maLength, maSmoothLength));
            if (Avrg == null)
                throw new NullReferenceException();

            MaBarValue = Avrg.BarValue;
            MaType = Avrg.MaType;
            MaLength = Avrg.Length;
            MaSmoothLength = Avrg.SmoothLength;

            DevBarValue = devBarValue;
            DevLength = devLength;
            DevSmoothLength = devSmoothLength;
        }


        public override void Init()
        {
            SyncSeries = Avrg;
            _bars = Avrg.SyncSeries as Bars001;
            if (_bars == null)
                throw new NullReferenceException("bars is Null");
            Bars = _bars;
            _getBarValue = _bars.GetFuncGetItemValue(DevBarValue);
            DebudAddITem = true;
        }

        public override void InitUpdate(DateTime dt)
        {
            Avrg.Update(dt);
        }

        
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)_bars[isync];
            //var last = (Item)(Items[ilast]);
            if (iprev > 0)
                AddItem(
                    new Item
                    {
                        DT = itemDt,
                        LastDT = tsi.LastDT,
                        SyncDT = syncDt,

                        Deviation = ((Item)(Items[ilast])).Deviation,
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

                        Deviation = ((float)b.High - (float)b.Low),
                        Ma = ((Average.Item)(Avrg[isync])).Ma
                    }
                );
            }
        }

        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            var avg = Avrg.GetMa(isync);

            var cnt = Math.Min(_bars.Count, DevLength + isync);
            double s = 0;
            var j = 0;
            for (var i = isync; i < cnt; i++)
            {
                j++;
                var v = _getBarValue(i);
                s += (v - avg) * (v - avg);
            }
            /*
            switch (DevBarValue)
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
            */

            var stdDev = j > 0 ? (float)Math.Sqrt(s / j) : prev.Deviation;
            last.Deviation = DevSmoothLength == 0
                              ? stdDev
                              : XAverage.Calculate(DevSmoothLength, stdDev, prev.Deviation);
            last.Ma = (float)avg;
           // last.Cnt = j;
        }

        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var b = (IBar)_bars[isync];
            var last = (Item)(Items[ilast]);

            last.Deviation = ((float)b.High - (float)b.Low);
            last.Ma = ((Average.Item)(Avrg[isync])).Ma;
        }

        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            throw new NotImplementedException();
        }
    }
}
