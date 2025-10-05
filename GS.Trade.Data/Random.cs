using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using GS.Extension;
using GS.Trade.Data.Bars;

namespace GS.Trade.Data
{
    public class BarRandom
    {
        public delegate void NewTickStrEventHandler(string quoteString);
        public event NewTickStrEventHandler NewTickStrEvent;

        public delegate void NewTickEventHandler(DateTime dt, string tickerkey,  double q, double bid, double ask);
        public event NewTickEventHandler NewTickEvent;

        public delegate void NewSecondEventHandler(DateTime dt);
        public event NewSecondEventHandler NewSecondEvent;

        public delegate void NewBarTickEventHandler(DateTime dt, string ticker, double price);
        public event NewBarTickEventHandler NewBarTickEvent; 

       // public  delegate void FirstTickInBArDelegate;

        public string Name { get; set; }
        public string Code { get; set; }
        public string TradeBoard { get; set; }

        public string Key {
            get
            {
                return TradeBoard.HasValue()
                    ? TradeBoard + "@" + Code
                    : Code;
            }
        }

        public int Min { get; set; }
        public int Max { get; set; }
        public float Value  { get; set;}

        public float Bid { get; set; }
        public float Ask { get; set; }

        public int Volume { get; set; }

        public float BidAskPrcnt { get; set; }

        private DateTime _date = DateTime.Today;
        private DateTime _dateTime = DateTime.Now;

        public DateTime DT
        {
            get { return _dateTime; }
            set { _dateTime = value; }
        }

        private readonly Tick _lastTick = new Tick();
        private readonly Tick _firstTickInBar = new Tick();
        public Tick FirstTickInBar
        {
            get { return _firstTickInBar; }
        }
        private readonly Bar _lastBar = new Bar();

        private  Random _random;

        private readonly Timer _timer;
       // private int Seconds;

        private float _minMove = 0.01f;
        private float _startValue;
        private const float InitValue = 100000f;

