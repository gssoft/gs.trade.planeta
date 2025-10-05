using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Extension;
using GS.ICharts;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Bars
{
    public abstract class Bars : TimeSeries, IChartable, IBars
    {
        protected Bars()
        {
        }
        protected Bars(string name, ITicker ticker, int timeIntSeconds, int shiftSeconds)
            : base(name, ticker, timeIntSeconds, shiftSeconds)
        {
        }
        protected Bars(string code, string name, Ticker ticker,  int timeIntSeconds, int shiftSeconds)
            : base(code, name, ticker, timeIntSeconds, shiftSeconds)
        {
        }
        /*
        public double Highest(BarValue e, int length)
        {
            var m = double.MinValue;
            switch (e)
            {
                case BarValue.High:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Max(((IBar) (Items[i])).High, m);
                    }
                    break;
                case BarValue.Median:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Max(((IBar)(Items[i])).MedianPrice, m);
                    }
                    break;
                case BarValue.Typical:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Max(((IBar)(Items[i])).TypicalPrice, m);
                    }
                    break;
                case BarValue.Close:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Max(((IBar) (Items[i])).Close, m);
                    }
                    break;
                case BarValue.Low:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Max(((IBar)(Items[i])).Low, m);
                    }
                    break;
                case BarValue.Open:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Max(((IBar)(Items[i])).Open, m);
                    }
                    break;
            }
            return m;
        }
        public double Lowest(BarValue e, int length)
        {
            var m = double.MaxValue;
            switch (e)
            {
                case BarValue.Low:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Min(((IBar)(Items[i])).Low, m);
                    }
                    break;
                case BarValue.Median:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Min(((IBar)(Items[i])).MedianPrice, m);
                    }
                    break;
                case BarValue.Typical:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Min(((IBar)(Items[i])).TypicalPrice, m);
                    }
                    break;
                case BarValue.Close:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Min(((IBar)(Items[i])).Close, m);
                    }
                    break;
                case BarValue.High:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Min(((IBar)(Items[i])).High, m);
                    }
                    break;
                case BarValue.Open:
                    for (var i = 0; i < length && i < Count; i++)
                    {
                        m = Math.Min(((IBar)(Items[i])).Open, m);
                    }
                    break;
                
            }
            return m;
        }
        */
        public double Highest(BarValue be, int startIndex, int length)
        {
            var m = double.MinValue;
            var getValue = GetFuncGetItemValue(be);
            for (var i = startIndex; i < length + startIndex && i < Count; i++)
            {
                m = Math.Max(getValue(i), m);
            }
            return m;
        }
        public double Lowest(BarValue be, int startIndex, int length)
        {
            var m = double.MaxValue;
            var getValue = GetFuncGetItemValue(be);
            
            for (var i = startIndex; i < length + startIndex && i < Count; i++)
            {
                m = Math.Min(getValue(i), m);
            }
            return m;
        }

        public Func<int,double> GetFuncGetItemValue(BarValue be)
        {
            switch( be )
            {
                case BarValue.Open:
                    return GetOpen;
                case BarValue.High:
                    return GetHigh;
                case BarValue.Low:
                    return GetLow;
                case BarValue.Close:
                    return GetClose;
                case BarValue.Typical:
                    return GetTypical;
                case BarValue.Median:
                    return GetMedian;
                default:
                    return GetTypical;
            }
        }

        public IBar Bar(int i)
        {
            return (IBar) this[i];
        }

        public double Open
        {
            get { return LastItemCompleted != null ? ((IBar)LastItemCompleted).Open : 0d; }
        }

        public double High => LastItemCompleted?.High ?? 0d;

        public double Low => LastItemCompleted?.Low ?? 0d;

        public double LastCompletedClose => LastItemCompleted?.Close ?? 0d;

        public double Close
        {
            //get { return LastItemCompleted != null ? LastItemCompleted.Close : 0d; }
            get { return LastItem != null ? LastItem.Close : 0d; }
        }

        public double Volume
        {
            get { return LastItemCompleted != null ? ((IBar)LastItemCompleted).Volume : 0d; }
        }

        public bool IsBlack => Count > 1 && LastItemCompleted.IsBlack;
        public bool IsWhite => Count > 1 && LastItemCompleted.IsWhite;

        public bool IsDoj => Count > 1 && LastItemCompleted.IsDoj;
        public bool IsFaded => Count > 1 && LastItemCompleted.IsFaded;

        public double GetOpen(int i)
        {
            return ((IBar)(Items[i])).Open;
        }
        public double GetHigh(int i)
        {
            return ((IBar)(Items[i])).High;
        }
        public double GetLow(int i)
        {
            return ((IBar)(Items[i])).Low;
        }
        public double GetClose(int i)
        {
            return ((IBar)(Items[i])).Close;
        }
        public double GetVolume(int i)
        {
            return ((IBar)(Items[i])).Volume;
        }
        public double GetTypical(int i)
        {
            return ((IBar)(Items[i])).TypicalPrice;
        }
        public double GetMedian(int i)
        {
            return ((IBar)(Items[i])).MedianPrice;
        }
        public double GetLine(int i)
        {
            return GetClose(i);
        }

        public bool IsNoValid01 (int index)
        {
        return Bar(index).High.IsEquals(Bar(index).Low); 
        }

        public bool IsNoValid02(int index)
        {
            return Bar(index).Close.IsEquals(Bar(index).Open)
                   &&
                   (Bar(index).Close.IsEquals(Bar(index).Low) || Bar(index).Close.IsEquals(Bar(index).High));
        }

        public double TrueRange(int index)
        {
            if (Count > index + 1)
            {
                //  var truehigh = Bar(index + 1).Close > Bar(index).High ? Bar(index + 1).Close : Bar(index).High;
                var truehigh = Bar(index + 1).Close.IsGreaterThan(Bar(index).High) ? Bar(index + 1).Close : Bar(index).High;
           //     var truelow = Bar(index + 1).Close < Bar(index).Low ? Bar(index + 1).Close : Bar(index).Low;
                var truelow = Bar(index + 1).Close.IsLessThan(Bar(index).Low) ? Bar(index + 1).Close : Bar(index).Low;

                double tr = truehigh - truelow;

                //if (IsNoValid01 ( index) )
                //{
                //    //var c = Bar(index).Close;
                //    //var h = Bar(index).High;
                //    //var l = Bar(index).Low;

                //    var tr1 = TrueRange(index + 1);
                //    return tr.IsGreaterThan(tr1) ? tr : tr1;
                //}
                //if ( IsNoValid02(index))
                //{
                //    //var c = Bar(index).Close;
                //    //var h = Bar(index).High;
                //    //var l = Bar(index).Low;

                //    var tr1 = TrueRange(index + 1);
                //    return tr.IsGreaterThan(tr1) ? tr : tr1;
                //}

                return tr;
            }
            if (Count > index)
            {
                return Bar(index).High - Bar(index).Low;
            }
            return 0;
        }
        public new IBar LastItem
        {
            get { return (IBar) base.LastItem; }
        }
        public new IBar LastItemCompleted
        {
            get { return (IBar) base.LastItemCompleted; }
        }

        public IEnumerable<double> OpenValues()
        {
            return Items.Select(t => ((IBar)(t)).Open);
        }
        public IEnumerable<double> HighValues()
        {
            return Items.Select(t => ((IBar)(t)).High);
        }
        public IEnumerable<double> LowValues()
        {
            return Items.Select(t => ((IBar)(t)).Low);
        }
        public IEnumerable<double> CloseValues()
        {
            return Items.Select(t => ((IBar)(t)).Close);
        }
        public IEnumerable<double> TypicalValues()
        {
            return Items.Select(t => ((IBar) (t)).TypicalPrice);
        }
        public IEnumerable<double> MedianValues()
        {
            return Items.Select(t => ((IBar)(t)).MedianPrice);
        }

        // Charts Support
        private ChartBarSeries _chartBarSeries;
        public virtual IChartBarSeries ChartBarSeries
        {
            get { return _chartBarSeries ?? CreateChartBarSeries(); }
        }
        private IChartBarSeries CreateChartBarSeries()
        {
            _chartBarSeries = new ChartBarSeries
                                  {
                                      Name = "Bars",
                                      ColorUp = 0x00ff00,
                                      ColorDown = 0xff0000,
                                      ColorEdge = 0x000000,

                                      CallGetCount = GetCount,
                                      CallGetDateTime = GetDateTime,
                                      CallGetOpen = GetOpen,
                                      CallGetHigh = GetHigh,
                                      CallGetLow = GetLow,
                                      CallGetClose = GetClose,
                                      CallGetVolume = GetVolume,

                                      CallGetLine = GetLine
                                  };
            return _chartBarSeries;
        }

        private IChartDataContainer _chartDataContainer;
        public virtual IChartDataContainer ChartDataContainer
        {
            get { return _chartDataContainer ?? CreateChartDataContainer(); }
        }
        private IChartDataContainer CreateChartDataContainer()
        {
            _chartDataContainer = new ChartDataContainer();
            var chart = new ChartData {Name = "Main", HeightExp = 100};
            chart.ChartBars.Add(ChartBarSeries);
            return _chartDataContainer;
        }


    }
}
