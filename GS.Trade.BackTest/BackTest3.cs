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

    //public enum ExecutionBarModeEnum : int
    //{
    //    MedianPrice = 1, TypicalPrice = 2, OLHC = 3, OHLC = 4, OC = 5
    //}


    public class BackTest3
    {
        public delegate void NextPassDelegate();
        public delegate void DoWorkDelegate();

        public event EventHandler<string> MessageEvent;
        public event EventHandler<DateTime> FinishEvent;

        protected virtual void OnFinishEvent()
        {
            EventHandler<DateTime> handler = FinishEvent;
            if (handler != null) handler(this, DateTime.Now);
        }

        protected virtual void OnMessageEvent(string e)
        {
            EventHandler<string> handler = MessageEvent;
            if (handler != null)
                handler(this, e);
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

        public TestModeEnum TestMode { get; set; }
        public IList<KeyValuePair<TestModeEnum, string>> TestModeKeyValuePairs {
            get { return TestMode.ToKeyValuePairList(); }
        }
        public ExecutionBarModeEnum ExecutionBarMode { get; set; }
        public IList<KeyValuePair<ExecutionBarModeEnum, string>>  ExecutionBarModeKeyValuePairs {
            get { return ExecutionBarMode.ToKeyValuePairList(); }
        }

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

        private readonly IList<TickerSlot> _ticerSlots = new List<TickerSlot>(); 
        private TickerSlot _tickerSlot;
        private bool _pause;
        private DateTime _crntDt;

        public bool RefreshUI { get; set; }

        //   public ChartForm Chart;

        public BackTest3()
        {
        }
        public void Init(ITradeContext tx)
        {
            // TradesNumber = 5000;
            //BarTickMode = BarTickModeEnum.Bar;
            _tradeContext = tx;
            SimulateTerminal = tx.TradeTerminals.GetSimulateTerminal();
            LoopDelay = 1;
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

            //   _tradeContext.Orders.SetBackTestMode(true);
           //_random.NewBarTickEvent += _tradeContext.Orders.NewTick;
           // _random.NewBarTickEvent += NewTick;

            _doWorkDelegate = new DoWorkDelegate(DoWork);

            //if(TestMode ==TestModeEnum.Real)
            //    CreateSeries();

            // Init in DoWork
            //if (TestMode == TestModeEnum.Real)
            //{
            //    var ticker = _tradeContext.Tickers.GetTickers.ToList().FirstOrDefault();
            //    if (ticker != null)
            //    {
            //        _tickerSlot = new TickerSlot { Ticker = ticker };
            //    }
            //}

        //   
        }
        /*
        private void NewTick(DateTime dt, string ticker, double price)
        {
            if (BarTickMode > 1) _tradeContext.Orders.NewTick(dt, ticker, price);
        }
         */
        
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
                //var ticker = _tradeContext.Tickers.GetTickers.ToList().FirstOrDefault();
                //if (ticker != null)
                //{
                //    _tickerSlot = new TickerSlot{Ticker = ticker, DateTimeTo = DateTime.Now, Days = 100};
                //    _tickerSlot.Init();
                //}

                var tickers = _tradeContext.Tickers.GetTickers.ToList();
                foreach (var ti in tickers)
                {
                    _tickerSlot = new TickerSlot { Ticker = ti, DateTimeTo = DateTime.Now, Days = 100 };
                    _tickerSlot.Init(_tradeContext.EventLog);
                    _tickerSlot.Strategies.Clear();
                    foreach (Strategy s in _strategies.Where(s => s.Ticker.Key == ti.Key).ToList())
                        _tickerSlot.Strategies.Add(s); 
                    _ticerSlots.Add(_tickerSlot);
                }
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

                    //b = _random.GetNextBar(15);
                    //barDt = b.DT.AddSeconds(-1);

                    //_random.GetBidAsk((float)b.Open, out bid, out ask);
                    //_tradeContext.Orders.ExecuteTick(barDt, _random.Key, b.Open, bid, ask);
                    //qStr = String.Format("{0};{1};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                    //                        _random.Key, barDt.Date, barDt, b.Open, bid, ask, b.Volume / 4);
                    //_tradeContext.Tickers.PutDdeQuote3(qStr);
                    //_tradeContext.Tickers.UpdateTimeSeries(barDt);

                    /* Last Correct
                    _tradeContext.Orders.NewBar(_random.Key, b); // Execute Prev Bar
                    _tradeContext.Tickers.UpdateAsyncSeries(_random.Key, b); // Add Prev Bar

                    
                    b = _random.GetNextBar(15);
                    t = _random.FirstTickInBar;

                    _tradeContext.Orders.NewTick(t.DT, t.Ticker, t.Value);
                    _tradeContext.Tickers.PutDdeQuote3(t.ToString());
                    _tradeContext.Tickers.UpdateTimeSeries(t.DT);
                    */
                    
                    /*
                    b = _random.GetNextBar(15);
                    _tradeContext.Orders.NewBar(_random.Key,b); // Execute Bar

                    // _tradeContext.Orders.NewTick(b.DT, "RIU1", (double)b.High);
                    // _tradeContext.Orders.NewTick(b.DT, "RIU1", (double)b.Low);
                    // _tradeContext.Tickers.PutDdeQuote3(t.ToString());
                    //_tradeContext.Tickers.UpdateBarSeries(_random.Key, b);
                    _tradeContext.Tickers.UpdateAsyncSeries(_random.Key, b);
                    _tradeContext.Tickers.UpdateTimeSeries(b.DT);
                    
                    // t = _random.GetNextTick();
                    //_tradeContext.Orders.NewTick(t.DT, t.Ticker, (double)t.Value);
                    //_tradeContext.Tickers.PutDdeQuote3(t.ToString());
                    //_tradeContext.Tickers.UpdateTimeSeries(t.DT);
                      */

                    //    _tradeContext.Orders.NewTick(t.DT, t.Ticker, (double)t.Value);
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
                    //var bar = _tickerSlot.GetNextBar();
                    foreach (var ts in _ticerSlots)
                    {
                        var bar = ts.GetNextBar();
                        if (bar != null)
                        {
                            _flagNewDay = (ts.LastBar != null && bar.DT.DateToInt() > ts.LastBar.DT.DateToInt())
                                          || ts.LastBar == null;

                            ts.LastBar = bar;
                            // Start Execution
                            switch (ExecutionBarMode)
                            {
                                case ExecutionBarModeEnum.TypicalPrice:
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.TypicalPrice,
                                        bar.TypicalPrice);
                                    break;
                                case ExecutionBarModeEnum.MedianPrice:
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.MedianPrice,
                                        bar.MedianPrice);
                                    break;
                                case ExecutionBarModeEnum.OC:
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open);
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close,
                                        bar.Close);
                                    break;
                                case ExecutionBarModeEnum.Compr:
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open);
                                    if (bar.IsWhite)
                                    {
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low,
                                            bar.Low);
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High,
                                            bar.High);
                                    }
                                    else if (bar.IsBlack)
                                    {
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High,
                                            bar.High);
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low,
                                            bar.Low);
                                    }
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close,
                                        bar.Close);
                                    break;
                                case ExecutionBarModeEnum.RunTo:
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Open, bar.Open);
                                    if (bar.IsWhite)
                                    {
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High,
                                            bar.High);
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low,
                                            bar.Low);
                                    }
                                    else if (bar.IsBlack)
                                    {
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Low,
                                            bar.Low);
                                        _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.High,
                                            bar.High);
                                    }
                                    _tradeContext.ExecuteTick(bar.DT, ts.Ticker.Key, bar.MedianPrice, bar.Close,
                                        bar.Close);
                                    break;
                            }

                            // _tradeContext.Tickers.UpdateAsyncSeries(ts.Ticker.Key, bar);
                            ts.Ticker.UpdateAsyncSeries(bar);
                            //_tradeContext.Tickers.UpdateTimeSeries(bar.DT);
                            ts.Ticker.UpdateTimeSeries(bar.DT);
                        }
                        // Nothing Bars else in this TickerSlot
                        else
                        {
                            // Nothing Bars else in this TickerSlot
                            ts.EndOfBarSeries = true;
                            if (_ticerSlots.All(ts1 => ts1.EndOfBarSeries))
                                _doWorkStatus = false;
                            else
                                continue;
                            //_doWorkStatus = false;
                        }
                        //if (_ticerSlots.All(ts1 => ts1.EndOfBarSeries))
                        //    _doWorkStatus = false;
                        
                        // Strategies.MainLoop
                        int ii = 0, jj = 0;
                        foreach (var s in ts.Strategies)
                        {
                            if (s.Bars != null && s.Bars.Count < 150)
                                continue;

                            if (ts.LastBar == null)
                                continue;

                            //if(ts.EndOfBarSeries)
                            //    continue;
                            jj++;
                            if (TradesNumber > 0)
                            {
                                if (AllHaveSameTradesNumber)
                                {
                                    if (s.PositionTotal.Quantity < TradesNumber)
                                    {
                                        s.MainBase();
                                        ii++;
                                    }
                                    else if (s.Position.IsOpened)
                                        s.Finish();
                                }
                                else
                                {
                                    if (TestMode == TestModeEnum.Real && ts.EndOfSession && s.Position.IsOpened)
                                    {
                                        s.Finish();

                                        _tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.High, ts.LastBar.High);
                                        _tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.Low, ts.LastBar.Low);

                                        ii++;
                                    }
                                    else
                                    {
                                        //if (TestMode == TestModeEnum.Real && _flagNewDay)
                                        //{
                                        //    s.StartNewDayInit();
                                        //}
                                        s.EntryEnabled = true;
                                        bool toContinue =
                                            ts.Strategies.Any(sp => sp.PositionTotal.Quantity < TradesNumber);
                                        if (toContinue)
                                        {
                                            s.MainBase();
                                            ii++;
                                        }
                                        // Finish when Max Trades NUmber is Reached
                                        else if (s.Position.IsOpened)
                                        {
                                            s.Finish();
                                            _tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.High, ts.LastBar.High);
                                            _tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.Low, ts.LastBar.Low);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                s.MainBase();
                                ii++;
                            }
                        }
                    }
                    //if (_ticerSlots.All(ts => ts.EndOfBarSeries))
                    //    _doWorkStatus = false;

                    if (NextPass != null) 
                        NextPass();
                    }
                    catch (Exception e)
                    {
                        //var exc = e;
                        throw new NullReferenceException(e.Message);
                    }
                }
                else
                {
                    int i = 0, j = 0;
                    foreach (Strategy s in _strategies)
                    {
                        if (s.Bars != null && s.Bars.Count < 300)
                            continue;
                        j++;
                        if (TradesNumber > 0)
                        {
                            if (AllHaveSameTradesNumber)
                            {
                                if (s.PositionTotal.Quantity < TradesNumber)
                                {
                                    s.MainBase();
                                    i++;
                                }
                                else if (s.Position.IsOpened)
                                    s.Finish();
                            }
                            else
                            {
                                if (TestMode == TestModeEnum.Real && _lastBar.DT.TimeToInt() > 234500)
                                {
                                    s.Finish();
                                    i++;
                                }
                                else
                                {
                                    if (TestMode == TestModeEnum.Real && _flagNewDay)
                                    {
                                        s.StartNewDayInit();
                                    }
                                    s.EntryEnabled = true;
                                    bool toContinue = _strategies.Any(sp => sp.PositionTotal.Quantity < TradesNumber);
                                    if (toContinue)
                                    {
                                        s.MainBase();
                                        i++;
                                    }
                                    else if (s.Position.IsOpened)
                                        s.Finish();
                                }
                            }
                        }
                        else
                        {
                            s.MainBase();
                            i++;
                        }
                    }
                    if (j > 0 && i == 0)
                        _doWorkStatus = false;

                    // 15.09.24 Update from ChartUpdates in NextPass
                    //if( RefreshUI )
                    //    UpdatePositionFromticker();

                    if (LoopDelay >= 0)
                        Thread.Sleep(LoopDelay);

                    // Chart.Update(null, null);
                    // Thread.Sleep(2000);
                    if (NextPass != null) NextPass();
                    // SomeClear(t.DT);

                    //Delay(1000000);

                    //Thread.Sleep(100);

                    // if(_pause) Thread.Sleep(1000);
                }
            }

            OnMessageEvent("Preapare to Finish. Try to Close All Startegies");

            // Exit from Main Loop and go to Finish
           // if (!idosomething) return;

            if (TestMode == TestModeEnum.Real)
            {
                foreach (var ts in _ticerSlots)
                {
                    if (ts.LastBar == null)
                        continue;
                    //var ssOpened = (from ts.S s in _strategies where s.Position.IsOpened select s).ToList();
                    var so = ts.Strategies.Where(s => s.Position.IsOpened).ToList();
                    while (so.Any())
                    {
                        //_tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.Low, ts.LastBar.Low);
                        //_tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.High, ts.LastBar.High);
                        foreach (var s in so)
                        {
                            s.Finish();
                            _tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.Low, ts.LastBar.Low); 
                            _tradeContext.ExecuteTick(ts.LastBar.DT, ts.Ticker.Key, ts.LastBar.MedianPrice, ts.LastBar.High, ts.LastBar.High);
                        }

                        so = ts.Strategies.Where(s => s.Position.IsOpened).ToList();
                    }
                }
            }
            else
            {
                try
                {
                    foreach (Strategy s in _strategies)
                    {
                        s.Finish();
                    }
                    t = _random.GetNextTick2();
                    //_tradeContext.Orders.NewTick(t.DT, t.Ticker, (double)t.Value);
                    _tradeContext.ExecuteTick(t.DT, t.Ticker, t.Value, t.Bid, t.Ask);
                    _tradeContext.Tickers.PutDdeQuote3(t.ToString());

                    var ssOpened = (from Strategy s in _strategies where s.Position.IsOpened select s).ToList();
                    //while ((from Strategy s in _strategies where s.Position.IsOpened select s).Any())
                    while (ssOpened.Any())
                    {
                        //foreach (var s in
                        //    from Strategy s in _strategies where s.Position.IsOpened select s)
                        foreach (var s in ssOpened)
                        {
                            s.Finish();
                        }

                        t = _random.GetNextTick2();
                        //_tradeContext.Orders.NewTick(t.DT, t.Ticker, (double)t.Value);
                        _tradeContext.ExecuteTick(t.DT, t.Ticker, t.Value, t.Bid, t.Ask);
                        _tradeContext.Tickers.PutDdeQuote3(t.ToString());

                        Thread.Sleep(TimeSpan.FromSeconds(3));
                        ssOpened = (from Strategy s in _strategies where s.Position.IsOpened select s).ToList();
                    }

                    _tradeContext.Evlm(EvlResult.INFO, EvlSubject.TECHNOLOGY, "BackTest", "BackTest",
                        "MainLoop Finish", "", "");

                }
                catch (Exception e)
                {
                    throw new NullReferenceException("Something is wrong");
                }
            }
            OnFinishEvent();
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
