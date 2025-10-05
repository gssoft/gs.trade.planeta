using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GS.Extension;
using GS.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Bars;
using GS.Trade.Strategies;
using GS.Trade;
using GS.Trade.Interfaces;
using GS.Trade.TradeContext;


namespace GS.Trade.BackTest
{
    using Strategies;
    using TradeContext;

   public class BackTest7
    {
        public int MinBarsCountToStart = 15;
        private const int Kminmove = 1;

        public delegate void NextPassDelegate();
        public delegate void DoWorkDelegate();

        public event EventHandler<string> MessageEvent;
        public event EventHandler<DateTime> FinishEvent;

        protected virtual void OnFinishEvent()
        {
            EventHandler<DateTime> handler = FinishEvent;
            handler?.Invoke(this, DateTime.Now);
        }

        protected virtual void OnMessageEvent(string e)
        {
            EventHandler<string> handler = MessageEvent;
            handler?.Invoke(this, e);
        }

        public NextPassDelegate NextPass;

        private DoWorkDelegate _doWorkDelegate;
        private IAsyncResult _asyncWork;

        private ITradeContext _tradeContext;
        private IEnumerable<IStrategy> _strategies;

        public bool AllHaveSameTradesNumber { get; set; }

        protected ISimulateTerminal SimulateTerminal; 

        private BarRandom _random;
        private Bar _lastBar;

     //   public DoWorkStatus{} get; set;
        private bool _doWorkStatus;

        public long TradesNumber { get; set; }
        public int StartValue { get; set; }
        public int LoopDelay { get; set; }
        public int SpreadInMinMoves { get; set; }
        public TestModeEnum TestMode { get; set; }
        public IList<KeyValuePair<TestModeEnum, string>> TestModeKeyValuePairs => TestMode.ToKeyValuePairList();
        public ExecutionBarModeEnum ExecutionBarMode { get; set; }
        public IList<KeyValuePair<ExecutionBarModeEnum, string>>  ExecutionBarModeKeyValuePairs => ExecutionBarMode.ToKeyValuePairList();

        private BarTickModeEnum _barTickMode;
        public BarTickModeEnum BarTickMode
        {
            get { return _barTickMode; }
            set
            {
                SetBarTickMode(value);
                _barTickMode = value;
            }
        }
        public void SetOrderExecMode( BackTestOrderExecutionMode m)
        {
            //if (_tradeContext.Orders != null)
            //    _tradeContext.Orders.BackOrderExecMode = m;
            if (SimulateTerminal != null)
                SimulateTerminal.BackOrderExecMode = m;
        }

        private readonly IList<TickerSlot> _tickerSlots = new List<TickerSlot>();

        public bool IsEndOfBarSeries {
            get { return _tickerSlots.All(ts1 => ts1.EndOfBarSeries); }
        }
        private TickerSlot _tickerSlot;
        private bool _pause;
        private DateTime _crntDt;

        public bool RefreshUI { get; set; }

        public BackTest7()
        {
        }
        public void Init(ITradeContext tx)
        {
            // TradesNumber = 5000;
            //BarTickMode = BarTickModeEnum.Bar;
            _tradeContext = tx;
            SimulateTerminal = tx.TradeTerminals.GetSimulateTerminal();
            LoopDelay = 1;
            SpreadInMinMoves = Kminmove;
            SetOrderExecMode(BackTestOrderExecutionMode.Optimistic);
           // ExecutionBarMode = ExecutionBarModeEnum.OLHC;
           // _strategies = new Strategies.Strategies("MyStrategies", "xStyle", _tradeContext);
           // _strategies.Init();
            _strategies = _tradeContext.StrategyCollection;

            _random = new BarRandom(StartValue, 1000)
                          {
                              Name = "RIU1",
                              Code = "RIU1",
                              TradeBoard = "SPBFUT",
                              DT = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day,
                                                DateTime.Now.Hour, DateTime.Now.Minute, 0, 0)
                          };

