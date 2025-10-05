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
    
    public class X002 : Strategy
    {
        [XmlIgnore]
        public override IBars Bars { get; protected set; }
        [XmlIgnore]
        public override int MaxBarsBack { get; protected set; }

        private Xma018 _xma018;
    //    private BarSeries _bars;

        public int Ma1Length { get; set; }
        public int Atr1Length { get; set; }
        public float KAtr1 { get; set; }
        public int Mode { get; set; }

        public float KAtrStop { get; set; }
        public int SwingCount { get; set; }
      //  public int EntryMode { get; set; }

        private DateTime _lastDT = DateTime.MinValue;

        private int _trend;
        private bool _trendWasChanged;

        private long _tradeNumber;
        private int _position;

        private double _lastLimitBuyPrice;
        private double _lastLimitSellPrice;
        private double _lastStopPrice;

        // private float _kAtrStop = 1.5f;
        private int _swingCount;
        private double _xBand;

        public X002()
        {
            // need for Serialization

        }
        public X002(ITradeContext tx, string name, string code, Ticker ticker, uint timeInt) : base(tx, name, code, ticker, timeInt)
        {
        }
        public override void Init()
        {
            base.Init(); if (IsWrong) return;
            Position.PositionChangedEvent += PositionIsChangedEventHandler;
            
            _xma018 = (Xma018) TradeContext.RegisterTimeSeries(new Xma018("Xma18", Ticker, (int)TimeInt, Ma1Length, Atr1Length, KAtr1, Mode));
            _xma018.Init();

            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, "Strategy", "Strategy", "Init " + Code, ToString(), "");

        }

        private void PositionIsChangedEventHandler(long oldposition, long newposition)
        {
            if( newposition > 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.StopLimit, OrderOperationEnum.Buy);
            else if ( newposition < 0)
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.StopLimit, OrderOperationEnum.Sell);

            TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Code, "X002." + Ticker.Code, "PositionChanged",
                                         "New: "+newposition +"; Old: "+ oldposition, "");

            if( Math.Sign(oldposition) != Math.Sign(newposition) ||
                ( oldposition == 0 && newposition != 0)
                )
                _swingCount = 0;
                SetStopOrderFilledStatus(false);
        }

        public override void Main()
        {
            if( _xma018.Count < 2 ) return;
            if (_xma018.LastItemCompletedDT.CompareTo(_lastDT) <= 0) return;

            //TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.PROGRAMMING, Name, "Main",
            //                                    _xma018.LastItemCompleted.ToString(), _xma018.ToString());

            _lastDT = _xma018.LastItemCompletedDT;

            if (ExitInEmergencyWhenStopUnFilled()) return;

            if (_trend != _xma018.Trend )
            {
                _trend = _xma018.Trend;
                _trendWasChanged = true;
            }
            if( _xma018.TrendChanged )
            {
                _lastLimitBuyPrice = 0.0;
                _lastLimitSellPrice = 0.0;

                _swingCount++;
            }

            //   if (!_trendWasChanged ) return;

            //var contract = _position == 0 ? 1 : 2;
            var contract = Position.Pos == 0 ? 1 : 2;

            // *****************************  Long Entry ***********************

            if (  _xma018.Trend > 0 &&
               ( (Position.Pos < 0 && _swingCount >= SwingCount) || 
                 (Position.Pos == 0) )
               )
            {
                double limitprice;
                string comment;

                _xBand = ((Xma018.Item)(_xma018.LastItemCompleted)).High - ((Xma018.Item)(_xma018.LastItemCompleted)).Low;

                // Ma Entry
                limitprice = ((Xma018.Item)(_xma018.LastItemCompleted)).Low;
                comment = "Buy Ma";

                if ( !  _xma018.Bars.IsWhite && 
                        _xma018.Bars.LastItemCompleted.Close.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) < 0 )
                {
                    limitprice = _xma018.Bars.LastItemCompleted.Close;
                    comment = "Buy Candle";
                }                

                limitprice = Ticker.ToMinMove(limitprice, +1);

                if (_lastLimitBuyPrice.Equals(limitprice)) return;

                _lastLimitBuyPrice = limitprice;

                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.Limit, OrderOperationEnum.All);
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.StopLimit, OrderOperationEnum.Sell);
              
                TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            OrderOperationEnum.Buy, limitprice, contract,
                                                DateTime.Today.Add(new TimeSpan(23,49,15)), comment);

                var stopprice = limitprice - KAtrStop * _xBand;

                stopprice = Ticker.ToMinMove(stopprice, -1);
                
                TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            OrderOperationEnum.Sell,
                                            stopprice , stopprice, 1,
                                                DateTime.Today.Add(new TimeSpan(23, 49, 15)),"");

                TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Name, "Main", "Main",
                                         "New Buy Signal. Price: " + _xma018.Bars.LastItemCompleted.Close, "");
                    
            }

            // ******************************** Short Entry *****************
            else if (_xma018.Trend < 0  && 
                    (   
                        (Position.Pos > 0 && _swingCount >= SwingCount) || 
                        (Position.Pos == 0)
                    )
               )
            {
                double limitprice;
                string comment;

                _xBand = ((Xma018.Item)(_xma018.LastItemCompleted)).High - ((Xma018.Item)(_xma018.LastItemCompleted)).Low;

                // Ma Entry
                limitprice = ((Xma018.Item) (_xma018.LastItemCompleted)).High;
                comment = "Sell Ma";

                if( ! _xma018.Bars.IsBlack &&
                      _xma018.Bars.LastItemCompleted.Close.CompareTo(((Xma018.Item)(_xma018.LastItemCompleted)).Ma) > 0)
                {
                    limitprice = _xma018.Bars.LastItemCompleted.Close;
                    comment = "Sell Candle";
                }

                limitprice = Ticker.ToMinMove(limitprice, -1);

                if (_lastLimitSellPrice.Equals(limitprice)) return;

                _lastLimitSellPrice = limitprice;

                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.Limit, OrderOperationEnum.All);
                TradeTerminal.KillAllOrders(this, Code, TradeAccount.Code, Ticker.ClassCode, Ticker.Code,
                                                    OrderTypeEnum.StopLimit, OrderOperationEnum.Buy);

                TradeTerminal.SetLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            OrderOperationEnum.Sell,
                                            limitprice, contract, DateTime.Today.Add(new TimeSpan(23, 49, 15)), comment);

                var stopprice = limitprice + KAtrStop*_xBand;

                stopprice = Ticker.ToMinMove(stopprice, +1);

                TradeTerminal.SetStopLimitOrder(TradeAccount.Code, Code, Ticker.ClassCode, Ticker.Code,
                                            OrderOperationEnum.Buy,
                                            stopprice, stopprice, 1,
                                                DateTime.Today.Add(new TimeSpan(23, 49, 15)),"");

                TradeContext.EventLog.AddItem(EvlResult.ATTENTION, EvlSubject.TRADING, Name, "Main", "Main",
                                         "New Sell Signal. Price: " + _xma018.Bars.LastItemCompleted.Close, "");
                   // _trendWasChanged = false;   
            }
        }
        public override void Finish()
        {
            TradeContext.EventLog.AddItem(EvlResult.SUCCESS, EvlSubject.TECHNOLOGY, Name, Name, "Finish",
                                         ToString(), "");
        }

        protected override void PositionIsChangedEventHandler2(IPosition2 oldpos, IPosition2 newpos, PositionChangedEnum changedResult)
        {
            throw new NotImplementedException();
        }

        public override string Key
        {
            get { return String.Format("Type={0};Name={1};Code={2};TradeAccountKey={3};TickerKey={4};TimeInt={5}",
                        GetType(), Name, Code, TradeAccountKey, TickerKey, TimeInt); }
        }
        public override string ToString()
        {
            return String.Format("Name={0};Code={1};Account={2};Ticker={3};TimeInt={4}",
                        Name, Code, TradeAccountKey, TickerKey, TimeInt);
        }
    }
}
