using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Trade.Data.Bars;

namespace GS.Trade.Data.Studies.Stochastic
{
    public class StochasticFast : Stochastic
    {
        public StochasticFast(string name, ITicker ticker, int timeIntSeconds,
                                    BarValue highValue, BarValue lowValue, BarValue closeValue,
                                    int length, float overSold, float overBought)
            : base(name, ticker, timeIntSeconds, highValue, lowValue, closeValue, length, overSold, overBought)
        {
            StType = StochasticType.Fast;
        }
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var b = (Bar)SyncSeries[isync];
            if (iprev > 0) // Copy Last
            {
                var last = ((Item)Items[ilast]);
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            K = last.K,
                            D = last.D
                        }
                    );
            }
            else // First Tick Item in Series
            {
                AddItem(
                    new Item
                        {
                            DT = itemDt,
                            LastDT = tsi.LastDT,
                            SyncDT = syncDt,

                            K = FastK(isync, Length),
                            D = FastK(isync, Length)
                    }
                    );
            }
        }
        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            var last = (Item)(Items[ilast]);

            last.K = FastK(isync, Length);
            last.D = last.K;
        }
        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            last.K = FastK(isync, Length);
            last.D = prev.D + Factor * (last.K - prev.D);
        }
    }
}