        /*
        public BarRandom( float value )
        {
            //Min = min;
            //Max = max;
            Value = value;

         //   _lastTick = new Tick();
         //   _lastTBar = new Bar();

            _random = new Random();
           // _bar = new Bar(DateTime.Now, Value, Value, Value, Value, 0);

        }
        */
        public BarRandom(float value, double miliseconds)
        {
            _startValue = value;
            Value = value;

            //_random = new Random((int)unchecked(DateTime.Now.Ticks % Int32.MaxValue));
            ResetRandom();

            _timer = new Timer(miliseconds) { AutoReset = true };
            _timer.Elapsed += GetQuoteStr;

            BidAskPrcnt = 0.00010f/2;

        }
        public Bar GetNextBar()
        {
            var v = _random.Next(0,100) / 100f;
            var s = _random.Next(1, 11);
            
            var sign = (s > 5) ? +1 : -1;
            Value += sign*v;

            _date = _date.AddDays(1);
            var bar = new Bar(_date, Value, Value, Value, Value, v*100);

            for(var i=0; i<10;i++)
            {
                v = _random.Next(0,100) / 100f;
                s = _random.Next(1, 11);

                sign = (s > 5) ? +1 : -1;

                var vv = Value + sign * v;

                if (vv > bar.High) bar.High = vv;
                if (vv < bar.Low) bar.Low = vv;
                bar.Close = vv;

                bar.Volume += v * 100 + s;
            }

            Value = (float)bar.Close;
            return bar;
        }
        public Bar GetNextBar(int timeIntSeconds)
        {
            var v = _random.Next(0, 100) / 1000000f;

            v = Value*v;
            //v = Value*v * 100000f / _startValue;
            
            v = _minMove * (int)(v / _minMove);

            var s = _random.Next(1, 11);

            var sign = (s > 5) ? +1 : -1;
            Value += sign * v;

             var date = _dateTime.Date;

            _dateTime = _dateTime.AddMilliseconds(-_dateTime.Millisecond);

            _dateTime = _dateTime.AddSeconds(1);

            _lastBar.LastDT = _dateTime;

            _lastBar.DT = _dateTime;
            _lastBar.Open = Value;
            _lastBar.High = Value;
            _lastBar.Low = Value;
            _lastBar.Close = Value;
            _lastBar.Volume = Value*100 + s;
            _lastBar.LastDT = _dateTime;

            _firstTickInBar.DT = _dateTime;
            _firstTickInBar.TradeBoard = TradeBoard;
            _firstTickInBar.Ticker = Code;
            _firstTickInBar.Value = Value;
            _firstTickInBar.Volume = (long)_lastBar.Volume;

            float bid;
            float ask;
            GetBidAsk(Value, out bid, out ask);
            Bid = bid;
            Ask = ask;
            _firstTickInBar.Bid = Bid;
            _firstTickInBar.Ask = Ask;

            if (NewBarTickEvent != null) NewBarTickEvent(_dateTime, "RIU1", (double)Value);

            // var bar = new Bar(_dateTime, Value, Value, Value, Value, v * 100);

            for (var i = 0; i < timeIntSeconds-1; i++)
            {
                v = _random.Next(0, 100) / 1000000f;
                v = Value * v;
                v = _minMove * (int)(v / _minMove);

                s = _random.Next(1, 11);
                sign = (s > 5) ? +1 : -1;
                Value += sign * v;

                if (Value > (float)_lastBar.High) _lastBar.High = Value;
                if (Value < (float)_lastBar.Low) _lastBar.Low = Value;
                _lastBar.Close = Value;
                _lastBar.Volume += v * 100 + s;

                _dateTime = _dateTime.AddSeconds(1);
                _lastBar.LastDT = _dateTime;

                if (NewBarTickEvent != null) NewBarTickEvent(_dateTime, Code, (double)Value);
            }

            //_dateTime = _dateTime.AddSeconds(timeIntSeconds);
            _dateTime = _dateTime.AddMilliseconds(-_dateTime.Millisecond);
          //  _lastBar.LastDT = _dateTime;
            _lastBar.DT = _dateTime;

            if( date.Date.CompareTo(_dateTime.Date) != 0) ResetRandom();

            return _lastBar;
        }
        public Bar GetNextBar2(int timeIntSeconds)
        {
            GetNextValue2();

            var date = _dateTime.Date;

            _dateTime = _dateTime.AddMilliseconds(-_dateTime.Millisecond);

            _dateTime = _dateTime.AddSeconds(1);

            _lastBar.LastDT = _dateTime;

            _lastBar.DT = _dateTime;
            _lastBar.Open = Value;
            _lastBar.High = Value;
            _lastBar.Low = Value;
            _lastBar.Close = Value;
            _lastBar.Volume = Volume;
            _lastBar.LastDT = _dateTime;

            _firstTickInBar.DT = _dateTime;
            _firstTickInBar.TradeBoard = TradeBoard;
            _firstTickInBar.Ticker = Code;
            _firstTickInBar.Value = Value;
            _firstTickInBar.Volume = (long)_lastBar.Volume;

            float bid;
            float ask;
            GetBidAsk2(Value, out bid, out ask);
            Bid = bid;
            Ask = ask;
            _firstTickInBar.Bid = Bid;
            _firstTickInBar.Ask = Ask;

            if (NewBarTickEvent != null) NewBarTickEvent(_dateTime, "RIU1", (double)Value);

            // var bar = new Bar(_dateTime, Value, Value, Value, Value, v * 100);
            
            for (var i = 0; i < timeIntSeconds - 1; i++)
            {
                GetNextValue2();

                if (Value > (float)_lastBar.High) _lastBar.High = Value;
                if (Value < (float)_lastBar.Low) _lastBar.Low = Value;
                _lastBar.Close = Value;
                _lastBar.Volume += Volume;
                //_lastBar.Volume += v * 100 + s;

                _dateTime = _dateTime.AddSeconds(1);
                _lastBar.LastDT = _dateTime;

                if (NewBarTickEvent != null) NewBarTickEvent(_dateTime, Code, (double)Value);
            }

            //_dateTime = _dateTime.AddSeconds(timeIntSeconds);
            _dateTime = _dateTime.AddMilliseconds(-_dateTime.Millisecond);
            //  _lastBar.LastDT = _dateTime;
            _lastBar.DT = _dateTime;

            if (date.Date.CompareTo(_dateTime.Date) != 0) ResetRandom();

            return _lastBar;
        }
        private void GetNextValue2()
        {
            //var v = (float)(_random.Next(0, 100) / 100.0);

            // var v = _random.Next(0, 100) / 100f;
            var v = _random.Next(0, 100) / 1000000f; // 0.01%
            v = InitValue * v;
            v = _minMove * (int)(v / _minMove);

            var s = _random.Next(1, 11);
            var sign = (s > 5) ? +1 : -1;

            Value += sign * v;

            float ask;
            float bid;

            GetBidAsk2(Value, out bid, out ask);

            Bid = bid;
            Ask = ask;

            Volume = _random.Next(1, 101);
        }
        private void GetNextValue()
        {
            //var v = (float)(_random.Next(0, 100) / 100.0);

            // var v = _random.Next(0, 100) / 100f;
            var v = _random.Next(0, 100) / 1000000f; // 0.01%
            v = Value * v;
            v = _minMove * (int)(v / _minMove);

            var s = _random.Next(1, 11);
            var sign = (s > 5) ? +1 : -1;

            Value += sign * v;

            float ask;
            float bid;

            GetBidAsk(Value, out bid, out ask);

            Bid = bid;
            Ask = ask;

            Volume = _random.Next(1, 101);
        }
        public void GetBidAsk(float value, out float bid, out float ask)
        {
            var ba = value - value * BidAskPrcnt;
            bid = _minMove * (int)(ba / _minMove);
            ba = value + value * BidAskPrcnt;
            ask = _minMove * (int)(ba / _minMove);
        }
        public void GetBidAsk2(float value, out float bid, out float ask)
        {
            var ba = value - InitValue * BidAskPrcnt;
            bid = _minMove * (int)(ba / _minMove);
            ba = value + InitValue * BidAskPrcnt;
            ask = _minMove * (int)(ba / _minMove);
        }

