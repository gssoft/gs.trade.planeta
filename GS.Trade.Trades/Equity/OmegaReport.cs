using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GS.Extension;

namespace GS.Trade.Trades.Equity
{
    public abstract class TradeReport
    {
    }

    public class OmegaReport : TradeReport
    {
        public OmegaReport()
        {
            StartDate = new DateTime().Date;
            EndDate = new DateTime().Date;
        }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public double TotalNetProfit { get; set; }
        public double GrossProfit { get; set; }
        public double GrossLoss { get; set; }

        public long TotalTrades { get; set; }
        public long NumberWinTrades { get; set; }
        public long NumberLosingTrades { get; set; }
        public double PercentProfitable { get; set; }

        public double LargestWinTrade { get; set; }
        public double LargestLosingTrade { get; set; }
        public double AverageWinTrade { get; set; }
        public double AverageLosingTrade { get; set; }
        public double AverageTrade { get; set; }
        public double RatioAvgWinToAvgLoss { get; set; }

        public double StdDevWinTrade { get; set; }
        public double StdDevLosingTrade { get; set; }
        public double StdDevTrade { get; set; }

        public double ProfitFactor { get; set; }
        public double MaxDrawDown { get; set; }

        public double NetPrtfToLrgLossRatio { get; set; }
        public double NetPrtfToMaxDDRatio { get; set; }

        public double Expectancy {
            get
            {
                return TotalTrades > 0 && (NumberWinTrades > 0 || NumberLosingTrades >0 ) 
                        ? (
                            NumberWinTrades / (double)TotalTrades * AverageWinTrade +
                            NumberLosingTrades / (double)TotalTrades * AverageLosingTrade).Round(2)
                        : 0;
            }
        }
        public double ExpectancyPerAvgLoss
        {
            get { return AverageLosingTrade.CompareTo(0d) < 0 
                            ? 100 * (-Expectancy / AverageLosingTrade).Round(2) 
                            : 0; }
        }

        public double CoefOfVar {
            get
            {
                return AverageTrade.CompareTo(0d) != 0
                    ? 100 * (StdDevTrade / AverageTrade).Round(2)
                    : 0;
            }
        }
        public double WinCoefOfVar
        {
            get
            {
                return AverageWinTrade.CompareTo(0d) > 0
                    ? 100 * ( StdDevWinTrade / AverageWinTrade).Round(2)
                    : 0;
            }
        }
        public double LossCoefOfVar
        {
            get
            {
                return AverageLosingTrade.CompareTo(0d) < 0
                    ? 100 * (-StdDevLosingTrade/AverageLosingTrade).Round(2)
                    : 0;
            }
        }

       

        public int Days {
            get { return (EndDate.Date - StartDate.Date).Days + 1; }
        }

        public double AvgDailyProfit
        {
            get { return Days != 0 ? (TotalNetProfit/Days).Round(2) : 0; }
        }

        public string TotalNetProfitStr
        {
            get { return TotalNetProfit.ToString("N2"); }
        }
        public string GrossProfitStr
        {
            get { return GrossProfit.ToString("N2"); }
        }
        public string GrossLossStr
        {
            get { return GrossLoss.ToString("N2"); }
        }

        public string TotalTradesStr
        {
            get { return TotalTrades.ToString("N0"); }
        }
        public string NumberWinTradesStr
        {
            get { return NumberWinTrades.ToString("N0"); }
        }
        public string NumberLosingTradesStr
        {
            get { return NumberLosingTrades.ToString("N0"); }
        }
        public string PercentProfitableStr
        {
            get { return PercentProfitable.ToString("N2"); }
        }

        public string LargestWinTradeStr
        {
            get { return LargestWinTrade.ToString("N2"); }
        }
        public string LargestLosingTradeStr
        {
            get { return LargestLosingTrade.ToString("N2"); }
        }
        public string AverageWinTradeStr
        {
            get { return AverageWinTrade.ToString("N2"); }
        }
        public string AverageLosingTradeStr
        {
            get { return AverageLosingTrade.ToString("N2"); }
        }
        public string AverageTradeStr
        {
            get { return AverageTrade.ToString("N2"); }
        }
        public string RatioAvgWinToAvgLossStr
        {
            get { return RatioAvgWinToAvgLoss.ToString("N2"); }
        }

