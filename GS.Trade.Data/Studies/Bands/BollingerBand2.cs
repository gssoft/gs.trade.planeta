using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Studies.Averages;

namespace GS.Trade.Data.Studies.Bands
{
    public class BollingerBand2 : Band1
    {
        private Average _avrg;
        private StdDev _stdDev;

        public BollingerBand2(string name, Ticker ticker, int timeIntSeconds, BarValue be, int length, float kStdDevUp, float kStdDevDown) 
                : base(name, ticker, timeIntSeconds)
        {
            MaBarValue = be;
            MaLength = length;

            DevBarValue = be;
            DevLength = length;

            KDevUp = kStdDevUp;
            KDevDown = kStdDevDown;
        }

        public override void Init()
        {
            if (_avrg != null) return;
            _avrg = (SAverage)Ticker.RegisterTimeSeries(new SAverage("SAvrg", Ticker, TimeIntSeconds, MaBarValue, MaLength, MaSmoothLength));
            if (_avrg == null)
                throw new NullReferenceException("Avrg is NUll");
            _stdDev = (StdDev) Ticker.RegisterTimeSeries(new StdDev("StdDev", Ticker, _avrg, DevBarValue, DevLength, 0 ));
            if(_stdDev == null)
                throw new NullReferenceException("StdDev is NUll");
            SyncSeries = _stdDev;
        }

        public override void InitUpdate(DateTime dt)
        {
            _avrg.Update(dt);
            _stdDev.Update(dt);
        }

        public override void AddNewItem()
        {
            throw new NotImplementedException();
        }
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var stdDev = (StdDev.Item)_stdDev[isync];
            //var last = (Item)(Items[ilast]);

            if( iprev > 0) // Copy Operation Last to New Item
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            Ma = ((Item)(Items[ilast])).Ma,
                            High = ((Item)(Items[ilast])).High,
                            Low = ((Item)(Items[ilast])).Low
                         }
                    );
            
            else // Initialization Operation from SyncSeries
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            Ma = stdDev.Ma,
                            High = stdDev.Ma + KDevUp * stdDev.StdDev,
                            Low = stdDev.Ma - KDevDown * stdDev.StdDev
                        }
                    );
        }

        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);

            var stdDev = (StdDev.Item)_stdDev[isync];

            last.Ma = stdDev.Ma;
            last.High = last.Ma + KDevUp * stdDev.StdDev;
            last.Low = last.Ma - KDevDown * stdDev.StdDev;
        }

        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var last = (Item)(Items[ilast]);

            var stdDev = (StdDev.Item)_stdDev[isync];

            last.Ma = stdDev.Ma;
            last.High = last.Ma + KDevUp * stdDev.StdDev;
            last.Low = last.Ma - KDevDown * stdDev.StdDev;
        }

        public override void CopyItem(TimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            throw new NotImplementedException();
        }
    }
}
