using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Trade.Data.Bars;

namespace GS.Trade.Data.Studies.Averages
{
    public class SAverage : Average
    {
        public int Color;

        public SAverage(string name, ITicker ticker, int timeIntSeconds, BarValue be, int length, int smoothLength)
            : base(name, MaType.Simple, ticker, be, timeIntSeconds, length, smoothLength)
        {
            DebudAddITem = true;
        }
        public static float Calculate(int length, float newValue, float previousValue)
        {
          //  var k = (float)(2.0 / (length + 1));
          //  return (float)(k * newValue + (1.0 - k) * previousValue);
            return 0f;
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
                    Ma = iprev == 0 ? GetBarValue(b) : ((Item)(Items[ilast])).Ma
                }
                );
        }
        
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);
            var bars = (Bars001)SyncSeries;

            var cnt = Math.Min(bars.Count, Length + isync);
            var s = 0f;
            var j = 0;
            switch (BarValue)
            {
                case BarValue.Median:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).MedianPrice;
                    }
                    break;
                case BarValue.Typical:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).TypicalPrice;
                    }
                    break;
                case BarValue.Close:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).Close;
                    }
                    break;
                case BarValue.High:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).High;
                    }
                    break;
                case BarValue.Low:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).Low;
                    }
                    break;
                case BarValue.Open:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).Open;
                    }
                    break;
                default:
                    for (var i = isync; i < cnt; i++)
                    {
                        j++;
                        s += (float)((IBar)bars[i]).Close;
                    }
                    break;
            }
            var av = j > 0 ? s/j : prev.Ma;
            last.Ma = SmoothLength == 0
                              ? av
                              : XAverage.Calculate(SmoothLength, av, prev.Ma);
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
