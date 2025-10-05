using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GS.Interfaces;
using GS.Trade.Interfaces;
using GS.Trade.Trades;
using GS.Trade.Trades.UI.Model;
using GS.Trade.Windows.Models;

namespace GS.Trade.Windows
{
    /// <summary>
    /// Логика взаимодействия для PositionTotals_Window2.xaml
    /// </summary>
    public partial class PositionTotalsWindow4 : Window
    {
        private ITradeContext _tx;
        private IEventLog _evl;
        //private Positions.Totals _positions;

        public ObservableCollection<PositionTotalNpc2> PositionCollection;
        //public ObservableCollection<PositionTotalNpc2> TotalSysCollection;
        public ObservableCollection<TotalStat01> TotalStrategiesList;
        public ObservableCollection<TotalStat01> TotalTickersList;
        public ObservableCollection<TotalStat01> TotalTimeIntsList;
        //  public List<Positions.Totals.PositionTotal> TempList;

        private readonly object _locker;
        //private SimpleProcess _observeProcess;

        public PositionTotalsWindow4()
        {
            InitializeComponent();

            _locker = new object();
            PositionCollection = new ObservableCollection<PositionTotalNpc2>();
            //TotalSysCollection = new ObservableCollection<PositionTotalNpc2>();
            TotalStrategiesList = new ObservableCollection<TotalStat01>();
            TotalTickersList = new ObservableCollection<TotalStat01>();
            TotalTimeIntsList = new ObservableCollection<TotalStat01>();
         }
        private void CheckNullReference()
        {
            if (_evl == null || _tx == null)
                throw new NullReferenceException($"{GetType().Name}: EventLog = null or TradeContext == null)");
        }
        public void Init(ITradeContext tx, IEventLog eventlog)
        {
            _tx = tx;
            _evl = eventlog;

            CheckNullReference();

            _evl.AddItem(EvlResult.SUCCESS, EvlSubject.INIT,
                    GetType().Name, Title, "Initialization Start", "", "");

            IsProcessTaskInUse = true;
            SetupProcessTask();

            ProcessTask?.Start();

            _evl?.AddItem(EvlResult.SUCCESS, EvlSubject.INIT,
                                GetType().Name, Title, "Initialization Finish", "", "");
        }
        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            CheckNullReference();
           // _positions.PositionTotalsEvent += PositionsEventHandler;

            //lstPositionTotals.ItemsSource = _positions.PositionTotals.PositionTotalsObserveCollection;
            lstPositionTotals.ItemsSource = PositionCollection;
            LstPositionTotalSystem.ItemsSource = TotalStrategiesList;
            LstTotalTickers.ItemsSource = TotalTickersList;
            LstTotalTimeInts.ItemsSource = TotalTimeIntsList;

            _tx.EventHub.Subscribe("UI.Positions", "Total", PositionsEventHandler);
            _tx.EventHub.Subscribe("UI.Positions", "Total.MaxMinProfit", PositionsEventHandler);
            // if (_observeProcess != null) _observeProcess.Start();
            GetAllTotals();

