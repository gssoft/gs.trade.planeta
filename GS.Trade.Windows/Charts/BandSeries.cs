using System;
using GS.Extension;
using GS.ICharts;
// using GS.Trade.Data;

namespace GS.Trade.Windows.Charts
{
    public class BandSeries
    {
        //private Strategies.Strategies StrategiesForLoading;

        protected IBandSeries _bandSeries;

        private int _chartDataLength;

        public double[] LineChartData { get; private set; }
        public double[] HighChartData { get; private set; }
        public double[] LowChartData { get; private set; }

        public string Name { get; set; }
        
        public int LineColor { get; set; }
        public int BandLineColor { get; set; }
        public int FillColor { get; set; }

        public bool NeedDrawLine { get; set; }

        public BandSeries(IBandSeries bs)
        {
            _bandSeries = bs;
        }
        public void SetBandSeries(IBandSeries bs)
        {
            _bandSeries = bs;
            if (_chartDataLength != 0)
                SetChartDataLength(_chartDataLength);
        }
        public void SetChartDataLength(int len)
        {
            _chartDataLength = len;

            LineChartData = new double[_chartDataLength];
            HighChartData = new double[_chartDataLength];
            LowChartData = new double[_chartDataLength];
        }
        public virtual void FillData()
        {
            if (_bandSeries == null || LineChartData == null ) return;
            // if (_bandSeries.Count == 0) return;

            var l = LineChartData.Length - 1;
            var length = Math.Min(_bandSeries.Count, LineChartData.Length);
            for (var i = 0; i < length; i++)
            {
                LineChartData[l - i] = _bandSeries.GetLine(i);
                HighChartData[l - i] = _bandSeries.GetHigh(i);
                LowChartData[l - i] = _bandSeries.GetLow(i);
            }
            var d = LineChartData.Length - _bandSeries.Count;
            if (d <= 0) return;
            for (var i = 0; i < d; i++)
            {
                LineChartData[i] = ChartDirector.Chart.NoValue;
                HighChartData[i] = ChartDirector.Chart.NoValue;
                LowChartData[i] = ChartDirector.Chart.NoValue;

            }
        }
    }
    public class Band2Series : BandSeries
    {
        private readonly IBars _bars;

        public Band2Series(IBars bars, IBandSeries bs)
            : base(bs)
        {
            _bars = bars;
        }
        public override void FillData()
        {
            if (_bandSeries == null || LineChartData == null) return;
            //  if (_bandSeries.Count == 0) return;

            // _xBand.Update(_bars.LastTickDT, _evl);

            var l1 = Math.Min(_bandSeries.Count, _bars.Count);
            var length = Math.Min(l1, LineChartData.Length);

            var l = LineChartData.Length - 1;

            // var bandDt = _bandSeries.GetDateTime(0);
            // var barDt = TimeSeries.GetBarDateTime2(_bars[0].DT, _bandSeries.TimeIntSeconds);
            // if (TimeSeries.DtToLong(barDt) > TimeSeries.DtToLong(bandDt)) return;
            if (_bandSeries.Count > 0 && _bars.Count > 0)
            {
                var bandDt = _bandSeries.GetDateTime(0);
                // 15.11.11
                var barDt = (_bars.Bar(0).DT).ToBarDateTime2(_bandSeries.TimeIntSeconds);
                // if (TimeSeries.DtToLong(barDt) > TimeSeries.DtToLong(bandDt)) return;
                if (barDt.ToLongInSec() > bandDt.ToLongInSec())
                        return;
            }
            var j = 0;
            for (var i = 0; i < length; i++)
            {
                //    var bandDt = _bandSeries.GetDateTime(0);
                //    var barDt = TimeSeries.GetBarDateTime2(_bars[0].DT, _bandSeries.TimeIntSeconds);
                //    if (TimeSeries.DtToLong(barDt) > TimeSeries.DtToLong(bandDt)) return;

                // 15.11.11
                //while (TimeSeries.DtToLong(_bandSeries.GetDateTime(i)) == TimeSeries.DtToLong(TimeSeries.GetBarDateTime2(_bars.Bar(j).DT, _bandSeries.TimeIntSeconds))
                while (_bandSeries.GetDateTime(i).ToLongInSec() == (_bars.Bar(j).DT).ToBarDateTime2(_bandSeries.TimeIntSeconds).ToLongInSec() && l - j >= 0)
                {
                    LineChartData[l - j] = _bandSeries.GetLine(i);
                    HighChartData[l - j] = _bandSeries.GetHigh(i);
                    LowChartData[l - j] = _bandSeries.GetLow(i);
                    j++;
                    if (_bars.Count <= j) break;
                    // barDt = TimeSeries.GetBarDateTime2(_bars[j].DT, _bandSeries.TimeIntSeconds);
                }
                if (_bars.Count <= j) break;
                // if (j == 0)
                //     _evl.AddItem(EnumEventLog.FATAL, EnumEventLogSubject.PROGRAMM, "BandUpdate", bandDt.CompareTo(barDt) + " " + bandDt.ToString("HH:mm:ss:fffff") + " " + barDt.ToString("HH:mm:ss:fffff") + " l-j=" + (l - j),
                //         _bars[j].ToString(), _xBand[i].ToString());
            }
            for (; l - j >= 0; j++)
            {
                /*
                LineChartData[l - j] = _bars.FirstBaseValue;
                HighChartData[l - j] = _bars.FirstBaseValue;
                LowChartData[l - j] = _bars.FirstBaseValue;
              */
                LineChartData[l - j] = ChartDirector.Chart.NoValue;
                HighChartData[l - j] = ChartDirector.Chart.NoValue;
                LowChartData[l - j] = ChartDirector.Chart.NoValue;
            }
        }
    }
}