using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GS.Trade.Data.Studies.Averages;

namespace GS.Trade.Data.Studies.Stochastic
{
    public class StochasticClassic : Stochastic
    {
        private float _prevFastK;
        private float _prevFastD;

        protected StochasticClassic(string name, Ticker ticker, int timeIntSeconds,
                                    BarValue highValue, BarValue lowValue, BarValue closeValue,
                                    int length, int adjustK, int adjustD, 
                                    float overSold, float overBought)
            : base(name, ticker, timeIntSeconds, highValue, lowValue, closeValue, length, overSold, overBought)
        {
            AdjustK = adjustK;
            AdjustD = adjustD;

            StType = StochasticType.Classic;
        }

        public override void Calculate(ITimeSeriesItem tsi, int isync, int ilast, int iprev)
        {
            var last = (Item)(Items[ilast]);
            var prev = (Item)(Items[iprev]);

            var fastK = FastK(isync, Length);
            last.K = XAverage.Calculate(AdjustK + 3, fastK, _prevFastK);    // SlowKClassic = Average(FastK(FastKLen), Length);
            var fastDClassic = XAverage.Calculate(3 + 2, fastK, _prevFastK);   // FastDClassic = Average(FastK(KLength), 3);
            last.D = XAverage.Calculate(AdjustD + 3, fastDClassic, _prevFastD);       // SlowDClassic = Average(FastDClassic(FastKLen), Length);

            _prevFastK = fastK;
            _prevFastD = fastDClassic;
        }

        public override void InitItem(ITimeSeriesItem tsi, int isync, int ilast)
        {
            throw new NotImplementedException();
        }
    }
}
