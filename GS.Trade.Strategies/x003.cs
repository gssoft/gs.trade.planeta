using System;
using System.Xml.Serialization;
using GS.Interfaces;
using GS.Trade.Data.Studies.GS.xMa018;
using GS.Trade.Interfaces;
using GS.Trade.Data;
using GS.Trade.Data.Studies.GS;
using GS.Trade.Trades;

namespace GS.Trade.Strategies
{
   
    //public class X003 : Strategy
    //{
    //    [XmlIgnore]
    //    public override IBars Bars { get; protected set; }
    //    [XmlIgnore]
    //    public override int MaxBarsBack { get; protected set; }

    //    private Xma018 _xma018;
    ////    private BarSeries _bars;

    //    public int MaLength { get; set; }
    //    public int MaAtrLength { get; set; }
    //    public float MaAtrK { get; set; }
    //    public int MaMode { get; set; }

    //    public float KAtrStop { get; set; }
    //    public int SwingCount { get; set; }

    //    public int EntryMode { get; set; }

    //    //  public int EntryMode { get; set; }

    //    private DateTime _lastDT = DateTime.MinValue;

    //    private double _currentLimitPrice;
    //    private double _lastLimitBuyPrice;
    //    private double _lastLimitSellPrice;

    //    private ulong _lastLimitLongTransID;
    //    private ulong _lastLimitShortTransID;

    //    private double _currentLimitExitPrice;
    //    private double _lastLimitExitPrice;

    //    private long _lastLimitLongExitTransID;
    //    private long _lastLimitShortExitTransID;

    //    //private float _kAtrStop = 1.5f;
    //    private int _swingCount;
    //    private double _xBand;


    //    public X003()
    //    {
    //        // need for Serialization

    //    }
    //    public X003(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt)
    //        : base(tx, name, code, ticker, timeInt)
    //    {
    //    }
    //    public override void Init()
    //    {
    //        base.Init(); if (IsWrong) return;

    //        Position.PositionChangedEvent += PositionIsChangedEventHandler;

    //        _xma018 = (Xma018)TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt, MaLength, MaAtrLength, MaAtrK, MaMode));
    //        _xma018.Init();

    //        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategy", "Strategy", "Init " + Code, ToString(), "");
    //        return;
    //    }

    //    private void PositionIsChangedEventHandler(long oldposition, long newposition)
    //    {
    //        if (oldposition < 0)
    //            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
    //                                                OrderTypeEnum.StopLimit, OrderOperationEnum.Buy);
    //        else if (oldposition > 0)
    //            TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
    //                                                OrderTypeEnum.StopLimit, OrderOperationEnum.Sell);

    //        TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, "X001." + Ticker.Code, "PositionChanged",
    //                                     "New: " + newposition + "; Old: " + oldposition, "");

    //        if (Math.Sign(oldposition) != Math.Sign(newposition) || (oldposition == 0 && newposition != 0) )
    //            _swingCount = 0;

    //        SetStopOrderFilledStatus(false);
    //    }

    //    public override void Main()
    //    {
    //        if (_xma018.Count < 2) return;
    //        if (_xma018.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;

    //        //TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.PROGRAMMING, Name, "Main",
    //        //                                    _xma018.LastItemCompleted.ToString(), _xma018.ToString());

    //        _lastDT = _xma018.LastItemCompletedDT;

    //        if (_xma018.TrendChanged)
    //        {
    //            _lastLimitBuyPrice = 0.0;
    //            _lastLimitSellPrice = 0.0;

    //            _swingCount++;
    //        }


    //        var contract = Position.Pos == 0 ? 1 : 2;

    //        // *****************************  Long Entry ***********************

    //        if (_xma018.Trend > 0 && Position.Pos <= 0)
    //        //( (Position.Pos < 0 && _swingCount >= SwingCount) || (Position.Pos == 0) ) )
    //        {
    //            string comment;

    //            if (ExitInEmergencyWhenStopUnFilled()) return;

    //            _xBand = ((Xma018.Item)(_xma018.LastItemCompleted)).High - ((Xma018.Item)(_xma018.LastItemCompleted)).Low;

    //            if (EntryMode == 2 && Position.Pos < 0 && _swingCount >= SwingCount)
    //            {
    //                _currentLimitExitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
    //                _currentLimitExitPrice = Ticker.ToMinMove(_currentLimitExitPrice, -1);

    //                if (!_lastLimitExitPrice.Equals(_currentLimitExitPrice))
    //                {
    //                    _lastLimitExitPrice = _currentLimitExitPrice;
    //                    TradeTerminal.KillLimitOrderByTransID(Ticker.ClassCode, Ticker.Code, _lastLimitShortExitTransID);

    //                    _lastLimitShortExitTransID =
    //                        TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
    //                                            OrderOperationEnum.Buy,
    //                                            _currentLimitExitPrice, Position.Quantity, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
    //                                           "Exit from Short");
    //                }
    //            }

    //            // if (!_xma018.Bars.IsWhite &&
    //            //     _xma018.Bars.LastItemCompleted.Close.CompareTo(((Xma018.Item) (_xma018.LastItemCompleted)).Ma) < 0)
    //            /*
    //            if ( Ticker.Bid.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0 ||
    //                _xma018.Bars.LastItemCompleted.Close.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0 )
    //             */ 
    //            {
    //                //limitprice = _xma018.Bars.LastItemCompleted.Close;

