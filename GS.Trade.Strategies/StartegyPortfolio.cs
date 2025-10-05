using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GS.Interfaces;

namespace GS.Trade.Strategies
{
    public partial class Strategy
    {
        [XmlIgnore]
        public int BuyContractsRequest { get; private set; }
        [XmlIgnore]
        public int SellContractsRequest { get; private set; }
        public long SellRequest(long contracts)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            // until 2019.09.30
            //if (IsPortfolioRiskEnable
            //    // && IsPosOpened 
            //    && PortfolioRisk.IsSellRequestEnabled(this))
            //{
            //    PortfolioRisk.SkipTheTickToOthers(this, 1);
            //    return contracts;
            //}
            SellContractsRequest = 0;
            if (IsHedger) return 1;

            if (!IsPortfolioRiskEnable) return 0;
            if (PortfolioRisk.IsSellRequestEnabled())
            {
                //var rqstcnt = PortfolioRisk.SellRequestCount;
                //Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    StrategyTimeIntTickerString, $"{methodname} SellRqsts:{rqstcnt} ",
                //    PortfolioRisk?.SellRequestShortInfo, PortfolioRisk?.ShortDescription);

                SellContractsRequest = (int)Contract;
                
                //rqstcnt = PortfolioRisk.SellRequestCount;
                //Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    StrategyTimeIntTickerString, $"{methodname} SellRqsts:{rqstcnt} ",
                //    PortfolioRisk?.SellRequestShortInfo, PortfolioRisk?.ShortDescription);

                Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                StrategyTimeIntTickerString, $"{methodname} Sells Enabled",
                PortfolioRisk?.SellRequestShortInfo, PortfolioRisk?.ShortDescription);

                return 1;
            }
            SellContractsRequest = 0;
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                StrategyTimeIntTickerString, $"{methodname} Sells Disabled",
                PortfolioRisk?.SellRequestShortInfo, PortfolioRisk?.ShortDescription);
            return 0;
        }
        public long BuyRequest(long contracts)
        {
            var methodname = MethodBase.GetCurrentMethod().Name + "()";
            //if (IsPortfolioRiskEnable
            //    // && IsPosOpened 
            //    && PortfolioRisk.IsBuyRequestEnabled(this))
            //{
            //    PortfolioRisk.SkipTheTickToOthers(this, 1);
            //    return contracts;
            //}
            //return 0;
            BuyContractsRequest = 0;
            if (IsHedger) return 1;

            if (!IsPortfolioRiskEnable) return 0;
            if (PortfolioRisk.IsBuyRequestEnabled())
            {
                //var rqstcnt = PortfolioRisk.BuyRequestCount;
                //Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    StrategyTimeIntTickerString, $"{methodname} BuyRqsts:{rqstcnt} ",
                //    PortfolioRisk?.BuyRequestShortInfo, PortfolioRisk?.ShortDescription);

                BuyContractsRequest = (int)Contract;

                //rqstcnt = PortfolioRisk.BuyRequestCount;
                //Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                //    StrategyTimeIntTickerString, $"{methodname} BuyRqsts:{rqstcnt}",
                //    PortfolioRisk?.BuyRequestShortInfo, PortfolioRisk?.ShortDescription);

                Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString,
                StrategyTimeIntTickerString, $"{methodname} Buys Enabled",
                PortfolioRisk?.BuyRequestShortInfo, PortfolioRisk?.ShortDescription);

                return 1;
            }
            BuyContractsRequest = 0;
            Evlm2(EvlResult.INFO, EvlSubject.TRADING, StrategyTimeIntTickerString, 
                StrategyTimeIntTickerString, $"{methodname} Buys Disabled",
                PortfolioRisk?.BuyRequestShortInfo, PortfolioRisk?.ShortDescription);
            return 0;
        }
    }
}