        public void GetQuoteStr(object o, ElapsedEventArgs e)
        {
            GetNextValue();

            var s = String.Format("{0};{1};{2};{3:N2};{4:N2};{5:N2};{6}",
                "RIU1", DateTime.Now.Date, DateTime.Now.TimeOfDay, Value, Bid, Ask, Volume);

            if( NewTickStrEvent != null) NewTickStrEvent(s);
            if (NewTickEvent != null) NewTickEvent(DateTime.Now, Code, Value, Bid, Ask);

            NewSyncSecond();
        }
        public Tick GetNextTick()
        {
            var dt = _dateTime.Date;

            GetNextValue();

            _dateTime = _dateTime.AddSeconds(1);
            if (dt.Date != _dateTime.Date) ResetRandom();
            
            _lastTick.DT = _dateTime;
            _lastTick.TradeBoard = TradeBoard;
            _lastTick.Ticker = Code;
            _lastTick.Value = Value;
            _lastTick.Bid = Bid;
            _lastTick.Ask = Ask; 
            _lastTick.Volume = Volume;

            return _lastTick;
        }
        public Tick GetNextTick2()
        {
            var dt = _dateTime.Date;

            GetNextValue2();

            _dateTime = _dateTime.AddSeconds(1);
            if (dt.Date != _dateTime.Date) ResetRandom();

            _lastTick.DT = _dateTime;
            _lastTick.TradeBoard = TradeBoard;
            _lastTick.Ticker = Code;
            _lastTick.Value = Value;
            _lastTick.Bid = Bid;
            _lastTick.Ask = Ask;
            _lastTick.Volume = Volume;

            return _lastTick;
        }

        public void NewSyncSecond()
        {
            if (NewSecondEvent == null) return;

            var dt = DateTime.Now;
            dt = dt.AddMilliseconds(-dt.Millisecond);
            NewSecondEvent(dt);
        }

        public void Start()
        {
            _dateTime = DateTime.Now;
            _timer.Start();
        }
        public void Stop()
        {
            _timer.Stop();
            _timer.Elapsed -= GetQuoteStr;
            _timer.Close();
            _timer.Dispose();
        }
        private void ResetRandom()
        {
            var r = new Random((int)unchecked(DateTime.Now.Ticks % Int32.MaxValue));

            var v = r.Next(0, Int32.MaxValue);
            v = r.Next(0, Int32.MaxValue);
            v = r.Next(0, Int32.MaxValue);

            _random = new Random( v );
        }
        public void AddSeconds(int seconds)
        {
            var dt = _dateTime.Date;
            _dateTime = _dateTime.AddSeconds(seconds);
            if (dt.Date != _dateTime.Date) ResetRandom();
        }

        public class Tick
        {
            public DateTime DT { get; set; }
            public string TradeBoard { get; set; }
            public string Ticker { get; set; }
            public float Value { get; set; }
            public float Bid { get; set; }
            public float Ask { get; set; }
            public long Volume { get; set; }

            public Tick()
            {
            }
            public override string ToString()
            {
                return String.Format("{7};{0};{1:d};{2:H:mm:ss.fff};{3:N2};{4:N2};{5:N2};{6}",
                Ticker, DT.Date, DT, Value, Bid, Ask, Volume, TradeBoard);
            }
        }

    }
}