        public string StdDevWinTradeStr
        {
            get { return StdDevWinTrade.ToString("N2"); }
        }
        public string StdDevLosingTradeStr
        {
            get { return StdDevLosingTrade.ToString("N2"); }
        }
        public string StdDevTradeStr
        {
            get { return StdDevTrade.ToString("N2"); }
        }

        public string ProfitFactorStr
        {
            get { return ProfitFactor.ToString("N2"); }
        }
        public string MaxDrawDownStr
        {
            get { return MaxDrawDown.ToString("N2"); }
        }

        public string NetPrtfToLrgLossRatioStr
        {
            get { return NetPrtfToLrgLossRatio.ToString("N2"); }
        }
        public string NetPrtfToMaxDDRatioStr
        {
            get { return NetPrtfToMaxDDRatio.ToString("N2"); }
        }


        public void Update(IList<TradeItem> trs)
        {
            //var lst = trs.ToList();
            var lst = trs;
            if (lst.Any())
            {
                StartDate = lst.Min(t => t.Dt2);
                EndDate = lst.Max(t => t.Dt2);
            }
            else
                return;

            
            foreach (var t in lst)
            {
                TotalNetProfit += t.Profit;
                TotalTrades++;
                if (t.IsWin)
                {
                    GrossProfit += t.Profit;
                    NumberWinTrades++;
                }
                else
                {
                    GrossLoss += t.Profit;
                    NumberLosingTrades++;
                }
            }
            TotalNetProfit = TotalNetProfit.Round(2);
            GrossProfit = GrossProfit.Round(2);
            GrossLoss = GrossLoss.Round(2);

            if (GrossLoss.CompareTo(0d) < 0)
                ProfitFactor = (-GrossProfit / GrossLoss).Round(2);
            if (TotalTrades > 0)
                PercentProfitable = (NumberWinTrades / (double)TotalTrades * 100d).Round(2);

            // Win Trades
            var winTrades = lst.Where(t => t.IsWin);
            if (winTrades.Any())
            {
                LargestWinTrade = winTrades.Max(t => t.Profit).Round(2);
                AverageWinTrade = winTrades.Average(t => t.Profit).Round(2);
                StdDevWinTrade = winTrades.Select(t => t.Profit).StdDev().Round(2);
            }
            else
            {
                LargestWinTrade = 0;
                AverageWinTrade = 0;
            }
            // Loss Trades
            var losstrades = lst.Where(t => !t.IsWin);
            if (losstrades.Any())
            {
                LargestLosingTrade = losstrades.Min(t => t.Profit).Round(2);
                AverageLosingTrade = losstrades.Average(t => t.Profit).Round(2);
                StdDevLosingTrade = losstrades.Select(t => t.Profit).StdDev().Round(2);
            }
            else
            {
                LargestLosingTrade = 0;
                AverageLosingTrade = 0;
            }
            //AverageWinTrade = lst.Where(t => t.IsWin).Average(t => t.Profit).Round(2);
            //AverageLosingTrade = lst.Where(t => !t.IsWin).Average(t => t.Profit).Round(2);
            AverageTrade = lst.Average(t => t.Profit).Round(2);

            if (AverageLosingTrade.CompareTo(0d) < 0)
                RatioAvgWinToAvgLoss = (-AverageWinTrade / AverageLosingTrade).Round(2);

            StdDevTrade = lst.Select(t => t.Profit).StdDev().Round(2);
            //StdDevLosingTrade = lst.Where(t => !t.IsWin).Select(t => t.Profit).StdDev().Round(2);
            //StdDevWinTrade = lst.Where(t => t.IsWin).Select(t => t.Profit).StdDev().Round(2);

            var eq = new Equity(10);
            foreach (var t in lst)
                eq.Update(t.Dt1, t.Profit);
            MaxDrawDown = eq.GetMaxDrawDown().Round(2);

            if (MaxDrawDown.CompareTo(0d) > 0)
                NetPrtfToMaxDDRatio = Math.Abs(TotalNetProfit / (double)MaxDrawDown).Round(2);
            if (LargestLosingTrade.CompareTo(0d) < 0)
                NetPrtfToLrgLossRatio = Math.Abs(TotalNetProfit / (double)LargestLosingTrade).Round(2);
        }

        public class TradeItem
        {
            public DateTime Dt1 { get; set; }
            public DateTime Dt2 { get; set; }

            public double Profit { get; set; }
            public double Cost { get; set; }

            public bool IsWin
            {
                get { return Profit.CompareTo(0d) > 0; }
            }
        }
    }
}