    //                // _currentLimitPrice = Math.Min(Ticker.Bid, _xma018.Bars.LastItemCompleted.Close);

    //                _currentLimitPrice = ((Xma018.Item) (_xma018.LastItemCompleted)).Low;
    //                comment = "Buy Band.Low";

    //                 _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, +1);

    //                if (_lastLimitBuyPrice.Equals(_currentLimitPrice)) return;

    //                _lastLimitBuyPrice = _currentLimitPrice;

    //                TradeTerminal.KillLimitOrder(null, this, Ticker.ClassCode, Ticker.Code, _lastLimitLongTransID);

    //                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
    //                                            OrderTypeEnum.StopLimit, OrderOperationEnum.Sell);
    //                //_lastLimitLongTransID =
    //                //    TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
    //                //                            OrderOperationEnum.Buy, _currentLimitPrice, Contracts,
    //                //                            DateTime.Today.Add(new TimeSpan(23, 49, 15)), comment);

    //                var stopprice = _currentLimitPrice - KAtrStop * _xBand;

    //                stopprice = Ticker.ToMinMove(stopprice, -1);

    //                TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
    //                                                OrderOperationEnum.Sell,
    //                                                stopprice, stopprice, Contracts,
    //                                                DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");

    //                TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code, Ticker.Code,
    //                                              "New Buy Signal. Price: " + _currentLimitPrice, "");

    //            }
    //        }
    //        // ******************************** Short Entry *****************
    //        else if (_xma018.Trend < 0 && Position.Pos >= 0)
    //        // (  (Position.Pos > 0 && _swingCount >= SwingCount) || (Position.Pos == 0) ))
    //        {
    //            string comment;

    //            if (ExitInEmergencyWhenStopUnFilled()) return;

    //            _xBand = ((Xma018.Item)(_xma018.LastItemCompleted)).High - ((Xma018.Item)(_xma018.LastItemCompleted)).Low;

    //            if (EntryMode == 2 && Position.Pos > 0 && _swingCount >= SwingCount)
    //            {
    //                _currentLimitExitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).Ma;
    //                _currentLimitExitPrice = Ticker.ToMinMove(_currentLimitExitPrice, -1);

    //                if (!_lastLimitExitPrice.Equals(_currentLimitExitPrice))
    //                {
    //                    _lastLimitExitPrice = _currentLimitExitPrice;
    //                    TradeTerminal.KillLimitOrderByTransID(Ticker.ClassCode, Ticker.Code, _lastLimitLongExitTransID);

    //                    _lastLimitLongExitTransID =
    //                        TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
    //                                            OrderOperationEnum.Sell,
    //                                            _currentLimitExitPrice, Position.Quantity, DateTime.Today.Add(new TimeSpan(23, 49, 15)),
    //                                            "Exit from Long");
    //                }
    //            }

    //            //if (!_xma018.Bars.IsBlack &&
    //            //      _xma018.Bars.LastItemCompleted.Close.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0)
    //            /*
    //            if (Ticker.Ask.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0 ||
    //                _xma018.Bars.LastItemCompleted.Close.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0 )
    //             */ 
    //            {
    //                //limitprice = _xma018.Bars.LastItemCompleted.Close;

    //               // _currentLimitPrice = Math.Max(Ticker.Ask, _xma018.Bars.LastItemCompleted.Close);

    //                comment = "Sell Band.High";
    //                _currentLimitPrice = ((Xma018.Item)(_xma018.LastItemCompleted)).High;
                    
    //                _currentLimitPrice = Ticker.ToMinMove(_currentLimitPrice, -1);

    //                if (_lastLimitSellPrice.Equals(_currentLimitPrice)) return;

    //                _lastLimitSellPrice = _currentLimitPrice;

    //                TradeTerminal.KillLimitOrder(null, this, Ticker.ClassCode, Ticker.Code, _lastLimitShortTransID);

    //                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
    //                                            OrderTypeEnum.StopLimit, OrderOperationEnum.Buy);
    //                //_lastLimitShortTransID =
    //                //TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
    //                //                            OrderOperationEnum.Sell,
    //                //                            _currentLimitPrice, Contracts,
    //                //                            DateTime.Today.Add(new TimeSpan(23, 49, 15)),
    //                //                            comment);

    //                var stopprice = _currentLimitPrice + KAtrStop * _xBand;

    //                stopprice = Ticker.ToMinMove(stopprice, +1);

    //                TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
    //                                                OrderOperationEnum.Buy,
    //                                                stopprice, stopprice, Contracts,
    //                                                DateTime.Today.Add(new TimeSpan(23, 49, 15)), "");

    //                TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, Code, Ticker.Code,
    //                                              "New Sell Signal. Price: " + _currentLimitPrice, "");
    //            }
    //        }
    //    }

    //    public override void Finish()
    //    {
    //        TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Finish",
    //                                     ToString(), "");
    //    }

    //    protected override void PositionIsChangedEventHandler2(IPosition2 oldpos, IPosition2 newpos, PositionChangedEnum changedResult)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public override string Key
    //    {
    //        get
    //        {
    //            return String.Format("Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}",
    //                  GetType(), Name, Code, TradeAccountKey, TickerKey, TimeInt);
    //        }
    //    }

    //    public override string ToString()
    //    {
    //        return String.Format("Name={0};Code={1};Account={2};Ticker={3};TimeInt={4}",
    //                    Name, Code, TradeAccountKey, TickerKey, TimeInt);
    //    }

    //}

}
