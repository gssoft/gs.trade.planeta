using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using GS.Trade.Windows.Annotations;

namespace GS.Trade.Windows.Models
{
    public class TotalStat01 :  INotifyPropertyChanged
    {
        private decimal _dailyMaxProfit;
        private decimal _dailyMinProfit;

        public string Account { get; set; }
        public string Ticker { get; set; }
        public string Strategy { get; set; }
        public string TimeIntStr { get; set; }
        public int TimeInt { get; set; }
        public decimal ProfitLoss { get; set; }
        public decimal ProfitAvg { get; set; }
        public double ProfitStd { get; set; }
        public decimal ProfitMax { get; set; }
        public decimal ProfitMin { get; set; }
        public decimal DailyMaxProfit
        {
            get { return _dailyMaxProfit; }
            set
            {
                _dailyMaxProfit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DailyMaxProfitStr"));
            }
        }
        public decimal DailyMaxLoss
        {
            get { return _dailyMinProfit; }
            set
            {
                _dailyMinProfit = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DailyMaxLossStr"));
            }
        }
        public long Quantity { get; set; }
        public int Count { get; set; }
        public DateTime FirstTradeDT { get; set; }
        public DateTime LastTradeDT { get; set; }
        public decimal TradeAvg => Quantity != 0 ? ProfitLoss/Quantity : 0;
        public string TradeAvgStr => TradeAvg.ToString("N");
        public string ProfitLossStr => ProfitLoss.ToString("N");
        public string ProfitAvgStr => ProfitAvg.ToString("N");
        public string ProfitStdStr => ProfitStd.ToString("N");
        public string ProfitMaxStr => ProfitMax.ToString("N");
        public string ProfitMinStr => ProfitMin.ToString("N");
        public string DailyMaxProfitStr => DailyMaxProfit.ToString("N");
        public string DailyMaxLossStr => DailyMaxLoss.ToString("N");
        public string FirstTradeDTStr => FirstTradeDT.ToString("G");
        public string LastTradeDTStr => LastTradeDT.ToString("G");

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        //[NotifyPropertyChangedInvocator]
        //protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
