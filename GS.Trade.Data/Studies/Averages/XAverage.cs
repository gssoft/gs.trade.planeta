using System;
using System.Collections.Generic;
using GS.ICharts;
using GS.Trade.Data.Bars;
using GS.Trade.Data.Chart;

namespace GS.Trade.Data.Studies.Averages
{
    public class XAverage : Average
    {
        private readonly float _k;
        public int Color;

        public XAverage(string name, ITicker ticker, int timeIntSeconds, BarValue be, int length, int smoothLength)
            : base(name, MaType.Exponential, ticker, be, timeIntSeconds, length, smoothLength)
        {
            _k = 2f / (Length + 1);
            DebudAddITem = true;
        }
        public static float Calculate(int length, float newValue, float previousValue)
        {
            var k = (float)(2.0 / (length + 1));
            return (float)(k * newValue + (1.0 - k) * previousValue);
        }

        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)SyncSeries[isync];
            AddItem(
                new Item
                    {
                        DT = itemDt,
                        LastDT = tsi.LastDT,
                        SyncDT = syncDt,

                        Ma = iprev == 0 ? GetBarValue(b) : ((Item) (Items[ilast])).Ma
                    }
                );
        }
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)SyncSeries[isync];
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            var ma = _k * GetBarValue(b) + (1.0f - _k) * prev.Ma;
            
            last.Ma = SmoothLength == 0
                              ? ma
                              : Calculate(SmoothLength, ma, prev.Ma);
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var b = (Bar)SyncSeries[isync];
            var last = (Item)(Items[ilast]);

            last.Ma = GetBarValue(b);
        }
        
        public override string Key
        {
            get
            {
                return String.Format("[Type={0};Ticker={1};TimeIntSeconds={2};ShiftIntSecond={3};Length={4}]",
                                                  GetType(), Ticker.Code, TimeIntSeconds, ShiftIntSecond, Length);
            }
        }
        public override string ToString()
        {
            return String.Format("Type={0};Name={1};Ticker={2};TimeIntSecond={3};ShiftIntSecond={4};Length={5};ItemsCount={6}",
                GetType(), Name, Ticker.Code, TimeIntSeconds, ShiftIntSecond, Length, Count);
        }
    }
}
