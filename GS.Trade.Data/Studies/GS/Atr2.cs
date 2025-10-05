using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Extension;
using GS.ICharts;
using GS.Interfaces;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.GS
{
    public class Atr2 : Atr
    {
        private Bars001 _bars;
        private Atr _atr1;
        private Atr _atr2;

        private readonly int _atrLength1;
        private readonly int _atrLength2;

        //public override double LastAtr => Atr?.Count > 0
        //                ? (((Atr.Item)LastItem)?.Atr ?? 0)
        //                : 0;
        //public override double LastAtrCompleted => Atr.Count > 1
        //                ? (((Atr.Item)LastItemCompleted)?.Atr ?? 0)
        //                : 0;
        public override double LastAtr => Atr?.LastAtr ?? 0;
        public override double LastAtrCompleted => Atr?.LastAtrCompleted ?? 0;

        public bool IsFastHigher => _atr1 != null && _atr2 != null &&
                                    // _atr1.Count > 1 && _atr2.Count > 1 &&
                                    _atr1.LastAtrCompleted.IsGreaterThan(_atr2.LastAtrCompleted);
        public bool IsSlowHigher => _atr1 != null && _atr2 != null &&
                                    // _atr1.Count > 1 && _atr2.Count > 1 &&
                                    _atr1.LastAtrCompleted.IsLessThan(_atr2.LastAtrCompleted);

        public Atr Atr { get; private set; }

        public Atr2(string name, ITicker ticker, int timeIntSeconds, int length1, int length2)
            : base(name, ticker, timeIntSeconds, length1)
        {
            _atrLength1 = length1;
            _atrLength2 = length2;
        }
        public override ITimeSeriesItem this[int index] => 
            index >= 0 && index < Atr?.Count ? Atr?.Items[index] : null;

        public override void Init()
        {
            if (SyncSeries != null) return;
            //_bars = Ticker.GetBarSeries("TypeName=Bar;TimeIntSeconds=" + TimeIntSeconds);
            // _bars = Ticker.GetBarSeries("TypeName=Bar;TimeIntSeconds=15");
            //_bars = Ticker.GetTimeSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond));

            _bars = Ticker.RegisterAsyncSeries(new Bars001("Bars", Ticker, TimeIntSeconds, ShiftIntSecond)) as Bars001;

            _atr1 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr.1", Ticker, TimeIntSeconds, _atrLength1));
            _atr2 = (Atr)Ticker.RegisterTimeSeries(new Atr("MyAtr.2", Ticker, TimeIntSeconds, _atrLength2));
            Atr = _atr1;

            if (_bars == null || _atr1 == null || _atr2 == null)
                throw new NullReferenceException("Atr Init Bar == null or Atr == null");
            _atr1.Init();
            _atr2.Init();

            SyncSeries = _atr1;
            //  UpToDate();
        }
        /*
        public override double FirstBaseValue
        {
            get { return _bars.FirstBaseValue; }
        }
        */
        public override void InitUpdate(DateTime dt)
        {
            //_atr1.Update(dt);
            //_atr2.Update(dt);
            //_atr = _atr1.LastAtrCompleted.IsLessThan(_atr2.LastAtrCompleted)
            //        ? _atr2
            //        : _atr1
            //        ;
        }

        public override void Update(DateTime dt)
        {
            _atr1.Update(dt);
            _atr2.Update(dt);
           // Atr = _atr1.LastAtrCompleted.IsLessThan(_atr2.LastAtrCompleted)
           Atr = IsSlowHigher ? _atr2 : _atr1;
        }

    
        private IList<ILineSeries> _chartLines;
        public override IList<ILineSeries> ChartLines => _chartLines ?? CreateChartLines();

        private IList<ILineSeries> CreateChartLines()
        {
            var chl = new List<ILineSeries>
                       {
                        //  28.04.2018
                           //new ChartLine
                           //    {
                           //        Name = $"Atr1({_atr1.Length},0)",
                           //        //Color =  0xff0000,
                           //        CallGetColor = () => _atr1.Equals(Atr) ? 0x0000ff :  0xff0000,
                           //        CallGetCount = _atr1.GetCount,
                           //        CallGetDateTime = _atr1.GetDateTime,
                           //        CallGetLine = _atr1.GetAtr
                           //    },
                           //   new ChartLine
                           //    {
                           //        Name = $"Atr2({_atr2.Length},0)",
                           //       // Color = 0xff0000,
                           //        CallGetColor = () => _atr2.Equals(Atr) ? 0x0000ff :  0xff0000,
                           //        CallGetCount = _atr2.GetCount,
                           //        CallGetDateTime = _atr2.GetDateTime,
                           //        CallGetLine = _atr2.GetAtr
                           //    }
                           // 28.04.2108
                              new ChartLine
                               {
                                   Name = $"Atr1({_atr1.Length},0)",
                                   //Color =  0xff0000,
                                   // CallGetColor = () => _atr1.Equals(Atr) ? 0x0000ff :  0xff0000,
                                   CallGetColor = () => IsFastHigher ? 0x0000ff :  0xff0000,
                                   CallGetCount = _atr1.GetCount,
                                   CallGetDateTime = _atr1.GetDateTime,
                                   CallGetLine = _atr1.GetAtr
                               },
                              new ChartLine
                               {
                                   Name = $"Atr2({_atr2.Length},0)",
                                  // Color = 0xff0000,
                                  // CallGetColor = () => _atr2.Equals(Atr) ? 0x0000ff :  0xff0000,
                                   CallGetColor = () => IsSlowHigher ? 0x0000ff :  0xff0000,
                                   CallGetCount = _atr2.GetCount,
                                   CallGetDateTime = _atr2.GetDateTime,
                                   CallGetLine = _atr2.GetAtr
                               }
                              // ,
                              //new ChartLine
                              // {
                              //     Name = String.Format("Atr3({0},0)", _atr.Length),
                              //     Color = 0x0000ff,
                              //     CallGetCount = () => _atr.GetCount(),
                              //     CallGetDateTime = (i) => _atr.GetDateTime(i),
                              //     CallGetLine = (i) => _atr.GetAtr(i)
                              // }
                       };
            _chartLines = chl;
            return _chartLines;
        }
        
        public override string Key =>
            $"[Type={GetType()};Ticker={Ticker.Code};" +
            $"TimeIntSeconds={TimeIntSeconds};Length1={_atrLength1};Length2={_atrLength2}]"
            ;

        public override string ToString() => 
            $"Type={GetType()};Name={Name},Ticker={Ticker.Code}," +
            $"TimeInt={TimeIntSeconds},Length1={_atrLength1};Length2={_atrLength2};" +
            $"ItemsCount={Items.Count}";
    }
}
