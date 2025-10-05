using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.ICharts;
using GS.Trade.Data.Chart;
using GS.Trade.Data.Studies.Averages;

namespace GS.Trade.Data.Studies.Bands
{
    public abstract class Band : TimeSeries
    {
        public float KDevUp { get; set; }
        public float KDevDown { get; set; }

        protected BarValue MaBarValue { get; set; }
        protected MaType MaType { get; set; }
        protected Int32 MaLength { get; set; }
        protected Int32 MaSmoothLength { get; set; }

        protected BarValue DevBarValue { get; set; }
        protected Int32 DevLength { get; set; }
        protected Int32 DevSmoothLength { get; set; }

        public Average Avrg {get; protected set;}

        protected Band(string name, ITicker ticker, int timeIntSeconds)
            : base(name, ticker, timeIntSeconds)
        {
        }

        public abstract double GetMa(int i);
        public abstract double GetHigh(int i);
        public abstract double GetLow(int i);

       // public abstract double GetDeviation(int i);
       // public abstract double GetDeviationUp(int i );
       // public abstract double GetDeviationDown(int i);

        public override IList<ILineSeries> ChartLines { get { return CreateChartLineList(); } }
        private IList<ILineSeries> CreateChartLineList()
        {
            return new List<ILineSeries>
                       {
                           new ChartLine
                               {
                                   Name = "L." + Name,
                                   Color = 0x00ff00,
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa
                               }
                       };
        }

        public override IList<IBandSeries> ChartBands { get { return CreateChartBandList(); } }
        private IList<IBandSeries> CreateChartBandList()
        {
            return new List<IBandSeries>
                       {
                           new ChartBand
                               {
                                   BandName="B."+ Name,
                                   BandColor = 0xff3333,
                                   BandLineColor = 0xff0000,
                                   BandFillColor = unchecked((int)0xc0ff1111),
                                   CallGetCount = GetCount,
                                   CallGetDateTime = GetDateTime,
                                   CallGetLine = GetMa,
                                   CallGetHigh = GetHigh,
                                   CallGetLow = GetLow
                               }
                       };
        }
    }
}