            _doWorkDelegate = new DoWorkDelegate(DoWork);
        }       
        private void DoWork()
        {
            BarRandom.Tick t;
            Bar b;
            b = _random.GetNextBar2(15);
            var barDt = b.DT;  //.AddSeconds(-1);
            float bid;
            float ask;

            if (TestMode == TestModeEnum.Real)
            {
                InitReal();
            }

            while (_doWorkStatus)
            {
                OnMessageEvent("Start Working Process");
                if (TestMode == TestModeEnum.Random && BarTickMode == BarTickModeEnum.Bar)
                {
                    string qStr;
                    if (b.IsWhite)
                    {
                        _random.GetBidAsk2((float) b.Low, out bid, out ask);
                        //_tradeContext.Orders.ExecuteTick(barDt, _random.Key, b.Low, bid, ask);
                        _tradeContext.ExecuteTick(barDt, _random.Key, b.Low, bid, ask);
                        //qStr = String.Format("{0};{1};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                        //                         _random.Key, barDt.Date, barDt, b.Low, bid, ask, b.Volume / 4);
                        //_tradeContext.Tickers.PutDdeQuote3(qStr);

                        _random.GetBidAsk2((float)b.Close, out bid, out ask);
                        _tradeContext.ExecuteTick(barDt, _random.Key, b.Close, bid, ask);
                        
                        _random.GetBidAsk2((float) b.High, out bid, out ask);
                        _tradeContext.ExecuteTick(barDt, _random.Key, b.High, bid, ask);
                        // qStr = String.Format("{0};{1};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                        //                     _random.Key, barDt.Date, barDt, b.High, bid, ask, b.Volume / 4);
                        // _tradeContext.Tickers.PutDdeQuote3(qStr);
                          
                    }
                    else
                    {
                        _random.GetBidAsk2((float)b.High, out bid, out ask);
                        _tradeContext.ExecuteTick(barDt, _random.Key, b.High, bid, ask);
                        // qStr = String.Format("{0};{1};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                        //                     _random.Key, barDt.Date, barDt, b.High, bid, ask, b.Volume / 4);
                        // _tradeContext.Tickers.PutDdeQuote3(qStr);
                        
                        _random.GetBidAsk2((float)b.Close, out bid, out ask);
                        _tradeContext.ExecuteTick(barDt, _random.Key, b.Close, bid, ask);

                        _random.GetBidAsk2((float)b.Low, out bid, out ask);
                        _tradeContext.ExecuteTick(barDt, _random.Key, b.Low, bid, ask);
                        //qStr = String.Format("{0};{1};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                        //                         _random.Key, barDt.Date, barDt, b.Low, bid, ask, b.Volume / 4);
                        // _tradeContext.Tickers.PutDdeQuote3(qStr);
                    }

                    //_random.GetBidAsk((float)b.Close, out bid, out ask);
                   //// _tradeContext.Orders.NewTick(barDt, _random.Key, b.Close, bid, ask);
                    //qStr = String.Format("{0};{1};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                    //                       _random.Key, barDt.Date, barDt, b.Close, bid, ask, b.Volume / 4);
                    // _tradeContext.Tickers.PutDdeQuote3(qStr);

                    _tradeContext.Tickers.UpdateAsyncSeries(_random.Key, b); // Add Prev Bar

                    b = _random.GetNextBar2(15);
                    barDt = b.DT;
                    t = _random.FirstTickInBar;

                    _tradeContext.ExecuteTick(t.DT, t.Ticker, t.Value, t.Bid, t.Ask);
                    _tradeContext.Tickers.PutDdeQuote3(t.ToString());
                    _tradeContext.Tickers.UpdateTimeSeries(t.DT);
                }
                else if (TestMode == TestModeEnum.Random && BarTickMode == BarTickModeEnum.Tick)
                {
                    t = _random.GetNextTick2();
                   // t.DT = t.DT.AddSeconds(-1);
                    _tradeContext.ExecuteTick(t.DT, t.Ticker, t.Value, t.Bid, t.Ask);
                    _tradeContext.Tickers.PutDdeQuote3(t.ToString());
                    _tradeContext.Tickers.UpdateTimeSeries(t.DT);
                }
                else if (TestMode == TestModeEnum.Real)
                {
                    try
                    {
                    foreach (var ts in _tickerSlots)
                    {
                        if (ts.EndOfBarSeries)
                        {
                            if (!IsEndOfBarSeries)
                                continue;

                            _doWorkStatus = false;
                            break;
                        }
                        var bar = ts.GetNextBar();
                        if (bar != null)
                        {
                            //_flagNewDay = (ts.LastBar != null && bar.DT.DateToInt() > ts.LastBar.DT.DateToInt())
                            //              || ts.LastBar == null;
                            // New Date bar/ Bar Dates are not Same
                            if(ts.LastBar != null && ts.LastBar.DT.DateToInt() != bar.DT.DateToInt())
                                FinishStrategies(ts.LastBar, ts);

                            ts.LastBar = bar;
                            // Start Execution
                            ExecuteBar(bar, ts);

                            ts.Ticker.UpdateAsyncSeries2(bar);
                            ts.Ticker.UpdateTimeSeries(bar.DT);

                            foreach (var s in ts.Strategies)
                            {
                                if (s.Bars != null && s.Bars.Count < MinBarsCountToStart)
                                    continue;

                                if (TradesNumber > 0)
                                {
                                    if (AllHaveSameTradesNumber)
                                    {
                                        if (s.PositionTotal.Quantity < TradesNumber)
                                        {
                                            s.MainBase();
                                          //  ii++;
                                        }
                                        else if (s.Position.IsOpened)
                                        {
                                            s.Finish();
                                            ExecuteBar(ts.LastBar, ts);
                                        }
                                    }
                                    else
                                    {
                                        if (ts.EndOfSession)
                                        {
                                            if (!s.Position.IsOpened)
                                                continue;

                                            s.Finish();
                                            ExecuteBar(ts.LastBar, ts);
                                            // ii++;
                                        }
                                        // Session is working
                                        else
                                        {
                                            s.EntryEnabled = true;
                                            bool toContinue =
                                                ts.Strategies.Any(sp => sp.PositionTotal.Quantity < TradesNumber);
                                            if (toContinue)
                                            {
                                                s.MainBase();
                                               // ii++;
                                            }
                                            // Finish when Max Trades NUmber is Reached
                                            else if (s.Position.IsOpened)
                                            {
                                                s.Finish();
                                                ExecuteBar(ts.LastBar, ts);
                                            }
                                        }
                                    }
                                }
                                else s.MainBase();
                            }
                        }
                        // Nothing Bars else in this TickerSlot
                        else
                        {
                            // Nothing Bars else in this TickerSlot
                            ts.EndOfBarSeries = true;
                            //if(IsEndOfBarSeries)
                            //    _doWorkStatus = false;
                            //else
                            //    continue;
                            //_doWorkStatus = false;
                            FinishStrategies(ts.LastBar, ts);
                        }
                    }
                    // Next Pass
                    NextPass?.Invoke();
                    }
                    catch (Exception e)
                    {
                        //var exc = e;
                        throw new NullReferenceException(e.Message);
                        // SendException(e);
                    }
                }
            }
            // End of While
            OnMessageEvent("Preapare to Finish. Try to Close All Startegies");

            // Exit from Main Loop and go to Finish
           // if (!idosomething) return;

            if (TestMode == TestModeEnum.Real)
            {
                foreach (var ts in _tickerSlots)
                {
                    if (ts.LastBar == null)
                        continue;
                    //var ssOpened = (from ts.S s in _strategies where s.Position.IsOpened select s).ToList();
                    FinishStrategies(ts.LastBar, ts);
                }
            }
            OnFinishEvent();
        }
        private void InitReal()
        {
            var tickers = _tradeContext.Tickers.GetTickers.ToList();
            foreach (var ti in tickers)
            {
                _tradeContext.Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "BackTest", "Init()", "", "", "");
                _tickerSlot = new TickerSlot {Ticker = ti, DateTimeTo = DateTime.Now, Days = 100};
                _tickerSlot.Init(_tradeContext.EventLog);
                _tickerSlot.Strategies.Clear();
                foreach (var strategy in _strategies.Where(s => s.Ticker.Key == ti.Key).ToList())
                {
                    var s = (Strategy) strategy;
                    s.SetWorkingStatus(true, "Init Real");
                    _tickerSlot.Strategies.Add(s);
                }
                _tickerSlots.Add(_tickerSlot);
            }
        }
        private void FinishStrategies(Bar bar, TickerSlot ts)
        {
            if (bar == null || ts == null)
                return;
            var so = ts.Strategies.Where(s => s.Position.IsOpened).ToList();
            while (so.Any())
            {
                //_tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.Low, ts.LastBar.Low);
                //_tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.High, ts.LastBar.High);
                foreach (var s in so)
                {
                    s.Finish();
                    //_tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.Low,
                    //    ts.LastBar.Low);
                    //_tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.High,
                    //    ts.LastBar.High);
                    while (s.Position.IsOpened)
                    {
                        ExecuteBar(bar, ts);

                        bar.High = bar.High + ts.Ticker.MinMove;
                        bar.Low = bar.Low - ts.Ticker.MinMove;
                    }
                }
                so = ts.Strategies.Where(s => s.Position.IsOpened).ToList();
            }
        }
        public void ExecuteBar(Bar bar, TickerSlot ts)
        {
            var minmove = ts.Ticker.MinMove * Kminmove;

            ts.Ticker.SetLast(bar);
            switch (ExecutionBarMode)
            {
                case ExecutionBarModeEnum.Close:
                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, ts.Ticker.LastPrice, ts.Ticker.Bid, ts.Ticker.Ask);
                    break;
                case ExecutionBarModeEnum.TypicalPrice:
                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.TypicalPrice, bar.TypicalPrice);
                    break;
                case ExecutionBarModeEnum.MedianPrice:
                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.MedianPrice, bar.MedianPrice);
                    break;
                case ExecutionBarModeEnum.OC:
                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open);
                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close, bar.Close);
                    break;
                case ExecutionBarModeEnum.RunFrom:   
                    // 16.04.2018    Running from your orders          
                    if (bar.IsWhiteLight)
                    {
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High, bar.High + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low, bar.Low + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close, bar.Close + minmove);
                    }
                    else if (bar.IsBlackLight)
                    {
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open - minmove, bar.Open);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low - minmove, bar.Low);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High - minmove, bar.High);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close - minmove, bar.Close);
                    }
                    break;
                case ExecutionBarModeEnum.RunTo:
                    // 16.04.2018    Running to your orders          
                    if (bar.IsWhiteLight)
                    {
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open - minmove, bar.Open);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low - minmove, bar.Low);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High - minmove, bar.High);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close - minmove, bar.Close);
                    }
                    else if (bar.IsBlackLight)
                    {
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High, bar.High + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low, bar.Low + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close, bar.Close + minmove);
                    }
                    break;
                case ExecutionBarModeEnum.Compr:                
                    // 16.04.2018
                    if (bar.IsWhiteLight)
                    {
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High - minmove, bar.High);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low, bar.Low + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close - minmove, bar.Close);
                    }
                    else if (bar.IsBlackLight)
                    {
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open - minmove, bar.Open);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low, bar.Low + minmove);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High - minmove, bar.High);
                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close, bar.Close + minmove);
                    }
                    break;
            }
        }
        private void CreateSeries()
        {
            var ts = _tradeContext.Tickers.GetTickers;
            foreach (var ti in ts)
            {
                // var bs = ti.BarSeries;
                var bs = ti.GetSeries(5);
                foreach (var nb in bs.Select(b => new Bar
                {
                    DT = b.DT,
                    LastDT = b.DT,
                    Open = b.Open,
                    High = b.High,
                    Low = b.Low,
                    Close = b.Close,
                    Volume = b.Volume,
                    Ticks = b.Ticks
                }))
                {
                    _tradeContext.Tickers.UpdateAsyncSeries(ti.Key, nb);
                }
            }
        }

        public void ClearAll()
        {
            _tradeContext.Evlm2(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "BackTest", "ClearAll()", "", "","");
            foreach (var ts in _tickerSlots)
            {
                ts.Ticker.ClearBarSeries();
                ts.Ticker.ClearTimeSeries();
                foreach (var s in ts.Strategies)
                    s.Clear();
            }
        }

        public void Stop()
        {
            _doWorkStatus = false;
        }
        public void Start()
        {
            _tradeContext.EventLog.AddItem(EvlResult.INFO, EvlSubject.TECHNOLOGY, "BackTest", "BackTest", "MainLoop Start",
                                           "TradesToTestNumber=" + TradesNumber, "");
            _doWorkStatus = true;
            _asyncWork = _doWorkDelegate.BeginInvoke(null, null);
        }
        private void SetBarTickMode(BarTickModeEnum mode)
        {
            _pause = true;
            switch (mode)
            {
                case BarTickModeEnum.Bar:
                    _random.NewBarTickEvent -= _tradeContext.Orders.NewTick;
                    break;
                case BarTickModeEnum.Tick:
                    _random.NewBarTickEvent -= _tradeContext.Orders.NewTick;
                    _random.NewBarTickEvent += _tradeContext.Orders.NewTick;
                    break;
            }
            _pause = false;
        }

        private void SomeClear(DateTime dt)
        {
            if (_crntDt.Date.CompareTo(dt.Date) >= 0) return;
            _crntDt = dt.Date;

            _tradeContext.Tickers.ClearSomeData(500);

            _tradeContext.EventLog.ClearSomeData(500);

            _tradeContext.Orders.ClearSomeData(0);

            Thread.Sleep(3000);
        }

        private uint _delay;
        private void Delay(uint i)
        {
            //Thread.Sleep(30);

            if (++_delay%i != 0) return;

            _tradeContext.Tickers.ClearSomeData(100);
            _tradeContext.EventLog.ClearSomeData(100);
            _tradeContext.Orders.ClearSomeData(0);
            _tradeContext.Trades.ClearSomeData(100);

            Thread.Sleep(3000);
        }

        private int _curSecond = 0;

        // NewDayFlag
        private bool _flagNewDay;

        private void UpdatePositionFromticker()
        {
            var second = DateTime.Now.Second;
            if (second == _curSecond)
                return;
            _curSecond = second;
            foreach (var s in _strategies)
            {
                s.UpdateFromLastTick();
            }
        }
        public void UpdatePositionFromtickerInChartUpdate()
        {
            foreach (var s in _strategies)
            {
                s.UpdateFromLastTick();
            }
        }
    }
}
