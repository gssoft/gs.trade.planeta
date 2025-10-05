using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GS.Trade.Data.Studies.Stochastic
{
    public class StochasticSlow : Stochastic
    {
        public StochasticSlow(string name, ITicker ticker, int timeIntSeconds,
                                    BarValue highValue, BarValue lowValue, BarValue closeValue,
                                    int length, float overSold, float overBought)
            : base(name, ticker, timeIntSeconds, highValue, lowValue, closeValue, length, overSold, overBought)
        {
            StType = StochasticType.Slow;
        }
        public override void AddNewItem(DateTime syncDt, DateTime itemDt, ITimeSeriesItem tsi, int isync, int ilast, int isCopyOrInit)
        {
            if (isCopyOrInit > 0) // Copy !!! LAST !!!!
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

            var fastK = FastK(isync, Length);

            last.K = prev.K + Factor * (fastK - prev.K);
            last.D = (last.K + prev.D * 2) / 3;

         //   last.FastK = FastK(isync);
         //   last.FastD = prev.FastD + (Factor * (last.FastK - prev.FastD));
         //   last.SlowK = last.FastD;
         //   last.SlowD = ((prev.SlowD * 2) + last.FastD) / 3;
            
        }
    }
}
