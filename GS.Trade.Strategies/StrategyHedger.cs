using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using GS.Interfaces;

namespace GS.Trade.Strategies
{
    public partial class Strategy
    {
        #region Hedger ----------------------------------------------------------------------
        public bool IsHedger { get; set; }
        public HedgeStatusEnum HedgerStatus { get; set; }
        public void StartLongHedger()
        {
            if (!IsHedger) return;

            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            ShortEnabled = true;
            LongEnabled = false;

            if (HedgerStatus == HedgeStatusEnum.LongHedge) return;
            HedgerStatus = HedgeStatusEnum.LongHedge;
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, StrategyTimeIntTickerString,
                $"{methodname} {HedgerStatus}", $"LongEnabled:{LongEnabled} ShortEnabled:{ShortEnabled}", PortfolioRisk?.ShortDescription ?? "");
        }
        public void StartShortHedger()
        {
            if (!IsHedger) return;

            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            ShortEnabled = false;
            LongEnabled = true;

            if (HedgerStatus == HedgeStatusEnum.ShortHedge) return;
            HedgerStatus = HedgeStatusEnum.ShortHedge;
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, StrategyTimeIntTickerString,
                $"{methodname} {HedgerStatus}", $"LongEnabled:{LongEnabled} ShortEnabled:{ShortEnabled}", PortfolioRisk?.ShortDescription ?? "");
        }
        public void StopHedger()
        {
            if (!IsHedger) return;

            var methodname = MethodBase.GetCurrentMethod().Name + "()";

            ShortEnabled = LongEnabled = false;

            if (HedgerStatus == HedgeStatusEnum.NoHedge) return;
            HedgerStatus = HedgeStatusEnum.NoHedge;
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, StrategyTimeIntTickerString,
                $"{methodname} {HedgerStatus}", $"LongEnabled:{LongEnabled} ShortEnabled:{ShortEnabled}", PortfolioRisk?.ShortDescription ?? "");
        }
        #endregion
    }
}