            // ProcessTask?.Start();
        }
        private void WindowClosed(object sender, EventArgs e)
        {
            //if (_observeProcess != null) _observeProcess.Stop();
            _tx.EventHub.UnSubscribe("UI.Positions", "Total", PositionsEventHandler);
            PositionCollection?.Clear();

            _evl?.Evlm2(EvlResult.INFO, EvlSubject.INIT, GetType().Name, Title, "WindowClosed", "", "");
            ProcessTask?.Stop();                    
        }
        private void PositionsEventHandler(object o, Events.IEventArgs args)
        {
            var ip = args?.Object as IPosition2;
            if (ip == null)
                return;

            ProcessTask?.EnQueue(args);
        }       
        private void GetAllTotals()
        {
            PositionCollection?.Clear();
            var ps = _tx.GetPositionTotals();
            foreach (var ip in ps)
                // PositionCurrentCollection.Add((PositionNpc)p);
                PositionCollection?.Add(new PositionTotalNpc2(ip));

            // CreateStat();
            TitleUpdate();
        }
        private void TitleUpdate()
        {
            var totalProfit = PositionCollection?.Sum(s => s.PnL2).ToString("N");
            Title = $"Totals P&L: {totalProfit}  Count: {PositionCollection?.Count}";
        }
        private void CreateTotalStrategiesReport(object sender, RoutedEventArgs routedEventArgs)
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";
            TotalStrategiesList.Clear();
            if (PositionCollection == null || !PositionCollection.Any()) return;
            try
            {
                var v = (from t in PositionCollection
                         group t by new
                         {
                             t.AccountCode, t.StrategyCode
                         }
                         into g
                         //let standardDeviation = DbFunctions.StandardDeviation(g.Select(t => t.PnL2))
                         //where standardDeviation != null
                         select new TotalStat01()
                         {
                             Account = g.Key.AccountCode,
                             Strategy = g.Key.StrategyCode,
                             Ticker = "All",
                             TimeInt = 0,
                             Quantity = g.Sum(t => t.Quantity),
                             Count = g.Count(),
                             ProfitLoss = g.Sum(t => t.PnL2),
                             ProfitAvg = g.Average(t => t.PnL2),
                             ProfitMax = g.Max(t => t.PnL2),
                             ProfitMin = g.Min(t => t.PnL2),
                             // ProfitStd = standardDeviation,
                             // ProfitStd = DbFunctions.StandardDeviation(g.Select(t => t.PnL2)),
                             ProfitStd = GSMath.Math.StandardDeviation(g.Select(t => t.PnL2)),
                             DailyMaxProfit = g.Max(t => t.Strategy.DailyMaxProfit),
                             DailyMaxLoss = g.Min(t => t.Strategy.DailyMaxLoss),
                             FirstTradeDT = g.Min(t => t.FirstTradeDT),
                             LastTradeDT = g.Max(t => t.LastTradeDT)
                         }).OrderBy(t => t.Account + t.Strategy); // .ToList();

                foreach (var i in v) TotalStrategiesList.Add(i);
                _isNeedRefreshStat = false;
            }
            catch (Exception e)
            {
                var m = e.Message;
                _evl?.Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, GetType().FullName, e.GetType().Name,
                    method, e.Message, ToString());
            }        
        }
        private void CreateTotalTickersReport(object sender, RoutedEventArgs routedEventArgs)
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";

            TotalTickersList.Clear();
            if (PositionCollection == null || !PositionCollection.Any()) return;
            try
            {
                var v = (from t in PositionCollection
                         group t by new
                         {
                             t.AccountCode, TickerCode = t.Ticker.Code
                         }
                    into g
                    select new TotalStat01()
                    {
                        Account = g.Key.AccountCode, 
                        Ticker = g.Key.TickerCode,
                        Strategy = "All",
                        TimeInt = 0,
                        Quantity = g.Sum(t=>t.Quantity),
                        Count = g.Count(),
                        ProfitLoss = g.Sum(t => t.PnL2),
                        ProfitAvg = g.Average(t => t.PnL2),
                        ProfitMax = g.Max(t=>t.PnL2),
                        ProfitMin = g.Min(t => t.PnL2),
                        ProfitStd = GSMath.Math.StandardDeviation(g.Select(t => t.PnL2)),
                        DailyMaxProfit = g.Max(t => t.Strategy.DailyMaxProfit),
                        DailyMaxLoss = g.Min(t => t.Strategy.DailyMaxLoss),
                        FirstTradeDT = g.Min(t => t.FirstTradeDT),
                        LastTradeDT = g.Max(t => t.LastTradeDT)
                    }).OrderBy(t => t.Account + t.Ticker); // .ToList();

                foreach (var i in v) TotalTickersList.Add(i);
                _isNeedRefreshStat = false;
            }
            catch (Exception e)
            {
                _evl?.Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, GetType().FullName, e.GetType().Name,
                    method, e.Message, ToString());
            }
        }
        private void CreateTotalTimeIntsReport(object sender, RoutedEventArgs routedEventArgs)
        {
            var method = MethodBase.GetCurrentMethod()?.Name + "()";

            TotalTimeIntsList.Clear();
            if (PositionCollection == null || !PositionCollection.Any()) return;
            try
            {
                var v = (from t in PositionCollection // group t by t.Strategy.TimeInt
                     group t by new
                     {
                         t.AccountCode, t.Strategy.TimeInt
                     }
                     into g
                     select new TotalStat01()
                     {
                         Account = g.Key.AccountCode,
                         Strategy = "All",
                         Ticker = "All",
                         TimeInt = g.Key.TimeInt,
                         Quantity = g.Sum(t => t.Quantity),
                         Count = g.Count(),
                         ProfitLoss = g.Sum(t => t.PnL2),
                         ProfitAvg = g.Average(t => t.PnL2),
                         ProfitMax = g.Max(t => t.PnL2),
                         ProfitMin = g.Min(t => t.PnL2),
                         ProfitStd = GSMath.Math.StandardDeviation(g.Select(t => t.PnL2)),
                         DailyMaxProfit = g.Max(t => t.Strategy.DailyMaxProfit),
                         DailyMaxLoss = g.Min(t => t.Strategy.DailyMaxLoss),
                         FirstTradeDT = g.Min(t=>t.FirstTradeDT),
                         LastTradeDT = g.Max(t => t.LastTradeDT)
                     }).OrderBy(t => t.Account + t.TimeInt); // .ToList();

            foreach (var i in v) TotalTimeIntsList.Add(i);
            _isNeedRefreshStat = false;
            }
            catch (Exception e)
            {
                _evl?.Evlm2(EvlResult.FATAL, EvlSubject.PROGRAMMING, GetType().FullName, e.GetType().Name,
                    method, e.Message, ToString());
            }
        }
        private bool _isNeedRefreshStat;
        public void CreateStat()
        {
            if (!_isNeedRefreshStat) return;
            Dispatcher.BeginInvoke((ThreadStart) (() =>
            {
                TitleUpdate();
                CreateTotalStrategiesReport(null, null);
                CreateTotalTickersReport(null, null);
                CreateTotalTimeIntsReport(null, null);
            }));
        }
        private void OnTabControlSelectionChanged1(object sender, SelectionChangedEventArgs e)
        {
            if (!Equals(sender, e.OriginalSource)) return;
            var tc = sender as TabControl;
            if (tc == null) return;
            var tabItemName = ((TabItem)tc.SelectedItem).Name;
            if (tabItemName == "Total") return;
            Dispatcher.BeginInvoke((ThreadStart) (() =>
            {
                var item = ((TabItem)((TabControl) sender).SelectedItem).Name;
                switch (item)
                {
                    case "Strategies":
                        CreateTotalStrategiesReport(null, null);
                        break;
                    case "Tickers":
                        CreateTotalTickersReport(null, null);
                        break;
                    case "TimeInts":
                        //CreateTotalTimeIntReport(null, null);
                        break;
                }
            }));
        }
        private void OnTabControlSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(!Equals(sender, e.OriginalSource)) return;
            var tc = sender as TabControl;
            if (tc == null) return;
            var tabItemName = ((TabItem)tc.SelectedItem).Name;
            if (tabItemName == "Total") return;
            Dispatcher.BeginInvoke((Action<string>)(name =>
            {
                var item = name;
                switch (item)
                {
                    case "Strategies":
                        CreateTotalStrategiesReport(null, null);
                        break;
                    case "Tickers":
                        CreateTotalTickersReport(null, null);
                        break;
                    case "TimeInt":
                        CreateTotalTimeIntsReport(null, null);
                        break;
                }
                TitleUpdate();
            }
            ), tabItemName);
        }
    }
}
